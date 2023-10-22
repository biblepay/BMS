using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.Common;

namespace BBPTestHarness
{
    public static class Raid2
    {
        // BiblePay Raid.  A new innovative idea sent by the Holy Spirit.
        // We store our data across all providers, with linear density.  Therefore 5 providers = 5.0 density.
        // We are self healing.  When files are missing, we fill in the gaps with the missing files.
        // For Byzantine Fault Tolerance (BFT) we use a trusted manifest sourced from our BBP cluster.
        // Therefore the replicator simply works constantly towards consensus, with each provider being independent.
        // Think of our replication layer as layer 0.  
        // Layer 1 is the RAID layer, and is implemented on *each* provider (not *across* providers).  
        // Performance is very high because we can pull data async from each provider.
        public class Provider
        {
            public string keyID;
            public string secretKey;
            public string URL;
            public string bucketName;
            public string Suffix;
            public string GetFullURL()
            {
                string sURL = "https://" + bucketName + "." + URL;
                return sURL;
            }
        };

        public static List<Provider> lProvider = new List<Provider>();
        private static List<Provider> GetProviders(bool fStorj)
        {
            if (lProvider.Count > 0 && !fStorj)
                return lProvider;
            // 1: IPFS:
            lProvider.Clear();
            Provider p = new Provider();
            p.keyID = BMSCommon.Encryption.DecryptAES256("KOdFgM/tcljVlHYMahFYcc2hgwT5ZO8+1nOV0Rg8ryI=", "");
            p.secretKey = BMSCommon.Encryption.DecryptAES256("hIdC0jy062hwXLCk27n0yZVZeX2+4WxVtfngJWp6JZ4ejd+0AksnmJ0lfp0RP41+", "");
            p.URL = "s3.filebase.com";
            p.bucketName = "bbpipfs";
            p.Suffix = "";
            lProvider.Add(p);
            // 2: STORJ:
            p = new Provider();
            p.keyID = BMSCommon.Encryption.DecryptAES256("KOdFgM/tcljVlHYMahFYcc2hgwT5ZO8+1nOV0Rg8ryI=", "");
            p.secretKey = BMSCommon.Encryption.DecryptAES256("hIdC0jy062hwXLCk27n0yZVZeX2+4WxVtfngJWp6JZ4ejd+0AksnmJ0lfp0RP41+", "");
            p.URL = "s3.filebase.com";
            p.Suffix = "";
            p.bucketName = "bbp";
            lProvider.Add(p);

            // 3: CONTABO
            // https://eu2.contabostorage.com/3ee9e3f028454fce8615e7edfa728e65:bbp/video/96/mypath/cone.png

            p = new Provider(); 
            p.URL = "eu2.contabostorage.com";   //had /bbp behind it
            p.Suffix = "3ee9e3f028454fce8615e7edfa728e65:bbp";
            p.bucketName = "bbp";
            p.keyID = "7529071c7730428c8324b8c300cf4381";
            p.secretKey = "7f16967b5395efa344c37f8fec3ebcac";
            lProvider.Add(p);
            /*
            Access Key ID: jxta3i32hfljvbkeqnrhsipkhwmq
            Secret Key: jzpwhgrzl34ewlmrzhy7ija7gb54hh3bo3x32723xkvfi3oyhagpy
            Endpoint     : https://gateway.storjshare.io
            */
            if (fStorj)
            {
                // BACKUP DATABASE bank INTO 's3://bbp0/backup?AWS_ACCESS_KEY_ID=jxta3i32hfljvbkeqnrhsipkhwmq&AWS_SECRET_ACCESS_KEY=jzpwhgrzl34ewlmrzhy7ija7gb54hh3bo3x32723xkvfi3oyhagpy&AWS_ENDPOINT=gateway.storjshare.io&AWS_REGION=us-east-1';
                // BACKUP INTO 's3://bbp0/fullbackup?AWS_ACCESS_KEY_ID=jxta3i32hfljvbkeqnrhsipkhwmq&AWS_SECRET_ACCESS_KEY=jzpwhgrzl34ewlmrzhy7ija7gb54hh3bo3x32723xkvfi3oyhagpy&AWS_ENDPOINT=gateway.storjshare.io&AWS_REGION=us-east-1';

                p = new Provider();
                p.URL = "gateway.storjshare.io";
                p.URL = "http://blah.biblepay.org.global.prod.fastly.net/";

                p.Suffix = "";
                p.bucketName = "bbp0";
                p.keyID = "jxta3i32hfljvbkeqnrhsipkhwmq";
                p.secretKey = "jzpwhgrzl34ewlmrzhy7ija7gb54hh3bo3x32723xkvfi3oyhagpy";
                lProvider.Add(p);
            }
            
            return lProvider;
        }
        private static Amazon.S3.AmazonS3Client GetS3Client(Provider p)
        {
            AmazonS3Config config = new AmazonS3Config()
            {
                ServiceURL = string.Format("https://{0}", p.URL),
                UseHttp = true,
                ForcePathStyle = true
            };
            var client = new Amazon.S3.AmazonS3Client(p.keyID, p.secretKey, config);
            return client;
        }

        public static string GetURL(Provider p, string sObjName)
        {
            string sURL = "";
            if (p.Suffix == null || p.Suffix == "")
            {
                sURL = "https://" + p.bucketName + "." + p.URL + "/" + sObjName;

            }
            else
            {
                sURL = "https://" + p.URL + "/" + p.Suffix + "/" + sObjName;
            }
            return sURL;
        }

        public static async Task<string> UploadS3(Provider p, string sPath, string sObjName)
        {
            try
            {
                var client = GetS3Client(p);

                FileStream fs = File.OpenRead(sPath);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = fs,
                    Key = sObjName,
                    BucketName = p.bucketName
                };
                var fileTransferUtility = new TransferUtility(client);
                await fileTransferUtility.UploadAsync(uploadRequest);
                await IPFS.SetACLPublic(client, p.bucketName, sObjName);
                string sURL = GetURL(p, sObjName);

                return sURL;
            }
            catch (Exception ex)
            {
                Log("UploadS3::" + ex.Message);
                return "";
            }
        }

        

    

private static int nLoopCount = 0;



    }
}
