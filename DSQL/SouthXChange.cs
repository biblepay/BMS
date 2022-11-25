using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BiblePay.BMS.Extensions;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using SouthXchange;
using SouthXchange.Model;
using static BMSCommon.BitcoinSyncModel;
using static BMSCommon.CryptoUtils;
using static BMSCommon.Model;
using static BMSCommon.Pricing;
using static BMSCommon.StringExtension;

namespace BiblePay.BMS.DSQL
{
    public class SouthXChange
    {
        public static bool PushDeposit(string sSymbol, double nAmount, string sHash, string sAddress)
        {
            string sql = "Insert into SouthXChange (id, hash, symbol, added, amount, status, address) values (uuid(), @hash, @symbol, now(), @amt, 'RECVD', @address);";
            MySqlCommand m1 = new MySqlCommand(sql);
            m1.Parameters.AddWithValue("@hash", sHash);
            m1.Parameters.AddWithValue("@symbol", sSymbol);
            m1.Parameters.AddWithValue("@amt", nAmount);
            m1.Parameters.AddWithValue("@address", sAddress);
            bool f = BMSCommon.Database.ExecuteNonQuery2(m1);
            return f;
        }

        public static bool UpdateDeposit(string hash, string BBPTXID, double nBBPAmount, string sStatus)
        {
            string sql = "Update SouthXChange Set BBPTXID=@bbptxid,BBPAmount=@bbpamt,STATUS=@status where hash=@hash;";
            MySqlCommand m1 = new MySqlCommand(sql);
            m1.Parameters.AddWithValue("@hash", hash);
            m1.Parameters.AddWithValue("@bbptxid", BBPTXID);
            m1.Parameters.AddWithValue("@bbpamt", nBBPAmount);
            m1.Parameters.AddWithValue("@status", sStatus);
            bool f = BMSCommon.Database.ExecuteNonQuery2(m1);
            return f;
        }

        public static bool PushAddress(string sSymbol, string sERC20Address, string sAddress)
        {
            string sql = "Insert into UserAddress (id, added, symbol, address, ERC20Address) values (uuid(), now(), @symbol, @address, @erc20);";
            MySqlCommand m1 = new MySqlCommand(sql);
            m1.Parameters.AddWithValue("@symbol", sSymbol);
            m1.Parameters.AddWithValue("@address",sAddress);
            m1.Parameters.AddWithValue("@erc20", sERC20Address);
            bool f = BMSCommon.Database.ExecuteNonQuery2(m1);
            return f;
        }

        public static string GetSXAddressByERC20Address(string sSymbol, string sERC20Address)
        {
            string sql = "Select * from UserAddress where ERC20Address=@erc20 and symbol=@symbol;";
            MySqlCommand m1 = new MySqlCommand(sql);
            m1.Parameters.AddWithValue("@symbol", sSymbol);
            m1.Parameters.AddWithValue("@erc20", sERC20Address);
            string sAddress = BMSCommon.Database.GetScalarString(m1, "address");
            return sAddress;
        }

        public static int MAX_BBP_PAYMENT = 10000000;
        public static string PayUnpaidDeposits(HttpContext h, bool fTestNet, string sERC20, string sBBPAddress, string sForeignAddress, string sSymbol)
        {
            string sql = "Select * from SouthXChange where symbol=@symbol and address=@fa and STATUS='RECVD' and BBPTXID is null;";
            MySqlCommand m1 = new MySqlCommand(sql);

            bool bValid = BMSCommon.WebRPC.ValidateBiblepayAddress(fTestNet, sBBPAddress);
            if (!bValid)
                return "BBP_PAY_ADDRESS_INVALID";

            if (sForeignAddress == String.Empty || sForeignAddress == null)
                return "FOREIGN_ADDRESS_EMPTY";

            string poolAccount = BMSCommon.Common.GetConfigurationKeyValue("PoolPayAccount");
            if (poolAccount == String.Empty)
                return "BMS_NOT_CONFIGURED";
            // Verify the bbp price is actually healthy
            price1 nBTCPrice = BMSCommon.Pricing.GetCryptoPrice("BTC");
            price1 nBBPPrice = BMSCommon.Pricing.GetCryptoPrice("BBP");
            price1 nDOGEPrice = BMSCommon.Pricing.GetCryptoPrice("DOGE");

            double nUSDBBP = nBTCPrice.AmountUSD * nBBPPrice.Amount;
            double nUSDDOGE = nBTCPrice.AmountUSD * nDOGEPrice.Amount;

            if (nUSDBBP < .000015)
            {
                //bbp price below 15 milli sat-usd
                return "BBP_PRICE_TOO_LOW";
            }

            if (nUSDDOGE < .01)
            {
                return "DOGE_PRICE_TOO_LOW";
            }

            double nBBPPerDoge = nUSDDOGE / nUSDBBP;

            m1.Parameters.AddWithValue("@symbol", sSymbol);
            m1.Parameters.AddWithValue("@fa", sForeignAddress);

            DataTable dt = BMSCommon.Database.GetDataTable2(m1);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sID = dt.Rows[i]["id"].ToString();
                double nDogeAmount = BMSCommon.Common.GetDouble(dt.Rows[i]["amount"].ToString());
                double nBBPAmount = nBBPPerDoge * nDogeAmount;
                if (nBBPAmount < MAX_BBP_PAYMENT)
                {
                    string sUpd = "Update SouthXChange set STATUS='PAYING' where id='" + sID + "';";
                    MySqlCommand mUpd = new MySqlCommand(sUpd);
                    BMSCommon.Database.ExecuteNonQuery2(mUpd);
                    List<ChainPayment> Payments = new List<ChainPayment>();
                    ChainPayment p = new ChainPayment();
                    p.bbpaddress = sBBPAddress;
                    p.amount = nBBPAmount;
                    Payments.Add(p);
                    string txid = BMSCommon.WebRPC.SendMany(fTestNet, Payments, poolAccount, "PB Payments");
                    BMSCommon.Common.Log("PayUnpaidDeposits::Sent out " + p.amount.ToString() + " " + p.bbpaddress);
                    if (txid != null && txid != "" && txid.Length > 20)
                    {
                        UpdateDeposit(dt.Rows[i]["hash"].ToString(), txid, p.amount, "PAID");
                        DSQL.UI.GetAvatarBalanceNumeric(h, true);
                    }
                }
            }
            return String.Empty;
        }

