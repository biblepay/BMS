using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BiblePay.BMS.Common;
using static BMSCommon.Common;

namespace BiblePay.BMS.DSQL
{
    public static class ProcessAsyncHelper
    {
        public static void StartNewThread()
        {
            // This is used by our upgrader.. We spawn a new thread when the code changes..
            string sPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            ProcessStartInfo pi = new ProcessStartInfo("dotnet", "BiblePay.BMSD.dll");
            pi.UseShellExecute = true;
            pi.WorkingDirectory = sPath;
            pi.CreateNoWindow = false;
            pi.WindowStyle = ProcessWindowStyle.Normal;
            Process procchild = Process.Start(pi);
        }

        public static bool NeedsUpgraded()
        {
            // This routine checks to see if we need to upgrade.
            string fullURL = GetUpgradeCDN() + "/BMS/GetUpgradeManifest";
            MyWebClient wc = new MyWebClient();
            string sLastPath = "";
            try
            {
                // ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                string sLocalDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string sManifest = wc.DownloadString(fullURL);
                string[] vManifest = sManifest.Split("<ROW>");
                for (int i = 0; i < vManifest.Length; i++)
                {
                    string sData = vManifest[i];
                    string[] vData = sData.Split("|");
                    if (vData.Length >= 3)
                    {
                        string sDir = vData[0];
                        string sFN = vData[1];
                        bool fDLL = (sFN.ToLower().Contains("dll"));
                        if (!sFN.Contains(".zip"))
                        {
                            string sHash = vData[2];
                            string sLocalPath = Path.Combine(sLocalDir, sFN);
                            string sLocalHash = GetShaOfFile(sLocalPath);
                            sLastPath = sLocalPath;
                            if (sLocalHash != sHash && fDLL)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log("BiblePay NeedsUpgrade::" + ex.Message + "::" + sLastPath);
                return false;
            }
        }

    }


}
