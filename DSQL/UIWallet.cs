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

        public static Encryption.KeyType GetKeyPair(HttpContext h, string sNonce = "")
        {
            string ERC20Signature;
            string ERC20Address;
            Encryption.KeyType k = new Encryption.KeyType();
            h.Request.Cookies.TryGetValue("erc20signature", out ERC20Signature);
            h.Request.Cookies.TryGetValue("erc20address", out ERC20Address);
            if (ERC20Signature != null)
            {
                BBPAPI.ERC712Authenticator b = new BBPAPI.ERC712Authenticator();
                bool f = b.VerifyERC712Signature(ERC20Signature, ERC20Address);
                if (f)
                {
                    string sDerivationSource = ERC20Signature + sNonce;
                    k = Encryption.DeriveKey(IsTestNet(h), sDerivationSource);
                    return k;
                }
            }
            return k;
        }

        public static Encryption.KeyType GetKeyPairByGUID(bool fTestNet, string sGUID, string sNonce = "")
        {
            Encryption.KeyType k = new Encryption.KeyType();
            string sDerivationSource = sGUID + sNonce;
            k = Encryption.DeriveKey(fTestNet, sDerivationSource);
            return k;
        }


        public static Encryption.KeyType GetKeyPair2(bool fTestNet, string sERC20Address, string sSig, string sNonce = "")
        {
            Encryption.KeyType k = new Encryption.KeyType();
            BBPAPI.ERC712Authenticator b = new BBPAPI.ERC712Authenticator();
            bool f = b.VerifyERC712Signature(sSig, sERC20Address);
            if (f)
            {
                string sDerivationSource = sSig + sNonce;
                k = Encryption.DeriveKey(fTestNet, sDerivationSource);
                return k;
            }
            return k;
        }

        public static KeyType GetKeyPair3(bool fTestNet, string sDerivSource)
        {
            KeyType k = new KeyType();
            k = Encryption.DeriveKey(fTestNet, sDerivSource);
            return k;
        }

    }
}
