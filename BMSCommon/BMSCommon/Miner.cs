using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static BMSCommon.DataRowExtensions;

namespace BMSCommon
{
    public static class CryptoUtils
    {
        // Chain Params
        public static int POW_TARGET_SPACING = 60 * 1;  // 1 minute blocks
        public static Dictionary<string, Block> mapBlockIndex = new Dictionary<string, Block>();
        public static Block GenesisBlock = null;
        public static int MAX_TRANSACTIONS_PER_BLOCK = 2500;

        public class Pin
        {
            public string URL;
            public string Added;
            public string userid;
            public long size;
            public string syncedlocal;
            public string syncedremote;
            public string table = "pin";
        }

        public class Transaction
        {
            public string Data;
            public int Time;
            public string BlockHash;
            public int Height;
            public string GetHash()
            {
                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                string sNew = rgx.Replace(Data, "");
                string sHash = Common.GetSha256String(sNew);
                return sHash;
            }
        }

        public class Block
        {
            // Header
            public int Version;  // 4 byte
            public string PreviousBlockHash; // 32 byte
            public string MerkleRoot;  // Based on Transactions
            public int Time; // 4 bytes
            public double Difficulty;
            public BigInteger Target;
            public Int64 Nonce;
            // End of Header
            public int BlockNumber;
            public List<CryptoUtils.Transaction> Transactions = new List<CryptoUtils.Transaction>();
            // Memory Only
            public string NextBlockHash;
            public string Hash;
            public bool IsGenesis;
            public int MemoryPoolTransactions;
            public string xGetCheapHash()
            {
                string s = Version.ToString() + PreviousBlockHash + MerkleRoot + Time.ToString() + ToHexString(Target) + Nonce.ToString();
                string hash = Common.GetSha256String(s);
                return hash;
            }
        }

            public static string ToHexString(BigInteger b)
            {
                // mission critical to do: is this a uint256 and is it 64 chars?
                string s = b.ToString("x");
                return s;
            }
            public static BigInteger ToBigInteger(string value)
            {
                if (value == null)
                return 0;

                BigInteger result = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    string v = value.Substring(i, 1);
                    int iDec = Convert.ToInt32(v, 16);
                    result = result * 16 + (iDec);
                }
                return result;
            }


