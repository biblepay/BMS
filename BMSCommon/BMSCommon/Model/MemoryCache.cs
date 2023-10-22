using System;
using System.Collections.Generic;
using System.Text;

namespace BMSCommon
{
    public static class MemoryCache
    {
        private static Dictionary<string,string> keyValuePairs = new Dictionary<string,string>();
        private static Dictionary<string, int> KeyValueTimes = new Dictionary<string,int>();
        public static string GetKeyValue(string sKey, int nMaxSeconds)
        {
            string sOut = String.Empty;
            bool fExists = keyValuePairs.TryGetValue(sKey, out sOut);
            int nTS = 0;
            KeyValueTimes.TryGetValue(sKey, out nTS);
            int nElapsed = Common.UnixTimestamp() - nTS;
            if (nElapsed > nMaxSeconds)
            {
                return String.Empty;
            }
            return sOut;
        }
        public static void SetKeyValue(string sKey, string sValue)
        {
            keyValuePairs[sKey] = sValue;
            KeyValueTimes[sKey] = Common.UnixTimestamp();
        }
    }
}
