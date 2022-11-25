using BMSCommon;
using BMSCommon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using static BMSCommon.BitcoinSyncModel;
using static BMSCommon.CryptoUtils;
using static BMSCommon.DataTableExtensions;
using static BMSCommon.Model;

namespace BiblePay.BMS.Controllers
{
	public partial class BBPController : Controller
	{

		public static async Task<string> GetNFTDisplayList(HttpContext h, string sType)
		{
			string html = "<div class='row js-list-filter' id='nftlist'>";
			int nColNo = 0;
			int nTotal = 0;
			string sChain = IsTestNet(h) ? "test" : "main";
			User u0 = await GetUser(h);
			List<NFT> l = await NFT.GetListOfNFTs(sChain, u0.ERC20Address, sType);

			for (int i = 0; i < l.Count; i++)
			{
				NFT n = l[i];
				if (n.LowestAcceptableAmount() > 0)
				{
					string sScrollY = n.Description.Length > 100 ? "overflow-y:scroll;" : "";
					string sPrice = n.BuyItNowAmount.ToString();
					string sCaption = "";
					bool fOrphan = n.Type.ToLower() == "orphan";

      				sCaption = fOrphan ?  "Sponsor me" :  "Buy it now";

					string sConfirm = "Are you sure you would like to buy this NFT for " + n.BuyItNowAmount.ToString() + " BBP?";

					string sBuyItNowButton = GetStandardButton(n.GetHash(), sCaption, "nft_buy", "var e={};e.nftid='" + n.GetHash() 
						+ "';e.Amount=" + n.BuyItNowAmount.ToString() + ";", sConfirm);
					string sEditButton = GetStandardButton(n.GetHash(), "EDIT", "nft_edit", "var e={};e.nftid='" + n.GetHash() + "';", "");
					string sButtonCluster = sType == "my" ? sEditButton : sBuyItNowButton;

					string sURLLow = CleanseXSSAdvanced(n.AssetURL);
					string sIntro = "<div class='col-xl-4'><div id='c_3' class='card border shadow-0 mb-g shadow-sm-hover' data-filter-tags='nft_cool'><div class='d-flex flex-row align-items-center'>";

					sIntro += "<div class='card-body border-faded border-top-0 border-left-0 border-right-0 rounded-top'>";
					string sOutro = "</div></div></div></div>";
					string sAsset = "";// "<iframe xwidth=95% style='height: 200px;width:300px;' src='" + sURLLow + "'></iframe>
					if (sURLLow.Contains(".gif") || sURLLow.Contains("=w600") || sURLLow.Contains(".jpg") || sURLLow.Contains(".jpeg") || sURLLow.Contains(".png"))
					{
						string sUrlToClick = fOrphan ? n.AssetBIO : n.AssetURL;
						if (n.AssetBIO == null)
							sUrlToClick = n.AssetURL;
						string sImage = "<img style='max-width:99%;height:300px;object-fit:cover;' src='" + sURLLow + "'/>";
						string sAnchor = "<a href='" + sUrlToClick + "' target='_blank'>";
						sAsset = sAnchor + sImage + "</a>";
						//sAsset = sImage;
					}
					else if (sURLLow.Contains(".mp4") || sURLLow.Contains(".mp3"))
					{
						sAsset = "<video xclass='connect-bg' width='300' height='200' style='background-color:black' controls><source src='" + sURLLow + "' xtype='video/mp4' /></video>";
					}

					string sFooter = "Price: " + sPrice + "<br>" + sButtonCluster;

					string sName = n.Name;
					string s1 = "<td style='cell-spacing:4px;padding:7px;border:1px' cellpadding=7 cellspacing=7>"
						+ "<b>" + sName + "</b>" + sAsset + "<div style='border=1px;xheight:75px;xwidth:310px;" + sScrollY + "'><font style='font-size:11px;'>"
						+ n.Description + "</font></div><br>" + sFooter + "</td>";
					string sTextBody = "<div style='border=1px;height:75px;xwidth:340px;" + sScrollY + "'><font style='font-size:11px;'>"
						+ n.Description + "</font></div>";
					s1 = sIntro + "<b>" + sName + "</b><br>" + sAsset + sTextBody + sFooter + sOutro;
					html += s1;
					nColNo++;
					nTotal++;
				}
            }
			html += "</div>";
			if (nTotal == 0)
				html = "No NFTs found.";
            return html;
		}


		public async Task<IActionResult> NFTListOrphans()
		{
			ViewBag.NFTList = await GetNFTDisplayList(HttpContext,"orphan");
			return View();
		}

		public async Task<IActionResult> MyNFTs()
        {
			ViewBag.NFTList = await GetNFTDisplayList(HttpContext, "my");
			return View();
        }

		public async Task<IActionResult> NFTListGeneral()
        {
            ViewBag.NFTList = await GetNFTDisplayList(HttpContext, "general");
            return View();
        }

