
using BBPAPI;
using BMSCommon.Models;
using BMSShared;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.Common;
using static BMSCommon.Extensions;
using BMSCommon.Model;
using static BMSCommon.Model.BitcoinSyncModel;

namespace BMSCommon
{
    public static class BitcoinSync
    {
        // Chain Params
        public static int POW_TARGET_SPACING = 60 * 1;  // 1 minute blocks
        public static Dictionary<string, BitcoinSyncBlock> mapBlockIndex = new Dictionary<string, BitcoinSyncBlock>();
        public static BitcoinSyncBlock GenesisBlock = null;
        public static int MAX_TRANSACTIONS_PER_BLOCK = 2500;


        public static void CheckDatabase()
        {
            //Busy wait while web server warms up
            for (int i = 0; i < 30; i++)
            {
                {
                    System.Threading.Thread.Sleep(1000);
                    string n = Global.msContentRootPath;
                    if (n != null)
                        break;
                }
            }
            string sNarr = "BMS Base content path=" + Global.msContentRootPath;
        }
        
        // BCSync() has temporarily been retired.
        // It stores each block and transaction (similar to a block explorer in cockroachdb).
        // It also allows us to store data in the BBP chain in a deterministic way.
        // However we are currently storing files in Storj, and metadata in cockroach.
        public static void BCSync()
        {
            try
            {
                CheckDatabase();
            }
            catch (Exception ex)
            {
                Log("Mine:" + ex.Message);
            }
            // Block Sync Main Entry Point
            int nStartTime = UnixTimestamp();
            int nNewBlockTime = UnixTimestamp();
            bool fPrimary = BBPAPI.Service.IsPrimary();
            return;
            /*
            while (true)
            {
                try
                {
                    BitcoinSync.SyncBlocks(false);
                    //WebRPC.LogRPCError("Looping " + DateTime.Now.ToString());
                }
                catch (Exception ex)
                {
                    Log("SyncBlocks::" + ex.Message);
                }
                System.Threading.Thread.Sleep(30000);
            }
            */
        }


        public static string GetInsertTransaction(bool fTestNet, BitcoinSyncTransaction t)
        {
            string sEscaped = t.Data.Replace("'", "\'");
            string sTable = fTestNet ? "ttransactionsync" : "transactionsync";
            string sql = "Delete from " + sTable + " where hash='" + t.GetHash() + "';\r\nInsert into " + sTable + " (hash,time,blockhash,height,data,Added) values ('" + t.GetHash() + "','"
                + t.Time.ToString() + "','" + t.BlockHash + "','" + t.Height.ToString() + "','" + sEscaped + "',now());";
            return sql;
        }


        // The Bitcoin Sync class syncs blocks and transactions into BMS backed mysql tables (our BMS View).
        public static int GetBestHeight(bool fTestNet)
        {
            string sChain = fTestNet ? "tblocksync" : "blocksync";
            string sql = "Select max(BlockNumber) h from " + sChain + ";";
            NpgsqlCommand m1 = new NpgsqlCommand(sql);
            double nBlock = 0;// DB.GetScalarDouble(m1, "h");
            return (int)nBlock;
        }
        private static int nMyLocalCount = 0;
        public static bool SyncBlocks(bool fTestNet)
        {
            if (System.Diagnostics.Debugger.IsAttached && false)
                return false;

            bool fDebug = false;
            try
            {
                int nLowHeight = GetBestHeight(fTestNet) + 1;
                int nMaxHeight = 0;//  WebRPC.GetHeight(fTestNet);  TODO-Redo
                if (nLowHeight < 147500 && fTestNet)
                    nLowHeight = 147500;
                if (nLowHeight < 340200 && !fTestNet)
                    nLowHeight = 340200;

                if (fDebug)
                {
                    nLowHeight = nLowHeight - 90;
                }

                for (int i = nLowHeight; i <= nMaxHeight; i++)
                {
                    BBPNetHeight h = new BBPNetHeight();
                    h.TestNet = fTestNet;
                    h.Height = i;
                    BitcoinSyncBlock b = BBPAPI.Interface.WebRPC.GetBlock(h).Result;
                    if (b != null)
                    {
                        InsertBlock(fTestNet, b);
                    }

                }
                nMyLocalCount++;
                if (fTestNet)
                {
                    if (nMyLocalCount < 5)
                    {
                    }
                }
               
                return true;
            }
            catch (Exception ex)
            {
                Log("syncblocks::" + ex.Message);
                return false;
            }
        }

