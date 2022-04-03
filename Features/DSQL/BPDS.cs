using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblePay.BMS.DSQL
{
    // BiblePay Persisted Data Storage System
    // We overloaded LiteDB (www.litedb.org) - Credit to Mauricio David for LiteDb
    public class KeyValuePair
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public double dValue { get; set; }
    }

    public class BPDS
    {
        public static string DBASE_FILE_LOC = BiblePay.BMS.Common.GetFolder("sql", "pds.db");

        public static double ReadDouble(string sType, string sKey)
        {
            object oResult = Read(sType, sKey);
            return Common.GetDouble(oResult);
        }

        public static void WriteDouble(string sType, string sKey, double dValue)
        {
            Write(sType, sKey, dValue.ToString());
        }

        private static LiteDatabase LDB = new LiteDatabase(DBASE_FILE_LOC);

        public static object Read(string sType, string sKey)
        {
             var myKeys = LDB.GetCollection<KeyValuePair>("kv");
             var results = myKeys.Find(x => x.Type.Equals(sType) && x.Key.Equals(sKey));
             var kvp = new KeyValuePair();
             if (results.Count() > 0)
             {
                    kvp = results.ElementAtOrDefault(0);
                    kvp.dValue++;
                    myKeys.Update(kvp);
                    return kvp.Value;
             }
            return String.Empty;
        }

        public static void Write(string sType, string sKey, object oValue)
        {
                var myKeys = LDB.GetCollection<KeyValuePair>("kv");
                var results = myKeys.Find(x => x.Type.Equals(sType) && x.Key.Equals(sKey));
                var kvp = new KeyValuePair();
                if (results.Count() == 0)
                {
                    kvp = new KeyValuePair()
                    {
                        Key = sKey,
                        Type = sType,
                        Value = oValue
                    };
                    myKeys.Insert(kvp);
                }
                else
                {
                    kvp = results.ElementAtOrDefault(0);
                    kvp.dValue++;
                    kvp.Value = oValue;
                    myKeys.Update(kvp);
                }
        }
    }
}
