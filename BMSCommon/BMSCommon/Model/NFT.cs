using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.Encryption;

namespace BMSCommon.Model
{


	public class NFTBuy
	{
		public bool TestNet = false;
		public string ID = string.Empty;
		public double Amount = 0;
		public string PrivateKey = String.Empty;
		public User Buyer;
		public NFT NFT;
		
		public NFTBuy()
		{
			PrivateKey = String.Empty;
			Buyer = new User();
			NFT = new NFT();
		}
	}
	public class NFTSearch
	{
		public string Chain = String.Empty;
		public string Address = String.Empty;
		public string Types = String.Empty;
		public string ID = String.Empty;
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
		public string id { get; set; }
		public DateTime updated { get; set; }
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
		public int hidden { get; set; }
		public int Version { get; set; }
		public string TXID { get; set; }
		public string Chain { get; set; }
		public string table = "NFT";

		public string GetHash()
		{
			return Encryption.GetSha256HashI(AssetURL);
		}

		public NFT()
		{
			Marketable = 0;
			Action = "CREATE";
			OwnerERC20Address = "";
			OwnerBBPAddress = "";
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

	}

}
