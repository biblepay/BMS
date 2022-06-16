using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static BMSCommon.Common;

namespace BMSCommon
{
    public static class BlockChair
    {


        public static bool ValidateAddressLength(string sAddress, int nReqLength)
        {
            int nLen = sAddress.Length;
            if (nLen != nReqLength)
                return false;
            return true;
        }

        public static string sTickers = "BBP,DASH,BTC,DOGE,ETH,LTC,XRP,XLM,ZEC,BCH";
        public static bool ValidateTicker(string sTicker)
        {
            string[] vTickers = sTickers.Split(",");
            for (int i = 0; i < vTickers.Length; i++)
            {
                if (vTickers[i] == sTicker)
                    return true;
            }
            return false;
        }

        public static bool ValidateForeignAddress(string sTicker, string sAddress)
        {
            if (sAddress.IsNullOrEmpty())
                return false;

            bool fValidateTicker = ValidateTicker(sTicker);
            if (!fValidateTicker)
                return false;

            if (sTicker == "DASH" || sTicker == "BTC" || sTicker == "DOGE" || sTicker == "BBP")
            {
                if (ValidateAddressLength(sAddress, 42))
                    return true;
                return ValidateAddressLength(sAddress, 34);
            }
            else if (sTicker == "LTC")
            {
                if (ValidateAddressLength(sAddress, 34))
                {
                    return true;
                }
                else
                {
                    return ValidateAddressLength(sAddress, 43);
                }
            }
            else if (sTicker == "ETH" || sTicker == "BCH")
            {
                return ValidateAddressLength(sAddress, 42);
            }
            else if (sTicker == "XRP")
            {
                return ValidateAddressLength(sAddress, 34);
            }
            else if (sTicker == "XLM")
            {
                return ValidateAddressLength(sAddress, 56);
            }
            else if (sTicker == "ZEC")
            {
                return ValidateAddressLength(sAddress, 35);
            }

            return false;
        }



        public struct BlockChairUTXO
        {
            public string Address;
            public string TXID;
            public double Amount;
            public double QueryAmount;
            public int Height;
            public int Ordinal;
            public int TxCount;
            public string Ticker;
            public int UTXOTxTime;
            public string Added;
            public bool found;
            public string Account;
            public double TotalBalance;
        };

        public static string GetMd5String(string sData)
        {
            byte[] arrData = System.Text.Encoding.UTF8.GetBytes(sData);
            var hash = System.Security.Cryptography.MD5.Create().ComputeHash(arrData);
            return ByteArrayToHexString(hash);
        }
        public static int HexToInteger(string hex)
        {
            int d = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return d;
        }
        public static double ConvertHexToDouble(string hex)
        {
            int d = HexToInteger(hex);
            double dOut = (double)d;
            return dOut;
        }

        public static double AddressToPin(string sBBPAddress, string sCryptoAddress)
        {
            string sConcat = sBBPAddress + sCryptoAddress;
            return AddressToPin0(sConcat);
        }
        public static double AddressToPin0(string sAddress)
        {
            if (sAddress.Length < 20)
                return -1;

            string sHash = GetMd5String(sAddress);
            string sMath5 = sHash.Substring(0, 5); // 0 - 1,048,575
            double d = ConvertHexToDouble("" + sMath5) / 11.6508;

            int nMin = 10000;
            int nMax = 99999;
            d += nMin;

            if (d > nMax)
                d = nMax;

            d = Math.Floor(d);
            return d;

            // Why a 5 digit pin?  Looking at the allowable suffix size (digits of scale after the decimal point), we have 8 in scale to work with.  
            // With BTC at $32,000 this would be $250 of value tied up at a minimum, in the pin suffix.
            // Therefore, we moved down to a 5 digit pin to make the reqt around $22 in latent value.
            // Note that this monetary overhead is not actually *lost*, it is simply tied up in the stake.
        }
        public static bool CompareMask(double nAmount, int nPin)
        {
            string sAmount = nAmount.ToString();
            bool fPin = sAmount.Contains(nPin.ToString());
            if (fPin)
                return fPin;
            string sPin = nPin.ToString();
            if (sPin.Substring(sPin.Length - 1, 1) == "0")
            {
                sPin = sPin.Substring(0, sPin.Length - 1);
                fPin = sAmount.Contains(sPin);
            }
            return fPin;
        }


        


 
    }
}
