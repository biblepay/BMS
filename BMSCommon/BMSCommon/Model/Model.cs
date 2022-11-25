using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace BMSCommon
{
    public static class Model
    {
        public static string msContentRootPath = null;

        public class DACResult
        {
            public string TXID = String.Empty;
            public bool Result;
            public string Error;
        }

        public struct ERCAsset
        {
            public string ERCAddress;
            public string Symbol;
            public string Chain;
            public double Price;
            public double Amount;
            public string ChainlinkAddress;
        };

        public struct BMSNode
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


        public class UTXOIntegration
        {
            public string table = "UTXOIntegration";
            public string data = "";
            public string added = "";
            public string signature = "";
            public int nHeight = 0;
        }
        public class SystemKey
        {
            public string Key;
            public string Value;
            public DateTime Added;
            public int nUnixTimeAdded = 0;
        }
        public class CharityReport
        {
            public DateTime Added { get; set; }
            public string Type { get; set; }
            public string Notes { get; set; }
            public double Amount { get; set; }
        }

        public class OrphanExpense3
        {
            public string Added;
            public double Amount;
            public string URL = String.Empty;
            public string Charity;
            public string HandledBy;
            public string ChildID;
            public double Balance;
            public string Notes;
            public int Version = 9;
            public string id;
            public string table = "OrphanExpense3";
        }

        public class VerseMemorizer
        {
            public string id = String.Empty;
            public string BookFrom = String.Empty;
            public string BookTo = String.Empty;
            public int ChapterFrom = 0;
            public int VerseFrom = 0;
            public int ChapterTo = 0;
            public int VerseTo = 0;

            public String Added = String.Empty;
        }
        public class SponsoredOrphan2
        {
            public string id = String.Empty;
            public string ChildID = String.Empty;
            public string Charity = String.Empty;
            public string BioURL = String.Empty;
            public string Added = String.Empty;
            public double MonthlyAmount = 0;
            public string Name = String.Empty;
            public String BioPicture = String.Empty;
            public int Active = 0;
        }

        public class price1
        {
            public string Ticker { get; set; }
            public double Amount { get; set; }
            public double AmountUSD { get; set; }
        }


        public class Articles
        {
            public string table = "Articles";
            public string Name;
            public string Description;
            public string Added;
            public string id;
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


        public class TurnkeySanc
        {
            public string id = String.Empty;
            public string erc20address = String.Empty;
            public string Added = String.Empty;
            public string BBPAddress = String.Empty;
            public string Nonce = String.Empty;
            public string LastPaid = String.Empty;
            public double Balance = 0;
            public string Signature = String.Empty;

        }

        public class Proposal
        {
            public DateTime Added;
            public string URL;
            public String SubmitTXID;
            public String PrepareTXID;
            public String NickName;
            public string ExpenseType;
            public string Name;
            public int nStartTime;
            public int nEndTime;
            public string BBPAddress;
            public double Amount;
            public string Chain;
            public string id;
            public DateTime Updated;
            public DateTime Submitted;
            public string Hex;
            public string ERC20Address;

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
            public string id;
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
            public string id;
        }


    }

    public static class BitcoinSyncModel
    {
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
                string sHash = Common.GetSha256String(sNew);
                return sHash;
            }
        }
        public class SupplyType
        {
            public double CirculatingSupply = 0;
            public double TotalSupply = 0;
            public double TotalBurned = 0;
        }


        public static MessageSigner _MessageSignerTest = new MessageSigner();
        public static MessageSigner _MessageSignerMain = new MessageSigner();
        public struct MessageSigner
        {
            public string SigningPublicKey;
            public string Signature;
            public string SignMessage;
        };


        public struct BalanceUTXO
        {
            public string Address;
            public NBitcoin.Money Amount;
            public NBitcoin.uint256 TXID;
            public NBitcoin.uint256 SpentToTXID;
            public int index;
            public int Height;
            public int SpentToIndex;
            public NBitcoin.Money SpentToNewChangeAmount;
            public bool Chosen;
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
