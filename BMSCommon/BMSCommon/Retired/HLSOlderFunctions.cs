using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.Common;

namespace BMSCommon.Retired
{
    class HLSOlderFunctions
    {

        public static async Task<bool> ConvertToHLS(string sURL, string sOutputID, string sCDN)
        {
            try
            {
                // download first
                string sDestinationDir = BMSCommon.Common.GetFolder("video");
                sDestinationDir = System.IO.Path.Combine(sDestinationDir, sOutputID);
                string sTargetDir = "video/" + sOutputID;

                if (!System.IO.Directory.Exists(sDestinationDir))
                {
                    System.IO.Directory.CreateDirectory(sDestinationDir);
                }
                string sSourcePath = BMSCommon.Common.GetFolder("temp");
                sSourcePath = System.IO.Path.Combine(sSourcePath, sOutputID + ".mp4");
                MyWebClient wc = new MyWebClient();
                wc.DownloadFile(sURL, sSourcePath);

                if (!System.IO.File.Exists(sSourcePath))
                {
                    Common.Log("HLS::File not found " + sURL);
                    return false;
                }

                Common.Log("HLS::Converting " + sURL + " into destination " + sDestinationDir);

                string sDestinationThumbDir = Path.Combine(sDestinationDir, "p.jpg");

                string args = "-y -ss 5 -i " + sSourcePath + " -vframes 1 -f mjpeg " + sDestinationThumbDir;
                string arg2 = "-y -i " + sSourcePath + " -g 60 -hls_time 5 -hls_list_size 0 ";
                string sTestFile = "";
                if (IsWindows())
                {
                    arg2 += sDestinationDir + "\\1.m3u8";

                }
                else
                {
                    arg2 += sDestinationDir + "/1.m3u8";
                }

                string sEXE = IsWindows() ? "c:\\inetpub\\wwwroot\\videos\\ffmpeg.exe" : "/usr/bin/ffmpeg";
                string sWD = IsWindows() ? "c:\\inetpub\\wwwroot\\videos" : "/inetpub/wwwroot/videos";

                if (!IsWindows())
                {
                    //sEXE = "bash";
                    //args = "/usr/bin/ffmpeg " + args;
                }

                if (IsWindows())
                {
                    string res1 = ProcessAsyncHelper.run_cmd2(sWD, sEXE, args);
                    string res2 = ProcessAsyncHelper.run_cmd2(sWD, sEXE, arg2);
                }
                else
                {
                    sEXE = "bash";
                    args = "/usr/bin/ffmpeg " + args;
                    arg2 = "/usr/bin/ffmpeg " + arg2;
                    await ProcessAsyncHelper.ExecuteShellCommand(sEXE, args, 10);
                    await ProcessAsyncHelper.ExecuteShellCommand(sEXE, arg2, 30);
                    long sz = ProcessAsyncHelper.WaitOnDirectorySize(sDestinationDir);
                    if (sz == 0)
                        return false;
                }

                // Verify the existence of the first ts file
                sTestFile = Path.Combine(sDestinationDir, "1.m3u8");
                if (!System.IO.File.Exists(sTestFile))
                {
                    Log("Catastrophic Error::Output " + sTestFile + " does not exist! Failing!");
                    return false;
                }

                string sHLS = GetHLS(sDestinationDir);
                // Move the directory to IPFS
                // bool f3 = await IPFS.UploadDirectoryToIPFS(sDestinationDir, sTargetDir, sCDN);
                //  step 3 :  Update the video with the resulting new filename
                return true;
            }
            catch (Exception)
            {
                return false;
            }
    }


        public static async Task<bool> ConvertToHLS2(string sCDN, string sVideoSite)
        {
            try
            {
                double nCutoff = UnixTimestamp() - (60 * 60);
                string sData = ExecuteMVCCommand("https://" + sVideoSite + "/BMSAPI.php?converthlsdata=1");
                string user_id = BMSCommon.Common.ExtractXML(sData, "<user_id>", "</user_id>");
                string id = ExtractXML(sData, "<id>", "</id>");
                string postFile = ExtractXML(sData, "<postFile>", "</postFile>");
                string time = ExtractXML(sData, "<time>", "</time>");
                if (id != "")
                {
                    //await SetAsProcessing(sVideoSite, 1, id);
                    string sFullURL = sCDN + "/" + postFile; //source url is /upload/videos/...
                    Log("Processing HLS " + sFullURL);
                    bool f = false; // await Common.ConvertToHLS(sFullURL, time, sCDN);
                    if (f)
                    {
                        string postFileName = time + "/1.m3u8";
                        string postDailyMotion = id;
                        string updated = ExecuteMVCCommand("https://" + sVideoSite + "/BMSAPI.php?hlsupf=1&postFile=&postFileName="
                            + postFileName + "&postDailymotion=" + postDailyMotion + "&id=" + id + "&time=" + time + "&user_id=" + user_id);
                        string sURL = time + "/1.m3u8";
                        string sDir = Path.Combine(GetFolder("video"), time);
                        string sHLS = GetHLS(sDir);
                        //sql = "INSERT INTO bbp.video1 (`id`,`time`,`hls`,`url`,`hlsid`) values (UNIX_TIMESTAMP(),UNIX_TIMESTAMP(),'" + sHLS + "','" + sURL + "','" + dt.Rows[i]["time"].ToString() + "')";
                    }
                    else
                    {
                        //await SetAsProcessing(sVideoSite, 0, id);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log("Error in HLS Conversion::" + ex.Message);
                return false;
            }
        }

    }
}
