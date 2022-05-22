using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using static BMSCommon.CryptoUtils;
using Microsoft.AspNetCore.Http;
using static BMSCommon.Pricing;

namespace BiblePay.BMS.DSQL
{
    public static class PortfolioBuilder
    {
        public static List<PPUser> lLastPB = new List<PPUser>();
        public static int nLastPB = 0;

        public struct PortfolioParticipant
        {
            public string UserID;
            public string RewardAddress;
            public double AmountBBP;
            public double AmountForeign;
            public double AmountUSD;
            public double AmountUSDBBP;
            public double AmountUSDForeign;
            public double Coverage;
            public string NickName;
            public double Strength;
            public List<Portfolios> lPortfolios;
        }


        private static double nSuperblockLimit = 125000;
        public struct PPUser
        {
            public string RewardAddress;
            public string NickName;
            public double Earnings;
            public double Strength;
            public double Coverage;
            public List<PPDetail> lDetail;
        };

        public struct PPDetail
        {
            public double AmountBBP;
            public double AmountForeign;
            public double AmountUSDBBP;
            public double AmountUSDForeign;
            public string Ticker;
            public double CryptoPrice;
        };

        public struct SimpleUTXO
        {
            public double nAmount;
            public string TXID;
            public int nOrdinal;
            public int nHeight;
            public string Address;
            public string Ticker;
        };

        public struct Portfolios
        {
            public string UserID;
            public double AmountBBP;
            public double AmountForeign;
            public double AmountUSDBBP;
            public double AmountUSDForeign;
            public double Coverage;
            public string Ticker;
            public string Nickname;
            public double Strength;
            public string Address;
            public double CryptoPrice;
            public int Time;
            public List<SimpleUTXO> lPositions;
        }

        public static Dictionary<string, List<Portfolios>> dictUTXO = new Dictionary<string, List<Portfolios>>();
        public static async Task<List<PPUser>> GetLeaderboardJson(bool fTestNet)
        {
            Dictionary<string, PortfolioParticipant> u = await GenerateUTXOReport(fTestNet);

            List<PPUser> lPP = new List<PPUser>();
            double nStrengthTotal = 0;

            foreach (KeyValuePair<string, PortfolioParticipant> pp in u)
            {
                if (pp.Value.Strength > 0)
                {
                    PPUser u1 = new PPUser();
                    u1.lDetail = new List<PPDetail>();
                    u1.RewardAddress = pp.Value.RewardAddress;
                    u1.NickName = pp.Value.NickName;
                    u1.Earnings = nSuperblockLimit * pp.Value.Strength;
                    u1.Strength = pp.Value.Strength * 100;
                    u1.Coverage = pp.Value.Coverage * 100;
                    nStrengthTotal += Math.Round(u1.Strength, 2);


                    for (int i = 0; i < pp.Value.lPortfolios.Count; i++)
                    {
                        if (pp.Value.lPortfolios[i].AmountBBP > 0 || pp.Value.lPortfolios[i].AmountForeign > 0)
                        {
                            PPDetail d2 = new PPDetail();
                            d2.Ticker = pp.Value.lPortfolios[i].Ticker;
                            d2.AmountBBP = pp.Value.lPortfolios[i].AmountBBP;
                            d2.AmountForeign = Math.Round(pp.Value.lPortfolios[i].AmountForeign, 2);
                            d2.AmountUSDBBP = Math.Round(pp.Value.lPortfolios[i].AmountUSDBBP, 2);
                            d2.AmountUSDForeign = Math.Round(pp.Value.lPortfolios[i].AmountUSDForeign, 2);
                            d2.CryptoPrice = pp.Value.lPortfolios[i].CryptoPrice;
                            u1.lDetail.Add(d2);
                        }
                    }
                    lPP.Add(u1);
                }
            }
            return lPP;
        }


        public static async Task<string> GetLeaderboard(HttpContext h, bool fTestNet)
        {
            string sMode = DSQL.UI.GetPBMode(h);
            
            string html = "<div style='font-size:10px;'><table class=saved>";
            // Column headers
            string sRow = "<tr><th width=5%>Address<th width=7%>Nick Name<th>Currency<th>Total BBP<th width=5%><small>Ttl Foreign</small><th>USD Value BBP<th width=5%><small>USD Value Foreign</small><th>Assessed USD<th>Coverage<th>Earnings<th>Strength</tr>";
            html += sRow;
            Dictionary<string, PortfolioParticipant> u = await GenerateUTXOReport(fTestNet);

            foreach (KeyValuePair<string, PortfolioParticipant> pp in u)
            {
                double nEarnings = nSuperblockLimit * pp.Value.Strength;
                if (pp.Value.Strength > -1)
                {
                    sRow = "<tr><td><font style='font-size:7px;'>" + pp.Value.RewardAddress
                        + "</font><td>" + pp.Value.NickName
                        + "<td>Various"
                        + "<td>" + Math.Round(pp.Value.AmountBBP, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.AmountForeign, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.AmountUSDBBP, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.AmountUSDForeign, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.AmountUSD, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.Coverage * 100, 2).ToString() + "%"
                        + "<td>" + Math.Round(nEarnings, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.Strength * 100, 2).ToString() + "%</tr>";
                    html += sRow;
                    if (sMode.ToLower() == "detail")
                    {
                        string sTD = "<td style='background-color:grey;'>";
                        for (int i = 0; i < pp.Value.lPortfolios.Count; i++)
                        {
                            if (pp.Value.lPortfolios[i].AmountBBP > 0 || pp.Value.lPortfolios[i].AmountForeign > 0)
                            {
                                sRow = "<tr>" + sTD + sTD + sTD + pp.Value.lPortfolios[i].Ticker
                                    + sTD + Math.Round(pp.Value.lPortfolios[i].AmountBBP, 2).ToString()
                                    + sTD + Math.Round(pp.Value.lPortfolios[i].AmountForeign, 2).ToString()
                                    + sTD + Math.Round(pp.Value.lPortfolios[i].AmountUSDBBP, 2).ToString()
                                    + sTD + Math.Round(pp.Value.lPortfolios[i].AmountUSDForeign, 2).ToString()
                                    + sTD + sTD + sTD + sTD;
                                html += sRow;
                            }
                        }
                    }
                }
            }
            html += "</table></div>";
            return html;
        }


