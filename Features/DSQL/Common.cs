//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using BiblePay.BMS.CustomExtensions;
using System.Security.Cryptography.X509Certificates;
using BiblePay.BMS.DSQL;
//using NBitcoin.DataEncoders;
//using NBitcoin;
using BiblePay.BMS;

namespace BiblePay.BMS
{
    public static class Common
    {
        public static int BMS_VERSION = 1025;
        public static int MIN_BMS_VERSION = 1001;
        public static int DEFAULT_PORT = 5000;
        public static bool fDebug = false;
        public static string BX_API = "https://chainz.cryptoid.info/bbp/api.dws?";

        public struct Node
        {
            public string hostname;
            public int port;
            public string URI;
            public string UrlPrefix;
        }

        public static string GE(string sData, int iCol)
        {
            string[] vGE = sData.Split("<COL>");
            if (vGE.Length > iCol)
                return vGE[iCol];
            return string.Empty;
        }

        public static string GV(string sData, int iCol)
        {
            string[] vGE = sData.Split("<VAL>");
            if (vGE.Length > iCol)
                return vGE[iCol];
            return string.Empty;
        }

        /*
        public static bool VerifySignature(string BBPAddress, string sMessage, string sSig)
        {
            BitcoinPubKeyAddress bob = new BitcoinPubKeyAddress(BBPAddress, Network.BiblepayMain);
            bool b1 = bob.VerifyMessage(sMessage, sSig);
            return b1;
        }
        */


        public static string ExtractXML(string sData, string sStartKey, string sEndKey)
        {
            int iPos1 = (sData.IndexOf(sStartKey, 0) + 1);
            if (iPos1 < 1)
                return string.Empty;

            iPos1 = (iPos1 + sStartKey.Length);
            int iPos2 = (sData.IndexOf(sEndKey, (iPos1 - 1)) + 1);
            if ((iPos2 == 0))
            {
                return String.Empty;
            }
            string sOut = sData.Substring((iPos1 - 1), (iPos2 - iPos1));
            return sOut;
        }

        public static void RenameFile(string sOldPath, string sNewPath)
        {
            // The System.IO.File.Move method fails in various circumstances (destination exists, network copies, destination exists in camelcase, etc), so we make our own
            if (sOldPath == sNewPath)
                return;
            string sTempPath = GetFolder("temp");
            string sFN = Guid.NewGuid().ToString();
            System.IO.File.Copy(sOldPath, sTempPath + GetPathDelimiter() + sFN);
            System.IO.File.Copy(sTempPath + GetPathDelimiter() + sFN, sNewPath);
            System.IO.File.Delete(sOldPath);
            System.IO.File.Delete(sTempPath + GetPathDelimiter() + sFN);
        }

