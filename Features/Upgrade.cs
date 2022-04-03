using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using BiblePay.BMS.CustomExtensions;

namespace BiblePay.BMS.CustomExtensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string str)
        {
            if (str == null || str == String.Empty)
                return true;
            return false;
        }

        public static string TrimAndReduce(this string str)
        {
            return str.Trim();
        }

        public static string ToNonNullString(this object o)
        {
            if (o == null)
                return String.Empty;
            return o.ToString();
        }

        public static string[] Split(this string str, string sDelimiter)
        {
            string[] vSplitData = str.Split(new string[] { sDelimiter }, StringSplitOptions.None);
            return vSplitData;
        }

        public static double ToDouble(this string o)
        {
            try
            {
                if (o == null)
                    return 0;
                if (o.ToString() == string.Empty)
                    return 0;
                double d = Convert.ToDouble(o.ToString());
                return d;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        
    }
}

namespace BiblePay.BMS
{
    // R ANDREWS - June 28th, 2019
    public class Upgrade
    {
        private string host = "https://social.biblepay.org/bms";
        private bool UpgradeFile(string sDir, string sFN)
        {
            MyWebClient wc = new MyWebClient();
            string sFullAddress = host + "/" + sFN;
            try
            {
                string sLocalPath = System.IO.Path.Combine(sDir, sFN);
                System.IO.File.Delete(sLocalPath);
                wc.DownloadFile(sFullAddress, sLocalPath);
                return true;
            }
            catch (Exception ex)
            {
                BiblePay.BMS.Common.Log("BiblePay::Upgrade::Replicate::Failure::" + sDir + "::" + sFN + "::" + ex.Message);
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
            catch(Exception ex)
            {
                BiblePay.BMS.Common.Log("BiblePayUpgrade (BUG)::TerminateProcess::Unable to terminate " + sName + " because " + ex.Message);
            }
            return false;
        }

        public static void StartNewThread()
        {
            string sPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            ProcessStartInfo pi = new ProcessStartInfo("dotnet", "BiblePay.BMSD.dll");
            pi.UseShellExecute = true;
            pi.WorkingDirectory = sPath;
            pi.CreateNoWindow = false;
            pi.WindowStyle = ProcessWindowStyle.Normal;
            Process procchild = Process.Start(pi);
        }



        public Upgrade()
        {
            TerminateProcess("dotnet");
            string fullURL = "https://sanc1.cdn.biblepay.org:5000/BMS/GetUpgradeManifest";
            MyWebClient wc = new MyWebClient();
            string sLastPath = "";
            bool fUpgraded = false;
            try
            {
                //ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
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
                        if (!sFN.Contains(".zip"))
                        {
                            string sHash = vData[2];
                            double dSz = vData[3].ToDouble();
                            string sLocalPath = Path.Combine(sLocalDir, sFN);
                            string sLocalHash = Common.GetShaOfFile(sLocalPath);
                            sLastPath = sLocalPath;
                            if (sLocalHash != sHash)
                            {
                                Common.Log("Upgrading :: " + sLocalPath);
                                fUpgraded = UpgradeFile(sLocalDir, sFN);
                                if (fUpgraded)
                                {
                                    Common.Log("Upgraed::" + sLocalDir + "::" + sFN);
                                }
                            }
                        }
                    }
                }

                if (!Common.IsWindows() && fUpgraded)
                {
                    StartNewThread();
                }

            }
            catch (Exception ex)
            {
                BiblePay.BMS.Common.Log("BiblePay Upgrader::Upgrade1::" + ex.Message  + "::" + sLastPath);
            }

        }
    }
}
