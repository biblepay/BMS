using LightningDB;
using MySql.Data.MySqlClient;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BMSCommon
{
	public static class Pricing
    {
		public static string msTickers = "BTC/USD,DASH/BTC,DOGE/BTC,LTC/BTC,ETH/BTC,XRP/BTC,XLM/BTC,BBP/BTC,ZEC/BTC,BCH/BTC";
		public static string msWeights = "1,185,130000,185,15,35000,125000,45000000,210,50";

		public struct Asset
		{
			public string ERCAddress;
			public string Symbol;
			public string Chain;
			public double Price;
			public double Amount;
			public string ChainlinkAddress;
		};

		


		public static double BigIntToDouble(BigInteger bi, int nDecimals)
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
		public static List<Asset> GetAssetList()
		{
			List<Asset> l1 = new List<Asset>();
			// Layer 1 ERC-20 Assets:
			l1.Add(new Asset { Chain = "POLYGON", Symbol = "MATIC", ERCAddress = "0x0", ChainlinkAddress = "0xAB594600376Ec9fD91F8e885dADF0CE036862dE0", Price = 0 });
			l1.Add(new Asset { Chain = "BSC", Symbol = "BSC", ERCAddress = "0x0", ChainlinkAddress = "0x0567F2323251f0Aab15c8dFb1967E4e8A7D42aeE", Price = 0 });
			l1.Add(new Asset { Chain = "POLYGON", Symbol = "WETH", ERCAddress = "0x7ceb23fd6bc0add59e62ac25578270cff1b9f619", ChainlinkAddress = "0xF9680D99D6C9589e2a93a78A04A279e509205945", Price = 0 });
			// Layer 2 ERC-20 Assets:
			l1.Add(new Asset { Chain = "BSC", Symbol = "CAKE", ERCAddress = "0x0e09fabb73bd3ade0a17ecc321fd13a19e81ce82", ChainlinkAddress = "0xB6064eD41d4f67e353768aA239cA86f4F73665a1", Price = 0 });
			l1.Add(new Asset { Chain = "BSC", Symbol = "FIELD", ERCAddress = "0x04d50c032f16a25d1449ef04d893e95bcc54d747", Price = .003 });
			l1.Add(new Asset { Chain = "ETH", Symbol = "SHIB", ERCAddress = "0x95ad61b0a150d79219dcf64e1e6cc01f0b64c4ce", Price = .00001081 });
			l1.Add(new Asset { Chain = "POLYGON", Symbol = "SHIB", ERCAddress = "0x6f8a06447ff6fcf75d803135a7de15ce88c1d4ec", Price = .00001081 });

			// Wrapped ERC-20 Assets:
			l1.Add(new Asset { Chain = "BSC", Symbol = "WBBP", ERCAddress = "0xcb1eec8630c5176611f72799853c3b7dbe4b8953", Price = 0 });
			l1.Add(new Asset { Chain = "POLYGON", Symbol = "renDOGE", ERCAddress = "0xcE829A89d4A55a63418bcC43F00145adef0eDB8E", ChainlinkAddress = "0xbaf9327b6564454F4a3364C33eFeEf032b4b4444", Price = 0 });
			// Native Non ERC-20 Layer 1 Assets:
			l1.Add(new Asset { Chain = "DOGE", Symbol = "DOGE", ERCAddress = "", Price = 0 });
			l1.Add(new Asset { Chain = "BITCOIN", Symbol = "BTC", ERCAddress = "", Price = 0 });
			l1.Add(new Asset { Chain = "DASH", Symbol = "DASH", ERCAddress = "", Price = 0 });
			return l1;
		}
		

		public class BBPChart
        {
			public string Name;
			public List<ChartSeries> CollectionSeries = new List<ChartSeries>();
			public List<double> XAxis = new List<double>();
			public string Type;
        }
		public class ChartSeries
        {
			public string Name;
			public string BorderColor;
			public string BackgroundColor;
			public bool Fill;
			public List<double> DataPoint = new List<double>();
        };

		public static string GenerateJavascriptChart(BBPChart c)
        {
			string html = "<script src='https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.9.4/Chart.js'></script>";
			html = "<script src='https://cdn.jsdelivr.net/npm/chart.js@2.9.4/dist/Chart.min.js'></script>";
			html += "<script src='https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.1/moment.min.js'></script>";

			string sID = Guid.NewGuid().ToString();

			html += "\r\n<canvas id='" + sID + "' style='width:100%;max-width:900px'></canvas>";
			html += "\r\n<script>\r\n";
			
			string xLegends = "var xLegends = [";
			string xdata = "";
			for (int i = 0; i < c.XAxis.Count; i++)
            {
				if (c.Type == "date")
				{
					DateTime dtPoint = BMSCommon.Common.FromUnixTimeStamp((int)c.XAxis[i]);
					string myChartPoint = dtPoint.ToString("s") + "Z";
					xdata += "'" + myChartPoint + "', ";
				
				}
				else
				{
					xdata += c.XAxis[i].ToString() + ", ";
				}
            }
			if (xdata.Length > 2)
				xdata = xdata.Substring(0, xdata.Length - 2);

			xLegends += xdata + "];\r\n";

			html += xLegends;
			//options: { plugins: { title: {display: true,text:'my title'}
			string sDisplayFormat = "type: 'time',	time:	{ displayFormats: {					'millisecond': 'MMM DD',            'second': 'MMM DD',            'minute': 'MMM DD',"
				+"            'hour': 'MMM DD',            'day': 'MMM DD',            'week': 'MMM DD',            'month': 'MMM DD',            'quarter': 'MMM DD',            'year': 'MMM DD',"
				+"			} }";
			
			sDisplayFormat = "";
			if (c.Type == "xdate")
            {
				sDisplayFormat = "type: 'time',";
            }

			//ticks: { autoSkip:true, maxTicksLimit:15 }}]
			string chartOptions = "options: {	"
				+ "scales: { xAxes:  [{ " + sDisplayFormat + "  }]  }}, ";
			html += "\r\n new Chart('" + sID + "', {   type: 'line', " + chartOptions + " data: { 	labels: xLegends,"
				 + " datasets: @ds } } );";


			string seriesData = "";
			for (int j = 0; j < c.CollectionSeries.Count; j++)
			{
				ChartSeries c1 = c.CollectionSeries[j];
				string dp = "";
				for (int k = 0; k < c1.DataPoint.Count;k++)
                {
					if (c.Type == "xdate")
					{
						DateTime dtPoint = BMSCommon.Common.FromUnixTimeStamp((int)c.XAxis[k]);
						string myChartPoint = "'" + dtPoint.ToString("s") + "Z" + "'";

						dp += "{ t: " + myChartPoint + ", y: " + c1.DataPoint[k].ToString() + "}, ";

					}
					else
					{
						dp += c1.DataPoint[k].ToString() + ", ";
					}
                }
				if (dp.Length > 2)
					dp = dp.Substring(0, dp.Length - 2);
				dp += "\r\n";

				seriesData += "[{ label: '" + c1.Name + "', \r\ndata: [" + dp + "], borderColor: '" + c1.BorderColor.ToString() + "',backgroundColor:'" 
					+ c1.BackgroundColor.ToString() + "',fill: true}]";

			}
			html = html.Replace("@ds", seriesData);
			html += "\r\n</script>";
			return html;
        }


		public static string GetChartOfIndex()
		{
			BBPChart b = new BBPChart();

			b.Name = "BiblePay Weighted CryptoCurrency Index";
			b.Type = "date";

			string[] vTickers = msTickers.Split(",");
			string[] vWeights = msWeights.Split(",");
			bool fUseIndividualCryptos = false;
			if (fUseIndividualCryptos)
			{
				for (int k = 0; k < vTickers.Length; k++)
				{
					string sTheTicker = Common.GE(vTickers[k], "/", 0);
					ChartSeries c1 = new ChartSeries();
					c1.Name = sTheTicker;
					c1.DataPoint = new List<double>();
					// s.ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.StackedArea;
					b.CollectionSeries.Add(c1);
				}
			}

			string sql1 = "Select added,USD from quotehistory where ticker='IndexValue'";
			Dictionary<DateTime, double> dPrices = new Dictionary<DateTime, double>();

			MySqlCommand m2 = new MySqlCommand(sql1);
			DataTable dt2 = Database.GetDataTable(m2);
			for (int j = 0; j < dt2.Rows.Count; j++)
            {
				DateTime dt = Convert.ToDateTime(dt2.Rows[j]["added"]);
				double nPrice = BMSCommon.Common.GetDouble(dt2.Rows[j]["USD"]);
				dPrices.Add(dt, nPrice);
            }

			//Index
			ChartSeries sIndex = new ChartSeries();
			sIndex.Name = b.Name;
			sIndex.BorderColor = "lime";
			sIndex.BackgroundColor = "green";
			b.CollectionSeries.Add(sIndex);
			double nPrice2 = 0;

			//Convert to opensource version: https://www.w3schools.com/ai/ai_chartjs.asp
			int iStep = 1;
			DateTime dtStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

			for (int i = 180; i > 1; i = i - iStep)
			{
				DateTime dt = dtStart.AddDays(-1 * i);

				long nTimestamp = BMSCommon.Common.DateToUnixTimestamp(dt);

				b.XAxis.Add(nTimestamp);


				if (fUseIndividualCryptos)
				{
					for (int j = 0; j < vTickers.Length; j++)
					{
						string sTheTicker = Common.GE(vTickers[j], "/", 0);
						string sql = "Select * from quotehistory where added='" + dt.ToShortDateString() + "' and ticker='" + sTheTicker+ "'";
						MySqlCommand m1 = new MySqlCommand(sql);
						DataTable dt1 = Database.GetDataTable(m1);
						if (dt1.Rows.Count > 0)
						{
							double dA = Common.GetDouble(dt1.Rows[0]["USD"]);
							if (fUseIndividualCryptos)
							{
								double dWeight = Common.GetDouble(Common.GE(vWeights[j], ",", 0));
								double dAdj = dWeight * dA;
								b.CollectionSeries[j].DataPoint.Add(dAdj);
							}
						}
					}
				}

				
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

		public static void StoreHistory(string sTicker, double sUSDValue, double sBTCValue, DateTime theDate)
		{
			string added = theDate.ToString("yyyy-MM-dd");
			string sql = "Delete from quotehistory where ticker=@ticker and added='" + added + "';";
			MySqlCommand cmd1 = new MySqlCommand(sql);
			cmd1.Parameters.AddWithValue("@ticker", sTicker);
			bool f1 = Database.ExecuteNonQuery(false, cmd1, "");
			sql = "Insert Into quotehistory (id,added,ticker,usd,btc) values (uuid(),'"
				+ added + "',@ticker,'" + sUSDValue.ToString() + "','" + sBTCValue.ToString() + "');";
			MySqlCommand cmd2 = new MySqlCommand(sql);
			cmd2.Parameters.AddWithValue("@ticker", sTicker);

			bool f2 = Database.ExecuteNonQuery(false, cmd2, "");

			if (sUSDValue < .01 && sTicker != "BBP")
			{
				Common.Log("Low quote " + sTicker + sUSDValue.ToString() + "," + sBTCValue.ToString());
			}
		}
		// Once per day we will store the historical quotes, to build the cryptocurrency index chart
		public static void StoreQuotes(int offset)
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
					StoreHistory(sTicker, nUSDValue, nQuote, theDate);
				}
				double dIndexValue = dTotalIndex / vTickers.Length;
				StoreHistory("IndexValue", dIndexValue, dIndexValue, theDate);
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

				Database.ExecuteNonQuery(false, cmd1, "");
				sql = "Insert into sys (id,systemkey,value,updated) values (uuid(),@ticker,'" + sPrice + "',now())";
				MySqlCommand cmd2 = new MySqlCommand(sql);
				cmd2.Parameters.AddWithValue("@ticker", sTicker);
				bool fSuccess = 			Database.ExecuteNonQuery(false, cmd2, "");
				bool f100 = false;

			}catch(Exception ex)
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
			DataTable dt = Database.GetDataTable(cmd1);

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

		public static double GetKeyDouble(string sKey, int nMaxSeconds)
        {
			double nValue = BMSCommon.Common.GetDouble(GetKeyValue(sKey, nMaxSeconds));
			return nValue;
        }
		public static string GetKeyValue(string sKey, int nMaxSeconds)
		{
			double age = 0;
			if (sKey == null)
				return "";
			string sql = "Select updated,Value from sys where systemkey=@skey;";
			MySqlCommand cmd1 = new MySqlCommand(sql);
			cmd1.Parameters.AddWithValue("@skey", sKey);
			DataTable dt = Database.GetDataTable(cmd1);
			if (dt.Rows.Count < 1)
			{
				return "";
			}
			string sValue = dt.Rows[0]["Value"].ToString();
			string s1 = dt.Rows[0]["Updated"].ToString();
			TimeSpan vTime = DateTime.Now - Convert.ToDateTime(s1);
			age = (int)vTime.TotalSeconds;
			if (age > nMaxSeconds)
				sValue = "";
			return sValue;
		}

		public static bool SetKeyValue(string sKey, string sValue)
        {
			string sql = "Delete from sys where systemkey=@skey;\r\nInsert into sys (id,systemkey,Updated,Value) values (uuid(),@skey,now(),@svalue);";
			MySqlCommand cmd1 = new MySqlCommand(sql);
			cmd1.Parameters.AddWithValue("@skey", sKey);
			cmd1.Parameters.AddWithValue("@svalue", sValue);
			bool f = Database.ExecuteNonQuery(false, cmd1, "");
			return f;
        }

		public static bool SetKeyDouble(string sKey, double nValue)
        {
			bool f = SetKeyValue(sKey, nValue.ToString());
			return f;
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

			try
			{
				int age = 0;
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


	public class price1
	{
		public string Ticker { get; set; }
		public double Amount { get; set; }
		public double AmountUSD { get; set; }
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


		public class UTXOIntegration
        {
			public string table = "UTXOIntegration";
			public string data = "";
			public string added = "";
			public string signature = "";
			public int nHeight = 0;
        }


		public static bool Latch(bool fTestNet, string sName, int nSeconds)
        {
			// This is a database backed latch.  If the seconds have not expired, return false.
			// Once the seconds expire, return true and set the latch.
			// Note that a different latch exists for testnet and mainnet.
			string sKey = fTestNet.ToString() + sName;
			double nKV = GetKeyDouble(sKey, nSeconds);
			if (nKV == 1)
				return false;
			SetKeyDouble(sKey, 1);
			return true;
		}


	}
}