		public async Task<IActionResult> NFTListChristian()
        {
            ViewBag.NFTList = await GetNFTDisplayList(HttpContext, "christian");
            return View();
        }
		public async Task<IActionResult> NFTAdd()
        {
			List<DropDownItem> ddTypes = new List<DropDownItem>();
			ddTypes.Add(new DropDownItem("General", "General (Digital Goods, MP3, PNG, GIF, JPEG, PDF, MP4, Social Media, Tweet, Post, URL"));
			ddTypes.Add(new DropDownItem("Christian", "Christian"));
			ddTypes.Add(new DropDownItem("Orphan", "Orphan (Child to be sponsored)"));

			// In edit mode, we prepopulate the values.
			string sID = Request.Query["id"].ToString() ?? "";
			string sMode = Request.Query["mode"].ToString() ?? "";
			if (sMode == "edit" && sID != String.Empty)
			{
				NFT n = await GetNFT(HttpContext, sID);
				ViewBag.txtName = n.Name;
				ViewBag.txtDescription = n.Description;
				ViewBag.txtURL = n.AssetURL;
                ViewBag.AssetURL = n.AssetURL;

				ViewBag.txtReserveAmount = n.ReserveAmount.ToString();
                ViewBag.txtBuyItNowAmount = n.BuyItNowAmount.ToString();
				ViewBag.txtid = sID;
				ViewBag.ddNFTType = ListToHTMLSelect(ddTypes, n.Type);
				//ViewBag.AssetURL = "/img/invisible.jpeg";
				ViewBag.chkMarketable = n.Marketable.ToString();
				ViewBag.chkMarketableChecked = n.Marketable == 1 ? "CHECKED" : "";
				ViewBag.chkDeleteChecked = n.Deleted == 1 ? "CHECKED" : "";
				ViewBag.Mode = "editme";
				ViewBag.Title = "Edit NFT " + n.GetHash();
			}
			else
			{
				ViewBag.ddNFTType = ListToHTMLSelect(ddTypes, "");
				ViewBag.AssetURL = "/img/invisible.jpeg";
				ViewBag.chkMarketable = "1";
				ViewBag.Mode = "create";
				ViewBag.Title = "Add new NFT";
			}
			return View();
        }

		private static int ToBoolInt(string sData)
        {
			if (sData == "1" || sData == "true")
				return 1;
			return 0;
        }


		[HttpPost]
		public async Task<IActionResult> UploadFileNFT(List<IFormFile> file)
		{
			try
			{
				if (file.Count > 0)
				{
					for (int i = 0; i < file.Count; i++)
					{
						string _FileName = Path.GetFileName(file[i].FileName);
						bool fOK = DSQL.UI.IsAllowableExtension(_FileName);
						if (fOK)
						{
							FileInfo fi = new FileInfo(_FileName);
							string sGuid = Guid.NewGuid().ToString() + "" + fi.Extension;
							string sDestFN = Path.Combine(Path.GetTempPath(), sGuid);
							using (var stream = new FileStream(sDestFN, System.IO.FileMode.Create))
							{
								await file[i].CopyToAsync(stream);
							}
							// Mission Critical 1 

							string sURL = await BBPTestHarness.IPFS.UploadIPFS(sDestFN, "upload/photos/" + sGuid, BMSCommon.Common.GetCDN());

							ServerToClient returnVal = new ServerToClient();
							returnVal.returnbody = "";
							returnVal.returntype = "uploadsuccess";
							returnVal.returnurl = sURL;
							string o1 = JsonConvert.SerializeObject(returnVal);
							return Json(o1);
						}
						else
						{
							string modal = DSQL.UI.GetModalDialog("Save NFT Image", "Extension not allowed");
							ServerToClient returnVal = new ServerToClient();
							returnVal.returntype = "modal";
							returnVal.returnbody = modal;
							string o1 = JsonConvert.SerializeObject(returnVal);
							return Json(o1);
						}
					}
				}
				ViewBag.Message = "Sent " + file[0].FileName + " successfully";
				Response.Redirect("/page/profile");
				return View();
			}
			catch
			{
				ViewBag.Message = "File upload failed!!";
				return View();
			}
		}




