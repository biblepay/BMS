using BMSCommon;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BiblePay.BMS.Common;
using static BMSCommon.BitcoinSync;
using static BMSCommon.Common;
using static BMSCommon.Model;
using static BMSCommon.WebRPC;

namespace BiblePay.BMS.DSQL
{

    public class HexadecimalEncoding
    {
        public static string StringToHex(string hexstring)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char t in hexstring)
            {
                //Note: X for upper, x for lower case letters
                sb.Append(Convert.ToInt32(t).ToString("x"));
            }
            return sb.ToString();
        }

        public static string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
        }
    }
    public static class ProcessAsyncHelper
    {
        public static void StartNewThread()
        {
            // This is used by our upgrader.. We spawn a new thread when the code changes..
            string sPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            ProcessStartInfo pi = new ProcessStartInfo("dotnet", "BiblePay.BMSD.dll");
            pi.UseShellExecute = true;
            pi.WorkingDirectory = sPath;
            pi.CreateNoWindow = false;
            pi.WindowStyle = ProcessWindowStyle.Normal;
            Process procchild = Process.Start(pi);
        }

        public static bool NeedsUpgraded()
        {
            // This routine checks to see if we need to upgrade.
            string fullURL = GetUpgradeCDN() + "/BMS/GetUpgradeManifest";
            MyWebClient wc = new MyWebClient();
            string sLastPath = "";
            try
            {
                // ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                string sLocalDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string sManifest = wc.DownloadString(fullURL);
                string[] vManifest = sManifest.Split("<ROW>");
                for (int i = 0; i < vManifest.Length; i++)
                {
                    string sData = vManifest[i];
                    string[] vData = sData.Split("|");
                    if (vData.Length >= 3)
                    {
                        string sDir = vData[0];
                        string sFN = vData[1];
                        bool fDLL = (sFN.ToLower().Contains("dll"));
                        if (!sFN.Contains(".zip"))
                        {
                            string sHash = vData[2];
                            string sLocalPath = Path.Combine(sLocalDir, sFN);
                            string sLocalHash = GetShaOfFile(sLocalPath);
                            sLastPath = sLocalPath;
                            if (sLocalHash != sHash && fDLL)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log("BiblePay NeedsUpgrade::" + ex.Message + "::" + sLastPath);
                return false;
            }
        }
    }

    public static class GovernanceProposal
    { 

        private static string GJE(string sKey, string sValue, bool bIncludeDelimiter, bool bQuoteValue)
        {
            // This is a helper for the Governance gobject create method
            string sQ = "\"";
            string sOut = sQ + sKey + sQ + ":";
            if (bQuoteValue)
            {
                sOut += sQ + sValue + sQ;
            }
            else
            {
                sOut += sValue;
            }
            if (bIncludeDelimiter) sOut += ",";
            return sOut;
        }

        public static string gobject_serialize_internal(Proposal p)
        {

            // gobject prepare 0 1 EPOCH_TIME HEX
            string sType = "1"; //Proposal
            string sQ = "\"";
            string sJson = "[[" + sQ + "proposal" + sQ + ",{";
            sJson += GJE("start_epoch", p.nStartTime.ToString(), true, false);
            sJson += GJE("end_epoch", p.nEndTime.ToString(), true, false);
            sJson += GJE("name", p.Name, true, true);
            sJson += GJE("payment_address", p.BBPAddress, true, true);
            sJson += GJE("payment_amount", p.Amount.ToString(), true, false);
            sJson += GJE("type", sType, true, false);
            sJson += GJE("expensetype", p.ExpenseType, true, true);
            sJson += GJE("url", p.URL, false, true);
            sJson += "}]]";
            // make into hex
            string Hex = HexadecimalEncoding.StringToHex(sJson);
            return Hex;
        }
        public async static Task<bool> gobject_serialize(bool fTestNet, Proposal p)
        {
            try
            {
                p.nStartTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                p.nEndTime = p.nStartTime + (60 * 60 * 24 * 7);
                p.Hex = gobject_serialize_internal(p);
                //m1.Parameters.AddWithValue("@Name", sName);
                //m1.Parameters.AddWithValue("@Address", sAddress);
                //m1.Parameters.AddWithValue("@Amount", sAmount);
                string sSerial = Newtonsoft.Json.JsonConvert.SerializeObject(p, Newtonsoft.Json.Formatting.Indented);
                await StorjIO.UplinkStoreDatabaseData("proposal", p.id, sSerial, String.Empty);
                await gobject_prepare(fTestNet, p);
                return true;
            }
            catch (Exception ex)
            {
                BMSCommon.Common.Log("Issue with Proposal Submit:: " + ex.Message);
                return false;
            }
        }

        public async static Task<bool> gobject_prepare(bool fTestNet, Proposal p)
        {
            // gobject prepare
            string sArgs = "0 1 " + p.nStartTime.ToString() + " " + p.Hex;
            string sCmd1 = "gobject prepare " + sArgs;
            object[] oParams = new object[5];
            oParams[0] = "prepare";
            oParams[1] = "0";
            oParams[2] = "1";
            oParams[3] = p.nStartTime.ToString();
            oParams[4] = p.Hex;
            NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
            dynamic oOut = n.SendCommand("gobject", oParams);
            string sPrepareTXID = oOut.Result.ToString();
            p.PrepareTXID = sPrepareTXID;
            p.Updated = DateTime.Now;
            string sSerial = Newtonsoft.Json.JsonConvert.SerializeObject(p, Newtonsoft.Json.Formatting.Indented);
            await StorjIO.UplinkStoreDatabaseData("proposal", p.id, sSerial, String.Empty);
            return true;
        }

        public static async Task<bool> gobject_submit(bool fTestNet, Proposal p)
        {
            try
            {
                if (p.PrepareTXID.IsNullOrEmpty())
                    return false;
                // Submit the gobject to the network - gobject submit parenthash revision time datahex collateraltxid
                string sArgs = "0 1 " + p.nStartTime.ToString() + " " + p.Hex + " " + p.PrepareTXID;
                string sCmd1 = "gobject submit " + sArgs;
                object[] oParams = new object[6];
                oParams[0] = "submit";
                oParams[1] = "0";
                oParams[2] = "1";
                oParams[3] = p.nStartTime.ToString();
                oParams[4] = p.Hex;
                oParams[5] = p.PrepareTXID;
                NBitcoin.RPC.RPCClient n = GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("gobject", oParams);
                string sSubmitTXID = oOut.Result.ToString();
                if (sSubmitTXID.Length > 20)
                {
                    // Update the record allowing us to know this has been submitted
                    p.Submitted = DateTime.Now;
                    p.SubmitTXID = sSubmitTXID;
                    string sSerial = Newtonsoft.Json.JsonConvert.SerializeObject(p, Newtonsoft.Json.Formatting.Indented);
                    await StorjIO.UplinkStoreDatabaseData("proposal", p.id, sSerial, String.Empty);
                    //BMSCommon.Database.ExecuteNonQuery(m1);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> SubmitProposals(bool fTestNet)
        {
            string sChain = fTestNet ? "test" : "main";
            List<Proposal> dt = await StorjIO.GetDatabaseObjects<Proposal>("proposal");
            dt = dt.Where(s => s.Chain == sChain && s.SubmitTXID == null).ToList();
            for (int y = 0; y < dt.Count; y++)
            {
                bool fSubmitted = await gobject_submit(fTestNet, dt[y]);
            }
            return true;
        }

    }

}
