using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BBPAPI
{
    public static class Bash
    {
        public static async Task<bool> ProcessCommandType1(string sCommand)
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
            
            await process.StandardInput.WriteLineAsync(sCommand);
            //process.WaitForExit();
            //var output = await process.StandardOutput.ReadLineAsync();
            //Console.WriteLine(output);
            System.Threading.Thread.Sleep(3000);
            //Console.WriteLine(output);
            //return output;
            return true;
        }
        public static async Task<string> ProcessCommandType2(string sCommand, bool fBackground)
        {
            try
            {
                string sFN = Guid.NewGuid().ToString() + ".dat";
                string sPath = Path.Combine(System.IO.Path.GetTempPath(), sFN);
                string sFullCommand = sCommand + ">" + sPath;
                if (fBackground)
                {
                    sFullCommand += " &";
                }
                Console.WriteLine(sFullCommand);
                await ProcessCommandType1(sFullCommand);
                string sData = System.IO.File.ReadAllText(sPath);
                Console.WriteLine(sData);
                System.IO.File.Delete(sPath);
                return sData;
            }catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
