using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BMSCommon
{
        public static class HalfordMemoryCache
        {
            // This is an in memory only app-cache
            public struct AppEntry
            {
                public string Key;
                public object Value;
                public System.DateTime Expiration;
            }
            public static Dictionary<string, AppEntry> dictAppCache = new Dictionary<string, AppEntry>();
            public static void Write(string sKey, object oValue, int nExpirationSeconds)
            {
                AppEntry e = new AppEntry();
                e.Key = sKey;
                e.Value = oValue;
                e.Expiration = System.DateTime.Now.AddSeconds(nExpirationSeconds);
                dictAppCache[e.Key] = e;
            }
            public static object Read(string sKey)
            {
                try
                {
                    AppEntry e = new AppEntry();
                    bool f = dictAppCache.TryGetValue(sKey, out e);
                    if (System.DateTime.Now > e.Expiration)
                        e.Value = null;
                    return e.Value;
                }
                catch (Exception ex)
                {
                    Common.Log("Unable to readhalfordcache::" + ex.Message);
                    return string.Empty;
                }
            }
        }





    public static class HalfordDatabase2
    {

        public static void WriteKV(string sKey, string sData)
        {
            WriteToPosition(sKey, sData);
        }
        public static string ReadKV(string sKey)
        {
            string sData = ReadFrom(sKey);
            return sData;
        }
        private static void WriteToPosition(string sKey, string sData)
        {
            sKey = sKey.PadRight(64);
            sData = sData.PadRight(64);


            if (sKey.Length > 64)
                sKey = sKey.Substring(0, 63);

            string hash = Common.GetSha256String(sKey);
            int iDecHash  = Convert.ToInt32(hash.Substring(0,4), 16);
            int iPos = iDecHash * 128;
            if (sData.Length > 64)
                sData = sData.Substring(0, 63);

            string dbFile = Common.GetFolder("Database") + Common.GetPathDelimiter() + "halfdb.xdat";
            var fileStream = new FileStream(dbFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            var bw = new BinaryWriter(fileStream);
            byte[] binKey = Encoding.UTF8.GetBytes(sKey);
            byte[] binData = Encoding.UTF8.GetBytes(sData);
            bw.Seek(iPos, SeekOrigin.Begin);
            bw.Write(binKey);
            bw.Seek(iPos + 64, SeekOrigin.Begin);
            bw.Write(binData);
            bw.Close();
            fileStream.Close();
        }

        private static string ReadFrom(string sKey)
        {
            if (sKey.Length > 64)
                sKey = sKey.Substring(0, 63);

            string hash = Common.GetSha256String(sKey);
            int iDecHash = Convert.ToInt32(hash.Substring(0, 4), 16);
            int iPos = iDecHash * 128;
            string dbFile = Common.GetFolder("Database") + Common.GetPathDelimiter() + "halfdb.xdat";
            var fileStream = new FileStream(dbFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var br = new BinaryReader(fileStream);
            br.BaseStream.Seek(iPos, SeekOrigin.Begin);
            byte[] bKey=             br.ReadBytes(64);
            br.BaseStream.Seek(iPos + 64, SeekOrigin.Begin);
            byte[] bVal = br.ReadBytes(64);
            string strVal = Encoding.UTF8.GetString(bVal);
            string strKey = Encoding.UTF8.GetString(bKey);
            br.Close();
            fileStream.Close();

            if (strKey == sKey)
                return strVal;

            return "";


        }


    }












        public class HalfordDatabase : Attribute
        {

            private static int DB_ENTROPY = 2;

            // This is a disk based KV Pair database

            [HalfordDatabase]
            private static void RemoveOldIndex(HalfordFileIndex ind)
            {
                string sIndex = ind.filename;
                var fs = new FileStream(sIndex, FileMode.Open, FileAccess.ReadWrite);
                string sData = "-1".PadRight(32);
                byte[] b = Encoding.ASCII.GetBytes(sData);
                fs.Position = ind.start;
                fs.Write(b, 0, b.Length);
                fs.Close();
            }


            // The disk based application cache calls "SetKV" and "GetKV"
            public static void SetKV(string sData, string sKey)
            {
                AppendData(sData, sKey);
            }
            public static string GetKV(string sKey)
            {
                return RetrieveKV("", sKey);
            }

            public static void SetKV(string sData, string sKey, int nSeconds)
            {
                double nExpiration = Common.UnixTimestamp() + nSeconds;
                string sNewData = sData + "<zcolumn>" + nExpiration.ToString();
                AppendData(sNewData, sKey);
            }


            public static double GetKVDoubleWithExpiration(string sKey)
            {
                double n1 = Common.GetDouble(HalfordDatabase.GetKVWithExpiration(sKey));
                return n1;
            }

            public static void SetKVDoubleWithExpiration(string sKey, double nValue, double nExpirationSeconds)
            {
                HalfordDatabase.SetKV(nValue.ToString(), sKey, (int)nExpirationSeconds);
            }


            public static string GetKVWithExpiration(string sKey)
            {

                string sData = RetrieveKV("", sKey);
                if (sData == null || sData == "")
                    return "";

                if (sData.Contains("<zcolumn>"))
                {
                    string[] vData = sData.Split(new string[] { "<zcolumn>" }, StringSplitOptions.None);

                    double nTimestamp = Common.GetDouble(vData[1]);
                    if (Common.UnixTimestamp() > nTimestamp)
                    {
                        return "";
                    }
                    return vData[0];
                }
                return sData;
            }


            public static void AppendData(string sData, string sKey)
            {
                string hash = Common.GetSha256String(sKey);
                string dbFile = Common.GetFolder("Database") + Common.GetPathDelimiter() + hash.Substring(0, DB_ENTROPY) + ".xdat";
                string dbIndex = Common.GetFolder("Database") + Common.GetPathDelimiter() + hash.Substring(0, DB_ENTROPY) + ".xindex";
                if (!Directory.Exists(Common.GetFolder("Database")))
                {
                    Directory.CreateDirectory(Common.GetFolder("Database"));
                }
                long nStart = 0;
                if (System.IO.File.Exists(dbFile))
                {
                    FileInfo fi = new FileInfo(dbFile);
                    nStart = fi.Length;
                }
                string index = hash + "|" + dbFile + "|" + nStart.ToString() + "|" + sData.Length.ToString() + "|||||<MYEOF1>\r\n".PadRight(100);
                HalfordFileIndex OldIndex = GetIndexOfIndex(hash);
                if (OldIndex.datalength > 0)
                {
                    // This old item exists... remove the key, then add the new key....
                    RemoveOldIndex(OldIndex);
                }
                byte[] b = Encoding.ASCII.GetBytes(sData);
                var fileStream = new FileStream(dbFile, FileMode.Append, FileAccess.Write, FileShare.None);
                var bw = new BinaryWriter(fileStream);
                bw.Write(b);
                bw.Write("\r\n<MY_EOF_MONIKER>\r\n");
                bw.Close();
                fileStream.Close();
                // Write index
                System.IO.StreamWriter sw = new System.IO.StreamWriter(dbIndex, true);
                sw.WriteLine(index);
                sw.Close();
            }


            public static string GetElement(string data, string delimiter, int n)
            {
                string[] vE = data.Split(new string[] { delimiter }, StringSplitOptions.None);

                if (vE.Length > n)
                {
                    return vE[n];
                }
                return "";
            }
            public struct HalfordFileIndex
            {
                public string hash;
                public int start;
                public string filename;
                public int datalength;
            };

            private static HalfordFileIndex GetIndexOfIndex(string sHash)
            {
                string dbIndex = Common.GetFolder("Database") + Common.GetPathDelimiter() + sHash.Substring(0, DB_ENTROPY) + ".xindex";
                if (!System.IO.File.Exists(dbIndex))
                {
                    return new HalfordFileIndex();
                }
                System.IO.StreamReader file = new System.IO.StreamReader(dbIndex);
                string sLine = "";
                int iPos = 0;
                while ((sLine = file.ReadLine()) != null)
                {
                    HalfordFileIndex i = new HalfordFileIndex();
                    i.hash = GetElement(sLine, "|", 0);
                    i.filename = GetElement(sLine, "|", 1).Replace(".xdat", ".xindex");
                    i.start = iPos;
                    i.datalength = (int)Common.GetDouble(GetElement(sLine, "|", 3));
                    if (i.hash == sHash)
                    {
                        file.Close();
                        return i;
                    }
                    iPos += sLine.Length + 2;
                }
                file.Close();
                return new HalfordFileIndex();
            }

            private static HalfordFileIndex GetIndex(string sHash)
            {
                string dbIndex = Common.GetFolder("Database") + Common.GetPathDelimiter() + sHash.Substring(0, DB_ENTROPY) + ".xindex";
                if (!System.IO.File.Exists(dbIndex))
                {
                    return new HalfordFileIndex();
                }
                System.IO.StreamReader file = new System.IO.StreamReader(dbIndex);
                string sLine = "";
                while ((sLine = file.ReadLine()) != null)
                {
                    if (sLine.Substring(0, 5) != "     ")
                    {
                        HalfordFileIndex i = new HalfordFileIndex();
                        i.hash = GetElement(sLine, "|", 0);
                        i.filename = GetElement(sLine, "|", 1);
                        i.start = (int)Common.GetDouble(GetElement(sLine, "|", 2));
                        i.datalength = (int)Common.GetDouble(GetElement(sLine, "|", 3));
                        if (i.hash == sHash)
                        {
                            file.Close();
                            return i;
                        }
                    }
                }
                file.Close();
                return new HalfordFileIndex();
            }

            public static string FetchDatabaseObjectBase(string suffix)
            {
                if (suffix == "" || suffix == null)
                {
                    return null;
                }
                string myRetrieve = RetrieveKV(Common.GetSha256String(suffix));
                return myRetrieve;

            }

            public static string RetrieveKV(string sHash, string sKey = "")
            {
                sHash = sHash.Replace(".xdat", "");
                if (sKey != "")
                {
                    sHash = Common.GetSha256String(sKey);
                }
                HalfordFileIndex i = GetIndex(sHash);
                if (i.datalength > 0)
                {
                    string dbFile = Common.GetFolder("Database") + Common.GetPathDelimiter() + sHash.Substring(0, DB_ENTROPY) + ".xdat";
                    var fs = new FileStream(dbFile, FileMode.Open);

                    fs.Seek(i.start, SeekOrigin.Begin);
                    byte[] bits = new byte[i.datalength];
                    fs.Read(bits, 0, i.datalength);
                    fs.Close();
                    string result = System.Text.Encoding.UTF8.GetString(bits);
                    return result;
                }
                return null;
            }
        }

    }


