using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Threading.Tasks;
using static BMSCommon.Model;

namespace BMSCommon
{
	public static class Pricing
    {
		public static string msTickers = "BTC/USD,DASH/BTC,DOGE/BTC,LTC/BTC,ETH/BTC,XRP/BTC,XLM/BTC,BBP/BTC,ZEC/BTC,BCH/BTC";
		public static string msWeights = "1,185,130000,185,15,35000,125000,45000000,210,50";

	

		public static double PricingBigIntToDouble(BigInteger bi, int nDecimals)
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
				double nOut = (double)(nNew / (decimal)Common.GetDouble(nDivisor.ToString()));
				return nOut;
			}
			else
			{
				double nBal = Common.GetDouble(divided.ToString());
				return nBal;
			}
		}
		
		


		public static void StorePriceHistory(string sTicker, double sUSDValue, double sBTCValue, DateTime theDate)
		{
			string added = theDate.ToString("yyyy-MM-dd");
			string sql = "Delete from quotehistory where ticker=@ticker and added='" + added + "';";
			MySqlCommand cmd1 = new MySqlCommand(sql);
			cmd1.Parameters.AddWithValue("@ticker", sTicker);
			bool f1 = Database.ExecuteNonQuery2(cmd1);
			sql = "Insert Into quotehistory (id,added,ticker,usd,btc) values (uuid(),'"
				+ added + "',@ticker,'" + sUSDValue.ToString() + "','" + sBTCValue.ToString() + "');";
			MySqlCommand cmd2 = new MySqlCommand(sql);
			cmd2.Parameters.AddWithValue("@ticker", sTicker);

			bool f2 = Database.ExecuteNonQuery2(cmd2);

			if (sUSDValue < .01 && sTicker != "BBP")
			{
				Common.Log("Low quote " + sTicker + sUSDValue.ToString() + "," + sBTCValue.ToString());
			}
		}
		// Once per day we will store the historical quotes, to build the cryptocurrency index chart
		public static void StorePriceQuotes(int offset)
		{
			try
			{
				DateTime theDate = DateTime.Now;
				if (offset < 0)
				{
					theDate = DateTime.Now.Subtract(TimeSpan.FromDays(offset * -1));
				}
				string[] vTickers = msTickers.Split(",");
				string[] vWeights = msWeights.Split(",");
				double dTotalIndex = 0;
				double nBTCUSD = GetPriceQuote("BTC/USD");
				for (int i = 0; i < vTickers.Length; i++)
				{
					double nQuote = GetPriceQuote(vTickers[i]);
					double nUSDValue = 0;
					if (vTickers[i] != "BTC/USD")
					{
						nUSDValue = nBTCUSD * nQuote;
					}
					else
					{
						nUSDValue = nQuote;
					}
					double dWeight = Common.GetDouble(Common.GE(vWeights[i], ",", 0));
					dTotalIndex += dWeight * nUSDValue;
					string sTicker = Common.GE(vTickers[i], "/", 0);
					StorePriceHistory(sTicker, nUSDValue, nQuote, theDate);
				}
				double dIndexValue = dTotalIndex / vTickers.Length;
				StorePriceHistory("IndexValue", dIndexValue, dIndexValue, theDate);
			}
			catch (Exception ex)
			{
				Common.Log("Store Quote History:" + ex.Message);
			}
		}


		public static void CacheQuote(string sTicker, string sPrice)
		{
			try
			{
				string sql = "Delete from sys where SystemKey=@ticker;";
				MySqlCommand cmd1 = new MySqlCommand(sql);
				cmd1.Parameters.AddWithValue("@ticker", sTicker);
				Database.ExecuteNonQuery2(cmd1);
				sql = "Insert into sys (id,systemkey,value,updated) values (uuid(),@ticker,'" + sPrice + "',now())";
				MySqlCommand cmd2 = new MySqlCommand(sql);
				cmd2.Parameters.AddWithValue("@ticker", sTicker);
				bool fSuccess = Database.ExecuteNonQuery2(cmd2);
				bool f100 = false;

			}
			catch(Exception ex)
            {
				bool f999 = false;
            }
		}

		public static double GetCachedQuote(string sTicker, out int age)
		{
			age = 0;
			if (sTicker == null)
				return 0;
			string sql = "Select updated,Value from sys where systemkey=@ticker;";
			MySqlCommand cmd1 = new MySqlCommand(sql);
			cmd1.Parameters.AddWithValue("@ticker", sTicker);
			DataTable dt = Database.GetDataTable2(cmd1);

			if (dt.Rows.Count < 1)
			{
				return 0;
			}
			double d1 = Common.GetDouble(dt.Rows[0]["Value"]);
			string s1 = dt.Rows[0]["Updated"].ToString();
			TimeSpan vTime = DateTime.Now - Convert.ToDateTime(s1);
			age = (int)vTime.TotalSeconds;
			return d1;
		}


		public static string TickerToName(string sTicker)
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
			Common.Log("Ticker mapping missing " + sTicker);
			return sTicker;
		}

		public static double GetPriceQuote(string ticker, int nAssessmentType = 0)
		{
			string sData1 = "";
			double dCachedQuote = 0;
			if (ticker == "BTC/USD")
			{
				ticker = "BTC/USDT";//SX has moved from USD to USDT
			}

			try
			{
				int age = 0;
				if (ticker=="BTC/BTC")
                {
					return 0;
                }
				dCachedQuote = GetCachedQuote(ticker, out age);
				if (dCachedQuote > 0 && age < (60 * 60 * 4))
					return dCachedQuote;

				string[] vTicker = ticker.Split("/");
				string LeftTicker = "";
				if (vTicker.Length == 2)
				{
					LeftTicker = vTicker[0];
				}
				if (LeftTicker == "XRP" || LeftTicker == "XLM" || LeftTicker == "BCH" || LeftTicker == "ZEC")
				{
					string sCoinName = TickerToName(LeftTicker);
					string sKey = BMSCommon.Encryption.DecryptAES256("ZtEciCL5O3gSru+1VvKpzppMuAflYzPkE4pZ8dz+F41U52tSupSEG8ldJKgRI/rw", "");

					string sURL1 = "https://api.blockchair.com/" + sCoinName + "/stats?key=" + sKey;
					sData1 = Common.ExecuteMVCCommand(sURL1);
					dynamic oJson =  JsonConvert.DeserializeObject<dynamic>(sData1);
					if (oJson != null)
					{
						double nMyValue = oJson["data"]["market_price_btc"].Value ?? 0;
						if (nMyValue > 0)
						{
							CacheQuote(ticker, nMyValue.ToString("0." + new string('#', 339)));
						}
						if (nMyValue == 0)
						{
							Common.Log("For some reason my quote is very low for " + LeftTicker + ", " + sData1 + ": " + nMyValue.ToString());
						}
						return nMyValue;
					}
                    else
                    {
						Common.Log("For some reason my quote is very low for " + LeftTicker + ", " + sData1 + ": ");
						return 0;

					}
				}

				string sURL = "https://www.southxchange.com/api/price/" + ticker;
				string sData = "";

				sData = BMSCommon.Common.ExecuteMVCCommand(sURL);
				string bid = Common.ExtractXML(sData, "Bid\":", ",").ToString();
				string ask = Common.ExtractXML(sData, "Ask\":", ",").ToString();
				double dbid = Common.GetDouble(bid);
				double dask = Common.GetDouble(ask);
				double dTotal = dbid + dask;
				double dmid = dTotal / 2;
				if (nAssessmentType == 1)
					dmid = dbid;
				if (dmid > 0)
				{
					CacheQuote(ticker, dmid.ToString("0." + new string('#', 339)));
				}
				else
				{
					return dCachedQuote;
				}
				return dmid;
			}
			catch (Exception ex)
			{
				Common.Log("Bad Pricing error " + ex.Message + " " + sData1);
				return dCachedQuote;
			}
		}



	public static price1 GetCryptoPrice(string sTicker)
	{
		price1 p = new price1();
		p.Amount = GetPriceQuote(sTicker + "/BTC", 0);
   	    double dUSDCryptoPrice = GetPriceQuote("BTC/USD");
		p.AmountUSD = dUSDCryptoPrice * p.Amount;
		p.Ticker = sTicker.ToUpper();
		if (sTicker.ToUpper() == "BTC")
			p.AmountUSD = dUSDCryptoPrice;
		return p;
	}


	}
}
