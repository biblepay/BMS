using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSCommon
{
    public static class PortfolioBuilder
    {



        public static double nSuperblockLimit = 125000;
        public static List<PPUser> lLastPB = new List<PPUser>();
        public static Dictionary<string, List<Portfolios>> dictUTXO = new Dictionary<string, List<Portfolios>>();
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

        public static DataTable GetActivePortfolioBuilderUsers(bool fTestNet)
        {
            string sPBField = fTestNet ? "tPortfolioBuilderAddress" : "PortfolioBuilderAddress";
            string sPBSigField = fTestNet ? "tPBSignature" : "PBSignature";
            string sUserTable = fTestNet ? "user" : "user";
            string sql = "Select NickName,ERC20Address," + sPBField + " as pbaddress," + sPBSigField + " as pbsig from " + sUserTable + " where " + sPBField + " is not null and ERC20Address is not null and LENGTH(ERC20Address) > 20;";
            MySqlCommand q = new MySqlCommand(sql);
            DataTable dt1 = BMSCommon.Database.GetDataTable(q);
            return dt1;
        }

        public static async Task<List<Portfolios>> QueryUTXOList2(bool fTestNet, string sBBPAddress, string sERCAddress, int nTimestamp)
        {
            List<Portfolios> l = new List<Portfolios>();
            // Cache Check
            bool fExists = dictUTXO.TryGetValue(sBBPAddress, out l);
            if (fExists)
            {
                int nElapsed = BMSCommon.Common.UnixTimestamp() - l[0].Time;

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
                    p1.Time = BMSCommon.Common.UnixTimestamp();
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



    }
}
