using System;
using System.Collections.Generic;
using System.Data;
using static BMSCommon.Common;


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
                    DateTime dtPoint = FromUnixTimeStamp((int)c.XAxis[i]);
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
                        DateTime dtPoint = FromUnixTimeStamp((int)c.XAxis[k]);
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
                    DateTime dtPoint = FromUnixTimeStamp((int)c.XAxis[i]);
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
                        DateTime dtPoint = FromUnixTimeStamp((int)c.XAxis[k]);
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


        public static string Mydt(DateTime dt)
        {
            string s = dt.ToString("yyyy-MM-dd 00:00:01");
            return s;
        }

       
    }
}
