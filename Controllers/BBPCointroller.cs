
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static BMSCommon.Common;
using static BiblePay.BMS.DSQL.SessionExtensions;
using Microsoft.AspNetCore.Http;
using static BiblePay.BMS.DSQL.UI;
using BMSCommon;
using static BMSCommon.BitcoinSync;
using Newtonsoft.Json;
using static BMSCommon.Pricing;
using static BMSCommon.CryptoUtils;
using BiblePay.BMS.Extensions;
using BiblePay.BMS.DSQL;
using static BMSCommon.Model;
using static BMSCommon.Models.NFT;
using BMSCommon.Models;

namespace PoolLeaderboardModel.Models
{
    public class PoolLeaderboardLineItem
    {
        public string BBPAddress { get; set; }
        public int BBPShares { get; set; }
        public int XMRShares { get; set; }
        public string Efficiency { get; set; }
        public string HashRate { get; set; }
        public string Updated { get; set; }
        public int Height { get; set; }
    }

    public class LeaderboardResponse
    {
        public IEnumerable<PoolLeaderboardLineItem> items { get; set; }
    }
}


namespace BiblePay.BMS.Controllers
{
    public partial class BBPController : Controller
    {

        [HttpPost]
        public async Task<IActionResult> UploadFileTimeline(List<IFormFile> file)
        {
            try
            {
                if (file.Count > 0)
                {
                    for (int i = 0; i < file.Count; i++)
                    {
                        string _FileName = Path.GetFileName(file[i].FileName);
                        bool fOK = DSQL.UI.IsAllowableExtension(_FileName);
                        if (fOK)
                        {
                            FileInfo fi = new FileInfo(_FileName);
                            string sGuid = Guid.NewGuid().ToString() + "" + fi.Extension;
                            string sDestFN = Path.Combine(Path.GetTempPath(), sGuid);
                            using (var stream = new FileStream(sDestFN, System.IO.FileMode.Create))
                            {
                                await file[i].CopyToAsync(stream);
                            }

                            string sURL = await BBPTestHarness.IPFS.UploadIPFS(sDestFN, "upload/photos/" + sGuid, BMSCommon.Common.GetCDN());
                            ServerToClient returnVal = new ServerToClient();
                            returnVal.returnbody = "";
                            returnVal.returntype = "uploadsuccess";
                            returnVal.returnurl = sURL;
                            string o1 = JsonConvert.SerializeObject(returnVal);
                            return Json(o1);
                        }
                        else
                        {
                            string modal = DSQL.UI.GetModalDialog("Save Timeline Image", "Extension not allowed");
                            ServerToClient returnVal = new ServerToClient();
                            returnVal.returntype = "modal";
                            returnVal.returnbody = modal;
                            string o1 = JsonConvert.SerializeObject(returnVal);
                            return Json(o1);
                        }
                    }
                }
                ViewBag.Message = "Sent " + file[0].FileName + " successfully";
                Response.Redirect("/bbp/timeline");
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }

        public async Task<NFT> GetNFT(HttpContext h, string sID)
        {
            string sChain = IsTestNet(h) ? "test" : "main";
            NFT n = await NFT.GetNFT(sChain, sID);
            return n;
        }

        protected string GetVote(string ID, string sAction)
        {
            if (ID == "")
            {
                string sData = "(N/A)";
                return sData;
            }
            string sNarr = "From the RPC console in biblepaycore, enter the following command:<br><br>gobject vote-many " + ID + " funding " + sAction;
            string sJS = "<a style='cursor:pointer;' onclick=\"var e={}; e.Narr='" + sNarr + "';DoCallback('show_modal',e);\">Vote " + sAction + "</a>";
            return sJS;
        }


        protected async Task<string> GetProposalsList(HttpContext h)
        {

            string sChain = IsTestNet(h) ? "test" : "main";
            await GovernanceProposal.SubmitProposals(IsTestNet(h));
            List<Proposal> dt = await StorjIO.GetDatabaseObjects<Proposal>("proposal");
            dt = dt.Where(s => s.Chain == sChain && DateTime.Now.Subtract(s.Added).TotalSeconds < 86400*30).ToList();
            dt = dt.OrderByDescending(s => Convert.ToDateTime(s.Added)).ToList();
            string html = "<table class=saved><tr><th>UserName<th>Expense Type<th>Proposal Name<th>Amount<th>PrepareTXID<th>URL<th>Chain<th>Updated<th>Submit TXID<th>Vote Yes<th>Vote No</tr>\r\n";
            for (int y = 0; y < dt.Count; y++)
            {
                string sURLAnchor = "<a href='" + dt[y].URL + "' target=_blank>View Proposal</a>";
                string sID = dt[y].SubmitTXID;
                string div = "<tr>"
                    + "<td>" + dt[y].NickName
                    + "<td>" + dt[y].ExpenseType
                    + "<td>" + dt[y].Name
                    + "<td>" + dt[y].Amount.ToString();
                div += "<td><small>"
                    + Mid(dt[y].PrepareTXID.ToString(), 0, 10)
                    + "</small>"
                    + "<td>" + sURLAnchor
                    + "<td>" + dt[y].Chain.ToString()
                    + "<td>" + dt[y].Updated.ObjToMilitaryTime()
                + "<td><font style='font-size:7px;'>" + dt[y].SubmitTXID + "</font>"
                + "<td>" + GetVote(sID, "yes") + "<td>" + GetVote(sID, "no");
                html += div + "\r\n";
            }
            html += "</table>";
            return html;
        }

        public IActionResult ProposalAdd()
        {
            List<DropDownItem> ddCharity = new List<DropDownItem>();
            ddCharity.Add(new DropDownItem("Charity", "Charity"));
            ddCharity.Add(new DropDownItem("PR", "PR"));
            ddCharity.Add(new DropDownItem("P2P", "P2P"));
            ddCharity.Add(new DropDownItem("IT", "IT"));
            ddCharity.Add(new DropDownItem("XSPORK", "XSPORK"));
            ViewBag.ddCharity = ListToHTMLSelect(ddCharity, "Charity");
            return View();
        }

        public static async Task<string> GetChatMessages(HttpContext h)
        {
            // chat messages
            string sMsgs = String.Empty;
            User u0 = await GetUser(h);
            string sMyId = u0.ERC20Address;
            string sChattingWithUID = h.Session.GetString("CHATTING_WITH");
            if (sMyId == null || sChattingWithUID == null)
            {
                return "";
            }
            if (!chat_depersisted)
            {
                DepersistChatItems(IsTestNet(h));
            }
            if (dictChats.ContainsKey(sMyId))
            {
                for (int i = 0; i < dictChats[sMyId].chats.Count; i++)
                {
                    ChatItem cs = dictChats[sMyId].chats[i];
                    if (cs.From == sMyId && sChattingWithUID == cs.To)
                    {
                        string chatsent = GetTemplate("chatsent.htm");
                        chatsent = chatsent.Replace("@body", cs.body);
                        chatsent = chatsent.Replace("@time", cs.time.ToString());
                        sMsgs += chatsent + "\r\n";
                    }
                    else if (cs.To == sMyId && sChattingWithUID == cs.From)
                    {
                        string chatreply = GetTemplate("chatreply.htm");
                        chatreply = chatreply.Replace("@body", cs.body);
                        chatreply = chatreply.Replace("@time", cs.time.ToString());
                        sMsgs += chatreply + "\r\n";
                    }
                }
            }
            return sMsgs;
        }

        public async Task<IActionResult> Chat()
        {
            string data = GetTemplate("chat.htm");
            string ci = String.Empty;
            // Set up the chat header
            string sUID = HttpContext.Session.GetString("CHATTING_WITH");
            User dtUser = await GetCachedUser(IsTestNet(HttpContext), sUID);
            if (dtUser != null)
            {
                data = data.Replace("@FriendsName", dtUser.NickName);
                string sFriendsAvatar = dtUser.BioURL;
                if (sFriendsAvatar == String.Empty)
                    sFriendsAvatar = "/img/demo/avatars/emptyavatar.png";
                data = data.Replace("@FriendsAvatar", sFriendsAvatar);
            }
            else
            {
                data = data.Replace("@FriendsName", "Choose someone to chat with from the right menu");
                data = data.Replace("FriendsAvatar", "/img/demo/avatars/emptyavatar.png");
            }
            // set up the contactlistitems:
            string sTable = IsTestNet(HttpContext) ? "tuser" : "user";
            string sSQL = "Select * from " + sTable;
            MySqlCommand m1 = new MySqlCommand(sSQL);
            DataTable dt = BMSCommon.Database.GetDataTable2(m1);
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sUserID = dt.Rows[i]["erc20address"].ToString();

                string contactitem = GetTemplate("contactlistitem.htm");
                contactitem = contactitem.Replace("@myname", dt.Rows[i]["nickname"].ToString());
                contactitem = contactitem.Replace("@uid", sUserID);
                bool fActive = BMSCommon.CryptoUtils.IsUserActive(false, sUserID);
                string sUserStatus = fActive ? "status-success" : "status-danger";
                string sUserStatusHR = fActive ? "Active" : "Off";

                contactitem = contactitem.Replace("@messengerstatus", sUserStatus); // status-success = active, status-danger=red, status=green, status-warning=yellow
                contactitem = contactitem.Replace("@status", sUserStatusHR);

                string sNickName = dt.Rows[i]["nickname"].ToString();
                contactitem = contactitem.Replace("@datafiltertag", sNickName.ToLower());

                string sAvatarURL = dt.Rows[i]["BioURL"].ToString();
                if (sAvatarURL == "")
                    sAvatarURL = "/img/demo/avatars/emptyavatar.png";

                contactitem = contactitem.Replace("@avatar", sAvatarURL);
                ci += contactitem + "\r\n";
            }
            data = data.Replace("@contactlistitems", ci);
            string sMsgs = await GetChatMessages(HttpContext);
            data = data.Replace("@chatmessages", sMsgs);
            // Chat poll
            string js = "<script>setTimeout(`DoCallback('Chat_Poll');`, 5000);</script>";
            data += js;
            ViewBag.Chat = data;
            return View();
        }

