using BMSCommon;
using BMSCommon.Model;
using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BBPAPI.Service;
using static BMSCommon.Model.BitcoinSyncModel;
using static BBPAPI.Interface.Core;

namespace BBPAPI.Interface
{
	public static class WebRPC
	{

		public static string GetFDPubKey(bool fTestNet)
		{
			string sPubKey = fTestNet ? "yTrEKf8XQ7y7tychC2gWuGw1hsLqBybnEN" : "BB2BwSbDCqCqNsfc7FgWFJn4sRgnUt4tsM";
			return sPubKey;
		}



		public async static Task<DACResult> SendMoney(SendMoneyRequest m)
		{
			DACResult r = await ReturnObject<DACResult>("webrpc/SendMoney", m);
			return r;
		}
		public async static Task<bool> GObjectPrepare(Proposal p)
		{
			bool f = await ReturnObject<bool>("webrpc/GObjectPrepare", p);
			return f;
		}

		public async static Task<bool> GObjectSubmit(Proposal p)
		{
			bool f = await ReturnObject<bool>("webrpc/GObjectSubmit", p);
			return f;
		}



		public async static Task<string> GetAddressUTXOs(BBPNetAddress a)
		{
			string s = await ReturnObject<string>("webrpc/GetAddressUTXOs", a);
			return s;
		}
		public async static Task<SupplyType> GetSupply(bool fTestNet)
		{
			SupplyType r = await ReturnObject<SupplyType>("webrpc/GetSupply", fTestNet);
			return r;
		}

		public async static Task<BitcoinSyncBlock> GetBlock(BBPNetHeight h)
		{
			BitcoinSyncBlock r = await ReturnObject<BitcoinSyncBlock>("webrpc/GetBlock", h);
			return r;
		}


		public async static Task<bool> ValidateBBPAddress(BBPNetAddress a)
		{
			bool r = await ReturnObject<bool>("webrpc/ValidateBBPAddress", a);
			return r;
		}

		public static bool SubmitProposals(User u, bool fTestNet)
		{
			string sChain = fTestNet ? "test" : "main";
			List<Proposal> dt = BBPAPI.Interface.Repository.GetDatabaseObjects<Proposal>("proposal");
			dt = dt.Where(s => s.Chain == sChain && s.SubmitTXID == null).ToList();
			for (int y = 0; y < dt.Count; y++)
			{
				Proposal p = dt[y];
				p.TestNet = fTestNet;
				p.User = u;
				bool fSubmitted = BBPAPI.Interface.WebRPC.GObjectSubmit(p).Result;
			}
			return true;
		}


		internal static DACResult SendBBPOutsideChain(bool fTestNet, string sType, string sToAddress, string sPubKey, string sPrivKey,
				double nAmount, string sPayload)
		{
			BBPNetAddress b1 = new BBPNetAddress();
			b1.TestNet = fTestNet;
			b1.Address = sPubKey;
			string sUnspentData = BBPAPI.Interface.WebRPC.GetAddressUTXOs(b1).Result;
			string sErr = String.Empty;
			string sTXID = String.Empty;
			NBitcoin.Crypto.BBPTransaction.PrepareFundingTransaction(fTestNet, nAmount, sToAddress, sPrivKey, sPayload, sUnspentData, out sErr, out sTXID);
			DACResult r = new DACResult();
			if (sErr != String.Empty)
			{
				r.Error = sErr;
				return r;
			}
			BBPNetHex h = new BBPNetHex();
			h.TestNet = fTestNet;
			h.Hex = sTXID;
			r = BBPAPI.Interface.WebRPC.SendRawTx(h).Result;
			return r;
		}


		public async static Task<DACResult> SendRawTx(BBPNetHex h)
		{
			DACResult r = await ReturnObject<DACResult>("webrpc/SendRawTx", h);
			return r;
		}
		public async static Task<List<MasternodeListItem>> GetMasternodeList(bool fTestnet)
		{
			List<MasternodeListItem> r = await ReturnObject<List<MasternodeListItem>>("webrpc/GetMasternodeList", fTestnet);
			return r;
		}
		internal static double QueryAddressBalanceCached(bool fTestNet, string sAddress, int nMaxAgeInSeconds)
		{
			BBPNetAddress b = new BBPNetAddress();
			b.TestNet = fTestNet;
			b.Address = sAddress;

			string sCached = BMSCommon.MemoryCache.GetKeyValue("utxo_" + sAddress, nMaxAgeInSeconds);
			if (String.IsNullOrEmpty(sCached))
			{
				sCached = BBPAPI.Interface.WebRPC.GetAddressUTXOs(b).Result;
				BMSCommon.MemoryCache.SetKeyValue("utxo_" + sAddress, sCached);
			}
			double nBal = QueryAddressBalanceNewMethod(b);
			return nBal;
		}


		internal static double QueryAddressBalanceNewMethod(BBPNetAddress bn)
		{
			string sUTXOData = BBPAPI.Interface.WebRPC.GetAddressUTXOs(bn).Result;
			double nBal = CalculateAddressBalance(sUTXOData);
			return nBal;
		}
		internal static double CalculateAddressBalance(string sData)
		{
			try
			{
				dynamic oJson = JsonConvert.DeserializeObject<dynamic>(sData);
				double nTotal = 0;
				foreach (var j in oJson)
				{
					BalanceUTXO u = new BalanceUTXO();
					u.Amount = new NBitcoin.Money((decimal)j["satoshis"], NBitcoin.MoneyUnit.Satoshi);
					u.index = Convert.ToInt32(j["outputIndex"].Value);
					u.TXID = new NBitcoin.uint256((string)j["txid"]);
					u.Height = (int)j["height"].Value;
					u.Address = j["address"].Value;
					nTotal += (double)u.Amount.ToDecimal(MoneyUnit.BTC);
				}
				return nTotal;
			}
			catch (Exception)
			{
				// Wrong chain?
				return -1;
			}
		}

	}
}
