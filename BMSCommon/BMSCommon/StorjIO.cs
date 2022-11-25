using FFMpegCore;
using FFMpegCore.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uplink.NET;
using uplink.NET.Models;
using uplink.NET.Services;

namespace BMSCommon
{
    public static class StorjIO
    {


        public static Dictionary<string, List<string>> dictAllKeys = new Dictionary<string, List<string>>();
        public static async Task<bool> GetAllDatabaseKeys()
        {
            if (dictAllKeys.Count > 0)
            {
                return true;
            }
            string sData = await StorjIO.UplinkGetDatabaseData("index", GetPrefix());
            string[] vData = sData.Split("<row>");
            for (int i = 0; i < vData.Length; i++)
            {
                string sRow = vData[i];
                string[] vRow = sRow.Split("<col>");
                if (vRow.Length > 1)
                {
                    string sKey = vRow[1];
                    string[] vKeys = sKey.Split("/");
                    if (vKeys[1] == "database")
                    {
                        string sTable = vKeys[2];
                        string sValue = vKeys[3];
                        if (!dictAllKeys.ContainsKey(sTable))
                        {
                            dictAllKeys.Add(sTable, new List<string>());
                        }
                        string sKey2 = sKey.Replace(StorjIO.GetPrefix() + "/", "");
                        if (sValue != "upload")
                        {
                            dictAllKeys[sTable].Add(sValue);
                        }

                    }
                }

            }
            return true;
        }

        public static Dictionary<string, dynamic> dictVariableObjects = new Dictionary<string, dynamic>();
        public static async Task<List<T>> GetDatabaseObjects<T>(string sTableName)
        {
            if (dictVariableObjects.ContainsKey(sTableName))
            {
                return (List<T>)dictVariableObjects[sTableName];
            }
            await GetAllDatabaseKeys();
            if (!dictAllKeys.ContainsKey(sTableName))
            {
                return new List<T>();
            }
            List<string> sKeys = dictAllKeys[sTableName];
            //List<object> l = new List<object>();
            string sFolder = Common.GetFolder("database");
            List<T> lOut = await StorjIO.StorjDownloadDataMultiple<T>(sKeys, sTableName, sFolder);
            for (int i = 0; i < lOut.Count; i++)
            {
                dynamic e = lOut[i];
            }
            dictVariableObjects[sTableName] = lOut;
            return lOut;
        }


        public static async Task<bool> WhenAllWithExpiration(List<Task> t, int nSeconds)
        {
            int nTimeout = Common.UnixTimestamp() + nSeconds;
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            while (true)
            {
                double nCompleted = 0;
                double nFailed = 0;
                for (int i = 0; i < t.Count; i++)
                {
                    Task t1 = t[i];
                    if (t1.IsCompleted)
                    {
                        nCompleted++;
                    }
                    if (t1.IsFaulted || t1.IsCanceled)
                    {
                        nFailed++;

                    }
                }
                double nPerc = nCompleted / t.Count;
                if (nCompleted == t.Count)
                {
                    return true;
                }
                if (nCompleted + nFailed == t.Count)
                {
                    return false;
                }
                int nTime = Common.UnixTimestamp();
                if (nTime > nTimeout)
                {
                    return false;
                }
                //System.Threading.Thread.Sleep(1000);
                await Task.Delay(1000, tokenSource.Token);
                bool f400 = false;


            }

        }

        private static Access UplinkAccess()
        {
            var access = new Access(BMSCommon.Common.GetConfigurationKeyValue("storageaccess"));
            return access;
        }

