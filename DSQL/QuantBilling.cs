using OptionsShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static BMSCommon.BitcoinSyncModel;
using static BMSCommon.Model;
using static BMSCommon.WebRPC;

namespace BiblePay.BMS.DSQL
{
    public static class QuantBilling
    {
        public static async void Looper()
        {
            while (true)
            {
                try
                {
                    ChargeMonthlyQuantSubscriptionFees(false);
                }
                catch (Exception ex)
                {
                    BMSCommon.Common.Log("Looper::" + ex.Message);
                }
                System.Threading.Thread.Sleep(20000);
            }
        }
        public static void InsertTxHistory(string UserID, string sCategory, string sDescription, double nAmount, string sTXID)
        {
            string sql = "Insert into TxHistory (Category,id,UserID,Added,Updated,Amount,Description,TXID) values (@category,newid(),@userid,getdate(),getdate(),@amount,@description,@TXID);";
            SqlCommand s = new SqlCommand(sql);
            s.Parameters.AddWithValue("@userid", UserID);
            s.Parameters.AddWithValue("@description", sDescription);
            s.Parameters.AddWithValue("@amount", nAmount);
            s.Parameters.AddWithValue("@TXID", sTXID);
            s.Parameters.AddWithValue("@category", sCategory);
            SQLDatabase.ExecuteNonQuery(s,"localhost");
        }
        public static async Task<bool> ChargeMonthlyQuantSubscriptionFees(bool fTestNet)
        {
            double nCoreBalance = BMSCommon.WebRPC.GetCachedCoreWalletBalance(false);
            bool fPrimary = BMSCommon.Common.IsPrimary();
            if (!fPrimary)
                return false;

            if (DateTime.UtcNow.Day != 1)
                return false;

            bool fLatch = await BMSCommon.Database.LatchNew(fTestNet, "ChargeMonthlyQuantSubscriptionFees", 60 * 60 * 8);
            if (!fLatch)
                return false;

            string sql = "Select * from Subscription where isnull(LastBilled,'1-1-1900') < getdate()-15;";
            DataTable dt = SQLDatabase.GetDataTable(sql);
            string sPAKey = fTestNet ? "tPoolAddress" : "PoolAddress";
            string sPoolAddress = BMSCommon.Common.GetConfigurationKeyValue(sPAKey);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //10-29-2022
                string sID = dt.Rows[i]["id"].ToString();
                string sERCAddress = dt.Rows[i]["UserID"].ToString();
                string sSig = dt.Rows[i]["Signature"].ToString();
                BMSCommon.Encryption.KeyType kPayer = DSQL.UI.GetKeyPair2(fTestNet, sERCAddress, sSig);
                double nFee = DSQL.UI.ConvertUSDToBiblePay(dt.Rows[i]["MonthlyCost"].ToDouble()) / 100;
                double nBalance = DSQL.UI.QueryAddressBalance(fTestNet, kPayer.PubKey);
                string sResult = String.Empty;
                if (nBalance > nFee)
                {
                    DACResult r = UI.SendBBPFromSubscription(fTestNet, kPayer, sPoolAddress, nFee, "QuantSubscription");
                    if (r.TXID != String.Empty)
                    {
                        string sqlUpdate1 = "Update Subscription set Status='OK',Updated=getdate(),LastBilled=getdate() where id='" + sID + "'";
                        SQLDatabase.ExecuteNonQuery(sqlUpdate1);
                        string sDesc = "Monthly subscription fee for " + dt.Rows[i]["Description"].ToString() + " - " + sID;
                        InsertTxHistory(sERCAddress, "SUBSCRIPTION", sDesc, nFee,r.TXID);
                    }
                    else
                    {
                        sResult = "Charge Failed.";
                    }
                }
                else
                {
                    sResult = "Balance too low";
                }
                if (sResult != String.Empty)
                {
                    string sqlUpdate = "Update Subscription set Status='BAD',Updated=getdate(),LastError='" + sResult + "' where id='" + sID + "'";
                    SQLDatabase.ExecuteNonQuery(sqlUpdate);
                }
            }

            return true;
        }

    }
}
