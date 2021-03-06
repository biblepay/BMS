using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BMSCommon
{
    public static class Encryption
    {
        public struct KeyType
        {
            public string PrivKey;
            public string PubKey;
        }
        public static KeyType DeriveKey(bool fTestNet, string sSha)
        {
            NBitcoin.Mnemonic m = new NBitcoin.Mnemonic(sSha);
            NBitcoin.ExtKey k = m.DeriveExtKey(null);
            KeyType k1 = new KeyType();
            k1.PrivKey = k.PrivateKey.GetWif(fTestNet ? NBitcoin.Network.TestNet : NBitcoin.Network.Main).ToWif().ToString();
            k1.PubKey = k.ScriptPubKey.GetDestinationAddress(fTestNet ? NBitcoin.Network.TestNet : NBitcoin.Network.Main).ToString();
            return k1;
        }



        public static string GetBurnAddress(bool fTestNet)
        {
            // These are hardcoded in the biblepaycore wallet:
            string sBurnAddress = !fTestNet ? "B4T5ciTCkWauSqVAcVKy88ofjcSasUkSYU" : "yLKSrCjLQFsfVgX8RjdctZ797d54atPjnV";
            return sBurnAddress;
        }
        public static string GetSha256HashI(string rawData)
        {
            if (rawData == null)
            {
                rawData = String.Empty;
            }
            // The I means inverted (IE to match a uint256)
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private static string GetContentType(string sFullPath)
        {
            FileInfo fi = new FileInfo(sFullPath);
            string ext = fi.Extension.ToLower();
            string contentType = "";
            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif")
                contentType = "image/png";
            else if (ext == ".mp4" || ext == ".webm")
            {
                contentType = "video/mp4";
            }
            else if (ext == ".htm")
            {
                contentType = "text/html";
            }
            else if (ext == ".xdat" || ext == ".dat")
            {
                contentType = "text/html";
            }
            return contentType;
        }

        private static byte[] myBytedKey = new byte[32];
        private static byte[] myBytedIV = new byte[16];
        private static void InitializeAES()
        {
            // These static bytes were ported in from Biblepay-QT, because OpenSSL uses a proprietary method to create the 256 bit AES-CBC key: EVP_BytesToKey(EVP_aes_256_cbc(), EVP_sha512()
            string sAdvancedKey = "98,-5,23,119,-28,-99,-5,90,62,-63,82,39,63,-67,-85,37,-29,-65,97,80,57,-24,71,67,119,14,-67,12,-96,99,-84,-97";
            string sIV = "29,44,121,61,-19,-62,55,-119,114,105,-123,-101,52,-45,29,-109";
            var vKey = sAdvancedKey.Split(new string[] { "," }, StringSplitOptions.None);
            var vIV = sIV.Split(new string[] { "," }, StringSplitOptions.None);
            myBytedKey = new byte[32];
            myBytedIV = new byte[16];

            for (int i = 0; i < vKey.Length; i++)
            {
                int iMyKey = (int)Common.GetDouble(vKey[i]);
                myBytedKey[i] = (byte)(iMyKey + 0);
            }
            for (int i = 0; i < vIV.Length; i++)
            {
                int iMyIV = (int)Common.GetDouble(vIV[i]);
                myBytedIV[i] = (byte)(iMyIV + 0);
            }
        }


        private static byte[] GetBytedKeyFromPassword(string password)
        {
            InitializeAES();
            if (password == null || password == "")
            {
                return myBytedKey;
            }
            byte[] myByted = new byte[32];
            password = password.PadRight(64);
            byte[] bytes = Encoding.ASCII.GetBytes(password);
            for (int i = 0; i < 32; i++)
            {
                myByted[i] = bytes[i];
            }
            return myByted;
        }


        static string DecryptAES256(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }

        static byte[] EncryptAES256(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }

        public static string EncryptAES256(string sData, string password)
        {
            byte[] myBytedLocal = GetBytedKeyFromPassword(password);
            byte[] myByteOut = EncryptAES256(sData, myBytedLocal, myBytedIV);
            return System.Convert.ToBase64String(myByteOut);
        }

        public static string DecryptAES256(string sData, string password)
        {
            if (sData == "")
                return "";
            try
            {
                byte[] myBytedLocal = GetBytedKeyFromPassword(password);
                byte[] b = System.Convert.FromBase64String(sData);
                string plainText = DecryptAES256(b, myBytedLocal, myBytedIV);
                return plainText;
            }
            catch (Exception)
            {
                return "";
            }
        }

    }

}
