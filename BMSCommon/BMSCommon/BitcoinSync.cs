using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.CryptoUtils;
using static BMSCommon.PortfolioBuilder;
using static BMSCommon.Pricing;

namespace BMSCommon
{
    public static class BitcoinSync
    {

        public class OrphanExpense3
        {
            public string Added;
            public double Amount;
            public string URL = "";
            public string Charity;
            public string HandledBy;
            public string ChildID;
            public double Balance;
            public string Notes;
            public int Version = 9;
            public string table = "OrphanExpense3";
        }

        public class Expense
        {
            public string Added;
            public double Amount;
            public string URL = "";
            public string Charity;
            public string HandledBy;
            public string Notes;
            public string table = "Expense";
        }

        public class Revenue
        {
            public string Added;
            public double BBPAmount;
            public double BTCRaised;
            public double BTCPrice;
            public double Amount;
            public string Notes;
            public string HandledBy;
            public string Charity;
            public string table = "Revenue";
        }
        public static double GetChildBalance(string sChildID)
        {
            string sql = "select Balance from bms0.OrphanExpense3 where ChildID=@childid order by STR_TO_DATE(Added,'%m/%d/%Y %h:%i:%s') desc limit 1;";
            MySqlCommand m1 = new MySqlCommand(sql);
            m1.Parameters.AddWithValue("@childid", sChildID);
            double nBal = Database.GetScalarDouble(m1, "Balance");
            return nBal;
        }

        public static string GetInsertTransaction(bool fTestNet, CryptoUtils.Transaction t)
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
            MySqlCommand m1 = new MySqlCommand(sql);
            double nBlock = Database.GetScalarDouble(m1, "h");
            return (int)nBlock;
        }
        private static int nMyLocalCount = 0;
        public static async Task<bool> SyncBlocks(bool fTestNet)
        {
            bool fDebug = false;
            try
            {
                int nLowHeight = GetBestHeight(fTestNet) + 1;
                int nMaxHeight = BMSCommon.WebRPC.GetHeight(fTestNet);
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
                    CryptoUtils.Block b = BMSCommon.WebRPC.GetBlock(fTestNet, i);
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
                        BMSCommon.WebRPC.LogRPCError("Got node testnet height " + nMaxHeight.ToString());
                    }
                }
                await PayDailyTurnkeySanctuaryRewards(fTestNet);

