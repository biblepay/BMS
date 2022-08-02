using BMSCommon;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using static BMSCommon.CryptoUtils;

namespace BiblePay.BMS.DSQL
{

    public static class TimeUtility
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;
        public static string GetRelativeTime(DateTime yourDate)
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - yourDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }
    }


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

        public class ChatItem
        {
            public string body;
            public DateTime time;
            public string To;
            public string From;
        }

        public class ChatSession
        {
            public List<ChatItem> chats = new List<ChatItem>();
        }

        public static Dictionary<string, ChatSession> dictChats = new Dictionary<string, ChatSession>();

        public static bool PersistDatabaseChatItem(bool fTestNet, ChatItem ci)
        {
            string sql = "insert into chat (id,added,Recipient,Sender,body) values (uuid(), now(), @recipient, @sender, @body);";
            MySqlCommand m1 = new MySqlCommand(sql);
            m1.Parameters.AddWithValue("@recipient", ci.To);
            m1.Parameters.AddWithValue("@sender", ci.From);
            m1.Parameters.AddWithValue("@body", ci.body);
            bool fIns = BMSCommon.Database.ExecuteNonQuery(m1);
            return fIns;
        }

        public static void AddChatItem(bool fTestNet, ChatItem ci, bool fPersist)
        {
            if (!dictChats.ContainsKey(ci.From))
            {
                ChatSession cs = new ChatSession();
                dictChats.Add(ci.From, cs);
            }
            if (!dictChats.ContainsKey(ci.To))
            {
                ChatSession cs1 = new ChatSession();
                dictChats.Add(ci.To, cs1);
            }
            DSQL.UI.dictChats[ci.From].chats.Add(ci);
            DSQL.UI.dictChats[ci.To].chats.Add(ci);
            string sURL = "/bbp/chat";
            if (fPersist)
            {
                InsertNotification(fTestNet, ci.To, ci.From, "You have received a chat message", "chat", sURL);
                PersistDatabaseChatItem(fTestNet, ci);
            }
        }

        public static bool chat_depersisted = false;
        public static void DepersistChatItems(bool fTestNet)
        {
            string sql = "Select * from chat order by Added;";
            MySqlCommand m1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable(m1);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ChatItem ci = new ChatItem();
                ci.body = dt.Rows[i]["body"].ToString();
                ci.From = dt.Rows[i]["sender"].ToString();
                ci.To = dt.Rows[i]["recipient"].ToString();
                ci.time = Convert.ToDateTime(dt.Rows[i]["Added"]);
                AddChatItem(fTestNet, ci, false);

            }
            chat_depersisted = true;
        }

        public struct ServerToClient
        {
            public string returnbody;
            public string returntype;
            public string returnurl;
        }

        public class DropDownItem
        {
            public string key;
            public string text;
            public DropDownItem(string _key, string _text)
            {
                key = _key;
                text = _text;
            }
        }

        public static bool IsShadyAddress(string url)
        {
            try
            {

                Uri myUri = new Uri(url);
                string host = myUri.Host;
                bool fContainsNumbers = host.All(char.IsDigit);
                if (fContainsNumbers)
                {
                    return true;
                }

                IPHostEntry hostEntry;
                hostEntry = Dns.GetHostEntry(host);
                IPAddress[] ipv4Addresses = Array.FindAll(
                        Dns.GetHostEntry(host).AddressList,
                            a => a.AddressFamily == AddressFamily.InterNetwork);

                IPAddress[] ipv4MyAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                //DNS supports more than one record
                for (int i = 0; i < hostEntry.AddressList.Length; i++)
                {
                    bool fIsLoopback = IPAddress.IsLoopback(hostEntry.AddressList[i]);
                    if (fIsLoopback)
                    {
                        return true;
                    }

                }
                for (int i = 0; i < ipv4Addresses.Length; i++)
                {
                    bool fIsLoopback = IPAddress.IsLoopback(ipv4Addresses[i]);
                    if (fIsLoopback)
                    {
                        return true;
                    }
                    for (int j = 0; j < ipv4MyAddresses.Length; j++)
                    {
                        if (ipv4Addresses[i].ToString() == ipv4MyAddresses[j].ToString())
                            return true;
                    }

                }
                string s99 = "";

            }
            catch (Exception ex)
            {
                return true;
            }
            return false;
        }


        public static async Task<string> Scrapper(string url)
        {
            try
            {
                url = HttpUtility.UrlDecode(url);
                ScrapingBrowser browser = new ScrapingBrowser();
                WebPage page;
                bool fShady = IsShadyAddress(url);
                bool fHTTProtocols = false;
                if (url.Contains("https://") || url.Contains("http://"))
                {
                    fHTTProtocols = true;
                }
                if (url.Contains("127.0.0.1") || url.Contains("localhost") || url.Contains("//127") || url.Contains("local"))
                {
                    url = "";
                }
                if (fShady || !fHTTProtocols)
                {
                    url = "";
                }
                string webUrl = url;
                page = await browser.NavigateToPageAsync(new Uri(webUrl));

                var title = page.Html.SelectSingleNode("//meta[@property='og:title']")?.GetAttributeValue("content", string.Empty);
                if (string.IsNullOrEmpty(title))
                    title = page.Html.SelectSingleNode("//title")?.InnerText;

                var description = page.Html.SelectSingleNode("//meta[@property='og:description']")?.GetAttributeValue("content", string.Empty);
                if (string.IsNullOrEmpty(description))
                    description = page.Html.SelectSingleNode("//meta[@name='description']")?.GetAttributeValue("content", string.Empty);

                var image = page.Html.SelectSingleNode("//meta[@property='og:image']")?.GetAttributeValue("content", string.Empty);
                if (string.IsNullOrEmpty(image))
                    image = page.Html.SelectNodes("//img").FirstOrDefault().GetAttributeValue("src", string.Empty);

                string sDiv = "<div>Title: " + title + "<br>Description: " + description + "<br><img src='" + image + "' /></div>";
                return sDiv;
            }catch(Exception ex)
            {
                return "";
            }
        }
        public static BMSCommon.WebRPC.DACResult SendBBP(HttpContext h, string sToAddress, double nAmount, string sOptPayload = "", string sOptNonce = "")
        {
            BMSCommon.Encryption.KeyType k = DSQL.UI.GetKeyPair(h,sOptNonce);
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

        public static BMSCommon.WebRPC.DACResult SendBBPOldMethod(bool fTestNet, string sType, string sToAddress, double nAmount, string sPayload)
        {

            string sPackaged = BMSCommon.WebRPC.PackageMessage(fTestNet, sType, sPayload);

            string sPrivKey = BMSCommon.WebRPC.GetFDPair(fTestNet);
            string sPubKey = BMSCommon.WebRPC.GetFDPubKey(fTestNet);
            string sUnspentData = BMSCommon.WebRPC.GetAddressUTXOs(fTestNet, sPubKey);
            string sErr = "";
            string sTXID = "";
            NBitcoin.Crypto.BBPTransaction.PrepareFundingTransaction(fTestNet, nAmount, sToAddress, sPrivKey, sPackaged, sUnspentData, out sErr, out sTXID);
            BMSCommon.WebRPC.DACResult r = new BMSCommon.WebRPC.DACResult();
            if (sErr != "")
            {
                r.Error = sErr;
                return r;
            }
            r = BMSCommon.WebRPC.SendRawTx(fTestNet, sTXID);
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
            var s = string.Format("{0:0.00}", Math.Round(myNumber,2));
            return s;
        }

        public static string FormatCurrency(double nMyNumber)
        {
            var s = string.Format("{0:0.0000000000}", nMyNumber);
            return s;
        }

        public static DataTable GetUserByERC20Address(bool fTestNet, string sERC20Address)
        {
            string sTable = fTestNet  ? "tuser" : "user";
            string sql = "Select * from " + sTable + " where ERC20Address=@erc;";
            MySqlCommand m1 = new MySqlCommand(sql);
            m1.Parameters.AddWithValue("@erc", sERC20Address);
            DataTable dt = Database.GetDataTable(m1);
            return dt;
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
        public static string ListToHTMLSelect(List<DropDownItem> s, string sSelected)
        {
            string html = "";
            for (int i = 0; i < s.Count; i++)
            {
                DropDownItem di = s[i];
                string narr = "";
                if (s[i].key.ToLower() == sSelected.ToLower())
                    narr = " SELECTED";
                string row = "<option " + narr + " value='" + s[i].key + "'>" + s[i].text + "</option>";
                html += row + "\r\n";
            }
            return html;
        }

        public static string GetStandardButton(string sID, string sCaption, string sEvent, string sArgs, string sConfirmNarrative)
        {
            string sConfirmNarr = "";
            if (sConfirmNarrative != "")
            {
                sConfirmNarr = "var bConfirm=confirm('" + sConfirmNarrative + "');if (!bConfirm) return false;";

            }
            string sButton = "<button class='btn-default xbtn xbtn-info xbtn-block' id='" + sID + "' onclick=\"" + sConfirmNarr + sArgs + "DoCallback('" + sEvent + "',e);\" >" + sCaption + "</button>";
            return sButton;
        }

        public static bool ContainsReservedWord(string sData)
        {
            string data = sData.ToLower();
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            data = rgx.Replace(data, "");
            if (data.Contains("javascript") || data.Contains("script") || data.Contains("javas"))
            {
                return true;
            }
            return false;
        }
        public static string CleanseXSSAdvanced(string sData, bool fNeutralizeJS = false)
        {
            // Here is an evil one:
            // https://unchained.biblepay.org/PrayerBlog?entity=townhall111%22%20onpointermove=alert%28document.cookie%29%3e
            // Note how the attacker does not use the word javascript.. And he encodes the left and right parentheses; and the semicolon.
            // So we catch the word script in other places, but we dont catch this.  Lets remove the %01-%99

            // Note: Microsofts XSS method, htmlencode, does NOT protect against this!           
            sData = sData.Replace("document.", "");
            sData = sData.Replace(".domain", "");
            sData = sData.Replace(".cookie", "");
            sData = sData.Replace("javascript", "");
            sData = sData.Replace("%22", "");
            sData = sData.Replace("%20", "");
            sData = sData.Replace("%2528", "");
            sData = sData.Replace("%2529", "");
            sData = sData.Replace("alert", "");

            sData = sData.Replace("<", "");
            sData = sData.Replace(">", "");
            sData = sData.Replace("(", "{");
            sData = sData.Replace(")", "}");
            if (fNeutralizeJS)
            {
                sData = sData.Replace("$(", "");
                sData = sData.Replace("${", "");
                sData = sData.Replace("\"", "");
                sData = sData.Replace("'", "");
                sData = sData.Replace("`", "");
                sData = sData.Replace("{", "");
                sData = sData.Replace("}", "");
                sData = sData.Replace("onpointer", "");
            }
            sData = sData.Replace("script:", "");
            sData = sData.Replace("\\", "");
            sData = sData.Replace("%25", "");
            sData = sData.Replace("%28", "");
            sData = sData.Replace("%29", "");
            sData = sData.Replace("%3e", "");
            sData = sData.Replace("onmouse", "");
            sData = sData.Replace("onpointer", "");
            bool fCRW = ContainsReservedWord(sData);
            if (fCRW)
                return "blocked";
            return sData;
        }

        public static string GetNotificationCountHR(HttpContext h)
        {
            //You got 151 notifications
            double nCount = GetSessionDouble(h, "notificationcount");
            string sData = "You've got " + nCount.ToString() + " notification(s).";
            return sData;
        }

        public static int GetNotificationCount(HttpContext h)
        {
            int n1 = (int)GetSessionDouble(h, "notificationcount");
            return n1;
        }
        public static string GetAvatarBalance(HttpContext h, bool fEraseCache)
        {
            return FormatUSD(GetAvatarBalanceNumeric(h, fEraseCache));
        }

        public static double GetSessionDouble(HttpContext h, string sKey)
        {
            string sChain = GetChain(h);
            double n = BMSCommon.Common.GetDouble(h.Session.GetString(sChain + "_" + sKey));
            return n;
        }

        public static void SetSessionDouble(HttpContext h, string sKey, double nValue)
        {
            string sChain = GetChain(h);
            h.Session.SetString(sChain + "_" + sKey, nValue.ToString());
        }

        public static string GetAvatarPicture(bool fTestNet, string sUserID)
        {
            User u1 = GetCachedUser(fTestNet, sUserID);
            string sBioURL = "";
            if (u1 == null)
            {
                sBioURL = "/img/demo/avatars/emptyavatar.png";
            }
            else
            {
                sBioURL = u1.BioURL;
            }
            string sAvatar = "<span class='profile-image-md rounded-circle d-block' style=\"background-image:url('" + sBioURL + "'); "
                + "background-size: cover;\"></span> ";
            return sAvatar;
        }
        public static double GetAvatarBalanceNumeric(HttpContext h, bool fEraseCache)
        {
            BMSCommon.CryptoUtils.User u = GetUser(h);
            long nElapsed = (long)(BMSCommon.Common.UnixTimestamp() - GetSessionDouble(h, "lastbalancecheck"));
            string sChain = GetChain(h);
            BMSCommon.CryptoUtils.SetLastUserActivity(IsTestNet(h), u.ERC20Address);
            
            if (nElapsed < 60*2 && !fEraseCache)
            {
                double nNewBal = BMSCommon.Common.GetDouble(h.Session.GetString(sChain + "_balance"));
                //BMSCommon.Common.Log("(1)AVATAR_BALANCE::" + nElapsed.ToString() + "," + nNewBal.ToString());
                return nNewBal;
            }
            double nBal = QueryAddressBalance(IsTestNet(h), u.BBPAddress);
            if (nBal == 0)
                nBal = -1;
            //BMSCommon.Common.Log("(2)AVATAR_BALANCE::" + nElapsed.ToString() + "," + nBal.ToString());

            SetSessionDouble(h, "lastbalancecheck", BMSCommon.Common.UnixTimestamp());
            h.Session.SetString(sChain + "_balance", nBal.ToString());
            return nBal;
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
            string sKey = IsTestNet(s) ? "tUser" : "User";
            BMSCommon.CryptoUtils.User u = s.Session.GetObject<BMSCommon.CryptoUtils.User>(sKey);
            if (u == null)
                u = new BMSCommon.CryptoUtils.User();


            
            if (u.ERC20Address=="" || u.ERC20Address == null)
            {
                s.Request.Cookies.TryGetValue("erc20address", out u.ERC20Address);
            }

            if (!u.LoggedIn && u.ERC20Address != "")
            {
                u = BMSCommon.CryptoUtils.DepersistUser(IsTestNet(s),u.ERC20Address);
                s.Session.SetObject(sKey, u);
                if (u.ERC20Address == "" || u.ERC20Address == null)
                {
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
            string sKey = IsTestNet(s) ? "tUser" : "User";

            s.Session.SetObject(sKey, u);
        }

        public static string GetLogInAction(HttpContext s)
        {
            string s1 = GetLogInStatus(s);
            string sLogOut = "var c = confirm('Are you sure you want to Log Out?'); if (c) { DoCallback('profile_logout'); }";
            string sLoc = s1 == "LOGGED OUT" ? "location.href='/page/profile';" : sLogOut;
            return sLoc;
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


        public static BMSCommon.Encryption.KeyType GetKeyPair(HttpContext h, string sNonce = "")
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
                    string sDerivationSource = ERC20Signature + sNonce;
                    k = BMSCommon.Encryption.DeriveKey(IsTestNet(h), sDerivationSource);
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

        public static string GetTimelinePostDiv(HttpContext h, string sParentID)
        {
            string data = GetTemplate("timeline.htm");
            // This is the reply to dialog, hence we replace with the active user:
            data = data.Replace("@BioURL",GetBioURL(h));
            data = data.Replace("@parentid", sParentID);
            // Append the posts, one by one from all who posted on this thread.
            List<Timeline> l = Timeline.Get(IsTestNet(h), sParentID);
            for (int i = 0; i < l.Count; i++)
            {
                User uRow =CryptoUtils.GetCachedUser(IsTestNet(h), l[i].ERC20Address);
                if (uRow != null)
                {
                    string entry = GetTemplate("timelinepost.htm");
                    entry = entry.Replace("@BioURL", uRow.BioURL);
                    entry = entry.Replace("@VALUE", l[i].Body);
                    entry = entry.Replace("@divPaste", l[i].dataPaste);
                    data += "\r\n" + entry;
                }
            }
            return data;
        }

        public static string GetModalDialogJson(string title, string body, string optjs="")
        {
            ServerToClient returnVal = new ServerToClient();
            string modal = DSQL.UI.GetModalDialog(title, body);
            returnVal.returnbody = modal;
            returnVal.returntype = "modal";
            string outdata = JsonConvert.SerializeObject(returnVal);
            return outdata;
        }

        public static string MsgBoxJson(HttpContext h, string sTitle, string sHeading, string sBody)
        {
            MsgBox(h, sTitle, sHeading, sBody, false);
            ServerToClient returnVal = new ServerToClient();
            string m = "location.href='/bbp/messagepage';";
            returnVal.returnbody = m;
            returnVal.returntype = "javascript";
            string o1 = JsonConvert.SerializeObject(returnVal);
            return o1;
        }
        public static string GetAccordian(string id, string title, string body)
        {
            string data = GetTemplate("accordian.htm");
            data = data.Replace("@title", title);
            data = data.Replace("@body", body);
            data = data.Replace("@id", id);
            return data;
        }

        public static string GetNotificationItem(string id, string avatarURL, string message, string fullname, DateTime dtTime, string status)
        {
            string data = GetTemplate("notificationrecord.htm");
            data = data.Replace("@id", id);
            data = data.Replace("@avatarURL", avatarURL);
            data = data.Replace("@message", message);
            data = data.Replace("@fullname", fullname);
            data = data.Replace("@status", status);
            string sRelativeTime = TimeUtility.GetRelativeTime(dtTime);
            data = data.Replace("@timerelative", sRelativeTime);
            return data;
        }

        public static bool InsertNotification(bool fTestNet, string sUID, string sFromUser, string sBody, string sType, string sURL)
        {
            string sKey = sType + sUID + sFromUser;

            bool fLatch = BMSCommon.Pricing.Latch(fTestNet, sKey, 60 * 60 * 1);
            if (!fLatch)
                return false;

            string sTable = fTestNet ? "tnotification" : "notification";
            string sql = "Insert into " + sTable + " (id, uid, fromuser, time, added, body, type, URL) values (uuid(), @uid, @fromuser, UNIX_TIMESTAMP(now()), now(), @body, @type, @URL);";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            cmd1.Parameters.AddWithValue("@fromuser", sFromUser);
            cmd1.Parameters.AddWithValue("@uid", sUID);
            cmd1.Parameters.AddWithValue("@body", sBody);
            cmd1.Parameters.AddWithValue("@type", sType);
            cmd1.Parameters.AddWithValue("@URL", sURL);

            bool f = Database.ExecuteNonQuery(cmd1);
            return f;
        }

        public static string LevelToStatus(int i)
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
            return status;
        }

        public static string GetNotifications(HttpContext h, string sUserID)
        {
            string sTable = IsTestNet(h) ? "tnotification" : "notification";
            string sql = "Select * from " + sTable + " where uid=@uid;";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            cmd1.Parameters.AddWithValue("@uid", sUserID);
            DataTable dt = BMSCommon.Database.GetDataTable(cmd1);
            string html = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                User uRow = CryptoUtils.GetCachedUser(IsTestNet(h), dt.Rows[i]["FromUser"].ToString());
                DateTime dtTime = Convert.ToDateTime(dt.Rows[i]["added"]);
                bool fActive = IsUserActive(false, uRow.ERC20Address);

                string sStatus = fActive ? "status-success" : "status-danger";

                string sAnchor = "<a href='" + dt.Rows[i]["URL"].ToString() + "'>";
                string sFullMessage = sAnchor + dt.Rows[i]["body"].ToString() + "</a>";

                string row = GetNotificationItem(i.ToString(), uRow.BioURL,
                    sFullMessage, uRow.NickName, dtTime, sStatus); 
                html += row + "\r\n";
            }
            double nCount = dt.Rows.Count;
            SetSessionDouble(h, "notificationcount", nCount);

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
