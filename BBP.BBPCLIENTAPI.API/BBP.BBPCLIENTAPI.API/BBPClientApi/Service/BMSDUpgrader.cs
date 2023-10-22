using BMSCommon;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BBPAPI
{
	internal class BMSDUpgrader
    {

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

        private static int nLastUpgradeChecked = 0;
        public static bool fWebDeployMatches = false;

        public static async Task<bool> UpgradeBMSD()
        {
            bool fPrimary = BBPAPI.Service.IsPrimary();
            if (fPrimary)
            { return false; }


            try
            {
                int nAge = BMSCommon.Common.UnixTimestamp() - nLastUpgradeChecked;
                if (nAge < 60 * 30)
                {
                    return false;
                }
                nLastUpgradeChecked = BMSCommon.Common.UnixTimestamp();
                // This process only upgrades the BMSD file, and is called from the MVC Web process 
                // It does not upgrade the entire web solution, and is not called from BMSD
                string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
                string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
                string sBBPProgramDir = Path.Combine(programFiles, "biblepaycore");
                // Start of ZIP (Web upgrade process)
                string sHomeDir = BMSCommon.Common.GetHomePath();
                string sRID = GetRIDForPlatform();
                string sWebZipFileName = "BMS_" + sRID + ".zip";
                string sPrefix = "BB2BwSbDCqCqNsfc7FgWFJn4sRgnUt4tsM";
                string sRemotePathZip = sPrefix + "/binaries/" + sWebZipFileName;
                string sRemotePathHashWeb = sRemotePathZip + ".hash";
                string sRemoteHashValueWeb = await BBPAPI.StorjIOReadOnly.StorjDownloadString(BBPAPI.Globals._DBUser2, sRemotePathHashWeb);
                string sLocalZipFile = Path.Combine(sHomeDir, sWebZipFileName);
                string sLocalHashWeb = Common.GetShaOfFile(sLocalZipFile);
                string sBBPDir = Path.Combine(sHomeDir, "biblepay");
                // Compare existing hash of zip to remote hash of zip
                string sFileNameBin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "BMSD.exe" : "BMSD";
                string sFullEXEPath = Path.Combine(sBBPProgramDir, sFileNameBin);
                fWebDeployMatches = (sLocalHashWeb == sRemoteHashValueWeb);

                if (fWebDeployMatches)
                {
                    return true;
                }
                Common.Log("BMSD_HASH_LOCAL_WEB::" + sLocalHashWeb + " , remotehash=" + sRemoteHashValueWeb);
                if (!String.IsNullOrEmpty(sLocalHashWeb)  && !String.IsNullOrEmpty(sRemoteHashValueWeb) && sRemoteHashValueWeb != sLocalHashWeb)
                {
                    // Needs upgrade; spawn a copy of BMSD.
                    Common.Log("Node needs upgraded... Launching BMSD...");
                    if (System.IO.File.Exists(sFullEXEPath))
                    {
                        ProcessStartInfo start = new ProcessStartInfo();
                        start.FileName = sFullEXEPath;
                        start.WindowStyle = ProcessWindowStyle.Hidden;
                        start.CreateNoWindow = true;
                        Process.Start(start);
                        System.Environment.Exit(0);
                        return true;
                    }
                }

                // End of Zip (web upgrade process)
                string sFileNameBinWithRID = "BMSD_" + sRID;
                string sRemotePath = sPrefix + "/binaries/" + sFileNameBinWithRID;
                string sRemotePathHashBin = sRemotePath + "_binary.hash";
                string sRemoteHashValueBin = await BBPAPI.StorjIOReadOnly.StorjDownloadString(BBPAPI.Globals._DBUser2,sRemotePathHashBin);
                string sLocalHashPathBin = Path.Combine(sBBPProgramDir, sFileNameBin);
                string sLocalHashBin = Common.GetShaOfFile(sLocalHashPathBin);
                //Common.Log("Remote path Bin::" + sRemotePathBin + ",Web::" + sRemotePathHashWeb);
                
                if (false && !String.IsNullOrEmpty(sLocalHashBin) && !String.IsNullOrEmpty(sRemoteHashValueBin) && sLocalHashBin != sRemoteHashValueBin)
                {
                    Common.Log("BMSD_HASH_LOCAL_BIN::" + sLocalHashBin + " , remotehash=" + sRemoteHashValueBin);

                    // Empty out the final directory here.
                    Common.Log("BMSD needs upgraded...");
                    bool fDL = await BBPAPI.StorjIOReadOnly.StorjDownloadLg(sRemotePath + ".zip", sLocalZipFile);
                    Common.Log("1001.1");
                    if (fDL && false)
                    {

                        // Currently we are not allowed to upgrade a file in the users C:\Program files directory....
                        // We will need to move "bmsd.exe" to %appdata%\biblepay\bms
                        if (System.IO.File.Exists(sFullEXEPath))
                        {
                            System.IO.File.Delete(sFullEXEPath);
                        }
                        Common.Log("1002 (Unzipping BMSD into upgrade location) " + sBBPProgramDir);
                        // Unzip into place.
                        ICSharpCode.SharpZipLib.Zip.FastZip z = new ICSharpCode.SharpZipLib.Zip.FastZip();
                        z.CreateEmptyDirectories = true;
                        z.ExtractZip(sLocalZipFile, sBBPProgramDir, String.Empty);
                        Common.Log("1003 (Upgraded successfully).");
                        return true;
                    }
                    else
                    {
                        Common.Log("Unable to upgrade node");
                        return false;
                    }
                }
                else
                {
                    Common.Log("Does not need upgraded.");
                }

                return false; //not upgraded
            }
            catch(Exception ex1)
            {
                Common.Log("Unable to upgrade node..." + ex1.Message);
                return false;
            }

        }

    }
}
