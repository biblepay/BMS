using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
//using static BiblePayCommon.Common;
//using static BiblePayCommonNET.DataTableExtensions;
//using static BiblePayCommon.DataTableExtensions;
using System.Text;
using MySql.Data.MySqlClient;

namespace BiblePay.BMS
{
    public static class Report
    {
        public static string DoFormat(double myNumber, double nDiscPct = 0)
        {
            if (nDiscPct > 0)
            {
                double nDiscAmt = myNumber * (nDiscPct / 100);
                double nNewAmt = myNumber - nDiscAmt;
                myNumber = nNewAmt;
            }
            var s = string.Format("{0:0.00}", myNumber);
            return s;
        }

        public static string GetTableBeginning(string sTableName)
        {
            string css = "<style> html {    font-size: 1em;    color: black;    font-family: verdana }  .r1 { font-family: verdana; font-size: 10; }</style>";
            string logo = "https://www.biblepay.org/wp-content/uploads/2018/04/Biblepay70x282_96px_color_trans_bkgnd.png";
            string sLogoInsert = "<img width=300 height=100 src='" + logo + "'>";
            string HTML = "<HTML>" + css + "<BODY><div><div style='margin-left:12px'><TABLE class=r1><TR><TD width=70%>" + sLogoInsert
                + "<td width=25% align=center>" + sTableName + "</td><td width=5%>" + DateTime.Now.ToShortDateString() + "</td></tr>";
            HTML += "<TR><TD><td></tr>" + "<TR><TD><td></tr>" + "<TR><TD><td></tr>";
            HTML += "</table>";
            return HTML;
        }
        public static string GetCharityTableHTML(MySqlCommand c, int iMaxRows)
        {
            string css = "<style> html {    font-size: 4px;    color: black;    font-family: verdana }  .r1 { font-family: verdana; font-size: 4px; }</style>";
            string logo = "https://www.biblepay.org/wp-content/uploads/2018/04/Biblepay70x282_96px_color_trans_bkgnd.png";
            string sLogoInsert = "<img width=300 height=100 src='" + logo + "'>";
            string HTML = "<HTML>" + css + "<BODY><div><div style='margin-left:12px'><TABLE class=r1><TR><TD width=95%>" + sLogoInsert
                + "<td width=5% align=right>Accountability</td><td>" + DateTime.Now.ToShortDateString() + "</td></tr>";

            HTML += "<TR><TD><td></tr>" + "<TR><TD><td></tr>" + "<TR><TD><td></tr>";
            HTML += "</table>";

            string header = "<TR><Th>Date<Th>Amount<th>Charity<th>Child Name<th>Child ID<th>Balance<Th width=30%>Notes</tr>";
            HTML += "<table width=100%>" + header + "<tr><td colspan=7 width=100%><hr></tr>";
            double nDR = 0;
            double nCR = 0;
            double nTotal = 0;
            DataTable dt;
            try
            {
                dt = BMSCommon.Database.GetDataTable(c);
            }
            catch (Exception)
            {
                return "";
            }
            string sCharity = "";
            string sOldDate = "";
            double nBalance = 0;
            int iStart = 0;
            if (dt.Rows.Count > iMaxRows && iMaxRows > 0)
            {
                iStart = dt.Rows.Count - iMaxRows;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double dAmt = BMSCommon.Common.GetDouble(dt.Rows[i]["Amount"]);
                string sType = dAmt >= 0 ? "DR" : "CR";
                nTotal += BMSCommon.Common.GetDouble(dt.Rows[i]["Balance"]);
                string dt1 = Convert.ToDateTime(dt.Rows[i]["Added"]).ToShortDateString();
                sCharity = dt.Rows[i]["Charity"].ToString();
                string row = "<tr><td align=right>" + dt1 + "<td align=right>" + DoFormat(BMSCommon.Common.GetDouble(dt.Rows[i]["Amount"])) + "<td align=right>" + dt.Rows[i]["Charity"].ToString()
                    + "<td align=right>" + dt.Rows[i]["Name"] + "<td align=right>" + dt.Rows[i]["ChildID"] + "<td align=right>" + DoFormat(BMSCommon.Common.GetDouble(dt.Rows[i]["Balance"]))
                    + "<td align=right><small>" + dt.Rows[i]["Notes"].ToString() + "</small></tr>";

                // Add the totals
                if (dAmt > 0)
                {
                    nDR += dAmt;
                }
                else
                {
                    nCR += dAmt;
                }
                nBalance += dAmt;

                if (sOldDate != dt1 && i > 1 && (i >= iStart || iMaxRows==0))
                {
                    HTML += "<tr><td colspan=10><hr></td></tr>";
                }
                sOldDate = dt1;

                if (i >= iStart || iMaxRows==0)
                {
                    HTML += row;
                }
            }
            // sql = "update sponsoredOrphan set balance = (Select top 1 Balance from OrphanExpense where SponsoredOrphan.childid=orphanexpense.childid order by added desc)\r\n"
            //    + "Select sum(Amount) balance from OrphanExpense where charity='" + sCharity + "' and childid in (select childid from SponsoredOrphan)";
            //double nAmt = gData.GetScalarDouble(sql, "balance");


            HTML += "<tr><td>&nbsp;</td></tr>";
            HTML += "<tr><td>BALANCE:<td><td>" + DoFormat(nBalance) + "</tr>";
            HTML += "</body></html>";
            return HTML;
        }

