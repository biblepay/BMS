using NBitcoin;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static BMSCommon.Common;

namespace BMSCommon
{
    public static class Encryption
    {
		public static string SignMessage(bool fTestNet, string sPrivKey, string sMessage)
		{
			try
			{
				if (sPrivKey == null || sMessage == String.Empty || sMessage == null)
					return string.Empty;

				BitcoinSecret bsSec;
				if (!fTestNet)
				{
					bsSec = Network.Main.CreateBitcoinSecret(sPrivKey);
				}
				else
				{
					bsSec = Network.TestNet.CreateBitcoinSecret(sPrivKey);
				}
				string sSig = bsSec.PrivateKey.SignMessage(sMessage);
				string sPK = bsSec.GetAddress().ToString();
				var fSuc = VerifySignature(fTestNet, sPK, sMessage, sSig);
				return sSig;
			}
			catch (Exception)
			{
				return String.Empty;
			}
		}

		public static bool VerifySignature(bool fTestNet, string BBPAddress, string sMessage, string sSig)
		{
			if (BBPAddress == null || sSig == String.Empty || BBPAddress == "" || BBPAddress == null || sSig == null || BBPAddress.Length < 20)
				return false;
			try
			{
				BitcoinPubKeyAddress bpk;
				if (fTestNet)
				{
					bpk = new BitcoinPubKeyAddress(BBPAddress, Network.TestNet);
				}
				else
				{
					bpk = new BitcoinPubKeyAddress(BBPAddress, Network.Main);
				}

				bool b1 = bpk.VerifyMessage(sMessage, sSig, true);
				return b1;
			}
			catch (Exception)
			{
				return false;
			}
		}




		public static string GetPubKeyFromPrivKey(string sPrivKey, bool fTestNet)
        {
            string sPubKey = NBitcoin.Crypto.BBPTransaction.GetPubKeyFromPrivKey(fTestNet, sPrivKey);
            return sPubKey;
        }

		public static byte[] FromHex(string hex)
		{
			hex = hex.Replace("-", "");
            hex = hex.Replace("0x", "");
			byte[] raw = new byte[hex.Length / 2];
			for (int i = 0; i < raw.Length; i++)
			{
				raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
			}
			return raw;
		}

		public static string FromHexString(string sHex)
		{
            byte[] b = FromHex(sHex);
            string s = Encoding.ASCII.GetString(b);
            return s;
		}


