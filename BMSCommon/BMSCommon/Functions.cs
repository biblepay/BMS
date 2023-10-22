using BMSCommon.Model;
//using BMSShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using static BMSCommon.Common;


namespace BMSCommon
{
    public static class Functions
    {
        

        public static string GetPubKeyFromPrivKey(string sPrivKey, bool fTestNet)
        {
            string sPubKey = NBitcoin.Crypto.BBPTransaction.GetPubKeyFromPrivKey(fTestNet, sPrivKey);
            return sPubKey;
        }


        public static string GetRootFolder()
        {
            bool fUnix = Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX;
            string sPath = fUnix ? "/" : "c:\\";
            return sPath;
        }
        public static string GetPathDelimiter()
        {
            string sPathDelimiter = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) ? "/" : "\\";
            return sPathDelimiter;
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

        /*
        
        public static string GetConfigationKeyValue(string _Key)
        {
            string sPath = GetFolder("") + "";
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
                    if (sKey.ToLower() == _Key.ToLower())
                        return sValue;
                }

            }
            return string.Empty;
        }

        */



        public static string GE(string sData, string sDelim, int iEle)
        {
            string[] vGE = sData.Split(sDelim);
            if (vGE.Length > iEle)
                return vGE[iEle];
            else
                return "";
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


        public static string ByteArrayToHexString(byte[] arrInput)
        {
            StringBuilder sOutput = new StringBuilder(arrInput.Length);

            for (int i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString().ToLower();
        }



        public static int GetFolderTimeStamp(int iTimeStamp)
        {
            DateTime startDate = new DateTime(2019, 1, 1);
            int iBase = DateToUnixTimestamp(startDate);
            int iOffset = (iTimeStamp - iBase) % 86400;
            int iActual = iTimeStamp - iOffset;
            return iActual;
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