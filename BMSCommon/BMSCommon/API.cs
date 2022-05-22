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

        public static bool AmISynced()
        {
            CryptoUtils.Block b = CryptoUtils.GetBestBlock();
            if (b == null) return false;
            int elapsed = Common.UnixTimestamp() - b.Time;
            bool fSynced = elapsed < (60 * 60);
            return fSynced;
        }

        public static int GetTheirBlockCount(string sNodeURL)
        {
            string sEP = sNodeURL + "/BMS/GetBlockCount";
            string sData = BMSCommon.Common.ExecuteMVCCommand(sEP,5);
            double d = Common.GetDouble(sData);
            return (int)d;
        }

        public static int GetTheirMemPoolCount(string sNodeURL)
        {
            string sEP = sNodeURL + "/BMS/GetMemPoolCount";
            string sData = BMSCommon.Common.ExecuteMVCCommand(sEP,5);
            double d = Common.GetDouble(sData);
            return (int)d;
        }

        public static void Rollback(CryptoUtils.Block b)
        {
            string sHash = b.GetBlockHash();
            string sql = "Delete from transactions where blockhash='" + sHash + "';";
            sql += "Delete from blocks where hash='" + sHash + "';";
            MySqlCommand m1 = new MySqlCommand(sql);
            bool FOK = Database.ExecuteNonQuery(false, m1, "");

            for (int i = 0; i < b.Transactions.Count; i++)
            {
                try
                {
                    API.mapTransactions.Remove(b.Transactions[i].GetHash());
                }
                catch (Exception)
                {

                }
            }
            try
            {
                CryptoUtils.mapBlockIndex.Remove(sHash);
            }
            catch(Exception ex)
            {
                Common.Log("Rollback " + ex.Message);
            }
        }

        public static int PerformRollback()
        {
            double nAvgHeight = 0;
            double nTotalHeight = 0;
            double nCounts = 0;
            int nRolledBack = 0;
            for (int i = 0; i < mNodes.Count; i++)
            {
                Node n = mNodes[i];
                if (!n.IsMine && n.FullyQualified)
                {
                    int nMyCount = API.GetBlockCount();
                    int nCt = GetTheirBlockCount(n.URL);
                    if (nCt > 0)
                    {
                        nCounts++;
                        nTotalHeight += nCt;
                    }
                }
            }
            if (nCounts > 0)
            {
                nAvgHeight = nTotalHeight / nCounts;
                int nMyCount = API.GetBlockCount();
                Common.Log("Counts " + nCounts.ToString() + ", " + nMyCount.ToString() + ", avg = " + nAvgHeight.ToString());
                if (nMyCount < nAvgHeight && nAvgHeight > 1 && nMyCount > 10 & nAvgHeight > nMyCount - 10)
                {
                    // stuck node, rollback the chain.
                    string sHash = GetBestBlockHash();
                    CryptoUtils.Block b99 = CryptoUtils.GetBestBlock();
                    Rollback(b99);
                    nRolledBack++;
                    Common.Log("Rolled back");
                }
            }
            else
            {
                Common.Log("nCounts==0");
            }
            return nRolledBack;
        }


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
                            string sBestPrior = API.GetBestBlockHashPrior();
                            var task = Request_GetBlocks(n.URL, sBestPrior);
                            List<CryptoUtils.Block> l = task.Result;
                            fBlocks = true;
                            if (l != null)
                            {
                                for (int j = 0; j < l.Count; j++)
                                {
                                    CryptoUtils.Block b = l[j];
                                    Common.Log("Connecting block " + j.ToString());
                                    bool fConnected = API.ConnectBlock(b,false);
                                }
                            }
                            Common.Log("Finished");
                        }
                        if (nMemPoolCt > nMyMemPoolCt && !fBlocks)
                        {
                            var task2  = Request_GetMemPool(n.URL);
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
            }catch(Exception ex)
            {
                Common.Log("FromNetwork::" + ex.Message);
                return false;
            }
        }

        public static bool TxExistsInMapBlockIndex(string hash)
        {
            try
            {
                bool fExists = API.mapTransactions.Contains(hash);
                return fExists;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool BlockTxExistsInMapBlockIndex(CryptoUtils.Block b)
        {
            for (int i = 0; i < b.Transactions.Count; i++)
            {
                bool fExists = TxExistsInMapBlockIndex(b.Transactions[i].GetHash());
                if (fExists)
                    return true;
            }
            return false;
        }

        public static bool ConnectBlock(CryptoUtils.Block b,bool fRelayToOthers)
        {
            try
            {
                // Ensure the Block Transactions are not already in another block
                bool fExists =  BlockTxExistsInMapBlockIndex(b);
                if (fExists)
                {
                    return false;
                }
                // Ensure block meets difficulty
                bool f = CryptoUtils.CheckProofOfWork(b);
                if (!f)
                {
                    Common.Log("ConnectBlock::ERROR::POW Failed for " + b.GetBlockHash());
                    return false;
                }

                bool fMerkle = CryptoUtils.CalculateMerkleRoot(b) == b.MerkleRoot;
                if (!fMerkle)
                {
                    Common.Log("ConnectBlock::ERROR::Merkle root failed for " + b.GetBlockHash());
                    return false;
                }
                if (b.PreviousBlockHash == null)
                {
                    Common.Log("ConnectBlock::ERROR:PreviousBlockHash is null.");
                    return false;

                }

                // Dont worry if the block doesnt connect:
                CryptoUtils.Block bAncestor = CryptoUtils.GetBlock(b.PreviousBlockHash);

                if (true)
                {
                    if (bAncestor == null)
                    {
                        Common.Log("ConnectBlock::ERROR::Ancestor does not exist for " + b.GetBlockHash());
                        return false;
                    }
                }

                // Does another block already connect to previousblockhash?
                CryptoUtils.Block bAnyBlock = CryptoUtils.GetBlockByPreviousBlockHash(b.PreviousBlockHash);
                if (bAnyBlock != null)
                {
                    Common.Log("ConnectBlock::ERROR::Already have a block pointing to the prior block for " + b.GetBlockHash());
                    return false;
                }

                // If the Ancestor is already connected to something...
                if (bAncestor != null && bAncestor.NextBlockHash != null && bAncestor.NextBlockHash != b.GetBlockHash())
                {
                    Common.Log("ConnectBlock::ERROR::Ancestor already connects to block " + bAncestor.NextBlockHash);
                    return false;
                }

                if (b.Transactions.Count == 0)
                {
                    Common.Log("ConnectBlock::ERROR::Tx count == 0 for " + b.GetBlockHash());
                    return false;
                }

                if (CryptoUtils.mapBlockIndex.ContainsKey(b.GetBlockHash()))
                {
                    Common.Log("ConnectBlock::ERROR::Already in mapblockindex.");
                    return false;
                }
                // Prune the memory pool
                for (int i = 0; i < b.Transactions.Count; i++)
                {
                    try
                    {
                        if (dMemoryPool.ContainsKey(b.Transactions[i].GetHash()))
                        {
                            dMemoryPool.Remove(b.Transactions[i].GetHash());
                        }
                    }
                    catch(Exception ex2)
                    {

                    }
                }

                bool fSuccess = b.AddBlockIndex(false);
                if (!fSuccess)
                    return false;

                
                CryptoUtils.Block bPrevHash = CryptoUtils.GetBlockByPrevBlockHash(b.PreviousBlockHash);
                if (bPrevHash == null)
                    return false;
                b.BlockNumber = bPrevHash.BlockNumber + 1;
                try
                {
                    API.InsertBlock(b);
                }
                catch(Exception ex)
                {
                    Common.Log("MISSIONCRITICAL::CONNECTBLOCK::" + ex.Message);
                    return false;
                }

                if (fRelayToOthers)
                {
                    RelayBlock(b);
                    //Common.Log("Relaying Block " + b.GetBlockHash());
                }
                return true;
            }
            catch(Exception ex)
            {
                Common.Log("ConnectBlock::" + ex.Message);
                return false;
            }
        }

        public static bool fNeedsRelayed = false;
        public static bool AddToMemoryPool(CryptoUtils.Transaction tx, bool fRelayToOthers)
        {
            string sHash = tx.GetHash();
            if (dMemoryPool.ContainsKey(sHash))
                return false;
            bool fExists = TxExistsInMapBlockIndex(sHash);
            if (fExists)
                return false;

            dMemoryPool.Add(sHash, tx);
            // Relay
            if (fRelayToOthers)
                fNeedsRelayed = true;

            return true;
        }
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


        public static string GetCDN()
        {
            string sCDN = "https://globalcdn.biblepay.org:" + DEFAULT_PORT.ToString();
            return sCDN;
        }

        public static int GetNetworkHeight()
        {
            string sURL = GetCDN() + "/GetBestBlockHeight";
            string sData = Common.ExecuteMVCCommand(sURL,5);
            double d = Common.GetDouble(sData);
            return (int)d;
        }

        // Wrappers

        public static bool LegacyInsertTransaction(CryptoUtils.Transaction t)
        {
            string sql = "Insert into transactions (hash,time,blockhash,height,data,Added) values (@hash, @time, @blockhash, @height, @data, now());";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            cmd1.Parameters.AddWithValue("@hash", t.GetHash());
            cmd1.Parameters.AddWithValue("@time", t.Time);
            cmd1.Parameters.AddWithValue("@blockhash", t.BlockHash);
            cmd1.Parameters.AddWithValue("@height", t.Height);
            cmd1.Parameters.AddWithValue("@data", t.Data);
            bool f = Database.ExecuteNonQuery(false, cmd1, "");
            return f;
        }

        public static string GetInsertTransaction(CryptoUtils.Transaction t)
        {
            string sEscaped = t.Data.Replace("'", "\'");
            string sql = "Insert into transactions (hash,time,blockhash,height,data,Added) values ('" + t.GetHash() + "','" 
                + t.Time.ToString() + "','" + t.BlockHash + "','" + t.Height.ToString() + "','" + sEscaped + "',now());";
            return sql;
        }

        public class FakeTx
        {
            public string field1 = "";
            public string field2 = "";
            public int me_id = 0;
            public string table = "FakeTx";
            public double nMeID = 1;
        }

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
        public static object xGetEntityValue(object src, string propName)
        {
            try
            {
                object value = src.GetType().GetProperty(propName).GetValue(src, null);
                return value;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static object GetEntityValue(Dictionary<string,object> src, string propName)
        {
             foreach (KeyValuePair<string, object> kv in src)
                {
                    if (propName.ToLower() == kv.Key.ToLower())
                    {
                        return kv.Value;
                    }
                }
                return null;
        }
        public static void SetEntityValue(object o, string sColName, object oNewValue)
        {
            PropertyInfo propertyInfo = o.GetType().GetProperty(sColName);
            if (propertyInfo != null)
            {
                string sPropType = propertyInfo.PropertyType.ToString();
                if (sPropType == "System.Int32" || sPropType == "System.Int64" || sPropType == "System.Double")
                {
                    if (oNewValue.ToString() == "")
                        oNewValue = 0;
                    // If they are sending a BOOL into an int32/int64 field:
                    if (oNewValue.ToString().ToLower() == "false" || oNewValue.ToString().ToLower() == "true")
                    {
                        oNewValue = Convert.ToBoolean(oNewValue);
                    }
                }
                if (sPropType == "System.Boolean")
                {
                    if (oNewValue.ToString() == "")
                        oNewValue = false;
                }
                if (sPropType == "System.DateTime")
                {
                    oNewValue = Convert.ToDateTime(oNewValue);
                }
                propertyInfo.SetValue(o, Convert.ChangeType(oNewValue, propertyInfo.PropertyType), null);
            }
        }

        public static bool TableColumnExists(string sTableName, string sColName)
        {
            try
            {
                string sDBName = Database.GetDatabaseName();

                string sql2 = "select COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA='" + sDBName + "' and TABLE_NAME='" + sTableName + "';";
                MySqlCommand cmd2 = new MySqlCommand(sql2);
                DataTable dt = Database.GetMySqlDataTable(false, cmd2, "");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string sFN = dt.Rows[i]["COLUMN_NAME"].ToString();
                    if (sFN.ToLower() == sColName.ToLower())
                        return true;
                }
                return false;
            }catch(Exception ex)
            {
                Common.Log("TCE::" + ex.Message);
                return false;
            }
        }

        private static string ObjToInsert(Dictionary<string,object> o, string sTable, string sGuid)
        {

            string query = "INSERT INTO {0} (_id,{1}) VALUES ('" + sGuid + "',{2})";
            try
            {
                string sHiddenFields = "ReceivablexPrivateKey;ERxCxQuantity;Redemptionx;";
                var objV = "";
                string sFieldName = "";
                var objF = "";
                bool fSkip = false;
                foreach (KeyValuePair<string, object> p in o.ToList())
                {
                    Type myType = p.Value.GetType();    // info.PropertyType;
                    sFieldName = p.Key;                 // info.Name;
                    object theValue = p.Value;          // info.GetValue(p, null);
                    fSkip = false;
                    if (sFieldName == "table" || sHiddenFields.Contains(sFieldName))
                        fSkip = true;

                    if (!fSkip)
                    {
                        objF += sFieldName + ",";

                        if (myType == typeof(string))
                        {
                            if (theValue == null)
                            {
                                objV += "null,";
                            }
                            else
                            {

                                string sValue = (string)theValue; // info.GetValue(p, null);
                                if (sValue.Length > 255)
                                {
                                    sValue = sValue.Substring(0, 255);  //limit of schema for string...
                                }
                                sValue = sValue.Replace("'", "''");
                                objV += "'" + sValue + "',";
                            }
                        }
                        else if (myType == typeof(DateTime))
                        {
                            objV += "'" + Convert.ToDateTime(theValue).ToString("yyyy-MM-dd hh:mm:ss") + "',";
                        }
                        else if (myType == typeof(bool))
                        {
                            objV += "'" + ((bool)theValue == true ? "1" : "0") + "',";
                        }
                        else if (myType == typeof(Int32) || myType == typeof(Int64) || myType == typeof(Double))
                        {
                            objV += "'" + Common.GetDouble(theValue).ToString() + "',";
                        }
                        else
                        {
                            objV += "'" + (string)theValue + "',";
                        }
                    }
                }

                objV = Common.Mid(objV, 0, objV.Length - 1);
                objF = Common.Mid(objF, 0, objF.Length - 1);
                string sSql = string.Format(string.Format(query, sTable, objF, objV)) + ";";
                return sSql;
            }
            catch (Exception ex)
            {
                string sTest = ex.Message;
                return String.Empty;
            }
        }

        public static string ReflectionTypeToMySqlType(string sPropType)
        {
            if (sPropType == "System.Int32" || sPropType == "System.Int64" || sPropType == "System.Double")
            {
                return "float";
            }
            return "varchar(255)";
        }

        public static bool EstablishSchema(Dictionary<string,object> o)
        {
            try
            {
                string sDBName = Database.GetDatabaseName();
                string sTableName = (string)GetEntityValue(o, "table");
                if (sTableName == null || sTableName == "")
                    return false;

                bool fTableExists = Database.TableExists(false, sDBName, sTableName);
                if (!fTableExists)
                {
                    string sql = "Create table " + sTableName + " (_id varchar(64) primary key);";
                    MySqlCommand cmd = new MySqlCommand(sql);
                    Database.ExecuteNonQuery(false, cmd, "");
                }

                foreach (KeyValuePair<string, object> p in o.ToList())
                {
                    string sFieldName = p.Key;
                    object sFieldValue = p.Value;
                    string sPropType = p.Value.GetType().ToString(); // property.PropertyType.ToString();
                    string sFieldType = ReflectionTypeToMySqlType(sPropType);
                    if (!TableColumnExists(sTableName, sFieldName))
                    {
                        if (sFieldName != "table")
                        {
                            string sql = "ALTER TABLE " + sTableName + " add " + sFieldName + " " + sFieldType + ";";
                            MySqlCommand cmd = new MySqlCommand(sql);
                            bool fOK = Database.ExecuteNonQuery(false, cmd, "");
                        }
                    }
                }
            return true;
            }
            catch(Exception ex)
            {
                Common.Log("EstSchema::" + ex.Message);
                return false;
            }
        }
       
        public static bool InsertBiblePayDatabaseTransactions(CryptoUtils.Block b)
        {
            try
            {

                List<string> sObjects = new List<string>();
                for (int i = 0; i < b.Transactions.Count; i++)
                {
                    CryptoUtils.Transaction t = b.Transactions[i];
                    var o = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(t.Data);
                    string sTable = (string)GetEntityValue(o, "table");
                    if (sTable != null)
                    {
                        if (!sObjects.Contains(sTable))
                        {
                            sObjects.Add(sTable);
                            EstablishSchema(o);
                        }
                    }
                }

                StringBuilder bSQL = new StringBuilder();

                for (int i = 0; i < b.Transactions.Count; i++)
                {
                    CryptoUtils.Transaction t = b.Transactions[i];
                    t.BlockHash = b.GetBlockHash();
                    var o = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(t.Data);
                    string sTable = (string)GetEntityValue(o, "table");
                    if (sTable != null)
                    {
                        // Step 1 : Delete exactly by sha256 hash (which removes a previously synced duplicate)
                        string sDelete = "Delete from " + sTable + " where _id='" + t.GetHash() + "';";
                        // Step 2 : Delete by Primary Key 
                        object oPriKeyField = GetEntityValue(o, "PrimaryKey");
                        string sPriKeyField = (oPriKeyField ?? "").ToString();


                        string sSQL = "";
                        sSQL += sDelete;
                        if (sPriKeyField != null && sPriKeyField != "")
                        {
                            string sLastPriKeyValue = GetEntityValue(o, sPriKeyField).ToString();
                            string sDelete2 = "Delete from " + sTable + " where " + sPriKeyField + "='" + sLastPriKeyValue + "';";
                            sSQL += sDelete2;
                        }
                        sSQL += ObjToInsert(o, sTable, t.GetHash());
                        bSQL.Append(sSQL);           
                    }
                }
                if (bSQL.Length > 0)
                {
                    string sData = "START TRANSACTION; " + bSQL.ToString() + "COMMIT; ";
                    MySqlCommand m1 = new MySqlCommand(sData);
                    bool fSuccess = Database.ExecuteNonQuery(false, m1, "");

                    bool f1000 = false;

                }
                return true;
            }
            catch(Exception ex)
            {
                Common.Log("InsBBPDbTransactions::" + ex.Message);
                return false;
            }
        }

        public static bool InsertBlock(CryptoUtils.Block b)
        {
            try
            {
                if (b == null)
                    return false;

                // check merkle
                string sMR = CryptoUtils.CalculateMerkleRoot(b);
                if (b.MerkleRoot != sMR)
                {
                    Common.Log("InserBlock::Bad Merkle::" + b.GetBlockHash());
                    return false;
                }
                bool f = false;
                StringBuilder bIns = new StringBuilder();

                for (int i = 0; i < b.Transactions.Count; i++)
                {
                    CryptoUtils.Transaction t = b.Transactions[i];
                    t.Height = b.BlockNumber;
                    t.BlockHash = b.GetBlockHash();
                    string sData = GetInsertTransaction(t);
                    bIns.Append(sData);
                    // Cant fail here in case we rolled back and resynced
                }
                if (bIns.Length > 0)
                {
                    string sTrans = "START TRANSACTION; " + bIns.ToString() + "COMMIT; ";
                    try
                    {
                        MySqlCommand m10 = new MySqlCommand(sTrans);
                        f = Database.ExecuteNonQuery(false, m10, "");
                    }
                    catch (Exception ex2)
                    {
                        Common.Log("Problem With InsertBlock::" + ex2.Message);
                    }
                }

                string sql = "Select count(*) ct from transactions where blockhash='" + b.GetBlockHash() + "';";
                MySqlCommand m1 = new MySqlCommand(sql);
                double nCt = Database.GetScalarDouble(false, m1, "ct");
                if (nCt < b.Transactions.Count)
                {
                    Common.Log("Tx count does not match for block " + b.GetBlockHash());
                }
                try
                {
                    sql = "Insert into blocks (hash,Version,PreviousBlockHash,MerkleRoot,Time,Target,Nonce,BlockNumber,Transactions,Added) values "
                       + "(@hash, @version, @previousblockhash, @merkleroot, @time, @target, @nonce, @blocknumber, @transactions, now());";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    string hash = b.GetBlockHash();
                    cmd1.Parameters.AddWithValue("@hash", b.GetBlockHash());
                    cmd1.Parameters.AddWithValue("@version", b.Version);
                    cmd1.Parameters.AddWithValue("@previousblockhash", b.PreviousBlockHash);
                    cmd1.Parameters.AddWithValue("@merkleroot", b.MerkleRoot);
                    cmd1.Parameters.AddWithValue("@time", b.Time);
                    cmd1.Parameters.AddWithValue("@target", CryptoUtils.ToHexString(b.Target));
                    cmd1.Parameters.AddWithValue("@nonce", b.Nonce);
                    cmd1.Parameters.AddWithValue("@blocknumber", b.BlockNumber);
                    cmd1.Parameters.AddWithValue("@transactions", b.GetTransactions());
                    f = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!f)
                        return false;
                    // This is the global hook to insert the Transactions
                    bool fDB = InsertBiblePayDatabaseTransactions(b);
                    // We return 'f' because some tx's dont have database backing;
                    return f;
                }
                catch (Exception ex3)
                {
                    Common.Log("insblock3::" + ex3.Message);
                    return false;
                }
            }catch(Exception ex4)
            {
                Common.Log("insBlock4::" + ex4.Message);
                return false;
            }
        }

        public static string GetBestBlockHash()
        {
            BMSCommon.CryptoUtils.Block b = BMSCommon.CryptoUtils.GetBestBlock();
            return b.GetBlockHash();
        }

        public static string GetBestBlockHashPrior()
        {
            BMSCommon.CryptoUtils.Block b = BMSCommon.CryptoUtils.GetBestBlock();
            if (b.IsGenesis)
                return b.GetBlockHash();

            return b.PreviousBlockHash;
        }

        public static int GetBlockCount()
        {
            try
            {
                BMSCommon.CryptoUtils.Block b = BMSCommon.CryptoUtils.GetBestBlock();
                int nHeight = b.BlockNumber;
                return nHeight;
            }catch(Exception ex)
            {
                return 0;
            }
        }
        public static int GetMemPoolCount()
        {
            int nCount = BMSCommon.API.dMemoryPool.Count;
            return nCount;
        }

    }
}
