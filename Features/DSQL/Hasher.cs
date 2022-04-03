using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BiblePay.BMS.Common;
using System.Text;
using System.IO;

namespace BiblePay.BMS.DSQL
{

    public class HashBuilder
    {
        private StringBuilder _hashes = new StringBuilder();
        private const string sHashDelimiter = "=";
        private const string sColDelimiter = ",";
        public const string sEmptyHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
        public string ToStr()
        {
            return _hashes.ToString();
        }

    }
    public struct BlockIndex
    {
        public string Hash;
        public int TransactionCount;
        public string BlockKey;
        public int Port;
    }

    public struct NodeHash
    {
        public string IP;
        public string ObjectHash;
    }

    public struct HalfordReplicationStat
    {
        public int Seen;
    }

    public class Hasher
    {
        public static string GetBlockKey(string sPath, int iPort)
        {
            DirectoryInfo di = new DirectoryInfo(sPath);
            string sKey = iPort.ToString() + di.Name;
            return sKey;
        }

        public static double GetNodeVersion(string sIP, int iTimeout)
        {
            string sURL = "https://" + sIP + ":5000/BMS/Version";
            string sResponse = ExecuteMVCCommand(sURL, iTimeout);
            double dResponse = GetDouble(sResponse);
            return dResponse;
        }

        public static string GetHash(string sJson)
        {
            string sHash = GetSha256String(sJson);
            return sHash;
        }

    }
}
