using BBPAPI;
using BBPAPI.Model;
using BiblePay.BMS.DSQL;
using BiblePay.BMS.Extensions;
using BiblePay.BMS.Models;
using BMSCommon;
using BMSCommon.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using static BiblePay.BMS.DSQL.DOMItem;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UI;
using static BiblePay.BMS.DSQL.UIWallet;
using static BMSCommon.Common;
using static BMSCommon.Extensions;

namespace BiblePay.BMS.Controllers
{
	public class QuantController : Controller
    {

        [HttpPost]
        public JsonResult ProcessDoCallback([FromBody] ClientToServer o)
        {
            ServerToClient returnVal = new ServerToClient();
            User u0 = GetUser(HttpContext);

            if (o.Action == "Profile_SaveQuant")
            {
                double nLiq = GetFormData(o.FormData, "txtNetLiq").ToDouble();
                if (nLiq > 0)
                {
                }
            }
            else if (o.Action == "quant_subscribe")
            {
                dynamic a = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sProdID = a.strategyid.Value;
                string sResult = QuantController.SubscribeToProduct(sProdID, this.HttpContext);
                if (sResult == String.Empty)
                {
                    DSQL.UI.MsgBox(HttpContext, "Subscribed", "Subscribed",
                        "Thank you for subscribing to this quant strategy. "
                        + "Your account will automatically be debited on the first of each month for the monthly "
                        + "subscription fee denominated in BBP.  You may cancel at any time by visiting My Subscriptions. <br><br>As long as your account is in good standing, you will receive a weekly "
                        + "Signal e-mail containing the analysis service Signal Output for hypothetical trades that this strategy would execute if "
                        + "the computer were following these rules for a hypothetical process in an investment fund.  <br><br>By using this service you agree that all investment signals are for informational purposes only, "
                        + " and do not constitute trading advice.  It is at your sole discretion to fully evaluate each possible trade and make a SELF DIRECTED DECISION.   "
                        + " By using this service you agree to take responsibility for your own actions, and you hereby hold BiblePay and our Quant division harmless "
                        + " from all harm that may arise by acting on your Self Directed actions in your personal trading account.  <br><br>PAST PERFORMANCE IS NOT A GUARANTEE OF FUTURE RESULTS.\r\n", false);
                }
                else
                {
                    DSQL.UI.MsgBox(HttpContext, "Error", "Error", sResult, false);
                }

                string m = "location.href='bbp/messagepage';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else
            {
                throw new Exception("");
            }
            return Json("");
        }




        public string SerializeToString(List<QuantChartItem> theChart)
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(theChart.GetType());
            StringWriter m = new StringWriter();
            x.Serialize(m, theChart);
            return m.ToString();
        }

        public static List<QuantChartItem> DeserializeFromString(string sData)
        {
            List<QuantChartItem> tDummy = new List<QuantChartItem>();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(tDummy.GetType());
            try
            {
                StringReader srIn = new StringReader(sData);
                List<QuantChartItem> j = (List<QuantChartItem>)x.Deserialize(srIn);
                return j;
            }
            catch (Exception)
            {
            }
            return null;
        }

		/*

public List<QuantChartItem> GetCachedMACD(string sType)
{
	List<QuantChartItem> t = GetQuantChart(sType);
	if (t != null)
		return t;
	t = GetMACDOverTime(sType);
	PutQuantChart(sType,t);
	return t;
}

public List<QuantChartItem> GetCachedRFR()
{
	List<QuantChartItem> t = GetQuantChart("rfr");
	if (t != null)
		return t;
	t = GetRFROverTime();
	PutQuantChart("rfr", t);
	return t;
}

public List<QuantChartItem> GetCachedVIX()
{
	List<QuantChartItem> t = GetQuantChart("vix");
	if (t != null)
		return t;
	t = GetVIXOverTime();
	PutQuantChart("vix", t);
	return t;
}
public List<QuantChartItem> GetCachedSPY()
{
	List<QuantChartItem> t = GetQuantChart("spy");
	if (t != null && t.Count > 0)
		return t;
	t = GetSPYOverTime();
	PutQuantChart("spy", t);
	return t;
}
*/
        /*
		public static List<QuantChartItem> GetQuantChart(string id)
        {
            string sData = DB.Financial.DeserializeOptionsObject(id, "BackTest");
            List<QuantChartItem> t1 = DeserializeFromString(sData);
            return t1;
        }
        private void PutQuantChart(string sID, List<QuantChartItem> theChart)
        {
            string sData = SerializeToString(theChart);
            DB.Financial.SerializeOptionsObject(sID, "BackTest", sData, 0, "Chart", "Chart");
        }
        */


