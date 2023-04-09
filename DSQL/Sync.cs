using BMSCommon;
using BMSCommon.Model;
using System;
using System.Collections.Generic;
using static BMSCommon.Common;

namespace BiblePay.BMS.DSQL
{
    public class Sync
    {

        
        // BWS gets created, and runs one free running Syncer thread

        public static void xSyncer(object oMyURL)
        {
            // Primary entry point for services

            while (1 == 1)
            {
                try
                {
                    //await BBPAPI.Service.BackgroundAngel(BMSCommon.API.GetCDN());
                    //await BiblePay.BMS.DSQL.PB.DailyUTXOExport(false, BMSCommon.Common.IsPrimary());
                    System.Threading.Thread.Sleep(300000);
                }
                catch (Exception ex2)
                {
                    Log("Syncer::Caught a crash::" + ex2.Message);
                    System.Threading.Thread.Sleep(60000);
                }
            }
        }

        public static int nLoopCount = 0;
        public static long METRIC_FILECOUNT = 0;
        public static long METRIC_SYNCED_COUNT = 0;
        
        public struct NickName
        {
            public string ID;
            public string nickName;
        };

        public static List<NickName> _nicknames = new List<NickName>();
        // PlaceHolder for bbp.click/tinyurl (access file by your user nickname/tinyurl)

    }
}

