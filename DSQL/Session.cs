﻿using BBPAPI.Model;
using BiblePay.BMS.Extensions;
using BMSCommon;
using BMSShared;
using Google.Authenticator;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using static BiblePay.BMS.DSQL.UIWallet;
using static BMSCommon.Common;

namespace BiblePay.BMS.DSQL
{
    public static class SessionHelper
    {
        public static double GetSessionDouble(HttpContext h, string sKey)
        {
            string sChain = GetChain(h);
            double n = GetDouble(h.Session.GetString(sChain + "_" + sKey));
            return n;
        }

        public static void SetSessionDouble(HttpContext h, string sKey, double nValue)
        {
            string sChain = GetChain(h);
            h.Session.SetString(sChain + "_" + sKey, nValue.ToString());
        }
        public static string GetChain(HttpContext s)
        {
            string s1 = s.Session.GetString("Chain");
            if (s1 == null)
            {
                s.Session.SetString("Chain", "MAINNET");
                s1 = s.Session.GetString("Chain");
            }

            return s1;
        }

        public static string GetPBMode(HttpContext s)
        {
            string s1 = s.Session.GetString("PortfolioBuilderLeaderboardMode");
            if (s1 == null) return "Summary";
            return s1;
        }
        public static bool IsTestNet(HttpContext s)
        {
            string sChain = GetChain(s);
            return sChain == "MAINNET" ? false : true;
        }
        public static string GetChainColor(HttpContext s1)
        {
            string sChain = GetChain(s1);
            if (sChain == "TESTNET")
                return "background-color:LIME;";
            return "";
        }
        // When we move away from ERC-20 we can delete this one
        public static User GetUser(HttpContext s)
        {
            string sKey = IsTestNet(s) ? "tUser" : "User";
            User u = s.Session.GetObject<User>(sKey);
            if (u == null)
                u = new User();

            if (String.IsNullOrEmpty(u.ERC20Address))
            {
                string e = String.Empty;
                s.Request.Cookies.TryGetValue("erc20address", out e);
                u.ERC20Address = e;
            }

            if (!u.LoggedIn && !String.IsNullOrEmpty(u.ERC20Address))
            {
                u = User.GetCachedUser(IsTestNet(s), u.ERC20Address);
                if (u==null)
                {
                    return u;
                }    
                s.Session.SetObject(sKey, u);
                if (String.IsNullOrEmpty(u.ERC20Address))
                {
                    string f = String.Empty;
                    s.Request.Cookies.TryGetValue("erc20address", out f);
                    u.ERC20Address = f;
                }
            }

            Encryption.KeyType k = DSQL.UIWallet.GetKeyPair(s);
            u.BBPAddress = k.PubKey;
            if (u.ERC20Address != null && u.ERC20Address.Length > 20)
            {
                u.LoggedIn = true;
            }
            else
            {
                u.LoggedIn = false;
            }
            return u;
        }



        public static User GetUserMFA(HttpContext s, string sNickName, string sMFACode)
        {
            string sKey = IsTestNet(s) ? "tUser" : "User";
            User u = s.Session.GetObject<User>(sKey);
            if (u == null)
                u = new User();

            if (u.LoggedIn)
            {
                return u;
            }
            if (!u.LoggedIn)
            {
                u = User.GetUserByNickName(IsTestNet(s), sNickName);

                bool isValid = BBPAPI.ERCUtilities.ValidateMFA(u, sMFACode);
                
                if (isValid)
                {
                    u.LoggedIn = true;
                    s.Session.SetObject(sKey, u);
                }
                else
                {
                    u.LoggedIn = false;
                }
            }

            return u;
        }





        public static void SetUser(User u, HttpContext s)
        {
            string sKey = IsTestNet(s) ? "tUser" : "User";
            s.Session.SetObject(sKey, u);
        }

        public static string GetLogInAction(HttpContext s)
        {
            string s1 = GetLogInStatus(s);
            string sLogOut = "var c = confirm('Are you sure you want to Log Out?'); if (c) { DoCallback('profile_logout','','profile/processdocallback'); }";
            string sLoc = s1 == "LOGGED OUT" ? "location.href='/profile/profile';" : sLogOut;
            return sLoc;
        }

        public static string GetNickName(HttpContext s)
        {
            string s1 = GetLogInStatus(s);
            if (s1 == "LOGGED IN")
            {
                User u = s.GetCurrentUser();
                return u.NickName;
            }
            else
            {
                return "Guest";
            }

        }
        public static string GetLogInStatus(HttpContext s)
        {
            User u = GetUser(s);
            string status = String.Empty;
            Encryption.KeyType k = GetKeyPair(s);

            if (u == null || k.PubKey == null)
            {
                status = "LOGGED OUT";
            }
            else
            {
                status = "LOGGED IN";
            }
            return status;
        }




    }
}