        public static async Task<string> btnSubmitProposal_Click(HttpContext h, string sData)
        {
            Proposal p = new Proposal();
            p.id = Guid.NewGuid().ToString();
            string sError = String.Empty;
            p.Name = GetFormData(sData, "txtName");
            p.BBPAddress = GetFormData(sData, "txtAddress");
            p.Amount = GetDouble(GetFormData(sData, "txtAmount"));
            p.Added = DateTime.Now;
            User u0 = h.GetCurrentUser();
            p.NickName = u0.NickName;
            p.ERC20Address = u0.ERC20Address;

            p.ExpenseType = GetFormData(sData, "ddCharity");
            p.URL = GetFormData(sData, "txtURL");
            p.Chain = IsTestNet(h) ? "test" : "main";

            if (p.Name.Length < 5)
                sError = "Proposal name too short.";
            if (p.NickName.IsNullOrEmpty())
                sError = "Please log in first so that your nickname can be populated on the proposal.";
            if (p.BBPAddress.Length < 24)
                sError = "Address must be valid.";
            if (p.Amount <= 0 || p.Amount > 13000000)
                sError = "Amount must be populated.";
            
            if (!u0.LoggedIn)
                sError = "You must be logged in.";

            bool fValid = BMSCommon.WebRPC.ValidateAddress(IsTestNet(h), p.BBPAddress);
            if (!fValid)
            {
                sError = "Address is not valid for this chain.";
            }

            if (GetDouble(p.Amount) > 17000000 || GetDouble(p.Amount) < 1)
            {
                sError = "Amount is too high (over superblock limit) or too low.";
            }

            double nMyBal = GetDouble(GetAvatarBalance(h, false));
            if (nMyBal < 2501)
                sError = "Balance too low.";

            if (sError != String.Empty)
            {
                string sJson1 = MsgBoxJson(h, "Error", "Error", sError);
                return sJson1;
            }
            // Submit
            await DSQL.GovernanceProposal.gobject_serialize(IsTestNet(h), p);
            string sJson = MsgBoxJson(h,"Success", "Success", "Thank you.  Your proposal will be submitted in six blocks.");
            return sJson;
        }
    
        
        public IActionResult Admin()
        {
            if (HttpContext.GetCurrentUser().ERC20Address != "0xafe8c2709541e72f245e0da0035f52de5bdf3ee5")
            {
                Response.Redirect("/gospel/about");
            }
            return View();
        }

