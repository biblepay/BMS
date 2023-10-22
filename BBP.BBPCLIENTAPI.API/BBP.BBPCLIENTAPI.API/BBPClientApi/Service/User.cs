using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BMSCommon.Model;
using NBitcoin.Secp256k1;
using static BMSCommon.Common;

namespace BBPAPI.Model
{
    public static class UserFunctions
    { 


        private static List<User> lCachedUsers = new List<User>();
        public static void EraseUserCache()
        {
            lCachedUsers.Clear();
        }
        public static User GetCachedUser(bool fTestNet, string sBBPAddress)
        {
            try
            {
                if (lCachedUsers == null || lCachedUsers.Count == 0)
                {
                    lCachedUsers = BBPAPI.Interface.Repository.GetDatabaseObjects<User>("user");
                }
                List<User> lUser = lCachedUsers.Where(s => s.BBPAddress == sBBPAddress).ToList();
                if (lUser.Count == 0)
                {
                    return null;
                }
                return lUser[0];
            }
            catch(Exception ex)
            {
                return new User();
            }
        }

        public static User GetUserByNickName(bool fTestNet, string sNickName)
        {
            List<User> lUsers = BBPAPI.Interface.Repository.GetDatabaseObjects<User>("user");
            List<User> lUser = lUsers.Where(s => s.NickName == sNickName).ToList();
            if (lUser.Count == 0) return new User();
            return lUser[0];
        }

        public static User GetUserByBBPAddress(bool fTestNet, string sBBPAddress)
        {
            List<User> lUsers = BBPAPI.Interface.Repository.GetDatabaseObjects<User>("user");
            List<User> lUser = lUsers.Where(s => s.BBPAddress == sBBPAddress).ToList();
            if (lUser.Count == 0) 
                return null;
            return lUser[0];
        }

        public static User GetUserByID(bool fTestNet, string sID)
        {
            List<User> lUsers = BBPAPI.Interface.Repository.GetDatabaseObjects<User>("user");
            List<User> lUser = lUsers.Where(s => s.id == sID).ToList();
            if (lUser.Count == 0)
                return null;
            return lUser[0];
        }

        public static User GetAndCacheUser(string sBBPPrivKey)
        {
			// If the user exists, get him, if not make him
            string sPubKey = BMSCommon.Encryption.GetPubKeyFromPrivKey(sBBPPrivKey, false);

			User uRow = BBPAPI.Model.UserFunctions.GetCachedUser(false, sPubKey);
			if (uRow == null)
			{
				User u = new User();
				//persist
				u.NickName = sPubKey;
                u.BBPPrivKeyMainNet = sBBPPrivKey;
                u.BBPAddress = sPubKey;
                bool fSaved = BBPAPI.Interface.Repository.PersistUser(u);
                return u;
			}
            else
            {
                uRow.BBPPrivKeyMainNet = sBBPPrivKey;
                return uRow;
            }
		}
        

        public static Dictionary<string, User> tdictUsers = new Dictionary<string, User>();
        public static Dictionary<string, User> mdictUsers = new Dictionary<string, User>();
        public static Dictionary<string, int> dictLastUserActivity = new Dictionary<string, int>();
        public static void SetLastUserActivity(bool fTestNet, string sERC20Address)
        {
            if (sERC20Address == null || sERC20Address == "")
                return;

            if (!dictLastUserActivity.ContainsKey(sERC20Address))
            {
                dictLastUserActivity.Add(sERC20Address, 0);
            }
            dictLastUserActivity[sERC20Address] = UnixTimestamp();
        }

        public static int GetLastUserActivity(bool fTestNet, string sERC20Address)
        {
            int nLastActivity = 0;

            if (String.IsNullOrEmpty(sERC20Address))
                return 0;

            if (dictLastUserActivity.Count < 1)
                return 0;

            dictLastUserActivity.TryGetValue(sERC20Address, out nLastActivity);
            return nLastActivity;
        }

        public static bool IsUserActive(bool fTestNet, string sERC20Address)
        {
            int nElapsed = UnixTimestamp() - GetLastUserActivity(fTestNet, sERC20Address);
            bool fActive = (nElapsed < (60 * 60));
            return fActive;
        }


    }
}


