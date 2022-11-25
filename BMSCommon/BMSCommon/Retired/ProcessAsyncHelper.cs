using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BMSCommon
{
    public static class ProcessAsyncHelper
    {

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
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

        public static long WaitOnDirectorySize(string video_dir)
        {
            // Monitor
            long lastsz = 0;
            long interval = 6;  //Seconds
            long timeout_secs = 2500;
            long sz = 0;
            for (int i = 0; i < timeout_secs / interval; i++)
            {
                sz = DirSize(new DirectoryInfo(video_dir));
                if (i % 20 == 0)
                {
                    Common.Log("Shell_Wait::dir_sz=" + sz.ToString());
                }
                System.Threading.Thread.Sleep((int)interval * 1000);

                if (lastsz == sz && i > 2 && sz > 0)
                    break;
                if (lastsz == sz && i > 3)
                    break;
                lastsz = sz;
            }
            Common.Log("Shell wait finished size=" + sz.ToString());
            return sz;
        }


        public static string run_cmd2(string sFullWorkingDirectory, string sFileName, string args)
        {
            string result = " ";
            try
            {
                sFullWorkingDirectory = sFullWorkingDirectory.Replace("//", "/");
                sFileName = sFileName.Replace("//", "/");
                //args = args.Replace("//", "/");

                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = sFileName;
                start.Arguments = string.Format("{0}", args);
                start.UseShellExecute = false;
                start.CreateNoWindow = false;
                start.WindowStyle = ProcessWindowStyle.Normal;
                start.RedirectStandardOutput = true;
                start.WorkingDirectory = sFullWorkingDirectory;
                Common.Log("fwd: " + sFullWorkingDirectory + ", filename=" + sFileName + ", args " + args);
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
        /*
        public static string GetHLS(string sDestinationDir)
        {
            string sHLS = String.Empty;
            string[] filePaths = Directory.GetFiles(sDestinationDir);

            for (int j = 0; j < filePaths.Length; j++)
            {
                FileInfo fi = new FileInfo(filePaths[j]);
                sHLS += fi.Name + ",";
            }
            return sHLS;
        }
        */


        public static async Task<ProcessResult> ExecuteShellCommand(string command, string arguments, int timeout)
        {
            var result = new ProcessResult();

            using (var process = new Process())
            {
                // If you run bash-script on Linux it is possible that ExitCode can be 255.
                // To fix it you can try to add '#!/bin/bash' header to the script.
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
                    Common.Log("Started bash " + arguments);
                }
                catch (Exception error)
                {
                    // Usually it occurs when an executable file is not found or is not executable
                    Common.Log("cant find " + error.Message);
                    result.Completed = true;
                    result.ExitCode = -1;
                    result.Output = error.Message;

                    isStarted = false;
                }

                if (isStarted)
                {
                    // Reads the output stream first and then waits because deadlocks are possible
                    Common.Log("started async process...");

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
                            Common.Log("Killing hung process..." + arguments);
                            //process.kill ? in linux this process shouldnt need killed I think...because its running in bash
                        }
                        catch
                        {
                        }
                    }
                }
            }
            Common.Log("Exiting fork of process " + command);
            return result;
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

}
