using BMSCommon;
using BMSShared;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Text;
using static BMSCommon.BBPCharting;
using static BMSCommon.Common;
using BMSCommon.Model;

namespace BBPAPI
{
    public static class PricingService
    {
        private static string msTickers = "BTC/USD,DASH/BTC,DOGE/BTC,LTC/BTC,ETH/BTC,XRP/BTC,XLM/BTC,BBP/BTC,ZEC/BTC,BCH/BTC";
        private static string msWeights = "1,185,130000,185,15,35000,125000,45000000,210,50";

        private static double PricingBigIntToDouble(BigInteger bi, int nDecimals)
        {
            BigInteger nDivisor = 1;
            if (nDecimals == 18)
            {
                nDivisor = 1000000000000000000;
            }
            else if (nDecimals == 10)
            {
                nDivisor = 10000000000;
            }
            else if (nDecimals == 8)
            {
                nDivisor = 100000000;
            }
            else
            {
                throw new Exception("Divisor unknown");
            }

            BigInteger divided = BigInteger.Divide(bi, nDivisor);
            if (divided < 1000)
            {
                decimal nNew = (decimal)bi;
                double nOut = (double)(nNew / (decimal)GetDouble(nDivisor.ToString()));
                return nOut;
            }
            else
            {
                double nBal = GetDouble(divided.ToString());
                return nBal;
            }
        }

        /*
        public static string GetChartOfIndex()
        {
            BBPChart b = new BBPChart
            {
                Name = "BiblePay Weighted CryptoCurrency Index",
                Type = "date"
            };

            string[] vTickers = BBPAPI.PricingService.msTickers.Split(",");
            string[] vWeights = PricingService.msWeights.Split(",");
           
            var sql1 = "Select added,USD from bbp.quotehistory where ticker='IndexValue'";
            var dPrices = new Dictionary<DateTime, double>();

            NpgsqlCommand m2 = new NpgsqlCommand(sql1);
            DataTable dt2 = DB.GetDataTable(m2);
            for (int j = 0; j < dt2.Rows.Count; j++)
            {
                DateTime dt = Convert.ToDateTime(dt2.Rows[j]["added"]);
                double nPrice = GetDouble(dt2.Rows[j]["USD"]);
                dPrices.Add(dt, nPrice);
            }

            //Index
            ChartSeries sIndex = new ChartSeries
            {
                Name = b.Name,
                BorderColor = "lime",
                BackgroundColor = "green"
            };
            b.CollectionSeries.Add(sIndex);
            double nPrice2 = 0;

            //Convert to opensource version: https://www.w3schools.com/ai/ai_chartjs.asp
            int iStep = 1;
            DateTime dtStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            for (int i = 180; i > 1; i = i - iStep)
            {
                DateTime dt = dtStart.AddDays(-1 * i);

                long nTimestamp = DateToUnixTimestamp(dt);

                b.XAxis.Add(nTimestamp);

               

                bool fGot = dPrices.TryGetValue(dt, out nPrice2);
                if (nPrice2 == 0)
                {
                    // This is a base level for the cryptocurrency homogenized index.  This doesn't get hit after chart is 60 days old.
                    nPrice2 = 15000;
                }

                b.CollectionSeries[0].DataPoint.Add(nPrice2);

            }

            string html = GenerateJavascriptChart(b);
            return html;
        }
        */



        public static double ConvertUSDToBiblePay(double nUSD)
        {
            price1 nBTCPrice = GetCryptoPrice("BTC/USD");
            price1 nBBPPrice = GetCryptoPrice("BBP/BTC");
            double nUSDBBP = nBTCPrice.AmountUSD * nBBPPrice.Amount;
            double nOut = nUSD / (nUSDBBP + .000000001);
            return nOut;
        }

        private static price1 _nBTCPrice = GetCryptoPrice("BTC/USD");
        private static price1 _nBBPPrice = GetCryptoPrice("BBP/BTC");

        public static double ConvertUSDToBiblePayWithCache(double nUSD)
        {
            //price1 nBTCPrice = GetCryptoPrice("BTC/USD");
            //price1 nBBPPrice = GetCryptoPrice("BBP/BTC");
            double nUSDBBP = _nBTCPrice.AmountUSD * _nBBPPrice.Amount;
            double nOut = nUSD / (nUSDBBP + .000000001);
            return nOut;
        }


        internal static string TickerToName(string sTicker)
        {
            if (sTicker == "DOGE")
            {
                return "dogecoin";
            }
            else if (sTicker == "BTC")
            {
                return "bitcoin";
            }
            else if (sTicker == "DASH")
            {
                return "dash";
            }
            else if (sTicker == "LTC")
            {
                return "litecoin";
            }
            else if (sTicker == "XRP")
            {
                return "ripple";
            }
            else if (sTicker == "XLM")
            {
                return "stellar";
            }
            else if (sTicker == "BCH")
            {
                return "bitcoin-cash";
            }
            else if (sTicker == "ZEC")
            {
                return "zcash";
            }
            else if (sTicker == "ETH")
            {
                return "ethereum";
            }
            Log("Ticker mapping missing " + sTicker);
            return sTicker;
        }
        public static BMSCommon.Model.price1 GetCryptoPrice(string sTicker)
        {
            price1 p = new price1();
            p.Amount = BBPAPI.Interface.Core.GetPriceQuote(sTicker);
            double dUSDCryptoPrice = BBPAPI.Interface.Core.GetPriceQuote("BTC/USD");
            p.AmountUSD = dUSDCryptoPrice * p.Amount;
            p.Ticker = sTicker.ToUpper();
            if (sTicker.ToUpper() == "BTC" || sTicker == "BTC/BTC" || sTicker == "BTC/USD" || sTicker == "BTC/USDT")
            {
                p.AmountUSD = dUSDCryptoPrice;
            }
            return p;
        }


        
    }
}
