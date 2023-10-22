using BBPAPI;
using BBPAPI.Model;
using BiblePay.BMS.Extensions;
using BMSCommon;
using BMSCommon.Model;
using BMSCommon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using static BiblePay.BMS.Controllers.AttachmentController;
using static BiblePay.BMS.DSQL.DOMItem;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UI;
using static BiblePay.BMS.DSQL.UIWallet;
using static BiblePay.BMS.DSQL.Utility;
using static BMSCommon.Common;
using static BMSCommon.Encryption;

namespace BiblePay.BMS.Controllers
{
	public class NFTController : Controller
	{

		[HttpPost]
		public JsonResult ProcessDoCallback([FromBody] ClientToServer o)
		{
			ServerToClient returnVal = new ServerToClient();
			User u0 = GetUser(HttpContext);


			if (o.Action == "nft_create")
			{
				string sRedir = NFTController.btnSubmitNFT_Click(HttpContext, o.FormData, "create");
				return Json(sRedir);
			}
			else if (o.Action == "nft_editme")
			{
				string sRedir = NFTController.btnSubmitNFT_Click(HttpContext, o.FormData, "edit");
				return Json(sRedir);
			}
			else if (o.Action == "nft_buy")
			{
				dynamic oExtra = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
				string sNFTID = oExtra.nftid.Value;
				double nBuyItNowAmount = oExtra.Amount.Value;
                User uBuyer = HttpContext.GetCurrentUser();
				NFTBuy n = new NFTBuy();
				n.ID = sNFTID;
				n.TestNet = IsTestNet(HttpContext);
				n.Amount = nBuyItNowAmount;
				n.PrivateKey = uBuyer.GetPrivateKey();
				n.Buyer = uBuyer;
				DACResult r1 = BBPAPI.Interface.NFTLogic.BuyNFT(n).Result;
				if (!String.IsNullOrEmpty(r1.Error))
                {
                    string s1 = MsgBoxJson(HttpContext, "Error", "Error", r1.Error);
                    return Json(s1);
				}
				else
                {
                    string s2 = MsgBoxJson(HttpContext, "Success", "Success",
                        r1.Response);
                    return Json(s2);
				}
			}
			else if (o.Action == "nft_edit")
			{
				dynamic a = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
				string sID = a.nftid.Value;
				string m = "location.href='/nft/nftadd?mode=edit&id=" + sID + "';";
				returnVal.returnbody = m;
				returnVal.returntype = "javascript";
				string o1 = JsonConvert.SerializeObject(returnVal);
				return Json(o1);
			}
            else
            {
				throw new Exception("Unknown method.");
            }
		}

		public NFT GetNFT(HttpContext h, string sID)
		{
			string sChain = IsTestNet(h) ? "test" : "main";
			NFTSearch n1 = new NFTSearch();
			n1.Chain = sChain;
			n1.ID = sID;
			NFT n = BBPAPI.Interface.NFTLogic.GetNFT(n1).Result;
			return n;
		}

