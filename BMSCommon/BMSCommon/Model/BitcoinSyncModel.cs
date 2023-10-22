using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Numerics;
using System.Text.RegularExpressions;
using BMSCommon;
using NBitcoin;
using static BMSCommon.Encryption;


namespace BMSCommon.Model
{
    public static class BitcoinSyncModel
    {

        public class BitcoinSyncTransaction
        {
            public string Data;
            public int Time;
            public string BlockHash;
            public int Height;
            public string GetHash()
            {
                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                string sNew = rgx.Replace(Data, "");
                string sHash = Encryption.GetSha256String(sNew);
                return sHash;
            }
        }
        public class SupplyType
        {
            public double CirculatingSupply = 0;
            public double TotalSupply = 0;
            public double TotalBurned = 0;
        }



        public struct BalanceUTXO
        {
            public string Address;
            public NBitcoin.Money Amount;
            public NBitcoin.Money satoshis;
            public NBitcoin.uint256 TXID;
            public NBitcoin.uint256 prevtxid;
            public int index;
            public int Height;
        };
        public struct BalanceUTXO2
        {
            // Native client format
            public string satoshis;
            public string address;
            public int height;
            public int index;
            public int outputIndex;
            public string txid;
        };

        public struct ChainPayment
        {
            public string bbpaddress;
            public double amount;
        }

        public struct SimpleUTXO
        {
            public double nAmount;
            public string TXID;
            public int nOrdinal;
            public int nHeight;
            public string Address;
            public string Ticker;
        };

        public class BitcoinSyncBlock
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
            public List<BitcoinSyncTransaction> Transactions = new List<BitcoinSyncTransaction>();
            // Memory Only
            public string NextBlockHash;
            public string Hash;
            public bool IsGenesis;
            public int MemoryPoolTransactions;

        }

    }
}