        public String ToFS(object d)
        {
            double n = GetDouble(d);
            n = Math.Round(n, 2);
            return n.ToString();
        }

		/*
        public static List<QuantChartItem> GetMACDOverTime(string sType)
        {
            List<QuantChartItem> l = new List<QuantChartItem>();
            DateTime dtStart = Convert.ToDateTime("5/1/2006");
            for (int i = 0; i < 20*365; i+=14)
            {
                DateTime dt = dtStart.AddDays(i);
                MACD m = OptionsShared.VIX.GetMACD(dt);
                if (m != null)
                {
                    QuantChartItem c = new QuantChartItem();
                    c.date = dt;
                    if (sType == "MACD")
                    {
                        c.value = m.DMA12 - m.DMA26;
                    }else if (sType=="BollingerUpper")
                    {
                        c.value = m.BollingerUpperBand;
                    }
                    else if (sType=="BollingerLower")
                    {
                        c.value = m.BollingerLowerBand;
                    }
                    else if (sType=="KeltnerUpper")
                    {
                        c.value = m.KeltnerUpperBand;
                    }
                    else if (sType=="KeltnerLower")
                    {
                        c.value = m.KeltnerLowerBand;
                    }
                    else if (sType=="TTMSqueeze")
                    {
                        c.value = m.TTMSqueeze;
                    }
                    l.Add(c);
                }
            }
            return l;
        }
        
        public static List<QuantChartItem> GetRFROverTime()
        {
            List<QuantChartItem> l = new List<QuantChartItem>();
            DateTime dtStart = Convert.ToDateTime("5/1/2006");
            for (int i = 0; i < 20 * 365; i += 14)
            {
                DateTime dt = dtStart.AddDays(i);
                double r = OptionsShared.VIX.GetRFR(dt);
                if (r != 0)
                {
                    QuantChartItem c = new QuantChartItem();
                    c.date = dt;
                    c.value = r;
                    l.Add(c);
                }
            }
            return l;

        }
        */





		/*
        public static List<QuantChartItem> GetVIXOverTime()
        {
            List<QuantChartItem> l = new List<QuantChartItem>();
            DateTime dtStart = Convert.ToDateTime("5/1/2006");
            for (int i = 0; i < 20 * 365; i += 14)
            {
                DateTime dt = dtStart.AddDays(i);
                double nVIX = OptionsShared.VIX.GetVIX(dt);
                if (nVIX != 0)
                {
                    QuantChartItem c = new QuantChartItem();
                    c.date = dt;
                    c.value = nVIX;
                    l.Add(c);
                }
            }
            return l;
        }
        public static List<QuantChartItem> GetSPYOverTime()
        {
            List<QuantChartItem> l = new List<QuantChartItem>();
            DateTime dtStart = Convert.ToDateTime("5/1/2006");
            for (int i = 0; i < 20 * 365; i += 14)
            {
                DateTime dt = dtStart.AddDays(i);
                double nVIX = OptionsShared.VIX.GetSpyULPrice(dt);
                if (nVIX != 0)
                {
                    QuantChartItem c = new QuantChartItem();
                    c.date = dt;
                    c.value = nVIX;
                    l.Add(c);
                }
            }
            return l;
        }
        public List<QuantChartItem> ConvertToQuantChart(TradeAnalysisReport t, string sType)
        {
            List<QuantChartItem> l = new List<QuantChartItem>();
            for (int i = 0; i < t.Analysis.Count; i++)
            {
                TradeAnalysisDetail d = t.Analysis[i];
                QuantChartItem c = new QuantChartItem();
                c.date = d.OpenDate;
                if (sType == "netliq")
                {
                    c.value = d.NetLiquidationValue;
                }
                else
                    if (sType == "roi")
                {
                    c.value = d.ROI * 100;
                }
                else if (sType == "st")
                {
                    c.value = d.WinsShortTermPercentage * 100;
                }
                else if (sType == "averagecost")
                {
                    c.value = -1 * (d.AverageCost * 100);
                }
                else if (sType == "averageloss")
                {
                    c.value = -1*(d.AverageLoss * 100);
                }
                else if (sType == "averagewin")
                {
                    c.value = d.AverageWin * 100;
                }
                l.Add(c);
            }
            return l;
        }
        */