        public static string ByteArrayToHexString(byte[] arrInput)
        {
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (int i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString().ToLower();
        }

        public static string GetSha256String(string sData)
        {
            byte[] arrData = System.Text.Encoding.UTF8.GetBytes(sData);
            var hash = System.Security.Cryptography.SHA256.Create().ComputeHash(arrData);
            return ByteArrayToHexString(hash);
        }

        public static string GetShaOfBytes(byte[] bytes)
        {
            var hash = System.Security.Cryptography.SHA256.Create().ComputeHash(bytes);
            return ByteArrayToHexString(hash);
        }
        public static string GetShaOfFile(string sFile)
        {
            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(sFile);
                string sHash = GetShaOfBytes(fileBytes);
                return sHash;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static DateTime FromUnixTimeStamp(int Timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Timestamp).ToUniversalTime();
        }

        public static void WriteToFile(string path, string data, int TimeStamp)
        {
            File.WriteAllText(path, data);
            File.SetLastWriteTimeUtc(path, FromUnixTimeStamp(TimeStamp));
        }

        public static string HashOfList(List<string> l)
        {
            l.Sort();
            var result = string.Join("", l.ToArray());
            string Hash = GetSha256String(result);
            return Hash;
        }
        public struct FolderHash
        {
            public int Timestamp;
            public int Count;
            public string Hash;
        }

        public static double GetDouble(object o)
        {
            try
            {
                if (o == null) return 0;
                if (o.ToString() == "") return 0;
                double d = Convert.ToDouble(o.ToString());
                return d;
            }
            catch (Exception)
            {
                // Letters?
                return 0;
            }
        }

        public struct HalfordFileInfo
        {
            public string FileName;
            public int FileSize;
            public string FullPath;
            public string LocalPath;
            public string Hash;
        }
        
        public static string GetConfigurationKeyValue(string _Key)
        {
            string sPath = GetFolder("") + "bms.conf";
            string sData = System.IO.File.ReadAllText(sPath);
            string[] vData = sData.Split("\n");
            for (int i = 0; i < vData.Length; i++)
            {
                string sEntry = vData[i];
                sEntry = sEntry.Replace("\r", "");
                string[] vRow = sEntry.Split("=");
                if(vRow.Length >= 2)
                {
                    string sKey = vRow[0];
                    string sValue = vRow[1];
                    if (sKey == _Key)
                        return sValue;
                }

            }
            return string.Empty;
        }

        public static int UnixTimestamp(DateTime dt)
        {
            DateTime dt2 = Convert.ToDateTime(new DateTime(dt.Year, dt.Month, dt.Day, 12, 0, 0));
            Int32 unixTimestamp = (Int32)(dt2.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public static int UnixTimestamp()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public static int GetFolderTimeStamp(int iTimeStamp)
        {
            DateTime startDate = new DateTime(2019, 1, 1);
            int iBase = UnixTimestamp(startDate);
            int iOffset = (iTimeStamp - iBase) % 86400;
            int iActual = iTimeStamp - iOffset;
            return iActual;
        }

        public static string GetUpgradeFileHashes(string sDirectory)
        {
            StringBuilder sHashes = new StringBuilder();

            if (!Directory.Exists(sDirectory))
            {
                Log("No such dir " + sDirectory);
                return "";
            }

            Log("Gathering...");
            try
            {
                string[] Files = Directory.GetFiles(sDirectory);
                List<string> hashes = new List<string>();

                foreach (string File1 in Files)
                {
                    
                    // For each timestamped directory in the folder list, see if they exist in other servers.
                    string fileHash = GetShaOfFile(File1);
                    FileInfo fi1 = new FileInfo(Path.Combine(File1));
                    bool bReplicate = true;
                    if (fi1.Extension == ".pdb" || fi1.Extension == ".manifest" || fi1.Extension == ".xml" || fi1.Extension == ".json" || fi1.Extension == "" || fi1.Extension == null || fi1.Extension == "." || fi1.Extension==".pfx")
                        bReplicate = false;
                    if (fi1.Extension != ".dll")
                        bReplicate = false;

                    if (fi1.Name == "xBiblePay.BMSD.dll")
                    {
                        bReplicate = false;
                    }
                    if (bReplicate)
                    {
                        string sData = sDirectory + "|" + fi1.Name + "|" + fileHash + "|" + fi1.Length.ToString() + "<ROW>";
                        sHashes.Append(sData);
                    }
                }
                return sHashes.ToString();
            }catch(Exception ex)
            {
                Log("error " + ex.Message);
                return "";
            }
        }

        public static string GetCDN()
        {
            string sCDN = "https://sanc1.cdn.biblepay.org:5000";
            return sCDN;
        }
        public static bool IsWindows()
        {
            bool fUnix = Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX;
            return !fUnix;
        }

        public static string NormalizeFilePath(string sPath)
        {
            if (IsWindows())
            {
                sPath = sPath.Replace("/", "\\");
                sPath = sPath.Replace("\\\\", "\\");
                return sPath;
            }
            else
            {
                sPath = sPath.Replace("\\", "/");
                sPath = sPath.Replace("//", "/");
                return sPath;
            }
        }
        public static string GetRootFolder()
        {
            bool fUnix = Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX;
            string sPath = fUnix ? "/" : "c:\\";
            //string sHomePath = ? Environment.GetEnvironmentVariable("HOME")                      : Environment.ExpandEnvironmentVariables("%APPDATA%");
            return sPath;
        }
        public static string GetPathDelimiter()
        {
            string sPathDelimiter = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) ? "/" : "\\";
            return sPathDelimiter;
        }
        public static string GetVideoFolder()
        {
            string sHomePath = GetRootFolder();
            string sPathDelimiter = GetPathDelimiter();
            sHomePath += sPathDelimiter + "inetpub" + sPathDelimiter + "wwwroot" + sPathDelimiter + "bms" + sPathDelimiter;
            return sHomePath;
        }

        public static string GetFolder(string sType)
        {
            string sHomePath = GetRootFolder();
            string sPathDelimiter = GetPathDelimiter();
            sHomePath += sPathDelimiter + "inetpub" + sPathDelimiter + "wwwroot" + sPathDelimiter + "bms" + sPathDelimiter;

            string s1 = sHomePath;
            if (sType != "")
                s1 += sPathDelimiter + sType;

            string sSqlPath = Path.Combine(s1);
            if (!Directory.Exists(sSqlPath))
                Directory.CreateDirectory(sSqlPath);
            return sSqlPath;
        }

        public static string GetFolder(string sType, string sFileName)
        {
            string sPathDelimiter = GetPathDelimiter();
            string sPath = GetFolder(sType) + sPathDelimiter + sFileName;
            if (IsWindows())
            {
                sPath = sPath.Replace("\\\\", "\\");
            }
            else
            {
                sPath = sPath.Replace("//", "/");
            }
            return sPath;
        }

        public static void Log(string sData)
        {
            try
            {
                string sPath = GetFolder("Log") + "dsql.log";
                System.IO.StreamWriter sw = new System.IO.StreamWriter(sPath, true);
                string Timestamp = DateTime.Now.ToString();
                sw.WriteLine(Timestamp + ": " + sData);
                sw.Close();
            }
            catch (Exception ex)
            {
                string sMsg = ex.Message;
            }
        }
        
        public static string GetFolder(string sType, int iTimestamp)
        {
            int iNewTimeStamp = Common.GetFolderTimeStamp(iTimestamp);
            string sPath = GetFolder(sType);
            string sFullPath = Path.Combine(sPath, iNewTimeStamp.ToString());
            if (!Directory.Exists(sFullPath)) Directory.CreateDirectory(sFullPath);
            return sFullPath;
        }


        public static string ExecuteMVCCommand(string URL, int iTimeout=30)
        {
            MyWebClient wc = new MyWebClient();
            try
            {
                wc.SetTimeout(iTimeout);
                string d = wc.DownloadString(URL);
                return d;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
   


    public class MyWebClient : System.Net.WebClient
    {
        private int DEFAULT_TIMEOUT = 30000;
        public void SetTimeout(int iTimeout)
        {
            DEFAULT_TIMEOUT = iTimeout * 1000;
        }
        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            System.Net.WebRequest w = base.GetWebRequest(uri);
            w.Timeout = DEFAULT_TIMEOUT;
            return w;
        }
    }

    public static class Global
    {
        public static int _retired = 0;
    }
    
}

// This namespace is used for the SSL certificate
namespace FluffySpoon
{
    public class X509
    {
        public static X509Certificate2 GetSSL()
        {
            try
            {
                string sPath = BiblePay.BMS.Common.GetFolder("database", "fluffy.bytes");
                string sURL = "https://social.biblepay.org/bms/fluffy.bytes";
                sURL = "https://bbpipfs.s3.filebase.com/database/fluffy.bytes";
                MyWebClient wc = new MyWebClient();
                wc.DownloadFile(sURL, sPath);
                byte[] b = System.IO.File.ReadAllBytes(sPath);
                var cert = new X509Certificate2(b);
                return cert;
            }
            catch (Exception ex)
            {
                Common.Log("GetSSL::" + ex.Message);
                return null;
            }
        }
    }
}
