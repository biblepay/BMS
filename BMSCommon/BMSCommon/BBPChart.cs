using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BMSCommon
{
    public static class BBPCharting
    {
		public class BBPChart
		{
			public string Name;
			public List<ChartSeries> CollectionSeries = new List<ChartSeries>();
			public List<double> XAxis = new List<double>();
			public string Type;
		}
		public class ChartSeries
		{
			public string Name;
			public string BorderColor;
			public string BackgroundColor;
			public bool Fill;
			public List<double> DataPoint = new List<double>();
		};

		public static string GenerateJavascriptChart(BBPChart c)
		{
			string html = "<script src='https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.9.4/Chart.js'></script>";
			html = "<script src='https://cdn.jsdelivr.net/npm/chart.js@2.9.4/dist/Chart.min.js'></script>";
			html += "<script src='https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.1/moment.min.js'></script>";

			string sID = Guid.NewGuid().ToString();

			html += "\r\n<canvas id='" + sID + "' style='width:100%;max-width:900px'></canvas>";
			html += "\r\n<script>\r\n";

			string xLegends = "var xLegends = [";
			string xdata = "";
			for (int i = 0; i < c.XAxis.Count; i++)
			{
				if (c.Type == "date")
				{
					DateTime dtPoint = BMSCommon.Common.FromUnixTimeStamp((int)c.XAxis[i]);
					string myChartPoint = dtPoint.ToString("s") + "Z";
					xdata += "'" + myChartPoint + "', ";

				}
				else
				{
					xdata += c.XAxis[i].ToString() + ", ";
				}
			}
			if (xdata.Length > 2)
				xdata = xdata.Substring(0, xdata.Length - 2);

			xLegends += xdata + "];\r\n";

			html += xLegends;
			//options: { plugins: { title: {display: true,text:'my title'}
			string sDisplayFormat = "type: 'time',	time:	{ displayFormats: {					'millisecond': 'MMM DD',            'second': 'MMM DD',            'minute': 'MMM DD',"
				+ "            'hour': 'MMM DD',            'day': 'MMM DD',            'week': 'MMM DD',            'month': 'MMM DD',            'quarter': 'MMM DD',            'year': 'MMM DD',"
				+ "			} }";

			sDisplayFormat = "";
			if (c.Type == "xdate")
			{
				sDisplayFormat = "type: 'time',";
			}

			//ticks: { autoSkip:true, maxTicksLimit:15 }}]
			string chartOptions = "options: {	"
				+ "scales: { xAxes:  [{ " + sDisplayFormat + "  }]  }}, ";
			html += "\r\n new Chart('" + sID + "', {   type: 'line', " + chartOptions + " data: { 	labels: xLegends,"
				 + " datasets: @ds } } );";


			string seriesData = "";
			for (int j = 0; j < c.CollectionSeries.Count; j++)
			{
				ChartSeries c1 = c.CollectionSeries[j];
				string dp = "";
				for (int k = 0; k < c1.DataPoint.Count; k++)
				{
					if (c.Type == "xdate")
					{
						DateTime dtPoint = BMSCommon.Common.FromUnixTimeStamp((int)c.XAxis[k]);
						string myChartPoint = "'" + dtPoint.ToString("s") + "Z" + "'";
						dp += "{ t: " + myChartPoint + ", y: " + c1.DataPoint[k].ToString() + "}, ";
					}
					else
					{
						dp += c1.DataPoint[k].ToString() + ", ";
					}
				}
				if (dp.Length > 2)
					dp = dp.Substring(0, dp.Length - 2);
				dp += "\r\n";

				seriesData += "[{ label: '" + c1.Name + "', \r\ndata: [" + dp + "], borderColor: '" + c1.BorderColor.ToString() + "',backgroundColor:'"
					+ c1.BackgroundColor.ToString() + "',fill: true}]";

			}
			html = html.Replace("@ds", seriesData);
			html += "\r\n</script>";
			return html;
		}



		public static string GenerateJavascriptMultiAxisChart(BBPChart c)
		{
			string html = "<script src='https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.9.4/Chart.js'></script>";
			html = "<script src='https://cdn.jsdelivr.net/npm/chart.js@2.9.4/dist/Chart.min.js'></script>";
			html += "<script src='https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.1/moment.min.js'></script>";

			string sID = Guid.NewGuid().ToString();

			html += "\r\n<canvas id='" + sID + "' style='width:100%;max-width:900px'></canvas>";
			html += "\r\n<script>\r\n";

			string xLegends = "var xLegends = [";
			string xdata = "";
			for (int i = 0; i < c.XAxis.Count; i++)
			{
				if (c.Type == "date")
				{
					DateTime dtPoint = BMSCommon.Common.FromUnixTimeStamp((int)c.XAxis[i]);
					string myChartPoint = dtPoint.ToString("s") + "Z";
					xdata += "'" + myChartPoint + "', ";

				}
				else
				{
					xdata += c.XAxis[i].ToString() + ", ";
				}
			}
			if (xdata.Length > 2)
				xdata = xdata.Substring(0, xdata.Length - 2);

			xLegends += xdata + "];\r\n";

			html += xLegends;
			//options: { plugins: { title: {display: true,text:'my title'}
			string sDisplayFormat = "type: 'time',	time:	{ displayFormats: {					'millisecond': 'MMM DD',            'second': 'MMM DD',            'minute': 'MMM DD',"
				+ "            'hour': 'MMM DD',            'day': 'MMM DD',            'week': 'MMM DD',            'month': 'MMM DD',            'quarter': 'MMM DD',            'year': 'MMM DD',"
				+ "			} }";

			sDisplayFormat = "";
			if (c.Type == "xdate")
			{
				sDisplayFormat = "type: 'time',";
			}

			//ticks: { autoSkip:true, maxTicksLimit:15 }}]
			string chartOptions = "options: {	"
				+ "scales: { xAxes:  [{ " + sDisplayFormat + "  }]  }}, ";
			html += "\r\n new Chart('" + sID + "', {   type: 'line', " + chartOptions + " data: { 	labels: xLegends,"
				 + " datasets: @ds } } );";


			string seriesData = "[";
			for (int j = 0; j < c.CollectionSeries.Count; j++)
			{
				ChartSeries c1 = c.CollectionSeries[j];
				string dp = String.Empty;
				for (int k = 0; k < c1.DataPoint.Count; k++)
				{
					if (c.Type == "xdate")
					{
						DateTime dtPoint = BMSCommon.Common.FromUnixTimeStamp((int)c.XAxis[k]);
						string myChartPoint = "'" + dtPoint.ToString("s") + "Z" + "'";
						dp += "{ t: " + myChartPoint + ", y: " + c1.DataPoint[k].ToString() + "}, ";
					}
					else
					{
						dp += c1.DataPoint[k].ToString() + ", ";
					}
				}
				if (dp.Length > 2)
					dp = dp.Substring(0, dp.Length - 2);
				dp += "\r\n";
				// fill: true (generic chart)

				seriesData += "{ label: '" + c1.Name + "', \r\ndata: [" + dp + "], borderColor: '" + c1.BackgroundColor.ToString() + "',backgroundColor:'"
					+ c1.BorderColor.ToString() + "',zyAxisID: '" + c1.Name + "',fill: false},";

			}
			seriesData = seriesData.Substring(0, seriesData.Length - 1);
			seriesData += "]";

			html = html.Replace("@ds", seriesData);
			html += "\r\n</script>";
			return html;
		}



		public static string GetChartOfIndex()
		{
			BBPChart b = new BBPChart();

			b.Name = "BiblePay Weighted CryptoCurrency Index";
			b.Type = "date";

			string[] vTickers = Pricing.msTickers.Split(",");
			string[] vWeights = Pricing.msWeights.Split(",");
			bool fUseIndividualCryptos = false;
			if (fUseIndividualCryptos)
			{
				for (int k = 0; k < vTickers.Length; k++)
				{
					string sTheTicker = Common.GE(vTickers[k], "/", 0);
					ChartSeries c1 = new ChartSeries();
					c1.Name = sTheTicker;
					c1.DataPoint = new List<double>();
					// s.ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.StackedArea;
					b.CollectionSeries.Add(c1);
				}
			}

			string sql1 = "Select added,USD from quotehistory where ticker='IndexValue'";
			Dictionary<DateTime, double> dPrices = new Dictionary<DateTime, double>();

			MySqlCommand m2 = new MySqlCommand(sql1);
			DataTable dt2 = Database.GetDataTable2(m2);
			for (int j = 0; j < dt2.Rows.Count; j++)
			{
				DateTime dt = Convert.ToDateTime(dt2.Rows[j]["added"]);
				double nPrice = BMSCommon.Common.GetDouble(dt2.Rows[j]["USD"]);
				dPrices.Add(dt, nPrice);
			}

			//Index
			ChartSeries sIndex = new ChartSeries();
			sIndex.Name = b.Name;
			sIndex.BorderColor = "lime";
			sIndex.BackgroundColor = "green";
			b.CollectionSeries.Add(sIndex);
			double nPrice2 = 0;

			//Convert to opensource version: https://www.w3schools.com/ai/ai_chartjs.asp
			int iStep = 1;
			DateTime dtStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

			for (int i = 180; i > 1; i = i - iStep)
			{
				DateTime dt = dtStart.AddDays(-1 * i);

				long nTimestamp = BMSCommon.Common.DateToUnixTimestamp(dt);

				b.XAxis.Add(nTimestamp);

				if (fUseIndividualCryptos)
				{
					for (int j = 0; j < vTickers.Length; j++)
					{
						string sTheTicker = Common.GE(vTickers[j], "/", 0);
						string sql = "Select * from quotehistory where added='" + dt.ToShortDateString() + "' and ticker='" + sTheTicker + "'";
						MySqlCommand m1 = new MySqlCommand(sql);
						DataTable dt1 = Database.GetDataTable2(m1);
						if (dt1.Rows.Count > 0)
						{
							double dA = Common.GetDouble(dt1.Rows[0]["USD"]);
							if (fUseIndividualCryptos)
							{
								double dWeight = Common.GetDouble(Common.GE(vWeights[j], ",", 0));
								double dAdj = dWeight * dA;
								b.CollectionSeries[j].DataPoint.Add(dAdj);
							}
						}
					}
				}


				bool fGot = dPrices.TryGetValue(dt, out nPrice2);
				if (nPrice2 == 0)
				{
					// This is a base level for the cryptocurrency homogenized index.  This doesn't get hit after chart is 60 days old.
					nPrice2 = 15000;
				}

				b.CollectionSeries[0].DataPoint.Add(nPrice2);

			}

			string html = GenerateJavascriptChart(b);
			return html;
		}

	}
}