        /*

        public class SanctuaryProfitability
        {
            public double nInvestmentAmount = 0;
            public double nTurnkeySanctuaryNetROI = 0;
            public double nSanctuaryNetROI = 0;
            public double nSanctuaryGrossROI = 0;
            public double nTurnkeySancGrossROI = 0;
            public double nGrossDailyRewards = 0;
            public double nNetDailyRewards = 0;
            public double nNetDailyTurnkeyRewards = 0;
            public double nBlockSubsidy = 0;
            public int nMasternodeCount = 0;
            public int nHeight = 0;
            public double nUSDRevenuePerMonth = 0;
            public double nUSDBBP = 0;
        }

        public static void GetAnnualProfitLevel(int nType, ref SanctuaryProfitability sp, double nCostUSDPerMonth)
        {
            double nDailyCost = nCostUSDPerMonth / 30 / sp.nUSDBBP;

            double nMonthlyNetProfit = sp.nUSDRevenuePerMonth - nCostUSDPerMonth;
            double nMonthlyGrossProfit = sp.nUSDRevenuePerMonth;

            if (nMonthlyNetProfit < 0 && nType == 1)
                nMonthlyNetProfit = .01;

            double nAnnualNetProfit = nMonthlyNetProfit * 12;
            double nAnnualGrossProfit = nMonthlyGrossProfit * 12;
            if (nType == 0)
            {
                sp.nSanctuaryNetROI = nAnnualNetProfit / sp.nInvestmentAmount * 100;
                sp.nSanctuaryGrossROI = nAnnualGrossProfit / sp.nInvestmentAmount * 100;
                sp.nNetDailyRewards = sp.nGrossDailyRewards - nDailyCost;
            }
            else
            {

                sp.nTurnkeySanctuaryNetROI = nAnnualNetProfit / sp.nInvestmentAmount * 100;
                sp.nTurnkeySancGrossROI = nAnnualGrossProfit / sp.nInvestmentAmount * 100;
                sp.nNetDailyTurnkeyRewards = nMonthlyNetProfit / 30;
                sp.nNetDailyTurnkeyRewards = sp.nGrossDailyRewards - nDailyCost;
                if (sp.nNetDailyTurnkeyRewards < 0)
                    sp.nNetDailyTurnkeyRewards = 1;

            }

        }

        */




