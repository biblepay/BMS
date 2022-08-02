using BMSCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static BiblePay.BMS.DSQL.UI;
using static BMSCommon.DataTableExtensions;

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
				string data = GetTemplate("WatchVideo.htm");
				data = data.Replace("@poster", lVideo[0].Cover);
				data = data.Replace("@m3u8", "/video/" + lVideo[0].Source + "/1.m3u8");
				// Tack on the comments for this video.
				string comments = GetTimelinePostDiv(HttpContext, sID);
				data = data.Replace("@comments", comments);

				return data;
			}
            else
            {
				string data = "Video not found.";
				return data;
            }

		}

		public IActionResult WatchVideo()
		{
			ViewBag.WatchSancVideo = GetVideo();
			return View();
		}


		public string GetVideoList()
        {
			List<Video> lVideo = Video.Get(IsTestNet(HttpContext),"");
			int nPag = (int)BMSCommon.Common.GetDouble(Request.Query["pag"]);
			string html = "<div class='row js-list-filter' id='nftlist'>";
			int nTotal = 0;
			int nItemNo = 0;
			for (int i = nPag; i < nPag + 30 && i < lVideo.Count; i++)
			{
				Video v = lVideo[i];
				string sScrollY = v.Description.Length > 100 ? "overflow-y:scroll;" : "";
				string sVisibility = nItemNo < 29 ? "galleryvisibile" : "galleryinvisible";

				string sIntro = "<div class='col-xl-4 " + sVisibility + "'><div id='c_3' class='card border shadow-0 mb-g shadow-sm-hover' data-filter-tags='nft_cool'><div class='d-flex flex-row align-items-center'>";
				sIntro += "<div class='card-body border-faded border-top-0 border-left-0 border-right-0 rounded-top'>";
				string sOutro = "</div></div></div></div>";
				string sAsset = "";
				string sImg = BMSCommon.Common.GetCDN() + "/" + v.Cover;
				nItemNo++;
				string sUrlToClick = "bbp/watchvideo?id=" + v.id.ToString();

				string sImage = "<img style='max-width:99%;height:300px;object-fit:fill;' src='" + sImg + "'/>";
				string sAnchor = "<a href='" + sUrlToClick + "'>";
				sAsset = sAnchor + sImage + "</a>";
				string sTextBody = "<div style='border=1px;height:75px;xwidth:340px;" + sScrollY + "'><font style='font-size:11px;'>"
						+ v.Description + "</font></div>";
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
