using BMSShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMSCommon.Model;

namespace BiblePay.BMS.DSQL.Retired
{
    public class Retired
    {
        /*
        public static DACResult RetiredSendBBPOldMethod(bool fTestNet, string sType, string sToAddress, double nAmount, string sPayload)
        {
            string sPackaged = BMSCommon.WebRPC.PackageBBPChainDataMessage(fTestNet, sType, sPayload);
            string sPrivKey = BMSCommon.WebRPC.GetFDPair(fTestNet);
            string sPubKey = BMSCommon.WebRPC.GetFDPubKey(fTestNet);
            string sUnspentData = BMSCommon.WebRPC.GetAddressUTXOs(fTestNet, sPubKey);
            string sErr = "";
            string sTXID = "";
            .Crypto.BBPTransaction.PrepareFundingTransaction(fTestNet, nAmount, sToAddress, sPrivKey, sPackaged, sUnspentData, out sErr, out sTXID);
            DACResult r = new DACResult();
            if (sErr != "")
            {
                r.Error = sErr;
                return r;
            }
            r = BMSCommon.WebRPC.SendRawTx(fTestNet, sTXID);
            return r;
        }
        */

    }
}
