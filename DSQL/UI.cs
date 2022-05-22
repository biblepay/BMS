using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BiblePay.BMS.DSQL
{

    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static bool ObjectExists(this ISession session, string key)
        {
            var value = session.GetString(key);
            return (value == null ? false : true);
        }
    }

    public static class UI
    {

        public static BMSCommon.WebRPC.DACResult SendBBP(HttpContext h, string sToAddress, double nAmount, string sOptPayload = "")
        {
            BMSCommon.Encryption.KeyType k = DSQL.UI.GetKeyPair(h);
            string sData = BMSCommon.WebRPC.GetAddressUTXOs(IsTestNet(h), k.PubKey);
            string sErr = "";
            string sTXID = "";
            NBitcoin.Crypto.BBPTransaction.PrepareFundingTransaction(IsTestNet(h), nAmount, sToAddress, k.PrivKey, sOptPayload, sData, out sErr, out sTXID);
            BMSCommon.WebRPC.DACResult r = new BMSCommon.WebRPC.DACResult();
            if (sErr != "")
            {
                r.Error = sErr;
                return r;
            }
            r = BMSCommon.WebRPC.SendRawTx(IsTestNet(h), sTXID);
            return r;
        }

        public static string GetFormData(string sFormData, string sFieldName)
        {
            string[] vFormData = sFormData.Split("<row>");
            for (int i = 0; i < vFormData.Length; i++)
            {
                string[] cFormData = vFormData[i].Split("<col>");
                if (cFormData.Length > 2)
                {
                    string sID = cFormData[1];
                    string sValue = cFormData[2];
                    if (sID.ToLower() == sFieldName.ToLower())
                    {
                        return sValue;
                    }
                }
            }
            return "";
        }

        public static void MsgBox(HttpContext h, string sTitle, string sHeading, string sBody, bool fRedirect)
        {
            h.Session.SetString("msgbox_title", sTitle);
            h.Session.SetString("msgbox_heading", sHeading);
            h.Session.SetString("msgbox_body", sBody);
            if (fRedirect)
                h.Response.Redirect("bbp/messagepage");
        }

        public static double QueryAddressBalance(bool fTestNet, string sAddress)
        {
            string sUTXOData = BMSCommon.WebRPC.GetAddressUTXOs(fTestNet, sAddress);

            return NBitcoin.Crypto.BBPTransaction.QueryAddressBalance(fTestNet, sAddress, sUTXOData);
        }
        public static string FormatUSD(double myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);
            return s;
        }

        public static string GetBioURL(HttpContext h)
        {
            BMSCommon.CryptoUtils.User u = DSQL.UI.GetUser(h);
            string sBIO = GetBioURL(u.BioURL);
            return sBIO;
        }
        public static string GetBioURL(string URL)
        {
            string empty = "/img/demo/avatars/emptyavatar.png";
            if (URL == null || URL == "")
                return empty;
            return URL;
        }


        public static bool IsAllowableExtension(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext.Length < 1) return false;
            ext = ext.Substring(1, ext.Length - 1).ToLower();
            string allowed = "jpg;jpeg;gif;bmp;png";
            string[] vallowed = allowed.Split(";");
            for (int i = 0; i < vallowed.Length; i++)
            {
                if (vallowed[i] == ext)
                    return true;
            }
            return false;
        }
        public static string ListToHTMLSelect(List<string> s, string sSelected)
        {
            string html = "";
            for (int i = 0; i < s.Count; i++)
            {
                string data = s[i];
                string narr = "";
                if (data.ToLower() == sSelected.ToLower())
                    narr = " SELECTED";
                string row = "<option " + narr + " value='" + data + "'>" + data + "</option>";
                html += row + "\r\n";
            }
            return html;
        }



        private static long nLastBalanceMain = 0;
        private static long nLastBalanceTest = 0;
        public static string GetAvatarBalance(HttpContext h, bool fEraseCache)
        {
            BMSCommon.CryptoUtils.User u = GetUser(h);
            long nLastBal = IsTestNet(h) ? nLastBalanceTest : nLastBalanceMain;
            long nElapsed = BMSCommon.Common.UnixTimestamp() - nLastBal;
            string sChain = GetChain(h);

            if (nElapsed < 60*3 && !fEraseCache)
            {
                return h.Session.GetString(sChain + "_balance");
            }
            double nBal = QueryAddressBalance(IsTestNet(h), u.BBPAddress);
            if (IsTestNet(h))
            {
                nLastBalanceTest = BMSCommon.Common.UnixTimestamp();
            }
            else
            {
                nLastBalanceMain = BMSCommon.Common.UnixTimestamp();
            }
            h.Session.SetString(sChain + "_balance", FormatUSD((double)nBal));
            return FormatUSD((double)nBal) + "";
        }


        public static string GetChain(HttpContext s)
        {
            string s1 = s.Session.GetString("Chain");
            if (s1==null)
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

        
        public static BMSCommon.CryptoUtils.User GetUser(HttpContext s)
        {
            BMSCommon.CryptoUtils.User u = s.Session.GetObject<BMSCommon.CryptoUtils.User>("User");
            if (u == null)
                u = new BMSCommon.CryptoUtils.User();


            
            if (u.ERC20Address=="" || u.ERC20Address == null)
            {
                // 5-14-2022
                s.Request.Cookies.TryGetValue("erc20address", out u.ERC20Address);
            }

            if (!u.LoggedIn && u.ERC20Address != "")
            {
                u = BMSCommon.CryptoUtils.DepersistUser(u.ERC20Address);
                s.Session.SetObject("User", u);
                if (u.ERC20Address == "" || u.ERC20Address == null)
                {
                    // 5-14-2022
                    s.Request.Cookies.TryGetValue("erc20address", out u.ERC20Address);
                }

            }

            BMSCommon.Encryption.KeyType k = DSQL.UI.GetKeyPair(s);
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

        public static void SetUser(BMSCommon.CryptoUtils.User u, HttpContext s)
        {
            s.Session.SetObject("User", u);
        }

        public static string GetLogInStatus(HttpContext s)
        {
            BMSCommon.CryptoUtils.User u = GetUser(s);
            string status = "";
            BMSCommon.Encryption.KeyType k = GetKeyPair(s);

            if (u==null || k.PubKey == null)
            {
                status = "LOGGED OUT";
            }
            else
            {
                status = "LOGGED IN";
            }
            return status;

        }

        public static BMSCommon.Encryption.KeyType GetKeyPair(HttpContext h)
        {
            string ERC20Signature;
            string ERC20Address;
            BMSCommon.Encryption.KeyType k = new BMSCommon.Encryption.KeyType();
            h.Request.Cookies.TryGetValue("erc20signature", out ERC20Signature);
            h.Request.Cookies.TryGetValue("erc20address", out ERC20Address);
            if (ERC20Signature != null)
            {
                DSQL.ERC712Authenticator b = new DSQL.ERC712Authenticator();
                bool f = b.VerifyERC712Signature(ERC20Signature, ERC20Address);
                if (f)
                {
                    k = BMSCommon.Encryption.DeriveKey(IsTestNet(h), ERC20Signature);
                    return k;
                }
            }
            return k;
        }
        public static string GetTemplate(string sName)
        {
            string sLoc = Path.Combine(BMSCommon.Database.msContentRootPath, "wwwroot/templates/" + sName);
            string data = System.IO.File.ReadAllText(sLoc);
            return data;
        }

        public static string GetModalDialog(string title, string body, string optjs="")
        {
            string data = GetTemplate("modal.htm");
            data = data.Replace("@title", title);
            data = data.Replace("@body", body);
            data = data.Replace("@modalid", "modalid1");
            data = data.Replace("@optjs", optjs);
            return data;
        }

        public static string GetAccordian(string id, string title, string body)
        {
            string data = GetTemplate("accordian.htm");
            data = data.Replace("@title", title);
            data = data.Replace("@body", body);
            data = data.Replace("@id", id);
            return data;
        }

        public static string GetNotificationItem(string id, string avatarURL, string message, string fullname, string timerelative, string status)
        {
            string data = GetTemplate("notificationrecord.htm");
            data = data.Replace("@id", id);
            data = data.Replace("@avatarURL", avatarURL);
            data = data.Replace("@message", message);
            data = data.Replace("@fullname", fullname);
            data = data.Replace("@status", status);
            data = data.Replace("@timerelative", "4 seconds ago");
            return data;
        }

        public static string GetNotifications()
        {
            string html = "";
            for (int i = 0; i < 14; i++)
            {
                string status = "status-danger";
                if (i == 0)
                    status = "status-warning";
                if (i == 1)
                    status = "status-danger";
                if (i == 2)
                    status = "statusinfo";
                if (i == 3)
                    status = "status";

                string row = GetNotificationItem(i.ToString(), "/img/demo/avatars/avatar-m.png", "Hello furry friend " + i.ToString(), "James Cheoo", "4 seconds in the future", status);
                html += row + "\r\n";
            }
            return html;
        }

        public static string GetBasicTable(string tableid, string title)
        {
            string data = GetTemplate("basictable.htm");
            data = data.Replace("@tableid", tableid);
            data = data.Replace("@tablename", title);
            string th = "<th>Col1</th><th>Col2</th><th>Col3</th>";
            string tr = "";
            for (int i = 0; i < 50; i++)
            {
                string row = "<tr><td>Val " + i.ToString() + "</td><td>col2 " + i.ToString() + "</td><td>value 3</td></tr>";
                tr += row + "\r\n";
            }
            data = data.Replace("@tableheaders", th);
            data = data.Replace("@tablerows", tr);
            return data;
        }



    }




}
