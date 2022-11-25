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
using BMSCommon;
using System.Net.Mail;
using static BMSCommon.BitcoinSyncModel;
using static BMSCommon.Model;

namespace BiblePay.BMS.DSQL
{
    public static class PB
    {
        public static async Task<List<PPUser>> GetLeaderboardJson(bool fTestNet)
        {
            Dictionary<string, PortfolioParticipant> u = await BBPTestHarness.BlockChairTestHarness.GenerateUTXOReport(fTestNet);

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
            string sRow = "<tr><th width=5%>Address<th width=7%>User";
            if (sMode.ToLower() != "summary")
            {
                sRow += "<th width=7%>Symbol";
            }
            sRow += "<th>Total BBP<th width=5%><small>Ttl Foreign</small><th>USD Value BBP<th width=5%><small>USD Value Foreign</small><th>Assessed USD<th>RX Performance<th>RX Magnitude<th>Coverage<th>Earnings<th>Strength</tr>";
            html += sRow;
            Dictionary<string, PortfolioParticipant> u = await BBPTestHarness.BlockChairTestHarness.GenerateUTXOReport(fTestNet);



            double nRXMagnitude = 0;
            // Calculate the total magnitude of RX miners
            foreach (KeyValuePair<string, PortfolioParticipant> pp in u)
            {
                if (pp.Value.RXRewards > 100)
                {
                    nRXMagnitude += pp.Value.Strength;
                }
            }
            double nRXMultiplier = 1 / (nRXMagnitude + .01);

            foreach (KeyValuePair<string, PortfolioParticipant> pp in u)
            {
                double nEarnings = nSuperblockLimit * pp.Value.Strength;
                if (pp.Value.Strength > -1)
                {
                    string sBioURL = await DSQL.UI.GetAvatarPicture(fTestNet, pp.Value.UserID);
                    string sAvatar = sBioURL + pp.Value.NickName;
                    sRow = "<tr><td><font style='font-size:7px;'>" + pp.Value.RewardAddress
                        + "</font>" + "<td>" + sAvatar;
                    if (sMode.ToLower() != "summary")
                    {
                        sRow += "<td>";
                    }
                    sRow += "<td>" + Math.Round(pp.Value.AmountBBP, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.AmountForeign, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.AmountUSDBBP, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.AmountUSDForeign, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.AmountUSD, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.RXRewards, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.BonusMagnitude, 2).ToString()
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
                                    + sTD + sTD + sTD + sTD + sTD + sTD;
                                html += sRow;
                            }
                        }
                    }
                }
            }
            html += "</table></div>";
            return html;
        }



        public static string InsertUTXODataIntoChainLegacy(bool fTestNet, string sMessageKey, string sUTXOReport)
        {
            try
            {
                string sBurnAddress = BMSCommon.Encryption.GetBurnAddress(fTestNet);
                string sTXID = "";
                DACResult r0 = DSQL.UI.SendBBPOldMethod(fTestNet, sMessageKey, sBurnAddress, 5, sUTXOReport);
                sTXID = r0.TXID;
                if (sTXID == "")
                {
                    throw new Exception("Unabled to add to memory pool.");
                }
                return sTXID;
            }
            catch (Exception)
            {
                BMSCommon.Common.Log("An error occured in the daily utxo report. ");
                return "";
            }
        }


        public static async Task<bool> DailyUTXOExport(bool fTestNet, bool fPrimary)
        {
            if (!fPrimary)
                return false;


            bool fCacheLatch = BMSCommon.Database.LatchOld(fTestNet, "cachelatch", 60 * 60);
            if (fCacheLatch)
            {
                await BBPTestHarness.BlockChairTestHarness.GenerateUTXOReport(fTestNet);
            }

            bool fLatch = BMSCommon.Database.LatchOld(fTestNet, "utxoexport", 60 * 30);
            if (!fLatch)
                return false;

            double nNextHeight = 0;

            try
            {
                bool fExists = BMSCommon.Retired.UTXORPC.GetNextContract(fTestNet, out nNextHeight);
                if (fExists || nNextHeight == 0)
                {
                    return false;
                }

                if (!fTestNet)
                {
                    BMSCommon.Common.Log("CREATING UTXO DAILY EXPORT FOR HEIGHT " + nNextHeight.ToString());
                }
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
                BitcoinSyncTransaction t = new BitcoinSyncTransaction();
                t.Time = BMSCommon.Common.UnixTimestamp();
                t.Data = Newtonsoft.Json.JsonConvert.SerializeObject(u);

                // Check utxo signature here
                bool fOK = true;
                if (fOK)
                {
                    string UtxoTXID = InsertUTXODataIntoChainLegacy(fTestNet, "GSC", sData);
                    if (UtxoTXID == "")
                    {
                        BMSCommon.Common.Log("Unable to persist utxo data");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
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
                    BBPTestHarness.Common.SendMail(false, m);
                }
                return false;
            }
        }



        public async static Task<string> ExecutePortfolioBuilderExport(bool fTestNet, int nNextHeight)
        {
            Dictionary<string, PortfolioBuilder.PortfolioParticipant> u = await BBPTestHarness.BlockChairTestHarness.GenerateUTXOReport(fTestNet);
            string sSummary = "<data><ver>3.0</ver>";


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



    }
}
