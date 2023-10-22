using BMSCommon.Model;
using BMSCommon;
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
using uplink.NET.Models;
using uplink.NET.Services;
using static BMSCommon.Common;

namespace BBPAPI
{
    public static class StorjIOReadOnly
    {

        private static Access UplinkAccess()
        {
            string sNewROKey = "12v2VtAKhDsvm8Paf3zqBfSpsFgg2kgzcP1zp2LtEGjZ7K9ckHypYRwX9Cnq64tnyhnJS2pQfucopbWirqaJVqgXwUKBagpE23GFtCFQdC9nNoKZSAy9NYLs711T9wz3QQwQ7wLiXhVvZbm1cKKdRakLtFAJLTtaNZbENyxULiWDiZ4i4FBVmcofF2gsRQQKQKUrb6ZZmurwKVQyiJWjrZF2tmwgUjytYvu8VDbx4zwDRj9PNVzpA57oZAA6eF92wU6Z9LmiDfVpfS147EoNNEJ2HdjRB43KqcBFnNGoFVwCcqADF1vEoib8pdedCBJjVV79LdZKFyLYp4GTZ";
            var access = new Access(sNewROKey);
            return access;
        }


		private static ObjectService _objsvc = null;
		private static BucketService _bucketsvc = null;
		private static Bucket _bucket = null;
		private static async Task<ObjectService> GetObjectService()
		{
			if (_objsvc != null && false)
			{
				return _objsvc;
			}
			Access access = UplinkAccess();
			_bucketsvc = new BucketService(access);
			_bucket = await _bucketsvc.GetBucketAsync("bbp0");
			_objsvc = new ObjectService(access);
			return _objsvc;
		}

		private static void SetStorjTempPath()
		{
			string sTempDir = IsWindows() ? System.IO.Path.GetTempPath() : "~/biblepay/temp";
			//Common.Log(sTempDir);
			uplink.NET.Models.Access.SetTempDirectory(System.IO.Path.GetTempPath());
			//Common.Log("Set");
		}

		public static async Task<string> StorjDownloadString(User u, string sSource0)
		{
			try
			{
				SetStorjTempPath();

				Access access = UplinkAccess();
				var bucketService = new BucketService(access);
				Bucket thebucket = await bucketService.GetBucketAsync("bbp0");
				var objectService = new ObjectService(access);
				//string sSrc = AddPrefixToDestination(u, sSource0, sOverriddenKey);
				//Common.Log("source of Storjdownloadstring:: " + sSrc);
				var dlop = await objectService.DownloadObjectAsync(thebucket, sSource0, new DownloadOptions(), false);
				await dlop.StartDownloadAsync();
				string sData = System.Text.Encoding.Default.GetString(dlop.DownloadedBytes);
				return sData;
			}
			catch (Exception ex)
			{
				Common.Log("unable to storjdownloadstring::" + ex.Message);
				return String.Empty;
			}
		}



		public static async Task<Stream> StorjDownloadStream(string sSource1)
		{

			try
			{
				ObjectService objectService = await GetObjectService();
				//string sSource = GetFullDownloadPath("", sSource1);
				// metadata
				if (sSource1.StartsWith("/"))
				{
					sSource1 = sSource1.Substring(1, sSource1.Length - 1);
				}
				sSource1 = sSource1.Replace("//", "/");
				var dlop = await objectService.DownloadObjectAsync(_bucket, sSource1, new DownloadOptions(), false);
				var myObject = await objectService.GetObjectAsync(_bucket, sSource1);
				Stream stream = new DownloadStream(_bucket, (int)myObject.SystemMetadata.ContentLength, sSource1);
				return stream;

			}
			catch (Exception ex)
			{
				if (ex.Message.Contains("object not found"))
				{
					throw (ex);

				}
				Log("Error in StorjDownloadStream::" + ex.Message);
				throw (ex);
			}
		}


		public static string UplinkROCreate()
        {
            Access access = UplinkAccess();
            var permission = new uplink.NET.Models.Permission();
            permission.AllowDownload = true;
            permission.AllowList = true;
            permission.NotAfter = Convert.ToDateTime("1-1-2050");
            permission.NotBefore = Convert.ToDateTime("11-1-2022");
            var prefixes = new List<SharePrefix>();
            prefixes.Add(new SharePrefix { Bucket = "bbp0", Prefix = "/" });
            var restrictedAccess = access.Share(permission, prefixes);
            string serializedAccess = restrictedAccess.Serialize();
            return serializedAccess.ToString();
        }
		private static int OctetLength(string sData, string sDelimiter, int nOctetNumber)
		{
			string[] vData = sData.Split(sDelimiter);
			if (vData.Length < nOctetNumber)
				return 0;
			return vData[nOctetNumber].Length;
		}


		private static string AddMyPrefix(string sSource)
		{
			if (sSource.StartsWith("/"))
			{
				sSource = sSource.Substring(1, sSource.Length - 1);
			}
			int nOctLen = OctetLength(sSource, "/", 0);

			if (sSource.Contains("upload/tickets"))
			{
				return sSource;
			}
			if (nOctLen != 34)
			{
				sSource = "BB2BwSbDCqCqNsfc7FgWFJn4sRgnUt4tsM/" + sSource;
			}
			sSource = sSource.Replace("//", "/");

			return sSource;

		}


		public static async Task<bool> StorjDownloadLg(string sPreSource, string sDest)
        {
            uplink.NET.Models.Access.SetTempDirectory(System.IO.Path.GetTempPath());
            try
            {
                Access access = UplinkAccess();
                var bucketService = new BucketService(access);
                Bucket thebucket = await bucketService.GetBucketAsync("bbp0");
                var objectService = new ObjectService(access);
				// metadata
				//if (sSource.StartsWith("/"))
				// {
				//    sSource = sSource.Substring(1, sSource.Length - 1);
				// }
				string sSource = AddMyPrefix(sPreSource);

                // object download itself:
                var dlop = await objectService.DownloadObjectAsync(thebucket, sSource, new DownloadOptions(), false);
                await dlop.StartDownloadAsync();
                System.IO.File.WriteAllBytes(sDest, dlop.DownloadedBytes);
                return dlop.Completed;
            }
            catch (Exception ex)
            {
                Log("Failed to download storjio file " + sPreSource + "::" + ex.Message);
                return false;
            }
        }



    }
}