        public static DataTable GetActivePortfolioBuilderUsers(bool fTestNet)
        {
            string sPBField = fTestNet ? "tPortfolioBuilderAddress" : "PortfolioBuilderAddress";
            string sPBSigField = fTestNet ? "tPBSignature" : "PBSignature";
            string sUserTable = fTestNet ? "user" : "user";
            string sql = "Select NickName,ERC20Address," + sPBField + " as pbaddress," + sPBSigField + " as pbsig from " + sUserTable + " where " + sPBField + " is not null and ERC20Address is not null and LENGTH(ERC20Address) > 20;";
            MySqlCommand q = new MySqlCommand(sql);
            DataTable dt1 = BMSCommon.Database.GetMySqlDataTable(fTestNet, q, "");
            return dt1;
        }


        public static Portfolios GetPortfolioSum(List<Portfolios> p)
        {
            Portfolios n = new Portfolios();
            n.AmountForeign = 0;
            n.AmountBBP = 0;
            for (int k = 0; k < p.Count; k++)
            {
                Portfolios pActive = p[k];
                BMSCommon.Pricing.price1 prc = new BMSCommon.Pricing.price1(); 

                if (pActive.Ticker == "BBP")
                {
                    n.AmountBBP += pActive.AmountBBP;
                    n.AmountUSDBBP += (pActive.CryptoPrice * pActive.AmountBBP);
                }
                else
                {
                    n.AmountForeign += pActive.AmountForeign;
                    n.AmountUSDForeign += (pActive.CryptoPrice * pActive.AmountForeign);
                    string sTest1000 = "";
                }
            }
            return n;
        }



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
                //User u = BMSCommon.CryptoUtils.DepersistUser(sUserERC);
                

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

            return dictParticipants;
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


        public static async Task<double> GetDWU(bool fTestNet)
        {
            double nLimit = 350000;
            price1 nBTCPrice = GetCryptoPrice("BTC");
            price1 nBBPPrice = GetCryptoPrice("BBP");
            double nUSDBBP = nBTCPrice.AmountUSD * nBBPPrice.Amount;
            double nAnnualReward = nLimit * 365 * nUSDBBP;
            double nNativeTotal = await GetSumOfUTXOs(fTestNet);
            double nGlobalBBPPortfolio = nNativeTotal * nUSDBBP;
            double nDWU = nAnnualReward / (nGlobalBBPPortfolio + .01);
            //LogPrintf("\nReward %f, Total %f, DWU %f, USDBBP %f ", nAnnualReward, (double)nNativeTotal / COIN, nDWU, nUSDBBP);
            if (nDWU > 2.0)
                nDWU = 2.0;
            return nDWU;
        }


        public static async Task<List<Portfolios>> QueryUTXOList2(bool fTestNet, string sBBPAddress, string sERCAddress, int nTimestamp)
        {
            List<Portfolios> l = new List<Portfolios>();
            // Cache Check
            bool fExists = dictUTXO.TryGetValue(sBBPAddress, out l);
            if (fExists)
            {
                int nElapsed =BMSCommon.Common.UnixTimestamp() - l[0].Time;

                if (nElapsed < (60 * 30))
                {
                    return l;
                }
            }

            l = new List<Portfolios>();

            // validate the address(es) both bbp and erc
            double nBBP = BMSCommon.WebRPC.GetBBPPosition(fTestNet, sBBPAddress);
            List<BMSCommon.Pricing.Asset> lAssets = await BMSCommon.Pricing.QueryTokenBalances(sERCAddress);

            if (nBBP == 0 && false)
                return l;

            Portfolios pBBP = new Portfolios();
            pBBP.Ticker = "BBP";
            pBBP.AmountBBP = nBBP;
            pBBP.Address = sBBPAddress;
            pBBP.Time = BMSCommon.Common.UnixTimestamp();
           
            BMSCommon.Pricing.price1 priceBBP = BMSCommon.Pricing.GetCryptoPrice("BBP");
            pBBP.CryptoPrice = priceBBP.AmountUSD;
            l.Add(pBBP);

            for (int i = 0; i < lAssets.Count; i++)
            {
                BMSCommon.Pricing.Asset l0 = lAssets[i];
                if (l0.Amount > 0)
                {
                    Portfolios p1 = new Portfolios();
                    p1.Ticker = l0.Symbol;
                    p1.AmountForeign = l0.Amount;
                    p1.Address = l0.ERCAddress;
                    p1.Time =BMSCommon.Common.UnixTimestamp();
                    double nQuote = await BMSCommon.Pricing.GetChainLinkPrice(l0.Chain, l0.ChainlinkAddress);
                    if (p1.Ticker.ToLower() == "wbbp")
                        nQuote = priceBBP.AmountUSD;
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






    }
}
