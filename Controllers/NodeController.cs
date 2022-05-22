using BiblePay.BMS.DSQL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using static BiblePay.BMS.Common;
using static BMSCommon.Common;


namespace BiblePay.BMS.Controllers
{
    public class BMSController : Controller
    {

        public struct UnchainedReply
        {
            public string error;
            public string URL;
            public int result;
            public string userid;
            public double version;
        }

        [Route("api/web/getdirectoryinfo")]
        [HttpPost]
        public async Task<IActionResult> getdirectoryinfo()
        {
            UnchainedReply u = new UnchainedReply();
            try
            {
                var key = Request.Headers["key"].ToString();
                string sNN = "";
                string sUID = BBPTestHarness.Service.ValidateKey(key, out sNN);
                if (sUID == "")
                {
                    throw new Exception("API Key invalid.  To obtain a key, go to unchained.biblepay.org | Wallet.");
                }
                List<string> s = BMSCommon.DSQL.QueryIPFSFolderContents("", "", key);
                Log("Bms result " + s.Count.ToString() + " uid " + sUID.ToString());


                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(s);
                var r3 = Ok(new { sJson3 });
                return r3;

            }
            catch (Exception ex)
            {
                u.error = "getdirectoryinfo::BadSelectError::" + ex.Message;
                Log("gdi" + ex.Message);
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                var r3 = Ok(new { sJson3 });
                return r3;
            }
        }


        [Route("api/web/getblocks")]
        [HttpPost]
        [RequestSizeLimit(10000)]
        public async Task<ActionResult> GetBlocks()
        {
            UnchainedReply u = new UnchainedReply();
            try
            {
                var key = Request.Headers["key"].ToString();
                string sStartHash = Request.Headers["hash"].ToString();
                BMSCommon.CryptoUtils.Block b = null;
                BMSCommon.CryptoUtils.mapBlockIndex.TryGetValue(sStartHash, out b);
                if (b == null)
                {
                    throw new Exception("no such block");
                }
                List<BMSCommon.CryptoUtils.Block> l = new List<BMSCommon.CryptoUtils.Block>();
                for (int i = 0; i < 50; i++)
                {
                    if (b == null)
                        break;
                    l.Add(b);
                    string nbh = b.NextBlockHash;
                    b = null;
                    if (nbh != null)
                    {
                        bool f = BMSCommon.CryptoUtils.mapBlockIndex.TryGetValue(nbh, out b);
                    }
                }

                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(l);
                return Content(sJson3);
            }
            catch (Exception ex)
            {
                Log("getblocks::ERROR::" + ex.Message);
                u.error = "Error::" + ex.Message;
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                //var r3 = Ok(new { sJson3 });
                //return Json(u);
                return Content(sJson3);
            }
        }

        [Route("api/web/getmempool")]
        [HttpPost]
        [RequestSizeLimit(10000)]
        public async Task<ActionResult> GetMemPool()
        {
            UnchainedReply u = new UnchainedReply();
            try
            {
                List<BMSCommon.CryptoUtils.Transaction> l = new List<BMSCommon.CryptoUtils.Transaction>();
                foreach (KeyValuePair<string, BMSCommon.CryptoUtils.Transaction> t in BMSCommon.API.dMemoryPool)
                {
                    l.Add(t.Value);
                }
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(l);
                return Content(sJson3);
            }
            catch (Exception ex)
            {
                Log("getmempool::ERROR::" + ex.Message);
                u.error = "Error::" + ex.Message;
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                return Content(sJson3);
            }
        }