		public static string SubscribeToProduct(string sProductID, HttpContext h)
        {
            string sError = String.Empty;
            if (!h.GetCurrentUser().LoggedIn)
            {
                sError = "Not logged in.";
            }
            double nBalance = BBPAPI.ERCUtilities.QueryAddressBalance(IsTestNet(h), h.GetCurrentUser().BBPAddress);
            if (nBalance < 1000)
            {
                sError = "Your BBP balance is too low; please add funds.";
            }
            string sSig = String.Empty;
            h.Request.Cookies.TryGetValue("erc20signature", out sSig);
            if (sSig == String.Empty)
            {
                sError = "Your metamask signature is null, please log out and log back in first.";
            }

            if (h.GetCurrentUser().EmailAddress == String.Empty)
            {
                sError = "Your e-mail address must be populated first.";
            }
            if (sError == String.Empty)
            {

                DataTable dt = null;// DB.Financial.GetFinancialBacktestProduct(sProductID);
                if (dt.Rows.Count == 0)
                {
                    sError = "Unable to find product.";
                }
                else
                {

                    double nCt = 0;// DB.Financial.GetSubscriptionCountByUser(h.GetCurrentUser().ERC20Address, sProductID);
                    if (nCt > 0)
                    {
                        sError = "Sorry, you are already subscribed to this product.";
                    }
                    else
                    {
                        bool f1 = false;// DB.Financial.SubscribeToProduct(h.GetCurrentUser().ERC20Address, sSig, sProductID, dt.Rows[0]["Narrative"].ToString(), GetDouble(dt.Rows[0]["Cost"]));

                        if (!f1)
                        {
                            sError = "Unable to subscribe";
                        }
                        else
                        {
                            return String.Empty;
                        }
                    }
                }
            }
            return sError;
        }

        public ActionResult ProductList()
        {
            DataTable dt = null;// DB.Financial.GetFinancialProductList();
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='saved'>");
            sb.Append("<tr><th>Product ID<th>Narrative<th>Added<th>ROI %<th>Cost BBP<th>Monthly Cost USD<th>Subscribe</tr>");
            foreach (DataRow dtr in dt.Rows)
            {
                string sID = dtr["id"].ToString().Substring(0, 16);
                string sROI = dtr["ROI"].AsDouble().ToString(); // Percentage();
                double nCost = dtr["cost"].AsDouble();
                double nCostBBP = ConvertUSDToBiblePay(nCost);
                string sSubscribe = GetStandardButton("btnsubscribe", "Subscribe", "quant_subscribe", 
                    "var e={};e.strategyid='" + dtr["id"].ToString() + "';",
                    "Are you sure you would like to subscribe to this algorithm?", "quant/processdocallback");
                string sRow = "<tr><td>" + sID + "<td>" + dtr["Narrative"] + "<td>"
                    + dtr["Added"].ToShortDateString() + "<td>" + sROI + "<td>" + FormatUSD(nCostBBP) + " BBP" 
                    + "<td>" + nCost.ToString() + "<td>" + sSubscribe + "</td></tr>";
                sb.Append(sRow);
            }
            sb.Append("</table>");
            BlackBoxEditModel m = new BlackBoxEditModel();
            m.Report = sb.ToString();
            return View(m);
        }


        public ActionResult Subscriptions()
        {
            string sMyID = HttpContext.GetCurrentUser().ERC20Address;
            DataTable dt = null;// DB.Financial.GetSubscriptionsByUserID(sMyID);
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='saved'>");
            sb.Append("<tr><th>Product ID<th>Narrative<th>Added<th>Monthly Cost BBP<th>Monthly Cost USD</tr>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sID = dt.Rows[i]["id"].ToString().Substring(0, 16);
                double nCost = dt.Rows[i]["MonthlyCost"].ToString().ToDouble();
                double nCostBBP = ConvertUSDToBiblePay(nCost);
                string sRow = "<tr><td>" + sID + "<td>" + dt.Rows[i]["Description"] + "<td>"
                    + dt.Rows[i]["Added"].ToShortDateString() + "<td>" + FormatUSD(nCostBBP) + " BBP"
                    + "<td>" + nCost.ToString() + "</tr>";
                sb.Append(sRow);
            }
            sb.Append("</table>");
            BlackBoxEditModel m = new BlackBoxEditModel();
            m.Report = sb.ToString();
            return View(m);
        }

