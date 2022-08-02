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

        public static double nSuperblockLimit = 270427;
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
            string sUserTable = fTestNet ? "tuser" : "user";
            string sql = "Select NickName,ERC20Address," + sPBField + " as pbaddress," + sPBSigField + " as pbsig from " + sUserTable + " where " + sPBField + " is not null and ERC20Address is not null and LENGTH(ERC20Address) > 20;";
            MySqlCommand q = new MySqlCommand(sql);
            DataTable dt1 = BMSCommon.Database.GetDataTable(q);
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






    }
}
