using System.Collections.Generic;
using static BMSCommon.BitcoinSyncModel;
using static BMSCommon.Model;

namespace BMSCommon
{
    public static class API
    {
        public static List<BMSNode> mNodes = new List<BMSNode>();
        public static Dictionary<string, BitcoinSyncTransaction> dMemoryPool = new Dictionary<string, BitcoinSyncTransaction>();
        public static List<string> mapTransactions = new List<string>();
        public static int DEFAULT_PORT = 8443;

        public static string GetCDN()
        {
            string sCDN = "https://globalcdn.biblepay.org:" + DEFAULT_PORT.ToString();
            return sCDN;
        }

        public static int GetNetworkHeight()
        {
            string sURL = GetCDN() + "/GetBestBlockHeight";
            string sData = Common.ExecuteMVCCommand(sURL, 5);
            double d = Common.GetDouble(sData);
            return (int)d;
        }

        public static int GetMemPoolCount()
        {
            int nCount = BMSCommon.API.dMemoryPool.Count;
            return nCount;
        }

    }
}