        public static async Task<string> StorjUpload(string sBucket, string sSource, string sPreDest, CustomMetadata cm)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    string sDest = GetPrefix() + "/" + sPreDest;
                    uplink.NET.Models.Access.SetTempDirectory(System.IO.Path.GetTempPath());
                    Access access = UplinkAccess();
                    var bucketService = new BucketService(access);
                    Bucket thebucket = await bucketService.GetBucketAsync(sBucket);
                    byte[] bytesToUpload = System.IO.File.ReadAllBytes(sSource);
                    var objectService = new ObjectService(access);
                    var uploadOperation = await objectService.UploadObjectAsync(thebucket, sDest, new UploadOptions(), bytesToUpload, cm, false);
                    await uploadOperation.StartUploadAsync();
                    return sDest;
                }
                catch (Exception ex)
                {
                    BMSCommon.Common.Log("UplinkUL::" + ex.Message);
                    return String.Empty;
                }
            }
            return String.Empty;
        }
        public class UploadWrapper
        {
            public Task taskUpload { get; set; }
            public string Destination { get; set; }
            public string SourceFileName { get; set; }
            public int StartTime { get; set; }
        }
        public static async Task<bool> StorjUploadEntireFolder(string sBucket, string sSourceFolder, string sDest, CustomMetadata cm)
        {
                try
                {
                    string sDestNew = GetPrefix() + "/" + sDest;

                    uplink.NET.Models.Access.SetTempDirectory(System.IO.Path.GetTempPath());
                    Access access = UplinkAccess();
                    var bucketService = new BucketService(access);
                    Bucket thebucket = await bucketService.GetBucketAsync(sBucket);
                    DirectoryInfo d = new DirectoryInfo(sSourceFolder);
                    var objectService = new ObjectService(access);
                    sSourceFolder = Common.NormalizeFilePath(sSourceFolder);

                    List<UploadWrapper> l = new List<UploadWrapper>();

                foreach (var file in d.GetFiles("*.*"))
                {
                    string sDest2 = sDestNew + "/" + file.Name;
                    StreamReader sr = new StreamReader(file.FullName);
                    var uploadOperation = await objectService.UploadObjectAsync(thebucket, sDest2, new UploadOptions(), sr.BaseStream, cm);
                    //var uploadOperation = await objectService.UploadObjectAsync(thebucket, sDest2, new UploadOptions(), bytesToUpload, cm, false);
                    UploadWrapper u = new UploadWrapper();
                    u.taskUpload = uploadOperation.StartUploadAsync();
                    u.SourceFileName = file.FullName;
                    u.Destination = sDest2;
                    u.StartTime = Common.UnixTimestamp();
                    l.Add(u);
                }
                // Babysit these
                int nBadThreshhold = 60 * 10;
                while (true)
                {
                    int nCompleted = 0;

                    for (int i = 0; i < l.Count; i++)
                    {
                        UploadWrapper uw = l[i];
                        int nElapsed = Common.UnixTimestamp() - uw.StartTime;
                        if (!uw.taskUpload.IsCompleted && (nElapsed > nBadThreshhold))
                        {
                            // Restart this one
                            StreamReader sr = new StreamReader(uw.SourceFileName);
                            var uploadOperation = await objectService.UploadObjectAsync(thebucket, uw.Destination, new UploadOptions(), sr.BaseStream, cm);
                            uw.taskUpload = uploadOperation.StartUploadAsync();
                            uw.StartTime = Common.UnixTimestamp();
                            l[i] = uw;
                        }
                        if (uw.taskUpload.IsCompleted)
                        {
                            nCompleted++;
                        }
                        if (nCompleted == l.Count)
                        {
                            return true;
                        }
                    }
                    await Task.Delay(1000);
                }

                return true;
                
            }
            catch (Exception ex)
            {
                string sMyData = ex.Message;

            }
            return false;
        }

        public static async Task<List<T>> StorjDownloadDataMultiple<T>(List<string> lSource, string sTable, string sDestFolder)
        {
            Access access = UplinkAccess();
            var bucketService = new BucketService(access);
            Bucket thebucket = await bucketService.GetBucketAsync("bbp0");
            var objectService = new ObjectService(access);
            List<Task> lDownloadsInProgress = new List<Task>();
            //List<DownloadOperation> lDLOOp = new List<DownloadOperation>();
            List<T> lObjects = new List<T>();
            List<Task<DownloadOperation>> lToDownload = new List<Task<DownloadOperation>>();
            for (int i = 0; i < lSource.Count; i++)
            {
                string sMySource = GetPrefix() + "/database/" + sTable + "/" + lSource[i];
                lToDownload.Add(objectService.DownloadObjectAsync(thebucket, sMySource, new DownloadOptions(), false));
                
            }
            await Task.WhenAll(lToDownload);
            for (int i = 0; i < lSource.Count; i++)
            { 
                //DownloadOperation lActiveDO = lToDownload[i].
                var dloTask = lToDownload[i].Result.StartDownloadAsync();
                lDownloadsInProgress.Add(dloTask);
                //lDLOOp.Add(dlop);
            }
            await Task.WhenAll(lDownloadsInProgress);
            for (int i = 0; i < lSource.Count; i++)
            {
                string sFN = Common.FileNameFromFullURL(lSource[i]);
                string sTP = sDestFolder + Common.GetPathDelimiter() + sFN;
                string s1 = System.Text.Encoding.Default.GetString(lToDownload[i].Result.DownloadedBytes);
                dynamic o1 = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s1);
                lObjects.Add(o1);
            }
            return lObjects;
        }

        public static async Task<bool> StorjDownload(string sSource, string sDest)
        {
            uplink.NET.Models.Access.SetTempDirectory(System.IO.Path.GetTempPath());
            try
            {
                Access access = UplinkAccess();
                var bucketService = new BucketService(access);
                Bucket thebucket = await bucketService.GetBucketAsync("bbp0");
                var objectService = new ObjectService(access);
                // metadata
                var myObject0 = objectService.GetObjectAsync(thebucket, sSource);
                // object download itself:
                var dlop = await objectService.DownloadObjectAsync(thebucket, sSource, new DownloadOptions(), false);
                await dlop.StartDownloadAsync();
                Common.Log(dlop.DownloadedBytes.ToString());
                System.IO.File.WriteAllBytes(sDest, dlop.DownloadedBytes);
                await Task.WhenAll(myObject0);
                CustomMetadata cm = myObject0.Result.CustomMetadata;
                return dlop.Completed;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> StorjFileExists(string sSource)
        {
            uplink.NET.Models.Access.SetTempDirectory(System.IO.Path.GetTempPath());
            Access access = UplinkAccess();
            var bucketService = new BucketService(access);
            Bucket thebucket = await bucketService.GetBucketAsync("bbp0");
            var objectService = new ObjectService(access);
            try
            {

                // metadata
                string sFullSource = GetPrefix() + "/" + sSource;
                var myObject0 = objectService.GetObjectAsync(thebucket, sFullSource);
            
                await Task.WhenAll(myObject0);
                return myObject0.IsCompleted;

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static string UplinkAccessCreate(string sBBPAddress)
        {
            Access access = UplinkAccess();
            var permission = new Permission();
            permission.AllowDownload = true;
            permission.AllowList = true;
            permission.NotAfter = Convert.ToDateTime("1-1-2050");
            permission.NotBefore = Convert.ToDateTime("11-1-2022");

            var prefixes = new List<SharePrefix>();
            prefixes.Add(new SharePrefix { Bucket = "bbp0", Prefix = sBBPAddress + "/" });
            var restrictedAccess = access.Share(permission, prefixes);
            string serializedAccess = restrictedAccess.Serialize();
            return serializedAccess.ToString();
        }


        private static async Task<bool> BillForStorage(string sBBPAddress, long nTotalSize, int nTotalItems)
        {
            double nCharge = nTotalSize / 10000000;

            string sData = "<bbpaddress>" + sBBPAddress + "</bbpaddress>\r\n"
                + "<totalitems>" + nTotalItems.ToString() + "</totalitems>\r\n"
                + "<totalsize>" + nTotalSize.ToString() + "</totalsize>\r\n"
                + "<updated>" + DateTime.Now.ToShortDateString() + "</updated>\r\n"
                + "<charge>" + nCharge.ToString() + "</charge>\r\n";

            return             await StorjIO.UplinkStoreDatabaseData("usage", sBBPAddress, sData, String.Empty);
        }

        private static async Task<bool> WriteStorjIndex(string sBBPAddress, StringBuilder sbdata)
        {
            return await StorjIO.UplinkStoreDatabaseData("index", sBBPAddress, sbdata.ToString(), String.Empty);
        }

        public static async Task<ObjectList> StorjGetObjects(string sBucket, string sPrefix, bool fRecursive)
        {
            uplink.NET.Models.Access.SetTempDirectory(System.IO.Path.GetTempPath());
            var access = UplinkAccess();
            var bucketService = new BucketService(access);
            ListObjectsOptions listOptions = new ListObjectsOptions();
            listOptions.Prefix = sPrefix;
            listOptions.System = true;
            listOptions.Custom = true;
            listOptions.Recursive = fRecursive;
            Bucket thebucket = await bucketService.GetBucketAsync("bbp0");
            var objectService = new ObjectService(access);

            ObjectList objects = await objectService.ListObjectsAsync(thebucket, listOptions);
            return objects;
        }
        // Once every minute we shall dump the "Today" changes, only if they changed
        public static StringBuilder sbToday = new StringBuilder();
        public static int mnLastTodayLen = 0;
        public static async Task<bool> DumpToday()
        {
            // Mission Critical:  Call DumpToday() once per minute.
            if (sbToday.Length == mnLastTodayLen)
            {
                return false;
            }
            await UplinkStoreDatabaseData("index", "today", sbToday.ToString(), String.Empty);
            sbToday.Clear();
            return true;
        }
        public static async Task<bool> PerformBilling()
        {
            // glean root level keys
            ObjectList l = await StorjGetObjects("bbp0", String.Empty,false);
            for (int i = 0; i < l.Items.Count; i++)
            {
                uplink.NET.Models.Object o = l.Items[i];
                // this is a root key
                if (o.Key.Length == 35 && o.Key.Contains("/"))
                {
                    long nTotalSize = 0;
                    int nTotalItems = 0;
                    StringBuilder sbKeys = new StringBuilder();

                    ObjectList lSubFolder = await StorjGetObjects("bbp0", o.Key, true);
                    for (int j = 0; j < lSubFolder.Items.Count; j++)
                    {
                        uplink.NET.Models.Object oSubItem = lSubFolder.Items[j];
                        nTotalSize += oSubItem.SystemMetadata.ContentLength;
                        nTotalItems++;
                        bool bMasked = false;
                        if (lSubFolder.Items[j].Key.EndsWith(".ts"))
                            bMasked = true;
                        if (lSubFolder.Items[j].Key.Contains("accesstoken"))
                            bMasked = true;

                        if (!bMasked)
                        {
                            string sRow = o.Key + "<col>" + lSubFolder.Items[j].Key + "<col>" + oSubItem.SystemMetadata.ContentLength.ToString() + "<row>\r\n";
                            sbKeys.Append(sRow);
                        }
                        // decentralized harddisk email
                        // security cam monitoring
                        
                    }
                    // bill them here; create invoice mmddyyy.dat
                    string sKey = o.Key.Replace("/", "");
                    await BillForStorage(sKey, nTotalSize, nTotalItems);
                    // Keep track of keys here:
                    await WriteStorjIndex(sKey, sbKeys);

                }
            }
            bool f1 = false;
            return false;
        }
        public static string GetPrefix()
        {
            string sDest = BMSCommon.Common.GetConfigurationKeyValue("storageprivkey");
            if (sDest==String.Empty)
            {
                throw new Exception("No storageprivatekey specified in bms.conf");
            }
            string sPubKey = BMSCommon.Common.GetPublicKeyFromPrivateKey(sDest, false);
            return sPubKey;
        }
        public static async Task<bool> UplinkStoreDatabaseData(string sTable, string sKey, string sData, string sCustomAttributesJSON)
        {
            string sFN = sKey + ".dat";
            string sPath = Common.GetFolder("database", sFN);
            try
            {
                CustomMetadata cm = new CustomMetadata();
                var o = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(sCustomAttributesJSON);
                if (o != null)
                {
                    foreach (KeyValuePair<string, string> kvp in o)
                    {
                        cm.Entries.Add(new CustomMetadataEntry { Key = kvp.Key, Value = kvp.Value });
                    }
                }
                sPath = Common.NormalizeFilePath(sPath);

                CreateDir(sPath);

                System.IO.File.WriteAllText(sPath, sData);
                string sDest =  "database/" + sTable + "/" + sFN;
                await StorjUpload("bbp0", sPath, sDest, cm);
                return true;
                //
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private static async Task<bool> UplinkDeleteDatabaseData(string sTable, string sKey)
        {
            string sFN = sKey + ".dat";
            string sPath = Common.GetFolder("database", sFN);
            try
            {
                string sDest = "database/" + sTable + "/" + sFN;
                uplink.NET.Models.Access.SetTempDirectory(System.IO.Path.GetTempPath());
                Access access = UplinkAccess();
                var bucketService = new BucketService(access);
                Bucket thebucket = await bucketService.GetBucketAsync("bbp0");
                var objectService = new ObjectService(access);
                await objectService.DeleteObjectAsync(thebucket, sDest);
                return true;
            }
            catch (Exception ex)
            {
                BMSCommon.Common.Log("UplinkDL::" + ex.Message);
                return false;
            }
        }


        public static void CreateDir(string sPath)
        {
            string sDestDirOnly = BMSCommon.Common.ChopLastOctetFromPath(sPath);
            if (!System.IO.Directory.Exists(sDestDirOnly))
            {
                System.IO.Directory.CreateDirectory(sDestDirOnly);
            }

        }
        public static async Task<bool> UplinkStoreDataDirect0(string sKey, string sData, string sJSON)
        {
            string sPath = Common.GetFolder(String.Empty, sKey);
            sPath = Common.NormalizeFilePath(sPath);
            CreateDir(sPath);
            try
            {
                CustomMetadata cm = new CustomMetadata();
                if (sJSON != String.Empty)
                {
                    var o = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(sJSON);
                    foreach (KeyValuePair<string, string> kvp in o)
                    {
                        cm.Entries.Add(new CustomMetadataEntry { Key = kvp.Key, Value = kvp.Value });
                    }
                }
                cm.Entries.Add(new CustomMetadataEntry { Key = "1", Value = "1" });
                System.IO.File.WriteAllText(sPath, sData);
                string s1URL = await StorjUpload("bbp0", sPath, sKey, cm);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<string> UplinkGetDatabaseData(string sTable, string sKey)
        {
            string sFN = sKey + ".dat";
            string sSourcePath = GetPrefix() + "/database/" + sTable + "/" + sFN;
            string sPath = Common.GetFolder("dl", sFN);
            await StorjDownload(sSourcePath, sPath);
            if (System.IO.File.Exists(sPath))
            {
                string sData = System.IO.File.ReadAllText(sPath);
                return sData;
            }
            return String.Empty;
        }
        public static async void ConvertToHLSV3(object oVidLoc)
        {
            await ConvertToHLSV2(oVidLoc);
        }
        public static async Task<bool> ConvertToHLSV2(object oVideoLoc)
        {
            // The location of the mp4 file is sent in
            if (Common.IsWindows())
            {
                GlobalFFOptions.Configure(new FFOptions { BinaryFolder = "c:\\code\\ffmpeg" });
            }
            else
            {
                // Use the default linux system location.
            }
            string sVideoLocation = oVideoLoc.ToString();
            if (!System.IO.File.Exists(sVideoLocation))
            {
                throw new Exception("File not found: " + sVideoLocation);
            }
            string sHash = Common.GetSha256String(sVideoLocation.ToString());
            string sNewHLSPath = Common.GetFolder("video") + Common.GetPathDelimiter() + sHash;
            string bitmapPath = sNewHLSPath + Common.GetPathDelimiter() + "poster.jpg";
            var bitmap = FFMpeg.Snapshot(sVideoLocation, new Size(640,480), TimeSpan.FromSeconds(15));

            if (!System.IO.Directory.Exists(sNewHLSPath))
            {
                System.IO.Directory.CreateDirectory(sNewHLSPath);
            }
            bitmap.Save(bitmapPath, ImageFormat.Jpeg);

            string sOutputPath = sNewHLSPath + Common.GetPathDelimiter() + "1.m3u8";
            /*
            FFMpegArguments  .FromFileInput(inputPath).OutputToFile(sOutputPath, false, options => options
            .WithVideoCodec(VideoCodec.LibX264)    .WithConstantRateFactor(21)    .WithAudioCodec(AudioCodec.Aac)
            .WithVariableBitrate(4).WithCustomArgument("-hls_time 5").WithCustomArgument("-g 60").WithCustomArgument("-hls_list_size 0").
            WithVideoFlters(flterOptions => filterOptions
            .Scale(VideoSize.Ld))        .WithFastStart())    .ProcessSynchronously();
            -hls_segment_filename xxxxx_%%d.ts xxxxx.m3u8
            */
            if (!System.IO.File.Exists(sOutputPath))
            {
                FFMpegArguments.FromFileInput(sVideoLocation).OutputToFile(sOutputPath, false, options => options
                .WithCustomArgument("-y").WithCustomArgument("-hls_time 10").WithCustomArgument("-g 30").
                WithCustomArgument("-hls_list_size 0")).ProcessSynchronously();
            }
            // Upload entire folder.
            await StorjIO.StorjUploadEntireFolder("bbp0", sNewHLSPath, "video/" + sHash, new CustomMetadata());

            return true;
        }


        public async static Task<bool> UploadFileToSanc2(string sCDN, string sFilePath, string sURL, string sPrivKey)
        {
            try
            {
                string sEP = sCDN + "/api/web/bbpingress2";
                HttpContent bytesContent = new ByteArrayContent(System.IO.File.ReadAllBytes(sFilePath));
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), sEP))
                    {
                        httpClient.Timeout = new System.TimeSpan(0, 60, 00);
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Key", sPrivKey);
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("url", sURL);
                        var multipartContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        multipartContent.Add(bytesContent, "file", System.IO.Path.GetFileName(sFilePath));
                        request.Content = multipartContent;
                        // the following line is not good, but OK for debugging:
                        var oInitialResponse = await httpClient.PostAsync(sEP, multipartContent);
                        string sJsonResponse = await oInitialResponse.Content.ReadAsStringAsync();
                        Common.Log("Uploaded " + sFilePath + " to " + sURL);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
