using BMSCommon.Model;
using FFMpegCore;
using System;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static BMSCommon.Common;
using static BMSCommon.Encryption;

namespace BBPAPI
{
    public class FFMpegUtils
    {

        public struct VideoObject
        {
            public string FilePath;
            public string BBPAddress;
            public string BBPPrivKey;
        };

		/*
		 *         private static void LegacyHLS4Code()
	{

		 *   // Convert to HLS here in the background ** TICAL, verify the HLS actually works:
					if (sFullDest.ToLower().Contains(".mp4"))
					{
						string sHash = Encryption.GetSha256String(sFullDest.ToString());

						System.Threading.Thread t1 = new System.Threading.Thread(FFMpegUtils.ConvertToHLSV3);
						StorjIO.VideoObject vo = new StorjIO.VideoObject();
						vo.BBPAddress = sPubKey;
						vo.FilePath = sFullDest.ToString();
						vo.BBPPrivKey = key;
						t1.Start(vo);
						u.HLSURL = Global.GetPublicCDN() + "/video/" + vo.BBPAddress + "/" + sHash + "/1.m3u8";
						//string sNewHLSPath = Common.GetFolder("video") + Common.GetPathDelimiter() + sHash;
						// make a new video object to be viewed from HLS viewers
						Video v = new Video();
						v.id = sHash;
						v.Title = "BMS " + v.id;
						v.BBPAddress = vo.BBPAddress;
						v.Source = sHash;
						if (vo.BBPAddress==String.Empty)
						{
							v.Source = sHash;
						}
						bool f15 = DataAsAdmin<Video>("options.video", v, v.id);
						u.HLSURL2 = Global.GetPublicCDN() + "/bbp/watchvideo?id=" + v.id;
						s://localhost:/bbp/watchvideo?id=1637194862
					}
		*/

	/*


	public static async Task<bool> ConvertToHLSV2(object o)
	{
		VideoObject oVideoLoc = (VideoObject)o;
		// The location of the mp4 file is sent in
		if (IsWindows())
		{
			GlobalFFOptions.Configure(new FFOptions { BinaryFolder = "c:\\code\\ffmpeg" });
		}
		else
		{
			// Use the default linux system location.
		}
		string sVideoLocation = oVideoLoc.FilePath;
		if (!System.IO.File.Exists(sVideoLocation))
		{
			throw new Exception("File not found: " + sVideoLocation);
		}
		string sHash = GetSha256String(sVideoLocation.ToString());
		if (!System.IO.File.Exists(sOutputPath))
		{
			FFMpegArguments.FromFileInput(sVideoLocation).OutputToFile(sOutputPath, false, options => options
			.WithCustomArgument("-y").WithCustomArgument("-hls_time 10").WithCustomArgument("-g 30").
			WithCustomArgument("-hls_list_size 0")).ProcessSynchronously();
		}
		// Upload entire folder.
		await StorjIO.StorjUploadEntireFolder(BBPAPI.Globals._DBUser2,sNewHLSPath, "video/" + sHash, oVideoLoc.BBPPrivKey);
		return true;
	}
	*/



	public static string GetOneFrameFromMp4UsingFFMPEG(string sMP4, string sDestStoreFolder)
        {
            // TODO: Bring ffmpeg to the release....

            string sRootFolder = System.IO.Path.Combine(Global.msContentRootPath, "wwwroot", "bbp" );

            if (IsWindows())
            {
                GlobalFFOptions.Configure(new FFOptions { BinaryFolder = sRootFolder });
            }
            else
            {
                // Use the default linux system location.
            }
            string sHash = GetSha256String(sMP4);
            string sNewHLSPath = NormalizeFilePath(GetFolder("video") + GetPathDelimiter() + sMP4 + GetPathDelimiter() + sHash);
            try
            {
                string sF1 = sHash + ".jpg";
                string bitmapPath = Path.Combine(sDestStoreFolder, sF1);
                var bitmap = FFMpeg.Snapshot(sMP4, new Size(640, 480), TimeSpan.FromSeconds(15));
                bitmap.Save(bitmapPath, ImageFormat.Jpeg);
                return bitmapPath;
            }
            catch (Exception ex)
            {

            }
            return "";
        }


        /*
        internal async static Task<bool> UploadFileToSanc2(string sCDN, string sFilePath, string sURL, string sPrivKey)
        {
            try
            {
                string sEP = sCDN + "/api/web/bbpingress";
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
                        Log("Uploaded " + sFilePath + " to " + sURL);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        */


    }
}
