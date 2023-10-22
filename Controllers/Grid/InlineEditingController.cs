#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using BBPAPI;
using BMSCommon;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;

namespace BiblePay.BMS.Controllers
{
	public partial class GridController : Controller
    {
        public class ColumnChartData
        {
            public DateTime Date { get; set; }
            public double ULPrice;
            public double PutExtrinsic;
            public double CallExtrinsic;
            public double Strike;
        }

        public IActionResult InlineEditing()
        {

            DataTable dt = BBPAPI.Interface.Phone.GetRatesReport(1000);

            //var order = OrdersDetails.GetAllRecords();
            ViewBag.RateSource = dt;
            ViewBag.ddDataSource = new string[] { "Top", "Bottom" };

            List<ColumnChartData> chartData = new List<ColumnChartData>
            {
               // new ColumnChartData{ country= "USA", gold=50, silver=70, bronze=45 },
               // new ColumnChartData{ country="China", gold=40, silver= 60, bronze=55 },
               // new ColumnChartData{ country= "Japan", gold=70, silver= 60, bronze=50 },
               // new ColumnChartData{ country= "Australia", gold=60, silver= 56, bronze=40 },
               // new ColumnChartData{ country= "France", gold=50, silver= 45, bronze=35 },
               // new ColumnChartData{ country= "Germany", gold=40, silver=30, bronze=22 },
               // new ColumnChartData{ country= "Italy", gold=40, silver=35, bronze=37 },
               // new ColumnChartData{ country= "Sweden", gold=30, silver=25, bronze=27 }
            };
            ViewBag.dataSource = chartData;

            string sql = "exec  GetDividendReport '6-1-2008','10-1-2008','spy'; ";
            DataTable dt1 = null;
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                double ulPrice = dt1.Rows[i]["ULPrice"].ToString().ToDouble();
                string dataDate = dt1.Rows[i]["datadate"].ToString();
                ColumnChartData ccd =new ColumnChartData();
                ccd.ULPrice = ulPrice;
                ccd.Date = Convert.ToDateTime(dataDate);
                ccd.PutExtrinsic = dt1.Rows[i]["PutExtrinsic"].ToString().ToDouble() * 10;
                ccd.CallExtrinsic = dt1.Rows[i]["CallExtrinsic"].ToString().ToDouble() * 10;
                ccd.Strike = dt1.Rows[i]["Strike"].ToString().ToDouble();

                if (ccd.Strike == 100 || ccd.Strike == 150)
                {
                    chartData.Add(ccd);
                }
            }

            return View();
        }
    }
}