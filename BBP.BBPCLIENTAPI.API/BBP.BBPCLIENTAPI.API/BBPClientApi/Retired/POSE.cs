using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BMSShared.Common;

namespace BBPTestHarness
{
    public static class POSE
    {

        private static int nCurCountCore = 0;
        public static int nHashes = 0;
        public static long nStartTime = UnixTimestamp();
        public static long nEndTime = 0;
        public static int nCurCore = 0;

        public static double ProcSpeedTest(long nSeconds, int iCores)
        {
            nHashes = 0;
            nStartTime = UnixTimestamp();
            nEndTime = nStartTime + nSeconds;
            for (int i = 0; i < iCores; i++)
            {
                nCurCore = i;
                System.Threading.Thread t = new System.Threading.Thread(IndividualCoreTest);
                t.Start();
            }
            while (true)
            {
                long nTime = UnixTimestamp();

                if (nTime > nEndTime)
                    break;
                System.Threading.Thread.Sleep(100);
            }
            return nHashes;
        }

        public static double GetDiskSizeTB()
        {
            try
            {
                string p1 = System.IO.Directory.GetCurrentDirectory();
                string p2 = System.IO.Path.GetPathRoot(p1);
                DriveInfo drive = new DriveInfo(p2);
                double totalBytes = drive.TotalSize;
                //var freeBytes = drive.AvailableFreeSpace;
                //var freePercent = (int)((100 * freeBytes) / totalBytes);
                var TB = totalBytes / 1000000000000.01;
                return TB;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        

        private static void IndividualCoreTest()
        {
            string sNewValue = "";
            int nMyHashes = 0;
            nCurCountCore++;
            while (true)
            {
                sNewValue = GetSha256String(sNewValue);
                nHashes++;
                long nTime = UnixTimestamp();
                nMyHashes++;
                if (nTime > nEndTime)
                    break;
            }
        }

        


        public static double GetCPUUsage()
        {
            //return .1;

            var startTime = DateTime.UtcNow;
            Process[] p = Process.GetProcesses();
            TimeSpan startCPUUsage = new TimeSpan();
            // Start watching CPU

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int i = 1; i < p.Length; i++)
            {
                Process p1 = p[i];
                try
                {
                    startCPUUsage += p1.TotalProcessorTime;
                }
                catch (Exception)
                {
                    // access denied to one process
                }
            }

            // Meansure something else, such as .Net Core Middleware
            for (int i = 0; i < 1; i++)
            {
                System.Threading.Thread.Sleep(1000);
            }
            // Stop watching to meansure
            stopWatch.Stop();
            var endTime = DateTime.UtcNow;
            TimeSpan endCPUUsage = new TimeSpan();
            for (int i = 1; i < p.Length; i++)
            {
                try
                {
                    Process p1 = p[i];
                    endCPUUsage += p1.TotalProcessorTime;
                }
                catch (Exception ex)
                {

                }
            }
            var cpuUsedMs = (endCPUUsage - startCPUUsage).TotalMilliseconds;

            var totalMsPassed = (endTime - startTime).TotalMilliseconds;

            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

            var cpuUsagePercentage = cpuUsageTotal * 100;
            return cpuUsagePercentage;
        }
        
        private static long nLastPerc = 0;
        private static string sProcData = "";
        public static string GetPerformanceReport()
        {
            long nElapsed = UnixTimestamp() - nLastPerc;
            if (nElapsed < (60 * 60 * 2))
            {
                return sProcData;
            }
            nLastPerc = UnixTimestamp();
            int nProcCount = Environment.ProcessorCount;
            double nHashes2 = 0;
            nHashes2 = ProcSpeedTest(7, 8);
            double nProcUtiliz = GetCPUUsage();
            sProcData = "PROCS=" + nProcCount.ToString() + "|HASHES=" + nHashes2.ToString() + "|DISKSIZE=" + GetDiskSizeTB().ToString() + "|VIDEOFOLDER="
                + GetRootFolderSize().ToString() + "|CPUUSAGE=" + nProcUtiliz.ToString();

            return sProcData;
        }

    }
}
