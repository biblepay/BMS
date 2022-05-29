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

		
		public static async Task<List<Asset>> QueryTokenBalances(string sERC20Address)
		{
			List<Asset> l1 = GetAssetList();
			for (int i = 0; i < l1.Count; i++)
			{
				Asset l0 = l1[i];
				l0.Amount = await GetResolvedBalance(l0.Chain, l0.ERCAddress, sERC20Address);
				//string sSumm = sERC20Address + "," + l0.Symbol + "," + nBalance.ToString();
				l1[i] = l0;
			}
			return l1;
		}

		public static string minABI = @"[{""constant"":false,""inputs"":[{""name"":""_spender"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""approve"",""outputs"":[{""name"":""success"",""type"":""bool""}],""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""name"":""supply"",""type"":""uint256""}],""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_from"",""type"":""address""},{""name"":""_to"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[{""name"":""success"",""type"":""bool""}],""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_owner"",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""name"":""balance"",""type"":""uint256""}],""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_to"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""name"":""success"",""type"":""bool""}],""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_owner"",""type"":""address""},{""name"":""_spender"",""type"":""address""}],""name"":""allowance"",""outputs"":[{""name"":""remaining"",""type"":""uint256""}],""type"":""function""},{""inputs"":[{""name"":""_initialAmount"",""type"":""uint256""}],""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""_from"",""type"":""address""},{""indexed"":true,""name"":""_to"",""type"":""address""},{""indexed"":false,""name"":""_value"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""_owner"",""type"":""address""},{""indexed"":true,""name"":""_spender"",""type"":""address""},{""indexed"":false,""name"":""_value"",""type"":""uint256""}],""name"":""Approval"",""type"":""event""}]";

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
		public static string GetEtherEndpoint(string sName, out int nChainID)
		{
			string sURL = "";
			nChainID = 0;
			if (sName.ToUpper() == "BSC")
			{
				sURL = BMSCommon.Common.GetConfigurationKeyValue("etherendpointbsc");
				nChainID = 56;
			}
			else if (sName.ToUpper() == "MATIC" || sName.ToUpper() == "POLYGON")
			{
				sURL = BMSCommon.Common.GetConfigurationKeyValue("etherendpointmatic");
				nChainID = 137;
			}
			return sURL;
		}
		public static async Task<double> GetContractBalance(string sNetwork, string sContractAddress, string sAccount)
		{
			int nChainID = 0;
			string sPoint =  GetEtherEndpoint(sNetwork, out nChainID);
			// Note: This is to get a Contract balance 
			try
			{
				var web3 = new Web3(sPoint);
				var contract = web3.Eth.GetContract(minABI, sContractAddress);
				var balanceFunction = contract.GetFunction("balanceOf");
				BigInteger balance = await balanceFunction.CallAsync<BigInteger>(sAccount);
				int nDecimals = 18;
				if (sContractAddress == "0xcE829A89d4A55a63418bcC43F00145adef0eDB8E")
					nDecimals = 8; //renDOGE
				double nBal = BigIntToDouble(balance, nDecimals);
				return nBal;
			}
			catch (Exception ex)
			{
				string myerr = ex.Message;
				return 0;
			}
		}
		public static async Task<double> GetAccountBalance(string sNetwork, string sERCAccount)
		{
			if (sERCAccount == null || sERCAccount == "")
			{
				return 0;
			}
			// Note: This is to get an ACCOUNT balance (not a smart contract balance)
			int nChainID = 0;
			string sPoint = GetEtherEndpoint(sNetwork, out nChainID);
			var web3 = new Web3(sPoint);
			try
			{
				//var latestBlockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
				HexBigInteger b1 = await web3.Eth.GetBalance.SendRequestAsync(sERCAccount);
				BigInteger b2 = b1.Value;
				double nOut = BMSCommon.Common.GetDouble(b2.ToString()) / 1000000000000000000;
				return nOut;
			}
			catch (Exception ex)
			{
				Common.Log("GAB::" + ex.Message);
				return 0;
			}
		}

		public static async Task<double> GetResolvedBalance(string sNetwork, string sContractAddress, string sAccountAddress)
		{
			// This accepts an account or a contract
			double nBalance = 0;
			if (sContractAddress == "0x0")
			{
				nBalance = await GetAccountBalance(sNetwork, sAccountAddress);
				return nBalance;
			}
			else
			{
				nBalance = await GetContractBalance(sNetwork, sContractAddress, sAccountAddress);
				return nBalance;
			}
		}

		public static List<Asset> GetAssetList()
		{
			List<Asset> l1 = new List<Asset>();
			l1.Add(new Asset { Chain = "POLYGON", Symbol = "MATIC", ERCAddress = "0x0", ChainlinkAddress = "0xAB594600376Ec9fD91F8e885dADF0CE036862dE0", Price = 0 });
			l1.Add(new Asset { Chain = "BSC", Symbol = "BSC", ERCAddress = "0x0", ChainlinkAddress = "0x0567F2323251f0Aab15c8dFb1967E4e8A7D42aeE", Price = 0 });
			l1.Add(new Asset { Chain = "POLYGON", Symbol = "WETH", ERCAddress = "0x7ceb23fd6bc0add59e62ac25578270cff1b9f619", ChainlinkAddress = "0xF9680D99D6C9589e2a93a78A04A279e509205945", Price = 0 });
			l1.Add(new Asset { Chain = "BSC", Symbol = "WBBP", ERCAddress = "0xcb1eec8630c5176611f72799853c3b7dbe4b8953", Price = 0 });
			l1.Add(new Asset { Chain = "BSC", Symbol = "CAKE", ERCAddress = "0x0e09fabb73bd3ade0a17ecc321fd13a19e81ce82", ChainlinkAddress = "0xB6064eD41d4f67e353768aA239cA86f4F73665a1", Price = 0 });
			l1.Add(new Asset { Chain = "BSC", Symbol = "FIELD", ERCAddress = "0x04d50c032f16a25d1449ef04d893e95bcc54d747", Price = .003 });
			l1.Add(new Asset { Chain = "POLYGON", Symbol = "renDOGE", ERCAddress = "0xcE829A89d4A55a63418bcC43F00145adef0eDB8E", ChainlinkAddress = "0xbaf9327b6564454F4a3364C33eFeEf032b4b4444", Price = 0 });
			return l1;
		}
		public static async Task<double> GetMaticPrice()
		{
			double nResult = await GetChainLinkPrice("POLYGON", "0xAB594600376Ec9fD91F8e885dADF0CE036862dE0");
			return nResult;
		}

		public static async Task<double> GetChainLinkPrice(string sNetwork, string sContractID)
		{
			int nChainID = 0;
			if (sContractID == null || sContractID == "")
				return 0;

			string sPoint = GetEtherEndpoint(sNetwork, out nChainID);
			string sABI = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_aggregator\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_accessController\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"int256\",\"name\":\"current\",\"type\":\"int256\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"roundId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"updatedAt\",\"type\":\"uint256\"}],\"name\":\"AnswerUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"roundId\",\"type\":\"uint256\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"startedBy\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"startedAt\",\"type\":\"uint256\"}],\"name\":\"NewRound\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"}],\"name\":\"OwnershipTransferRequested\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"acceptOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"accessController\",\"outputs\":[{\"internalType\":\"contract AccessControllerInterface\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"aggregator\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_aggregator\",\"type\":\"address\"}],\"name\":\"confirmAggregator\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"internalType\":\"uint8\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"description\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_roundId\",\"type\":\"uint256\"}],\"name\":\"getAnswer\",\"outputs\":[{\"internalType\":\"int256\",\"name\":\"\",\"type\":\"int256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint80\",\"name\":\"_roundId\",\"type\":\"uint80\"}],\"name\":\"getRoundData\",\"outputs\":[{\"internalType\":\"uint80\",\"name\":\"roundId\",\"type\":\"uint80\"},{\"internalType\":\"int256\",\"name\":\"answer\",\"type\":\"int256\"},{\"internalType\":\"uint256\",\"name\":\"startedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"updatedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint80\",\"name\":\"answeredInRound\",\"type\":\"uint80\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_roundId\",\"type\":\"uint256\"}],\"name\":\"getTimestamp\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"latestAnswer\",\"outputs\":[{\"internalType\":\"int256\",\"name\":\"\",\"type\":\"int256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"latestRound\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"latestRoundData\",\"outputs\":[{\"internalType\":\"uint80\",\"name\":\"roundId\",\"type\":\"uint80\"},{\"internalType\":\"int256\",\"name\":\"answer\",\"type\":\"int256\"},{\"internalType\":\"uint256\",\"name\":\"startedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"updatedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint80\",\"name\":\"answeredInRound\",\"type\":\"uint80\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"latestTimestamp\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint16\",\"name\":\"\",\"type\":\"uint16\"}],\"name\":\"phaseAggregators\",\"outputs\":[{\"internalType\":\"contract AggregatorV2V3Interface\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"phaseId\",\"outputs\":[{\"internalType\":\"uint16\",\"name\":\"\",\"type\":\"uint16\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_aggregator\",\"type\":\"address\"}],\"name\":\"proposeAggregator\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"proposedAggregator\",\"outputs\":[{\"internalType\":\"contract AggregatorV2V3Interface\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint80\",\"name\":\"_roundId\",\"type\":\"uint80\"}],\"name\":\"proposedGetRoundData\",\"outputs\":[{\"internalType\":\"uint80\",\"name\":\"roundId\",\"type\":\"uint80\"},{\"internalType\":\"int256\",\"name\":\"answer\",\"type\":\"int256\"},{\"internalType\":\"uint256\",\"name\":\"startedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"updatedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint80\",\"name\":\"answeredInRound\",\"type\":\"uint80\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"proposedLatestRoundData\",\"outputs\":[{\"internalType\":\"uint80\",\"name\":\"roundId\",\"type\":\"uint80\"},{\"internalType\":\"int256\",\"name\":\"answer\",\"type\":\"int256\"},{\"internalType\":\"uint256\",\"name\":\"startedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"updatedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint80\",\"name\":\"answeredInRound\",\"type\":\"uint80\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_accessController\",\"type\":\"address\"}],\"name\":\"setController\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_to\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"version\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
			try
			{
				var account = new Account(BMSCommon.Common.GetConfigurationKeyValue("nftprivkey"), nChainID);

				var web4 = new Web3(account, sPoint);
				var contract = web4.Eth.GetContract(sABI, sContractID);
				var balanceFunction = contract.GetFunction("latestAnswer");
				BigInteger bal = await balanceFunction.CallAsync<BigInteger>();
				double nBal = Common.GetDouble(bal.ToString()) / 100000000;
				return nBal;
			}
			catch (Exception ex)
			{
				Common.Log("ChainLink::GetPrice::" + ex.Message);
				return 0;
			}
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
					string sKey = Common.GetConfigurationKeyValue("blockchairkey");
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

	public async static Task<string> ExecutePortfolioBuilderExport(bool fTestNet, int nNextHeight)
	{
			Dictionary<string, PortfolioBuilder.PortfolioParticipant> u = await PortfolioBuilder.GenerateUTXOReport(fTestNet);
			string sSummary = "<data><ver>2.1</ver>";
			foreach (KeyValuePair<string, PortfolioBuilder.PortfolioParticipant> pp in u)
			{
				{
					if (pp.Value.Strength > 0)
					{
						string sSummaryRow = "<row>"
						+ pp.Value.RewardAddress
						+ "<col>" + pp.Value.NickName
						+ "<col>"
						+ "<col>" + pp.Value.AmountBBP.ToString()
						+ "<col>" + pp.Value.AmountForeign.ToString()
						+ "<col>" + pp.Value.AmountUSDBBP.ToString()
						+ "<col>" + pp.Value.AmountUSDForeign.ToString()
						+ "<col>" + pp.Value.AmountUSD.ToString()
						+ "<col>" + BMSCommon.Common.DoubleToString(pp.Value.Coverage, 4)
						+ "<col>" + BMSCommon.Common.DoubleToString(pp.Value.Strength, 4)
						+ "<col>" + "\r\n";
						sSummary += sSummaryRow;
					}
				}
			}
			sSummary += "</data>";
			string sHash = "<hash>" + BMSCommon.Encryption.GetSha256HashI(sSummary) + "</hash>";
			DateTime dt1 = System.DateTime.UtcNow;
			string sDate = "<DATE>" + dt1.ToString("MM_dd_yy") + "</DATE>";
			sSummary += sHash;
			sSummary += sDate;
			sSummary += "<height>" + nNextHeight.ToString() + "</height>";
			sSummary += "\r\n<EOF>\r\n";
			return sSummary;
	}

		public class UTXOIntegration
        {
			public string table = "UTXOIntegration";
			public string data = "";
			public string added = "";
			public string signature = "";
			public int nHeight = 0;
        }

        public static async Task<bool> DailyUTXOExport(bool fTestNet)
		{
			string sKey = fTestNet.ToString() + "utxoexport";
			double nKV = GetKeyDouble(sKey, 60*30);
			if (nKV == 1)
				return false;

			SetKeyDouble(sKey, 1);
			double nNextHeight = 0;

			try
			{
				bool fExists = WebRPC.GetNextContract(fTestNet, out nNextHeight);
				if (fExists || nNextHeight == 0)
				{
					//Log("Export exists for " + fTestNet.ToString());
					//if (!fTestNet)
					// {
					//string sData2 = await BiblePayUtilities.ExecutePortfolioBuilderExport(fTestNet, (int)nNextHeight);
					// }
					return false;
				}

				if (!fTestNet)
				{
					BMSCommon.Common.Log("CREATING UTXO DAILY EXPORT FOR HEIGHT " + nNextHeight.ToString());
				}
				//todo check if it already exists here.
				//BiblePayCommon.Entity.utxointegration2 o = new BiblePayCommon.Entity.utxointegration2();
				string sData = await ExecutePortfolioBuilderExport(fTestNet, (int)nNextHeight);
				if (!fTestNet && sData.Length < 400)
				{
					BMSCommon.Common.Log("DailyUTXOExport::Data too short to save!  " + sData);
					return false;
				}
				UTXOIntegration u = new UTXOIntegration();
				u.added = DateTime.Now.ToString();
				u.nHeight = (int)nNextHeight;
				u.data = sData;
				BMSCommon.CryptoUtils.Transaction t = new BMSCommon.CryptoUtils.Transaction();
				// remoe the tx from memory pool when we accept the block (connectblock)
				t.Time = Common.UnixTimestamp();
				t.Data = Newtonsoft.Json.JsonConvert.SerializeObject(u);
				BMSCommon.BitcoinSync.AddToMemoryPool(fTestNet, t);

				// Check utxo signature here
				string sSP = BMSCommon.Common.GetConfigurationKeyValue("utxoprivkey");
				bool fOK = true;
				if (fOK)
				{
					string UtxoTXID = WebRPC.InsertDataIntoChain(fTestNet, "GSC", sData, sSP);
					if (UtxoTXID == "")
					{
						BMSCommon.Common.Log("Unable to persist utxo data");
						return false;
					}
				}
				return true;
			}
			catch(Exception ex)
			{ 

				BMSCommon.Common.Log("DailyUtxoExport::ERROR::" + ex.Message);
				if (!fTestNet)
				{
						MailAddress mTo = new MailAddress("rob@biblepay.org", "Rob Andrews");
						MailMessage m = new MailMessage();
						m.To.Add(mTo);
						string sSubject = "UNABLE TO EXPORT UTXO EXPORT! ";
						m.Subject = sSubject;
						m.Body = "Error, " + ex.Message.ToString() + "\r\n for height " + nNextHeight.ToString();
						m.IsBodyHtml = false;
						API.SendMail(false, m);
				}
				return false;
			}
		}
	}
}
