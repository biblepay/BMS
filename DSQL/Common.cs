using BiblePay.BMS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static BMSCommon.Common;

namespace BiblePay.BMS
{
    public static class Common
    {
        public static int BMS_VERSION = 1055;
        public static int MIN_BMS_VERSION = 1001;
        public static int DEFAULT_PORT = 8443;
        public static bool fDebug = false;
        public static string BX_API = "https://chainz.cryptoid.info/bbp/api.dws?";



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
                string[] Files = Directory.GetFileSystemEntries(sDirectory, "*", SearchOption.AllDirectories);

                List<string> hashes = new List<string>();

                foreach (string File1 in Files)
                {

                    // For each timestamped directory in the folder list, see if they exist in other servers.
                    FileInfo fi1 = new FileInfo(File1);
                    bool bReplicate = true;
                    if (fi1.Extension == ".pdb" || fi1.Extension == ".log" || 
                        fi1.Extension == ".manifest" || fi1.Extension == ".xml" || fi1.Extension == "" || fi1.Extension == null 
                        || fi1.Extension == "." || fi1.Extension == ".pfx" || fi1.Name.Contains("htaccess"))
                        bReplicate = false;
                    //if (fi1.Extension != ".dll")                        bReplicate = false;

                    if (fi1.Name == "xBiblePay.BMSD.dll")
                    {
                        bReplicate = false;
                    }
                    string sFullPath = fi1.DirectoryName;

                    string sSubDir = sFullPath.Replace(sDirectory, "");
                    if (sSubDir.Contains("runtimes"))
                    {
                        bReplicate = false;
                    }
                    if (bReplicate)
                    {
                        string fileHash = GetShaOfFile(File1);

                        string sData = sSubDir + "|" + fi1.Name + "|" + fileHash + "|" + fi1.Length.ToString() + "<ROW>";
                        sHashes.Append(sData);
                    }
                }
                return sHashes.ToString();
            }
            catch (Exception ex)
            {
                Log("error " + ex.Message);
                return "";
            }
        }


        public static string GetUpgradeCDN()
        {
            string sCDN = "https://sanc1.cdn.biblepay.org:" + DEFAULT_PORT.ToString();
            return sCDN;
        }



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
                string sPath = BMSCommon.Common.GetFolder("database", "fluffy.bytes");
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
                BMSCommon.Common.Log("GetSSL::" + ex.Message);
                return null;
            }
        }
    }
}
