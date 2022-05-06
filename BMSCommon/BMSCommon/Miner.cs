using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static BMSCommon.DataRowExtensions;

namespace BMSCommon
{
    public static class CryptoUtils
    {
        // Chain Params

        public static BigInteger POW_LIMIT = ToBigInteger("ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"); 
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
            //public string Hash;
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
            public BigInteger Target;
            public Int64 Nonce;
            // End of Header
            public int BlockNumber;
            public List<Transaction> Transactions = new List<Transaction>();
            // Memory Only
            public string NextBlockHash;
            public bool IsGenesis;
            public int MemoryPoolTransactions;
            public string GetBlockHash()
            {
                string s = Version.ToString() + PreviousBlockHash + MerkleRoot + Time.ToString() + ToHexString(Target) + Nonce.ToString();
                string hash = Common.GetSha256String(s);
                return hash;
            }
            public string GetTransactions()
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Transactions.Count; i++)
                {
                    sb.Append(Transactions[i].GetHash() + ",");
                }
                string sList = sb.ToString();
                if (sList.Length > 0)
                    sList = sList.Substring(0, sList.Length - 1);
                return sList;
            }

            public bool AddBlockIndex()
            {

                if (this.PreviousBlockHash != null)
                {
                    Block bDuplicate = GetBlockByPreviousBlockHash(this.PreviousBlockHash);
                    if (bDuplicate != null && bDuplicate.GetBlockHash() != this.GetBlockHash())
                    {
                        Common.Log("Unable to add block::Duplicate for " + this.GetBlockHash());
                        return false;
                    }

                }

                if (mapBlockIndex.ContainsKey(this.GetBlockHash()))
                {
                    return false;
                }
                Block bAncestor = null;
                bool fGotAncestor = false;
                if (!this.IsGenesis)
                {
                    fGotAncestor = mapBlockIndex.TryGetValue(this.PreviousBlockHash, out bAncestor);
                }
                if (fGotAncestor)
                {
                    if (bAncestor.NextBlockHash == null)
                    {
                        bAncestor.NextBlockHash = this.GetBlockHash();
                        mapBlockIndex[bAncestor.GetBlockHash()] = bAncestor;
                    }
                }
                mapBlockIndex.Add(this.GetBlockHash(), this);
                for (int i = 0; i < this.Transactions.Count; i++)
                {
                    try
                    {
                        API.mapTransactions.Add(this.Transactions[i].GetHash());
                    }
                    catch (Exception)
                    {
                        // if already in there, no issue.
                    }
                }

                if (this.IsGenesis)
                {
                    GenesisBlock = this;
                }
                return true;
            }

        }

        public static Block GetBlock(string sHash)
        {
            if (sHash == null)
                return null;

            Block b = new Block();
            bool f = mapBlockIndex.TryGetValue(sHash, out b);
            if (!f)
            {
                return null;
            }
            return b;
        }

        public static Block GetBlockByNumber(int nHeight)
        {
            foreach (KeyValuePair<string, Block> b in CryptoUtils.mapBlockIndex)
            {
                if (b.Value.BlockNumber == nHeight)
                    return b.Value;
            }

            return null;
        }

        public static Block GetBlockByPreviousBlockHash(string sHash)
        {
            foreach (KeyValuePair<string, Block> b in CryptoUtils.mapBlockIndex)
            {
                if (b.Value.PreviousBlockHash == sHash)
                    return b.Value;
            }
            return null;
        }


        public static string ToHexString(BigInteger b)
        {
            // mission critical to do: is this a uint256 and is it 64 chars?
            string s = b.ToString("x");
            return s;
        }
        public static BigInteger ToBigInteger(string value)
        {
            BigInteger result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                string v = value.Substring(i, 1);
                int iDec = Convert.ToInt32(v, 16);
                result = result * 16 + (iDec);
            }
            return result;
        }

        public static BigInteger CalculateDifficulty()
        {
            // Loop through prior N blocks
            BigInteger cTotal = new BigInteger();
            int nSamples = 16;
            Block cBlockCurrent = GetBestBlock();
            Block cBlockEnd = GetBestBlock();
            if (cBlockCurrent == null)
            {
                return POW_LIMIT;
            }
            int nProcessed = 0;
            while (true)
            {
                if (cBlockCurrent == null || nProcessed >= nSamples || cBlockCurrent.PreviousBlockHash == null)
                    break;
                if (cBlockCurrent.Target != null)
                {
                    cTotal += cBlockCurrent.Target;
                }
                nProcessed++;
                mapBlockIndex.TryGetValue(cBlockCurrent.PreviousBlockHash, out cBlockCurrent);
            }
            if (cTotal == 0)
            {
                cTotal = POW_LIMIT; // Genesis block
                nProcessed = 1;
            }
            if (nProcessed < 0)
                nProcessed = 1;
            BigInteger bnNew = cTotal / nProcessed;
            int nBlockTimeEnd = cBlockEnd.Time;
            int nBlockTimeStart = cBlockCurrent.Time;
            int nActualTimespan = nBlockTimeEnd - nBlockTimeStart;
            int nTargetTimespan = nProcessed * POW_TARGET_SPACING;
            if (nActualTimespan < nTargetTimespan / 3)
                nActualTimespan = nTargetTimespan / 3;
            if (nActualTimespan > nTargetTimespan * 3)
                nActualTimespan = nTargetTimespan * 3;
            // Retarget
            bnNew *= nActualTimespan;
            bnNew /= nTargetTimespan;
            if (bnNew > POW_LIMIT)
            {
                bnNew = POW_LIMIT;
            }
            return bnNew;
        }

        public static string CalculateMerkleRoot(Block b)
        {
            List<string> myMerkle = new List<string>();
            for (int i = 0; i < b.Transactions.Count; i++)
            {
                string sHash = b.Transactions[i].GetHash();
                myMerkle.Add(sHash);
            }
            myMerkle.Sort();
            string sHashList = string.Join("", myMerkle);
            string sMerkleRoot = Common.GetSha256String(sHashList);
            return sMerkleRoot;
        }
        public static Block GetBestBlock()
        {
            if (GenesisBlock == null)
                return null;
            CryptoUtils.Block currentBlock = GenesisBlock;
            int iHeight = 1;
            while (true)
            {
                if (currentBlock.NextBlockHash == null)
                    return currentBlock;

                if (mapBlockIndex.ContainsKey(currentBlock.NextBlockHash))
                {
                    currentBlock = mapBlockIndex[currentBlock.NextBlockHash];
                    iHeight++;
                    currentBlock.BlockNumber = iHeight;
                }
                else
                {
                    return currentBlock;
                }
                if (currentBlock == null)
                    return null;
            }
        }

        public static Block GetBlockByPrevBlockHash(string sPrevHash)
        {
            if (GenesisBlock == null)
                return null;
            CryptoUtils.Block currentBlock = GenesisBlock;
            int iHeight = 1;
            while (true)
            {
                if (currentBlock == null)
                    return null;

                if (currentBlock.GetBlockHash() == sPrevHash)
                {
                    return currentBlock;
                }
                if (currentBlock.NextBlockHash == null)
                    return null;

                if (mapBlockIndex.ContainsKey(currentBlock.NextBlockHash))
                {
                    currentBlock = mapBlockIndex[currentBlock.NextBlockHash];
                    iHeight++;
                    currentBlock.BlockNumber = iHeight;
                }
                else
                {
                    return null;
                }
            }
        }


        public static Block CreateBlock()
        {
            // The block assembler creates blocks from memory pool transactions.
            Block b = new Block();
            b.Target = CalculateDifficulty();
            b.Time = Common.UnixTimestamp();
            b.Version = 1;
            int iTxCount = 0;
            foreach (KeyValuePair<string, Transaction> tx in API.dMemoryPool.ToList())
            {
                if (!API.TxExistsInMapBlockIndex(tx.Value.GetHash()))
                {
                    b.Transactions.Add(tx.Value);
                    iTxCount++;
                    if (iTxCount >= MAX_TRANSACTIONS_PER_BLOCK)
                        break;
                }
            }
            b.MerkleRoot = CalculateMerkleRoot(b);
            // Best Block Hash
            b.PreviousBlockHash = GetBestBlock().GetBlockHash();
            b.BlockNumber = GetBestBlock().BlockNumber + 1;
            b.MemoryPoolTransactions = API.dMemoryPool.Count;
            return b;
        }
        public static bool CheckProofOfWork(Block b)
        {
            string hash = b.GetBlockHash();
            BigInteger bnHash = ToBigInteger(hash);
            return (bnHash < b.Target);
        }

        public static Block CreateGenesisBlock()
        {
            Block b = new Block();
            b.Target = POW_LIMIT;
            b.Time = 1650844081;
            b.Version = 1;
            b.Nonce = 1;
            Transaction t = new Transaction();
            t.Time = b.Time;
            t.Data = "Pure and undefiled religion before God and the Father is this: to visit orphans and widows in their trouble, and to keep oneself unspotted from the world.";
            b.Transactions.Add(t);
            b.MerkleRoot = CalculateMerkleRoot(b);
            b.Nonce = 513656;
            b.IsGenesis = true;
            b.BlockNumber = 1;
            return b;
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
            t.BlockHash = b.GetBlockHash();
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
            b.Time = r.Field<int>("Time");
            b.Version = r.Field<int>("Version");
            string txList = r.Field<string>("Transactions");
            string sTxList = EncloseInTicks(txList);
            string sql = "Select * from transactions where hash in (" + sTxList + ");";
            MySqlCommand cmd = new MySqlCommand(sql);
            DataTable dt = Database.GetMySqlDataTable(false, cmd, "");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Transaction t = DeserializeTransaction(b, dt.Rows[i]);
                t.BlockHash = b.GetBlockHash();
                b.Transactions.Add(t);
            }
            if (b.GetBlockHash() == "00000e7ddf4a6c60f059f3c6343aed82cb5353a87f8bfe6df57fb482049af9a5")
                b.IsGenesis = true;
            string sMR = CalculateMerkleRoot(b);
            if (b.MerkleRoot != sMR)
            {
                b = null;
            }
            return b;
        }

        public static async Task<bool> LoadBlockIndex()
        {
            try
            {
                CryptoUtils.CheckDatabase();
                mapBlockIndex.Clear();
                API.dMemoryPool.Clear();
                string sql = "Select * from blocks order by time;";
                MySqlCommand cmd = new MySqlCommand(sql);
                DataTable dt = Database.GetMySqlDataTable(false, cmd, "");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Block b = DeserializeBlock(dt.Rows[i]);
                    if (b != null)
                    {
                        if (b.PreviousBlockHash != null)
                        {
                            Block ancestorBlock = GetBlock(b.PreviousBlockHash);
                            if (ancestorBlock != null)
                            {
                                ancestorBlock.NextBlockHash = b.GetBlockHash();
                                mapBlockIndex[ancestorBlock.GetBlockHash()] = ancestorBlock;
                            }
                        }
                        b.AddBlockIndex();
                    }
                }
                return true;
            }catch(Exception ex)
            {
                Common.Log("LoadBlockIndex::" + ex.Message);
                return false;
            }
        }

        public static void CheckDatabase()
        {
            string sDBName = Database.GetDatabaseName();
            bool fDBExists = Database.DatabaseExists(false, sDBName);
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
            bool fBlocks = Database.TableExists(false, sDBName, "blocks");
            if (!fBlocks)
            {
                string sql = "create table blocks (hash varchar(64) primary key, time int, transactions mediumtext, Version int, PreviousBlockHash varchar(64), "
                    + "MerkleRoot varchar(64), Target varchar(64), Nonce int, BlockNumber int, Added datetime);";
                MySqlCommand cmd1 = new MySqlCommand(sql);
                bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                if (!fSuccess)
                    throw new Exception("Unable to create table blocks");
            }
            fBlocks = Database.TableExists(false, sDBName, "transactions");
            if (!fBlocks)
            {
                string sql = "create table transactions (hash varchar(64) primary key, time int, blockhash varchar(64), height int, Data mediumtext, Added datetime);";
                MySqlCommand cmd1 = new MySqlCommand(sql);
                bool fSuccess = Database.ExecuteNonQuery(false, cmd1, "");
                if (!fSuccess)
                    throw new Exception("Unable to create transactions table.");
            }
            string sql1 = "Select count(*) ct from blocks;";
            MySqlCommand cmd = new MySqlCommand(sql1);
            double nCt = Database.GetScalarDouble(false, cmd, "ct");
            if (nCt == 0)
            {
                // No genesis
                Block b = CryptoUtils.CreateGenesisBlock();
                for (int i = 0; i < 1000000; i++)
                {
                    if (CryptoUtils.CheckProofOfWork(b))
                    {
                        b.BlockNumber = 1;
                        bool f =    API.InsertBlock(b);
                        if (!f)
                        {
                            throw new Exception("Unable to store genesis block.");
                        }
                        break;
                    }
                    b.Nonce++;
                }
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

        public static async void CreateFakeTransactions()
        {
            // This is for stress testing during initial dvelopment
            for (int i = 0; i < 9; i++)
            {
                if (false)
                {
                    Transaction t = new Transaction();
                    // remoe the tx from memory pool when we accept the block (connectblock)
                    t.Time = Common.UnixTimestamp();
                    FakeTable f = new FakeTable();
                    f.id = Guid.NewGuid().ToString();
                    f.mycolumn1 = Guid.NewGuid().ToString();
                    f.URL = "https://" + Guid.NewGuid().ToString();
                    f.myint1 = i;
                    t.Data = Newtonsoft.Json.JsonConvert.SerializeObject(f);
                    BMSCommon.API.AddToMemoryPool(t, true);
                }
                if (true)
                {
                    Transaction t = new Transaction();
                    t.Time = Common.UnixTimestamp();
                    API.Junk2 j = new API.Junk2();
                    j.field1 = "https://" + Guid.NewGuid().ToString();
                    j.field2 = "https://" + Guid.NewGuid().ToString();
                    j.me_id = i * 1000;
                    t.Data = Newtonsoft.Json.JsonConvert.SerializeObject(j);
                    BMSCommon.API.AddToMemoryPool(t, true);
                }
            }
            bool f999 = false;
        }
    }
    public static class Miner
    {
        // Each BMS node mines in the background.  The miner solves blocks on the sidechain.
        public static int mnHashes = 0;
        public static async void Mine()
        {
            await CryptoUtils.LoadBlockIndex();
            CryptoUtils.Block b = CryptoUtils.CreateBlock();
            int nTotalElapsed = 0;
            int nBlockTimeElapsed = 0;
            int nStartTime = Common.UnixTimestamp();
            int nNewBlockTime = Common.UnixTimestamp();
            bool fPrimary = BMSCommon.Common.IsPrimary();
            CryptoUtils.Block bestBlock = CryptoUtils.GetBestBlock();
            while (true)
            {
                try
                {
                    b.Nonce++;
                    mnHashes++;
                    // Once during modulus, breathe:
                    if (mnHashes % 256000 == 0 || !fPrimary)
                    {
                        nTotalElapsed = Common.UnixTimestamp() - nStartTime;
                        nBlockTimeElapsed = Common.UnixTimestamp() - nNewBlockTime;
                        System.Threading.Thread.Sleep(10);
                        bestBlock = CryptoUtils.GetBestBlock();
                        bool fFullBlock = b.Transactions.Count >= CryptoUtils.MAX_TRANSACTIONS_PER_BLOCK;
                        if (b.MemoryPoolTransactions != API.dMemoryPool.Count && !fFullBlock)
                        {
                            // memory pool has changed...
                            System.Threading.Thread.Sleep(5000); // give a little time to fill the mem pool 
                            b = CryptoUtils.CreateBlock();
                        }
                        if (b.PreviousBlockHash != bestBlock.GetBlockHash())
                        {
                            b = CryptoUtils.CreateBlock();
                        }
                    }
                    // Once per 30 seconds or so, create a new block:
                    if (nBlockTimeElapsed > 25)
                    {
                        nNewBlockTime = Common.UnixTimestamp();
                        nBlockTimeElapsed = Common.UnixTimestamp() - nNewBlockTime;
                        try
                        {
                            API.GetBlocksFromNetwork();
                        }
                        catch (Exception ex)
                        {

                        }
                        b = CryptoUtils.CreateBlock();
                        nNewBlockTime = Common.UnixTimestamp();
                    }
                    if (b.Transactions.Count > 0 && CryptoUtils.CheckProofOfWork(b) && b.Nonce > 256000 && fPrimary)
                    {
                        bool fFullBlock = b.Transactions.Count >= CryptoUtils.MAX_TRANSACTIONS_PER_BLOCK;
                        bool fStale = false;
                        if (b.MemoryPoolTransactions != API.dMemoryPool.Count)
                            fStale = true;
                        if (!fStale || fFullBlock)
                        {
                            // Solved.
                            API.ConnectBlock(b, true);
                            b = CryptoUtils.CreateBlock();
                        }
                    }
                    if (!fPrimary)
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                }
                catch(Exception ex)
                {
                    Common.Log("Miner::MinerThreadCrash::" + ex.Message);
                }
            }
        }
    }
}
