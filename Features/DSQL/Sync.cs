using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static BiblePay.BMS.Common;
using static BiblePay.BMS.DSQL.Hasher;

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
                int nMyPort = BWS.GetPort(oMyURL.ToString());
                try
                {
                    ReplicateVideos();
                }
                catch (Exception ex)
                {
                    Log("Syncer::SyncWithOthers" + ex.Message);
                }

                await BBPTestHarness.Service.ConvertToHLSDaemon();
                System.Threading.Thread.Sleep(60000);
                bool fNeeds = ProcessAsyncHelper.NeedsUpgraded();
                if (fNeeds)
                {
                    Log("Upgrading node...");
                    ProcessAsyncHelper.StartNewThread();

                }
            }
        }

        /*
        private static async Task<bool> PerformSeeder(string sURL, string sFullSourcePath)
        {
            // We are only seeding the  ny
            if (sURL.Contains("https://bbpnyc"))
            {
                string sNewURL = "video/" + sURL.Replace("https://bbpnyc.b-cdn.net/", "");

                if (System.IO.File.Exists(sFullSourcePath))
                {
                    await BBPTestHarness.IPFS.UploadIPFS(sFullSourcePath, sNewURL);
                    bool f2 = false;
                }
            }
            return true;
        }
        */

        public static int nLoopCount = 0;
        public static bool fHobbledState = false;
        private static void ReplicateVideos()
        {
            string sVideoPath = GetVideoFolder();

            string sReplication = GetConfigurationKeyValue("replication");
            if (sReplication == "0")
                return;
            double nFreePct = BiblePay.BMS.DSQL.modLegacyCryptography.GetFreeDiskSpacePercentage();
            if (nFreePct < 10)
            {
                // Hard drive space too low to perform replication...  This node runs in a hobbled state
                if (!fHobbledState)
                    Log("!EMERGENCY! We are now in a Hobbled " + nFreePct.ToString());
                fHobbledState = true;
                return;
            }

            List<ManifestEntry> l = GetManifest();
            MemorizeNickNames();

            MyWebClient wc = new MyWebClient();

            int nProcessed = 0;
            int nMax = 100; // This allows the service to breathe (once per 10, we break)
            nLoopCount++;

            if (nLoopCount % 10 == 0)
            {
                Log("Replication Count " + l.Count.ToString());
            }
            for (int i = 0; i < l.Count; i++)
            {
                string sFullPath = Path.Combine(sVideoPath, l[i].Path);
                sFullPath = NormalizeFilePath(sFullPath);
                bool f1 = System.IO.File.Exists(sFullPath);
                if (f1)
                {
                    continue;
                }

                string sRootDir = API.ChopLastOctetFromPath(sFullPath);
                if (!System.IO.Directory.Exists(sRootDir))
                {
                    System.IO.Directory.CreateDirectory(sRootDir);
                }

                int iRepl = 0;
                try
                {
                    wc.DownloadFile(l[i].URL, sFullPath);
                    iRepl++;
                    if (!sFullPath.Contains(".ts") || (iRepl % 10 == 0))
                    {
                        Log("Sync::Replicated " + sFullPath + "  __ " + i.ToString());
                    }
                    nProcessed++;
                    if (nProcessed > nMax)
                    {
                        Log("Processed>nMax,exiting at " + i.ToString());
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Log("Repl err 1.0::" + ex.Message + " for " + l[i].URL + " to " + sFullPath);
                    // Error (does not exist in CDN)
                }

            }
            Log("Repl::Finished");
            System.Threading.Thread.Sleep(5000);
        }


        // The manifest contains a list of DSQL-Video-HLS objects.  This list only contains immutable videos.
        public struct ManifestEntry
        {
            public string ID;
            public string Path;
            public int RecordNbr;
            public string URL;
            public int DetailNbr;
            public string HashCode;
        };


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
                string sURL = GetCDN() + "/shard/nicknames.shard";
                 MyWebClient wc = new MyWebClient();
                _nicknames.Clear();
                string data = wc.DownloadString(sURL);
                string[] d = data.Split("<ROW>");
                for (int i = 0; i < d.Length; i++)
                {
                    NickName n = new NickName();
                    n.ID = BBPTestHarness.IPFS.GetElement(d[i], 0,"|");
                    n.nickName = BBPTestHarness.IPFS.GetElement(d[i], 1,"|");
                    if (n.nickName != "" && n.ID != "")
                    {
                        _nicknames.Add(n);
                    }
                }
            }
            catch(Exception ex)
            {
                Log("Error memorizing nicknames:: " + ex.Message);
            }
       }
       public static List<ManifestEntry> GetManifest()
       {
            // We split the inbound manifest into 16 shards to make each list smaller.
            // In the near future, we will check the hashes of the shards so we can skip over unchanging shards making this much more efficient.

            List<ManifestEntry> lVideo = new List<ManifestEntry>();
            int iRecordNbr = 0;
            int iDetailNbr = 0;
            string sOldRootDir = "";
            string sCDN = GetCDN();
            
            string sPath = GetFolder("Log") + "manifest.log";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(sPath,false);

            for (int j = 0; j < 15; j++)
            {
                string sPrefix = j.ToString("x");
                string sURL1 = sCDN + "/shard/" + sPrefix + ".shard";
                string sManifest = ExecuteMVCCommand(sURL1);
                string[] vManifest = sManifest.Split("\n");
                for (int i = 1; i < vManifest.Length; i++)
                {
                    string sData = vManifest[i];
                    sData = sData.Replace("\r", "");
                    sData = sData.Replace("\n", "");
                    string sRootDir = API.ChopLastOctetFromURL(sData);
                    if (sData != null && sData.Length > 3)
                    {
                        iDetailNbr++;
                        if (sRootDir != sOldRootDir)
                        {
                            iRecordNbr++;
                        }
                        ManifestEntry m = new ManifestEntry();
                        m.DetailNbr = iDetailNbr;
                        m.RecordNbr = iRecordNbr;
                        m.Path = sData;
                        m.URL = sCDN + "/" + sData;
                        lVideo.Add(m);
                        sw.WriteLine(sData);
                    }
                }
            }
            sw.Close();
            return lVideo;
        }

    }
}