        [Route("api/web/pushtx")]
        [HttpPost]
        [RequestSizeLimit(500000000)]
        public async Task<IActionResult> PushTx(List<IFormFile> file)
        {
            // This is the place we accept new bms transactions
            UnchainedReply u = new UnchainedReply();
            try
            {
                if (file.Count == 0)
                {
                    throw new Exception("No memory pool file given.");
                }
                var filePaths = new List<string>();
                var filePath = Path.GetTempFileName();
                filePaths.Add(filePath);
                var postedFile = file[0];
                string sGuid = Guid.NewGuid().ToString() + ".dat";
                string sDestFN = Path.Combine(Path.GetTempPath(), sGuid);
                using (var stream = new FileStream(sDestFN, System.IO.FileMode.Create))
                {
                    await postedFile.CopyToAsync(stream);
                }
                System.IO.FileInfo fi = new FileInfo(sDestFN);
                String data = System.IO.File.ReadAllText(sDestFN);
                List<BMSCommon.CryptoUtils.Transaction> l = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BMSCommon.CryptoUtils.Transaction>>(data);
                for (int i = 0; i < l.Count; i++)
                {
                    bool f = BMSCommon.API.AddToMemoryPool(l[i], true);
                }
                u.version = 1.1;
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                System.IO.File.Delete(sDestFN);
                var r3 = Ok(new { sJson3 });
                return r3;
            }
            catch (Exception ex)
            {
                Log("pushtx:Recv_post_error::" + ex.Message);
                u.error = "post_error::" + ex.Message;
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                var r3 = Ok(new { sJson3 });
                return r3;
            }
        }


        [Route("api/web/pushblock")]
        [HttpPost]
        [RequestSizeLimit(17000000)]
        public async Task<IActionResult> PushBlock(string sPost)
        {
            // This is the place we accept new bms transactions
            UnchainedReply u = new UnchainedReply();
            try
            {
                BMSCommon.CryptoUtils.Block b = Newtonsoft.Json.JsonConvert.DeserializeObject<BMSCommon.CryptoUtils.Block>(sPost);
                bool f = BMSCommon.API.ConnectBlock(b, true);
                if (!f)
                    throw new Exception("Block rejected.");
                Log("Accepted block " + b.GetBlockHash());
                u.version = 1.1;
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                var r3 = Ok(new { sJson3 });
                return r3;
            }
            catch (Exception ex)
            {
                Log("pushblock:post_error::" + ex.Message);
                u.error = "post_error::" + ex.Message;
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                var r3 = Ok(new { sJson3 });
                return r3;
            }
        }



