using BMSCommon;
using BMSCommon.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using static BMSCommon.Common;

namespace SanctuaryUpgrader
{
	public static class Publisher
    {



        public static void ExtractZipFolder()
        {
            // unzip
            ICSharpCode.SharpZipLib.Zip.FastZip z = new ICSharpCode.SharpZipLib.Zip.FastZip();
            z.CreateEmptyDirectories = true;
            z.ExtractZip("c:\\inetpub\\mywwwroot.zip", "c:\\inetpub\\biblepay\\myunzipwwwroot", "");
        }

        public static class RIDS
        {
            public static string WINDOWS = "win-x64";
            public static string WINDOWS_EXTENSION = "exe";
            public static string LINUX = "linux-x64";
            public static string LINUX_EXTENSION = String.Empty;
            public static string OSX = "osx-x64";
            public static string OSX_EXTENSION = "app";
        }

        


        public static class Projects
        {
            public static string BMSD = "B:\\BiblePay\\BiblePayBMS\\BMS.BMSD\\BiblePay.BMSD.csproj";
            public static string BMS = "B:\\BiblePay\\BiblePayBMS\\BMS\\BiblePay.BMS.csproj";
        }

        private static async Task<bool> PushToCloud(string sFinalZipPath, string sStorjDest, string sOutputDir, string sRID, string sOrigBMSDFileName)
        {
            string sProdKey = GetConfigKeyValue("foundationprivkey");
            // The file itself
            UploadFileObject u = new UploadFileObject();
            u.SourceFilePath = sFinalZipPath;
            u.StorjDestinationPath = sStorjDest;
            u.OverriddenBBPPrivateKey = sProdKey;
            UploadFileResult url = BBPAPI.Interface.PinLogic.UploadFile(u).Result;
            // Find Hash
            string sHash = Common.GetShaOfFile(sFinalZipPath);
            // If this is BMSD.exe, get the hash of the exe too
            if (sOrigBMSDFileName != String.Empty)
            {
                string sEXEPath = Path.Combine(sOutputDir, sOrigBMSDFileName);
                string sEXEHash = Common.GetShaOfFile(sEXEPath);
                string sEXEHashPath = sOutputDir + "BMSD_" + sRID + "_binary.hash";
                string sStorjDestHashEXE = "binaries/BMSD_" + sRID + "_binary.hash";
                // storj location: binaries/BMSD_win-x64_binary.hash
                System.IO.File.WriteAllText(sEXEHashPath, sEXEHash);
                // The hash of the file
                UploadFileObject u2 = new UploadFileObject();
                u2.SourceFilePath = sEXEHashPath;
                u2.StorjDestinationPath = sStorjDestHashEXE;
                u2.OverriddenBBPPrivateKey = sProdKey;
                UploadFileResult urlEXE = BBPAPI.Interface.PinLogic.UploadFile(u2).Result;
                bool f14000 = false;
            }

            string sHashPath = sFinalZipPath + ".hash";
            string sStorjDestHash = sStorjDest + ".hash";
            System.IO.File.WriteAllText(sHashPath, sHash);
            // The hash of the file
            UploadFileObject u3 = new UploadFileObject();
            u3.SourceFilePath = sHashPath;
            u3.StorjDestinationPath = sStorjDestHash;
            u3.OverriddenBBPPrivateKey = sProdKey;
            UploadFileResult urlHash00 = BBPAPI.Interface.PinLogic.UploadFile(u3).Result;
            // Push the current version of the BBP API
            string sStorjVersionDest = "binaries/BBPAPI_VERSION.dat";
            string sStorjVersionLocalPath = Path.Combine(Path.GetTempPath(), "BBPAPI_VERSION.dat");
            //System.IO.File.WriteAllText(sStorjVersionLocalPath, BBPAPI.DB.BBPAPI_VERSION.ToString());
            UploadFileObject u4 = new UploadFileObject();
            u4.SourceFilePath = sStorjVersionLocalPath;
            u4.StorjDestinationPath = sStorjVersionDest;
            u4.OverriddenBBPPrivateKey = sProdKey;
            UploadFileResult urlHash01 = BBPAPI.Interface.PinLogic.UploadFile(u4).Result;
            return true;
        }

