using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace BMSCommon
{
    public static class DSQL
    {
        public static string GetSQLTemplate(string sName)
        {
            //string projectRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string sLoc = Path.Combine(Database.msContentRootPath, "wwwroot/templates/" + sName);
            string data = System.IO.File.ReadAllText(sLoc);
            return data;
        }

        public static string GetURL(string sPath)
        {
		    string sNewURL = BMSCommon.Common.NormalizeURL("https://bbpipfs.s3.filebase.com/" + sPath);
            return sNewURL;
        }
        public static List<string> QueryIPFSFolderContents(bool fTestNet, string sPath, string sDelim, string sUSERID)
        {
			string sTable = fTestNet ? "tpin" : "pin";
            string sql = "Select * from " + sTable;
            if (sUSERID != "")
            {
                sql += " where userid=@userid;";
            }
            MySqlCommand cmd1 = new MySqlCommand(sql);
            cmd1.Parameters.AddWithValue("@userid", sUSERID);

            DataTable dt = Database.GetDataTable(cmd1);
            List<string> l = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sURL = dt.Rows[i]["URL"].ToString();
                bool fTS = sURL.Contains(".ts");
                if (true)
                {
                    l.Add(sURL);
                }
            }
            return l;
        }

    }

	public class NFT
	{

		public enum NFTCategory
		{
			GENERAL,
			CHRISTIAN,
			ORPHAN
		};


		public string Name { get; set; }
		public string Action { get; set; }
		public string Description { get; set; }
		public string AssetURL { get; set; }
		public string AssetHQURL { get; set; }
		public string AssetBIO { get; set; }
		public int AssetMonths { get; set; }
		public string JSONURL { get; set; }
		public string TokenID { get; set; }
		public int time { get; set; }

		public string Type { get; set; }
		public double MinimumBidAmount { get; set; }
		public double ReserveAmount { get; set; }
		public double BuyItNowAmount { get; set; }
		public string OwnerERC20Address { get; set; }
		public string OwnerBBPAddress { get; set; }

		public int nIteration { get; set; }
		public string LastOwnerERC20Address { get; set; }
		public int Marketable { get; set; }
		public int Deleted { get; set; }
		public string Hash { get; set; }
		public string PrimaryKey = "Hash";

		public int Version { get; set; }
		public string TXID { get; set; }
		public string Chain { get; set; }
		public string table = "NFT";

		public string GetHash()
		{
			return BMSCommon.Encryption.GetSha256HashI(AssetURL);
		}

		public NFT()
		{
			Marketable = 0;
			Action = "CREATE";
			OwnerERC20Address = "";
			OwnerBBPAddress = "";
		}

		public void Save(bool fTestNet)
		{
			this.Hash = GetHash();
			this.time = BMSCommon.Common.UnixTimestamp();
			this.Version = 3;
			BMSCommon.CryptoUtils.Transaction t = new BMSCommon.CryptoUtils.Transaction();
			t.Data = Newtonsoft.Json.JsonConvert.SerializeObject(this);
			BMSCommon.BitcoinSync.AddToMemoryPool(fTestNet, t);

			List<CryptoUtils.Transaction> txList = new List<CryptoUtils.Transaction>();
			txList.Add(t);
		}
		public NFTCategory GetCategory()
		{
			NFTCategory n1 = 0;
			if (this.Type.ToLower() == "christian")
			{
				n1 = NFTCategory.CHRISTIAN;
			}
			else if (this.Type.ToLower() == "orphan")
			{
				n1 = NFTCategory.ORPHAN;
			}
			else
			{
				n1 = NFTCategory.GENERAL;
			}
			return n1;
		}

		public double LowestAcceptableAmount()
		{
			double nAcceptable = 100000000;
			if (ReserveAmount > 0 && BuyItNowAmount > 0)
			{
				// This is an Auction AND a buy-it-now NFT, so accept the lower of the two
				nAcceptable = Math.Min(ReserveAmount, BuyItNowAmount);
			}
			else if (ReserveAmount > 0 && BuyItNowAmount == 0)
			{
				// This is an auction (but not a buy it now)
				nAcceptable = ReserveAmount;
			}
			else if (BuyItNowAmount > 0 && ReserveAmount == 0)
			{
				nAcceptable = BuyItNowAmount;
			}
			return nAcceptable;
		}

		private static bool ContainsKey(List<NFT> n, string id)
        {
			for (int i = 0; i < n.Count; i++)
            {
				if (n[i].GetHash() == id)
					return true;
            }
			return false;
        }

		public static List<NFT> GetListOfNFTs(string sChain, string sUserERC20Address, string sTypes)
		{

			List<string> lBanned = new List<string>();

			//for (int i = 0; i < oNftBlacklist.Count; i++)
			//{
			//	lBanned.Add(oNftBlacklist[i].hash);
			//}

			int MIN_NFT_VERSION = 3;
			List<NFT> nList = new List<NFT>();
			//string sChain = fTestNet ? "test" : "main";
			string sFilter = "";
			if (sTypes == "my")
			{
				sFilter = " and OwnerERC20Address='" + sUserERC20Address + "' and deleted=0";
			}
			else if (sTypes != "")
			{
				sFilter = "and type in ('" + sTypes + "') and marketable=1 and deleted=0";
			}
			string sChainClause = "";
			if (sChain != "")
            {
				sChainClause = " and Chain='" + sChain + "'";
            }
			string sNFTTable = sChain == "test" ? "tNFT" : "NFT";

			string sql = "Select * from " + sNFTTable + " where version >= 2 " + sChainClause + " " + sFilter + ";";
			MySqlCommand m1 = new MySqlCommand(sql);
			DataTable dt = BMSCommon.Database.GetDataTable(m1);
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				NFT n = new NFT();
				n.OwnerERC20Address = dt.Rows[i].Field<string>("OwnerERC20Address");
				n.OwnerBBPAddress = dt.Rows[i].Field<string>("OwnerBBPAddress");
				if (n.OwnerBBPAddress != null)
				{
					n.Name = dt.Rows[i]["Name"].ToString();
					n.AssetURL = dt.Rows[i]["AssetURL"].ToString();
					n.Deleted = (int)BMSCommon.Common.GetDouble(dt.Rows[i]["Deleted"].ToString());
					n.Marketable = (int)BMSCommon.Common.GetDouble(dt.Rows[i]["Marketable"].ToString());
					n.Chain = dt.Rows[i]["Chain"].ToString();
					n.TokenID = dt.Rows[i].Field<string>("TokenID");
					n.Type = dt.Rows[i].Field<string>("Type");
					n.Description = dt.Rows[i]["Description"].ToString();
					n.BuyItNowAmount = dt.GetColDouble(i, "BuyItNowAmount");
					n.ReserveAmount = dt.GetColDouble(i, "ReserveAmount");
					n.Version = (int)dt.GetColDouble(i, "Version");
					if (n.Deleted == 0 && n.Version >= MIN_NFT_VERSION)
					{
						if (!ContainsKey(nList, n.GetHash()))
                        {
							nList.Add(n);

						}
					}
				}
			}
			return nList;
		}

		public static NFT GetNFT(string sChain, string sID)
		{
			List<NFT> l = GetListOfNFTs(sChain, null, "");
			for (int i = 0; i < l.Count; i++)
			{
				if (l[i].GetHash() == sID)
					return l[i];
			}
			return null;
		}
	}

}