                return true;
            }
            catch(Exception ex)
            {
                BMSCommon.Common.Log("syncblocks::" + ex.Message);
                WebRPC.LogRPCError("syncblocks:" + ex.Message);
                return false;
            }
        }


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

        private static void GetAnnualProfitLevel(int nType, ref SanctuaryProfitability sp, double nCostUSDPerMonth)
        {
            double nDailyCost = nCostUSDPerMonth / 30 / sp.nUSDBBP;

            double nMonthlyNetProfit = sp.nUSDRevenuePerMonth - nCostUSDPerMonth;
            double nMonthlyGrossProfit = sp.nUSDRevenuePerMonth;

            if (nMonthlyNetProfit < 0 && nType==1)
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
        public static SanctuaryProfitability GetMasternodeROI(bool fTestNet)
        {
            // Sanctuary DWU
            SanctuaryProfitability sp = new SanctuaryProfitability();

            sp.nMasternodeCount = WebRPC.GetMasternodeCount(fTestNet);

            sp.nHeight = GetBestHeight(fTestNet) - 1;
            int nRewardQtyPerDay = 205 / sp.nMasternodeCount;
            string sRecipPaid = "";
            WebRPC.GetSubsidy(fTestNet, sp.nHeight, ref sRecipPaid, ref sp.nBlockSubsidy);
            sp.nGrossDailyRewards = nRewardQtyPerDay * sp.nBlockSubsidy;
            price1 nBTCPrice = BMSCommon.Pricing.GetCryptoPrice("BTC");
            price1 nBBPPrice = BMSCommon.Pricing.GetCryptoPrice("BBP");
            sp.nUSDBBP = nBTCPrice.AmountUSD * nBBPPrice.Amount;

            sp.nUSDRevenuePerMonth = sp.nGrossDailyRewards * 30 * sp.nUSDBBP;
            sp.nInvestmentAmount = 4500001 * sp.nUSDBBP;

            GetAnnualProfitLevel(0, ref sp, 3);

            GetAnnualProfitLevel(1, ref sp,1);

            return sp;
        }

        public static double GetAdditionalPortfolioBuilderJuices(bool fTestNet)
        {
            // For those who made pb donations, we divide them by 30 and return the sum, this gives us the boost
            string sTable = fTestNet ? "tpbdonation" : "pbdonation";

            string sql = "Select sum(amount) a from " + sTable + " WHERE TIMESTAMPDIFF(MINUTE, added, now()) < (1440 * 30);";
            MySqlCommand m1 = new MySqlCommand(sql);
            double nAmt = BMSCommon.Database.GetScalarDouble(m1, "a");
            double nJuice = nAmt / 30;
            return nJuice;
        }

        public static async Task<bool> PayDailyTurnkeySanctuaryRewards(bool fTestNet)
        {

            double nCoreBalance = BMSCommon.WebRPC.GetCachedCoreWalletBalance(false);
            bool fPrimary = BMSCommon.Common.IsPrimary();

            if (!fTestNet || nCoreBalance < 256000 || !fPrimary)
                return false;


            bool fLatch = BMSCommon.Pricing.Latch(fTestNet, "turnkeysanctuaryrewards", 60 * 60 * 24);
            if (!fLatch)
                return false;

            SanctuaryProfitability sp = GetMasternodeROI(fTestNet);

            if (sp.nHeight < 1000 || sp.nBlockSubsidy < 50 || sp.nMasternodeCount < 1)
            {
                // Strange
                Common.Log("Strange error occurring in PDTSR:: " + sp.nHeight.ToString() + " with " + sp.nBlockSubsidy.ToString());
                return false;
            }

            string sTable = fTestNet ? "tturnkeysanctuaries" : "turnkeysanctuaries";
            string sql = "Select * from " + sTable + " order by Added;";
            MySqlCommand m1 = new MySqlCommand(sql);
            DataTable dt = Database.GetDataTable(m1);
            List<BMSCommon.WebRPC.Payment> Payments = new List<BMSCommon.WebRPC.Payment>();
            double nTotal = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // Ensure they didnt spend the collateral here
                string sAddress = dt.Rows[i]["BBPAddress"].ToString();
                double nBal = WebRPC.QueryAddressBalanceNewMethod(fTestNet, sAddress);
                if (nBal >= 4500001)
                {
                    double nReward = 1 * sp.nNetDailyTurnkeyRewards;
                    if (nReward > .25)
                    {
                        bool bValid = BMSCommon.WebRPC.ValidateBiblepayAddress(fTestNet, sAddress);
                        if (bValid)
                        {
                            nTotal += nReward;
                            BMSCommon.WebRPC.Payment p = new BMSCommon.WebRPC.Payment();
                            p.bbpaddress = sAddress;
                            p.amount = nReward;
                            Payments.Add(p);
                            Common.Log("Turnkey::Sending out " + nReward.ToString() + " to " + p.bbpaddress + " and daily reward is " + sp.nNetDailyTurnkeyRewards.ToString()
                                + " netdailyturnkeyrew, and " + sp.nGrossDailyRewards + " grossrewards, bbpprice=" + sp.nUSDBBP.ToString());
                        }
                    }
                }
            }
            if (Payments.Count > 0)
            {
                string poolAccount = BMSCommon.Common.GetConfigurationKeyValue("PoolPayAccount");
                if (poolAccount == "")
                    return false;
                string txid = BMSCommon.WebRPC.SendMany(fTestNet, Payments, poolAccount, "PB Payments");
                Common.Log("TurnkeySanctuaryPayments::Sent out qty " + Payments.Count.ToString() + " totaling " + nTotal.ToString() + ".");
                return true;
            }

            return true;
        }


        public static bool InsertBlock(bool fTestNet, CryptoUtils.Block b)
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
                        MySqlCommand m10 = new MySqlCommand(sTrans);
                        f = Database.ExecuteNonQuery(m10);
                        if (!f)
                        {
                            Common.Log("Problem with " + sTrans);
                            System.Threading.Thread.Sleep(60000);
                        }
                    }
                    catch (Exception ex2)
                    {
                        Common.Log("Problem With InsertBlock::" + ex2.Message);
                    }
                }

                /*
                string sql = "Select count(*) ct from transactions where blockhash='" + b.Hash + "';";
                MySqlCommand m1 = new MySqlCommand(sql);
                double nCt = Database.GetScalarDouble(false, m1, "ct");
                if (nCt < b.Transactions.Count)
                {
                    Common.Log("Tx count does not match for block " + b.Hash);
                }
                */



                try
                {
                    string sTable = fTestNet ? "tblocksync" : "blocksync";
                    string sql = "Delete from " + sTable + " where hash=@hash;\r\nInsert into " + sTable + " (hash,Version,PreviousBlockHash,NextBlockHash,MerkleRoot,Time,Target,Nonce,BlockNumber,Added) values "
                       + "(@hash, @version, @previousblockhash, @nextblockhash, @merkleroot, @time, @target, @nonce, @blocknumber, now());";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    cmd1.Parameters.AddWithValue("@hash", b.Hash);
                    cmd1.Parameters.AddWithValue("@version", b.Version);
                    cmd1.Parameters.AddWithValue("@previousblockhash", b.PreviousBlockHash);
                    cmd1.Parameters.AddWithValue("@nextblockhash", b.NextBlockHash);

                    cmd1.Parameters.AddWithValue("@merkleroot", b.MerkleRoot);
                    cmd1.Parameters.AddWithValue("@time", b.Time);
                    cmd1.Parameters.AddWithValue("@nonce", b.Nonce);
                    cmd1.Parameters.AddWithValue("@blocknumber", b.BlockNumber);
                    f = Database.ExecuteNonQuery(cmd1);
                    if (!f)
                    {
                        return false;
                    }
                    // This is the global hook to insert the Transactions
                    bool fDB = InsertBiblePayDatabaseTransactions(fTestNet,b);
                    // We return 'f' because some tx's dont have database backing;
                    return f;
                }
                catch (Exception ex3)
                {
                    Common.Log("insblock3::" + ex3.Message);
                    return false;
                }
            }
            catch (Exception ex4)
            {
                Common.Log("insBlock4::" + ex4.Message);
                return false;
            }
        }
        public static bool InsertBiblePayDatabaseTransactions(bool fTestNet, CryptoUtils.Block b)
        {
            string sPrefix = fTestNet ? "t" : "";
            try
            {
                List<string> sObjects = new List<string>();
                //Dictionary<string, int> dObjects= new Dictionary<string, int>();

                for (int i = 0; i < b.Transactions.Count; i++)
                {
                    CryptoUtils.Transaction t = b.Transactions[i];
                    var o = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(t.Data);
                    string sTable = sPrefix + (string)GetEntityValue(o, "table");
                    if (sTable != null)
                    {
                        if (sTable == "tuser")
                        {
                            bool f1101 = false;
                        }

                        string sKey = sTable + o.Count.ToString();
                        if (!sObjects.Contains(sKey))
                        {
                            sObjects.Add(sKey);
                            EstablishSchema(fTestNet, o);
                        }
                        if (sObjects.Contains(sTable))
                        {

                        }
                    }
                }

                StringBuilder bSQL = new StringBuilder();

                for (int i = 0; i < b.Transactions.Count; i++)
                {
                    CryptoUtils.Transaction t = b.Transactions[i];
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
                            string sError = "";
                            bool fApproved = ApproveTransaction(sTable, sPriKeyField, sLastPriKeyValue, t, out sError);
                            if (fApproved)
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
                            else
                            {
                                Console.WriteLine("not approved " + sLastPriKeyValue);

                            }
                        }
                        catch (Exception ex)
                        {
                            BMSCommon.Common.Log("Issue with inserting customized object " + ex.Message);
                        }
                    }
                }
                if (bSQL.Length > 0)
                {
                    string sData = "START TRANSACTION; " + bSQL.ToString() + "COMMIT; ";
                    MySqlCommand m1 = new MySqlCommand(sData);
                    bool fSuccess = Database.ExecuteNonQuery(m1);
                    bool f1000 = false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.Log("InsBBPDbTransactions::" + ex.Message);
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
            catch (Exception ex)
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
                string sDBName = Database.GetDatabaseName();

                string sql2 = "select COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA='" + sDBName + "' and TABLE_NAME='" + sTableName + "';";
                MySqlCommand cmd2 = new MySqlCommand(sql2);
                DataTable dt = Database.GetDataTable(cmd2);
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
                Common.Log("TCE::" + ex.Message);
                return false;
            }
        }

        private static string ObjToInsert(Dictionary<string, object> o, string sTable, string sGuid)
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return typeof(String);
            }
        }
        public static bool EstablishSchema(bool fTestNet, Dictionary<string, object> o)
        {
            string sPrefix = fTestNet ? "t" : "";

            try
            {
                string sDBName = Database.GetDatabaseName();
                string sTableName = sPrefix + (string)GetEntityValue(o, "table");
                if (sTableName == null || sTableName == "")
                    return false;

                bool fTableExists = Database.TableExists(sDBName, sTableName);
                if (!fTableExists)
                {
                    string sql = "Create table " + sTableName + " (_id varchar(64) primary key);";
                    MySqlCommand cmd = new MySqlCommand(sql);
                    Database.ExecuteNonQuery(cmd);
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
                            MySqlCommand cmd = new MySqlCommand(sql);
                            bool fOK = Database.ExecuteNonQuery(cmd);
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

        public static bool ApproveTransaction(string sTable, string sPriKeyField, string sLastPriKeyValue, CryptoUtils.Transaction t, out string sError)
        {
            sError = "";
            try
            {
                if (sPriKeyField == null || sPriKeyField == "")
                    return true;
                
                if (sTable.ToLower() == "nft")
                {
                    string sDelete2 = "Delete from " + sTable + " where " + sPriKeyField + "='" + sLastPriKeyValue + "';";

                    NFT newNFT = Newtonsoft.Json.JsonConvert.DeserializeObject<NFT>(t.Data);

                    NFT oldNFT = BMSCommon.NFT.GetNFT(newNFT.Chain, newNFT.GetHash());
                    if (newNFT.Action.ToLower() == "edit")
                    {
                        if (oldNFT.OwnerBBPAddress != newNFT.OwnerBBPAddress || oldNFT.OwnerERC20Address != newNFT.OwnerERC20Address)
                        {
                            sError = "bad ownership edit.";
                            return false;
                        }
                    }
                    else if (newNFT.Action.ToLower() == "buy")
                    {
                        if (newNFT.TXID == null || newNFT.TXID == "")
                        {
                            sError = "TXID is null";
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                BMSCommon.Common.Log("Approve transaction " + ex.Message);
                return true;
            }

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
        private static Transaction DeserializeTransaction(Block b, DataRow r)
        {
            Transaction t = new Transaction();
            t.Data = r.Field<string>("Data");
            t.Height = r.Field<int>("height");
            t.Time = r.Field<int>("time");
            return t;
        }

        private static Block DeserializeBlock(DataRow r)
        {
            Block b = new Block();
            b.BlockNumber = r.ToInt("blocknumber");
            b.MerkleRoot = r.Field<string>("MerkleRoot");
            b.Nonce = r.Field<int>("nonce");
            b.PreviousBlockHash = r.Field<string>("PreviousBlockHash");
            b.Target = ToBigInteger(r.Field<string>("Target"));
            b.Hash = r.Field<string>("Hash");
            b.Time = r.Field<int>("Time");
            b.Version = r.Field<int>("Version");
          
            return b;
        }

        public static CryptoUtils.Block GetBestBlock(bool fTestNet)
        {
            string sTable = fTestNet ? "tblocksync" : "blocksync";
            string sql = "Select * from " + sTable + " where blocknumber = (Select max(blocknumber) from " + sTable + ");";
            MySqlCommand m1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable(m1);
            CryptoUtils.Block b = new CryptoUtils.Block();
            if (dt.Rows.Count < 1)
                return b;
            b = DeserializeBlock(dt.Rows[0]);
            return b;
        }
        

        public static string AddToMemoryPool2(bool fTestNet, Transaction t)
        {   //Step 1 : Insert locally
            CryptoUtils.Block b = new Block();
            b.Transactions.Add(t);
            InsertBiblePayDatabaseTransactions(fTestNet, b);

            // Step 2: Relay

            string sData = t.Data;
            string sTXID = BMSCommon.WebRPC.PushChainData2(fTestNet, "DATA", sData);
            if (sTXID=="")
            {
                throw new Exception("Unabled to add to memory pool.");
            }
            return sTXID;
        }





    }
}
