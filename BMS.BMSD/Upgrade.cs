using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Model;

namespace BiblePay.BMSD
{
    public static class Upgrade
    {

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
        public static bool IsPortOpen(string sAddress, int nPort)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(sAddress, nPort);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool StartInternalPort()
        {
            try
            {
                int port = 7999;
                TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                return true;
            }
            catch(Exception ex)
            {
                Common.Log("Unable to open port 7999");
                return false;
            }
        }

        public static bool BusyWaitForPort(int nPort, int nTimeout)
        {

            for (int i = 0; i < nTimeout; i++)
            {
                bool fOpen = IsPortOpen("127.0.0.1", nPort);
                if (fOpen)
                {
                    return true;
                }
                System.Threading.Thread.Sleep(1000);
            }
            return false;
        }
        public static void StartNewWebServer(ProcessWindowStyle pws)
        {
            // This check prevents more than one copy from running
            bool fOpen = IsPortOpen("127.0.0.1", 8080);
            if (!fOpen)
            {
                // If we are the only one running:
                //string sPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string sPath = Common.GetHomePath();
                string sFileName = Common.IsWindows() ? "BiblePay.BMS.EXE" : "BiblePay.BMS";

                if (Common.IsMac())
                {
                    Common.Log("This is a mac");
                    sFileName = "BiblePay.BMS.app";
                }
                Common.Log("Looking for file :: " + sFileName);

                string sArgs = Common.IsWindows() ? "" : "";
                ProcessStartInfo pi = new ProcessStartInfo(sFileName, sArgs)
                {
                    UseShellExecute = true,
                    WorkingDirectory = sPath,
                    CreateNoWindow = false,
                    WindowStyle = pws
                };
                Process procchild = Process.Start(pi);
                Common.Log("Started new web server...Waiting for port to open first...");
                BusyWaitForPort(8080, 30);
                Common.Log("Continuing...");
            }
            OpenBrowser("http://localhost:8080");
        }

        private static async Task<bool> UpgradeFileStorj(string sLocalLocation, string sSource)
        {
            if (!System.IO.Directory.Exists(Path.GetDirectoryName(sLocalLocation)))
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(sLocalLocation));
            }

            try
            {
                bool f = await BBPAPI.StorjIOReadOnly.StorjDownloadLg(sSource, sLocalLocation);
                return f;
            }
            catch (Exception ex)
            {
                Common.Log("Upgradefilestorj::" + ex.Message);
                return false;
            }
        }

        public static bool TerminateProcess(string sName)
        {
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
                Common.Log("BiblePayUpgrade (BUG)::TerminateProcess::Unable to terminate " + sName + " because " +
                           ex.Message);
            }
            return false;
        }


        public static string GetRIDForPlatform()
        {
            string sRID = String.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                sRID = "osx-x64";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                sRID = "win-x64";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))

            {
                sRID = "linux-x64";
            }
            return sRID;
        }

        public static string GetBiblePayCoreConfigPath()
        {
            string homePathLinux = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            homePathLinux += "/.biblepay";
            string homePathWin = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\biblepay";
            return GetRIDForPlatform() == "win-x64" ? homePathWin : homePathLinux;
        }

        public static void NotifyIPC(string sData)
        {
            try
            {
                string sFolder = GetBiblePayCoreConfigPath();
                string sPath = Path.Combine(sFolder, "ipc.dat");
                Common.Log("Notify IPC v1.1 as of directory= " + sPath);
                System.IO.File.WriteAllText(sPath, sData);
            }catch(Exception ex)
            {
                Common.Log("Notify ipc::error::" + ex.Message + "::" + sData);
            }
        }
        public static async Task<bool> UpgradeNode(bool fTerminate)
        {
            await BBPAPI.ServiceInit.Init();
            string sLocalDir = Common.GetHomePath();
            //Common.IsWindows() ? "c:\\inetpub\\biblepay" : "~/biblepay";
            if (!System.IO.Directory.Exists(sLocalDir))
            {
                System.IO.Directory.CreateDirectory(sLocalDir);
            }
            Common.Log("0001");
            // Compare existing hash of zip to remote hash of zip
            string sRID = GetRIDForPlatform();
            string sFileName = "BMS_" + sRID + ".zip";
            string sPrefix = "BB2BwSbDCqCqNsfc7FgWFJn4sRgnUt4tsM"; // Foundation public key
            string sRemotePath = sPrefix  + "/binaries/" + sFileName;
            string sRemotePathHash = sRemotePath + ".hash";
            string sRemoteHashValue = await BBPAPI.StorjIOReadOnly.StorjDownloadString(BBPAPI.Globals._DBUser2,sRemotePathHash);
            string sRemotePathHashLinux = sPrefix + "/binaries/BMS_linux-x64.zip.hash";
            string sRemoteHashValueLinux = await BBPAPI.StorjIOReadOnly.StorjDownloadString(BBPAPI.Globals._DBUser2, sRemotePathHash);
            string sLocalHashPath = Path.Combine(sLocalDir, sFileName);
            string sLocalHash = Common.GetShaOfFile(sLocalHashPath);
            string sLocalZipFile = Path.Combine(sLocalDir, sFileName);
            Common.Log(sLocalZipFile);
            Common.Log("Remote path " + sRemotePathHash);

            Common.Log("h1 " + sLocalHash + " , remotehash=" + sRemoteHashValue);

            if (sLocalHash != sRemoteHashValue)
            {
                NotifyIPC("Upgrading Node... Please Wait...  This can take up to 5 minutes...  (Version " 
                    + sRemoteHashValue + ")...");

                if (fTerminate)
                {
                    //TerminateProcess("dotnet");
                    TerminateProcess("BiblePay.BMS");
                }
                // Empty out the final directory here.
                
                bool fDL = await BBPAPI.StorjIOReadOnly.StorjDownloadLg(sRemotePath, sLocalZipFile);
                
                if (fDL)
                {
                    // Unzip into place.
                    ICSharpCode.SharpZipLib.Zip.FastZip z = new ICSharpCode.SharpZipLib.Zip.FastZip();
                    z.CreateEmptyDirectories = true;
                    z.ExtractZip(sLocalZipFile,
                        sLocalDir, String.Empty);
                    return true;
                }
                else
                {
                    Common.Log("Unable to upgrade node");
                    return false;
                }

                return true; //upgraded
            }
            else
            {
                NotifyIPC("1");
                Common.Log("Does not need upgraded.");
            }

            return false; //not upgraded
        }
    }
}