		public static string GetNFTDisplayList(HttpContext h, string sType, int nMarketable)
		{
			string html = "<div class='row js-list-filter' id='nftlist'>";
			int nColNo = 0;
			int nTotal = 0;
			string sChain = IsTestNet(h) ? "test" : "main";
			User u0 = GetUser(h);
			NFTSearch ns = new NFTSearch();
			ns.Chain = sChain;
			ns.Address = u0.ERC20Address;
			ns.Types = sType;
			List<NFT> l = BBPAPI.Interface.NFTLogic.GetListOfNFTs(ns).Result;

			for (int i = 0; i < l.Count; i++)
			{
				NFT n = l[i];
				bool fBuyable = (n.LowestAcceptableAmount() > 0 && n.Marketable == 1);
				bool fIncludeHere = n.Marketable == nMarketable || nMarketable == -1;
				if (n.Description == null)
					fIncludeHere = false;
				if (fIncludeHere)
				{
					string sScrollY = n.Description.Length > 100 ? "overflow-y:scroll;" : "";
					string sPrice = n.BuyItNowAmount.ToString();
					string sCaption = "";
					bool fOrphan = n.Type.ToLower() == "orphan";
      				sCaption = fOrphan ?  "Sponsor me" :  "Buy it now";
					string sConfirm = "Are you sure you would like to buy this NFT for " + n.BuyItNowAmount.ToString() + " BBP?";
					string sBuyItNowButton = GetStandardButton(n.GetHash(), sCaption, "nft_buy", "var e={};e.nftid='" + n.GetHash() 
						+ "';e.Amount=" + n.BuyItNowAmount.ToString() + ";", sConfirm, "nft/processdocallback");
					string sEditButton = GetStandardButton(n.GetHash(), "EDIT", "nft_edit", "var e={};e.nftid='" + n.GetHash() + "';", "", "nft/processdocallback");
					string sButtonCluster = sType == "my" ? sEditButton : sBuyItNowButton;
					string sURLLow = CleanseXSSAdvanced(n.AssetURL);
					string sIntro = "<div class='col-xl-4'><div id='c_3' class='card border shadow-0 mb-g shadow-sm-hover' data-filter-tags='nft_cool'><div class='d-flex flex-row align-items-center'>";
					sIntro += "<div class='card-body border-faded border-top-0 border-left-0 border-right-0 rounded-top'>";
					string sOutro = "</div></div></div></div>";
                    string sAsset = String.Empty;
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

					string sFooter = fBuyable ? "Price: " + sPrice + "<br>" + sButtonCluster  : "";
					if (sType=="my")
                    {
						sFooter = sButtonCluster;
                    }
					string sERC = "<small>" + n.OwnerERC20Address + "</small>";
					string sName = n.Name + "<br>" + sERC;
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
			if (nTotal == 0 && nMarketable==1)
				html = "No NFTs found.";
            return html;
		}
		public IActionResult NFTListOrphans()
		{
			ViewBag.NFTList = GetNFTDisplayList(HttpContext,"orphan",1);
			return View();
		}

		public IActionResult MyNFTs()
        {
			ViewBag.NFTList = GetNFTDisplayList(HttpContext, "my",-1);
			return View();
        }

		public IActionResult NFTListGeneral()
        {
            ViewBag.NFTList = GetNFTDisplayList(HttpContext, "general",1);
			ViewBag.NFTListOwned = GetNFTDisplayList(HttpContext, "general", 0);
			return View();
        }

		public IActionResult NFTListChristian()
        {
            ViewBag.NFTList = GetNFTDisplayList(HttpContext, "christian",1);
			ViewBag.NFTListOwned = GetNFTDisplayList(HttpContext, "christian", 0);

            return View();
        }
		public IActionResult NFTAdd()
        {
			List<DropDownItem> ddTypes = new List<DropDownItem>();
			ddTypes.Add(new DropDownItem { key0 = "General", text0 = "General (Digital Goods, MP3, PNG, GIF, JPEG, PDF, MP4, Social Media, Tweet, Post, URL" });
			ddTypes.Add(new DropDownItem{ key0 = "Christian", text0 = "Christian" });
			ddTypes.Add(new DropDownItem{ key0 = "Orphan", text0 = "Orphan (Child to be sponsored)" });

			// In edit mode, we prepopulate the values.
			string sID = Request.Query["id"].ToString() ?? "";
			string sMode = Request.Query["mode"].ToString() ?? "";

			if (sMode == "edit" && sID != String.Empty)
			{
				NFT n = GetNFT(HttpContext, sID);
                ViewBag.NFT = n;

				ViewBag.txtName = n.Name;
				ViewBag.txtDescription = n.Description;
				ViewBag.txtURL = n.AssetURL;
				ViewBag.AssetURL = n.AssetURL;
				ViewBag.txtReserveAmount = n.ReserveAmount.ToString();
                ViewBag.txtBuyItNowAmount = n.BuyItNowAmount.ToString();
				ViewBag.txtid = sID;
				ViewBag.ddNFTType = ListToHTMLSelect(ddTypes, n.Type);
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

		private static int FromBoolToInt(string sData)
        {
			if (sData == "1" || sData == "true")
				return 1;
			return 0;
        }


		[HttpPost]
		public async Task<IActionResult> UploadFileNFT(List<IFormFile> file)
		{
	        for (int i = 0; i < file.Count;)
            {
				FreshUpload fresh = new FreshUpload(this, file[i]);
				if (fresh.NotAllowedError != null)
					return fresh.NotAllowedError;
                Pin p = new Pin();
                p = BBPAPI.Utilities.PinLogic.StoreFile(HttpContext.GetCurrentUser(), fresh.FullDiskFileName, fresh.StorjDestination, "");
                ServerToClient returnVal = new ServerToClient();
                returnVal.returnbody = "";
                returnVal.returntype = "uploadsuccess";
                returnVal.returnurl = p.URL;
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            throw new Exception("no file");
		}

        public static string DacToMsgBox(HttpContext h, DACResult r0, string sTitle)
        {
            if (r0.Error == String.Empty)
            {
                string s1 = DSQL.UI.MsgBoxJson(h, sTitle, "Success", r0.Response);
                return s1;
            }
            else
            {
                string s2 = DSQL.UI.MsgBoxJson(h, sTitle, "Failure", r0.Error);
                return s2;
            }
		}

		public static string btnSubmitNFT_Click(HttpContext h, string sFormData, string _msMode)
		{
			try
			{
				string sChain = IsTestNet(h) ? "test" : "main";
				NFT n = new NFT();
				n.AssetURL = GetFormData(sFormData, "txtURL");
				n.Name = GetFormData(sFormData, "txtName");
				User u0 = GetUser(h);
				n.OwnerERC20Address = u0.ERC20Address;
				n.AssetBIO = GetFormData(sFormData, "txtBIOURL");
				n.OwnerBBPAddress = u0.BBPAddress;
				n.Action = _msMode;
				n.Description = GetFormData(sFormData, "txtDescription");
				n.Marketable = FromBoolToInt(GetFormData(sFormData, "chkMarketable"));
				n.Deleted = FromBoolToInt(GetFormData(sFormData, "chkDelete"));
				n.BuyItNowAmount = GetDouble(GetFormData(sFormData, "txtBuyItNowAmount"));
				n.ReserveAmount = GetDouble(GetFormData(sFormData, "txtReserveAmount"));
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

				double dBBPBalance = GetDouble(DSQL.UI.GetAvatarBalance(h, false));
				if (dBBPBalance < 251)
					sError += "Sorry, you must have more than 250 bbp available to create an NFT. ";
			
				// Pay for this NFT charge first
				NFTBuy n11 = new NFTBuy();
				n11.TestNet = IsTestNet(h);
				n11.PrivateKey = h.GetCurrentUser().GetPrivateKey();
				n11.NFT = n;
				DACResult r0 = BBPAPI.Interface.NFTLogic.Save(n11).Result;
				string sNarr = (r0.Error == String.Empty) ? "Successfully submitted this NFT on TXID " 
                                                          + r0.TXID 
                                                          + ". <br><br>Thank you for using BiblePay Non Fungible Tokens.<br><br>"
					+ "NOTE:  Please wait for one block to pass before you view the NFT.  " : sError;
                r0.Response = sNarr;
                return DacToMsgBox(h, r0,"Process NFT");
			}
			catch (Exception ex)
			{
				Log(ex.Message);
				string s3 = DSQL.UI.MsgBoxJson(h, "Process NFT", "Error", ex.Message);
				return s3;
			}

		}

	}
}
