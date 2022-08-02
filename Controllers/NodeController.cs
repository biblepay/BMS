using BiblePay.BMS.DSQL;
using BMSCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using static BiblePay.BMS.Common;
using static BMSCommon.Common;
using static BMSCommon.CryptoUtils;

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
                bool fTestNet = true; // Mission Critical, how to determine chain here?
                List<string> s = BMSCommon.DSQL.QueryIPFSFolderContents(fTestNet, "", "", key);
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
                    // Mission Critial - how do we choose the chain here?
                    bool fOK = await BBPTestHarness.Service.RegisterPin(true, sDestinationURL, sUID, sFullDest, true);
                    fOK = await BBPTestHarness.Service.RegisterPin(false, sDestinationURL, sUID, sFullDest, true);
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
                        bool fOK = await BBPTestHarness.Service.RegisterPin(true, sDestinationURL, sUID, sFullDest, false);
                        fOK = await BBPTestHarness.Service.RegisterPin(false, sDestinationURL, sUID, sFullDest, false);

                    }
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
            Block b = BMSCommon.BitcoinSync.GetBestBlock(DSQL.UI.IsTestNet(HttpContext));
            string s = b.Hash;
            return s;
        }

        [Route("BMS/GetBlockCount")]
        public int GetBlockCount()
        {
            int n = BMSCommon.BitcoinSync.GetBestHeight(DSQL.UI.IsTestNet(HttpContext));
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
            public string Best_Block_Hash_Main;
            public int Block_Count_Main;
            public string Best_Block_Hash_Test;
            public int Block_Count_Test;
            public List<string> RPCErrorList;
            public int TESTNET_RPC_HEIGHT;
            public int MAINNET_RPC_HEIGHT;
            //public int Hashes;
        };


        [Route("BMS/msd")]
        public int Rollback()
        {
            //BMSCommon.Tests.MigrateSidechainData();
            MailAddress mTo = new MailAddress("rob@biblepay.org", "Rob Andrews");
            MailMessage m = new MailMessage();
            m.To.Add(mTo);
            string sSubject = "UNABLE TO ______";
            m.Subject = sSubject;
            m.Body = "Error, ____________ for .";
            m.IsBodyHtml = false;
            BBPTestHarness.IPFS.SendMail(false, m);
            return 70;
        }

        [Route("BMS/Supply")]
        public string Supply()
        {
            // this endpoint shows bbp circulatin supply
            BMSCommon.SupplyType s = BMSCommon.WebRPC.GetSupply(false);
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(s, Newtonsoft.Json.Formatting.Indented);
            return sJson;
        }

        [Route("BMS/Debug")]
        public async Task<string> Debug()
        {
            /*
            DataTable dr1 = BBPTestHarness.IPFS.GetVideoDataTableRetired();
            for (int i = 0; i < dr1.Rows.Count; i++)
            {
                //string sql = "Select * from social.Wo_Movies where description <> ''";
                try
                {
                    Video v = new Video();
                    v.Title = dr1.Rows[i]["name"].ToString();
                    if (v.Title.Length > 999)
                        v.Title = v.Title.Substring(0, 999);

                    v.Description = dr1.Rows[i]["description"].ToString();
                    if (v.Description.Length > 999)
                        v.Description = v.Description.Substring(0, 999);

                    v.Category = dr1.Rows[i]["genre"].ToString();
                    v.Duration = dr1.Rows[i]["duration"].ToInt32();
                    v.Cover = dr1.Rows[i]["cover"].ToString();
                    v.Source = dr1.Rows[i]["iframe"].ToString();
                    v.id = dr1.Rows[i]["id"].ToString();
                    v.Save(false);
                }
                catch(Exception ex)
                {
                    bool f999 = false;
                }
            }
            */
            return "7";
            
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
            s.BMS_VERSION = BMS_VERSION;
            s.COMMON_VERSION = BMSCommon.Common.BMSCOMMON_VERSION;

            BMSCommon.CryptoUtils.Block blockProd = BMSCommon.BitcoinSync.GetBestBlock(false);
            BMSCommon.CryptoUtils.Block blockTest = BMSCommon.BitcoinSync.GetBestBlock(true);

            if (blockProd != null)
            {
                s.Block_Count_Main = blockProd.BlockNumber;
                s.Best_Block_Hash_Main = blockProd.Hash;
            }
            if (blockTest != null)
            {
                s.Block_Count_Test = blockTest.BlockNumber;
                s.Best_Block_Hash_Test = blockTest.Hash;
            }

            s.Memory_Pool_Count = BMSCommon.API.dMemoryPool.Count;
            s.Status = "SUFFICIENT";
            s.URL = sBindURL;
            s.EOF = "<EOF>";
            if (false)
                            s.RPCErrorList = WebRPC.listRPCErrors;

            try
            {
                s.TESTNET_RPC_HEIGHT = BMSCommon.WebRPC.GetHeight(true);
                s.MAINNET_RPC_HEIGHT = BMSCommon.WebRPC.GetHeight(false);
            }
            catch(Exception)
            {

            }
            string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(s, Newtonsoft.Json.Formatting.Indented);
            return sJson;
        }
        [Route("BMS/Scratch/{id}")]
        public string Scratch()
        {
            string id = Request.RouteValues["id"].ToString() ?? "";

            string value = BMSCommon.Pricing.GetKeyValue("scratch_" + id, (60 * 60 * 10));

            BMSCommon.Pricing.SetKeyValue("scratch_" + id, "ACCESSED");

            string sOut = "<scratch>" + value + "</scratch>\r\n<EOF>\r\n";
            return sOut;
        }

        [Route("BMS/POSE")]
        public string POSE()
        {
            // We need to ensure MySQL is installed and synced too, somehow, in POSE return...
            // MISSION CRITICAL!
            string sPOSE = BBPTestHarness.POSE.GetPerformanceReport();
            sPOSE += "\r\n|v1.0|Status: SUFFICIENT\n|\r\n<EOF>\n";
            return sPOSE;
        }

        [Route("BMS/U1")]
        public async Task<string> U1()
        {
            //bool f = await PortfolioBuilder.DailyUTXOExport(true);
            return "";
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

