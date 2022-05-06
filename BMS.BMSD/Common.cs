using System;
using System.Collections.Generic;
using System.Text;

namespace BiblePay.BMSD
{
    public static class Common
    {

        public static string ByteArrayToHexString(byte[] arrInput)
        {
            StringBuilder sOutput = new StringBuilder(arrInput.Length);

            for (int i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString().ToLower();
        }
        public static string NormalizeURL(string sURL)
        {
            sURL = sURL.Replace("https://", "{https}");
            sURL = sURL.Replace("///", "/");
            sURL = sURL.Replace("//", "/");
            sURL = sURL.Replace("\\", "/");
            sURL = sURL.Replace("//", "/");

            sURL = sURL.Replace("{https}", "https://");
            return sURL;
        }
        public static int UnixTimestamp()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public static string GetShaOfBytes(byte[] bytes)
        {
            var hash = System.Security.Cryptography.SHA256.Create().ComputeHash(bytes);
            return ByteArrayToHexString(hash);
        }

        public static bool IsWindows()
        {
            bool fUnix = Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX;
            return !fUnix;
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

        public static string EnsurePathHasNoLeadingSlash(string sPath)
        {
            if (sPath.StartsWith("\\") && sPath.Length > 1)
            {
                sPath = sPath.Substring(1, sPath.Length - 1);
                return sPath;

            }
            else if (sPath.StartsWith("/") && sPath.Length > 1)
            {
                sPath = sPath.Substring(1, sPath.Length - 1);
                return sPath;
            }
            return sPath;
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

        public static void Log(string sData)
        {
            try
            {
                string sPath = IsWindows() ? "c:\\inetpub\\wwwroot\\bms" : "/inetpub/wwwroot/bms";
                if (!System.IO.Directory.Exists(sPath))
                    System.IO.Directory.CreateDirectory(sPath);

                string sPath2 = System.IO.Path.Combine(sPath, "bms.log");
                System.IO.StreamWriter sw = new System.IO.StreamWriter(sPath2, true);
                string Timestamp = DateTime.Now.ToString();
                sw.WriteLine(Timestamp + ": " + sData);
                sw.Close();
            }
            catch (Exception ex)
            {
                string sMsg = ex.Message;
            }
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
