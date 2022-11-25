using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace BiblePay.BMSD
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the BiblePay BMS System v2.5");
            Console.WriteLine("1. PDF");
            Console.WriteLine("F4. Reserved");
            Console.WriteLine("<ESC>.  Exit Program");
            MainAsync(args).Wait();
            Environment.Exit(0);
        }

        public static async void HandleKeyboard()
        {
            try
            {
                ConsoleKey K = Console.ReadKey().Key;
                BiblePay.BMSD.DataLoader d = new DataLoader();
                switch (K)
                {
                    case ConsoleKey.F1:
                        break;
                    case ConsoleKey.F3:
                        break;
                    case ConsoleKey.F4:

                        break;
                    case ConsoleKey.F5:
                        break;
                    case ConsoleKey.Escape:
                        System.Environment.Exit(0);
                        break;
                    case ConsoleKey.D1:

                        break;
                    case ConsoleKey.D4:
                        break;
                    case ConsoleKey.D2:

                        break;

                }
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("/output error"))
                {
                    Common.Log("HandleKB::" + ex.Message);
                }

            }
        }


        private static void UpgNode()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                bool fNeeds = Upgrade.UpgradeNode(false);
                if (fNeeds)
                {
                    string sNarr = "Node needs upgraded: " + fNeeds.ToString();
                    Console.WriteLine(sNarr);
                    Upgrade.UpgradeNode(true);
                }
            }
        }
        private static int nLastUpgCheck = 0;

        public static void BackgroundThread()
        {
            Common.Log("BMSD is starting up...");

            while (1 == 1)
            {
                try
                {
                    System.Threading.Thread.Sleep(1000);
                    int nElapsed = Common.UnixTimestamp() - nLastUpgCheck;
                    if (nElapsed > 60 * 5)
                    {
                        nLastUpgCheck = Common.UnixTimestamp();
                        //Common.Log("Checking for upgrade...");
                        UpgNode();
                    }
                }
                catch (Exception ex2)
                {
                    Common.Log("2::" + ex2.Message);
                }
            }

        }
        public static async Task MainAsync(string[] args)
        {
            try
            {
                // Upgrade
                System.Threading.Thread.Sleep(1000);  //Wait for upgrader to die off
                if (false)
                {
                    Console.WriteLine("This node does not have a configuration file.  Please nano /inetpub/wwwroot/bms/bms.conf");
                    System.Environment.Exit(0);
                }

                try
                {
                    Upgrade.StartNewWebServer();
                }
                catch(Exception ex5)
                {
                    Common.Log(ex5.Message);
                }

                System.Threading.Thread t = new System.Threading.Thread(BackgroundThread);
                t.Start();

                while (1 == 1)
                {
                    try
                    {
                        System.Threading.Thread.Sleep(1000);
                        HandleKeyboard();
                    }
                    catch(Exception ex2)
                    {
                        Common.Log("12311::"+ex2.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.Message);
            }
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Exiting...");
        }
    }
}

