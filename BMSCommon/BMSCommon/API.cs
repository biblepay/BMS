using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.Data;
using System.Dynamic;
using System.IO;
using static BMSCommon.WebRPC;
using System.Net.Mail;

namespace BMSCommon
{
    public static class API
    {
        public struct Node
        {
            public string URL;
            public string FullyQualifiedDomainName;
            public int ProcessorCount;
            public double RootFolderSize;
            public double HPS;
            public double HardDriveSize;
            public bool IsMine;
            public bool FullyQualified;
            public double ProcessorUtilization;
        }
        public static List<Node> mNodes = new List<Node>();
        public static Dictionary<string, CryptoUtils.Transaction> dMemoryPool = new Dictionary<string, CryptoUtils.Transaction>();
        public static List<string> mapTransactions = new List<string>();

        public static int DEFAULT_PORT = 8443;
        /*
        public static async Task<bool> RelayTransactions()
        {
            List<CryptoUtils.Transaction> l = new List<CryptoUtils.Transaction>();
            foreach (KeyValuePair<string, CryptoUtils.Transaction> t in API.dMemoryPool.ToList())
            {
                l.Add(t.Value);
            }

            for (int i = 0; i < mNodes.Count; i++)
            {
                Node n = mNodes[i];
                if (!n.IsMine && n.FullyQualified)
                {
                    await PushTxToSanc(n.FullyQualifiedDomainName, l);
                }
            }
            return true;
        }
        


        public static async Task<bool> RelayBlock(CryptoUtils.Block b)
        {
            for (int i = 0; i < mNodes.Count; i++)
            {
                Node n = mNodes[i];
                if (!n.IsMine && n.FullyQualified)
                {
                    await PushBlockToSanc(n.FullyQualifiedDomainName, b);
                }

            }
            return true;
        }
        */


        public static int GetTheirBlockCount(string sNodeURL)
        {
            string sEP = sNodeURL + "/BMS/GetBlockCount";
            string sData = BMSCommon.Common.ExecuteMVCCommand(sEP, 5);
            double d = Common.GetDouble(sData);
            return (int)d;
        }

        public static int GetTheirMemPoolCount(string sNodeURL)
        {
            string sEP = sNodeURL + "/BMS/GetMemPoolCount";
            string sData = BMSCommon.Common.ExecuteMVCCommand(sEP, 5);
            double d = Common.GetDouble(sData);
            return (int)d;
        }



        /*

        public static bool GetBlocksFromNetwork()
        {
            // If we are not synced, we loop through the network nodes, and ask for blocks...  
            try
            {
                if (API.fNeedsRelayed)
                {
                    API.fNeedsRelayed = false;
                    RelayTransactions();
                }


                for (int i = 0; i < mNodes.Count; i++)
                {
                    Node n = mNodes[i];
                    if (!n.IsMine && n.FullyQualified)
                    {
                        int nMyCount = API.GetBlockCount();
                        int nCt = GetTheirBlockCount(n.URL);
                        int nMemPoolCt = GetTheirMemPoolCount(n.URL);
                        int nMyMemPoolCt = GetMemPoolCount();
                        bool fBlocks = false;
                        if (nCt > nMyCount)
                        {
                            string sBestPrior = API.kHashPrior();
                            var task = Request_GetBlocks(n.URL, sBestPrior);
                            List<CryptoUtils.Block> l = task.Result;
                            fBlocks = true;
                            if (l != null)
                            {
                                for (int j = 0; j < l.Count; j++)
                                {
                                    CryptoUtils.Block b = l[j];
                                    Common.Log("Connecting block " + j.ToString());
                                    bool fConnected = API.ConnectBlock(b, false);
                                }
                            }
                            Common.Log("Finished");
                        }
                        if (nMemPoolCt > nMyMemPoolCt && !fBlocks)
                        {
                            var task2 = Request_GetMemPool(n.URL);
                            List<CryptoUtils.Transaction> l = task2.Result;
                            if (l != null)
                            {
                                Common.Log("Adding " + l.Count.ToString() + " to memory pool.");
                                for (int j = 0; j < l.Count; j++)
                                {
                                    AddToMemoryPool(l[j], false);
                                }
                            }
                        }
                    }
                }
                return true;
            } catch (Exception ex)
            {
                Common.Log("FromNetwork::" + ex.Message);
                return false;
            }
        }
        */


