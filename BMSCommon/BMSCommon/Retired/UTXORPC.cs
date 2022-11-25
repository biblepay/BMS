using System;
using System.Collections.Generic;
using System.Text;
using static BMSCommon.BitcoinSyncModel;

namespace BMSCommon.Retired
{
    public static class UTXORPC
    {

        public static bool GetNextContract(bool fTestNet, out double nHeight)
        {
            try
            {
                nHeight = 0;
                object[] oParams = new object[1];
                oParams[0] = "nextcontract";
                NBitcoin.RPC.RPCClient n = WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("exec", oParams);
                nHeight = Common.GetDouble(oOut.Result["nextdailysuperblock"].ToString());
                string sNextContract = oOut.Result["nextcontract"].ToString();
                string sHash = Common.ExtractXML(sNextContract, "<hash>", "</hash>");
                bool fExists = sHash.Length == 64;
                return fExists;
            }
            catch (Exception ex)
            {
                nHeight = 0;
                return false;
            }
        }


        public static string RPCInsertUTXODataIntoChain(bool fTestNet, string sMessageKey, string sUTXOData)
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

    }
}
