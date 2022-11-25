using BiblePay.BMS.Extensions;
using BiblePay.BMS.Models;
using BMSCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OptionsShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static BiblePay.BMS.Controllers.IntelController;
using static BiblePay.BMS.DSQL.UI;

namespace BiblePay.BMS.Controllers
{
    public class QuantController : Controller
    {
        public int SendMySignalEmail()
        {
            MailAddress mTo = new MailAddress("rob@biblepay.org", "Rob Andrews");
            MailMessage m = new MailMessage();
            m.To.Add(mTo);
            string sSubject = "UNABLE TO ______";
            m.Subject = sSubject;
            m.Body = "Error, ____________ for .";
            m.IsBodyHtml = false;
            BBPTestHarness.Common.SendMail(false, m);
            return 70;
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
            catch (Exception ex)
            {
            }
            return null;
        }

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
            if (t != null)
                return t;
            t = GetSPYOverTime();
            PutQuantChart("spy", t);
            return t;
        }
        public static List<QuantChartItem> GetQuantChart(string id)
        {
            string sData = OptionsShared.Database.DeserializeObject(id, "BackTest");
            List<QuantChartItem> t1 = DeserializeFromString(sData);
            return t1;
        }
        private void PutQuantChart(string sID, List<QuantChartItem> theChart)
        {
            string sData = SerializeToString(theChart);
            OptionsShared.Database.SerializeObject(sID, "BackTest", sData, 0, "Chart", "Chart");
        }

        public String ToFS(object d)
        {
            double n = BMSCommon.Common.GetDouble(d);
            n = Math.Round(n, 2);
            return n.ToString();
        }
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

        public double GetROI(string sStrategyID, int nLS)
        {
            string s = "Select ROI from BackTest where ID='" + sStrategyID + "' and LongShort = '" + nLS.ToString() + "';";
            SqlCommand cmd1 = new SqlCommand(s);
            double nROI = SQLDatabase.GetScalarDouble(cmd1, "ROI");
            return nROI;
        }

