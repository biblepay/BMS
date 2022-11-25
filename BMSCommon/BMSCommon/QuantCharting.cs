using System;
using System.Collections.Generic;
using System.Text;
using static BMSCommon.BBPCharting;
using static BMSCommon.Pricing;

namespace BMSCommon
{

	public class QuantChart
	{
		public string Name { get; set; }

		public List<QuantChartItem> Chart {get;set;}
		public string BackColor { get; set; }
		public string BorderColor { get; set; }
		public QuantChart(string sName, string sBackColor, string sBorderColor, List<QuantChartItem> qci)
        {
			Name = sName;
			BackColor = sBackColor;
			BorderColor = sBorderColor;
			Chart = qci;
        }
	}
	public class QuantChartItem
    {
		public DateTime date { get; set; }
		public double value { get; set; }
    }
    public static class QuantCharting
    {

		public static string GetChartOfGenericData(List<QuantChart> l)
		{
			BBPChart b = new BBPChart();
			b.Name = "?";
			b.Type = "date";
			for (int i = 0; i < l.Count; i++)
            {
				ChartSeries sCS = new ChartSeries();
				sCS.Name = l[i].Name;
				sCS.BackgroundColor = l[i].BackColor;
				sCS.BorderColor = l[i].BorderColor;
				b.CollectionSeries.Add(sCS);
			}
			int iStep = l[0].Chart.Count / 360;
			if (iStep < 1)
				iStep = 1;
			for (int i = 0; i < l[0].Chart.Count; i += iStep)
			{
				QuantChartItem dpBase = l[0].Chart[i];
				long nTimestamp = BMSCommon.Common.DateToUnixTimestamp(dpBase.date);
				if (!b.XAxis.Contains(nTimestamp))
					b.XAxis.Add(nTimestamp);
				// Item data
				for (int iCharts = 0; iCharts < l.Count; iCharts++)
				{
					for (int x = 0; x < l[iCharts].Chart.Count; x++)
					{
						QuantChartItem dp = l[iCharts].Chart[x];
						if (dp.date >= dpBase.date.AddDays(-14) && dp.date <= dpBase.date.AddDays(14))
						{
							b.CollectionSeries[iCharts].DataPoint.Add(dp.value);
							break;
						}
					}
				}
			}
			string html = GenerateJavascriptMultiAxisChart(b);
			return html;
		}


	}
}
