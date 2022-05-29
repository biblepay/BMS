using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using static BiblePay.BMS.Common;
using static BMSCommon.Common;
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

    public static class Proposal
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

        public static string gobject_serialize_internal(int nStartTime, int nEndTime, string sName, string sAddress, string sAmount, string sURL, string sExpenseType)
        {

            // gobject prepare 0 1 EPOCH_TIME HEX
            string sType = "1"; //Proposal
            string sQ = "\"";
            string sJson = "[[" + sQ + "proposal" + sQ + ",{";
            sJson += GJE("start_epoch", nStartTime.ToString(), true, false);
            sJson += GJE("end_epoch", nEndTime.ToString(), true, false);
            sJson += GJE("name", sName, true, true);
            sJson += GJE("payment_address", sAddress, true, true);
            sJson += GJE("payment_amount", sAmount, true, false);
            sJson += GJE("type", sType, true, false);
            sJson += GJE("expensetype", sExpenseType, true, true);
            sJson += GJE("url", sURL, false, true);
            sJson += "}]]";
            // make into hex
            string Hex = HexadecimalEncoding.StringToHex(sJson);
            return Hex;
        }
        public static bool gobject_serialize(bool fTestNet, string sERC20Address, string sNickName,
        string sName, string sAddress, string sAmount, string sURL, string sExpenseType)
        {
            string sChain = fTestNet ? "test" : "main";
            try
            {
                int unixStartTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                int unixEndTime = unixStartTimestamp + (60 * 60 * 24 * 7);
                string sHex = gobject_serialize_internal(unixStartTimestamp, unixEndTime, sName, sAddress, sAmount, sURL, sExpenseType);
                string sID = Guid.NewGuid().ToString();
                string sql = "Insert Into proposal (id,ExpenseType,ERC20Address,NickName,URL,name,Address,amount,unixstarttime,"
                    + "preparetxid,added,updated,hex,chain) values ('" + sID + "',@ExpenseType,@ERC20Address,@NickName,@URL,@Name,@Address,@Amount,'" + unixStartTimestamp.ToString()
                    + "',null,now(),now(),'" + sHex + "','" + sChain + "')";
                MySqlCommand m1 = new MySqlCommand(sql);
                m1.Parameters.AddWithValue("@ExpenseType", sExpenseType);
                m1.Parameters.AddWithValue("@ERC20Address", sERC20Address);
                m1.Parameters.AddWithValue("@NickName", sNickName);
                m1.Parameters.AddWithValue("@URL", sURL);
                m1.Parameters.AddWithValue("@Name", sName);
                m1.Parameters.AddWithValue("@Address", sAddress);
                m1.Parameters.AddWithValue("@Amount", sAmount);
                bool f1100 = BMSCommon.Database.ExecuteNonQuery(false, m1, "");
                gobject_prepare(fTestNet, sID, unixStartTimestamp, sHex);
                return true;
            }
            catch (Exception ex)
            {
                BMSCommon.Common.Log("Issue with Proposal Submit:: " + ex.Message);
                return false;
            }
        }

        public static void gobject_prepare(bool fTestNet, string sID, int StartTimeStamp, string sHex)
        {
            // gobject prepare
            string sArgs = "0 1 " + StartTimeStamp.ToString() + " " + sHex;
            string sCmd1 = "gobject prepare " + sArgs;
            object[] oParams = new object[5];
            oParams[0] = "prepare";
            oParams[1] = "0";
            oParams[2] = "1";
            oParams[3] = StartTimeStamp.ToString();
            oParams[4] = sHex;
            NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
            dynamic oOut = n.SendCommand("gobject", oParams);
            string sPrepareTXID = oOut.Result.ToString();
            string sql4 = "Update proposal Set PrepareTXID='" + sPrepareTXID + "',Updated=now() where id = '" + sID + "';";
            MySqlCommand m1 = new MySqlCommand(sql4);
            bool f12000 =             BMSCommon.Database.ExecuteNonQuery(false, m1, "");
            bool f12001 = false;

        }

        public static bool gobject_submit(bool fTestNet, string sID, int nProposalTimeStamp, string sHex, string sPrepareTXID)
        {
            try
            {
                if (sPrepareTXID == "")
                    return false;
                // Submit the gobject to the network - gobject submit parenthash revision time datahex collateraltxid
                string sArgs = "0 1 " + nProposalTimeStamp.ToString() + " " + sHex + " " + sPrepareTXID;
                string sCmd1 = "gobject submit " + sArgs;
                object[] oParams = new object[6];
                oParams[0] = "submit";
                oParams[1] = "0";
                oParams[2] = "1";
                oParams[3] = nProposalTimeStamp.ToString();
                oParams[4] = sHex;
                oParams[5] = sPrepareTXID;
                NBitcoin.RPC.RPCClient n = GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("gobject", oParams);
                string sSubmitTXID = oOut.Result.ToString();
                if (sSubmitTXID.Length > 20)
                {
                    // Update the record allowing us to know this has been submitted
                    string sql = "Update proposal set Submitted=now(),SubmitTXID='" + sSubmitTXID + "' where id = '" + sID + "'";
                    MySqlCommand m1 = new MySqlCommand(sql);
                    BMSCommon.Database.ExecuteNonQuery(fTestNet, m1, "");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void SubmitProposals(bool fTestNet)
        {
            string sChain = fTestNet ? "test" : "main";
            string sql = "Select * from proposal where CHAIN='" + sChain + "' and submitted is null;";
            MySqlCommand m1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable(m1);
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                bool fSubmitted = gobject_submit(fTestNet, dt.Rows[y]["id"].ToString(), (int)BMSCommon.Common.GetDouble(dt.Rows[y]["unixstarttime"].ToString()),
                    dt.Rows[y]["hex"].ToString(), dt.Rows[y]["preparetxid"].ToString());
                bool f11000 = false;

            }
        }

    }

}
