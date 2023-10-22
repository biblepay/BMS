using BMSCommon;
using MySql.Data.MySqlClient;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.BlockChair;
using static BMSCommon.Common;
using static BMSCommon.Model;
using static BMSCommon.PortfolioBuilder;
using static BMSCommon.Pricing;
using static BMSCommon.WebRPC;

namespace BBPTestHarness
{
    public static class BlockChairTestHarness
    {

        public static double GetForeignQuantityByERC20(bool fTestNet, string sERC20Address, string sSymbol)
        {
            List<BlockChair.BlockChairUTXO> l = GetNativeUTXOsByERC20(fTestNet, sERC20Address, sSymbol);
            double nTotal = 0;
            for (int i = 0; i < l.Count; i++)
            {
                // Mission critical todo: verify pin here
                double nQty = l[i].Amount;
                nTotal += nQty;
            }
            return nTotal;
        }

        public static List<BMSCommon.BlockChair.BlockChairUTXO> GetNativeUTXOsByERC20(bool fTestNet, string sERC20Address, string sSymbol)
        {
            string sTable = fTestNet ? "tUTXOPosition" : "UTXOPosition";
            string sql = "Select * from " + sTable + " where ERC20Address=@ERC20 and Symbol=@Symbol;";
            NpgsqlCommand m1 = new NpgsqlCommand(sql);
            m1.Parameters.AddWithValue("@ERC20", sERC20Address);
            m1.Parameters.AddWithValue("@Symbol", sSymbol);
            DataTable dt = CockroachDatabase.GetDataTable(m1);
            List<BlockChair.BlockChairUTXO> lUTXo = new List<BlockChair.BlockChairUTXO>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sAddress = dt.Rows[i]["ForeignAddress"].ToString();
                List<BlockChair.BlockChairUTXO> l = QueryUTXOs(sSymbol, sAddress, 0);
                for (int x = 0; x < l.Count; x++)
                {
                    lUTXo.Add(l[x]);
                }
            }
            return lUTXo;
        }

        private static dynamic GetBlockChairData(string sURL)
        {
            string sData = BMSCommon.Common.ExecuteMVCCommand(sURL);
            dynamic oJson = JsonConvert.DeserializeObject<dynamic>(sData);
            string sql = "Insert into BlockChairRequestLog (id,added,URL) values (gen_random_uuid(), now(), '" + sURL + "')";
            NpgsqlCommand m1 = new NpgsqlCommand(sql);
            BMSCommon.CockroachDatabase.ExecuteNonQuery(m1);
            return oJson;
        }

        private static void PersistUTXO(BlockChairUTXO u)
        {
            string sql = "Delete from BlockChair2 where ticker=@ticker and Address=@address and TXID=@txid;";
            sql += "\r\n INSERT INTO BlockChair2 (id,ticker,added,amount,Address,ordinal,txid,height,account,TotalBalance,utxotxtime,txcount) values (gen_random_uuid(),"
                 + "@ticker, now(), @amount,@address,@ordinal,@txid,@height,@account,@totalBalance,@utxotime,@txcount); ";
            NpgsqlCommand m1 = new NpgsqlCommand(sql);
            m1.Parameters.AddWithValue("@ticker", u.Ticker);
            m1.Parameters.AddWithValue("@amount", u.Amount);
            m1.Parameters.AddWithValue("@address", u.Address);
            m1.Parameters.AddWithValue("@ordinal", u.Ordinal);
            m1.Parameters.AddWithValue("@txid", u.TXID);
            m1.Parameters.AddWithValue("@height", u.Height);
            m1.Parameters.AddWithValue("@account", u.Account.ToNonNullString());
            m1.Parameters.AddWithValue("@totalBalance", u.TotalBalance);
            m1.Parameters.AddWithValue("@utxotime", u.UTXOTxTime);
            m1.Parameters.AddWithValue("@txcount", u.TxCount);
            bool f1 = CockroachDatabase.ExecuteNonQuery(m1);
            bool f2 = false;
        }

