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

namespace BiblePay.BMS.Controllers
{
    public partial class BBPController : Controller
    {

        public NFT GetNFT(HttpContext h, string sID)
        {
            string sChain = IsTestNet(h) ? "test" : "main";
            NFT n = BMSCommon.NFT.GetNFT(sChain, sID);
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

        protected string GetProposalsList(HttpContext h)
        {

            string sChain = IsTestNet(h) ? "test" : "main";
            DSQL.Proposal.SubmitProposals(IsTestNet(h));

            string sql = "Select * from proposal WHERE CHAIN='" + sChain + "' and TIMESTAMPDIFF(MINUTE, added, now()) < 86400*30 Order by Added desc";
            MySqlCommand m1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable(m1);
            string html = "<table class=saved><tr><th>UserName<th>Expense Type<th>Proposal Name<th>Amount<th>PrepareTXID<th>URL<th>Chain<th>Updated<th>SubmitTXID<th>Vote Yes<th>Vote No</tr>\r\n";

            for (int y = 0; y < dt.Rows.Count; y++)
            {
                string sURLAnchor = "<a href='" + dt.Rows[y]["URL"].ToString() + "' target=_blank>View Proposal</a>";

                string sID = dt.Rows[y]["SubmitTXID"].ToString();
                string div = "<tr>"
                    + "<td>" + dt.Rows[y]["NickName"].ToString()
                    + "<td>" + dt.Rows[y]["ExpenseType"].ToString()
                    + "<td>" + dt.Rows[y]["Name"].ToString()
                    + "<td>" + dt.Rows[y]["Amount"].ToString();
                div += "<td><small>"
                    + Mid(dt.Rows[y]["PrepareTXID"].ToString(), 0, 10)
                    + "</small>"
                    + "<td>" + sURLAnchor
                    + "<td>" + dt.Rows[y]["Chain"].ToString()
                    + "<td>" + dt.Rows[y]["Updated"].ToString()
                + "<td><small>" + dt.Rows[y]["SubmitTXID"].ToString() + "</small>"
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

        public static string btnSubmitProposal_Click(HttpContext h, string sData)
        {
            string sError = "";
            string txtName = GetFormData(sData, "txtName");
            string txtAddress = GetFormData(sData, "txtAddress");
            string txtAmount = GetFormData(sData, "txtAmount");
            string txtExpenseType = GetFormData(sData, "ddCharity");
            string txtURL = GetFormData(sData, "txtURL");
            if (txtName.Length < 5)
                sError = "Proposal name too short.";
            if (txtAddress.Length < 24)
                sError = "Address must be valid.";
            if (GetDouble(txtAmount) <= 0)
                sError = "Amount must be populated.";
            if (!GetUser(h).LoggedIn)
                sError = "You must be logged in.";

            bool fValid = BMSCommon.WebRPC.ValidateAddress(IsTestNet(h), txtAddress);
            if (!fValid)
            {
                sError = "Address is not valid for this chain.";
            }

            if (GetDouble(txtAmount) > 7000000 || GetDouble(txtAmount) < 1)
            {
                sError = "Amount is too high (over superblock limit) or too low.";
            }

            double nMyBal = GetDouble(GetAvatarBalance(h, false));
            if (nMyBal < 2501)
                sError = "Balance too low.";

            if (sError != "")
            {
                string sJson1 = MsgBoxJson(h, "Error", "Error", sError);
                return sJson1;
            }
            // Submit

            //DataOps.AdjBalance(-1 * 2500, gUser(this).UserId, "Proposal Fee - " + Left(txtURL.Text, 100));
            DSQL.Proposal.gobject_serialize(IsTestNet(h), GetUser(h).ERC20Address, GetUser(h).NickName, txtName, txtAddress, txtAmount, txtURL, txtExpenseType);
            string sJson = MsgBoxJson(h,"Success", "Success", "Thank you.  Your proposal will be submitted in six blocks.");
            return sJson;
        }
    
        public IActionResult Scratchpad()
        {
            return View();
        }
        public async Task<IActionResult> PortfolioBuilder()
        {
            ViewBag.CryptoCurrencyIndex = BMSCommon.Pricing.GetChartOfIndex();
            double nDWU = await BMS.DSQL.PB.GetDWU(DSQL.UI.IsTestNet(HttpContext)) * 100;
            ViewBag.DWU = Math.Round(nDWU,2).ToString() + "%";
            // Sanctuary DWU
            double nRewardPerBlock = 1000;
            double nRewardsPerDay = 10;
            double nRewardsPerMonth = nRewardPerBlock * nRewardsPerDay * 30;
            double nPricePerBBP = .00008244;
            double nRevUSDPerMonth = nRewardsPerMonth * nPricePerBBP;
            double nCostUSDPerMonth = 15;
            double nMonthlyProfit = nRevUSDPerMonth - nCostUSDPerMonth;
            double nAnnualProfit = nMonthlyProfit * 12;
            double nInvestment = 4500001 * nPricePerBBP;
            double nSancDWU =  nAnnualProfit /nInvestment* 100;
            ViewBag.SanctuaryCost = "$" + Math.Round(nInvestment, 2) + " USD";
            ViewBag.SanctuaryDWU = Math.Round(nSancDWU,2).ToString() + "%";
            return View();
        }

        public IActionResult TurnkeySanctuary()
        {

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

        public string GetImgSource()
        {
            try
            {
                string sql = "Select * from SponsoredOrphan2 where Charity not in ('sai') and ChildId not in ('Genevieve Umba')";
                MySqlCommand m1 = new MySqlCommand(sql);

                DataTable dt = BMSCommon.Database.GetDataTable(m1);
                int nHour = (DateTime.Now.Hour + DateTime.Now.DayOfYear) % dt.Rows.Count;
                string url = dt.Rows[nHour]["BioPicture"].ToString();
                return url;
            }
            catch (Exception)
            {
                return "https://i.ibb.co/W691XWC/Screen-Shot-2019-12-12-at-16-01-29.png";
            }
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
            DataTable dt = BMSCommon.Database.GetDataTable(m1);
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
            //and bbpaddress like '" + BMS.PurifySQL(txtAddress.Text, 100) + "%'
            //string sFilter = " and bbpaddress like 'bbp%'";
            string sql = "Select Height, bbpaddress, percentage, reward, subsidy, txid "
                + " FROM " + sTable + " WHERE subsidy > 1 and reward > .01 "
                + " AND height > " + nHeight.ToString() + "-205*7 ORDER BY height desc, bbpaddress;";
            MySqlCommand m1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable(m1);
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
            html += GetTR("Pool Fees XMR", "1% (minexmr.com)");
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
            html += GetTR("Job Count",x.GetJobCount().ToString());
            html += GetTR("Thread Count", x.iXMRThreadCount.ToString());
            html += GetTR("Worker Count", x.GetWorkerCount().ToString());
            sql = "Select sum(shares) suc, sum(fails) fail from " + sTableShare + " where TIMESTAMPDIFF(MINUTE, updated, now()) < 1440;";
            MySqlCommand m3 = new MySqlCommand(sql);
            double ts24 = BMSCommon.Database.GetScalarDouble(m3, "suc");
            double tis24 = BMSCommon.Database.GetScalarDouble(m3,  "fail");
            html += GetTR("Total Shares (24 hours)", ts24.ToString());
            html += GetTR("Total Invalid Shares (24 hours)", tis24.ToString());
            sql = "Select count(distinct height) h from " + sTableShare + " where TIMESTAMPDIFF(MINUTE, updated, now()) < 1440 and subsidy > 0 and reward > .05;";
            MySqlCommand m4 = new MySqlCommand(sql);
            double tbf24 = BMSCommon.Database.GetScalarDouble(m4,  "h");
            html += GetTR("Total Blocks Found (24 hours)", tbf24.ToString());
            html += "</table>";
            return html;
        }

        public IActionResult PoolAbout()
        {
            bool fTestNet = DSQL.UI.IsTestNet(HttpContext);
            ViewBag.PoolBonusNarrative = DSQL.XMRPoolBase.PoolBonusNarrative();
            ViewBag.PoolMetrics = GetPoolAboutMetrics(DSQL.UI.IsTestNet(HttpContext));
            ViewBag.OrphanPicture = GetImgSource();
            DSQL.XMRPoolBase x = fTestNet ? DSQL.PoolBase.tPool : DSQL.PoolBase.mPool;

            ViewBag.ChartOfHashRate = x.GetChartOfHashRate();
            ViewBag.ChartOfWorkers = x.GetChartOfWorkers();
            ViewBag.ChartOfBlocks = x.GetChartOfBlocks();

            return View();
        }

        public IActionResult PoolLeaderboard()
        {
            ViewBag.PoolLeaderboard = GetLeaderboard(DSQL.UI.IsTestNet(HttpContext));
            return View();
        }

        public IActionResult ProposalList()
        {
            ViewBag.ProposalList = GetProposalsList(HttpContext);
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

            ViewBag.PoolDNS = "https://sanc1.biblepay.org:" + nPort.ToString();
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
