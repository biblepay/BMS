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

namespace BiblePay.BMS.Controllers
{
    public partial class BBPController : Controller
    {
		public class NFT
		{


			public enum NFTCategory
			{
				GENERAL,
				CHRISTIAN,
				ORPHAN
			};


			public string Name { get; set; }
			public string Action { get; set; }
			public string Description { get; set; }
			public string AssetURL { get; set; }
			public string AssetHQURL { get; set; }
			public string AssetBIO { get; set; }
			public int AssetMonths { get; set; }
			public string JSONURL { get; set; }
			public string TokenID { get; set; }

			public string Type { get; set; }
			public double MinimumBidAmount { get; set; }
			public double ReserveAmount { get; set; }
			public double BuyItNowAmount { get; set; }
			public string OwnerERC20Address { get; set; }

			public int nIteration { get; set; }
			public string LastOwnerERC20Address { get; set; }
			public bool Marketable { get; set; }
			public bool fDeleted { get; set; }
			public string TXID { get; set; }

			public string GetHash()
			{
				return BMSCommon.Encryption.GetSha256HashI(AssetURL);
			}

			public NFT()
			{
				Marketable = false;
				Action = "CREATE";
				OwnerERC20Address = "";
			}

			public NFTCategory GetCategory()
			{
				NFTCategory n1 = 0;
				if (this.Type.ToLower() == "christian")
				{
					n1 = NFTCategory.CHRISTIAN;
				}
				else if (this.Type.ToLower() == "orphan")
				{
					n1 = NFTCategory.ORPHAN;
				}
				else
				{
					n1 = NFTCategory.GENERAL;
				}
				return n1;
			}

			public double LowestAcceptableAmount()
			{
				double nAcceptable = 100000000;
				if (ReserveAmount > 0 && BuyItNowAmount > 0)
				{
					// This is an Auction AND a buy-it-now NFT, so accept the lower of the two
					nAcceptable = Math.Min(ReserveAmount, BuyItNowAmount);
				}
				else if (ReserveAmount > 0 && BuyItNowAmount == 0)
				{
					// This is an auction (but not a buy it now)
					nAcceptable = ReserveAmount;
				}
				else if (BuyItNowAmount > 0 && ReserveAmount == 0)
				{
					nAcceptable = BuyItNowAmount;
				}
				return nAcceptable;
			}
		}

		public IActionResult NFTAdd()
        {
			List<string> ddTypes = new List<string>();
			ddTypes.Add("General (Digital Goods, MP3, PNG, GIF, JPEG, PDF, MP4, Social Media, Tweet, Post, URL)");
			ddTypes.Add("Christian");
			ddTypes.Add("Orphan (Child to be sponsored)");
			ViewBag.ddNFTType = ListToHTMLSelect(ddTypes, "");
			ViewBag.txtName = "a";
			ViewBag.AssetURL = "/img/invisible.jpeg";
			ViewBag.chkMarketable = "1";
			return View();
        }

		private static bool ToBool(string sData)
        {
			if (sData == "1" || sData == "true")
				return true;
			return false;
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
							string sURL = await BBPTestHarness.IPFS.UploadIPFS_Retired(sDestFN, "nft/"+sGuid);

							IntelController.ServerToClient returnVal = new IntelController.ServerToClient();
							returnVal.returnbody = "";
							returnVal.returntype = "uploadsuccess";
							returnVal.returnurl = sURL;
							string o1 = JsonConvert.SerializeObject(returnVal);
							return Json(o1);
						}
						else
						{
							string modal = DSQL.UI.GetModalDialog("Save NFT Image", "Extension not allowed");
							IntelController.ServerToClient returnVal = new IntelController.ServerToClient();
							returnVal.returntype = "modal";
							returnVal.returnbody = modal;
							string o1 = JsonConvert.SerializeObject(returnVal);
							return Json(o1);
						}
					}
				}
				ViewBag.Message = "Sent " + file[0].FileName + " successfully";
				Response.Redirect("/page/profile");
				//return Profile();
				return View();
			}
			catch
			{
				ViewBag.Message = "File upload failed!!";
				return View();
			}
		}
















		public static void btnSubmit_Click(HttpContext h, string sFormData, string _msMode)
		{
			try
			{
				NFT n = new NFT();
				n.AssetURL = GetFormData(sFormData, "txtAssetURL");

				n.Name = GetFormData(sFormData, "txtName");
				n.OwnerERC20Address = DSQL.UI.GetUser(h).ERC20Address;
				//string id = Request.QueryString["id"] ?? "";
				n.Action = _msMode;
				n.Description = GetFormData(sFormData, "txtDescription");
				n.Marketable = ToBool(GetFormData(sFormData, "chkMarketable"));
				n.fDeleted = ToBool(GetFormData(sFormData, "chkDeleted"));
				// n.HighQualityURL = txtHiQualityURL.Text;

				string sError = "";
				n.Type = GetFormData(sFormData, "ddNFTType");
				// todo: verify the dropdown selected value comes back along with the chkbox selected value: ddNFTType.SelectedValue;

				if (n.Type == "" || n.Type == null)
					sError += "NFT Type must be chosen. ";

				if (n.Type.ToLower() == "orphan")
				{
					DateTime ED = DateTime.Now.AddDays(n.AssetMonths * 30);
					n.Description += "\r\nView this <a href='" + n.AssetBIO + "'>orphan biography here.</a>";
					n.Description += "\r\nSponsor this child for " + n.AssetMonths.ToString() + " month(s), and help change this childs life.";
					n.Description += "\r\nIf you have any questions about this child or our accountability records, please e-mail rob@biblepay.org.";
					n.Description += "\r\nSponsorship End Date: " + ED.ToShortDateString();
					n.Description += "_______________________________________________________________________________________________________\r\n";
					n.Description += "Tags: Orphan Sponsorship";
				}


				if (n.Name.Length < 3)
					sError += "NFT Name must be populated. ";

				if (n.Description.Length < 5)
					sError += "NFT Description must be populated. ";

				if (n.AssetURL.Length < 10)
					sError += "You must enter an asset URL. ";

				if (n.Name.Length > 128)
					sError += "NFT Name must be < 128 chars.";

				if (n.Description.Length > 2048)
					sError += "NFT Description must be < 2048 chars.";
				if (n.AssetURL.Length > 512)
					sError += "URL Length must be < 512.";
				
				if (n.OwnerERC20Address.Length != 42)
					sError += "Your ERC-20 Address must be populated (you can do this from Profile | Wallet | Refresh | Save). ";

				double dBBPBalance = BMSCommon.Common.GetDouble(DSQL.UI.GetAvatarBalance(h, false));
				if (dBBPBalance < 1001)
					sError += "Sorry, you must have more than 1000 bbp available to create an NFT.  (Polygon gas fees get spent on our end). ";
			
				string sTXID = "";

				string sNarr = (sError == "") ? "Successfully submitted this NFT. <br><br>Thank you for using BiblePay Non Fungible Tokens." : sError;
				if (sError == "")
				{
					sNarr += "<br><br>";
					DSQL.UI.MsgBox(h, "Edit NFT", "Success", sNarr, true);
				}
				else
				{
					DSQL.UI.MsgBox(h, "Process NFT", "Failure", sNarr, true);
				}
			}
			catch (Exception ex)
			{
				BMSCommon.Common.Log(ex.Message);
				DSQL.UI.MsgBox(h, "Process NFT", "Error", ex.Message, true);

			}

		}

	}
}
