using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BMSCommon;
using NBitcoin;
using static BMSCommon.Encryption;
using static BMSCommon.Model.BitcoinSyncModel;

namespace BMSCommon.Model
{
    public class BBPNetAddress
    {
        public string Address {get;set;}
        public bool TestNet { get; set; }
        public BBPNetAddress()
        {
            Address = String.Empty;
        }
    }
	public class BBPNetHeight
    {
        public bool TestNet = false;
        public int Height = 0;
    }
    public class PhoneRegionCountryAddress
    {
        public long RegionID = 0;
        public string CountryState = String.Empty;
        public string Address = String.Empty;
        public PhoneRegionCountryAddress()
        {

        }
    }

    public class HashPath
    {
        public string Hash = String.Empty;
        public string Path = String.Empty;
    }
    public class ChatNotification
    {
        public bool TestNet = false;
        public string UID = String.Empty;
        public string FromUser = String.Empty;
        public string Body = String.Empty;
        public string Type = String.Empty;
        public string URL = String.Empty;
        public ChatNotification()
        {

        }
    }
    
    public class NewPhoneUser
    {
        public string Address = String.Empty;
        public long NewUserID = 0;
        public string NewUserName = String.Empty;
        public string BBPPrivateKey = String.Empty;
        public string PhoneNumber = String.Empty;
        public NewPhoneUser()
        {

        }
	}

    public class PhoneUserCountry
    {
        public int UserId = 0;
        public string NewCountry = String.Empty;
        public PhoneUserCountry()
        {

        }

	}
    public class VoiceGreeting
    {
        public int UserID = 0;
        public string Greeting = String.Empty;
        public int Duration = 0;
        public VoiceGreeting()
        {

        }

	}
    public class PhoneCallerDestination
    {
        public string CallerID = String.Empty;
        public string Destination = String.Empty;
        public string Body = String.Empty;
    }
    public class BBPAddressKey
    {
        public string Address = String.Empty;
        public string PrivateKey = String.Empty;
    }
    public class SMSMessage
    {
        public string From = String.Empty;
        public string To = String.Empty;
        public string Message = String.Empty;
        public SMSMessage()
        {

        }
	}




	public class HeaderPack
	{
		public List<string> listKeys = new List<string>();
		public List<string> listValues = new List<string>();
	}

	public class GetBusinessObject
    {
        public string ParentID = String.Empty;
        public bool TestNet = false;
    }

    public class PhoneUserMappingUpdate
    {
        public User User { get; set; }
        public int PhoneUser { get; set; }
        public string PhoneNumber { get; set; }

        public PhoneUserMappingUpdate()
        {
            PhoneUser = 0;
            PhoneNumber = String.Empty;
        }

	}

    public class DatabaseQuery
    {
        public string TableName = String.Empty;
        public string OrderBy = String.Empty;
        public string FullyQualifiedName = String.Empty;
        public object BusinessObject = null;
        public string Key = String.Empty;
        
        public DatabaseQuery()
        {

        }
    }

	public class NewPhoneObject
	{
		public long RegionID { get; set; }
		public string CountryState { get; set; }
		public string BBPAddress { get; set; }
		public NewPhoneObject()
		{
			RegionID = 0;
			CountryState = String.Empty;
			BBPAddress = String.Empty;
		}

	}

    public class UploadFileResult
    {
        public string URL { get; set; }
        public string Error { get; set; }
        public UploadFileResult()
        {
            URL = String.Empty;
            Error = String.Empty;   
        }
    }

    public class UploadFileObject
    {
        public User User {get;set;}
        public string SourceFilePath { get; set; }
        public string StorjDestinationPath { get; set; }
        public string OverriddenBBPPrivateKey { get; set; }
        public byte[] FileBytes { get; set; }
        public UploadFileObject()
        {
            SourceFilePath   = String.Empty;    
            StorjDestinationPath = String.Empty;
            OverriddenBBPPrivateKey = String.Empty;
            FileBytes = null;
            User = new User();
        }
    }