            public static void CheckDatabase()
            {
                //Busy wait while web server warms up
                for (int i = 0; i < 30; i++)
                {
                    {
                        System.Threading.Thread.Sleep(1000);
                        string n = Database.msContentRootPath;
                        if (n != null)
                            break;
                    }
                }
                string sNarr = "BMS Base content path=" + Database.msContentRootPath;
                WebRPC.LogRPCError(sNarr);


                string sDBName = Database.GetDatabaseName();
                bool fDBExists = Database.DatabaseExists(sDBName);
                if (!fDBExists)
                {
                    string sql = "create database " + sDBName + ";";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                    {
                        Common.Log("Unable to create database " + sDBName);
                        throw new Exception("CheckDatabase failed.");
                    }
                }

            bool fBlockChair = Database.TableExists(sDBName, "BlockChair2");
            if (!fBlockChair)
            {
                string sql = "create table BlockChair2 (id varchar(64) primary key, ticker varchar(100), added datetime, amount numeric(20,10), Address varchar(128), "
                    +"ordinal int, txid varchar(128), height int, account varchar(128), TotalBalance float, utxotxtime int, txcount int); ";
                MySqlCommand cmd1 = new MySqlCommand(sql);
                bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                if (!fSuccess)
                    throw new Exception("Unable to create table blockchair");

            }

            fBlockChair = Database.TableExists(sDBName, "BlockChairRequestLog");
            if (!fBlockChair)
            {
                string sql = "create table BlockChairRequestLog (id varchar(64) primary key, added datetime, URL varchar(1000));";
                MySqlCommand cmd1 = new MySqlCommand(sql);
                bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                if (!fSuccess)
                    throw new Exception("Unable to create table blockchairrl");

            }

            bool fBlocks = Database.TableExists(sDBName, "blocksync");
                if (!fBlocks)
                {
                    string sql = "create table blocksync (hash varchar(64) primary key, time int, Version int, PreviousBlockHash varchar(64), "
                        + "NextBlockHash varchar(64), MerkleRoot varchar(64), Target varchar(64), Nonce int, BlockNumber int, Added datetime);";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table blocksync");
                }

            bool f0 = Database.TableExists(sDBName, "pbdonation");
            if (!f0)
            {
                string sql = "create table pbdonation (id varchar(64) primary key, txid varchar(100), amount float, added datetime);";
                MySqlCommand cmd1 = new MySqlCommand(sql);
                bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                if (!fSuccess)
                    throw new Exception("Unable to create table pbdonation");
            }

            f0 = Database.TableExists(sDBName, "tpbdonation");
            if (!f0)
            {
                string sql = "create table tpbdonation (id varchar(64) primary key, txid varchar(100), amount float, added datetime);";
                MySqlCommand cmd1 = new MySqlCommand(sql);
                bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                if (!fSuccess)
                    throw new Exception("Unable to create table tpbdonation");
            }


            fBlocks = Database.TableExists(sDBName, "tblocksync");
            if (!fBlocks)
            {
                string sql = "create table tblocksync (hash varchar(64) primary key, time int, Version int, PreviousBlockHash varchar(64), "
                    + "NextBlockHash varchar(64), MerkleRoot varchar(64), Target varchar(64), Nonce int, BlockNumber int, Added datetime);";
                MySqlCommand cmd1 = new MySqlCommand(sql);
                bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                if (!fSuccess)
                    throw new Exception("Unable to create table tblocksync");
            }

            bool fTransactions = Database.TableExists(sDBName, "transactionsync");
            if (!fTransactions)
            {
                    string sql = "create table transactionsync (hash varchar(64) primary key, time int, blockhash varchar(64), height int, Data mediumtext, Added datetime);";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create transsync table.");
            }

            fTransactions = Database.TableExists(sDBName, "ttransactionsync");
            if (!fTransactions)
            {
                    string sql = "create table ttransactionsync (hash varchar(64) primary key, time int, blockhash varchar(64), height int, Data mediumtext, Added datetime);";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create transactions table.");
            }

            bool fHR = Database.TableExists(sDBName, "hashrate");
            if (!fHR)
            {
                    string sql = "create table hashrate (id varchar(64) primary key, MinerCount int, HashRate float, Added datetime, Height int, SolvedCount int);";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create ttranssync");
            }

            bool fProposal = Database.TableExists(sDBName, "proposal");
            if (!fProposal)
            {
                    string sql = "create table proposal (id varchar(64) primary key, ExpenseType varchar(50), ERC20Address varchar(128), NickName varchar(128), URL varchar(1024), Name varchar(1000), "
                        + "Address varchar(128), Amount float, UnixStartTime float, PrepareTXID varchar(128), SubmitTXID varchar(128), Added datetime, Updated datetime, Submitted datetime, Hex varchar(4000), Chain varchar(50));";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table Proposal");
            }


                bool ftHR = Database.TableExists(sDBName, "thashrate");
                if (!ftHR)
                {
                    string sql = "create table thashrate (id varchar(64) primary key, MinerCount int, HashRate float, Added datetime, Height int, SolvedCount int);";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table tHR");
                }
                bool ftProvision = Database.TableExists(sDBName, "turnkeysanctuaries");
                if (!ftProvision)
                {
                    string sql = "create table turnkeysanctuaries (id varchar(64) primary key, erc20address varchar(128), Added DateTime, "
                        + "BBPAddress varchar(64), Nonce varchar(64));";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table turnkeysanctuary");
                }

            ftProvision = Database.TableExists(sDBName, "tturnkeysanctuaries");
            if (!ftProvision)
            {
                string sql = "create table tturnkeysanctuaries (id varchar(64) primary key, erc20address varchar(128), Added DateTime, "
                    + "BBPAddress varchar(64), Nonce varchar(64));";
                MySqlCommand cmd1 = new MySqlCommand(sql);
                bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                if (!fSuccess)
                    throw new Exception("Unable to create table tturnkeysanctuary");
            }

            bool fW = Database.TableExists(sDBName, "tworker");
                if (!fW)
                {
                    string sql = "create table tworker (id varchar(64) primary key, bbpaddress varchar(128), moneroaddress varchar(128), Added datetime, IP varchar(50));";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table tHR");
                }

                bool fW2 = Database.TableExists(sDBName, "worker");
                if (!fW2)
                {
                    string sql = "create table worker (id varchar(64) primary key, bbpaddress varchar(128), moneroaddress varchar(128), Added datetime, IP varchar(50));";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table W");
                }



                bool fShare = Database.TableExists(sDBName, "share");
                if (!fShare)
                {
                    string sql = "create table share (bbpaddress varchar(64), shares float, fails int, height int, updated datetime, Reward float, Percentage float, Subsidy float, SucXMR float, "
                        + "FailXMR float, SucXMRC float, FailXMRC float, TXID varchar(128), Paid datetime, Solved int, BXMR float, BXMRC int, PRIMARY KEY (bbpaddress, height));";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table share");
                }

                bool ftShare = Database.TableExists(sDBName, "tshare");
                if (!ftShare)
                {
                    string sql = "create table tshare (bbpaddress varchar(64), shares float, fails int, height int, updated datetime, Reward float, Percentage float, Subsidy float, SucXMR float, "
                        + "FailXMR float, SucXMRC float, FailXMRC float, TXID varchar(128), Paid datetime, Solved int, BXMR float, BXMRC int, PRIMARY KEY (bbpaddress, height));";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table share");
                }


                bool fSPInsShare = Database.SPExists(false, sDBName, "insShare");
                if (!fSPInsShare)
                {
                    string data = BMSCommon.DSQL.GetSQLTemplate("insShare.htm");
                    data = data.Replace("@share1", "share");
                    string sFullName = "`insShare`";
                    data = data.Replace("`insShare`", sFullName);
                    MySqlCommand cmd1 = new MySqlCommand(data);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create stored procedure insShare");
                }

                bool fSPInsShareTestNet = Database.SPExists(false, sDBName, "tinsShare");
                if (!fSPInsShareTestNet)
                {
                    string data = BMSCommon.DSQL.GetSQLTemplate("insShare.htm");
                    data = data.Replace("`insShare`", "`tinsShare`");

                    data = data.Replace("@share1", "tshare");

                    MySqlCommand cmd1 = new MySqlCommand(data);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create stored procedure tinsShare");
                }

                bool fSPUpdLeaderboard = Database.SPExists(false, sDBName, "updLeaderboard");
                if (!fSPUpdLeaderboard)
                {
                    string data = BMSCommon.DSQL.GetSQLTemplate("updLeaderboard.htm");

                    data = data.Replace("tbl_share", "share");
                    data = data.Replace("tbl_worker", "worker");
                    data = data.Replace("tbl_hashrate", "hashrate");

                    MySqlCommand cmd1 = new MySqlCommand(data);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create stored procedure updLeaderboard");
                }


                bool fSPUpdtLeaderboard = Database.SPExists(false, sDBName, "updtLeaderboard");
                if (!fSPUpdtLeaderboard)
                {
                    string data = BMSCommon.DSQL.GetSQLTemplate("updLeaderboard.htm");
                    data = data.Replace("Leaderboard", "tLeaderboard");

                    data = data.Replace("tbl_share", "tshare");
                    data = data.Replace("tbl_worker", "tworker");
                    data = data.Replace("tbl_hashrate", "thashrate");

                    MySqlCommand cmd1 = new MySqlCommand(data);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create stored procedure upd-t-Leaderboard");
                }


                bool fBanDetails = Database.TableExists(sDBName, "bandetails");
                if (!fBanDetails)
                {
                    string sql = "create table bandetails (id varchar(64) primary key, IP varchar(128), Notes varchar(512), Added datetime, level float);";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create bandetails.");
                }

                bool fSystem = Database.TableExists(sDBName, "sys");
                if (!fSystem)
                {
                    string sql = "create table sys (id varchar(64) primary key, systemkey varchar(200), value varchar(200), updated DateTime);";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table system.");
                }
                bool fQH = Database.TableExists(sDBName, "quotehistory");
                if (!fQH)
                {
                    string sql = "create table quotehistory (id varchar(64) primary key,added datetime, ticker varchar(50),USD float, BTC float);";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table quotehistory.");
                }

                bool fDH = Database.TableExists(sDBName, "DifficultyHistory");
                if (!fDH)
                {
                    string sql = "create table DifficultyHistory (id varchar(64) primary key, height int, recipient varchar(64), subsidy float, Added datetime, Difficulty float);";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table dh.");
                }

                bool fDH2 = Database.TableExists(sDBName, "tDifficultyHistory");
                if (!fDH2)
                {
                    string sql = "create table tDifficultyHistory (id varchar(64) primary key, height int, recipient varchar(64), subsidy float, Added datetime, Difficulty float);";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table dh.");
                }

            // Adhoc updates
            string sql0 = "Delete from pin where URL like '%.ts';Delete from tpin where URL like '%.ts';";
            MySqlCommand cmd0 = new MySqlCommand(sql0);
            bool fSuccess0 = Database.ExecuteNonQuery(false, cmd0, "");

            // During 2022 expense audit I found some duplicated expenses and this removes them (you can still clearly see our expenses for Cameroon One maintains the pattern of paying for 90 orphans per month, so it is very clear the data is correct after these changes) :
            sql0 = "Delete from Expense where _id in ('0598f4608100f443f158a4a55da9e3f93e27ff024db7140eb078c3391a798d42', '616f79fb9d274959f04f8fe2678979a49442ff8a7f4f9be4d2caa21de1866564', 'c4a599f1f67f7f73d1ee90dfb4f804271309987f43389b6674953ad7b2ae88ad', 'ff372782da94d422a65b598b9db112e70e9455af260de53f7fef8880446c50c8');";
            cmd0 = new MySqlCommand(sql0);
            fSuccess0 = Database.ExecuteNonQuery(false, cmd0, "");
            sql0 = "update Expense set Amount = 1400 where _id = 'ea6b247582ac9bf94a2913abaa7350d2313cb2f0cdc9120911a312a6e5f710af';";
            cmd0 = new MySqlCommand(sql0);
            fSuccess0 = Database.ExecuteNonQuery(false, cmd0, "");
            sql0 = "Delete from OrphanExpense3 where Added='4-1-2022';";
            cmd0 = new MySqlCommand(sql0);
            fSuccess0 = Database.ExecuteNonQuery(false, cmd0, "");

            sql0 = "delete from Revenue where _id = 'de618099a892166d2e164a1000dadf07c2d16dc015d86e86b9c08741db031fdd';\r\nInsert into Revenue (_id,Added,Amount,Notes,HandledBy,Charity) values ('de618099a892166d2e164a1000dadf07c2d16dc015d86e86b9c08741db031fdd','6/1/2022 11:59:00 AM',38507.39,'Donation from Rob Andrews (General Fund)','bible_pay','General');";
            cmd0 = new MySqlCommand(sql0);
            fSuccess0 = Database.ExecuteNonQuery(false, cmd0, "");

            // End of adhoc updates
        }