        public ActionResult TransactionHistory()
        {
            string sMyID = HttpContext.GetCurrentUser().ERC20Address;
            DataTable dt = null;// DB.Financial.GetTxHistory(sMyID);
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='saved'>");
            sb.Append("<tr><th>TXID<th>Description<th>Amount<th>Added</tr>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sID = dt.Rows[i]["id"].ToString().Substring(0, 16);
                double nAmt = dt.Rows[i]["Amount"].ToString().ToDouble();
                string sRow = "<tr><td>" + dt.Rows[i]["TXID"].ToString() + "<td>" + dt.Rows[i]["Description"] + "<td>"
                    + FormatUSD(nAmt) + "<td>" + dt.Rows[i]["Added"].ToShortDateString()
                    + "</tr>";
                sb.Append(sRow);
            }
            sb.Append("</table>");
            BlackBoxEditModel m = new BlackBoxEditModel();
            m.Report = sb.ToString();
            return View(m);
        }

        /*

        public ActionResult BlackBoxList()
        {
            DataTable dt = DB.Financial.GetBlackBoxList();
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='saved'>");
            sb.Append("<tr><th>Launch<th>Launch SPY<th>Narrative<th>Added<TH>ROI %</tr>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                 string sID = dt.Rows[i]["id"].ToString();
                 string sNarr = dt.Rows[i]["Narrative"].ToString();
                 double nROI = dt.Rows[i]["ROI"].ToString().ToDouble();
                 string sROI = nROI == 0 ? "Calculating" : nROI.Percentage();
                 string sInnerLink = "quant/StrategyBacktest?id=" + sID;
                 string sInnerLinkSPY = "quant/StrategyBacktest?id=" + sID + "&SYMBOL=SPY";
                 string sInnerLinkSwitches = "quant/StrategyBacktest?id=" + sID + "&SWITCHES=VIX";
                 string sLink = "<a href='" + sInnerLink + "'>Launch Backtest</a>";
                 string sLinkSPY = "<a href='" + sInnerLinkSPY + "'>SPY Only</a>";
                 string sRow = "<tr><td>" + sLink + "<td>" + sLinkSPY + "<td>" 
                        + sNarr + "<td>" 
                        + dt.Rows[i]["Added"].ToString() + "<TD>" + sROI + "</tr>";
                 sb.Append(sRow);
            }
            sb.Append("</table>");
            BlackBoxEditModel m = new BlackBoxEditModel();
            m.Report = sb.ToString();
            return View(m);
        }
        */


        private string GetChartFrame(string sBoxId, string sType)
        {
            string sChart1 = "<iframe style='width:800px;height:450px;border:none;' src='chart/chartview?id="
                + sBoxId + "&type=" + sType + "'/></iframe>\r\n";
            return sChart1;
        }