        [Route("api/web/bbpingress")]
        [HttpPost]
        [RequestSizeLimit(5510000000)]
        [RequestFormLimits(MultipartBodyLengthLimit = Int32.MaxValue)]
        public async Task<IActionResult> Index(List<IFormFile> file)
        {
            UnchainedReply u = new UnchainedReply();
            try
            {
                var key = Request.Headers["key"].ToString();
                string immutable = Request.Headers["immutable"].ToString();
                string sDelete = Request.Headers["delete"].ToString();
                string sNN = "";
                string sUID = BBPTestHarness.Service.ValidateKey(key, out sNN);
                if (sUID == "")
                {
                    throw new Exception("API Key invalid.  To obtain a key, go to unchained.biblepay.org | Wallet.");
                }

                string sDestinationPrefix = "";
                if (sUID == "3")
                {
                    sDestinationPrefix = "";
                }
                else
                {
                    sDestinationPrefix = "video/" + sUID + "/";
                }

                string sDestinationURL = sDestinationPrefix + Request.Headers["url"].ToString();
                string sDestFolder = BMSCommon.Common.GetFolder("");
                string sFullDest = Path.Combine(sDestFolder, sDestinationURL);
                if (sFullDest.Contains("..") || sDestinationURL.Contains(".."))
                {
                    throw new Exception("IO Corruption error 03232022::" + sFullDest + "::" + sDestinationURL);
                }
                string sDestDirOnly = ChopLastOctetFromPath(sFullDest);

                if (sDelete == "1")
                {
                    bool fOK = await BBPTestHarness.Service.RegisterPin(sDestinationURL, sUID, sFullDest, true);
                }

                // Uploads into ingress

                if (file.Count == 0)
                {
                    Log("BBPIngress:No file posted... 00");
                    u.error = "You must post a file.";
                    u.result = -1;
                    u.URL = "";
                    string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                    var r1 = Ok(new { sJson });
                    return r1;
                }

                var postedFile = file[0];
                Log("Ingress::Key = " + key.ToString() + ",URL = " + sDestinationURL);

                DirectoryInfo di = new DirectoryInfo(sDestDirOnly);
                if (!System.IO.Directory.Exists(sDestDirOnly))
                {
                    System.IO.Directory.CreateDirectory(sDestDirOnly);
                }

                if (true)
                {
                    var filePaths = new List<string>();
                    var filePath = Path.GetTempFileName(); //we are using Temp file name just for the example. Add your own file path.
                    filePaths.Add(filePath);
                    //Common.Log("Starting ingress write " + filePath);
                    using (var stream = new FileStream(sFullDest, System.IO.FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                    System.IO.FileInfo fi = new FileInfo(sFullDest);
                    Log("BBPIngress_2::Writing " + sFullDest + ", sz = " + fi.Length.ToString());
                    if (fi.Length > 0)
                    {
                        bool fOK = await BBPTestHarness.Service.RegisterPin(sDestinationURL, sUID, sFullDest, false);
                    }
                    // Mission critical: Persist destination in Pins table for future charges...
                    u.URL = BMSCommon.API.GetCDN() + "/" + sDestinationURL;
                }
                u.version = 1.2;
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);

                var r3 = Ok(new { sJson3 });
                return r3;
            }
            catch (Exception ex)
            {
                Log("BBPIngress:Bad file post error::" + ex.Message);
                u.error = "Ingress::BadFileError::" + ex.Message;
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                var r3 = Ok(new { sJson3 });
                return r3;
            }
        }

        [Route("BMS/Nodes")]
        public string Nodes()
        {
            // Add % Synced, FullyQualifiedVersion to Node Info influencing the POVS test.
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(BMSCommon.API.mNodes, Newtonsoft.Json.Formatting.Indented);
            return sJson;
        }

        [Route("BMS/GetBestBlockHash")]
        public string GetBestBlockHash()
        {
            string s = BMSCommon.API.GetBestBlockHash();
            return s;
        }

        [Route("BMS/GetBlockCount")]
        public int GetBlockCount()
        {
            int n = BMSCommon.API.GetBlockCount();
            return n;
        }

        public struct PriceQuote
        {
            public string Price;
            public string XML;
        }
        [Route("BMS/GetPriceQuote")]
        public string GetPriceQuote()
        {
            string sPair = Request.Query["pair"];
            PriceQuote q = new PriceQuote();
            double dPrice = BMSCommon.Pricing.GetPriceQuote(sPair);
            q.Price = dPrice.ToString("0." + new string('#', 339));
            q.XML = "<MIDPOINT>" + q.Price + "</MIDPOINT><EOF>";
            String sJson = Newtonsoft.Json.JsonConvert.SerializeObject(q, Newtonsoft.Json.Formatting.Indented);
            return sJson;
        }
        public struct MobileAPI1
        {
            public double BTCUSD;
            public double BBPUSD;
            public string BBPBTC;
        }
        [Route("BMS/MobileAPI")]
        public string MobileAPI()
        {
            MobileAPI1 m = new MobileAPI1();
            m.BTCUSD = BMSCommon.Pricing.GetPriceQuote("BTC/USD");
            double nBBPBTC = BMSCommon.Pricing.GetPriceQuote("BBP/BTC");
            m.BBPUSD = m.BTCUSD * nBBPBTC;
            m.BBPBTC = nBBPBTC.ToString("0." + new string('#', 339));
            String sJson = Newtonsoft.Json.JsonConvert.SerializeObject(m);
            return sJson;
        }

        [Route("BMS/GetMemPoolCount")]
        public int GetMemPoolCount()
        {
            int n = BMSCommon.API.GetMemPoolCount();
            return n;
        }


        [Route("BMS/GetFileSize")]
        public string GetFileSize()
        {
            string sSource = Request.Query["name"];

            if (sSource == null)
                return "0";
            string sRootPath = GetVideoFolder();
            sSource = NormalizeFilePath(sSource);
            string sMainPath = NormalizeFilePath(sRootPath + sSource);
            if (System.IO.File.Exists(sMainPath))
            {
                FileInfo fi = new FileInfo(sMainPath);
                return fi.Length.ToString();
            }
            else
            {
                Log("GetFileSize::Cant find file @" + sMainPath + " from root " + sRootPath + ", and FN " + sSource);
            }
            return "0";
        }

        public struct StatusObject
        {
            public string URL;
            public int BMS_VERSION;
            public int COMMON_VERSION;
            public string Status;
            public double Synced_Count;
            public double File_Count;
            public double Synced_Percent;
            public string EOF;
            public int Memory_Pool_Count;
            public string Best_Block_Hash;
            public int Block_Count;
            public int Hashes;
        };

        [Route("BMS/CreateFake")]
        public string CreateFake()
        {

            BMSCommon.CryptoUtils.CreateFakeTransactions();
            return "Doing...";
        }

        [Route("BMS/Rollback")]
        public int Rollback()
        {
            int n = BMSCommon.API.PerformRollback();
            return n;
        }

        [Route("BMS/Status")]
        public string Status()
        {
            string sBindURL = GetConfigurationKeyValue("bindurl");
            StatusObject s = new StatusObject();
            double nCalc = Sync.METRIC_SYNCED_COUNT / (Sync.METRIC_FILECOUNT + .01);
            s.Synced_Count = Sync.METRIC_SYNCED_COUNT;
            s.File_Count = Sync.METRIC_FILECOUNT;
            s.Synced_Percent = nCalc;
            s.Hashes = BMSCommon.Miner.mnHashes;
            s.BMS_VERSION = BMS_VERSION;
            s.COMMON_VERSION = BMSCommon.Common.BMSCOMMON_VERSION;
            BMSCommon.CryptoUtils.Block b = BMSCommon.CryptoUtils.GetBestBlock();
            if (b != null)
            {
                s.Block_Count = b.BlockNumber;
                s.Best_Block_Hash = b.GetBlockHash();
            }
            s.Memory_Pool_Count = BMSCommon.API.dMemoryPool.Count;
            s.Status = "OK";
            s.URL = sBindURL;
            s.EOF = "<EOF>";
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(s, Newtonsoft.Json.Formatting.Indented);
            return sJson;
        }


        [Route("BMS/POSE")]
        public string POSE()
        {
            // We need to ensure MySQL is installed and synced too, somehow, in POSE return...
            // MISSION CRITICAL!
            string sPOSE = BBPTestHarness.POSE.GetPerformanceReport();
            sPOSE += "\r\n|v1.0|Status: OK\n|\r\n<EOF>\n";
            return sPOSE;
        }

        [Route("BMS/GetBlock")]
        public string GetBlock()
        {
            string blocknumber = Request.Query["id"].ToString() ?? "";
            double nBN = GetDouble(blocknumber);
            BMSCommon.CryptoUtils.Block b = null;
            if (nBN > 0)
            {
                b = BMSCommon.CryptoUtils.GetBlockByNumber((int)nBN);
            }
            else
            {
                b = BMSCommon.CryptoUtils.GetBlock(blocknumber);
            }
            if (b != null)
            {
                string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(b, Newtonsoft.Json.Formatting.Indented);
                return sJson;
            }
            else
            {
                return "404";
            }
        }


        [Route("BMS/KILL")]
        public string KILL()
        {
            bool fNeedsUpgraded = ProcessAsyncHelper.NeedsUpgraded();
            if (!fNeedsUpgraded)
                return "-2";
            ProcessAsyncHelper.StartNewThread();
            return "86";
        }

       

        private static int nLastUpgradeManifest = 0;
        private static string msUpgradeManifest = String.Empty;
        [Route("BMS/GetUpgradeManifest")]
        public string GetUpgradeManifest()
        {
            try
            {
                int nElapsed = UnixTimestamp() - nLastUpgradeManifest;
                if (nElapsed < (60 * 60) && msUpgradeManifest != String.Empty)
                {
                    return msUpgradeManifest;
                }
                var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
                Log("Requesting manifest from " + remoteIpAddress);

                string sPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string sHashes = string.Empty;
                string h1 = GetUpgradeFileHashes(sPath);
                nLastUpgradeManifest = UnixTimestamp();
                msUpgradeManifest = h1;
                return h1;
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                return "-1";
            }
        }
    }
}

