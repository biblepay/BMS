using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static BMSCommon.Common;

namespace BiblePay.BMS.DSQL
{
    public static class PoolPayments
    {



        static int nLastPaidM = UnixTimestamp();
        static int nLastPaidT = UnixTimestamp();

        public static bool PayPoolParticipants(XMRPoolBase x)
        {
            int nLastPaid1 = x.IsTestNet() ? nLastPaidT : nLastPaidM;
            int nElapsed = UnixTimestamp() - nLastPaid1;
            if (nElapsed < (60 * 60 * 8))
                return false;
            if (x.IsTestNet())
            {
                nLastPaidT = UnixTimestamp();
            }
            else
            {
                nLastPaidM = UnixTimestamp();
            }
            try
            {
                BMSCommon.Pricing.StoreQuotes(0);
                x.RecordDifficultyHistory();
                x.ClearBans();
            }
            catch (Exception ex2)
            {
                Log("PayPoolParticipants: " + ex2.Message);
            }
            try
            {
                // Create a batchid
                string batchid = Guid.NewGuid().ToString();
                
                string sTable = x.IsTestNet() ? "tshare" : "share";

                string sql = "Update " + sTable + " set txid=@batchid where Paid is null and subsidy > 1 and TIMESTAMPDIFF(MINUTE, updated, now()) > 1440; ";
                MySqlCommand command = new MySqlCommand(sql);

                command.Parameters.AddWithValue("@batchid", batchid);
                BMSCommon.Database.ExecuteNonQuery(command);
                sql = "Select bbpaddress, sum(Reward) reward from " + sTable + " WHERE txid = @batchid and paid is null group by bbpaddress;";

                command = new MySqlCommand(sql);
                command.Parameters.AddWithValue("@batchid", batchid);

                DataTable dt = BMSCommon.Database.GetDataTable(command);
                List<BMSCommon.WebRPC.Payment> Payments = new List<BMSCommon.WebRPC.Payment>();
                double nTotal = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string address = dt.Rows[i]["bbpaddress"].ToString();
                    double nReward = GetDouble(dt.Rows[i]["Reward"]);
                    bool bValid = BMSCommon.WebRPC.ValidateBiblepayAddress(x.IsTestNet(), address);
                    if (bValid && nReward > .01)
                    {
                        nTotal += nReward;
                        BMSCommon.WebRPC.Payment p = new BMSCommon.WebRPC.Payment();
                        p.bbpaddress = address;
                        p.amount = nReward;
                        Payments.Add(p);
                    }
                }

                string poolAccount = GetConfigurationKeyValue("PoolPayAccount");
                if (poolAccount == "")
                {
                    Log("Distress:  Unable to pay workers because pool account is not set.  Set [PoolPayAccount=poolname] in bms.conf.  Where poolname is the name of the address book entry receiving the rewards. ");
                }

                if (Payments.Count > 0)
                {
                    string txid = BMSCommon.WebRPC.SendMany(x.IsTestNet(), Payments, poolAccount, "PoolPayments " + x.GetBlockTemplate().height.ToString());
                    // send
                    if (txid != "")
                    {
                        sql = "Update " + sTable + " SET paid = now(), txid = @txid where txid = @batchid";
                        command = new MySqlCommand(sql);
                        command.Parameters.AddWithValue("@batchid", batchid);
                        command.Parameters.AddWithValue("@txid", txid);
                        BMSCommon.Database.ExecuteNonQuery(command);
                        return true;
                    }
                }
                return false;

            }
            catch (Exception ex)
            {
                Log("PayPool: " + ex.Message);
            }

            return false;
        }

    }




}