		public static async Task<string> btnSubmitNFT_Click(HttpContext h, string sFormData, string _msMode)
		{
			try
			{
				string sChain = IsTestNet(h) ? "test" : "main";
				NFT n = new NFT();
				n.AssetURL = GetFormData(sFormData, "txtURL");
				n.Name = GetFormData(sFormData, "txtName");
				User u0 = await GetUser(h);
				n.OwnerERC20Address = u0.ERC20Address;
				n.AssetBIO = GetFormData(sFormData, "txtBIOURL");
				n.OwnerBBPAddress = u0.BBPAddress;
				n.Action = _msMode;
				n.Description = GetFormData(sFormData, "txtDescription");
				n.Marketable = ToBoolInt(GetFormData(sFormData, "chkMarketable"));
				n.Deleted = ToBoolInt(GetFormData(sFormData, "chkDelete"));

				n.BuyItNowAmount = BMSCommon.Common.GetDouble(GetFormData(sFormData, "txtBuyItNowAmount"));
				n.ReserveAmount = BMSCommon.Common.GetDouble(GetFormData(sFormData, "txtReserveAmount"));
				n.Hash = n.GetHash();
				if (_msMode == "create")
				{
					n.id = n.Hash;
				}
                else
                {
					n.id = GetFormData(sFormData, "txtid");
                }
				n.Version = 3;
				n.Chain = sChain;
				string sError = String.Empty;
				n.Type = GetFormData(sFormData, "ddNFTType");
				// todo: verify the dropdown selected value comes back along with the chkbox selected value: ddNFTType.SelectedValue;

				if (n.id == null || n.id == String.Empty)
					sError = "Invalid id.";

				if (n.Type == String.Empty || n.Type == null)
					sError += "NFT Type must be chosen. ";

				if (n.Type == "Orphan" && (n.AssetBIO == null || n.AssetBIO.Length < 5))
					sError += "For Orphans, the Asset BIO page must be populated. ";

				if (_msMode == "create" && n.Deleted == 1)
					sError += "A new NFT cannot be deleted.";

				if (_msMode == "edit" && n.OwnerERC20Address != u0.ERC20Address)
					sError += "Sorry, you must own the NFT in order to edit it.";

				if (n.Type.ToLower() == "orphan")
				{
					DateTime ED = DateTime.Now.AddDays(n.AssetMonths * 30);
					/*
					n.Description += "\r\nView this <a href='" + n.AssetBIO + "'>orphan biography here.</a>";
					n.Description += "\r\nSponsor this child for " + n.AssetMonths.ToString() + " month(s), and help change this childs life.";
					n.Description += "\r\nIf you have any questions about this child or our accountability records, please e-mail rob@biblepay.org.";
					n.Description += "\r\nSponsorship End Date: " + ED.ToShortDateString();
					n.Description += "_______________________________________________________________________________________________________\r\n";
					n.Description += "Tags: Orphan Sponsorship";
					*/
				}

				if (n.Type.ToLower() != "general" && n.Type.ToLower() != "christian" && n.Type.ToLower() != "orphan")
					sError += "Invalid NFT type.";

				if (n.Name.Length < 3)
					sError += "NFT Name must be populated. ";

				if (n.Description.Length < 3)
					sError += "NFT Description must be populated. ";

				if (n.AssetURL.Length < 10)
					sError += "You must enter an asset URL. ";

				if (n.ReserveAmount > 0 && n.BuyItNowAmount > 0)
                {
					if (n.BuyItNowAmount < n.ReserveAmount)
						sError += "The Buy It Now Amount must be greater than the reserve amount when both are used.";
                }
				if (n.Name.Length > 128)
					sError += "NFT Name must be < 128 chars.";

				if (n.Description.Length > 2048)
					sError += "NFT Description must be < 2048 chars.";
				if (n.AssetURL.Length > 512)
					sError += "URL Length must be < 512.";
				
				if (n.OwnerERC20Address.Length != 42 || n.OwnerBBPAddress.Length < 10)
					sError += "Your ERC-20 Address must be populated (you can do this from Profile | Wallet | Refresh | Save). ";

				double dBBPBalance = BMSCommon.Common.GetDouble(DSQL.UI.GetAvatarBalance(h, false));
				if (dBBPBalance < 251)
					sError += "Sorry, you must have more than 250 bbp available to create an NFT. ";

				
				// Pay for this NFT charge first
				string sToAddress = WebRPC.GetFDPubKey(IsTestNet(h));

				DACResult r0 = DSQL.UI.SendBBP(h, sToAddress, 250, String.Empty, String.Empty);
				if (r0.TXID == String.Empty)
                {
					sError += "Sorry, your 250 BBP payment failed.";
                }

				string sNarr = (sError == String.Empty) ? "Successfully submitted this NFT on TXID " + r0.TXID + ". <br><br>Thank you for using BiblePay Non Fungible Tokens.<br><br>"
					+ "NOTE:  Please wait for one sidechain block to pass before you can view the NFT.  " : sError;
				if (sError == String.Empty)
				{
					bool f1 = await n.Save(IsTestNet(h));
					if (!f1)
                    {
						throw new Exception("Unable to save.");
                    }
					sNarr += "<br><br>";
					string s1 = DSQL.UI.MsgBoxJson(h, "Edit NFT", "Success", sNarr);
					return s1;
				}
				else
				{
					string s2 = DSQL.UI.MsgBoxJson(h, "Process NFT", "Failure", sNarr);
					return s2;
				}
			}
			catch (Exception ex)
			{
				BMSCommon.Common.Log(ex.Message);
				string s3 = DSQL.UI.MsgBoxJson(h, "Process NFT", "Error", ex.Message);
				return s3;
			}

		}

	}
}