        public async Task<IActionResult> AdminTurnkey()
        {
            if (HttpContext.GetCurrentUser().ERC20Address != "0xafe8c2709541e72f245e0da0035f52de5bdf3ee5")
                Response.Redirect("/gospel/about");
            if (System.Diagnostics.Debugger.IsAttached)
            {
                ViewBag.TurnkeySancReport = await Report.TurnkeyReport(IsTestNet(HttpContext), HttpContext.GetCurrentUser().ERC20Address);
            }
            return View();
        }

        public IActionResult Scratchpad()
        {
            return View();
        }
        public async Task<IActionResult> PortfolioBuilder()
        {
            ViewBag.CryptoCurrencyIndex = BMSCommon.BBPCharting.GetChartOfIndex();
            SanctuaryProfitability sp = await BMSCommon.BitcoinSync.GetMasternodeROI(IsTestNet(HttpContext));
            ViewBag.SanctuaryCost = "$" + Math.Round(sp.nInvestmentAmount, 2) + " USD";
            BBPTestHarness.BlockChairTestHarness.DwuPack d1 = await BBPTestHarness.BlockChairTestHarness.GetDWU(IsTestNet(HttpContext));
            ViewBag.DWUNarrative = "** On a portfolio with 100% coverage and foreign positions";
            ViewBag.BonusPercent = d1.nBonusPercent;
            ViewBag.DWU = Math.Round(d1.nDWU * 100, 2).ToString() + "%";
            ViewBag.SanctuaryGrossROI = Math.Round(sp.nSanctuaryGrossROI, 2).ToString() + "%";
            ViewBag.BonusROI = Math.Round(d1.nBonusPercent * 100, 2) + " %";
            ViewBag.TurnkeySanctuaryNetROI = Math.Round(sp.nTurnkeySanctuaryNetROI, 2).ToString() + "%";
            ViewBag.SanctuaryCount = sp.nMasternodeCount.ToString();
            MySancMetrics msm = await GetTurnkeySanctuariesCount();
            ViewBag.TurnkeySanctuaryCount = msm.nTotalCount;
            ViewBag.TurnkeySanctuaryTotal = msm.nTotalBalance;
            // native coins
            List <DropDownItem> ddSymbol = new List<DropDownItem>();
            ddSymbol.Add(new DropDownItem("BTC", "Bitcoin"));
            ddSymbol.Add(new DropDownItem("DOGE", "DOGE"));
            ddSymbol.Add(new DropDownItem("DASH", "DASH"));
            ViewBag.ddSymbol = ListToHTMLSelect(ddSymbol,"");
            return View();
        }

