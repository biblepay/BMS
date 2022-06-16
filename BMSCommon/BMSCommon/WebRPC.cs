using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static BMSCommon.Common;

namespace BMSCommon
{

    public class SupplyType
    {
        public double CirculatingSupply = 0;
        public double TotalSupply = 0;
        public double TotalBurned = 0;
    }

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

        public static string GetFDPubKey(bool fTestNet)
        {
            string sPubKey = fTestNet ? "yTrEKf8XQ7y7tychC2gWuGw1hsLqBybnEN" : "BB2BwSbDCqCqNsfc7FgWFJn4sRgnUt4tsM";
            return sPubKey;
        }

        private static string GetFDPair(bool fTestNet)
        {
            // pub BB2BwSbDCqCqNsfc7FgWFJn4sRgnUt4tsM
            // testnet pub yTrEKf8XQ7y7tychC2gWuGw1hsLqBybnEN
            string sProd = BMSCommon.Common.GetConfigurationKeyValue("foundationprivkey");
            string sTest = BMSCommon.Common.GetConfigurationKeyValue("foundationtestprivkey");
            return fTestNet ? sTest : sProd;
        }

        public static string SignMessage(bool fTestNet, string sPrivKey, string sMessage)
        {
            try
            {
                if (sPrivKey == null || sMessage == String.Empty || sMessage == null)
                    return string.Empty;

                BitcoinSecret bsSec;
                if (!fTestNet)
                {
                    bsSec = Network.Main.CreateBitcoinSecret(sPrivKey);
                }
                else
                {
                    bsSec = Network.TestNet.CreateBitcoinSecret(sPrivKey);
                }
                string sSig = bsSec.PrivateKey.SignMessage(sMessage);
                string sPK = bsSec.GetAddress().ToString();
                var fSuc = VerifySignature(fTestNet, sPK, sMessage, sSig);
                return sSig;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public static MessageSigner _MessageSignerTest =  new MessageSigner();
        public static MessageSigner _MessageSignerMain = new MessageSigner();
        public struct MessageSigner
        {
            public string SigningPublicKey;
            public string Signature;
            public string SignMessage;
        };

        public static string InsertUTXODataIntoChain(bool fTestNet, string sMessageKey, string sUTXOData)
        {
            try
            {
                // <MK>GSC</MK> = Daily GSC contract
                MessageSigner m = fTestNet ? _MessageSignerTest : _MessageSignerMain;
                if (m.Signature == null || m.Signature == "")
                {
                    Common.Log("The node must have at least 1MM bbp to sign messages.");
                    return "";
                }
                string sBurnAddress = BMSCommon.Encryption.GetBurnAddress(fTestNet);
                string sError = "";
                double nAmount = 101;
                //string sXML = "<MK>" + sMessageKey + "</MK><MV>" + sUTXOData + "</MV><BOMSG>" + m.SignMessage + "</BOMSG><BOSIG>" + m.Signature + "</BOSIG><BOSIGNER>" + m.SigningPublicKey + "</BOSIGNER>";
                                string sTXID = BMSCommon.WebRPC.PushChainData2(fTestNet, sMessageKey, sUTXOData);
                if (sTXID == "")
                {
                    throw new Exception("Unabled to add to memory pool.");
                }
                return sTXID;
            }
            catch (Exception)
            {
                BMSCommon.Common.Log("An error occured in the daily utxo report. ");
                return "";
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
            catch (Exception ex)
            {
                Log("GetRawTransaction1: for " + sTxid + " " + ex.Message);
                return "";
            }
        }


        public static string GetRawTransactionXML(string sTxid, bool fTestNet)
        {
            string XML = "";

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
                    }

                    if (sPtr != "")
                    {
                        string sAmount = oOut.Result["vout"][y]["value"].ToString();
                        string sData = oOut.Result["vout"][y]["txoutmessage"];
                        string sAddress = "";
                        if (oOut.Result["vout"][y]["scriptPubKey"]["addresses"] != null)
                        {
                            sAddress = oOut.Result["vout"][y]["scriptPubKey"]["addresses"][0].ToString();
                        }
                        //sOut += sAmount + "," + sAddress + "," + height + "|";
                        XML += sData;
                    }
                    else
                    {
                        break;
                    }
                }
                return XML;
            }
            catch (Exception ex)
            {
                Log("GetRawTransaction2: for " + sTxid + " " + ex.Message);
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

        public static SupplyType GetSupply(bool fTestNet)
        {
            SupplyType s = new SupplyType();
            try
            {
                object[] oParams = new object[0];
                NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("gettxoutsetinfo", oParams);
                s.CirculatingSupply = oOut.Result["total_circulating_money_supply"];
                s.TotalSupply = 5121307024.00; //max supply
                s.TotalBurned = oOut.Result["total_burned"];
                return s;
            }
            catch (Exception ex)
            {
                Log("GS " + ex.Message);
            }
            return s;

        }


        public static string PackageMessage(bool fTestNet, string sType, string sData)
        {
            // <MK>GSC</MK> = Daily GSC contract
            MessageSigner m = fTestNet ? _MessageSignerTest : _MessageSignerMain;
            if (m.Signature == null || m.Signature == "")
                throw new Exception("Unable to sign.");
            //string sSig = SignMessage(fTestNet, sFoundationSignPrivKey, sSignMessage);
            string sXML = "<MK>" + sType + "</MK><MV>" + sData + "</MV><BOMSG>" + m.SignMessage + "</BOMSG><BOSIG>" + m.Signature 
                + "</BOSIG><BOSIGNER>" + m.SigningPublicKey + "</BOSIGNER>";

            
            return sXML;
        }

        public static string PushChainData2(bool fTestNet, string sType, string sData)
        {
            try
            {
                object[] oParams = new object[2];
                oParams[0] = "bmstransaction";
                string sPackaged = PackageMessage(fTestNet, sType, sData);
                oParams[1] = sPackaged;
                NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("exec", oParams);
                string sTXID = oOut.Result["txid"];
                return sTXID;
            }
            catch (Exception ex)
            {
                Log("PCD " + ex.Message);
                return "";
            }
        }

        public static CryptoUtils.Block GetBlock(bool fTestNet, int nHeight)
        {
            CryptoUtils.Block b = new CryptoUtils.Block();

            try
            {
                object[] oParams = new object[1];
                oParams[0] = nHeight.ToString();
                NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("getblock", oParams);
                b.Hash = oOut.Result["hash"];
                b.MerkleRoot = oOut.Result["merkleroot"];
                b.Difficulty = oOut.Result["difficulty"];
                b.PreviousBlockHash = oOut.Result["previousblockhash"];
                b.NextBlockHash = oOut.Result["nextblockhash"];
                b.BlockNumber = nHeight;
                b.Time = oOut.Result["time"];
                for (int y = 0; y < oOut.Result["tx"].Count; y++)
                {
                    string sTXID = oOut.Result["tx"][y];
                    if (nHeight > 0)
                    {
                        string XML = GetRawTransactionXML(sTXID, fTestNet);
                        CryptoUtils.Transaction t = new CryptoUtils.Transaction();
                        string Extracted = BMSCommon.Common.ExtractXML(XML, "<MV>", "</MV>");
                        string sSigMessage = BMSCommon.Common.ExtractXML(XML, "<BOMSG>", "</BOMSG>");
                        string sSig = BMSCommon.Common.ExtractXML(XML, "<BOSIG>", "</BOSIG>");
                        string sSigningAddress = GetFDPubKey(fTestNet);
                        bool fSigValid = VerifySignature(fTestNet, sSigningAddress, sSigMessage, sSig);
                        // Mission Critical TODO: Implement ACLs so Sancs with more than 1MM BBP can insert sidechain records and run their own social media systems.
                        t.Data = Extracted;
                        t.BlockHash = b.Hash;
                        t.Time = b.Time;
                        t.Height = b.BlockNumber;
                        if (t.Data.Length > 1 && t.Data.Contains("{"))
                        {
                            if (fSigValid)
                            {
                                b.Transactions.Add(t);
                            }

                        }
                    }
                }
                bool f1 = false;

            }
            catch (Exception ex)
            {
                Log("GBFS " + ex.Message);
            }
            return b;
        }


        public struct BalanceUTXO
        {
            public string Address;
            public NBitcoin.Money Amount;
            public NBitcoin.uint256 TXID;
            public NBitcoin.uint256 SpentToTXID;
            public int index;
            public int Height;
            public int SpentToIndex;
            public NBitcoin.Money SpentToNewChangeAmount;
            public bool Chosen;
        };


        public static double QueryAddressBalanceNewMethod(bool fTestNet, string sAddress)
        {
            string sUTXOData = BMSCommon.WebRPC.GetAddressUTXOs(fTestNet, sAddress);
            double nBal = QueryAddressBalanceNewMethod(fTestNet, sAddress, sUTXOData);
            return nBal;
        }
        public static double QueryAddressBalanceNewMethod(bool fTestNet, string sAddress, string sData)
        {
            try
            {
                dynamic oJson = JsonConvert.DeserializeObject<dynamic>(sData);
                double nTotal = 0;
                foreach (var j in oJson)
                {
                    BalanceUTXO u = new BalanceUTXO();
                    u.Amount = new NBitcoin.Money((decimal)j["satoshis"], NBitcoin.MoneyUnit.Satoshi);
                    u.index = Convert.ToInt32(j["outputIndex"].Value);
                    u.TXID = new NBitcoin.uint256((string)j["txid"]);
                    u.Height = (int)j["height"].Value;
                    u.Address = j["address"].Value;
                    //lAllUTXO.Add(u);
                    nTotal += (double)u.Amount.ToDecimal(MoneyUnit.BTC);
                }
                return nTotal;
            }
            catch (Exception ex)
            {
                // Wrong chain?
                return -1;
            }
        }
        public static int GetMasternodeCount(bool fTestNet)
        {
            try
            {
                object[] oParams = new object[0];
                NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("masternodelist", oParams);
                string sData = oOut.Result.ToString();
                string[] vNodes = sData.Split("proTxHash");
                return vNodes.Length;

            }
            catch (Exception ex)
            {
                Log("GBFS " + ex.Message);
            }
            return 1;
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

        public static List<string> listRPCErrors = new List<string>();

        public static void LogRPCError(string sError)
        {
            if (listRPCErrors.Contains(sError))
                return;

            listRPCErrors.Add(sError);
            if (listRPCErrors.Count > 50)
            {
                listRPCErrors.RemoveAt(0);
            }
        }

        private static int nMyCounter = 0;
        public static NBitcoin.RPC.RPCClient GetRPCClient(bool fTestNet)
        {
            NBitcoin.RPC.RPCClient n = null;
            string sUser = "";
            string sPass = "";
            string sHost = "";
            string sTheUser = "";
            try
            {
                NBitcoin.RPC.RPCCredentialString r = new NBitcoin.RPC.RPCCredentialString();
                sUser = fTestNet ? "testnetrpcuser" : "rpcuser";
                sPass = fTestNet ? "testnetrpcpassword" : "rpcpassword";
                string sH = fTestNet ? "testnetrpchost" : "rpchost";
                System.Net.NetworkCredential t = new System.Net.NetworkCredential(GetConfigurationKeyValue(sUser), GetConfigurationKeyValue(sPass));
                r.UserPassword = t;
                sHost = GetConfigurationKeyValue(sH);
                n = new NBitcoin.RPC.RPCClient(r, sHost, fTestNet ? NBitcoin.Network.TestNet : NBitcoin.Network.Main);
                sTheUser = GetConfigurationKeyValue(sUser);

                if (nMyCounter==0)
                {
                    string sTNNarr = fTestNet ? "TESTNET" : "MAINNET";
                    string sNarr = "UNKNOWN IF::RPCCLIENT FOR " + sTNNarr + " for host [" + sHost + "] using user [" + sUser + " " + sTheUser + "] with a password length of " + sPass.Length.ToString();
                    LogRPCError(sNarr);
                }
                nMyCounter++;
                return n;
            } 
            catch(Exception ex)
            {
                string sTNNarr = fTestNet ? "TESTNET" : "MAINNET";
                string sNarr = "UNABLE TO GET RPCCLIENT FOR " + sTNNarr + " for host [" + sHost + "] using user [" + sUser + " " + sTheUser + "] with a password length of " + sPass.Length.ToString() + ". (" + ex.Message + ")";
                LogRPCError(sNarr);
                Log(sNarr);
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
            try
            {
                object[] oParams = new object[1];
                NBitcoin.RPC.RPCClient n = GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("getmininginfo");
                int nBlocks = (int)GetDouble(oOut.Result["blocks"]);
                return nBlocks;
            }
            catch (Exception ex)
            {
                return -1;
            }
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

        private static double _cachedbalance = 0;
        private static int nLastCacheTime = 0;
        public static double GetCachedCoreWalletBalance(bool fTestNet)
        {
            int nElapsed = BMSCommon.Common.UnixTimestamp() - nLastCacheTime;
            if (nElapsed > (60*60))
            {
                _cachedbalance = 0;
            }
            if (_cachedbalance==0)
            {
                _cachedbalance = GetCoreWalletBalance(fTestNet);
                if (_cachedbalance == 0)
                {
                    _cachedbalance = -1;
                }
                else
                {
                    nLastCacheTime = BMSCommon.Common.UnixTimestamp();
                }
            }
            return _cachedbalance;
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