	public class PhoneUser
    {
        public int UserId { get; set; }
        public bool GoodStanding { get; set; }
        public string BBPUserName { get; set; }
        public string Greeting { get; set; }
        public int AnswerDuration { get; set; }
        public string BBPPK { get; set; }
        public string BBPPW { get; set; }
        public string BBPAddress { get; set; }
        public double WalletBalance { get; set; }
        public string Error { get; set; }
        public double OutstandingOwed { get; set; }
        public string PhoneNumber { get; set; }
        public PhoneUser()
        {

        }

    }



    public class MasternodeListItem
    {
        public string proTxHash { get; set; }
        public string address { get; set; }
        public string Outpoint { get; set; }
        public string payee { get; set; }
        public string status { get; set; }
        public int lastpaidtime { get; set; }
        public int lastpaidblock { get; set; }
        public string owneraddress { get; set; }
        public string pubkeyoperator { get; set; }
        public string collateraladdress { get; set; }
        public string votingaddress { get; set; }

        public MasternodeListItem()
        {
            votingaddress = String.Empty;
            proTxHash = String.Empty;
            payee = String.Empty;
            status = String.Empty;
            lastpaidtime = 0;
            owneraddress = String.Empty;
            pubkeyoperator = String.Empty;
            Outpoint = String.Empty;
        }

    }
    public struct StatusObject
    {
        public string URL;
        public int BMS_VERSION;
        public int COMMON_VERSION;
        public string Status;
        public double Synced_Count;
        public double File_Count;
        public double Synced_Percent;
        public string EOF;
        public int Memory_Pool_Count;
        public string Best_Block_Hash_Main;
        public int Block_Count_Main;
        public string Best_Block_Hash_Test;
        public int Block_Count_Test;
        public List<string> RPCErrorList;
        public int TESTNET_RPC_HEIGHT;
        public int MAINNET_RPC_HEIGHT;
    };

    public struct MobileAPI1
    {
        public double BTCUSD;
        public double BBPUSD;
        public string BBPBTC;
    }

    public class BBPNetHex
    {
        public bool TestNet;
        public string Hex = String.Empty;
    }

    public class DACResult
    {
        public string TXID = String.Empty;
        public bool Result;
        public string Error = String.Empty;
        public string Response = String.Empty;
    }
    
    public class BabyObject
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public BabyObject()
        {
            Field1  = String.Empty;
            Field2 = String.Empty;  
        }

    }
    public class SendMoneyRequest
    {
        public bool TestNet = false;
        public double nAmount = 0;
        public string sToAddress = String.Empty;
        public string sOptPayload = String.Empty;
        public string PrivateKey = String.Empty;
	}

        public struct ServerToClient
        {
            public string returnbody;
            public string returntype;
            public string returnurl;
        }


        public struct PriceQuote
        {
            public string Price;
            public string XML;
        }

        public struct UnchainedReply
        {
            public string Error { get; set; }

            public string URL { get; set; }
            public string HLSURL { get; set; }
            public int Result = 0;
            public string UserID = String.Empty;
            public double version = 0;

            public UnchainedReply()
            {
                Error = String.Empty;
                URL = String.Empty;
                HLSURL = String.Empty;
            }
        }


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


        public class SystemKey
        {
            public string id { get; set; }
            public string Value { get; set; }
            public DateTime Added { get; set; }
            public int nUnixTimeAdded { get; set; }
        }
        public class price1
        {
            public string Ticker { get; set; }
            public double Amount { get; set; }
            public double AmountUSD { get; set; }
        }

        public class EmailAccount
        {
            public string UserName { get; set; }
            public string Domain { get; set; }
            public string BBPAddress { get; set; }
            public string Password { get; set; }
            public DateTime Added { get; set; }
            public int Enabled { get; set; }
             

        }
        public class TurnkeySanc
        {
            public string id { get; set; }
            public string erc20address { get; set; }
            public DateTime? Added { get; set; }
            public string BBPAddress { get; set; }
            public string Nonce { get; set; }
            public string LastPaid { get; set; }
            public double Balance { get; set; }
            public string Signature { get; set; }

        }

    public class TempleDetail 
    {
        public string templeid { get; set; }
        public DateTime? added { get; set; }
        public string name { get; set; }
        public string proregtxid { get; set; }
        public string utxo { get; set; }
        public string ip { get; set; }
        public int sequence { get; set; }
        public string userid { get; set; }

    }

}