        public static async Task<string> GetTurnkeySanctuaryReport(HttpContext h)
        {
            if (IsTestNet(h))
            {
                return String.Empty;
            }
            List<TurnkeySanc> dt = await StorjIO.GetDatabaseObjects<TurnkeySanc>("turnkeysanctuaries");

            dt = dt.Where(s => s.erc20address == h.GetCurrentUser().ERC20Address).ToList();
            dt = dt.OrderBy(s => Convert.ToDateTime(s.Added)).ToList();
            string data = "<table class='saved'><tr><th>Added<th width=50%>Address<th>Balance<th>Status<th>Action</tr>";

            string ERC20Signature;
            string ERC20Address;
            h.Request.Cookies.TryGetValue("erc20signature", out ERC20Signature);
            h.Request.Cookies.TryGetValue("erc20address", out ERC20Address);
            string sPrivKey = BMSCommon.Common.GetConfigurationKeyValue("foundationprivkey");

            for (int i = 0; i < dt.Count; i++)
            {
                string sBBPAddress = dt[i].BBPAddress;
                double nBalance = DSQL.UI.QueryAddressBalance(IsTestNet(h), sBBPAddress);
                string sCluster = String.Empty;
                string sNonce = Encryption.DecryptAES256(dt[i].Nonce, sPrivKey);

                string sStatus = "";
                if (nBalance >= 4500001)
                {
                    sStatus = "Active";
                    sCluster = GetStandardButton("btnliq", "Liquidate", "turnkey_liquidate", "var e={};e.address='" + sBBPAddress + "';",
                        "Are you sure you would like to liquidate this sanctuary?");
                }
                else
                {
                    sStatus = "Waiting for Funding";
                    sCluster = GetStandardButton("btnfund", "Fund", "turnkey_fund", "var e={};e.address='" + sBBPAddress + "';", "");
                }

                string row = "<td>" + dt[i].Added.ToString() + "<td><input readonly class='form-control' value='" 
                    + dt[i].BBPAddress + "'/></td><td>" 
                    + nBalance.ToString() + " BBP</td><td>" + sStatus + "<td>" + sCluster + "</td></tr>";
                data += row + "\r\n";
            }
            data += "</table>";
            if (dt.Count == 0)
            {
                data = "No sanctuaries found.";
            }
            else
            {
                data += GetStandardButton("btnbackup", "Back Up Keys", "turnkey_backup", "var e={};", "");
            }
            return data;
        }


