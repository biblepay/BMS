using BBPAPI;
using BMSCommon;
using BMSShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMSCommon.Model;
using static BMSCommon.Common;
using BBPAPI.Model;

namespace BBPAPI
{
    public static class GovernanceProposal
    {


        /*
        public static bool gobject_prepare(User u, bool fTestNet, Proposal p)
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
            NBitcoin.RPC.RPCClient n = WebRPC.GetRPCClient(fTestNet);
            dynamic oOut = n.SendCommand("gobject", oParams);
            string sPrepareTXID = oOut.Result.ToString();
            p.PrepareTXID = sPrepareTXID;
            p.Updated = DateTime.Now;
            string sSerial = Newtonsoft.Json.JsonConvert.SerializeObject(p, Newtonsoft.Json.Formatting.Indented);
            return DB.<BMSCommon.Model.Proposal>(u,"options.proposal", p, p.id);
        }
        */

        /*
        public static bool gobject_submit(User u, bool fTestNet, Proposal p)
        {
            try
            {

                if (p.SubmitTXID != null && p.SubmitTXID.Length > 1)
                {
                    return false;
                }

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
                NBitcoin.RPC.RPCClient n = WebRPC.GetRPCClient(fTestNet);
                dynamic oOut = n.SendCommand("gobject", oParams);
                string sSubmitTXID = oOut.Result.ToString();
                if (sSubmitTXID.Length > 20)
                {
                    // Update the record allowing us to know this has been submitted
                    p.Submitted = DateTime.Now;
                    p.SubmitTXID = sSubmitTXID;
                    return DB.<Proposal>(u,"options.proposal", p, p.id);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        */


    }
}
