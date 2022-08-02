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
using static BiblePay.BMS.DSQL.UI;

namespace BiblePay.BMS.Controllers
{
    public class GospelController : Controller
    {
        protected string GetIllustrationContent(string sURL)
        {
            if (sURL == "xmrinquiry")
            {
                sURL = "https://supportxmr.com";
            }
            // Reserved:  This same routine is supposed to handle the XMR pool payment owed page.
            sURL = sURL.Replace("javascript", "");
            sURL = sURL.Replace("script:", "");
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            string sCleansed = rgx.Replace(sURL.ToLower(), "");
            bool fOK = sURL.StartsWith("https://supportxmr.com") || sURL.StartsWith("https://www.freebibleimages.org/") || sURL.StartsWith("https://wiki.biblepay.org/");
            if (sURL.ToLower().Contains("script") || sURL.ToLower().Contains("javascript") || sURL.ToLower().Contains("(") || sURL.Contains(")"))
                fOK = false;
            if (sURL.ToLower().Contains("javas	cript"))
                fOK = false;
            if (sCleansed.Contains("java") || sCleansed.Contains("confirm"))
                fOK = false;
            if (fOK)
            {
                string sDec = System.Web.HttpUtility.UrlDecode(sURL);
                sDec = System.Web.HttpUtility.HtmlEncode(sDec);
                string sIframe = "<iframe width='95%' style='height: 90vh;' src='" + sDec + "'></iframe>";
                return sIframe;
            }
            else
            {
                return "404";
            }
        }
        public IActionResult GospelVideos()
        {
            return View();
        }
        protected string GetGospelContentFromFile(string sType)
        {
            string sPath = System.IO.Path.Combine(BMSCommon.Database.msContentRootPath, "wwwroot/media/JesusChrist/" + sType + ".htm");

            if (sType.ToLower().Contains("anr"))
            {
                return "404";
            }
            if (!System.IO.File.Exists(sPath))
            {
                return "404";
            }

            string sData = System.IO.File.ReadAllText(sPath, Encoding.Default);
            sData = sData.Replace("“", "\"");
            char z = sData[167];
            int n1 = (int)z;
            char apostrophe = (char)"'"[0];
            sData = sData.Replace((char)0xfffd, apostrophe);
            return sData;
        }

