using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BMSCommon.Model;
using static BiblePay.BMS.DSQL.SessionHelper;
using BMSShared;
using static BMSCommon.Common;
using BBPAPI;
using static BMSCommon.Encryption;
using BMSCommon;
using NBitcoin;
using BiblePay.BMS.Extensions;

namespace BiblePay.BMS.DSQL
{
    public static class UIWallet
    {
        public static double ConvertUSDToBiblePay(double nUSD)
        {
            price1 nBTCPrice = PricingService.GetCryptoPrice("BTC/USD");
            price1 nBBPPrice = PricingService.GetCryptoPrice("BBP/BTC");
            double nUSDBBP = nBTCPrice.AmountUSD * nBBPPrice.Amount;
            double nOut = nUSD / (nUSDBBP + .000000001);
            return nOut;
        }

        public static double ConvertBBPToUSD(double nBBP)
        {
            price1 nBTCPrice = PricingService.GetCryptoPrice("BTC/USD");
            price1 nBBPPrice = PricingService.GetCryptoPrice("BBP/BTC");
            double nUSDBBP = nBTCPrice.AmountUSD * nBBPPrice.Amount;
            double nOut = nUSDBBP * nBBP;
            return nOut;
        }

        /*
        public static Encryption.KeyType GetKeyPair(HttpContext h, string sNonce = "")
        {
            // If they are running the Core Wallet, use the core keypair, otherwise use the HttpContext.
            KeyType ukp = BBPAPI.ServiceInit.GetUserKeyPair(h.GetCurrentUser(),IsTestNet(h));
            if (!String.IsNullOrEmpty(ukp.PrivKey))
            {
                return ukp;
            }
            string ERC20Signature;
            string ERC20Address;
            Encryption.KeyType k = new Encryption.KeyType();
            h.Request.Cookies.TryGetValue("erc20signature", out ERC20Signature);
            h.Request.Cookies.TryGetValue("erc20address", out ERC20Address);


            return k;
        }
        */


        // This is only used by MFA - it derives a bbp key by 2fa seed
        public static BBPKeyPair GetKeyPairByGUID(bool fTestNet, string sGUID, string sNonce = "")
        {
            //Encryption.KeyType k = new Encryption.KeyType();
            BBPKeyPair p = new BBPKeyPair();
            string sDerivationSource = sGUID + sNonce;
            p = Encryption.DeriveKey(fTestNet, sDerivationSource);
            return p;
        }


        // Only used by GenerateToken() proof of concept:
        public static BBPKeyPair GetKeyPair3(bool fTestNet, string sDerivSource)
        {
            BBPKeyPair k = Encryption.DeriveKey(fTestNet, sDerivSource);
            return k;
        }

    }
}
