using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace BMSCommon
{
    public static class Common
    {

        public static void CreateDir(string sPath)
        {
            string sDestDirOnly = ChopLastOctetFromPath(sPath);
            if (!System.IO.Directory.Exists(sDestDirOnly))
            {
                System.IO.Directory.CreateDirectory(sDestDirOnly);
            }
        }

        public static bool IsValidEmailAddress(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; 
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
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
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
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
                return String.Empty;
            }
        }

        public static DateTime FromUnixTimeStamp(int Timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Timestamp).ToUniversalTime();
        }

        public static string ExtractXML(string sData, string sStartKey, string sEndKey)
        {
            var iPos1 = sData.IndexOf(sStartKey, 0) + 1;
            if (iPos1 < 1)
                return string.Empty;

            iPos1 += sStartKey.Length;
            var iPos2 = sData.IndexOf(sEndKey, iPos1 - 1) + 1;
            if (iPos2 == 0) return string.Empty;
            var sOut = sData.Substring(iPos1 - 1, iPos2 - iPos1);
            return sOut;
        }

        public static string GetBiblePayConfigFile(string sFileName)
        {
            string sFolder = GetBiblePayCoreConfigPath();
            string sPath = Path.Combine(sFolder, sFileName);
            return sPath;
        }
        // super mission critical::Embed bbp testharness version check to call out for currentversion from within the bbpapi, and if the code 
        // needs upgraded, and the user fails to upgrade we need to Exit from database and phone operations.

        public static string GetFolder(string sType)
        {
            string sHomePath = GetHomePath();
            string sPathDelimiter = GetPathDelimiter();
            sHomePath += sPathDelimiter;
            string s1 = sHomePath;
            if (sType != String.Empty)
                s1 += sPathDelimiter + sType;

            string sSqlPath = Path.Combine(s1);
            if (!Directory.Exists(sSqlPath))
                Directory.CreateDirectory(sSqlPath);
            return sSqlPath;
        }

        public static string DoubleToString(double nDouble, int nPlaces)
        {
            double nNewDouble = Math.Round(nDouble, nPlaces);
            string sData = ((decimal)nNewDouble).ToString();
            return sData;
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
        public static bool IsMac()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
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


        public static double GetRootFolderSize()
        {
            try
            {
                string sFolder = GetVideoFolder();
                DirectoryInfo d = new DirectoryInfo(sFolder);
                double nSz = DirSize(d);
                return nSz;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public static string GetVideoFolder()
        {
            string sHomePath = GetRootFolder();
            string sPathDelimiter = GetPathDelimiter();
            sHomePath += sPathDelimiter + "inetpub" + sPathDelimiter + "wwwroot" + sPathDelimiter + "bms" + sPathDelimiter;
            return sHomePath;
        }


        public static int UnixTimestamp()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
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
                sHLS += fi.Name + ",";
            }
            return sHLS;
        }


        public static BigInteger ToBigInteger(string value)
        {
            if (value == null)
                return 0;

            BigInteger result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                string v = value.Substring(i, 1);
                int iDec = Convert.ToInt32(v, 16);
                result = result * 16 + (iDec);
            }
            return result;
        }
        public static string GetConfigKeyValue(string _Key, string sPath="")
        {
            if (sPath == String.Empty)
            {
                sPath = GetFolder("") + "bms.conf";
            }

            if (!System.IO.File.Exists(sPath))
            {
                return "";
            }

            var sData = System.IO.File.ReadAllText(sPath);
            var vData = sData.Split("\n");
            for (int i = 0; i < vData.Length; i++)
            {
                string sEntry = vData[i];
                sEntry = sEntry.Replace("\r", "");
                string[] vRow = sEntry.Split("=");
                if (vRow.Length >= 2)
                {
                    string sKey = vRow[0];
                    string sValue = vRow[1];
                    if (sKey.ToLower() == _Key.ToLower())
                        return sValue;
                }

            }
            return string.Empty;
        }

        public static string GetRootFolder()
        {
            bool fUnix = Environment.OSVersion.Platform == PlatformID.Unix || 
                Environment.OSVersion.Platform == PlatformID.MacOSX;
            string sPath = fUnix ? "/" : "c:\\";
            return sPath;
        }
        public static string GetPathDelimiter()
        {
            string sPathDelimiter = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) ? "/" : "\\";
            return sPathDelimiter;
        }
        

        public static byte[] ConvertHtmlToBytes(string HTML, string PDFFileName)
        {
            string sGuid = Guid.NewGuid().ToString() + ".htm";
            string sDestFN = Path.Combine(Path.GetTempPath(), sGuid);
            System.IO.File.WriteAllText(sDestFN, HTML);
            byte[] b = System.IO.File.ReadAllBytes(sDestFN);
            return b;
        }

        public static double GetElementDouble(string sData, int iPos, string sDelimiter)
        {
            string s = GetElement(sData, iPos, sDelimiter);
            double nData = GetDouble(s);
            return nData;
        }
        public static string GetElement(string sData, int iPos, string sDelimiter)
        {
            string[] vData = sData.Split(sDelimiter);
            if (iPos > vData.Length - 1 || iPos < 0)
            {
                return "";
            }
            string myData = vData[iPos];
            return myData;
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
        public static long GetLocalFileSize(string sPath)
        {
            try
            {
                FileInfo fi = new FileInfo(sPath);
                return fi.Length;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string GetHomePath()
        {
            string homePathLinux = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            homePathLinux += "/biblepay";
            //string homePathWin = "c:\\inetpub\\wwwroot\\bms";
            string homePathWin = Environment.ExpandEnvironmentVariables("%APPDATA%") + "\\BiblePay\\BMS";
            if (!System.IO.Directory.Exists(homePathWin))
            {
                System.IO.Directory.CreateDirectory(homePathWin);
            }
            return IsWindows() ? homePathWin : homePathLinux;
        }

        public static string GetBiblePayCoreConfigPath()
        {
            string homePathLinux = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            homePathLinux += "/.biblepay";
            string homePathWin = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\biblepay";

            if (IsWindows())
            {
                return homePathWin;
            }
            else if (IsMac())
            {
                string homePathOSX = Environment.GetEnvironmentVariable("HOME") + "/Library/Application Support/Biblepay";
                return homePathOSX;
            }
            else
            {
                return homePathLinux;
            }
        }

        public static void Log(string sData)
        {
            try
            {
                string sPath = GetHomePath();
                if (!System.IO.Directory.Exists(sPath))
                    System.IO.Directory.CreateDirectory(sPath);

                string sPath2 = System.IO.Path.Combine(sPath, "bms.log");
                System.IO.StreamWriter sw = new System.IO.StreamWriter(sPath2, true);
                string timestamp = DateTime.Now.ToString();
                sw.WriteLine(timestamp + ": " + sData);
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public static double GetDouble(object o)
        {
            try
            {
                if (o == null) return 0;
                if (o.ToString() == String.Empty) return 0;
                double d = Convert.ToDouble(o.ToString());
                return d;
            }
            catch (Exception)
            {
                // Someone probably entered letters here
                return 0;
            }
        }



        private static byte[] myBytedKey = new byte[32];
        private static byte[] myBytedIV = new byte[16];
        private static void InitializeAES()
        {
            // These static bytes were ported in from Biblepay-QT, because OpenSSL uses a proprietary method to create the 256 bit AES-CBC key: EVP_BytesToKey(EVP_aes_256_cbc(), EVP_sha512()
            string sAdvancedKey = "98,-5,23,119,-28,-99,-5,90,62,-63,82,39,63,-67,-85,37,-29,-65,97,80,57,-24,71,67,119,14,-67,12,-96,99,-84,-97";
            string sIV = "29,44,121,61,-19,-62,55,-119,114,105,-123,-101,52,-45,29,-109";
            var vKey = sAdvancedKey.Split(new string[] { "," }, StringSplitOptions.None);
            var vIV = sIV.Split(new string[] { "," }, StringSplitOptions.None);
            myBytedKey = new byte[32];
            myBytedIV = new byte[16];

            for (int i = 0; i < vKey.Length; i++)
            {
                int iMyKey = (int)Common.GetDouble(vKey[i]);
                myBytedKey[i] = (byte)(iMyKey + 0);
            }
            for (int i = 0; i < vIV.Length; i++)
            {
                int iMyIV = (int)Common.GetDouble(vIV[i]);
                myBytedIV[i] = (byte)(iMyIV + 0);
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

        
        public static string ReverseHexString(string hexString)
        {
            var sb = new StringBuilder();
            for (var i = hexString.Length - 2; i > -1; i -= 2)
            {
                var hexChar = hexString.Substring(i, 2);
                sb.Append(hexChar);
            }
            return sb.ToString();
        }


        public static byte[] StringToByteArr(string hex)
        {
            try
            {
                if (hex == null)
                {
                    byte[] b1 = new byte[0];
                    return b1;
                }
                if (hex.Length % 2 == 1)
                    throw new Exception("The binary key cannot have an odd number of digits");

                byte[] arr = new byte[hex.Length >> 1];

                for (int i = 0; i < hex.Length >> 1; ++i)
                {
                    arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
                }
                return arr;
            }
            catch (Exception ex)
            {
                Log(" STBA " + ex.Message);
                byte[] b = new byte[0];
                return b;
            }
        }

        public static int GetHexVal(char hex)
        {
            try
            {
                int val = (int)hex;
                return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
            }
            catch (Exception ex)
            {
                Log("GHV " + ex.Message);
                return 0;
            }
        }

        public static int DateToUnixTimestamp(DateTime dt)
        {
            DateTime dt2 = Convert.ToDateTime(new DateTime(dt.Year, dt.Month, dt.Day, 12, 0, 0));
            Int32 unixTimestamp = (Int32)(dt2.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }


        public static bool InList(string sTypes, string sType)
        {
            if (sTypes == "all")
            {
                return true;
            }
            string[] vTypes = sTypes.Split(",");
            for (int i = 0; i < vTypes.Length; i++)
            {
                if (vTypes[i] == sType)
                    return true;
            }
            return false;
        }


        public static string ToNonNull(object o)
        {
            if (o == null)
                return String.Empty;

            string o1 = o.ToNonNullString();
            return o1;
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
        public static List<string> GetTSFileParams(string sPath)
        {
            List<string> t = new List<string>();

            if (!System.IO.File.Exists(sPath))
                return t;
            var sData = System.IO.File.ReadAllText(sPath);
            var vData = sData.Split("\n");
            for (int i = 0; i < vData.Length; i++)
            {
                string line = vData[i];
                if (line.Contains(".ts"))
                {
                    t.Add(line);
                }
            }
            return t;
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

}