        protected string GetStudies(bool fTestNet)
        {
            string sTable = fTestNet ? "tArticles" : "Articles";
            string sql = "Select * from " + sTable + " order by Name,Description;";
            MySqlCommand cmd = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable(cmd);
            string html = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sArticle = dt.Rows[i]["Name"].ToString();
                bool fHidden = false;
                if (sArticle == "ThreeDays" || sArticle == "NonChristianReligions")
                    fHidden = true;
                if (!fHidden)
                {
                    string row = "<a href=gospel/viewer?type=" + sArticle + ">" + dt.Rows[i]["Description"].ToString() + "</a><br>";
                    html += row;
                }
            }
            return html;
        }
        protected string GetOrphanCollage(bool fTestNet)
        {
            string sTable = fTestNet ? "tSponsoredOrphan2" : "SponsoredOrphan2";
            string sql = "Select * from " + sTable + " where active=1 And Charity not in ('sai') and ChildId not in ('Genevieve Umba') order by Charity,Name;";
            MySqlCommand m = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable(m);
            string sHTML = "<table><tr>";
            int iTD = 0;
            string sErr = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // Each Orphan should be a div with their picture in it
                string sMyBIO = dt.Rows[i]["BioURL"].ToString();
                string sName = dt.Rows[i]["ChildID"].ToString() + " - " + dt.Rows[i]["Charity"].ToString();
                string sBioImg = dt.Rows[i]["BioPicture"].ToString();
                if (sBioImg != "")
                {
                    string sMyOrphan = "<td style='padding:7px;border:1px solid lightgrey' cellpadding=7 cellspacing=7><a href='" + sMyBIO + "'>" + sName
                        + "<br><img style='width:300px;height:250px' src='" + sBioImg + "'></a><br></td>";
                    sHTML += sMyOrphan;
                    iTD++;
                    if (iTD > 2)
                    {
                        iTD = 0;
                        sHTML += "<td width=30%>&nbsp;</td></tr><tr>";
                    }
                }
                else
                {
                    sErr += "<a href='" + sMyBIO + "'>Missing</a>";
                }
            }
            sHTML += "</TR></TABLE>";
            return sHTML;
        }


        protected string GetArticles(bool fTestNet,string type)
        {
            string prefix = "";
            string sPrefix = fTestNet ? "t" : "";
            string table = type == "wiki" ? sPrefix+"Wiki" : sPrefix+"Illustrations";
            string sql = "Select * from " + table + " order by Name,Description";
            MySqlCommand cmd = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable(cmd);
            string html = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sArticle = dt.Rows[i]["Name"].ToString();
                string sDesc = dt.Rows[i]["Description"].ToString();
                string sNarr = sArticle + " - " + sDesc;
                string sURL = "";
                if (type == "wiki")
                {
                    sURL = sArticle;
                    sNarr = sDesc;
                    prefix = "<h3>Wiki Theological Articles</h3><br>&nbsp;<p><p><p><p>";
                }
                else
                {
                    sURL = System.Web.HttpUtility.UrlEncode(dt.Rows[i]["Url"].ToString());
                    prefix = "<h3>All credit for these Illustrations goes to <a href = 'https://www.freebibleimages.org/illustrations/'> Free Bible Images </ a> !</h3>"
                        + "<small><font>NOTE:  Choose then click \"VIEW SLIDESHOW\" for the presentation.<br/></small>"
                        + "</font></h3><br>&nbsp;<p><p>";
                }
                string URL2 = "<a href='gospel/viewer?target=" + sURL + "'>" + sNarr + "</a>";
                bool fDeleted = false;
                if (sURL == "https://wiki.biblepay.org/End_Times_Scriptural_Basis" || sDesc=="Grace and Mercy")
                    fDeleted = true;

                if (!fDeleted)
                {
                    string row = URL2 + "<br>";
                    html += row;
                }
            }
            string output = prefix + html;
            return output;
        }

        protected string GetAccountabilityPDFList()
        {

            string html = "<table>";
            for (int year = 2017; year <= DateTime.Now.Year; year++)
            {
                string row = "<tr><td><a href=/gospel/accountability?year=" + year.ToString() + ">Accounting Year " + year.ToString() + "</a></td></tr>\r\n";
                html += row;
            }
            html += "<tr><td><a href=/gospel/accountability?year=total>Grand Total (All Time)</a></td></tr>\r\n";
            html += "</table>";
            html += "<br><table><tr><td><a href=/gospel/accountability?type=cameroon-one>Cameroon-One Report</a></td></tr>\r\n";
            html += "<tr><td><a href=gospel/accountability?type=kairos>Kairos Report</a></td></tr>\r\n";
            html += "</table>";
            return html;
        }


        protected async Task<IActionResult> ConvertHtmlToPDF(string HTML, string PDFFileName)
        {
            byte[] b = await BBPTestHarness.IPFS.ConvertHtmlToBytes(HTML, PDFFileName);
            string sGuid = Guid.NewGuid().ToString() + ".html";
            return File(b, "application/html", sGuid);
        }
        protected async Task<IActionResult> GenerateAccountingReport(int nYear)
        {
            string sql = "SELECT * FROM (select _id,STR_TO_DATE(added,'%m/%d/%Y') as a1, DATE_FORMAT(STR_TO_DATE(added,'%m/%d/%Y'), '%b-%y') as Added,'DR' as Type,Amount,Charity, '' as Notes from Expense "
                + " union all  select _id, STR_TO_DATE(added,'%m/%d/%Y') as a1, DATE_FORMAT(STR_TO_DATE(added,'%m/%d/%Y'), '%b-%y'), 'CR' as Type,Amount, Charity, Notes from Revenue ) b where year(b.a1)='"
                + nYear.ToString() + "' order by a1 ";
            MySqlCommand command3 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable(command3);
            string html = Report.GetTableHTML("Accounting Report", dt, "Added;Type;Amount;Notes", "Amount", true);
            string accName = "BiblePay Accounting Year " + nYear.ToString() + ".pdf";
            return await ConvertHtmlToPDF(html,accName);
        }

        public async Task<IActionResult> GenerateTotalReport()
        {
            string sql = "SELECT uuid(),round(sum(amount),2) Amount,'' Notes,added,Type, 'Various' Charity, max(Dt1) FROM ( "
                + "select _id, added as a1,  DATE_FORMAT(STR_TO_DATE(added,'%m/%d/%Y'), '%b-%y') as Added, 'DR' as Type, Amount, Charity, '' as Notes, STR_TO_DATE(Added,'%m/%d/%Y') as dt1 from Expense"
                + "    union all"
                + " select _id, added as a1,  DATE_FORMAT(STR_TO_DATE(added,'%m/%d/%Y'), '%b-%y') as Added, 'CR' as Type, Amount, Charity, Notes, STR_TO_DATE(Added,'%m/%d/%Y') as dt1 from Revenue"
                + "  ) b group by added, Type  order by max(dt1)";
            MySqlCommand command3 = new MySqlCommand(sql);
            BMSCommon.Common.Log("1");
            DataTable dt = BMSCommon.Database.GetDataTable(command3);
            string html = Report.GetTableHTML("All Time", dt, "Amount;Added;Type;Charity", "Amount", true);
            string sName = "Totals.pdf";
            return await ConvertHtmlToPDF(html,sName);
        }

        public async Task<IActionResult> GenerateCharityReport(string sCharity)
        {
            string sOE = DSQL.UI.IsTestNet(HttpContext) ? "tOrphanExpense3" : "OrphanExpense3";
            string sSO2 = DSQL.UI.IsTestNet(HttpContext) ? "tSponsoredOrphan2" : "SponsoredOrphan2";

            string sql = "SELECT OrphanExpense._id,OrphanExpense.Amount,OrphanExpense.Notes,STR_TO_DATE(OrphanExpense.Added,'%m/%d/%Y') as ADDED,"
                +"OrphanExpense.Charity,OrphanExpense.ChildID,OrphanExpense.Balance,SO2.Name "
                + " from " + sOE + " as OrphanExpense "
                +" INNER JOIN " + sSO2 + " as SO2 on SO2.childid=OrphanExpense.ChildID where SO2.charity = @charity order by ADDED";
            MySqlCommand command3 = new MySqlCommand(sql);
            command3.Parameters.AddWithValue("@charity", sCharity);
            string html = Report.GetCharityTableHTML(command3, 0);
            string accName = "Charity Report - " + sCharity + ".pdf";
            return await ConvertHtmlToPDF(html,accName);
        }
        public IActionResult CoreGospel()
        {
            return View();
        }

        public IActionResult Wells()
        {
            return View();
        }

        public IActionResult Illustrations()
        {
            ViewBag.Illustrations = GetArticles(IsTestNet(HttpContext),"Illustrations");
            return View();
        }
        
        public IActionResult Collage()
        {
            ViewBag.Collage = GetOrphanCollage(IsTestNet(HttpContext));
            return View();
        }

        public async Task<IActionResult> Accountability()
        {

            string typ = HttpContext.Request.Query["type"].ToString();
            string y = HttpContext.Request.Query["year"].ToString();
            if (typ != "")
            {
                return await GenerateCharityReport(typ);
            }
            else if (y != "")
            {
                if (y == "total")
                {
                    return await GenerateTotalReport();

                }
                else
                {
                    return await GenerateAccountingReport ((int)BMSCommon.Common.GetDouble(y));
                }
            }
            else
            {
                ViewBag.AccountabilityReports = GetAccountabilityPDFList();
                return View();
            }
        }

        public IActionResult WikiTheology()
        {
            ViewBag.Wiki = GetArticles(IsTestNet(HttpContext),"wiki");
            return View();
        }

        public IActionResult TheologicalStudy()
        {
            ViewBag.TheologicalStudy = GetStudies(IsTestNet(HttpContext));
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Viewer()
        {
            string typ = HttpContext.Request.Query["type"].ToString();
            string t = HttpContext.Request.Query["target"].ToString();
            if (typ == "xmrinquiry")
            {
                ViewBag.GospelData = GetIllustrationContent(typ);
            }
            else if (typ != "")
            {
                ViewBag.GospelData = GetGospelContentFromFile(typ);
            }
            else if (t != "")
            {
                ViewBag.GospelData = GetIllustrationContent(t);
            }
            return View();
        }

    }
}
