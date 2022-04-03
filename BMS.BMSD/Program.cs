using BiblePay.BMS;
using MySql.Data.MySqlClient;
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
            Console.WriteLine("Welcome to the BiblePay BMS System v1.7");
            Console.WriteLine("1. D-DOS ATTACK");
            Console.WriteLine("3. DNS Test");
            Console.WriteLine("4. Test Case 1");
            Console.WriteLine("F1. IPFS Test case 1");
            Console.WriteLine("F4. Test Case 3");
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
                        TestCase1();
                        break;
                    case ConsoleKey.F5:
                        break;
                    case ConsoleKey.Escape:
                        System.Environment.Exit(0);
                        break;
                    case ConsoleKey.D1:
                        bool f = await DDOS();
                        break;
                    case ConsoleKey.D4:
                        TestCase2();
                        break;
                    case ConsoleKey.D2:
                        break;

                }
            }catch(Exception ex)
            {
                Common.Log("HandleKB::" + ex.Message);
                
            }
        }


        private static void IndividualAttack()
        {
            MyWebClient wc = new MyWebClient();

            for (int i = 0; i < 2000; i++)
            {
                string sURL = sAttackURL + i.ToString() + ".ts";
                try
                {
                    string sTest = wc.DownloadString(sURL);
                    nAttackBytes += sTest.Length;
                    nAttacks++;
                    if (sTest.Length > 1000)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            sTest = wc.DownloadString(sURL);
                            nAttackBytes += sTest.Length;
                            nAttacks++;

                        }
                    }
                }
                catch (Exception ex)
                {
                }
                if (!fAttacking)
                    break;
            }
        }


        private static async void TestCaseF5()
        {
            Environment.Exit(0);
        }
        private static long nAttackBytes = 0;
        private static int nAttacks = 0;
        private static string sAttackURL = "";
        private static bool fAttacking = false;
        private static async Task<bool> DDOS()
        {
            Console.WriteLine("!WARNING! DO NOT USE THIS TOOL FOR ANY OTHER PURPOSE THAN THE INTENDED TEST CASE!");
            Console.WriteLine("Enter the hostname to attack >");
            string sAddress = Console.ReadLine();
            //https://sanc1.cdn.biblepay.org:5000/video/175c2642fba5eae7f74f1554f2794507ab26c92f705398db658cee3b9b689aa5/1.m3u8
            Console.WriteLine("Starting Attack...");
            string sResource = "video/175c2642fba5eae7f74f1554f2794507ab26c92f705398db658cee3b9b689aa5/";
            double nStartTime = Common.UnixTimestamp();
            double nElapsed = 0;
            fAttacking = true;
            nAttackBytes = 0;
            nAttacks = 0;
            double nAttackDuration = 300;
            sAttackURL = Common.GetCDN() + "/" + sResource;
            int nPass = 0;
            int nMaxThreads = 100;
            int nThreadCount = 0;

            while (true)
            {
                if (nThreadCount < nMaxThreads)
                {
                    System.Threading.Thread t = new System.Threading.Thread(IndividualAttack);
                    t.Start();
                    nThreadCount++;
                }

                nElapsed = Common.UnixTimestamp() - nStartTime;
                nPass++;
                if (nPass % 4 == 0)
                {
                    Console.WriteLine(nPass.ToString() + "," + nAttacks.ToString() + ", " + nAttackBytes.ToString());
                }
                System.Threading.Thread.Sleep(5);

                if (nElapsed > nAttackDuration)
                    break;

            }
            fAttacking = false;
            Console.WriteLine("Attack finished...");

            return true;
        }

        private static async void TestCase2()
        {
        }

        private static void TestCase1()
        {
            double nHashes = 0, nHashes2 = 0;
            if (false)
            {
                nHashes = BiblePay.BMS.DSQL.modLegacyCryptography.ProcSpeedTest(15, 1);
                nHashes2 = BiblePay.BMS.DSQL.modLegacyCryptography.ProcSpeedTest(15, 8);
            }
            int nProcCount = Environment.ProcessorCount;
            double nFree = BiblePay.BMS.DSQL.modLegacyCryptography.GetDiskSizeTB();
            double nSz = BiblePay.BMS.DSQL.modLegacyCryptography.GetFreeDiskSpacePercentage();
            string sData = "Speed : " + nHashes.ToString() + ", " + nHashes2.ToString() + ", proccount: " + nProcCount.ToString() + ", nfree: " + nFree.ToString();
            Console.WriteLine(sData);
            Common.Log(sData);
        }
        public static async Task MainAsync(string[] args)
        {
            try
            {
                // Upgrade
                System.Threading.Thread.Sleep(1000);  //Wait for upgrader to die off
                string sBindURL = Common.GetConfigurationKeyValue("bindurl");
                if (sBindURL=="")
                {
                    Console.WriteLine("This node does not have a configuration file.  Please nano /inetpub/wwwroot/bms/bms.conf");
                    System.Environment.Exit(0);
                }
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    Upgrade u = new Upgrade();
                }


                try
                {
                    BiblePay.BMS.Program.Main(null);
                }
                catch(Exception ex5)
                {
                    Common.Log(ex5.Message);
                }
                

                while (1 == 1)
                {
                    try
                    {
                        System.Threading.Thread.Sleep(1000);
                        HandleKeyboard();
                    }
                    catch(Exception ex2)
                    {
                        Common.Log(ex2.Message);
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

/*
 * 
 * REQUIREMENTS:

  */

