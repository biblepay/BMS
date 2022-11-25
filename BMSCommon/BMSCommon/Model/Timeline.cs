using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.BitcoinSyncModel;

namespace BMSCommon.Models
{
	public class Timeline
	{
		public string Added;
		public int Time;
		public string Body;
		public string dataPaste;
		public string ERC20Address;
		public string BBPAddress;
		public string ParentID;
		public string id;
		public int Version = 2;
		public string table = "Timeline";
		public async Task<bool> Save(bool fTestNet)
		{
			this.Time = BMSCommon.Common.UnixTimestamp();
			this.Version = 1;
			this.id = Guid.NewGuid().ToString(); 
			//BitcoinSyncTransaction t = new BitcoinSyncTransaction();
			string sData = Newtonsoft.Json.JsonConvert.SerializeObject(this);
			await StorjIO.UplinkStoreDatabaseData("timeline", this.id, sData, String.Empty);
			return true;
		}

		public static async Task<List<Timeline>> Get(bool fTestNet, string sParentID)
		{
			List<Timeline> l = await StorjIO.GetDatabaseObjects<Timeline>("timeline");
			l = l.Where(s => s.ParentID == sParentID).ToList();
			l = l.OrderByDescending(s => Convert.ToDateTime(s.Added)).ToList();
			return l;
		}
	}
	public class UTXOPosition
	{
		public string Symbol;
		public string Added;
		public int Time;
		public string ERC20Address;
		public string BBPAddress;
		public string ForeignAddress;
		public string PrimaryKey = "ForeignAddress";
		public int Version = 1;
		public string table = "UTXOPosition";
		public string Hash = "";

		public string GetHash()
		{
			return BMSCommon.Encryption.GetSha256HashI(ERC20Address + ForeignAddress);
		}

		public void Save(bool fTestNet)
		{
			this.Hash = GetHash();
			this.Time = BMSCommon.Common.UnixTimestamp();
			this.Version = 1;
			BitcoinSyncTransaction t = new BitcoinSyncTransaction();
			t.Data = Newtonsoft.Json.JsonConvert.SerializeObject(this);
			BMSCommon.BitcoinSync.AddToMemoryPool3(fTestNet, t);
		}
	}


}