        public static string GetTableHTML(string sReportName, DataTable dt, string sCols, string sTotalCol, bool fShowCreditAndDebit = false)
        {
            StringBuilder HTML = new StringBuilder();

            try
            {
                HTML.Append(GetTableBeginning(sReportName));

                string[] vCols = sCols.Split(new string[] { ";" }, StringSplitOptions.None);
                string sHeader = "<tr>";
                for (int i = 0; i < vCols.Length; i++)
                {
                    sHeader += "<th>" + vCols[i] + "</th>";
                }
                sHeader += "</tr>";

                HTML.Append("<table width=100%>" + sHeader + "<tr><td colspan=5 width=100%><hr></tr>");

                double nTotal = 0;
                double nTotalDr = 0;
                double nTotalCr = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string sRow = "<tr>";

                    for (int j = 0; j < vCols.Length; j++)
                    {
                        string sValueControl = dt.Rows[i][vCols[j]].ToString();
                        if (vCols[j].ToLower() == "time")
                        {
                            sValueControl = BMSCommon.Common.FromUnixTimeStamp((int)BMSCommon.Common.GetDouble(dt.Rows[i]["time"])).ToShortDateString();
                        }
                        if (sValueControl.Length > 255)
                            sValueControl = sValueControl.Substring(0, 254);
                        
                        sRow += "<td align=right>" + sValueControl + "</td>";
                    }
                    sRow += "</tr>";
                    string sType = dt.Rows[i]["Type"].ToString();

                    double nAmt = BMSCommon.Common.GetDouble(dt.Rows[i][sTotalCol]);

                    if (sType == "CR")
                    {
                        nAmt = nAmt * -1;
                    }
                    if (sTotalCol != "")
                    {
                        nTotal += nAmt;
                        if (nAmt > 0)
                        {
                            nTotalDr += nAmt;
                        }
                        else
                        {
                            nTotalCr += nAmt;
                        }
                    }
                    HTML.Append(sRow);
                }

                HTML.Append("<tr><td>&nbsp;</td></tr>");

                if (fShowCreditAndDebit)
                {
                    HTML.Append("<tr><td>Total Expenses: <td><td>" + nTotalDr.ToString() + "</tr>");
                    HTML.Append("<tr><td>Total Revenue: <td><td>" + nTotalCr.ToString() + "</tr>");
                }

                if (!fShowCreditAndDebit && sTotalCol != "")
                {
                    HTML.Append("<tr><td>TOTAL:<td><td>" + nTotal.ToString() + "</tr>");
                }

                HTML.Append("</body></html>");
                return HTML.ToString();
            }
            catch (Exception ex)
            {
                BMSCommon.Common.Log(ex.Message);
            }

            return HTML.ToString();
        }

    }
}