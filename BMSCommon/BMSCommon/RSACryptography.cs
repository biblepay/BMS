using System;
using System.Security.Cryptography;
using System.Text;
using static BMSCommon.Common;

namespace BMSCommon
{
    public class RSAKeyPair
    {
        public string PubKey;
        public string PrivKey;
    }
    public static class RSACryptography
    {
        const int PROVIDER_RSA_FULL = 1;
        const string CONTAINER_NAME = "KeyContainer";

        public static RSAKeyPair GenerateKeyPair()
        {
            CspParameters cspParams;
            cspParams = new CspParameters(PROVIDER_RSA_FULL);
            cspParams.KeyContainerName = CONTAINER_NAME;
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams);
            rsa.PersistKeyInCsp = false;
            RSAKeyPair kp = new RSAKeyPair();
            kp.PrivKey = rsa.ToXmlString(true);
            kp.PubKey = rsa.ToXmlString(false);
            return kp;
        }

        static public byte[] EncryptRSA(byte[] Data, string sPubKey)
        {
            try
            {
                byte[] encryptedData;
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(sPubKey);
                encryptedData = RSA.Encrypt(Data, false);
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static string EncryptRSA(string sData, string sPubKey)
        {
            byte[] bIn = Encoding.ASCII.GetBytes(sData);
            byte[] bOut = EncryptRSA(bIn, sPubKey);
            string sOut = ByteArrayToHexString(bOut);
            return sOut;
        }


        public static byte[] DecryptRSA(byte[] Data, string sPrivKey)
        {
            try
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(sPrivKey);
                byte[] decryptedData = RSA.Decrypt(Data, false);
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public static string DecryptRSA(string sData, string sPrivKey)
        {
            byte[] bIn = StringToByteArr(sData);
            byte[] bOut = DecryptRSA(bIn, sPrivKey);
            string sOut = System.Text.Encoding.Default.GetString(bOut);
            return sOut;
        }
    }
}
