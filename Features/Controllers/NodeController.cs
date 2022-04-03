using BiblePay.BMS.DSQL;
using BMS;
using LiteDB;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using static BiblePay.BMS.Common;
using static BiblePay.BMS.DSQL.Metrics;


namespace BiblePay.BMS.Controllers
{
    //This is called with : Localhost:5000/BMS/Welcome

    public class BMSController : Controller
    {

        [Route("Home/Welcome")]
        [Route("Home/Welcome/{id?}")]
        public string Welcome()
        {
            return "Welcome ...<EOF></HTML></html>\r\n\r\n";
        }


        public struct UnchainedReply
        {
            public string error;
            public string URL;
            public int result;
            public double version;
        }

        [Route("api/web/bbpingress")]
        [HttpPost]
        [RequestSizeLimit(5500000000)]
        [RequestFormLimits(MultipartBodyLengthLimit = Int32.MaxValue)]
        public async Task<IActionResult> Index(List<IFormFile> file)
        {
            // This is the place we accept new mp4s, etc.
            UnchainedReply u = new UnchainedReply();
            try
            {
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

                var key = Request.Headers["key"].ToString();
                string immutable = Request.Headers["immutable"].ToString();
                string sNN = "";
                string sUID = BBPTestHarness.Service.ValidateKey(key,out sNN);
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
                    sDestinationPrefix = sUID + "/";    
                }

                string sDestinationURL = sDestinationPrefix + Request.Headers["url"].ToString();
                Log("Ingress::Key = " + key.ToString() + ",URL = " + sDestinationURL);

                var postedFile = file[0];
                string sDestFolder = GetFolder("");
                string sFullDest = Path.Combine(sDestFolder, sDestinationURL);
                if (sFullDest.Contains("..") || sDestinationURL.Contains(".."))
                {
                    throw new Exception("IO Corruption error 03232022::" + sFullDest + "::" + sDestinationURL);
                }    
                string sDestDirOnly = API.ChopLastOctetFromPath(sFullDest);
        
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
                    Common.Log("Starting ingress write " + filePath);
                    using (var stream = new FileStream(sFullDest, System.IO.FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                    System.IO.FileInfo fi = new FileInfo(sFullDest);
                    Log("BBPIngress_2::Writing " + sFullDest + ", sz = " + fi.Length.ToString());
                    if (fi.Length > 0)
                    {
                        bool fOK = await BBPTestHarness.Service.RegisterPin(sDestinationURL,sUID,sFullDest);
                    }
                    // Mission critical: Persist destination in Pins table for future charges...
                    // Send this Tx to biblepay here
                    //string sBurnAddress = BiblePayCommon.Encryption.GetBurnAddress(false);
                    //string sPub = BiblePayDLL.Sidechain.GetPubKeyFromPrivKey(false, key);
                    //string sPayload = "<sc><objtype>immutable</objtype><url>" + sURL + "</url></sc>";
                    //DACResult r0 = BiblePayDLL.Sidechain.CreateFundingTransaction2(false, nAmt, sBurnAddress, key, sPayload);
                    u.URL = GetCDN() + "/" + sDestinationURL;
                }
                u.version = 1.2;
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);

                var r3 = Ok(new { sJson3 });
                return r3;
            }
            catch(Exception ex)
            {
                Log("BBPIngress:Bad file post error::" + ex.Message);
                u.error = "Ingress::BadFileError::"+ex.Message;
                string sJson3 = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                var r3 = Ok(new { sJson3 });
                return r3;
            }
        }



        [Route("BMS/Status")]
        public string Status()
        {
            /*
            c.Props.TotalObjects = Metrics.nObjects;
            c.Props.Latency = Math.Round(Metrics.Latency() * 1000, 2) + "ms";
            c.Props.PeerCount = Metrics.nPeerCount;
            c.Props.Connections = Metrics.Connections;
            c.Props.SpaceUsed = Metrics.nSpaceUsed;
            *.
            */
            string sVersion = BMS_VERSION.ToString() + "|" + "0|v1.2|Status: OK\n|\r\n<EOF>\r\n";
            return sVersion;
        }


        [Route("BMS/POSE")]
        public string POSE()
        {
            string sPOSE = BBPTestHarness.IPFS.GetPerformanceReport();
            sPOSE += "\r\n|v1.0|Status: OK\n|\r\n<EOF>\n";
            return sPOSE;
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
            catch(Exception ex)
            {
                Log(ex.Message);
                return "-1";
            }
        }
    }
}