        public static bool Robocopy(string sSourceDir, string sTargetDir)
        {
            //robocopy B:\BiblePay\BiblePayBMS\BMS\bin\Publish m:\bms /xo /S
            string sCmd = "" + sSourceDir + " " + sTargetDir + " /xo /S";
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "robocopy";
            p.StartInfo.Arguments = sCmd;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            p.Start();
            System.Threading.Thread.Sleep(1000);

            string sOut = p.StandardOutput.ReadToEnd();
            bool fSucceeded = sOut.Contains("Ended");
            return fSucceeded;
        }
        public static async Task<bool> SendBuildCommand(string sRID, string sProject, string sBMSDOrigFileName)
        {
            //string sRID = "win-x64";
            string sMoniker = (sProject == Projects.BMSD) ? "BMSD" : "BMS";
            string sOutputNameNoExtension = "BiblePay." + sMoniker;

            string sOutput = "c:\\code\\publishDir\\" + sMoniker + "\\" + sRID + "\\";
            string sOutputZip = "c:\\code\\publishDirZips\\";
            //string sEXEDir = sOutput + sMoniker + "." + sRidExtension;

            /*

            string sCmd = "publish " + sProject + " -p:TargetFramework=netcoreapp3.1 "
                + " -p:RuntimeIdentifier=" + sRID + " -p:DeleteExistingFiles=true -p:ExcludeApp_Data=false -p:SelfContained=true "
                + " -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRunShowWarnings=true "
                + " -p:PublishReadyToRun=false --output " + sOutput;
            */


            string sBaseCmd = sProject + " -p:TargetFramework=net7.0 "
                + " -p:RuntimeIdentifier=" + sRID + " -p:DeleteExistingFiles=true -p:ExcludeApp_Data=false -p:SelfContained=true "
                + " -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRunShowWarnings=true "
                + " -p:PublishReadyToRun=false ";
            string sBuildCmd = "build " + sBaseCmd;
            string sPublishCmd = "publish " + sBaseCmd + " --output " + sOutput;



            /*
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "dotnet";
            p.StartInfo.Arguments = sBuildCmd;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            string sOut1 = p.StandardOutput.ReadToEnd();
            */


            var p2 = new System.Diagnostics.Process();
            p2.StartInfo.FileName = "dotnet";
            p2.StartInfo.Arguments = sPublishCmd;
            p2.StartInfo.RedirectStandardOutput = true;
            p2.StartInfo.UseShellExecute = false;
            p2.StartInfo.CreateNoWindow = true;
            p2.Start();
            string sOut2 = p2.StandardOutput.ReadToEnd();


            bool fSucceeded = !sOut2.Contains("failed");
            bool fSucceeded2 = !sOut2.Contains("failed");
            if (!fSucceeded)
            {
                return false;
            }
            var z = new ICSharpCode.SharpZipLib.Zip.FastZip();
            z.CreateEmptyDirectories = true;
            System.IO.Directory.CreateDirectory(sOutput);
            System.IO.Directory.CreateDirectory(sOutputZip);

            try
            {
                string sFinalZipPath = sOutputZip + sMoniker + "_" + sRID + ".zip";

                if (sRID == "osx-x64")
                {
                    System.IO.File.Move(sOutput + sOutputNameNoExtension, sOutput + sOutputNameNoExtension + ".app", true);
                }

                z.CreateZip(sFinalZipPath, sOutput, true, String.Empty);
                string sStorjDest = "binaries/" + sMoniker + "_" + sRID + ".zip";


                await PushToCloud(sFinalZipPath, sStorjDest, sOutput, sRID, sBMSDOrigFileName);

                Console.WriteLine("pushed " + sFinalZipPath);
                System.Diagnostics.Debug.WriteLine("Pushed " + sFinalZipPath);

            }
            catch (Exception ex)
            {
                fSucceeded = false;
            }

            return fSucceeded;
        }

        public static async Task<bool> PublishWebProject(bool fFull)
        {

            await BBPAPI.ServiceInit.Init();

            if (true)
            {
                await SendBuildCommand(RIDS.OSX, Projects.BMSD, "BiblePay.BMSD.app");
                await SendBuildCommand(RIDS.OSX, Projects.BMS, "");
            }


            // 1
            if (false)
            {
                await SendBuildCommand(RIDS.LINUX, Projects.BMSD, "BiblePay.BMSD");
                await SendBuildCommand(RIDS.LINUX, Projects.BMS, "");
            }
            // 2
            await SendBuildCommand(RIDS.WINDOWS, Projects.BMSD, "BiblePay.BMSD.EXE");
            await SendBuildCommand(RIDS.WINDOWS, Projects.BMS, "");

            // Robocopy the full directory to the Y: drive (the build VMS)
            
            Robocopy("c:\\code\\publishdir", "y:\\bmsrelease");
            // Copy the ffmpeg stuff here?
            // Robocopy("B:\\BiblePay\\BiblePayBMS\\BMS\\wwwroot\\bbp", "y:\\bmsrelease\\bbp");

            System.Environment.Exit(0);
            return true;

            /*
            */
        }
    }
}
