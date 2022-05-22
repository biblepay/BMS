using NBitcoin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static BMSCommon.Common;

namespace BMSCommon
{
    public static class WebRPC
    {
        public static bool ValidateBiblepayAddress(bool fTestNet, string sAddress)
        {
            try
            {
                object[] oParams = new object[1];
                oParams[0] = sAddress;
                NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("validateaddress", oParams);
                string sResult = oOut.Result["isvalid"].ToString();
                if (sResult.ToLower().Contains("true")) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool VerifySignature(bool fTestNet, string BBPAddress, string sMessage, string sSig)
        {
            if (BBPAddress == null || sSig == String.Empty || BBPAddress == "" || BBPAddress == null || sSig == null || BBPAddress.Length < 20)
                return false;
            try
            {
                BitcoinPubKeyAddress bpk;
                if (fTestNet)
                {
                    bpk = new BitcoinPubKeyAddress(BBPAddress, Network.TestNet);
                }
                else
                {
                    bpk = new BitcoinPubKeyAddress(BBPAddress, Network.Main);
                }

                bool b1 = bpk.VerifyMessage(sMessage, sSig, true);
                return b1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public struct SimpleUTXO
        {
            public double nAmount;
            public string TXID;
            public int nOrdinal;
            public int nHeight;
            public string Address;
            public string Ticker;
        };

        public static List<SimpleUTXO> GetBBPUTXOs2(bool fTestNet, string sAddress)
        {
            string sUTXOData = WebRPC.GetAddressUTXOs(fTestNet, sAddress);
            // Used by Portfolio Builder.
            List<SimpleUTXO> sUTXO = new List<SimpleUTXO>();
            List<NBitcoin.Crypto.UTXO> l = NBitcoin.Crypto.BBPTransaction.GetBBPUTXOs(fTestNet, sAddress, sUTXOData);
            for (int i = 0; i < l.Count; i++)
            {
                SimpleUTXO s = new SimpleUTXO();
                s.nAmount = l[i].Amount.Satoshi / 100000000;
                s.Address = sAddress;
                s.TXID = l[i].TXID.ToString();
                s.Ticker = "BBP";

                sUTXO.Add(s);
            }
            return sUTXO;
        }


        public static double GetBBPPosition(bool fTestNet, string sAddress)
        {
            List<SimpleUTXO> l = GetBBPUTXOs2(fTestNet, sAddress);
            double nTotal = 0;
            for (int i = 0; i < l.Count; i++)
            {
                nTotal += l[i].nAmount;
            }
            return nTotal;
        }


        public static string GetRawTransaction(string sTxid, bool fTestNet)
        {
            try
            {
                NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
                object[] oParams = new object[2];
                oParams[0] = sTxid;
                oParams[1] = 1;

                dynamic oOut = n.SendCommand("getrawtransaction", oParams);
                // Loop Through the Vouts and get the recip ids and the amounts
                string sOut = "";
                double locktime = oOut.Result["locktime"] == null ? 0 : GetDouble(oOut.Result["locktime"].ToString());
                double height1 = oOut.Result["height"] == null ? 0 : GetDouble(oOut.Result["height"].ToString());

                double height = 0;
                height = height1 > 0 ? height1 : locktime;


                for (int y = 0; y < oOut.Result["vout"].Count; y++)
                {
                    string sPtr = "";

                    try
                    {
                        sPtr = (oOut.Result["vout"][y] ?? "").ToString();
                    }
                    catch (Exception ey)
                    {

                        Log("Strange error in GetRawTransaction=" + ey.Message);

                    }

                    if (sPtr != "")
                    {
                        string sAmount = oOut.Result["vout"][y]["value"].ToString();
                        string sAddress = "";
                        if (oOut.Result["vout"][y]["scriptPubKey"]["addresses"] != null)
                        {
                            sAddress = oOut.Result["vout"][y]["scriptPubKey"]["addresses"][0].ToString();
                        }
                        else
                        {
                            sAddress = "?";
                        } //Happens when pool pays itself
                        sOut += sAmount + "," + sAddress + "," + height + "|";
                    }
                    else
                    {
                        break;
                    }
                }
                return sOut;
            }
            //Harvest Mission Critical todo:  Pass back the instant send lock bool here as an object!

            catch (Exception ex)
            {
                Log("GetRawTransaction1: for " + sTxid + " " + ex.Message);
                return "";
            }
        }

        public static double GetAmtFromRawTx(string sRaw, string sAddress, out int nHeight)
        {
            string[] vData = sRaw.Split(new string[] { "|" }, StringSplitOptions.None);
            for (int i = 0; i < vData.Length; i++)
            {
                string d = vData[i];
                if (d.Length > 1)
                {
                    string[] vRow = d.Split(new string[] { "," }, StringSplitOptions.None);
                    if (vRow.Length > 1)
                    {
                        string sAddr = vRow[1];
                        string sAmt = vRow[0];
                        string sHeight = vRow[2];
                        nHeight = (int)GetDouble(sHeight);

                        if (sAddr == sAddress && nHeight > 0)
                        {
                            return Convert.ToDouble(sAmt);
                        }

                    }
                }
            }
            nHeight = 0;
            return 0;
        }


        public static bool SubmitBlock(bool fTestNet, string hex)
        {
            try
            {
                object[] oParams = new object[1];
                oParams[0] = hex;
                NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("submitblock", oParams);
                string result = oOut.Result.Value;
                // To do return binary response code here; check response for fail and success
                if (result == null)
                    return true;
                if (result == "high-hash")
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                Log("SB " + ex.Message);
            }
            return false;
        }



        public static void GetSubsidy(bool fTestNet, int nHeight, ref string sRecipient, ref double nSubsidy)
        {
            try
            {
                object[] oParams = new object[2];
                oParams[0] = "subsidy";
                oParams[1] = nHeight.ToString();
                NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("exec", oParams);
                nSubsidy = GetDouble(oOut.Result["subsidy"]);
                sRecipient = oOut.Result["recipient"];
                return;
            }
            catch (Exception ex)
            {
                Log("GS " + ex.Message);
            }
            sRecipient = "";
            nSubsidy = 0;
        }

        public static string GetBlockForStratumHex(bool fTestNet, string poolAddress, string rxkey, string rxheader)
        {
            try
            {
                object[] oParams = new object[3];
                oParams[0] = poolAddress;
                oParams[1] = rxkey;
                oParams[2] = rxheader;
                NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("getblockforstratum", oParams);
                string result = oOut.Result["hex"];
                return result;
            }
            catch (Exception ex)
            {
                Log("GBFS " + ex.Message);
            }
            return "";
        }

        public struct Payment
        {
            public string bbpaddress;
            public double amount;
        }

        public static string SendMany(bool fTestNet, List<Payment> p, string sFromAccount, string sComment)
        {
            string sPack = "";
            for (int i = 0; i < p.Count; i++)
            {
                string sAmount = string.Format("{0:#.00}", p[i].amount);
                string sRowOld = "\"" + p[i].bbpaddress + "\"" + ":" + sAmount;
                string sRow = "<RECIPIENT>" + p[i].bbpaddress + "</RECIPIENT><AMOUNT>" + sAmount + "</AMOUNT><ROW>";
                sPack += sRow;
            }
            string sXML = "<RECIPIENTS>" + sPack + "</RECIPIENTS>";
            try
            {
                object[] oParams = new object[4];
                oParams[0] = "sendmanyxml";
                oParams[1] = sFromAccount;
                oParams[2] = sXML;
                oParams[3] = sComment;
                NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("exec", oParams);
                string sTX = oOut.Result["txid"].ToString();
                return sTX;
            }
            catch (Exception ex)
            {
                string test = ex.Message;
                Log(" Error while transmitting : " + ex.Message);
                return "";
            }
        }
        public static bool GetNextContract(bool fTestNet, out double nHeight)
        {
            try
            {
                nHeight = 0;
                object[] oParams = new object[1];
                oParams[0] = "nextcontract";
                NBitcoin.RPC.RPCClient n = WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("exec", oParams);
                nHeight = GetDouble(oOut.Result["nextdailysuperblock"].ToString());
                string sNextContract = oOut.Result["nextcontract"].ToString();
                string sHash = ExtractXML(sNextContract, "<hash>", "</hash>");
                bool fExists = sHash.Length == 64;
                return fExists;
            }
            catch (Exception ex)
            {
                nHeight = 0;
                return false;
            }
        }

        public static string GetAddressUTXOs(bool fTestNet, string address)
        {
            try
            {
                if (address == null || address == "")
                    return "";
                NBitcoin.RPC.RPCClient n = GetRPCClient(fTestNet);
                NBitcoin.BitcoinAddress[] a = new NBitcoin.BitcoinAddress[1];
                a[0] = NBitcoin.BitcoinAddress.Create(address, fTestNet ? NBitcoin.Network.TestNet : NBitcoin.Network.Main);
                dynamic o11 = n.GetAddressUTXOs(a);
                string result = o11.Result.ToString();
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public class DACResult
        {
            public string TXID = "";
            public bool Result;
            public string Error;
        }

        public static DACResult SendRawTx(bool fTestNet, string hex)
        {
            DACResult r0 = new DACResult();
            try
            {
                object[] oParams = new object[1];
                oParams[0] = hex;
                NBitcoin.RPC.RPCClient n = GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("sendrawtransaction", oParams);
                r0.TXID = oOut.Result.Value;
                if (r0.TXID == null)
                {
                    r0.Error = "Unable to push.";
                    return r0;
                }
                return r0;
            }
            catch (Exception ex)
            {
                Common.Log("SendRawTx:: " + ex.Message);
                r0.Error = ex.Message;
                return r0;
            }
        }

        public static NBitcoin.RPC.RPCClient GetRPCClient(bool fTestNet)
        {
            NBitcoin.RPC.RPCClient n = null;
            try
            {
                NBitcoin.RPC.RPCCredentialString r = new NBitcoin.RPC.RPCCredentialString();


                string sUser = fTestNet ? "testnetrpcuser" : "rpcuser";
                string sPass = fTestNet ? "testnetrpcpassword" : "rpcpassword";
                string sH = fTestNet ? "testnetrpchost" : "rpchost";
                System.Net.NetworkCredential t = new System.Net.NetworkCredential(GetConfigurationKeyValue(sUser), GetConfigurationKeyValue(sPass));
                r.UserPassword = t;
                string sHost = GetConfigurationKeyValue(sH);
                n = new NBitcoin.RPC.RPCClient(r, sHost, fTestNet ? NBitcoin.Network.TestNet : NBitcoin.Network.Main);
                return n;
            } 
            catch(Exception ex)
            {
                Log("Cannot retrieve RPC client for " + fTestNet.ToString() + " " + ex.Message);
                return n;
            }
        }

        public static string GetNewDepositAddress(bool fTestNet)
        {
            NBitcoin.RPC.RPCClient n = GetRPCClient(fTestNet);
            string sAddress = n.GetNewAddress().ToString();
            return sAddress;
        }

        public static int GetHeight(bool fTestNet)
        {
            object[] oParams = new object[1];
            NBitcoin.RPC.RPCClient n = GetRPCClient(fTestNet);
            dynamic oOut = n.SendCommand("getmininginfo");
            int nBlocks = (int)GetDouble(oOut.Result["blocks"]);
            return nBlocks;
        }

        public static bool ValidateAddress(bool fTestNet, string sAddress)
        {
            try
            {
                NBitcoin.RPC.RPCClient nClient = GetRPCClient(fTestNet);

                object[] oParams = new object[1];
                oParams[0] = sAddress;
                dynamic oOut = nClient.SendCommand("validateaddress", oParams);
                bool fValid = oOut.Result["isvalid"];
                return fValid;
            }
            catch (Exception ex)
            {
                Log("Unable to validate address::" + ex.Message);
                return false;
            }
        }

        public static double GetCoreWalletBalance(bool fTestNet)
        {
            try
            {
                NBitcoin.RPC.RPCClient n1 = GetRPCClient(fTestNet);
                NBitcoin.Money m1 = n1.GetBalance();
                double m2 = Common.GetDouble(m1.ToString());
                return m2;
            }
            catch (Exception ex)
            {
                Common.Log("GetBalanceRPC::" + ex.Message);
                return 0;
            }
        }
    }

}