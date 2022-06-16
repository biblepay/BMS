using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using static BiblePay.BMS.Common;
//using static BiblePay.BMS.DSQL.Hasher;
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

                    await BBPTestHarness.Service.ConvertToHLSDaemon(BMSCommon.API.GetCDN());
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
            if (nFreePct < 10)
            {
                // Hard drive space too low to perform replication...  This node runs in a hobbled state
                if (!fHobbledState)
                    Log("!EMERGENCY! We are now in a Hobbled " + nFreePct.ToString());
                fHobbledState = true;
                return;
            }

            bool fTestNet = true; // MISSION CRITICAL: SWITCH TO MAIN during go live
            List<string> l = BMSCommon.DSQL.QueryIPFSFolderContents(fTestNet, "", "", "");
            MemorizeNickNames();
            MyWebClient wc = new MyWebClient();
            int nProcessed = 0;
            int nMax = 100; // This allows the service to breathe (once per 10, we break)
            nLoopCount++;
            if (nLoopCount % 10 == 0)
            {
                Log("Replication Count " + l.Count.ToString());
            }

            METRIC_FILECOUNT = l.Count;
            METRIC_SYNCED_COUNT = 0;

            for (int i = 0; i < l.Count; i++)
            {
                string sFullPath = Path.Combine(sVideoPath, l[i]);
                sFullPath = NormalizeFilePath(sFullPath);
                bool f1 = System.IO.File.Exists(sFullPath);
                if (f1)
                {
                    METRIC_SYNCED_COUNT++;
                    continue;
                }

                string sRootDir = ChopLastOctetFromPath(sFullPath);
                if (!System.IO.Directory.Exists(sRootDir))
                {
                    System.IO.Directory.CreateDirectory(sRootDir);
                }

                int iRepl = 0;
                try
                {
                    string sPullURL = BMSCommon.DSQL.GetURL(l[i]);

                    wc.DownloadFile(sPullURL, sFullPath);
                    iRepl++;

                    if ((iRepl % 10 == 0))
                    {
                        Log("Sync::Replicated " + sFullPath + "  __ " + i.ToString());
                    }
                    nProcessed++;

                    if (nProcessed > nMax)
                    {
                        Log("Processed > nMax, exiting at " + i.ToString());
                        return;
                    }

                    if (sPullURL.ToLower().Contains("1.m3u8"))
                    {
                        // we need to pull the entire file.
                        List<string> t = GetTSFileParams(sFullPath);
                        if (t.Count > 0)
                        {
                            for (int z = 0; z < t.Count; z++)
                            {
                                sPullURL = BMSCommon.DSQL.GetURL(l[i]);
                                sPullURL = sPullURL.Replace("1.m3u8", t[z]);
                                string sOrigFileName = l[i];
                                sOrigFileName = sOrigFileName.Replace("1.m3u8", t[z]);
                                sFullPath = Path.Combine(sVideoPath, sOrigFileName);
                                bool fExists = System.IO.File.Exists(sOrigFileName);
                                if (!fExists)
                                {
                                    wc.DownloadFile(sPullURL, sFullPath);
                                    iRepl++;
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Log("Repl err 1.0::" + ex.Message + " for " + l[i] + " to " + sFullPath);
                    // Error (does not exist in CDN)
                }

            }
            Log("Repl::Finished");
            System.Threading.Thread.Sleep(6000);
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
                    n.ID = BBPTestHarness.IPFS.GetElement(d[i], 0, "|");
                    n.nickName = BBPTestHarness.IPFS.GetElement(d[i], 1, "|");
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

        public static double GetShardSize(string sURL)
        {
            string sData = BMSCommon.Common.ExecuteMVCCommand(sURL);
            double d = GetDouble(sData);
            return d;
        }


    }
}

