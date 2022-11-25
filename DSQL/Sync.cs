using System;
using System.Collections.Generic;
using static BMSCommon.Common;

namespace BiblePay.BMS.DSQL
{
    public class Sync
    {

        // This thread has a 1:1 relationship with the BWS
        // BWS gets created, and runs one free running Syncer thread

        public static async void Syncer(object oMyURL)
        {
            // Primary entry point for services

            MemorizeNickNames();


            while (1 == 1)
            {
                try
                {
                    try
                    {
                        ReplicateVideos();
                    }
                    catch (Exception ex)
                    {
                        Log("Syncer::SyncWithOthers" + ex.Message);
                    }

                    await BBPTestHarness.Service.BackgroundAngel(BMSCommon.API.GetCDN());
                    await BiblePay.BMS.DSQL.PB.DailyUTXOExport(false, BMSCommon.Common.IsPrimary());
                    await BiblePay.BMS.DSQL.PB.DailyUTXOExport(true, BMSCommon.Common.IsPrimary());

                    System.Threading.Thread.Sleep(60000);
                }
                catch (Exception ex2)
                {
                    Log("Syncer::Caught a crash::" + ex2.Message);
                    System.Threading.Thread.Sleep(60000);
                }
            }
        }

        public static int nLoopCount = 0;
        public static bool fHobbledState = false;
        public static long METRIC_FILECOUNT = 0;
        public static long METRIC_SYNCED_COUNT = 0;
        private static void ReplicateVideos()
        {
            string sVideoPath = GetVideoFolder();

            string sReplication = GetConfigurationKeyValue("replication");
            if (sReplication == "0")
                return;
            double nFreePct = GetFreeDiskSpacePercentage();
            bool fTestNet = true; // MISSION CRITICAL: SWITCH TO MAIN during go live
                                  // Instead of getting every single file from storage, lets do it on demand (in the CDN area).
                                  // Removing.
        }



        public struct NickName
        {
            public string ID;
            public string nickName;
        };

        public static List<NickName> _nicknames = new List<NickName>();
        public static void MemorizeNickNames()
        {
            try
            {
                // These nicknames aren't used yet, but we have a plan to allow our social.biblepay.org users to share files by nickname over the cdn.
                // Example:  https://johndoe.cdn.biblepay.org/myfile.txt can be resolved by nickname since we memorize them here.
                string sURL = BMSCommon.API.GetCDN() + "/shard/nicknames.shard";
                MyWebClient wc = new MyWebClient();
                _nicknames.Clear();
                string data = wc.DownloadString(sURL);
                string[] d = data.Split("<ROW>");
                for (int i = 0; i < d.Length; i++)
                {
                    NickName n = new NickName();
                    n.ID = BBPTestHarness.Common.GetElement(d[i], 0, "|");
                    n.nickName = BBPTestHarness.Common.GetElement(d[i], 1, "|");
                    if (n.nickName != "" && n.ID != "")
                    {
                        _nicknames.Add(n);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error memorizing nicknames:: " + ex.Message);
            }
        }



    }
}

