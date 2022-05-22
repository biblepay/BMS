using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace BiblePay.BMSD
{
    public static class Upgrade
    {

        private static string UpgradeHost = "https://social.biblepay.org/bms";
        private static string CDN = "https://globalcdn.biblepay.org:8443";

        public static void StartNewWebServer()
        {
            string sPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            ProcessStartInfo pi = new ProcessStartInfo("dotnet", "BiblePay.BMS.dll");
            pi.UseShellExecute = true;
            pi.WorkingDirectory = sPath;
            pi.CreateNoWindow = false;
            pi.WindowStyle = ProcessWindowStyle.Normal;
            Process procchild = Process.Start(pi);
        }
        private static bool UpgradeFile(string sLocalLocation, string sURL)
        {
            MyWebClient wc = new MyWebClient();
            try
            {
                //string sLocalPath = System.IO.Path.Combine(sDir, sFN);
                if (System.IO.File.Exists(sLocalLocation))
                {
                    System.IO.File.Delete(sLocalLocation);
                }   
                wc.DownloadFile(sURL, sLocalLocation);
                return true;
            }
            catch (Exception ex)
            {
                Common.Log("BiblePay::Upgrade::Replicate::Failure::" + sURL + "::" + sLocalLocation + "::" + ex.Message);
                return false;

            }
        }

        public static bool TerminateProcess(string sName)
        {
            if (Common.IsWindows())
                return false;

            // Used to Terminate a dotnetcore process by name (cross platform)
            try
            {
                Process[] proc = Process.GetProcessesByName(sName);
                Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

                if (proc.Length > 0)
                {
                    for (int i = 0; i < proc.Length; i++)
                    {

                        if (currentProcess.Id != proc[i].Id)
                        {
                            Common.Log("killing " + proc[i].Id.ToString());
                            proc[i].Kill();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.Log("BiblePayUpgrade (BUG)::TerminateProcess::Unable to terminate " + sName + " because " + ex.Message);
            }
            return false;
        }


        public static bool UpgradeNode(bool fDoIt)
        {
            if (fDoIt)
            {
                TerminateProcess("dotnet");
            }
            string fullURL = CDN + "/BMS/GetUpgradeManifest";
            MyWebClient wc = new MyWebClient();
            string sLastPath = "";
            bool fUpgraded = false;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                string sLocalDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string sManifest = wc.DownloadString(fullURL);
                string[] vManifest = sManifest.Split("<ROW>");
                for (int i = 0; i < vManifest.Length; i++)
                {
                    string sData = vManifest[i];
                    string[] vData = sData.Split("|");
                    if (vData.Length >= 3)
                    {
                        // File path is composed of My working dir + subdir + path

                        string sDir = vData[0];
                        sDir = Common.NormalizeFilePath(sDir);
                        sDir = Common.EnsurePathHasNoLeadingSlash(sDir);

                        string sFN = vData[1];
                        if (!sFN.Contains(".zip") && !sFN.Contains("htaccess"))
                        {
                            string sHash = vData[2];
                            //double dSz = GetDouble(vData[3]);
                            string sLocalPath = Path.Combine(sLocalDir, sDir, sFN);
                            string sLocalHash = Common.GetShaOfFile(sLocalPath);
                            sLastPath = sLocalPath;
                            bool fDLL = sLocalPath.ToLower().Contains("dll");

                            if (sLocalHash != sHash)
                            {
                                if (!fDoIt)
                                {
                                    Common.Log("Found difference in file " + sLocalPath);
                                }
                                if (fDLL && !fDoIt)
                                    return true;
                                if (fDoIt)
                                {
                                    Common.Log("Upgrading :: " + sLocalPath);
                                    string sURL = UpgradeHost + "/" + sDir + "/" + sFN;
                                    sURL = Common.NormalizeURL(sURL);

                                    string sLocalFolder = Path.Combine(sLocalDir, sDir);
                                    if (!System.IO.Directory.Exists(sLocalFolder))
                                    {
                                        System.IO.Directory.CreateDirectory(sLocalFolder);
                                    }

                                    fUpgraded = UpgradeFile(sLocalPath, sURL);
                                    if (fUpgraded)
                                    {
                                        Common.Log("Upgraded::" + sLocalDir + "::" + sFN);
                                    }
                                }
                            }
                        }
                    }
                }

                if (!Common.IsWindows() && fUpgraded)
                {
                    if (fDoIt)
                        StartNewWebServer();
                }
                return false;
            }
            catch (Exception ex)
            {
                Common.Log("BiblePay Upgrader::Upgrade1::" + ex.Message + "::" + sLastPath);
                return false;
            }

        }
    }
}

