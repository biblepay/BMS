using BBPAPI;
using BiblePay.BMS.Extensions;
using BMSCommon;
using BMSShared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UI;
using static BiblePay.BMS.DSQL.UIWallet;
using BMSCommon.Model;
using static BMSCommon.Common;

namespace BiblePay.BMS.Controllers
{
    public partial class BBPController : Controller
    {

        
        public IActionResult Admin()
        {
            if (HttpContext.GetCurrentUser().ERC20Address != "0xafe8c2709541e72f245e0da0035f52de5bdf3ee5")
            {
                Response.Redirect("/gospel/about");
            }
            return View();
        }

        public IActionResult AdminTurnkey()
        {
            if (HttpContext.GetCurrentUser().ERC20Address != "0xafe8c2709541e72f245e0da0035f52de5bdf3ee5")
                Response.Redirect("/gospel/about");
            if (System.Diagnostics.Debugger.IsAttached)
            {
                ViewBag.TurnkeySancReport = Report.TurnkeyReport(IsTestNet(HttpContext), HttpContext.GetCurrentUser().ERC20Address);
            }
            return View();
        }

        public IActionResult Scratchpad()
        {
            return View();
        }

        public static string GetTurnkeySanctuaryReport(HttpContext h)
        {
            if (IsTestNet(h))
            {
                return String.Empty;
            }

            if (!h.GetCurrentUser().LoggedIn)
            {
                return String.Empty;
            }
            List<BMSCommon.Model.TurnkeySanc> dt = DB.GetDatabaseObjectsAsAdmin<TurnkeySanc>("turnkeysanc");

            dt = dt.Where(s => s.erc20address == h.GetCurrentUser().ERC20Address).ToList();
            dt = dt.OrderBy(s => Convert.ToDateTime(s.Added)).ToList();
            string data = "<table class='saved'><tr><th>Added<th width=50%>Address<th>Balance<th>Status<th>Action</tr>";

            string ERC20Signature;
            string ERC20Address;
            h.Request.Cookies.TryGetValue("erc20signature", out ERC20Signature);
            h.Request.Cookies.TryGetValue("erc20address", out ERC20Address);
            string sPrivKey = GetConfigKeyValue("foundationprivkey");

            for (int i = 0; i < dt.Count; i++)
            {
                string sBBPAddress = dt[i].BBPAddress;
                double nBalance = BBPAPI.ERCUtilities.QueryAddressBalance(IsTestNet(h), sBBPAddress);
                string sCluster = String.Empty;
                string sNonce = Encryption.DecryptAES256(dt[i].Nonce, sPrivKey);

                string sStatus = String.Empty;
                if (nBalance > 1)
                {
                    sStatus = "Active";
                    sCluster = GetStandardButton("btnliq", "Liquidate", "turnkey_liquidate", "var e={};e.address='" + sBBPAddress + "';",
                        "Are you sure you would like to liquidate this sanctuary?", "turnkeysanc/processdocallback");
                }
                else
                {
                    sStatus = "Waiting for Funding";
                    sCluster = GetStandardButton("btnfund", "Fund", "turnkey_fund", "var e={}; e.address='" 
                        + sBBPAddress + "';", "", "turnkeysanc/processdocallback");
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
                data += GetStandardButton("btnbackup", "Back Up Keys", "turnkey_backup", "var e={};", "", "turnkeysanc/processdocallback");
            }
            return data;
        }


        public IActionResult TurnkeySanctuaries()
        {
            ViewBag.TurnkeySAnctuaryReport = GetTurnkeySanctuaryReport(HttpContext);
            return View();
        }


        public IActionResult MessagePage()
        {
            ViewBag.Title = HttpContext.Session.GetString("msgbox_title");
            ViewBag.Heading = HttpContext.Session.GetString("msgbox_heading");
            ViewBag.Body = HttpContext.Session.GetString("msgbox_body");
            return View();
        }


        public IActionResult Watch()
        {
            ViewBag.WatchVideo = DSQL.youtube.GetVideo(HttpContext);
            return View();
        }


        public string GetAtomicSwapPriceReport(string sDogeAddress)
        {
            price1 nBTCPrice = BBPAPI.PricingService.GetCryptoPrice("BTC/USD");
            price1 nBBPPrice = BBPAPI.PricingService.GetCryptoPrice("BBP/BTC");
            price1 nDOGEPrice = BBPAPI.PricingService.GetCryptoPrice("DOGE/BTC");
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
            ViewBag.DogeAddress = DB.SouthXChange.GetSXAddressByERC20Address("doge", HttpContext.GetCurrentUser().ERC20Address);
            ViewBag.PriceReport = GetAtomicSwapPriceReport(ViewBag.DogeAddress);
            ViewBag.Atomic = await DB.SouthXChange.GetSouthXChangeReport(HttpContext.GetCurrentUser(),IsTestNet(HttpContext));
            return View();
        }


        public IActionResult Videos()
        {
            ViewBag.VideoList = DSQL.youtube.GetSomeVideos(HttpContext);
            return View();
        }
        
        public IActionResult Univ()
        {
            return View();
        }

    }
}