        /*
        public ActionResult StrategyBacktest()
        {
            
            BlackBoxEditModel m = new BlackBoxEditModel();
            string sID = Request.Query["id"].ToString();
            //OptionsShared.ProfitLoss.LONGSHORT L0 = LS == 1 ? OptionsShared.ProfitLoss.LONGSHORT.LONG : OptionsShared.ProfitLoss.LONGSHORT.SHORT;
            OptionsShared.TradeAnalysisReport t = OptionsShared.TradeAnalysisReport.Get(sID);
            StringBuilder sHTML = new StringBuilder();

            // Graphs here
            // Bank over Time
            // Short Term Win % over time
            // ROI over time
            sHTML.Append(GetChartFrame(sID, "netliq"));
            sHTML.Append(GetChartFrame(sID, "roi"));
            sHTML.Append(GetChartFrame(sID, "stwin"));
            sHTML.Append(GetChartFrame(sID, "costs"));
            sHTML.Append(GetChartFrame(sID, "squeeze"));
            sHTML.Append(GetChartFrame(sID, "spy"));

            sHTML.Append("<h3>" + t.Name + " - " + t.Strategy);

            if (t.Analysis.Count < 1)
            {
                return View(m);
            }
            string sFooter = "<br><h4>Summary:</h4><table class=saved><tr><td>ROI<td>" + t.Analysis[t.Analysis.Count - 1].ROI.Percentage() + "</tr><tr><td>Alpha<td>"
            + t.Alpha.ToString() + "</tr><tr><td>Beta<td>" + t.Beta.ToString() + "</tr><tr><td>Sharpe Ratio<td>"
            + t.SharpeRatio.ToString() + "</tr><tr><td>Average Duration<td>" + t.AverageDuration.ToString() + "</tr><tr><td>Start Date<td>"
            + t.StartDate.ToShortDateString() + "</tr><tr><td>End Date<td>" + t.EndDate.ToShortDateString()
            + "</tr><tr><td>Leverage<td>" + t.Leverage.ToString();
            sFooter += "</td></tr><tr><td>Name<td>" + t.Name + "</tr><tr><td>Strategy<td>" + t.Strategy + "<tr><td>Long/Short<td>"
                + t.LongShort + "<tr><td>Risk Free Rate<td>" + t.RFR.ToString() + "<tr><td>Tone<td>" + t.Tone.ToString() + "</tr>";

            sFooter += "<tr><td>Monte Carlo Positive Sum<td>" + t.MonteCarloPositive
                + "<tr><td>Monte Carlo Negative Sum<td>" + t.MonteCarloNegative + "<td>Monte Carlo Steps<td>" + t.MonteCarloSteps + "</tr></table>";

            sHTML.Append(sFooter);

            sHTML.Append("<table class=saved>");

            string sHeader = "<tr style='background-color:yellow;'><td>Trade No<td>Symbol"
                        +"<td>Qty<td>Open Date<td>Avg Win<td>Avg Loss<td>UL Open<td>UL Close<td>ST Win %<td>LT Win %<td>Wins"
                        +"<td>P&L<td>ROI %<td>Net Liq.<td>Max Drawdown<td>Low Bank</tr>";

            for (int i = 0; i < t.Analysis.Count; i++)
            {
                TradeAnalysisDetail t1 = t.Analysis[i];
                string sRow = "<tr><td>" + t1.TradeNumber.ToString() + "<td>" +t1.Symbol.ToString() + "<td>" 
                    +  t1.Quantity.ToString() + "<td>" + t1.OpenDate.ToShortDateString() + "<td>" +  ToFS(t1.AverageWin * 100)
                    + "<td>" + ToFS(t1.AverageLoss * 100)
                    + "<td>" + t1.UnderlyingOpenPrice.ToString()
                    + "<td>" + t1.UnderlyingClosePrice.ToString()
                    + "<td>" + t1.WinsShortTermPercentage.Percentage() + "<td>" + t1.WinsLongTermPercentage.Percentage() + "<td>" + t1.Wins.ToString() 
                    + "<td>" + ToFS(t1.PL) + "<td>" +  t1.ROI.Percentage() 
                    + "<td>" + t1.NetLiquidationValue.ToCurrency() + "<td>" 
                    + t1.MaxDrawdown.ToCurrency() 
                    + "<td>" + t1.LowBank.ToCurrency() + "</tr>";
                if (i % 20 == 0)
                {
                    sHTML.Append(sHeader);
                }
                if (t1.Narrative != null)
                {
                    string sRow2 = "<tr><td colspan=10>" + t1.Narrative.ToString() + "</td></tr>";
                    sHTML.Append(sRow2);
                }
                sHTML.Append(sRow + "\r\n");

                if (i > 7000)
                    break;
        }
        sHTML.Append("</table>");
        m.Report = sHTML.ToString();
        return View(m);
    }
        */



public ActionResult BlackBoxEdit()
{
    string sID = Request.Query["id"].ToString();
    BlackBoxEditModel m = new BlackBoxEditModel();
    DataTable dt = null;// DB.Financial.GetStrategyByID(sID);
    if (dt.Rows.Count > 0)
    {
        string sCode = dt.Rows[0]["code"].ToString();
        m.Code = sCode;
    }
    return View(m);
}


[HttpPost]
public JsonResult Save([FromBody] ClientToServer o)
{
    ServerToClient2 returnVal = new ServerToClient2();
    returnVal.Body = "alert('saved');";
    returnVal.Type = "javascript";
    string sID = Request.Query["id"].ToString();
    if (sID == String.Empty)
        return Json("err");

    TransformDOM t = new TransformDOM(o.FormData);
    DOMItem t0 = t.GetDOMItem("g0", "mycode");
    string sql = "Update options.Strategy set code=@mycode where id=@id;";
    if (t0.Value.Length > 50)
    {
        NpgsqlCommand m1 = new NpgsqlCommand(sql);
        m1.Parameters.AddWithValue("@id", sID);
        m1.Parameters.AddWithValue("@mycode", t0.Value);
        //CockroachDatabase.ExecuteNonQuery(m1, "localhost");
    }
    return Json(returnVal);
}

[HttpPost]
public JsonResult DeleteBackTests([FromBody] ClientToServer o)
{
    BMSCommon.Model.ServerToClient2 returnVal = new ServerToClient2();
    returnVal.Body = "alert('Not Deleted');";
    returnVal.Type = "javascript";
    return Json(returnVal);
}


}
}
