using BiblePay.BMS;
using BiblePay.BMS.DSQL;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using static BiblePay.BMS.Common;

namespace BMS
{
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
}
