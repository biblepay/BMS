using BiblePay.BMS.Extensions;
using BMSCommon.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UI;
using static BMSCommon.Common;

namespace BiblePay.BMS.Controllers
{
    public partial class BBPController : Controller
	{
		public string GetVideo()
        {
			string sID = Request.Query["id"];
			List<Video> lVideo = Video.Get(IsTestNet(HttpContext), sID);
			if (lVideo.Count > 0)
			{
				ViewBag.VideoPoster = lVideo[0].Cover ?? String.Empty;

				if (!String.IsNullOrEmpty(lVideo[0].BBPAddress))
				{ // bbpaddr/video/hlspath/1.m3u8
					ViewBag.VideoFileName =  ""+ lVideo[0].BBPAddress + "/video/" + lVideo[0].Source + "/1.m3u8";
				}
                else
                {
					ViewBag.VideoFileName = "/video/" + lVideo[0].Source + "/1.m3u8";
                }
				// Tack on the comments for this video.
				ViewBag.VideoComments = GetTimelinePostDiv(HttpContext, sID);
				return String.Empty;
			}
            else
            {
				string data = "Video not found.";
				return data;
            }

		}

		public IActionResult WatchVideo()
		{
			ViewBag.WatchVideo = GetVideo();
			return View();
		}

		public string GetVideoList()
        {
			List<Video> lVideo = Video.Get(IsTestNet(HttpContext), String.Empty);
			lVideo = lVideo.Where(s => s.Description != String.Empty && s.Description != null).ToList();
			int nPag = (int)GetDouble(Request.Query["pag"]);
			string html = "<div class='row js-list-filter' id='nftlist'>";
			int nTotal = 0;
			int nItemNo = 0;
			bool fAdmin = (HttpContext.GetCurrentUser().ERC20Address == "0xafe8c2709541e72f245e0da0035f52de5bdf3ee5");
			int iLength = 100;
			for (int i = nPag; i < nPag + iLength && i < lVideo.Count; i++)
			{
				Video v = lVideo[i];
				string sDesc = v.Description ?? String.Empty;
				string sScrollY = sDesc.Length > 100 ? "overflow-y:scroll;" : "";
				string sVisibility = nItemNo < iLength-1 ? "galleryvisibile" : "galleryinvisible";

				string sIntro = "<div class='col-xl-4 " + sVisibility + "'>"
					+"<div id='c_3' class='card border shadow-0 mb-g shadow-sm-hover' data-filter-tags='nft_cool'>"
					+"<div class='d-flex flex-row align-items-center'>";
				sIntro += "<div class='card-body border-faded border-top-0 border-left-0 border-right-0 rounded-top'>";
				string sAsset = "";
				string sImg = GlobalSettings.GetCDN() + "/" + v.Cover;
				nItemNo++;
				string sUrlToClick = "bbp/watchvideo?id=" + v.id.ToString();
				string sTrash = fAdmin ? GetStandardButton("btndeletevideo", "Delete", "video_delete", 
					"var e={};e.id='" + v.Source + "';", String.Empty, "") : String.Empty;
				string sOutro = "</div></div></div>" + sTrash + "</div>";
				string sImage = "<img style='max-width:99%;height:300px;object-fit:fill;' src='" + sImg + "'/>";
				string sAnchor = "<a href='" + sUrlToClick + "'>";
				sAsset = sAnchor + sImage + "</a>";
				string sTextBody = "<div style='border=1px;height:75px;xwidth:340px;" + sScrollY + "'><font style='font-size:11px;'>"
						+ sDesc + "</font></div>";
				string s1 = sIntro + "<b>" + v.Title + "</b><br>" + sAsset + sTextBody + sOutro;
				html += s1;
				nTotal++;
			}
			html += "</div>";
			if (nTotal == 0)
				html = "No Videos found.";
			return html;
		}

		public IActionResult VideoList()
        {
            ViewBag.VideoList = GetVideoList();
            return View();
        }
    }
}
