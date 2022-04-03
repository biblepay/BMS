using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BiblePay.BMS.DSQL
{

    public static class modLegacyCryptography
    {
        private static TripleDESCryptoServiceProvider TripleDes = new TripleDESCryptoServiceProvider();
        public static string MerkleRoot = "0xda43abf15a2fcd57ceae9ea0b4e0d872981e2c0b72244477750ce6010a14efb8";

        public static string ToBase64(string data)
        {
            try
            {
                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }
        public static string FromBase64(string data)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(data));
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = default(StringBuilder);
            hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private static void Merkle(string sSalt)
        {
            try
            {
                TripleDes.Key = TruncateHash(MerkleRoot + sSalt.Substring(sSalt.Length - 4, 4), TripleDes.KeySize / 8);
                TripleDes.IV = TruncateHash("", TripleDes.BlockSize / 8);
            }
            catch (Exception)
            {
                return;
            }
        }

        private static byte[] TruncateHash(string key, int length)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] keyBytes = System.Text.Encoding.Unicode.GetBytes(key);
            byte[] hash = sha1.ComputeHash(keyBytes);
            Array.Resize(ref hash, length);
            return hash;
        }
        public static string Des3EncryptData2(string plaintext)
        {
            try
            {
                Merkle(MerkleRoot);
                byte[] plaintextBytes = System.Text.Encoding.Unicode.GetBytes(plaintext);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream encStream = new CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                encStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                encStream.FlushFinalBlock();
                try
                {
                    return Convert.ToBase64String(ms.ToArray());
                }
                catch (Exception)
                {
                }
                return String.Empty;
            } catch (Exception ex)
            {
                return "";
            }
        }

        public static string Des3DecryptData2(string encryptedtext)
        {
            Merkle(MerkleRoot);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedtext);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream decStream = new CryptoStream(ms, TripleDes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
            decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            decStream.FlushFinalBlock();
            return System.Text.Encoding.Unicode.GetString(ms.ToArray());
        }


        public static String SHA256(String sValue)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(sValue));
                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

        public static string GetMd5String(string sData)
        {
            byte[] arrData = System.Text.Encoding.UTF8.GetBytes(sData);
            var hash = System.Security.Cryptography.MD5.Create().ComputeHash(arrData);
            return Common.ByteArrayToHexString(hash);
        }

        public static async Task<double> GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            await Task.Delay(500);
            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal * 100;
        }

        public static int nHashes = 0;
        public static long nStartTime = Common.UnixTimestamp();
        public static long nEndTime = 0;
        public static int nCurCore = 0;

        private static void IndividualCoreTest()
        {
            string sNewValue = "";
            int nMyHashes = 0;
            nCurCountCore++;
            while (true)
            {
                sNewValue = SHA256(sNewValue);
                nHashes++;
                long nTime = Common.UnixTimestamp();
                nMyHashes++;
                if (nTime > nEndTime)
                    break;
            }
        }

        private static int nCurCountCore = 0;

        public static double ProcSpeedTest(long nSeconds, int iCores)
        {
            nHashes = 0;
            nStartTime = Common.UnixTimestamp();
            nEndTime = nStartTime + nSeconds;
            for (int i = 0; i < iCores; i++)
            {
                nCurCore = i;
                System.Threading.Thread t = new System.Threading.Thread(IndividualCoreTest);
                t.Start();
            }
            while (true)
            {
                long nTime = Common.UnixTimestamp();

                if (nTime > nEndTime)
                    break;
                System.Threading.Thread.Sleep(100);
            }
            return nHashes;
        }

        public static double GetDiskSizeTB()
        {
            try
            {
                string p1 = System.IO.Directory.GetCurrentDirectory();
                string p2 = System.IO.Path.GetPathRoot(p1);
                DriveInfo drive = new DriveInfo(p2);
                double totalBytes = drive.TotalSize;
                var TB = totalBytes / 1000000000000.01;
                return TB;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static double GetFreeDiskSpacePercentage()
        {
            try
            {
                string p1 = System.IO.Directory.GetCurrentDirectory();
                string p2 = System.IO.Path.GetPathRoot(p1);
                DriveInfo drive = new DriveInfo(p2);
                var totalBytes = drive.TotalSize;
                var freeBytes = drive.AvailableFreeSpace;
                var freePercent = (int)((100 * freeBytes) / totalBytes);
                return freePercent;
            }
            catch(Exception ex)
            {
                return 0;
            }
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
                return String.Empty;
            }
        }
    }
}