        public async static Task<string> GetSouthXChangeReport(HttpContext h)
        {
            string sCurrency = "doge";
            User u0 = h.GetCurrentUser();

            if (!u0.LoggedIn)
                return "NOT_LOGGED_IN";
            string sERC20 = u0.ERC20Address;
            string sBBPAddress = u0.BBPAddress;
            if (String.IsNullOrEmpty(sERC20))
            {
                return "NO_ERC_ADDRESS";
            }
            if (String.IsNullOrEmpty(sBBPAddress))
            {
                return "NO_BBP_ADDRESS";
            }
            string sxc1 = BMSCommon.Common.GetConfigurationKeyValue("sxcontext1");
            if (sxc1 == String.Empty)
                return "NO_SXC_CONFIGURATION";
            var context = new SxcContext(sxc1, BMSCommon.Common.GetConfigurationKeyValue("sxcontext2"));

            string sDOGE = GetSXAddressByERC20Address(sCurrency, sERC20);
            if (sDOGE == String.Empty)
            {
                // Make new DOGE address;
                AddressModel a = await context.GenerateNewAddressAsync(sCurrency);
                if (a.Address.Length > 20)
                {
                    PushAddress(sCurrency, sERC20, a.Address);
                    sDOGE = a.Address;
                }
            }

            if (sDOGE == String.Empty)
                return "NO_FOREIGN_ADDRESS";

            // Insert any hashes that do not exist in our table
            string sql = "Select distinct hash from SouthXChange where symbol='" + sCurrency + "';";
            MySqlCommand m1 = new MySqlCommand(sql);
            List<string> l = BMSCommon.Database.GetDataList(m1, "hash");
            ListTransactionsRequest lr = new ListTransactionsRequest();
            PagedResult<ListTransactionsResult> e = await context.ListTransactionsAsync(sCurrency, 0, 100);
            for (int i = 0; i < e.Result.Length; i++)
            {
                if (e.Result[i].Type.ToLower() == "deposit")
                {
                    if (!l.Contains(e.Result[i].Hash))
                    {
                        PushDeposit(sCurrency, (double)e.Result[i].Amount, e.Result[i].Hash, e.Result[i].Address);
                    }
                }
            }
            // pay the unpaid deposits
            PayUnpaidDeposits(h, DSQL.UI.IsTestNet(h), sERC20, sBBPAddress, sDOGE, sCurrency);

            // generate the user report
            sql = "Select * from SouthXChange where symbol=@symbol and address=@fa;";
            m1 = new MySqlCommand(sql);
            m1.Parameters.AddWithValue("@symbol", sCurrency);
            m1.Parameters.AddWithValue("@fa", sDOGE);

            string html = "<table class='saved'><th>Date<th>DOGE TXID<th>Amount<th>BBP TXID<th>BBP Amount<th>Status";

            DataTable dt = BMSCommon.Database.GetDataTable2(m1);
            if (dt.Rows.Count == 0)
            {
                html = String.Empty;
                return html;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sRow = "<tr><td width=5%>" 
                    + dt.Rows[i]["Added"].ObjToMilitaryTime() 
                    + "<td width=5% style='font-size:7px;'>" 
                    + dt.Rows[i]["hash"].ToString()
                    + "<td>" 
                    + dt.Rows[i]["Amount"].ToString() 
                    + "<td td width=5% style='font-size:7px;'>"
                    + dt.Rows[i]["BBPTXID"].ToString() + "<td>" 
                    + dt.Rows[i]["BBPAmount"].ToString() 
                    + "<td>" + dt.Rows[i]["Status"].ToString() + "</tr>\r\n";

                html += sRow;
            }
            html += "</table>";

            return html;
        }
    }
}
