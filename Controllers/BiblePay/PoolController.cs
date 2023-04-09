
using BBPAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using static BiblePay.BMS.DSQL.DOMItem;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UI;
using static BiblePay.BMS.DSQL.UIWallet;
using static BMSCommon.BitcoinSync;
using static BMSCommon.Common;
using static BMSCommon.Extensions;



namespace BiblePay.BMS.Controllers
{
    public class PoolController : Controller
    {
        public IActionResult PoolGettingStarted()
        {
            int nPortMainnet = (int)GetDouble(GetConfigKeyValue("XMRPort"));
            int nPortTestNet = (int)GetDouble(GetConfigKeyValue("XMRPortTestNet"));
            if (nPortMainnet == 0)
                nPortMainnet = 3001;
            if (nPortTestNet == 0)
                nPortTestNet = 3002;

            int nPort = IsTestNet(HttpContext) ? nPortTestNet : nPortMainnet;

            ViewBag.PoolDNS = "unchained.biblepay.org:" + nPort.ToString();
            return View();
        }


        protected string GetBlockHistoryReport(bool fTestNet)
        {
            return BBPAPI.DB.OperationProcs.GetPoolHistoryReport(fTestNet);
        }

        public string GetTR(string key, string value)
        {
            string tr = "<TR><TD width='25%'>" + key + ":</TD><TD>" + value + "</TD></TR>\r\n";
            return tr;
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


        protected string GetLeaderboard(bool fTestNet)
        {
            string html = BBPAPI.DB.OperationProcs.GetPoolLeaderboard2(fTestNet);
            return html;
        }

        public IActionResult BlockHistory()
        {
            ViewBag.BlockHistoryReport = GetBlockHistoryReport(IsTestNet(HttpContext));
            return View();
        }

        public IActionResult PoolAbout()
        {
            bool fTestNet = IsTestNet(HttpContext);
            ViewBag.PoolBonusNarrative = XMRPoolBase.PoolBonusNarrative();
            XMRPoolBase x = fTestNet ? PoolBase.tPool : PoolBase.mPool;
            ViewBag.PoolMetrics = PoolBase.GetPoolAboutMetrics(x,IsTestNet(HttpContext));
            ViewBag.OrphanPicture = String.Empty;
            ViewBag.ChartOfHashRate =  BBPAPI.XMRPoolBase.GetChartOfHashRate(fTestNet, x._template.height);
            ViewBag.ChartOfWorkers = BBPAPI.XMRPoolBase.GetChartOfWorkers(x._template.height, fTestNet);
            ViewBag.ChartOfBlocks = BBPAPI.XMRPoolBase.GetChartOfBlocks(fTestNet, x._template.height);
            return View();
        }

        public IActionResult PoolLeaderboard()
        {
            PoolLeaderboardModel.Models.LeaderboardResponse lr = new PoolLeaderboardModel.Models.LeaderboardResponse();
            lr.items = DB.OperationProcs.GetPoolLeaderboardReport(IsTestNet(HttpContext));
            return View(lr);
        }


    }
}
