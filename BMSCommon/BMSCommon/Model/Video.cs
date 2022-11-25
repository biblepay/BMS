using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.BitcoinSyncModel;

namespace BMSCommon.Models
{
	public class Video
	{
		public string Added;
		public int Time;
		public int Version = 1;
		public string table = "Video";
		public string Title;
		public string Description;
		public string Category;
		public string Cover;
		public string Source;
		public double Duration;
		public string id;
		public async Task<bool> Save(bool fTestNet)
		{
			this.Time = BMSCommon.Common.UnixTimestamp();
			string sData = Newtonsoft.Json.JsonConvert.SerializeObject(this);
			await StorjIO.UplinkStoreDatabaseData("video", this.id, sData, String.Empty);
			return true;
		}
		public static async Task<List<Video>> Get(bool fTestNet, string id)
		{
			List<Video> l = await StorjIO.GetDatabaseObjects<Video>("video");
			if (id != String.Empty)
			{
				l = l.Where(s => s.id == id).ToList();
			}
			return l;
		}
	}

}
