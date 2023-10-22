using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.Encryption;

namespace BMSCommon.Model
{
	public class Pin
	{
		// Todo: this can be our CDN:  bbp.click/filename -> reversehash -> object

		public string ID = String.Empty;
		public DateTime Added;
		public string UserID = String.Empty;
		public string URL = String.Empty;
		public string Path = String.Empty;
		public long Size;
		public string FileHash = String.Empty;
		public string BBPAddress = String.Empty;
		public string GetID()
		{
			return GetSha256HashI(Path);
		}



	}

}