        public static string GetMd5String(string sData)
        {
            byte[] arrData = System.Text.Encoding.UTF8.GetBytes(sData);
            var hash = System.Security.Cryptography.MD5.Create().ComputeHash(arrData);
            return ByteArrayToHexString(hash);
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string GetSha256String(string sData)
        {
            byte[] arrData = System.Text.Encoding.UTF8.GetBytes(sData);
            var hash = System.Security.Cryptography.SHA256.Create().ComputeHash(arrData);
            return ByteArrayToHexString(hash);
        }
        public static string GetPublicKeyFromPrivateKey(string sPrivKey, bool fTestNet)
        {
            if (sPrivKey == "_INTERNAL_")
            {
                return String.Empty;
            }
            string sPubKey = string.Empty;
            try
            {
              sPubKey = NBitcoin.Crypto.BBPTransaction.GetPubKeyFromPrivKey(fTestNet, sPrivKey);
            }
            catch(Exception)
            {
                
            }
            return sPubKey;
        }


        public class BBPKeyPair
        {
            public string PrivKey = String.Empty;
            public string PubKey = String.Empty;
            public bool TestNet = false;
            public BBPKeyPair()
            {

            }
        }
        public static BBPKeyPair DeriveKey(bool fTestNet, string sSha)
        {
            NBitcoin.Mnemonic m = new NBitcoin.Mnemonic(sSha);
            NBitcoin.ExtKey k = m.DeriveExtKey(null);
            BBPKeyPair k1 = new BBPKeyPair
            {
                PrivKey = k.PrivateKey.GetWif(fTestNet ? NBitcoin.Network.TestNet : NBitcoin.Network.Main).ToWif().ToString(),
                PubKey = k.ScriptPubKey.GetDestinationAddress(fTestNet ? NBitcoin.Network.TestNet : NBitcoin.Network.Main).ToString()
            };
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
            rawData ??= String.Empty;
            // The I means inverted (IE to match a uint256)
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
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


        private static byte[] GetBytedIV()
        {
            // These static bytes were ported in from Biblepay-QT, because OpenSSL uses a proprietary method to create the 256 bit AES-CBC key: EVP_BytesToKey(EVP_aes_256_cbc(), EVP_sha512()
            //string sAdvancedKey = "98,-5,23,119,-28,-99,-5,90,62,-63,82,39,63,-67,-85,37,-29,-65,97,80,57,-24,71,67,119,14,-67,12,-96,99,-84,-97";
            string sIV = "29,44,121,61,-19,-62,55,-119,114,105,-123,-101,52,-45,29,-109";
            //var vKey = sAdvancedKey.Split(new string[] { "," }, StringSplitOptions.None);
            var vIV = sIV.Split(new string[] { "," }, StringSplitOptions.None);
            //myBytedKey = new byte[32];
            byte[] myBytedIV = new byte[16];

            for (int i = 0; i < vIV.Length; i++)
            {
                int iMyIV = (int)Common.GetDouble(vIV[i]);
                myBytedIV[i] = (byte)(iMyIV + 0);
            }
            return myBytedIV;
        }



        private static byte[] GetBytedKey()
        {
           byte[] myBytedKey1 = new byte[32];
           string sAdvancedKey = "98,-5,23,119,-28,-99,-5,90,62,-63,82,39,63,-67,-85,37,-29,-65,97,80,57,-24,71,67,119,14,-67,12,-96,99,-84,-97";
           var vKey = sAdvancedKey.Split(new string[] { "," }, StringSplitOptions.None);
            for (int i = 0; i < vKey.Length; i++)
            {
                int iMyKey = (int)GetDouble(vKey[i]);
                myBytedKey1[i] = (byte)(iMyKey + 0);
            }
            return myBytedKey1;
        }




        private static byte[] GetBytedKeyFromPassword(string password)
        {
            
            if (password == null || password == "")
            {
                return new byte[32];
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
            byte[] bPlainText = new byte[2048];
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(cipherText, 0, cipherText.Length);
                        cs.FlushFinalBlock();
                    }
                    
                    bPlainText = ms.ToArray();

                }

            }
            plaintext = System.Text.Encoding.UTF8.GetString(bPlainText);

            return plaintext;
        }

        static byte[] EncryptAES256(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using (AesManaged aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var encryptor = aes.CreateEncryptor(Key, IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] rawPlaintext = System.Text.Encoding.UTF8.GetBytes(plainText);
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(rawPlaintext, 0, rawPlaintext.Length);
                        cs.FlushFinalBlock();
                    }
                    ms.Flush();
                    encrypted = ms.ToArray();
                }
            }
            // Return encrypted data    
            return encrypted;
        }

        public static string EncryptAES256(string sData, string password)
        {
            byte[] myBytedLocal = GetBytedKeyFromPassword(password);
            byte[] myBytedIv = GetBytedIV();
            byte[] myByteOut = EncryptAES256(sData, myBytedLocal, myBytedIv);
            string sOut = System.Convert.ToBase64String(myByteOut);
            return sOut;
        }

        public static string DecryptAES256(string sData, string password)
        {
            if (sData == String.Empty)
                return String.Empty;
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    byte[] myBytedIv = GetBytedIV();
                    byte[] myBytedLocal = GetBytedKeyFromPassword(password);
                    byte[] b = System.Convert.FromBase64String(sData);
                    string plainText = DecryptAES256(b, myBytedLocal, myBytedIv);
                    return plainText;
                }
                catch (Exception ex)
                {
                    // Log("DecAes256 try " + i.ToString() + "::From " + sData + "::" + ex.Message + "::");
                }
            }
            Log("DES256::Giving Up...");
            return String.Empty;
        }

    }

    public static class HexadecimalEncoding
    {
        public static string StringToHex(string hexstring)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char t in hexstring)
            {
                //Note: X for upper, x for lower case letters
                sb.Append(Convert.ToInt32(t).ToString("x"));
            }
            return sb.ToString();
        }

        public static string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64
        }
    }

}
