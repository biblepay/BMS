using BBPAPI.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto.Tls;
using System.Threading.Tasks;
using BMSCommon;
using BMSCommon.Model;
using NBitcoin;
using Npgsql;

namespace BBPAPI
{
    public static class Globals
    {

		public static User _DBUser2 = new User();
	}

	public static class ServiceInit
    {

        // Set the User, set the user permissions, set the user private key
        private static bool fInit = false;
        public static async Task<bool> Init()
        {
            if (fInit)
            {
                return true;
            }
            fInit = true;
            if (false)
            {
            }

            await SetBBPPrivKey();

            return true;
        }

        /*
        public static Encryption.KeyType GetUserKeyPair(User u, bool fTestNet)
        {
            Encryption.KeyType k = new Encryption.KeyType();
        

            k = Encryption.GetKeyPairFromPrivKey(fTestNet
                ? u.BBPPrivKeyTestNet
                : u.BBPPrivKeyMainNet,  fTestNet);
            return k;
        }
    */


		


		private static void FinishFirewallPush(Object o)
        {
            User u = (User)o;
            // Push it through the firewall
            try
            {
                /*
                string sURL = "https://unchained.biblepay.org/BMS/Firewall";
                //sURL = "https://localhost:8443/BMS/Firewall";

                string sXML = "<firewallprivkey>"
                              + Encryption.EncryptAES256(u.BBPPrivKeyMainNet,
                                  "FirewallPrivKey")
                              + "</firewallprivkey>";

                //string sResp = Service.PostToEndpoint(sURL, sXML, null).Result;
                Common.Log("P2E::Resp::" + sResp);
                */

            }
            catch (Exception ex)
            {
                Common.Log("SetBBPPrivKey::" + ex.Message);
            }
            Service._mlSanctuaries = BBPAPI.Interface.WebRPC.GetMasternodeList(false).Result;
        }
        internal static async Task<bool> SetBBPPrivKey()
        {
            
            // This is the area that Registers the User with Unchained
            // It will also whitelist the users Home IP address with the sancs

            var sPath = BMSCommon.Common.GetBiblePayConfigFile("unchained.conf");
            Common.Log("v1.2::Looking for BBP Config in " + sPath);

            if (!System.IO.File.Exists(sPath))
            {
                return false;
            }

            BBPAPI.Globals._DBUser2.BBPPrivKeyMainNet = Common.GetConfigKeyValue("unchained_mainnet_privkey", sPath);
            BBPAPI.Globals._DBUser2.BBPPrivKeyTestNet = Common.GetConfigKeyValue("unchained_testnet_privkey", sPath);
			BBPAPI.Globals._DBUser2.BBPAddress = NBitcoin.Crypto.BBPTransaction.GetPubKeyFromPrivKey(false, BBPAPI.Globals._DBUser2.BBPPrivKeyMainNet);
            if (BBPAPI.Globals._DBUser2.BBPPrivKeyMainNet != String.Empty)
            {
                // this is a desktop user
                // todo set bool for desktop
                BBPAPI.Globals._DBUser2.DesktopUser = true;
				// If db user is null:
				User uRow = BBPAPI.Model.UserFunctions.GetCachedUser(false, BBPAPI.Globals._DBUser2.BBPAddress);
				if (uRow == null)
				{
                    User u = new User();
					//persist
					u.NickName = u.BBPAddress;
					bool fSaved = BBPAPI.Interface.Repository.PersistUser(u);
                    BBPAPI.Globals._DBUser2 = u;
				}
                else
                {
                    BBPAPI.Globals._DBUser2 = uRow;
                        BBPAPI.Globals._DBUser2.DesktopUser = true;
					BBPAPI.Globals._DBUser2.BBPPrivKeyMainNet = Common.GetConfigKeyValue("unchained_mainnet_privkey", sPath);
					BBPAPI.Globals._DBUser2.BBPPrivKeyTestNet = Common.GetConfigKeyValue("unchained_testnet_privkey", sPath);
                        BBPAPI.Globals._DBUser2.LoggedIn = true;

				}
			}
			System.Threading.Thread s1 = new System.Threading.Thread(FinishFirewallPush);
            s1.Start(BBPAPI.Globals._DBUser2);
            return true;
        }
        
    }
}
