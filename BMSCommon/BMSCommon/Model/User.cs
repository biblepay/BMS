using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.Encryption;

namespace BMSCommon.Model
{
	public class Permission
	{
		public int Administrator { get; set; }
		public int PowerUser { get; set; }
		public int Representative { get; set; }
		public Permission()
		{
			Administrator = 0;
			PowerUser = 0;
			Representative = 0;
		}
	}
	public class User
	{
		public Permission Permissions { get; set; }
		public string ERC20Address { get; set; }
		public string id { get; set; }
		public string EmailAddress { get; set; }
		public string MFA { get; set; }
		public DateTime Added { get; set; }
		public DateTime Updated { get; set; }
		public string NickName { get; set; }
		public string BioURL { get; set; }
		public string BBPAddress { get; set; }
		public bool LoggedIn = false;

		public int EmailAddressVerified { get; set; }
		public bool DesktopUser { get; set; }
		public bool TestNet { get; set; }
		public string BBPPrivKeyMainNet { get; set; }
		public string BBPPrivKeyTestNet { get; set; }
		public BBPKeyPair GetKeyPair()
		{
			BBPKeyPair k = new BBPKeyPair();
			k.PrivKey = GetPrivateKey();
			k.PubKey = GetPublicKey();
			return k;
		}
		public string GetPrivateKey()
		{
			string sPrivKey = TestNet ? BBPPrivKeyTestNet : BBPPrivKeyMainNet;
			return sPrivKey;
		}
		public string GetPublicKey()
		{
			string sPrivKey = GetPrivateKey();
			string sPubKey = Encryption.GetPubKeyFromPrivKey(sPrivKey, TestNet);
			return sPubKey;
		}
		public User()
		{
			id = String.Empty;
			EmailAddress = String.Empty;
			NickName = String.Empty;
			ERC20Address = String.Empty;
			BBPPrivKeyMainNet = String.Empty;
			BBPPrivKeyTestNet = String.Empty;
			Permissions = new Permission();
		}

	}
}