        public static string SubscribeToProduct(string sProductID, HttpContext h)
        {
            string sql = "Insert into Subscription (id,userid,added,productid,description,monthlycost,signature) values "
                + "(newid(),@userid,getdate(),@productid,@desc,@cost,@signature);";
            SqlCommand m1 = new SqlCommand(sql);
            string sError = String.Empty;
            if (!h.GetCurrentUser().LoggedIn)
            {
                sError = "Not logged in.";
            }
            double nBalance = DSQL.UI.QueryAddressBalance(IsTestNet(h), h.GetCurrentUser().BBPAddress);
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

                string s = "Update BackTest set Cost=5 where cost is null;\r\nSelect * from BackTest where id='" + sProductID + "';";
                SqlCommand c1 = new SqlCommand(s);
                DataTable dt = SQLDatabase.GetDataTable(c1);
                if (dt.Rows.Count == 0)
                {
                    sError = "Unable to find product.";
                }
                else
                {

                    string s1 = "Select count(*) ct from Subscription where userid=@userid and productid=@productid;";
                    SqlCommand s10 = new SqlCommand(s1);
                    s10.Parameters.AddWithValue("@userid",h.GetCurrentUser().ERC20Address);
                    s10.Parameters.AddWithValue("@productid",sProductID);
                    double nCt = SQLDatabase.GetScalarDouble(s10, "ct");
                    if (nCt > 0)
                    {
                        sError = "Sorry, you are already subscribed to this product.";
                    }
                    else
                    {
                        m1.Parameters.AddWithValue("@userid", h.GetCurrentUser().ERC20Address);
                        m1.Parameters.AddWithValue("@signature", sSig);
                        m1.Parameters.AddWithValue("@productid", sProductID);
                        m1.Parameters.AddWithValue("@desc", dt.Rows[0]["Narrative"].ToString());
                        m1.Parameters.AddWithValue("@cost", dt.Rows[0]["Cost"]);
                        
                        bool f1 = SQLDatabase.ExecuteNonQuery(m1, "localhost");
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
            string sql = "Select * From BackTest where recordtype='strategy' order by ROI desc;";
            SqlCommand m1 = new SqlCommand(sql);
            DataTable dt = OptionsShared.SQLDatabase.GetDataTable(m1);
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='saved'>");
            sb.Append("<tr><th>Product ID<th>Narrative<th>Added<th>ROI %<th>Cost BBP<th>Monthly Cost USD<th>Subscribe</tr>");
            foreach (DataRow dtr in dt.Rows)
            {
                string sID = dtr["id"].ToString().Substring(0, 16);
                string sROI = dtr["ROI"].AsDouble().Percentage();
                double nCost = dtr["cost"].AsDouble();
                double nCostBBP = DSQL.UI.ConvertUSDToBiblePay(nCost);
                string sSubscribe = GetStandardButton("btnsubscribe", "Subscribe", "quant_subscribe", 
                    "var e={};e.strategyid='" + dtr["id"].ToString() + "';",
                        "Are you sure you would like to subscribe to this algorithm?");
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
            string sql = "Select * From Subscription where Userid='" + sMyID + "' order by Added desc;";
            SqlCommand m1 = new SqlCommand(sql);
            DataTable dt = OptionsShared.SQLDatabase.GetDataTable(m1);
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='saved'>");
            sb.Append("<tr><th>Product ID<th>Narrative<th>Added<th>Monthly Cost BBP<th>Monthly Cost USD</tr>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sID = dt.Rows[i]["id"].ToString().Substring(0, 16);
                double nCost = dt.Rows[i]["MonthlyCost"].ToDouble();
                double nCostBBP = DSQL.UI.ConvertUSDToBiblePay(nCost);
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
            string sql = "Select * From TxHistory where Userid='" + sMyID + "' order by Added desc;";
            SqlCommand m1 = new SqlCommand(sql);
            DataTable dt = OptionsShared.SQLDatabase.GetDataTable(m1);
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='saved'>");
            sb.Append("<tr><th>TXID<th>Description<th>Amount<th>Added</tr>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sID = dt.Rows[i]["id"].ToString().Substring(0, 16);
                double nAmt = dt.Rows[i]["Amount"].ToDouble();
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


        public ActionResult BlackBoxList()
        {
            string sql = "Select * From BackTest where RecordType in ('Strategy','Flagship') Order by ROI desc;";
            SqlCommand m1 = new SqlCommand(sql);
            DataTable dt = OptionsShared.SQLDatabase.GetDataTable(m1);
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='saved'>");
            sb.Append("<tr><th>Launch<th>Launch SPY<th>Narrative<th>Long Short Narr<th>Added<TH>ROI %</tr>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                    string sID = dt.Rows[i]["id"].ToString();
                    TradeAnalysisReport tar = TradeAnalysisReport.Get(sID);
                    string sNarr = tar.Name;
                    string LSNarr = tar.LongShort.ToString();
                    string sROI = tar.ROI == 0 ? "Calculating" : tar.ROI.Percentage();
                    string sInnerLink = "quant/StrategyBacktest?id=" + sID;
                    string sInnerLinkSPY = "quant/StrategyBacktest?id=" + sID + "&SYMBOL=SPY";
                    string sInnerLinkSwitches = "quant/StrategyBacktest?id=" + sID + "&SWITCHES=VIX";
                    string sLink = "<a href='" + sInnerLink + "'>Launch Backtest</a>";
                    string sLinkSPY = "<a href='" + sInnerLinkSPY + "'>SPY Only</a>";
                    string sLinkSW = "<a href='" + sInnerLinkSwitches + "'>Switches</a>";
                    string sRow = "<tr><td>" + sLink + "<td>" + sLinkSPY + "<td>" 
                        + sNarr + "<td>" + LSNarr + "<td>" 
                        +  dt.Rows[i]["Added"].ToString() + "<TD>" + sROI + "</tr>";
                    sb.Append(sRow);
            }
            sb.Append("</table>");
            BlackBoxEditModel m = new BlackBoxEditModel();
            m.Report = sb.ToString();
            return View(m);
        }

        private string GetChartFrame(string sBoxId, string sType)
        {
            string sChart1 = "<iframe style='width:800px;height:450px;border:none;' src='chart/chartview?id="
                + sBoxId + "&type=" + sType + "'/></iframe>\r\n";
            return sChart1;
        }

        public ActionResult StrategyBacktest()
        {
            //bulve
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

    string sHeader = "<tr style='background-color:grey;'><td>Trade No<td>Symbol<td>Qty<td>Open Date<td>Avg Win<td>Avg Loss<td>UL Open<td>UL Close<td>ST Win %<td>LT Win %<td>Wins<td>Volatility<td>P&L<td>ROI %<td>Net Liquidation<td>Max Drawdown<td>Low Bank<td>High Bank</tr>";

    for (int i = 0; i < t.Analysis.Count; i++)
    {
        TradeAnalysisDetail t1 = t.Analysis[i];
        //t1.symbol
        string sRow = "<tr><td>" + t1.TradeNumber.ToString() + "<td>" +t1.Symbol.ToString() + "<td>" 
            +  t1.Quantity.ToString() + "<td>" + t1.OpenDate.ToShortDateString() + "<td>" +  ToFS(t1.AverageWin * 100)
            + "<td>" + ToFS(t1.AverageLoss * 100)
            + "<td>" + t1.UnderlyingOpenPrice.ToString()
            + "<td>" + t1.UnderlyingClosePrice.ToString()
            + "<td>" + t1.WinsShortTermPercentage.Percentage() + "<td>" + t1.WinsLongTermPercentage.Percentage() + "<td>" + t1.Wins.ToString() 
            + "<td>" + t1.Volatility.Percentage() + "<td>" + ToFS(t1.PL) + "<td>" +  t1.ROI.Percentage() 
            + "<td>" + t1.NetLiquidationValue.ToString() + "<td>" + t1.MaxDrawdown.ToString() 
            + "<td>" + t1.LowBank.ToString() + "<td>" + t1.HighBank.ToString() + "</tr>";
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

public ActionResult BlackBoxEdit()
{
    string sID = Request.Query["id"].ToString();
    BlackBoxEditModel m = new BlackBoxEditModel();
    // Load from the database
    string sql = "Select * from Strategy where id=@id;";
    SqlCommand m1 = new SqlCommand(sql);
    m1.Parameters.AddWithValue("@id", sID);
    DataTable dt = OptionsShared.SQLDatabase.GetDataTable(m1);
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
    string sql = "Update Strategy set code=@mycode where id=@id;";
    if (t0.Value.Length > 50)
    {
        SqlCommand m1 = new SqlCommand(sql);
        m1.Parameters.AddWithValue("@id", sID);
        m1.Parameters.AddWithValue("@mycode", t0.Value);
        OptionsShared.SQLDatabase.ExecuteNonQuery(m1, "localhost");
    }
    return Json(returnVal);
}

[HttpPost]
public async Task<JsonResult> DeleteBackTests([FromBody] ClientToServer o)
{
ServerToClient2 returnVal = new ServerToClient2();
returnVal.Body = "alert('Deleted');";
returnVal.Type = "javascript";
string sql = "Delete from BackTest where id not in ('vix','macd','rfr');";
SqlCommand m1 = new SqlCommand(sql);
OptionsShared.SQLDatabase.ExecuteNonQuery(m1, "localhost");

return Json(returnVal);

}



}
}
