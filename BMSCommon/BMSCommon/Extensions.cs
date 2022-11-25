using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BMSCommon
{




    public static class DataTableExtensions
    {
        public static int ToInt(this DataRow row, int index)
        {
                return Convert.ToInt32(row[index].ToString());
        }

        public static int ToInt(this DataRow row, string index)
        {
                return Convert.ToInt32(row[index].ToString());
        }
        public static TimeSpan Elapsed(this DateTime datetime)
        {
            return DateTime.Now - datetime;
        }

        public static DataTable FilterDataTable(this DataTable table, string sql)
        {
            try
            {
                DataRow[] dr1 = table.Select(sql);
                DataTable dtNew = new DataTable();
                if (dr1.Length > 0)
                {
                    dtNew = table.Clone();

                    foreach (DataRow temp in dr1)
                    {
                        dtNew.ImportRow(temp);
                    }
                }
                return dtNew;
            }
            catch (Exception ex)
            {
                DataTable dt1 = new DataTable();
                return dt1;
            }
        }

        public static DataTable SortDataTable(this DataTable table, string sql)
        {
            try
            {
                table.DefaultView.Sort = sql;
                table.DefaultView.ApplyDefaultSort = true;
                return table;
            }
            catch (Exception ex)
            {
                return table;
            }

        }

        public static DataTable FilterAndSort(this DataTable table, string sFilter, string sSort)
        {
            try
            {
                DataRow[] dr1 = table.Select(sFilter, sSort);
                DataTable dtNew = new DataTable();
                if (dr1.Length > 0)
                {
                    dtNew = table.Clone();

                    foreach (DataRow temp in dr1)
                    {
                        dtNew.ImportRow(temp);
                    }
                }
                return dtNew;
            }
            catch (Exception)
            {
                DataTable dt1 = new DataTable();
                return dt1;
            }
        }


        public static string GetColValue(this DataTable table, string colName)
        {
            if (table.Rows.Count < 1)
                return String.Empty;
            if (!table.Columns.Contains(colName))
                return String.Empty;
            return table.Rows[0][colName].ToString();
        }

        public static double GetColDouble(this DataTable table, string colName)
        {
            return BMSCommon.Common.GetDouble(table.Rows[0][colName].ToString());
        }

        public static int GetColInt(this DataTable table, string colName)
        {
            return (int)BMSCommon.Common.GetDouble(table.Rows[0][colName].ToString());
        }

        public static string GetColValue(this DataTable table, int iRow, string colName)
        {
            if (!table.Columns.Contains(colName))
            {
                return "";
            }
            return table.Rows[iRow][colName].ToString();
        }

        public static double GetColDouble(this DataRowView dr, string colName)
        {
            if (dr == null)
                return 0;


            double nOut = BMSCommon.Common.GetDouble(dr[colName].ToString());
            return nOut;
        }


        public static double GetColDouble(this DataTable table, int iRow, string colName)
        {
            if (table.Rows.Count == 0)
                return 0;

            double nOut = BMSCommon.Common.GetDouble(table.Rows[iRow][colName].ToString());
            return nOut;
        }
        public static DateTime GetColDateTime(this DataTable table, int iRow, string sColName)
        {
            DateTime dt = new DateTime();

            if (table.Rows.Count == 0)
                return dt;

            double nOut = BMSCommon.Common.GetDouble(table.Rows[iRow][sColName].ToString());
            dt = BMSCommon.Common.FromUnixTimeStamp((int)nOut);
            return dt;
        }

    }



    public static class MyObjectExtensions
    {
        public static Int32 AsInt32(this object obj)
        {
            return Convert.ToInt32(obj);
        }
        public static double AsDouble(this object obj)
        {
            double d = Common.GetDouble(obj);
            return d;
        }

        public static string ToShortDateString(this object obj)
        {
            if (obj==null || obj.ToString()==String.Empty)
            {
                return String.Empty;
            }
            DateTime dt = Convert.ToDateTime(obj);
            return dt.ToShortDateString();
        }
        public static string Percentage2(this object obj)
        {
            double n = Math.Round(AsDouble(obj) * 100, 2);
            string s = n.ToString() + "%";
            return s;
        }

      
       
    
        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static Guid ToGuid(this object o)
        {
            return o.ToGuid();
        }

        public static Guid ToGuid(this String str)
        {
            return str.ToGuid();
        }

        public static DateTime ToDate(this String str)
        {
            if (str == String.Empty) return Convert.ToDateTime("1-1-1900");
            try
            {
                return Convert.ToDateTime(str);

            }
            catch (Exception)
            {
                return Convert.ToDateTime("1-1-1900");
            }
        }

        public static String ToStr(this object o)
        {
            if (o == null) return String.Empty;
            return o.ToString();
        }

        public static DateTime ToDateTime(object o)
        {
            if (o == null || ("" + o.ToString() == String.Empty)) return Convert.ToDateTime("1-1-1900");
            return Convert.ToDateTime(o);
        }

        public static Int32 Val(object sInput)
        {
            if (sInput == null || sInput == DBNull.Value) return 0;
            return Convert.ToInt32(sInput);
        }
    }

        public static class StringExtension
        {
            public static bool IsNullOrEmpty(this string str)
            {
                if (str == null || str == String.Empty)
                    return true;
                return false;
            }

            public static string TrimAndReduce(this string str)
            {
                return str.Trim();
            }

            public static string ToNonNullString(this object o)
            {
                if (o == null)
                    return String.Empty;
                return o.ToString();
            }

            public static string[] Split(this string str, string sDelimiter)
            {
                string[] vSplitData = str.Split(new string[] { sDelimiter }, StringSplitOptions.None);
                return vSplitData;
            }

        public static string ToMilitaryTime(this string o)
        {
            return ObjToMilitaryTime(o);
        }
            public static string ObjToMilitaryTime(this object o)
            {
                if (o==null)
                {
                  return String.Empty;
                }
                DateTime dt = Convert.ToDateTime(o);
                string sMyMilitary = dt.ToString("MM-dd-yy HH:mm");
                return sMyMilitary;
            }

            public static double ToDouble(this string o)
            {
                try
                {
                    if (o == null)
                        return 0;
                    if (o.ToString() == string.Empty)
                        return 0;
                    double d = Convert.ToDouble(o.ToString());
                    return d;
                }
                catch (Exception)
                {
                    return 0;
                }
            }

        }

}
