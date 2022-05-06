using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BMSCommon
{
    public static class Common
    {
        public static int BMSCOMMON_VERSION = 1003;

        public static int DEFAULT_PORT = 8443;
        public static string GetCDN()
        {
            string sCDN = "https://globalcdn.biblepay.org:" + DEFAULT_PORT.ToString();
            return sCDN;
        }


        public static string xByteArrayToHexString(byte[] arrInput)
        {
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (int i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString().ToLower();
        }

        public static string xGetSha256String(string sData)
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

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }

        public static double GetRootFolderSize()
        {
            try
            {
                string sFolder = GetVideoFolder();
                DirectoryInfo d = new DirectoryInfo(sFolder);
                double nSz = DirSize(d);
                return nSz;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public static string ExecuteMVCCommand(string URL, int iTimeout = 30)
        {
            MyWebClient wc = new MyWebClient();
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

            try
            {
                wc.SetTimeout(iTimeout);
                string d = wc.DownloadString(URL);
                return d;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string NormalizeURL(string sURL)
        {
            sURL = sURL.Replace("https://", "{https}");
            sURL = sURL.Replace("///", "/");
            sURL = sURL.Replace("//", "/");
            sURL = sURL.Replace("\\", "/");
            sURL = sURL.Replace("{https}", "https://");
            return sURL;
        }

        public static string Mid(string data, int nStart, int nLength)
        {
            // Ported from VB6, except this version is 0 based (NOT 1 BASED)
            if (nStart > data.Length)
            {
                return String.Empty;
            }

            if (nStart < 0)
            {
                return String.Empty;
            }

            int nNewLength = nLength;
            int nEndPos = nLength + nStart;
            if (nEndPos > data.Length)
            {
                nNewLength = data.Length - nStart;
            }
            if (nNewLength < 1)
                return String.Empty;

            string sOut = data.Substring(nStart, nNewLength);
            if (sOut.Length > nLength)
            {
                sOut = sOut.Substring(0, nLength);
            }
            return sOut;
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
                if (vRow.Length >= 2)
                {
                    string sKey = vRow[0];
                    string sValue = vRow[1];
                    if (sKey == _Key)
                        return sValue;
                }

            }
            return string.Empty;
        }

        public static bool IsPrimary()
        {
            string sBindURL = GetConfigurationKeyValue("bindurl");
            bool f = sBindURL.Contains("sanc1") || sBindURL.Contains("209.145.56.214");
            return f;
        }
        public static string ChopLastOctetFromURL(string sData)
        {
            string sDelimiter = "/";
            string[] vData = sData.Split(sDelimiter);
            string sOut = "";
            for (int i = 0; i < vData.Length - 1; i++)
            {
                sOut += vData[i] + sDelimiter;
            }
            return sOut;
        }
        public static string ChopLastOctetFromPath(string sData)
        {
            string sDelimiter = IsWindows() ? "\\" : "/";
            string[] vData = sData.Split(sDelimiter);
            string sOut = "";
            for (int i = 0; i < vData.Length - 1; i++)
            {
                sOut += vData[i] + sDelimiter;
            }
            return sOut;
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


        public static string ByteArrayToHexString(byte[] arrInput)
        {
            StringBuilder sOutput = new StringBuilder(arrInput.Length);

            for (int i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString().ToLower();
        }

        public static int DateToUnixTimestamp(DateTime dt)
        {
            DateTime dt2 = Convert.ToDateTime(new DateTime(dt.Year, dt.Month, dt.Day, 12, 0, 0));
            Int32 unixTimestamp = (Int32)(dt2.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }


        public static int GetFolderTimeStamp(int iTimeStamp)
        {
            DateTime startDate = new DateTime(2019, 1, 1);
            int iBase = DateToUnixTimestamp(startDate);
            int iOffset = (iTimeStamp - iBase) % 86400;
            int iActual = iTimeStamp - iOffset;
            return iActual;
        }
        public static string GetSha256String(string sData)
        {
            byte[] arrData = System.Text.Encoding.UTF8.GetBytes(sData);
            var hash = System.Security.Cryptography.SHA256.Create().ComputeHash(arrData);
            return ByteArrayToHexString(hash);
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
                // Someone probably entered letters here
                return 0;
            }
        }   


        public static string GetFolder(string sType, string sFileName)
        {
            string sPathDelimiter = GetPathDelimiter();
            string sPath = GetFolder(sType) + sPathDelimiter + sFileName;
            return sPath;
        }
        public static string GetHLS(string sDestinationDir)
        {
            string sHLS = "";
            string[] filePaths = Directory.GetFiles(sDestinationDir);

            for (int j = 0; j < filePaths.Length; j++)
            {
                FileInfo fi = new FileInfo(filePaths[j]);
                //string sObjName = sFileName + "/" + fi.Name;
                sHLS += fi.Name + ",";
            }
            return sHLS;
        }


        public static double GetFreeDiskSpacePercentage()
        {
            try
            {
                string p1 = System.IO.Directory.GetCurrentDirectory();
                string p2 = System.IO.Path.GetPathRoot(p1);
                DriveInfo drive = new DriveInfo(p2);
                var totalBytes = drive.TotalSize;
                var freeBytes = drive.AvailableFreeSpace;
                var freePercent = (int)((100 * freeBytes) / totalBytes);
                return freePercent;
            }
            catch (Exception)
            {
                return 0;
            }
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

        public static int UnixTimestamp()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
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