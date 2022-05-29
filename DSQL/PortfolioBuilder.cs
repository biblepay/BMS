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
using static BMSCommon.PortfolioBuilder;

namespace BiblePay.BMS.DSQL
{
    public static class PB
    {
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
                        string sTD = "<td class='highlight'>";
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






    }
}
