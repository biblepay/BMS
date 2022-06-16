using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BMSCommon
{

    public static class MyObjectExtensions
    {
        public static Int32 ToInt32(this object obj)
        {
            return Convert.ToInt32(obj);
        }
    }


    public static class DataRowExtensions
    {
        public static int ToInt(this DataRow row, int index)
        {
            return Convert.ToInt32(row[index].ToString());
        }

        public static int ToInt(this DataRow row, string index)
        {
            return Convert.ToInt32(row[index].ToString());
        }
    }


    public static class MyExtensions
    {
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
