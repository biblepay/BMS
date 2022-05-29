using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BiblePay.BMS.Controllers
{
    public class PageController : Controller
    {
        public IActionResult Chat() => View();
        public IActionResult Confirmation() => View();
        public IActionResult Contacts() => View();
        public IActionResult Error() => View();
        public IActionResult Error404() => View();
        public IActionResult Forget() => View();
        public IActionResult Login() => View();
        public IActionResult LoginAlt() => View();
        public string GenerateSignCommand(bool fTestNet, string sPBAddress, string sERC20Address, string sSigIn)
        {
            string sSigOut = "";
            string sMsg = BMSCommon.Encryption.GetSha256HashI(sERC20Address);

            if (sERC20Address == "")
            {
                return "<font color=red>First you must populate your ERC-20 Address.</font>";

            }
           else if (sPBAddress == "")
            {
                return "<font color=red>First, you must populate your BBP Portfolio Builder Address.</font>";
            }
           else if (sSigIn != "")
            {
                bool fVerify = BMSCommon.WebRPC.VerifySignature(fTestNet, sPBAddress, sMsg, sSigIn);

                if (!fVerify)
                {
                    sSigOut = "<font color=red>Signature invalid. </font> ";
                }
                else
                {
                    sSigOut = "<font color=red>Your stakes are signed.</font>";
                    return sSigOut;
                }
            }
            sSigOut += "<font color=red>signmessage " + sPBAddress + " " + sMsg + "</font>";
            return sSigOut;
        }
        public IActionResult Profile() 
        {
            BMSCommon.Encryption.KeyType k = DSQL.UI.GetKeyPair(HttpContext);
            ViewBag.BBPAddress = k.PubKey;
            BMSCommon.CryptoUtils.User u = DSQL.UI.GetUser(HttpContext);
            ViewBag.NickName = u.NickName;
            if (ViewBag.NickName == null || ViewBag.NickName == "")
                ViewBag.NickName = "Guest";
            bool fTestNet = DSQL.UI.IsTestNet(HttpContext);
            if (fTestNet)
            {
                ViewBag.PortfolioBuilderAddress = u.tPortfolioBuilderAddress;
                ViewBag.PBSignature = u.tPBSignature;
            }
            else
            {
                ViewBag.PortfolioBuilderAddress = u.PortfolioBuilderAddress;
                ViewBag.PBSignature = u.PBSignature;
            }
            ViewBag.SignRPC = GenerateSignCommand(DSQL.UI.IsTestNet(HttpContext), ViewBag.PortfolioBuilderAddress, u.ERC20Address, ViewBag.PBSignature);

            ViewBag.EmailAddress = u.EmailAddress;
            ViewBag.BioURL = DSQL.UI.GetBioURL(HttpContext);
            ViewBag.Balance = DSQL.UI.GetAvatarBalance(HttpContext,false);
            ViewBag.ERC20Address = u.ERC20Address;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Profile(List<IFormFile> file)
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
                            string sGuid = Guid.NewGuid().ToString() + "." + fi.Extension;
                            string sDestFN = Path.Combine(Path.GetTempPath(), sGuid);
                            using (var stream = new FileStream(sDestFN, System.IO.FileMode.Create))
                            {
                                await file[i].CopyToAsync(stream);
                            }
                            // Change the avatar (check the extension too)
                            string sURL = await BBPTestHarness.IPFS.UploadIPFS_Retired(sDestFN, sGuid);
                            BMSCommon.CryptoUtils.User u = DSQL.UI.GetUser(HttpContext);
                            u.BioURL = sURL;
                            u.Updated = System.DateTime.Now.ToString();
                            DSQL.UI.SetUser(u, HttpContext);
                            bool f = BMSCommon.CryptoUtils.PersistUser(DSQL.UI.IsTestNet(HttpContext),u);
                            break;
                        }
                        else
                        {
                            //throw new Exception("Extension not allowed");
                            string modal = DSQL.UI.GetModalDialog("Save Avatar", "Extension not allowed");
                            modal += "<script>openModal('modalid1');</script>";
                            ViewBag.Alert = modal;
                            return Profile();
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




        public IActionResult Projects() => View();
        public IActionResult Register() => View();
        public IActionResult Search() => View();
    }
}