        public async Task<IActionResult> TurnkeySanctuaries()
        {
            ViewBag.TurnkeySAnctuaryReport = await GetTurnkeySanctuaryReport(HttpContext);
            return View();
        }

        public IActionResult Timeline()
        {
            ViewBag.Timeline = DSQL.UI.GetTimelinePostDiv(HttpContext, "main");
            return View();
        }

        public IActionResult MessagePage()
        {
            ViewBag.Title = HttpContext.Session.GetString("msgbox_title");
            ViewBag.Heading = HttpContext.Session.GetString("msgbox_heading");
            ViewBag.Body = HttpContext.Session.GetString("msgbox_body");
            return View();
        }

        public async Task<IActionResult> PortfolioBuilderLeaderboard()
        {
            ViewBag.PortfolioBuilderLeaderboard = await DSQL.PB.GetLeaderboard(HttpContext, DSQL.UI.IsTestNet(HttpContext));
            ViewBag.PortfolioBuilderMode = DSQL.UI.GetPBMode(HttpContext);
            return View();
        }


        public static string GetHPSLabel(double dHR)
        {
            string KH = Math.Round(dHR / 1000, 2).ToString() + " KH/S";
            string MH = Math.Round(dHR / 1000000, 2).ToString() + " MH/S";
            string H = Math.Round(dHR, 2).ToString() + " H/S";
            if (dHR < 10000)
            {
                return H;
            }
            else if (dHR >= 10000 && dHR <= 1000000)
            {
                return KH;
            }
            else if (dHR > 1000000)
            {
                return MH;
            }
            else
            {
                return H;
            }
        }
        public string GetTR(string key, string value)
        {
            string tr = "<TR><TD width='25%'>" + key + ":</TD><TD>" + value + "</TD></TR>\r\n";
            return tr;
        }

    protected string GetLeaderboard(bool fTestNet)
    {
            string sTable = fTestNet ? "tLeaderboard" : "Leaderboard";
            string sql = "Select * from " + sTable + " order by bbpaddress;";
            MySqlCommand m1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable2(m1);
            string html = "<table class=saved><tr><th width=20%>BBP Address</th><th>BBP Shares<th>BBP Invalid<th>XMR Shares<th>XMR Charity Shares<th>Efficiency<th>Hash Rate<th>Updated<th>Height</tr>";
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                string bbpaddress = dt.Rows[y]["bbpaddress"].ToString() ?? "";
                string div = "<tr><td>" + bbpaddress
                    + "<td>" + dt.Rows[y]["shares"].ToString()
                    + "<td>" + dt.Rows[y]["fails"].ToString()
                    + "<td>" + dt.Rows[y]["bxmr"].ToString()
                    + "<td>" + dt.Rows[y]["bxmrc"].ToString()
                    + "<td>" + dt.Rows[y]["efficiency"].ToString() + "%"
                    + "<td>" + dt.Rows[y]["hashrate"].ToString() + " HPS"
                    + "<td>" + dt.Rows[y]["Updated"].ToString()
                    + "<td>" + dt.Rows[y]["Height"].ToString() + "</tr>";
                html += div + "\r\n";
            }
            html += "</table>";
            return html;
        }

