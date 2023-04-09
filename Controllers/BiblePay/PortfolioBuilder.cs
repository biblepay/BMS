using BBPAPI;
using BBPAPI.Model;
using BMSCommon.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using static BBPAPI.DB.Sanctuary;
using static BiblePay.BMS.DSQL.ControllerExtensions;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UI;
using static BMSCommon.BitcoinSync;

namespace BiblePay.BMS.Controllers
{
    public class PortfolioBuilderController : Controller
    {

        [HttpPost]
        public JsonResult ProcessDoCallback([FromBody] ClientToServer o)
        {
            User u0 = GetUser(HttpContext);
            if (o.Action == "PortfolioBuilder_ToggleMode")
            {
                if (HttpContext.Session.GetString("PortfolioBuilderLeaderboardMode") != "Detail")
                {
                    HttpContext.Session.SetString("PortfolioBuilderLeaderboardMode", "Detail");
                }
                else
                {
                    HttpContext.Session.SetString("PortfolioBuilderLeaderboardMode", "Summary");
                }
                return this.ReturnURL("/portfoliobuilder/portfoliobuilderleaderboard");
            }
            else
            {
                throw new Exception("Unknown method");
            }
        }


        public static SanctuaryProfitability _sp = null;
        public static DwuPack _dwupack = new DwuPack();
        public IActionResult PortfolioBuilder()
        {
            ViewBag.CryptoCurrencyIndex = PricingService.GetChartOfIndex();
            if (_sp==null)
                _sp = DB.Sanctuary.GetMasternodeROI(IsTestNet(HttpContext));

            ViewBag.SanctuaryCost = "$" + Math.Round(_sp.nInvestmentAmount, 2) + " USD";
            if (_dwupack.nDWU==0)
            {
                _dwupack = BBPAPI.BlockChairTestHarness.GetDWU(IsTestNet(HttpContext));
            }
            ViewBag.DWUNarrative = "** On a portfolio with 100% coverage and foreign positions";
            ViewBag.BonusPercent = _dwupack.nBonusPercent;
            ViewBag.DWU = Math.Round(_dwupack.nDWU * 100, 2).ToString() + "%";
            ViewBag.SanctuaryGrossROI = Math.Round(_sp.nSanctuaryGrossROI, 2).ToString() + "%";
            ViewBag.BonusROI = Math.Round(_dwupack.nBonusPercent * 100, 2) + " %";
            ViewBag.TurnkeySanctuaryNetROI = Math.Round(_sp.nTurnkeySanctuaryNetROI, 2).ToString() + "%";
            ViewBag.SanctuaryCount = _sp.nMasternodeCount.ToString();
            MySancMetrics msm = DB.Sanctuary.GetTurnkeySanctuariesCount();
            ViewBag.TurnkeySanctuaryCount = msm.nTotalCount;
            ViewBag.TurnkeySanctuaryTotal = msm.nTotalBalance;
            // native coins
            List<DropDownItem> ddSymbol = new List<DropDownItem>();
            ddSymbol.Add(new DropDownItem("BTC", "Bitcoin"));
            ddSymbol.Add(new DropDownItem("DOGE", "DOGE"));
            ddSymbol.Add(new DropDownItem("DASH", "DASH"));
            ViewBag.ddSymbol = ListToHTMLSelect(ddSymbol, String.Empty);
            return View();
        }


        public IActionResult PortfolioBuilderLeaderboard()
        {
            ViewBag.PortfolioBuilderLeaderboard = GetLeaderboard(HttpContext, IsTestNet(HttpContext));
            ViewBag.PortfolioBuilderMode = GetPBMode(HttpContext);
            return View();
        }


    }
}
