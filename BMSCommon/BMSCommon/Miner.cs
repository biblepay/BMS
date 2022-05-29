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
                Console.WriteLine("Base content path=" + Database.msContentRootPath);

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
                bool ftProvision = Database.TableExists(sDBName, "turnkeysanctuary");
                if (!ftProvision)
                {

                    string sql = "create table turnkeysanctuary (id varchar(64) primary key, erc20address varchar(128), Added DateTime, IP varchar(128), sanctuaryname varchar(128), "
                        + "configuration mediumtext, rootpassword varchar(70));";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                    if (!fSuccess)
                        throw new Exception("Unable to create table turnkeysanctuary");
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
                string TXID = BMSCommon.BitcoinSync.AddToMemoryPool(fTestNet, t);
                bool f = TXID != "";
            return f;
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
                    u.ERC20Address = dt.Rows[0]["ERC20Address"].ToString();
                    u.EmailAddress = dt.Rows[0]["EmailAddress"].ToString();
                    u.NickName = dt.Rows[0]["NickName"].ToString();
                    u.Updated = dt.Rows[0]["Updated"].ToString();
                    u.BioURL = dt.Rows[0]["BioURL"].ToString();

                    u.PBSignature = dt.Rows[0]["PBSignature"].ToString();
                    u.PortfolioBuilderAddress = dt.Rows[0]["PortfolioBuilderAddress"].ToString();
                    u.tPBSignature = dt.Rows[0]["tPBSignature"].ToString();
                    u.tPortfolioBuilderAddress = dt.Rows[0]["tPortfolioBuilderAddress"].ToString();


                    u.LoggedIn = false;
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
                CryptoUtils.CheckDatabase();
                // Block Sync Main Entry Point
                int nStartTime = Common.UnixTimestamp();
                int nNewBlockTime = Common.UnixTimestamp();
                bool fPrimary = BMSCommon.Common.IsPrimary();

            while (true)
            {
                try
                {
                    BMSCommon.BitcoinSync.SyncBlocks(true);
                    BMSCommon.BitcoinSync.SyncBlocks(false);
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