        protected string GetBlockHistoryReport(bool fTestNet)
        {
            DSQL.XMRPoolBase x = fTestNet ? DSQL.PoolBase.tPool : DSQL.PoolBase.mPool;
            int nHeight = fTestNet ? x._template.height : x._template.height;
            string sTable = fTestNet ? "tshare" : "share";
            string sql = "Select Height, bbpaddress, percentage, reward, subsidy, txid "
                + " FROM " + sTable + " WHERE subsidy > 1 and reward > .01 "
                + " AND height > " + nHeight.ToString() + "-205*7 ORDER BY height desc, bbpaddress;";
            MySqlCommand m1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable2(m1);
            string html = "<table class=saved><tr><th width=20%>Height</th><th>BBP Address<th>Percentage<th>Reward<th>Block Subsidy<th>TXID</tr>";
            double _height = 0;
            double oldheight = 0;
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                _height = GetDouble(dt.Rows[y]["height"]);
                if (oldheight > 0 && _height != oldheight)
                {
                    html += "<tr style='xbackground-color:white;'><td style='xbackground-color:white;' colspan=6><hr></td></tr>";
                }
                string div = "<tr><td>" + dt.Rows[y]["height"].ToString()
                    + "<td>" + dt.Rows[y]["bbpaddress"].ToString()
                    + "<td>" + Math.Round(GetDouble(dt.Rows[y]["percentage"].ToString()) * 100, 2) + "%"
                    + "<td>" + dt.Rows[y]["reward"].ToString()
                    + "<td>" + dt.Rows[y]["subsidy"].ToString()
                    + "<td><small><nobr>" + dt.Rows[y]["TXID"].ToString() + "</nobr></small></tr>";
                html += div + "\r\n";

                oldheight = _height;

            }
            if (dt.Rows.Count < 1)
            {
                html += "<tr><td>No data found.</td></tr>";
            }    
            html += "</table>";
            return html;
        }

        protected string GetPoolAboutMetrics(bool fTestNet)
        {
            string html = "<table>";

            DSQL.XMRPoolBase x = fTestNet ? DSQL.PoolBase.tPool : DSQL.PoolBase.mPool;

            string sLBTable = fTestNet ? "tLeaderboard" : "Leaderboard";
            string sTableShare = fTestNet ? "tshare" : "share";

            string sql = "Select sum(hashrate) hr FROM " + sLBTable;
            MySqlCommand m1 = new MySqlCommand(sql);
            double dHR = BMSCommon.Database.GetScalarDouble(m1, "hr");

            sql = "Select count(bbpaddress) ct from " + sLBTable;
            MySqlCommand m2 = new MySqlCommand(sql);

            double dCt = BMSCommon.Database.GetScalarDouble(m2, "ct");
            html += GetTR("Chain", fTestNet ? "TESTNET" : "MAINNET");

            string poolAccount = GetConfigurationKeyValue("PoolPayAccount");
            if (poolAccount == "")
            {
                html += GetTR("Disabled", "This pool is disabled because it is not configured.  See PoolPayAccount configuration Key.");
                return html;
            }

            html += GetTR("Miners", dCt.ToString());
            html += GetTR("Speed", GetHPSLabel(dHR));
            string sEmail = GetConfigurationKeyValue("OperatorEmailAddress");
            if (sEmail == "")
                sEmail = "Pool Owner, please set your email address [operatoremailaddress=your_email_address] in the config file";
            html += GetTR("Contact E-Mail", sEmail);
            html += GetTR("Pool Fees XMR", "1%");
            html += GetTR("Pool Fees BBP", Math.Round(GetDouble(GetConfigurationKeyValue("PoolFee")) * 100, 2) + "%");
            html += GetTR("Block Bonus", Math.Round(GetDouble(GetConfigurationKeyValue("PoolBlockBonus")), 0) + " BBP Per Block");
                
            html += GetTR("Build Version", DSQL.PoolBase.pool_version.ToString());
            html += GetTR("Startup Time", DSQL.XMRPoolBase.start_date.ToString());

            if (x is null || x._template.target == null)
            {
                html += GetTR("Error", "pool template is null");
            }
            else
            {
                UInt64 iTarget = UInt64.Parse(x._template.target.Substring(0, 12), System.Globalization.NumberStyles.HexNumber);
                double dDiff = 655350.0 / iTarget;
                string sHeight = x._template.height.ToString() + " (Diff: " + Math.Round(dDiff, 2) + ")";
                html += GetTR("Difficulty", dDiff.ToString());
                html += GetTR("Height", x._template.height.ToString());
            }
            html += GetTR("Job Count", x.GetJobCount().ToString());
            //html += GetTR("Thread Count", x.iXMRThreadCount.ToString());
            html += GetTR("Worker Count", x.GetWorkerCount().ToString());
            sql = "Select sum(shares) suc, sum(fails) fail from " + sTableShare + " where TIMESTAMPDIFF(MINUTE, updated, now()) < 1440;";
            MySqlCommand m3 = new MySqlCommand(sql);
            double ts24 = BMSCommon.Database.GetScalarDouble(m3, "suc");
            double tis24 = BMSCommon.Database.GetScalarDouble(m3, "fail");
            html += GetTR("Total Shares (24 hours)", ts24.ToString());
            //html += GetTR("Total Invalid Shares (24 hours)", tis24.ToString());
            sql = "Select count(distinct height) h from " + sTableShare + " where TIMESTAMPDIFF(MINUTE, updated, now()) < 1440 and subsidy > 0 and reward > .05;";
            MySqlCommand m4 = new MySqlCommand(sql);
            double tbf24 = BMSCommon.Database.GetScalarDouble(m4, "h");
            html += GetTR("Total Blocks Found (24 hours)", tbf24.ToString());
            html += "</table>";
            return html;
        }

        public IActionResult Watch()
        {
            ViewBag.WatchVideo = DSQL.youtube.GetVideo(HttpContext);
            return View();
        }

        public IActionResult PoolAbout()
        {
            bool fTestNet = DSQL.UI.IsTestNet(HttpContext);
            ViewBag.PoolBonusNarrative = DSQL.XMRPoolBase.PoolBonusNarrative();
            ViewBag.PoolMetrics = GetPoolAboutMetrics(DSQL.UI.IsTestNet(HttpContext));
            ViewBag.OrphanPicture = String.Empty;// GetImgSource();
            DSQL.XMRPoolBase x = fTestNet ? DSQL.PoolBase.tPool : DSQL.PoolBase.mPool;
            ViewBag.ChartOfHashRate = x.GetChartOfHashRate();
            ViewBag.ChartOfWorkers = x.GetChartOfWorkers();
            ViewBag.ChartOfBlocks = x.GetChartOfBlocks();
            return View();
        }

        public IActionResult PoolLeaderboard()
        {
            string sTable = IsTestNet(HttpContext) ? "tLeaderboard" : "Leaderboard";
            string sql = "Select * from " + sTable + " order by shares desc;";
            MySqlCommand m1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable2(m1);
            List<PoolLeaderboardModel.Models.PoolLeaderboardLineItem> l = new List<PoolLeaderboardModel.Models.PoolLeaderboardLineItem>();
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                string bbpaddress = dt.Rows[y]["bbpaddress"].ToString() ?? "";
                PoolLeaderboardModel.Models.PoolLeaderboardLineItem li = new PoolLeaderboardModel.Models.PoolLeaderboardLineItem();
                li.BBPAddress = dt.Rows[y]["bbpaddress"].ToString() ?? "";
                li.BBPShares = dt.Rows[y]["shares"].AsInt32();
                li.XMRShares = dt.Rows[y]["bxmr"].AsInt32();
                li.Efficiency = dt.Rows[y]["efficiency"].ToString() + "%";
                li.HashRate = dt.Rows[y]["hashrate"].ToString() + " HPS";
                li.Updated = dt.Rows[y]["Updated"].ToString();
                li.Height = dt.Rows[y]["Height"].AsInt32();
                l.Add(li);
            }
            PoolLeaderboardModel.Models.LeaderboardResponse lr = new PoolLeaderboardModel.Models.LeaderboardResponse();
            lr.items = l;
            return View(lr);
        }


        public string GetPriceReport(string sDogeAddress)
        {
            price1 nBTCPrice = BMSCommon.Pricing.GetCryptoPrice("BTC");
            price1 nBBPPrice = BMSCommon.Pricing.GetCryptoPrice("BBP");
            price1 nDOGEPrice = BMSCommon.Pricing.GetCryptoPrice("DOGE");
            double nUSDBBP = nBTCPrice.AmountUSD * nBBPPrice.Amount;
            double nUSDDOGE = nBTCPrice.AmountUSD * nDOGEPrice.Amount;
            string html = "<table class='saved'><tr><th>Symbol<th>USD Amount</tr>";

            if (nUSDBBP < .000015)
            {
                //bbp price below 15 milli sat-usd
                return "BBP_PRICE_TOO_LOW";
            }

            if (nUSDDOGE < .01)
            {
                return "DOGE_PRICE_TOO_LOW";
            }
            double nBBPPerDoge = Math.Round(nUSDDOGE / nUSDBBP, 4);

            string sRow = "<td>BTC/USD<td>" + FormatCurrency(nBTCPrice.AmountUSD) + "</tr>";
            html += sRow;
            sRow = "<td>DOGE/USD<td>" + FormatCurrency(nUSDDOGE) + "</tr>";
            html += sRow;
            sRow = "<td>BBP/USD<td>" + FormatCurrency(nUSDBBP) + "</tr>";
            html += sRow;
            html += "<td>BBP/DOGE<td>" + FormatCurrency(nBBPPerDoge) + "</tr>";
            double nExample = 1000 * nBBPPerDoge;
            string sNarr = "You will receive " + nBBPPerDoge.ToString() + " BBP per DOGE.  Example: Send 1000 DOGE to " + sDogeAddress + " and you will receive " + nExample.ToString() + " BBP in your web wallet. ";

            html += "</table>";

            html += "<br>" + sNarr;
            return html;

        }
        public async Task<IActionResult> AtomicSwap()
        {
            ViewBag.DogeAddress = DSQL.SouthXChange.GetSXAddressByERC20Address("doge", HttpContext.GetCurrentUser().ERC20Address);
            ViewBag.PriceReport = GetPriceReport(ViewBag.DogeAddress);
            ViewBag.Atomic  = await DSQL.SouthXChange.GetSouthXChangeReport(HttpContext);
            return View();
        }

        public async Task<IActionResult> ProposalList()
        {
            ViewBag.ProposalList = await GetProposalsList(HttpContext);
            return View();
        }
        public IActionResult PoolGettingStarted()
        {
            int nPortMainnet = (int)GetDouble(GetConfigurationKeyValue("XMRPort"));
            int nPortTestNet = (int)GetDouble(GetConfigurationKeyValue("XMRPortTestNet"));
            if (nPortMainnet == 0)
                nPortMainnet = 3001;
            if (nPortTestNet == 0)
                nPortTestNet = 3002;

            int nPort = DSQL.UI.IsTestNet(HttpContext) ? nPortTestNet : nPortMainnet;

            ViewBag.PoolDNS = "unchained.biblepay.org:" + nPort.ToString();
            return View();
        }

        public async Task<IActionResult> Videos()
        {
            ViewBag.VideoList = await DSQL.youtube.GetSomeVideos(HttpContext);
            return View();
        }
        public IActionResult BlockHistory()
        {
            ViewBag.BlockHistoryReport = GetBlockHistoryReport(DSQL.UI.IsTestNet(HttpContext));
            return View();
        }
        public IActionResult Univ()
        {
            return View();
        }

    }
}