        public static bool InsertBlock(bool fTestNet, BitcoinSyncBlock b)
        {
            try
            {
                if (b == null)
                    return false;

                bool f = false;
                StringBuilder bIns = new StringBuilder();
                for (int j = 0; j < b.Transactions.Count; j++)
                {
                    bIns.Append(GetInsertTransaction(fTestNet, b.Transactions[j]));
                }
                
                if (bIns.Length > 0)
                {
                    string sTransTable = fTestNet ? "ttransactionsync" : "transactionsync";
                    string sPre = "";
                    string sTrans = "START TRANSACTION; " + sPre + bIns.ToString() + "COMMIT; ";
                    try
                    {
                        NpgsqlCommand m10 = new NpgsqlCommand(sTrans);
                        f = false;// DB.ExuteNonQuery(m10);
                        if (!f)
                        {
                            Log("Problem with " + sTrans);
                            System.Threading.Thread.Sleep(60000);
                        }
                    }
                    catch (Exception ex2)
                    {
                       Log("Problem With InsertBlock::" + ex2.Message);
                    }
                }

                try
                {
                    string sTable = fTestNet ? "tblocksync" : "blocksync";
                    string sql = "Delete from " + sTable + " where hash=@hash;\r\nInsert into " + sTable + " (hash,Version,PreviousBlockHash,NextBlockHash,MerkleRoot,Time,Target,Nonce,BlockNumber,Added) values "
                       + "(@hash, @version, @previousblockhash, @nextblockhash, @merkleroot, @time, @target, @nonce, @blocknumber, now());";
                    NpgsqlCommand cmd1 = new NpgsqlCommand(sql);
                    cmd1.Parameters.AddWithValue("@hash", b.Hash);
                    cmd1.Parameters.AddWithValue("@version", b.Version);
                    cmd1.Parameters.AddWithValue("@previousblockhash", b.PreviousBlockHash);
                    string sNextBlockHash = b.NextBlockHash == null ? "" : b.NextBlockHash;
                    cmd1.Parameters.AddWithValue("@nextblockhash", sNextBlockHash);
                    cmd1.Parameters.AddWithValue("@merkleroot", b.MerkleRoot);
                    cmd1.Parameters.AddWithValue("@time", b.Time);
                    cmd1.Parameters.AddWithValue("@Target", String.Empty);
                    cmd1.Parameters.AddWithValue("@nonce", b.Nonce);
                    cmd1.Parameters.AddWithValue("@blocknumber", b.BlockNumber);
                    f = false;// DB.ExecuteNonQuery(cmd1);
                    if (!f)
                    {
                        return false;
                    }
                    // This is the global hook to insert the Transactions
                    /*
                    if (false)
                    {
                        bool fDB = InsertBiblePayDatabaseTransactions(fTestNet, b);
                    }
                    */

                    // We return 'f' because some tx's dont have database backing;
                    return f;
                }
                catch (Exception ex3)
                {
                    Log("insblock3::" + ex3.Message);
                    return false;
                }
            }
            catch (Exception ex4)
            {
                Log("insBlock4::" + ex4.Message);
                return false;
            }
        }
        public static bool InsertBiblePayDatabaseTransactions(bool fTestNet, BitcoinSyncBlock b)
        {
            string sPrefix = fTestNet ? "t" : "";
            try
            {
                List<string> sObjects = new List<string>();
                for (int i = 0; i < b.Transactions.Count; i++)
                {
                    BitcoinSyncTransaction t = b.Transactions[i];
                    var o = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(t.Data);
                    string sTable = sPrefix + (string)GetEntityValue(o, "table");
                    if (sTable != null)
                    {

                        string sKey = sTable + o.Count.ToString();
                        if (!sObjects.Contains(sKey))
                        {
                            sObjects.Add(sKey);
                            
                        }
                        if (sObjects.Contains(sTable))
                        {

                        }
                    }
                }

                StringBuilder bSQL = new StringBuilder();

                for (int i = 0; i < b.Transactions.Count; i++)
                {
                    BitcoinSyncTransaction t = b.Transactions[i];
                    t.BlockHash = b.Hash;
                    var o = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(t.Data);
                    string sTable = sPrefix + (string)GetEntityValue(o, "table");
                    if (sTable != null && sTable != "")
                    {
                        try
                        {
                            object oPriKeyField = GetEntityValue(o, "PrimaryKey");
                            string sPriKeyField = "";
                            if (oPriKeyField != null)
                                sPriKeyField = (oPriKeyField ?? "").ToString();
                            string sLastPriKeyValue = GetEntityValue(o, sPriKeyField).ToString();
                            if (true)
                            {
                                // Step 1 : Delete exactly by sha256 hash (which removes a previously synced duplicate)
                                string sDelete = "Delete from " + sTable + " where _id='" + t.GetHash() + "';";
                                string sSQL = "";
                                sSQL += sDelete;
                                if (sPriKeyField != null && sPriKeyField != "")
                                {
                                    string sDelete2 = "Delete from " + sTable + " where " + sPriKeyField + "='" + sLastPriKeyValue + "';";
                                    sSQL += sDelete2;
                                }
                                sSQL += ObjToInsert(o, sTable, t.GetHash());
                                bSQL.Append(sSQL);
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            Log("Issue with inserting customized object " + ex.Message);
                        }
                    }
                }
                if (bSQL.Length > 0)
                {
                    string sData = "START TRANSACTION; " + bSQL.ToString() + "COMMIT; ";
                    NpgsqlCommand m1 = new NpgsqlCommand(sData);
                    bool fSuccess = false;//     DB.ExecuteNonQuery(m1);
                    
                }
                return true;
            }
            catch (Exception ex)
            {
                Log("InsBBPDbTransactions::" + ex.Message);
                return false;
            }
        }


        public static object GetEntityValue(Dictionary<string, object> src, string propName)
        {
            try
            {
                foreach (KeyValuePair<string, object> kv in src)
                {
                    if (propName.ToLower() == kv.Key.ToLower())
                    {
                        if (kv.Value == null)
                        {
                            return "";
                        }
                        return kv.Value;
                    }
                }
                return "";
                //return null;
            }
            catch (Exception)
            {
                return "";
            }
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
                string sDBName = "public";

                string sql2 = "select COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA='" + sDBName
                    + "' and TABLE_NAME='" + sTableName.ToLower() + "';";
                NpgsqlCommand cmd2 = new NpgsqlCommand(sql2);
                DataTable dt = null;// DB.GetDataTable(cmd2);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string sFN = dt.Rows[i]["COLUMN_NAME"].ToString();
                    if (sFN.ToLower() == sColName.ToLower())
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log("TCE::" + ex.Message);
                return false;
            }
        }

