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

namespace BiblePay.BMS.DSQL
{
    public static class ProcessAsyncHelper
    {
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

        public static bool WaitOnDirectorySize(string video_dir)
        {
            // This is a busy wait loop that monitors an ffmpeg write (because the subdirectory is getting bigger), and breaks out when the process is done writing to the subdir files...
            Log("BusyWait::Wait Start::");
            // Monitor
            long lastsz = 0;
            long interval = 6;  //Seconds
            long timeout_secs = 2500;
            for (int i = 0; i < timeout_secs / interval; i++)
            {
                long sz = DirSize(new DirectoryInfo(video_dir));
                if (i % 40 == 0)
                {
                    Log("Shell_Wait::dir_sz=" + sz.ToString());
                }
                System.Threading.Thread.Sleep((int)interval * 1000);
                if (lastsz == sz && i > 2)
                             break;
                lastsz = sz;
            }
            Log("BusyWait finished...");
            return true;
        }

        public static async Task<ProcessResult> ExecuteShellCommand(string command, string arguments, int timeout)
        {
            var result = new ProcessResult();

            using (var process = new Process())
            {
                process.StartInfo.FileName = command;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                var outputBuilder = new StringBuilder();
                var outputCloseEvent = new TaskCompletionSource<bool>();
                process.OutputDataReceived += (s, e) =>
                {
                    // The output stream has been closed i.e. the process has terminated
                    if (e.Data == null)
                    {
                        outputCloseEvent.SetResult(true);
                    }
                    else
                    {
                        outputBuilder.AppendLine(e.Data);
                    }
                };

                var errorBuilder = new StringBuilder();
                var errorCloseEvent = new TaskCompletionSource<bool>();

                process.ErrorDataReceived += (s, e) =>
                {
                    // The error stream has been closed i.e. the process has terminated
                    if (e.Data == null)
                    {
                        errorCloseEvent.SetResult(true);
                    }
                    else
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

                bool isStarted;

                try
                {
                    isStarted = process.Start();
                    // this line outputs the actual args:
                    await process.StandardInput.WriteLineAsync(arguments);
                    Log("Started bash " + arguments);

                }
                catch (Exception error)
                {
                    // Usually it occurs when an executable file is not found or is not executable
                    Log("cant find " + error.Message);
                    result.Completed = true;
                    result.ExitCode = -1;
                    result.Output = error.Message;
                    isStarted = false;
                }

                if (isStarted)
                {
                    // Reads the output stream first and then waits because deadlocks are possible
                    Log("started async process...");
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    // Creates task to wait for process exit using timeout
                    var waitForExit = WaitForExitAsync(process, timeout);
                    // Create task to wait for process exit and closing all output streams
                    var processTask = Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task);
                    // Waits process completion and then checks it was not completed by timeout
                    if (await Task.WhenAny(Task.Delay(timeout), processTask) == processTask && waitForExit.Result)
                    {
                        result.Completed = true;
                        result.ExitCode = process.ExitCode;

                        // Adds process output if it was completed with error
                        if (process.ExitCode != 0)
                        {
                            result.Output = $"{outputBuilder}{errorBuilder}";
                        }
                    }
                    else
                    {
                        try
                        {
                            Log("Killing hung process..." + arguments);

                            // Kill hung process
                            process.Kill();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            Log("Exiting fork of process " + command);
            return result;
        }


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
            string fullURL = "https://sanc1.cdn.biblepay.org:5000/BMS/GetUpgradeManifest";
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
                        if (!sFN.Contains(".zip"))
                        {
                            string sHash = vData[2];
                            string sLocalPath = Path.Combine(sLocalDir, sFN);
                            string sLocalHash = Common.GetShaOfFile(sLocalPath);
                            sLastPath = sLocalPath;
                            if (sLocalHash != sHash)
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
                BiblePay.BMS.Common.Log("BiblePay NeedsUpgrade::" + ex.Message + "::" + sLastPath);
                return false;
            }
        }

        private static Task<bool> WaitForExitAsync(Process process, int timeout)
        {
            return Task.Run(() => process.WaitForExit(timeout));
        }

        public struct ProcessResult
        {
            public bool Completed;
            public int? ExitCode;
            public string Output;
        }
    }






    public static class API
    {

        public static string ChopLastOctetFromURL(string sData)
        {
            string sDelimiter = "/";
            string[] vData = sData.Split(sDelimiter);
            string sOut = "";
            for (int i = 0; i < vData.Length-1; i++)
            {
                sOut += vData[i] + sDelimiter;
            }
            return sOut;
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

        public static string GetHashKey(string sURL)
        {
            string sCode = ChopLastOctetFromURL(sURL);//removes .ts files for distinctness
            string sHash = Common.GetSha256String(sCode);
            string sMyCode = sHash.Substring(0, 1);
            return sMyCode;
        }
        // Provide a method to pull down the IPFS hashes into a manifest
        public static async Task<bool> RetrieveManifestTwo()
        {
            List<string> l = BBPTestHarness.IPFS.QueryIPFSFolderContents("","");
            string sPath = GetFolder("database");
            var dir = new DirectoryInfo(sPath);
            foreach (var file in dir.EnumerateFiles("*.shard"))
            {
                file.Delete();
            }
            for (int i = 0; i < l.Count; i++)
            {
                string s = l[i];
                string sKey = GetHashKey(s);
                string sShardPath = Path.Combine(sPath, sKey + ".shard");
                System.IO.StreamWriter sw = new System.IO.StreamWriter(sShardPath, true);
                sw.WriteLine(s);
                sw.Close();
            }
            return true;
        }

        public static async Task<string> run_linux(string args)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            process.Start();
            await process.StandardInput.WriteLineAsync(args);
            Log("executed " + args);
            return "";
        }


        public static string run_cmd2(string sFullWorkingDirectory, string sFileName, string args)
        {
            string result = " ";
            try
            {
                sFullWorkingDirectory = sFullWorkingDirectory.Replace("//", "/");
                sFileName = sFileName.Replace("//", "/");
                args = args.Replace("//", "/");

                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = sFileName;
                start.Arguments = string.Format("{0}", args);
                start.UseShellExecute = false;
                start.CreateNoWindow = true;
                start.WindowStyle = ProcessWindowStyle.Normal;
                start.RedirectStandardOutput = true;
                start.WorkingDirectory = sFullWorkingDirectory;
                Log("fwd: " + sFullWorkingDirectory + ", filename=" + sFileName + ", args " + args);

                using (Process process = Process.Start(start))
                {

                    using (StreamReader reader = process.StandardOutput)
                    {
                        result = reader.ReadToEnd();
                    }
                }
                Common.Log("run_cmd result " + result);
                return result;
            }
            catch (Exception ex)
            {
                Common.Log("Run cmd failed " + ex.Message + " " + ex.StackTrace);
            }
            return result;
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


        /*
        public static async Task<bool> ConvertToHLS(string sURL, string sOutputID)
        {
            try
            {
                string sDestinationDir = GetFolder("video");
                sDestinationDir = System.IO.Path.Combine(sDestinationDir, sOutputID);
                if (!System.IO.Directory.Exists(sDestinationDir))
                {
                    System.IO.Directory.CreateDirectory(sDestinationDir);
                }
                string sSourcePath = GetFolder("temp");
                sSourcePath = System.IO.Path.Combine(sSourcePath, sOutputID + ".mp4");
                MyWebClient wc = new MyWebClient();
                wc.DownloadFile(sURL, sSourcePath);
                if (!System.IO.File.Exists(sSourcePath))
                {
                    return false;
                }
                string sDestinationThumbDir = Path.Combine(sDestinationDir, "p.jpg");
                string args = "-y -ss 5 -i " + sSourcePath + " -vframes 1 -f mjpeg " + sDestinationThumbDir;
                string arg2 = "-y -i " + sSourcePath + " -g 60 -hls_time 5 -hls_list_size 0 ";
                if (IsWindows())
                {
                    arg2 += sDestinationDir + "\\1.m3u8";
                }
                else
                {
                    arg2 += sDestinationDir + "/1.m3u8";
                }
                string sEXE = IsWindows() ? "c:\\inetpub\\wwwroot\\videos\\ffmpeg.exe" : "/usr/bin/ffmpeg";
                string sWD = IsWindows() ? "c:\\inetpub\\wwwroot\\videos" : "/inetpub/wwwroot/videos";
                if (IsWindows())
                {
                    string res1 = run_cmd2(sWD, sEXE, args);
                    string res2 = run_cmd2(sWD, sEXE, arg2);
                }
                else
                {
                    sEXE = "bash";
                    args = "/usr/bin/ffmpeg " + args;
                    arg2 = "/usr/bin/ffmpeg " + arg2;
                    await ProcessAsyncHelper.ExecuteShellCommand(sEXE, args, 10);
                    await ProcessAsyncHelper.ExecuteShellCommand(sEXE, arg2, 30);
                    ProcessAsyncHelper.WaitOnDirectorySize(sDestinationDir);
                }
                string sHLS = GetHLS(sDestinationDir);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        */

    }
}