        private static BlockChairUTXO ConvertDataRowToUTXO(DataRow r)
        {
            BlockChairUTXO u = new BlockChairUTXO();
            u.Ticker = r["Ticker"].ToString();
            u.Address = r["Address"].ToString();
            u.Amount = BMSCommon.Common.GetDouble(r["Amount"]);
            u.Ordinal = (int)GetDouble(r["ordinal"]);
            u.Added = r["Added"].ToString();
            u.TXID = r["TXID"].ToString();
            u.Account = r["Account"].ToString();
            u.TotalBalance = GetDouble(r["TotalBalance"]);
            u.Height = (int)GetDouble(r["Height"]);
            u.found = true;
            return u;
        }

        public static List<BlockChairUTXO> QueryUTXOs(string sTicker, string sAddress, int nTime)
        {
            string sql1 = "Select max(added) dt1 from BlockChair2 where address=@address and Ticker=@ticker;";
            NpgsqlCommand m1 = new NpgsqlCommand(sql1);
            m1.Parameters.AddWithValue("@address", sAddress);
            m1.Parameters.AddWithValue("@ticker", sTicker);

            double nAge = CockroachDatabase.GetScalarAge(m1, "dt1");
            bool bRefresh = false;
            int nElapsed = UnixTimestamp() - nTime;
            if (nElapsed > (60 * 60 * 24))
            {
                // UTXO is Older
                if (nAge > 60 * 60 * 24)
                    bRefresh = true;
            }
            else
            {
                if (nAge > 60 * 15)
                    bRefresh = true;
            }

            if (nTime == 1)
            {
                bRefresh = true;
            }

            if (bRefresh)
            {
                CacheEntireUTXO(sTicker, sAddress);
            }

            string sql = "Select * from BlockChair2 where address=@address and Ticker=@ticker;";

            List<BlockChairUTXO> l = new List<BlockChairUTXO>();
            m1 = new NpgsqlCommand(sql);
            m1.Parameters.AddWithValue("@address", sAddress);
            m1.Parameters.AddWithValue("@ticker", sTicker);
            DataTable dt = CockroachDatabase.GetDataTable(m1);
            BlockChairUTXO u = new BlockChairUTXO();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                u = ConvertDataRowToUTXO(dt.Rows[i]);
                if (u.Amount > 0)
                {
                    l.Add(u);
                }
            }
            return l;
        }



        // Blockchair integration
        public static void CacheEntireUTXO(string sTicker, string sAddress)
        {
            // We should erase the old records at this point
            if (sAddress == "" || sAddress.Contains("."))
            {
                return;
            }
            int OperationCount = 0;
            string sKey = BMSCommon.Encryption.DecryptAES256("ZtEciCL5O3gSru+1VvKpzppMuAflYzPkE4pZ8dz+F41U52tSupSEG8ldJKgRI/rw", "");

            try
            {
                string sURL = "";
                if (sTicker == "XLM")
                {
                    // With stellar, the user must have one transaction matching the amount(with pin suffix) + balance must be equal to or greater than that stake.
                    sURL = "https://api.blockchair.com/stellar/raw/account/" + sAddress + "?transactions=true&operations=true&key=" + sKey;
                    dynamic oJson = GetBlockChairData(sURL);
                    dynamic oBalances = oJson["data"][sAddress]["account"]["balances"];
                    dynamic oOps = oJson["data"][sAddress]["operations"];
                    double nBalance = 0;
                    foreach (var b in oBalances)
                    {
                        if (b["asset_type"].Value == "native")
                        {
                            nBalance = GetDouble(b["balance"].Value);
                        }
                    }

                    foreach (var o in oOps)
                    {
                        BlockChairUTXO u = new BlockChairUTXO();
                        u.Ticker = sTicker;
                        u.Address = sAddress;
                        string sTxType = ToNonNull(o["type"]);
                        string sType = ToNonNull(o["asset_type"]);
                        if (sType == "native")
                        {
                            u.Amount = GetDouble(o["amount"].Value);
                            u.TXID = BMSCommon.Encryption.GetSha256HashI(u.Address + u.Amount.ToString());
                            if (u.Amount > 0)
                            {
                                PersistUTXO(u);
                                OperationCount++;
                            }
                        }
                    }
                }
                else if (sTicker == "XRP")
                {
                    // With Ripple, the user must have one total matching balance(to the pin) and no extra transactions.
                    sURL = "https://api.blockchair.com/ripple/raw/account/" + sAddress + "?transactions=true&key=" + sKey;
                    dynamic oJson = GetBlockChairData(sURL);
                    //                    dynamic oBalances = oJson["data"][sAddress]["account"]["account_data"];

                    dynamic oTx = oJson["data"][sAddress]["transactions"]["transactions"];
                    // double nBalance = GetDouble(oBalances["Balance"].Value) / 1000000;

                    int nTxCount = 0;
                    foreach (dynamic oMyTx in oTx)
                    {
                        BlockChairUTXO u = new BlockChairUTXO();

                        nTxCount++;
                        u = new BlockChairUTXO();
                        u.Ticker = sTicker;
                        u.Address = sAddress;
                        u.Amount = GetDouble(oMyTx["tx"]["Amount"].Value) / 1000000;
                        u.TXID = Encryption.GetSha256HashI(sAddress + u.Amount.ToString());
                        if (u.Amount > 0)
                        {
                            PersistUTXO(u);
                            OperationCount++;

                        }
                    }

                }
                else if (sTicker == "DOGE" || sTicker == "BTC" || sTicker == "DASH" || sTicker == "LTC" || sTicker == "ZEC" || sTicker == "BCH")
                {
                    string sTickerName = BlockChairTickerToName(sTicker);

                    sURL = "https://api.blockchair.com/" + sTickerName + "/dashboards/address/" + sAddress + "?key=" + sKey;
                    dynamic oJson = GetBlockChairData(sURL);
                    dynamic oBalances = oJson["data"][sAddress]["utxo"];
                    foreach (var b in oBalances)
                    {
                        BlockChairUTXO u = new BlockChairUTXO();
                        u.Ticker = sTicker;
                        u.Address = sAddress;
                        u.Amount = GetDouble(b["value"].Value) / 100000000;
                        u.TXID = b["transaction_hash"].Value;
                        u.Ordinal = (int)GetDouble(b["index"].Value);
                        // Make unique
                        u.TXID = b["transaction_hash"].Value + u.Ordinal.ToString();

                        u.Height = (int)GetDouble(b["block_id"].Value);
                        PersistUTXO(u);
                        OperationCount++;
                    }

                }
                else if (sTicker == "ETH")
                {
                    sURL = "https://api.blockchair.com/ethereum/dashboards/address/" + sAddress + "?transactions=true&key=" + sKey;
                    dynamic oJson = GetBlockChairData(sURL);
                    dynamic oBalance = oJson["data"][sAddress.ToLower()]["address"];
                    int nTxCount = (int)GetDouble(oJson["data"][sAddress.ToLower()]["address"]["transaction_count"].Value);
                    double nBalance = GetDouble(oBalance["balance"].Value) / 100000000 / 10000000000;
                    dynamic oCalls = oJson["data"][sAddress.ToLower()]["calls"];
                    int nOrdinal = 0;
                    foreach (dynamic oCall in oCalls)
                    {
                        BlockChairUTXO u = new BlockChairUTXO();
                        u.Ticker = sTicker;
                        u.Address = sAddress;
                        u.Amount = GetDouble(oCall["value"].Value) / 100000000 / 10000000000;
                        nOrdinal++;
                        u.TXID = Encryption.GetSha256HashI(u.Address + u.Amount.ToString() + nOrdinal.ToString());
                        PersistUTXO(u);
                        OperationCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        public static string BlockChairTickerToName(string sTicker)
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
            return "";
        }

        /*
        public static async Task<List<ERCAsset>> QueryTokenBalances(bool fTestNet, string sERC20Address)
        {
            List<ERCAsset> l1 = BMSCommon.Retired.ERC20Assets.GetAssetList();
            for (int i = 0; i < l1.Count; i++)
            {
                ERCAsset l0 = l1[i];
                l0.Amount = await GetResolvedBalance(fTestNet, l0.Chain, l0.ERCAddress, sERC20Address, l0.Symbol);
                l1[i] = l0;
            }
            return l1;
        }

        public static string minABI = @"[{""constant"":false,""inputs"":[{""name"":""_spender"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""approve"",""outputs"":[{""name"":""success"",""type"":""bool""}],""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""name"":""supply"",""type"":""uint256""}],""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_from"",""type"":""address""},{""name"":""_to"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[{""name"":""success"",""type"":""bool""}],""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_owner"",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""name"":""balance"",""type"":""uint256""}],""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_to"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""name"":""success"",""type"":""bool""}],""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_owner"",""type"":""address""},{""name"":""_spender"",""type"":""address""}],""name"":""allowance"",""outputs"":[{""name"":""remaining"",""type"":""uint256""}],""type"":""function""},{""inputs"":[{""name"":""_initialAmount"",""type"":""uint256""}],""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""_from"",""type"":""address""},{""indexed"":true,""name"":""_to"",""type"":""address""},{""indexed"":false,""name"":""_value"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""_owner"",""type"":""address""},{""indexed"":true,""name"":""_spender"",""type"":""address""},{""indexed"":false,""name"":""_value"",""type"":""uint256""}],""name"":""Approval"",""type"":""event""}]";
        public static async Task<double> GetERC20ContractBalance(string sNetwork, string sContractAddress, string sAccount)
        {
            int nChainID = 0;
            string sPoint = GetEtherEndpoint(sNetwork, out nChainID);
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
                double nBal = PricingBigIntToDouble(balance, nDecimals);
                return nBal;
            }
            catch (Exception ex)
            {
                string myerr = ex.Message;
                return 0;
            }
        }



        public static async Task<double> GetResolvedBalance(bool fTestNet, string sNetwork, string sContractAddress, string sAccountAddress, string sSymbol)
        {
            // This accepts an account or a contract
            double nBalance = 0;
            string sChain = fTestNet ? "testnet" : "mainnet";
            if (sContractAddress == "")
            {
                // Non ERC20 Layer 1:
                double n1 = BMSCommon.CockroachDatabase.GetKeyDouble(sChain + sAccountAddress + sSymbol, 60 * 60 * 8);
                if (n1 != 0)
                    return n1;
                double nAmt = GetForeignQuantityByERC20(fTestNet, sAccountAddress, sSymbol);
                if (nAmt == 0)
                    nAmt = -1;
                BMSCommon.CockroachDatabase.SetKeyDouble(sChain + sAccountAddress + sSymbol, nAmt);
                return nAmt;
            }
            if (sContractAddress == "0x0")
            {
                // ERC20 Layer 1
                double n2 = BMSCommon.CockroachDatabase.GetKeyDouble(sChain + sNetwork + sAccountAddress, 60 * 60 * 8);
                if (n2 != 0)
                    return n2;
                nBalance = await GetERC20AccountBalance(sNetwork, sAccountAddress);
                if (nBalance == 0)
                    nBalance = -1;
                BMSCommon.CockroachDatabase.SetKeyDouble(sChain + sNetwork + sAccountAddress, nBalance);
                return nBalance;
            }
            else
            {
                // ERC20 Layer 2
                double n3 = BMSCommon.CockroachDatabase.GetKeyDouble(sChain + sNetwork + sContractAddress + sAccountAddress, 60 * 60 * 8);
                if (n3 != 0)
                    return n3;
                nBalance = await GetERC20ContractBalance(sNetwork, sContractAddress, sAccountAddress);
                if (nBalance == 0)
                    nBalance = -1;
                BMSCommon.CockroachDatabase.SetKeyDouble(sChain + sNetwork + sContractAddress + sAccountAddress, nBalance);
                return nBalance;
            }
        }
        public static async Task<double> GetChainLinkPrice(ERCAsset a)
        {
            int nChainID = 0;

            if (a.ERCAddress == String.Empty)
            {
                // This is a native layer 1 asset
                price1 p = BMSCommon.Pricing.GetCryptoPrice(a.Symbol + "/BTC");
                return p.AmountUSD;
            }

            if (a.Symbol.ToLower() == "shib")
            {
                return a.Price;
            }
            if (String.IsNullOrEmpty(a.ChainlinkAddress))
                return 0;

            string sKey = a.Chain + a.ChainlinkAddress;
            double nCachedValue = BMSCommon.CockroachDatabase.GetKeyDouble(sKey, 60 * 60 * 12);
            if (nCachedValue > 0)
            {
                return nCachedValue;
            }

            string sPoint = GetEtherEndpoint(a.Chain, out nChainID);
            string sABI = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_aggregator\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_accessController\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"int256\",\"name\":\"current\",\"type\":\"int256\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"roundId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"updatedAt\",\"type\":\"uint256\"}],\"name\":\"AnswerUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"roundId\",\"type\":\"uint256\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"startedBy\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"startedAt\",\"type\":\"uint256\"}],\"name\":\"NewRound\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"}],\"name\":\"OwnershipTransferRequested\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"acceptOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"accessController\",\"outputs\":[{\"internalType\":\"contract AccessControllerInterface\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"aggregator\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_aggregator\",\"type\":\"address\"}],\"name\":\"confirmAggregator\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"internalType\":\"uint8\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"description\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_roundId\",\"type\":\"uint256\"}],\"name\":\"getAnswer\",\"outputs\":[{\"internalType\":\"int256\",\"name\":\"\",\"type\":\"int256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint80\",\"name\":\"_roundId\",\"type\":\"uint80\"}],\"name\":\"getRoundData\",\"outputs\":[{\"internalType\":\"uint80\",\"name\":\"roundId\",\"type\":\"uint80\"},{\"internalType\":\"int256\",\"name\":\"answer\",\"type\":\"int256\"},{\"internalType\":\"uint256\",\"name\":\"startedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"updatedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint80\",\"name\":\"answeredInRound\",\"type\":\"uint80\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_roundId\",\"type\":\"uint256\"}],\"name\":\"getTimestamp\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"latestAnswer\",\"outputs\":[{\"internalType\":\"int256\",\"name\":\"\",\"type\":\"int256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"latestRound\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"latestRoundData\",\"outputs\":[{\"internalType\":\"uint80\",\"name\":\"roundId\",\"type\":\"uint80\"},{\"internalType\":\"int256\",\"name\":\"answer\",\"type\":\"int256\"},{\"internalType\":\"uint256\",\"name\":\"startedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"updatedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint80\",\"name\":\"answeredInRound\",\"type\":\"uint80\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"latestTimestamp\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint16\",\"name\":\"\",\"type\":\"uint16\"}],\"name\":\"phaseAggregators\",\"outputs\":[{\"internalType\":\"contract AggregatorV2V3Interface\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"phaseId\",\"outputs\":[{\"internalType\":\"uint16\",\"name\":\"\",\"type\":\"uint16\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_aggregator\",\"type\":\"address\"}],\"name\":\"proposeAggregator\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"proposedAggregator\",\"outputs\":[{\"internalType\":\"contract AggregatorV2V3Interface\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint80\",\"name\":\"_roundId\",\"type\":\"uint80\"}],\"name\":\"proposedGetRoundData\",\"outputs\":[{\"internalType\":\"uint80\",\"name\":\"roundId\",\"type\":\"uint80\"},{\"internalType\":\"int256\",\"name\":\"answer\",\"type\":\"int256\"},{\"internalType\":\"uint256\",\"name\":\"startedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"updatedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint80\",\"name\":\"answeredInRound\",\"type\":\"uint80\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"proposedLatestRoundData\",\"outputs\":[{\"internalType\":\"uint80\",\"name\":\"roundId\",\"type\":\"uint80\"},{\"internalType\":\"int256\",\"name\":\"answer\",\"type\":\"int256\"},{\"internalType\":\"uint256\",\"name\":\"startedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"updatedAt\",\"type\":\"uint256\"},{\"internalType\":\"uint80\",\"name\":\"answeredInRound\",\"type\":\"uint80\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_accessController\",\"type\":\"address\"}],\"name\":\"setController\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_to\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"version\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
            try
            {
                string sNFTPK = BMSCommon.Encryption.DecryptAES256("HiVjwQ8gcYxp9ZAWMslsPsmZ7Szq4+VGz1abmGYIxHGeq9bFmCOIGVmTx0QQheUMZ1Q/PoJXdRTo5RvuEmyWzbbP15e9v+yrXJZsNvPZ3vw=", "");
                var account = new Account(sNFTPK, nChainID);
                var web4 = new Web3(account, sPoint);
                var contract = web4.Eth.GetContract(sABI, a.ChainlinkAddress);
                var balanceFunction = contract.GetFunction("latestAnswer");
                BigInteger nERC20Price = await balanceFunction.CallAsync<BigInteger>();
                double nThePrice = BMSCommon.Common.GetDouble(nERC20Price.ToString()) / 100000000;
                if (nThePrice == 0)
                    nThePrice = -1;
                BMSCommon.CockroachDatabase.SetKeyDouble(sKey, nThePrice);
                return nThePrice;
            }
            catch (Exception ex)
            {
                BMSCommon.Common.Log("ChainLink::GetPrice::" + ex.Message);
                return 0;
            }
        }

        public static string GetEtherEndpoint(string sName, out int nChainID)
        {
            string sURL = "";
            nChainID = 0;
            if (sName.ToUpper() == "BSC")
            {
                sURL = BMSCommon.Encryption.DecryptAES256("xu6u3q640fgaVkMs7N5gRW9HX7QUFS7RanZiPTQBZdhLLb44X+wFbcgWVpV1ZcsU4wJLrpZiIo7U7IRfTm6wS69z4efq4GmYjA9aZwaHpYtcFZBrhZ+p1gYI67hdQGMN", "");
                nChainID = 56;
            }
            else if (sName.ToUpper() == "MATIC" || sName.ToUpper() == "POLYGON")
            {
                sURL = BMSCommon.Encryption.DecryptAES256("HZJFRXeUjIOYoLhnTZml74mOncolsbRUXs5u4eumDEkJGaF5p89lERR262qqsLgmZndhk3c1wg8x/0QLmvcwlGC7lM4kxO1EFFXqR4uqrDVEygWqPHzXijDoaADXJRAc", "");
                nChainID = 137;
            }
            else if (sName.ToUpper() == "ETH")
            {
                sURL = BMSCommon.Encryption.DecryptAES256("gKmVoM6fpZF79xuhQG1LdYbDx0TxWvtEbGdPeF0N2lu6UFFX1UWpLId0WL9Ewy1N7CknTMpmzV2WCAMmbriL1K5g/G5NwoGDraRzVK+jpIM=", "");
                nChainID = 1;
            }
            return sURL;
        }

        public static async Task<double> GetERC20AccountBalance(string sNetwork, string sERCAccount)
        {
            if (String.IsNullOrEmpty(sERCAccount))
            {
                return 0;
            }
            // Note: This is to get an ACCOUNT balance (not a smart contract balance)
            int nChainID = 0;
            string sPoint = GetEtherEndpoint(sNetwork, out nChainID);
            var web3 = new Web3(sPoint);
            try
            {
                HexBigInteger b1 = await web3.Eth.GetBalance.SendRequestAsync(sERCAccount);
                BigInteger b2 = b1.Value;
                double nOut = BMSCommon.Common.GetDouble(b2.ToString()) / 1000000000000000000;
                return nOut;
            }
            catch (Exception ex)
            {
                BMSCommon.Common.Log("GAB::" + ex.Message);
                return 0;
            }
        }
        */



        public static async Task<List<Portfolios>> QueryUTXOList2(bool fTestNet, string sBBPAddress, string sERCAddress, int nTimestamp)
        {
            List<Portfolios> l = new List<Portfolios>();
            // Cache Check
            bool fExists = dictUTXO.TryGetValue(sBBPAddress, out l);
            if (fExists)
            {
                int nElapsed = BMSCommon.Common.UnixTimestamp() - l[0].Time;

                if (nElapsed < (60 * 60 * 8))
                {
                    return l;
                }
            }
            l = new List<Portfolios>();
            // validate the address(es) both bbp and erc
            double nBBP = BMSCommon.WebRPC.GetBBPPosition(fTestNet, sBBPAddress);
            List<ERCAsset> lAssets = await QueryTokenBalances(fTestNet, sERCAddress);

            if (nBBP == 0 && false)
                return l;

            Portfolios pBBP = new Portfolios();
            pBBP.Ticker = "BBP";
            pBBP.AmountBBP = nBBP;
            pBBP.Address = sBBPAddress;
            pBBP.Time = BMSCommon.Common.UnixTimestamp();
            price1 priceBBP = BMSCommon.Pricing.GetCryptoPrice("BBP/BTC");
            pBBP.CryptoPrice = priceBBP.AmountUSD;
            l.Add(pBBP);

            for (int i = 0; i < lAssets.Count; i++)
            {
                ERCAsset l0 = lAssets[i];
                if (l0.Amount > 0)
                {
                    Portfolios p1 = new Portfolios();
                    p1.Ticker = l0.Symbol;
                    p1.AmountForeign = l0.Amount;
                    p1.Address = l0.ERCAddress;
                    p1.Time = BMSCommon.Common.UnixTimestamp();
                    double nQuote = await GetChainLinkPrice(l0);
                    if (p1.Ticker.ToLower() == "wbbp")
                        nQuote = priceBBP.AmountUSD;
                    if (p1.Ticker.ToLower() == "shib")
                    {
                        bool f11000 = false;

                    }
                    if (nQuote == 0)
                    {
                        p1.CryptoPrice = l0.Price;
                    }
                    else
                    {
                        p1.CryptoPrice = nQuote;
                    }
                    l.Add(p1);
                }
            }
            dictUTXO.Remove(sBBPAddress);
            dictUTXO.Add(sBBPAddress, l);
            return l;
        }

        public static async Task<double> GetSumOfUTXOs(bool fTestNet)
        {
            Dictionary<string, PortfolioParticipant> u = await GenerateUTXOReport(fTestNet);
            double nTotal = 0;
            foreach (KeyValuePair<string, PortfolioParticipant> pp in u)
            {
                nTotal += pp.Value.AmountBBP;
            }
            return nTotal;
        }

        public struct DwuPack
        {
            public double nDWU;
            public double nBonusPercent;
            public double TotalBBPLocked;
            public double TotalRXBBPLocked;
        }

        public static async Task<DwuPack> GetDWU(bool fTestNet)
        {
            double nLimit = nSuperblockLimit;
            DwuPack d = new DwuPack();
            d.nBonusPercent = 0;

            price1 nBTCPrice = GetCryptoPrice("BTC/USD");
            price1 nBBPPrice = GetCryptoPrice("BBP/BTC");

            double nUSDBBP = nBTCPrice.AmountUSD * nBBPPrice.Amount;
            double nAnnualBBPPaid = nLimit * 365;
            double nNativeTotal = await GetSumOfUTXOs(fTestNet);
            double nDWU = nAnnualBBPPaid  / (nNativeTotal + .01);
            if (nDWU > 2.0)
                nDWU = 2.0;
            d.nDWU = nDWU;

            Dictionary<string, PortfolioParticipant> u = await BBPTestHarness.BlockChairTestHarness.GenerateUTXOReport(fTestNet);
            List<PPUser> lPP = new List<PPUser>();
            double nTotalRX = 0;
            double nTotalLocked = 0;
            foreach (KeyValuePair<string, PortfolioParticipant> pp in u)
            {
                if (pp.Value.RXRewards > RX_REWARDS_THRESHHOLD)
                {
                    nTotalRX += pp.Value.AmountBBP;
                }
                nTotalLocked += pp.Value.AmountBBP;
            }

            d.TotalBBPLocked = nTotalLocked;
            d.TotalRXBBPLocked = nTotalRX;
            double nJuices = BMSCommon.BitcoinSync.GetAdditionalPortfolioBuilderJuices(fTestNet);
            d.nBonusPercent = d.TotalRXBBPLocked / (nJuices + .01);
            if (d.nBonusPercent > 2)
                d.nBonusPercent = 2;

            return d;
        }


        public static int RX_REWARDS_THRESHHOLD = 10;

        public static async Task<Dictionary<string, PortfolioParticipant>> GenerateUTXOReport(bool fTestNet)
        {
            DataTable dt = GetActivePortfolioBuilderUsers(fTestNet);
            Dictionary<string, PortfolioParticipant> dictParticipants = new Dictionary<string, PortfolioParticipant>();
            List<string> lUsedAddresses = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                PortfolioParticipant pp = new PortfolioParticipant();
                string sUserERC = dt.Rows[i]["ERC20Address"].ToString();
                string sBBPAddress = dt.Rows[i]["pbaddress"].ToString();
                // if either the erc20 is used, or the bbpaddress is used, skip.
                if (lUsedAddresses.Contains(sBBPAddress) || lUsedAddresses.Contains(sUserERC))
                {
                    continue;
                }
                bool fValidBBPAddress = BMSCommon.WebRPC.ValidateBiblepayAddress(fTestNet, sBBPAddress);
                if (!fValidBBPAddress)
                    continue;

                // If the BBP address is not signed, skip
                double nCurTime = BMSCommon.Common.UnixTimestamp();
                if (nCurTime > (1647045300 + (86400 * 7)))
                {
                    string sSig = dt.Rows[i]["pbsig"].ToString();
                    string sMsg = BMSCommon.Encryption.GetSha256HashI(dt.Rows[i]["ERC20Address"].ToString());
                    bool fValidSig = BMSCommon.WebRPC.VerifySignature(fTestNet, sBBPAddress, sMsg, sSig);

                    if (!fValidSig)
                        continue;
                }

                if (sUserERC.Length > 10)
                {
                    lUsedAddresses.Add(sUserERC);
                }

                if (sBBPAddress.Length > 10)
                {
                    lUsedAddresses.Add(sBBPAddress);
                }
                bool fPortfolioParticipantExists = dictParticipants.TryGetValue(dt.Rows[i]["ERC20Address"].ToString(), out pp);
                if (!fPortfolioParticipantExists)
                {
                    pp.lPortfolios = new List<Portfolios>();
                    dictParticipants.Add(dt.Rows[i]["ERC20Address"].ToString(), pp);
                }

                List<Portfolios> p = new List<Portfolios>();

                try
                {
                    p = await QueryUTXOList2(fTestNet, sBBPAddress, sUserERC, 0);
                }
                catch (Exception ex)
                {
                    string sMyTest = ex.Message;
                }

                pp.NickName = dt.Rows[i]["NickName"].ToString();

                pp.UserID = dt.Rows[i]["ERC20Address"].ToString();
                pp.RewardAddress = sBBPAddress;

                Portfolios pTotal = new Portfolios();
                try
                {
                    pTotal = GetPortfolioSum(p);
                }
                catch (Exception)
                {

                }
                pp.AmountForeign += pTotal.AmountForeign;
                pp.AmountUSDBBP += pTotal.AmountUSDBBP;
                pp.AmountUSDForeign += pTotal.AmountUSDForeign;
                pp.AmountBBP += pTotal.AmountBBP;

                for (int k = 0; k < p.Count; k++)
                {
                    Portfolios indPortfolio = p[k];

                    indPortfolio.AmountUSDBBP = indPortfolio.CryptoPrice * indPortfolio.AmountBBP;
                    indPortfolio.AmountUSDForeign = indPortfolio.CryptoPrice * indPortfolio.AmountForeign;
                    pp.lPortfolios.Add(indPortfolio);

                }

                pp.Coverage = pp.AmountUSDBBP / (pp.AmountUSDForeign + .01);
                if (pp.Coverage > 1)
                    pp.Coverage = 1;
                dictParticipants[dt.Rows[i]["ERC20Address"].ToString()] = pp;

            }

            double nTotalUSD = 0;
            double nParticipants = 0;
            foreach (KeyValuePair<string, PortfolioParticipant> pp in dictParticipants.ToList())
            {
                PortfolioParticipant p1 = dictParticipants[pp.Key];
                p1.AmountUSD = pp.Value.AmountUSDBBP + (pp.Value.AmountUSDForeign * pp.Value.Coverage);
                dictParticipants[pp.Key] = p1;
                nTotalUSD += p1.AmountUSD;
                nParticipants++;
            }

            // Assign Strength
            foreach (KeyValuePair<string, PortfolioParticipant> pp in dictParticipants.ToList())
            {
                PortfolioParticipant p1 = dictParticipants[pp.Key];
                p1.Strength = (p1.AmountUSD / (nTotalUSD + .01)) * .99;
                dictParticipants[pp.Key] = p1;
            }
            double nRXMagnitude = 0;
            // Calculate the total magnitude of RX miners
            foreach (KeyValuePair<string, PortfolioParticipant> pp in dictParticipants.ToList())
            {
                if (pp.Value.RXRewards > RX_REWARDS_THRESHHOLD)
                {
                    nRXMagnitude += pp.Value.Strength;
                }
            }
            double nRXMultiplier = 1 / (nRXMagnitude + .01);
            // Assign the Bonus
            foreach (KeyValuePair<string, PortfolioParticipant> pp in dictParticipants.ToList())
            {
                PortfolioParticipant p1 = dictParticipants[pp.Key];
                if (pp.Value.RXRewards > RX_REWARDS_THRESHHOLD)
                {
                    p1.BonusMagnitude = pp.Value.Strength * nRXMultiplier;
                }
                dictParticipants[pp.Key] = p1;
            }

            return dictParticipants;
        }


    }
}
