using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static BMSCommon.WebRPC;

namespace BMSCommon
{
    public static class Tests
    {
        // Migrate from bms sidechain to bbpcore-sidechain
        public static void MigrateSidechainData()
        {
            int iRows = 0;

            string sPath = BMSCommon.Common.GetFolder("Log") + "mig_prod.log";
            bool fTestNet = false;
            System.IO.StreamWriter sw = new System.IO.StreamWriter(sPath, false);
            string Timestamp = DateTime.Now.ToString();

            try
            {
                string sql = "select * from bms0.transactions order by added;";
                MySqlCommand m1 = new MySqlCommand(sql);
                DataTable dt = Database.GetDataTable( m1 );
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    string sData = dt.Rows[i]["data"].ToString();
                    var o = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(sData);
                    string sTable = (string)BitcoinSync.GetEntityValue(o, "table");
                    bool f11000 = false;
                    bool fCopy = true;
                    if (sTable == "pin")
                    {
                        string URL = (string)BitcoinSync.GetEntityValue(o, "URL");
                        //&& !URL.Contains("/1.m3u8"))
                        if (URL.Contains(".ts")) 
                            fCopy = false;
                        bool f1101 = false;
                    }
                    if (sTable == "")
                        fCopy = false;

                    if (sTable == "Junk2" || sTable == "Junk" || sTable == "NFT" || sTable=="OrphanExpense" || sTable=="OrphanExpense2" || sTable=="SponsoredOrphan")
                        fCopy = false;

                    if (fCopy)
                    {
                        sw.WriteLine(sData);
                        sw.WriteLine("");
                        iRows++;
                        retry:
                        string txid = BMSCommon.WebRPC.PushChainData2(fTestNet, "DATA", sData);
                        if (txid == "")
                        {
                            bool f1105 = false;
                            System.Threading.Thread.Sleep(10000);
                            goto retry;
                        }
                        System.Threading.Thread.Sleep(70);

                    }
                }
                sw.Close();
            }catch(Exception ex)
            {

                string s1 = ex.Message;
            }
        }
    }
}