        public class User
            {
                public string ERC20Address = "";
                public string table = "user";
                public string EmailAddress = "";
                public string PrimaryKey = "ERC20Address";
                public string BBPSignature = "";
                public string BBPSignAddress = "";
                public string BBPSignatureTime = "";
                public string NickName = "";
                public string Updated = "";
                public string BioURL = "";
                public string PortfolioBuilderAddress = "";
                public string tPortfolioBuilderAddress = "";
                public string PBSignature = "";
                public string tPBSignature = "";
                public string BBPAddress = "";
                public bool LoggedIn = false;
            };

         public static bool PersistUser(bool fTestNet, User u)
         {
                if (u.ERC20Address == null || u.ERC20Address == "")
                {
                    return false;
                }
                Transaction t = new Transaction();
                t.Time = Common.UnixTimestamp();
                t.Data = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                string TXID = BMSCommon.BitcoinSync.AddToMemoryPool2(fTestNet, t);
                bool f = TXID != "";
            return f;
         }

        public static Dictionary<string, User> tdictUsers = new Dictionary<string, User>();
        public static Dictionary<string, User> mdictUsers = new Dictionary<string, User>();

        public static void MemorizeUsers(bool fTestNet)
        {
            string sTable = fTestNet ? "tuser" : "user";

            string sql = "Select * from " + sTable;
            MySqlCommand m1 = new MySqlCommand(sql);
            DataTable dt = Database.GetDataTable(m1);
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                User u = RowToUser(dt.Rows[i]);
                if (fTestNet)
                {
                    tdictUsers[u.ERC20Address] = u;
                }
                else
                {
                    mdictUsers[u.ERC20Address] = u;
                }
            }
        }

        public static User GetCachedUser(bool fTestNet, string sERC20Address)
        {
            User u = new User();

            if (fTestNet)
            {
                if (tdictUsers.Count == 0)
                    MemorizeUsers(fTestNet);
                tdictUsers.TryGetValue(sERC20Address, out u);
            }
            else
            {
                if (mdictUsers.Count == 0)
                    MemorizeUsers(fTestNet);
                mdictUsers.TryGetValue(sERC20Address, out u);
            }
            return u;
        }

        public static User RowToUser(DataRow dr)
        {
            User u = new User();
            u.ERC20Address = dr["ERC20Address"].ToString();
            u.EmailAddress = dr["EmailAddress"].ToString();
            u.NickName = dr["NickName"].ToString();
            u.Updated = dr["Updated"].ToString();
            u.BioURL = dr["BioURL"].ToString();
            u.PBSignature = dr["PBSignature"].ToString();
            u.PortfolioBuilderAddress = dr["PortfolioBuilderAddress"].ToString();
            u.tPBSignature = dr["tPBSignature"].ToString();
            u.tPortfolioBuilderAddress = dr["tPortfolioBuilderAddress"].ToString();
            u.LoggedIn = false;
            return u;
        }
        public static User DepersistUser(bool fTestNet, string sERC20Address)
            {
                string sTable = fTestNet ? "tuser" : "user";
                string sql = "Select * from " + sTable + " where ERC20Address=@e;";
                User u = new User();
                try
                {
                    MySqlCommand m1 = new MySqlCommand(sql);
                    m1.Parameters.AddWithValue("@e", sERC20Address);
                    DataTable dt = BMSCommon.Database.GetDataTable(m1);
                    if (dt.Rows.Count == 0)
                        return u;
                    u = RowToUser(dt.Rows[0]);
                    return u;
                }
                catch (Exception ex)
                {
                    Common.Log("DU::" + ex.Message);
                    return u;
                }
            }


            private struct FakeTable
            {
                public string id;
                public string mycolumn1;
                public string mycolumn2;
                public int myint1;
                public string URL;
            };
        }
        public static class Miner
        {
            // Mission Critical Todo: Change Miner to BitcoinSync (model has changed)
            // Each BMS node mines in the background.  The miner solves blocks on the sidechain.
            public static async void Mine()
            {
            try
            {
                CryptoUtils.CheckDatabase();
                WebRPC.LogRPCError("database initialized...");
            }
            catch(Exception ex)
            {
                Common.Log("Mine:"+ex.Message);
                WebRPC.LogRPCError("Mine:"+ex.Message);
            }
                // Block Sync Main Entry Point
                int nStartTime = Common.UnixTimestamp();
                int nNewBlockTime = Common.UnixTimestamp();
                bool fPrimary = BMSCommon.Common.IsPrimary();

            while (true)
            {
                try
                {
                    await BMSCommon.BitcoinSync.SyncBlocks(true);
                    await BMSCommon.BitcoinSync.SyncBlocks(false);
                    //WebRPC.LogRPCError("Looping " + DateTime.Now.ToString());
                }
                catch(Exception ex)
                {
                    Common.Log("SyncBlocks::" + ex.Message);
                }
                System.Threading.Thread.Sleep(30000);
            }

        }
   }
}
