using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;

namespace BiblePay.BMSD
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to BMSD v2.2");
            MainAsync(args);
            Environment.Exit(0);
        }

        public static void HandleKeyboard()
        {
            try
            {
                //ConsoleKey K = Console.ReadKey().Key;
                
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("/output error"))
                {
                    Common.Log("HandleKB::" + ex.Message);
                }

            }
        }

        public static bool MainAsync(string[] args)
        {
            try
            {
                Common.Log("VERSION::BMSD v6.01 is starting up...");

                bool fStarted = Upgrade.StartInternalPort();
                // If this doesnt work, we are running a duplicate copy.
                if (!fStarted)
                {
                    Common.Log("A duplicate copy is running, or port 7999 is listening already by another process..");
					Upgrade.NotifyIPC("A duplicate copy of BMS is running; unable to start.");

					Environment.Exit(0);
                    return false;
                }
                //bool fListening = Upgrade.IsPortOpen("127.0.0.1", 7999);
                //fListening = Upgrade.IsPortOpen("127.0.0.1", 7999);
                Common.Log("Testing for upgrade...");
                bool fUpgraded = Upgrade.UpgradeNode(true).Result;
                Common.Log("Starting web server...");
                Upgrade.StartNewWebServer(ProcessWindowStyle.Hidden);
            }
            catch (Exception ex)
            {
                Upgrade.NotifyIPC("There was a problem initializing the node::" + ex.Message);
                Common.Log("There was a problem initializing the node. Details: '" + ex.Message + "'");
            }
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Exiting...");
            return true;
        }
    }
}