        public static string ObjToInsert(Dictionary<string, object> o, string sTable, string sGuid)
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
                    Type myType = GetPropertyType1(p.Value);    // info.PropertyType;

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
                                int nMax = sFieldName.ToLower().Contains("data") ? 1000 : 255;
                                if (sValue.Length > nMax)
                                {
                                    sValue = sValue.Substring(0, nMax);  //limit of schema for string...
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
                            objV += "'" + GetDouble(theValue).ToString() + "',";
                        }
                        else
                        {
                            objV += "'" + (string)theValue + "',";
                        }
                    }
                }

                objV = Mid(objV, 0, objV.Length - 1);
                objF = Mid(objF, 0, objF.Length - 1);
                string sSql = string.Format(string.Format(query, sTable, objF, objV)) + ";";
                return sSql;
            }
            catch (Exception ex)
            {
                string sTest = ex.Message;
                return String.Empty;
            }
        }




        public static string ReflectionTypeToMySqlType(string sName, string sPropType)
        {
            if (sPropType == "System.Int32" || sPropType == "System.Int64" || sPropType == "System.Double")
            {
                return "float";
            }
            if (sName.ToLower().Contains("data"))
            {
                return "varchar(1000)";
            }
            return "varchar(255)";
        }


        public static string GetPropertyType(object p)
        {
            try
            {
                string sPropType = p.GetType().ToString();
                return sPropType;
            }
            catch (Exception)
            {
                return "System.String";
            }
        }

        public static Type GetPropertyType1(object p)
        {
            try
            {
                Type m = p.GetType();
                return m;
            }
            catch (Exception)
            {
                return typeof(String);
            }
        }
        private static bool EstablishSchema(bool fTestNet, Dictionary<string, object> o)
        {
            string sPrefix = fTestNet ? "t" : "";

            try
            {
                string sDBName = "";// DB.GetDatabaseName();
                string sTableName = sPrefix + (string)GetEntityValue(o, "table");
                if (sTableName == null || sTableName == String.Empty)
                    return false;

                bool fTableExists = true;// DB.TableExists(sDBName, sTableName);
                if (!fTableExists)
                {
                    string sql = "Create table " + sTableName + " (_id varchar(64) primary key);";
                    NpgsqlCommand cmd = new NpgsqlCommand(sql);
                    //DB.ExecuteNonQuery(cmd);
                }

                foreach (KeyValuePair<string, object> p in o.ToList())
                {
                    string sFieldName = p.Key;
                    object sFieldValue = p.Value;

                    string sPropType = GetPropertyType(p.Value);
                    string sFieldType = ReflectionTypeToMySqlType(sFieldName, sPropType);
                    if (!TableColumnExists(sTableName, sFieldName))
                    {
                        if (sFieldName != "table")
                        {
                            string sql = "ALTER TABLE " + sTableName + " add " + sFieldName + " " + sFieldType + ";";
                            NpgsqlCommand cmd = new NpgsqlCommand(sql);
                            bool fOK = false;//  DB.ExecuteNonQuery(cmd);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.Log("EstSchema::" + ex.Message);
                return false;
            }
        }


        public static string GetSQLTemplate(string sName)
        {
            string sLoc = System.IO.Path.Combine(Global.msContentRootPath, "wwwroot/templates/" + sName);
            string data = System.IO.File.ReadAllText(sLoc);
            return data;
        }

        private static string EncloseInTicks(string list)
        {
            string[] s = list.Split(",");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                sb.Append("'" + s[i] + "',");
            }
            string sOut = sb.ToString();
            if (sOut.Length > 0)
            {
                sOut = sOut.Substring(0, sOut.Length - 1);
            }
            return sOut;
        }
        private static BitcoinSyncTransaction DeserializeTransaction(BitcoinSyncBlock b, DataRow r)
        {
            BitcoinSyncTransaction t = new BitcoinSyncTransaction();
            t.Data = r.Field<string>("Data");
            t.Height = r.Field<int>("height");
            t.Time = r.Field<int>("time");
            return t;
        }

        private static BitcoinSyncBlock DeserializeBlock(DataRow r)
        {
            BitcoinSyncBlock b = new BitcoinSyncBlock();
            b.BlockNumber = r.ToInt("blocknumber");
            b.MerkleRoot = r.Field<string>("MerkleRoot");
            b.Nonce = (long)r["nonce"].ToString().ToDouble();
            b.PreviousBlockHash = r.Field<string>("PreviousBlockHash");
            b.Target = ToBigInteger(r.Field<string>("Target"));
            b.Hash = r.Field<string>("Hash");
            b.Time = (int)r["Time"].ToString().ToDouble();
            b.Version = (int)r["Version"].ToString().ToDouble();
            return b;
        }

        public static BitcoinSyncBlock GetBestBlock(bool fTestNet)
        {
            string sTable = fTestNet ? "tblocksync" : "blocksync";
            string sql = "Select * from " + sTable + " where blocknumber = (Select max(blocknumber) from " + sTable + ");";
            NpgsqlCommand m1 = new NpgsqlCommand(sql);
            DataTable dt = null;// DB.GetDataTable(m1);
            BitcoinSyncBlock b = new BitcoinSyncBlock();
            if (dt.Rows.Count < 1)
                return b;
            b = DeserializeBlock(dt.Rows[0]);
            return b;
        }

        /*
        public static string AddToMemoryPool3(bool fTestNet, BitcoinSyncTransaction t)
        {   //Step 1 : Insert locally
            BitcoinSyncBlock b = new BitcoinSyncBlock();
            b.Transactions.Add(t);
            InsertBiblePayDatabaseTransactions(fTestNet, b);

            // Step 2: Relay
            string sData = t.Data;
            string sPrivKey = GetConfigKeyValue("ivKey");
            string sTXID = WebRPC.PushChainData2(fTestNet, "DATA", sData, sPrivKey);
            if (sTXID == "")
            {
                throw new Exception("Unabled to add to memory pool.");
            }
            return sTXID;
        }
        */

    }
}