        /*        
                public async static Task<bool> PushTxToSanc(string sCDN, List<CryptoUtils.Transaction> tx)
                {
                    try
                    {
                        string sEP = sCDN + "/api/web/pushtx";
                        HttpClientHandler clientHandler = new HttpClientHandler();
                        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                        using (var httpClient = new System.Net.Http.HttpClient(clientHandler))
                        {
                            using (var request = new HttpRequestMessage(new HttpMethod("POST"), sEP))
                            {

                                httpClient.Timeout = TimeSpan.FromSeconds(60);
                                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                string data = Newtonsoft.Json.JsonConvert.SerializeObject(tx);
                                string sGuid = Guid.NewGuid().ToString() + ".dat";
                                string sFN = Path.Combine(Path.GetTempPath(), sGuid);
                                System.IO.File.WriteAllText(sFN, data);
                                var multipartContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
                                HttpContent bytesContent = new ByteArrayContent(System.IO.File.ReadAllBytes(sFN));
                                multipartContent.Add(bytesContent, "file", System.IO.Path.GetFileName(sFN));
                                request.Content = multipartContent;
                                // the following line is not good, but OK for debugging:
                                // ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                                var oInitialResponse = await httpClient.PostAsync(sEP, multipartContent);
                                string sJsonResponse = await oInitialResponse.Content.ReadAsStringAsync();
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Common.Log("PTTS::" + ex.Message);
                        return false;
                    }
                }


                public async static Task<List<CryptoUtils.Block>> Request_GetBlocks(string sCDN, string sStartHash)
                {
                    try
                    {
                        string sEP = sCDN + "/api/web/getblocks";
                        HttpClientHandler clientHandler = new HttpClientHandler();
                        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                        using (var httpClient = new System.Net.Http.HttpClient(clientHandler))
                        {
                            httpClient.Timeout = TimeSpan.FromSeconds(15);
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("hash", sStartHash);
                            var oInitialResponse = await httpClient.PostAsync(sEP, null);
                            string sJsonResponse = await oInitialResponse.Content.ReadAsStringAsync();
                            List<CryptoUtils.Block> o1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CryptoUtils.Block>>(sJsonResponse);
                            return o1;
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }

        public async static Task<List<CryptoUtils.Transaction>> Request_GetMemPool(string sCDN)
        {
            try
            {
                string sEP = sCDN + "/api/web/getmempool";
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (var httpClient = new System.Net.Http.HttpClient(clientHandler))
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var oInitialResponse = await httpClient.PostAsync(sEP, null);
                    string sJsonResponse = await oInitialResponse.Content.ReadAsStringAsync();
                    List<CryptoUtils.Transaction> o1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CryptoUtils.Transaction>>(sJsonResponse);
                    return o1;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async static Task<bool> PushBlockToSanc(string sCDN, CryptoUtils.Block b)
        {
            try
            {
                string sEP = sCDN + "/api/web/pushblock";
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (var httpClient = new System.Net.Http.HttpClient(clientHandler))
                {

                    httpClient.Timeout = TimeSpan.FromSeconds(20);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Key", "?");
                    string data = Newtonsoft.Json.JsonConvert.SerializeObject(b);
                    var content = new FormUrlEncodedContent(new[]
                    {
                         new KeyValuePair<string, string>("", data)
                    });
                    var oInitialResponse = await httpClient.PostAsync(sEP, content);
                    string sJsonResponse = await oInitialResponse.Content.ReadAsStringAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        */

        public static string GetCDN()
        {
            string sCDN = "https://globalcdn.biblepay.org:" + DEFAULT_PORT.ToString();
            return sCDN;
        }

        public static int GetNetworkHeight()
        {
            string sURL = GetCDN() + "/GetBestBlockHeight";
            string sData = Common.ExecuteMVCCommand(sURL, 5);
            double d = Common.GetDouble(sData);
            return (int)d;
        }

        // Wrappers


        public class Articles
        {
            public string table = "Articles";
            public string Name;
            public string Description;
            public string Added;
        }

        public class Wiki
        {
            public string table = "Wiki";
            public string Name;
            public string Description;
            public string Added;
        }

        public class Illustrations
        {
            public string table = "Illustrations";
            public string Name;
            public string Description;
            public string URL;
            public string Added;
        }


        public static int GetMemPoolCount()
        {
            int nCount = BMSCommon.API.dMemoryPool.Count;
            return nCount;
        }



    }
}
