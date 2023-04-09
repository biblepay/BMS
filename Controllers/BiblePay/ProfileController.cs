using BiblePay.BMS.Extensions;
using BMSShared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using OptionsShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static BiblePay.BMS.DSQL.DOMItem;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UIWallet;
using static BMSCommon.Common;
using BMSCommon;
using BiblePay.BMS.DSQL;
using Google.Authenticator;
using BBPAPI.Model;
using static BBPAPI.Model.User;
using BBPAPI;
using BMSCommon.Model;

namespace BiblePay.BMS.Controllers
{
    public class ProfileController : Controller
    {
        public string GenerateSignCommand(bool fTestNet, string sPBAddress, string sERC20Address, string sSigIn)
        {
            string sSigOut = String.Empty;
            string sMsg = Encryption.GetSha256HashI(sERC20Address);

            if (sERC20Address == String.Empty)
            {
                return "<font color=red>First you must populate your ERC-20 Address.</font>";

            }
            else if (sPBAddress == String.Empty)
            {
                return "<font color=red>First, you must populate your BBP Portfolio Builder Address.</font>";
            }
            else if (sSigIn != String.Empty)
            {
                bool fVerify = Sanctuary.VerifySignature(fTestNet, sPBAddress, sMsg, sSigIn);

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

        public IActionResult Wallet()
        {
            Encryption.KeyType k = GetKeyPair(HttpContext);
            ViewBag.BBPAddress = k.PubKey;
            ViewBag.Balance = DSQL.UI.GetAvatarBalance(HttpContext, false);
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult VerifyEmailAddress()
        {
            string sUserID = HttpContext.Request.Query["id"].ToString();
            string sKey = Encryption.Base64Decode(HttpContext.Request.Query["key"].ToString());
            string sError = String.Empty;
            bool fMatches = BBPAPI.ERCUtilities.DecryptionMatches(sKey, sUserID);

            if (!fMatches)
                sError = "Invalid URL.";
            User u = BBPAPI.Model.User.GetUserByID(false, sUserID);
            if (u == null)
                sError = "Invalid User.";
            if (sError != String.Empty)
            {
                DSQL.UI.MsgBox(HttpContext, "Error", "Error", sError, true);
                return View();
            }
            u.EmailAddressVerified = 1;
            u.LoggedIn = true;
            bool fOK = DB.OperationProcs.UpdateUserEmailAddressVerified(sUserID);
            HttpContext.EraseUserCache();
            string sChain = IsTestNet(HttpContext) ? "tUser" : "User";
            HttpContext.Session.SetObject(sChain, u);
            string sMsg = "Thank you for verifying your e-mail address (" + DateTime.Now.ToString() + ").";
            return this.MsgBox("Verified", "Verified", sMsg);
        }

        private void SetupTwoFactorAuthentication()
        {
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            ViewBag.MFASecret = Guid.NewGuid().ToString();
            Encryption.KeyType k = GetKeyPairByGUID(IsTestNet(HttpContext), ViewBag.MFASecret, String.Empty);
            string sSite = IsTestNet(HttpContext) ? "TEST unchained.biblepay.org" : "unchained.biblepay.org";
            var setupInfo = twoFactor.GenerateSetupCode(sSite, k.PubKey, ViewBag.MFASecret, false, 3);
            ViewBag.TwoFactorManualSetupCode = setupInfo.ManualEntryKey;
            ViewBag.TwoFactorQRImageUrl = setupInfo.QrCodeSetupImageUrl;
        }

        public IActionResult Register()
        {
            SetupTwoFactorAuthentication();
            return View();
        }
        public IActionResult Profile()
        {
            HttpContext.EraseUserCache();
            User u = HttpContext.GetCurrentUser();
            ViewBag.NickName = u.NickName;
            if (ViewBag.NickName == null || ViewBag.NickName == String.Empty)
                ViewBag.NickName = "Guest";
            bool fTestNet = IsTestNet(HttpContext);
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
            ViewBag.SignRPC = GenerateSignCommand(IsTestNet(HttpContext), ViewBag.PortfolioBuilderAddress, u.ERC20Address, ViewBag.PBSignature);
            // ToDo: Move from this encryption to something that will work long term
            ViewBag.EmailAddress = BBPAPI.ERCUtilities.BindEmailAddressValue(u);
            ViewBag.EmailAddressVerified = u.EmailAddressVerified == 1 ? "Yes" : "No";
            ViewBag.BioURL = DSQL.UI.GetBioURL(HttpContext);
            ViewBag.Balance = DSQL.UI.GetAvatarBalance(HttpContext, false);
            ViewBag.ERC20Address = u.ERC20Address;
            if (String.IsNullOrEmpty(u.MFA) && u.LoggedIn)
            {
                SetupTwoFactorAuthentication();
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Profile(List<IFormFile> file)
        {
            try
            {
                if (file.Count > 0)
                {
                    for (int i = 0; i < file.Count;)
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
                            // Change the avatar (check the extension too)
                            // mission critical #2
                            string sURL = await BBPAPI.IPFS.UploadIPFS(sDestFN, "upload/photos/" + sGuid, GlobalSettings.GetCDN());

                            User u = HttpContext.GetCurrentUser();
                            u.BioURL = sURL;
                            u.Updated = System.DateTime.Now;

                            SetUser(u, HttpContext);
                            bool f = PersistUser(IsTestNet(HttpContext), u);
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
                Response.Redirect("/profile/profile");
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }


        [HttpPost]
        public JsonResult ProcessDoCallback([FromBody] ClientToServer o)
        {
            ServerToClient returnVal = new ServerToClient();
            User u0 = GetUser(HttpContext);

            if (o.Action == "Profile_Save")
            {
                User u = GetUser(HttpContext);
                bool fTestNet = IsTestNet(HttpContext);
                BBPAPI.ERCUtilities.SetUserEmail(u,GetFormData(o.FormData, "txtEmailAddress"));

                u.Updated = System.DateTime.Now;
                u.NickName = GetFormData(o.FormData, "txtNickName");
                //u.ERC20Address = GetFormData(o.FormData, "txtERC20Address");
                if (String.IsNullOrEmpty(u.id))
                {
                    u.id = Guid.NewGuid().ToString();
                }
                if (fTestNet)
                {
                    u.tPBSignature = GetFormData(o.FormData, "txtPBSignature");
                    u.tPortfolioBuilderAddress = GetFormData(o.FormData, "txtPortfolioBuilderAddress");
                }
                else
                {
                    u.PBSignature = GetFormData(o.FormData, "txtPBSignature");
                    u.PortfolioBuilderAddress = GetFormData(o.FormData, "txtPortfolioBuilderAddress");
                }

                SetUser(u, HttpContext);
                bool f = BBPAPI.Model.User.PersistUser(IsTestNet(HttpContext), u);
                string sResult = f ? "Saved." : "Failed to save user record (are you logged in)?";
                if (!f)
                {
                    string modal = DSQL.UI.GetModalDialogJson("Save User Record", sResult, String.Empty);
                    return Json(modal);

                }
                else
                {
                    string m = "location.href='/profile/profile';";
                    returnVal.returnbody = m;
                    returnVal.returntype = "javascript";
                    string o1 = JsonConvert.SerializeObject(returnVal);
                    return Json(o1);
                }
            }
            else if (o.Action == "Profile_Authenticate")
            {
                User u = GetUser(HttpContext);
                return Json(String.Empty);
            }
            else if (o.Action == "Profile_Authenticate_Full")
            {
                User u = GetUser(HttpContext);
                string m = "location.href='/profile/profile';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "profile_logout")
            {
                string m = "setCookie('erc20signature', '', 30);setCookie('erc20address','',30);location.href='/gospel/about';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Profile_RefreshBalance")
            {
                DSQL.UI.GetAvatarBalance(HttpContext, true);
                string m = "location.href='profile/profile';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Profile_Login")
            {
                string sNickName = GetFormData(o.FormData, "txtNickName");
                string sCode = GetFormData(o.FormData, "txtMFACode");

                string sError = String.Empty;
                string sMFASecret = DB.GetUserMFAByNickName(sNickName);
                if (sMFASecret == String.Empty)
                    sError = "Invalid Nick Name.";
                if (sError != String.Empty)
                {
                    return this.ShowModalDialog(o, "Error", sError, String.Empty);
                }
                if (sCode == String.Empty)
                    sError = "Invalid MFA Code.";
                if (sError != String.Empty)
                {
                    return this.ShowModalDialog(o, "Error", sError, String.Empty);
                }

                User uDummy = new User();
                uDummy.MFA = sMFASecret;
                bool isValid = BBPAPI.ERCUtilities.ValidateMFA(uDummy, sCode);

                if (!isValid)
                {
                    sError = "Invalid MFA Code.";
                    return this.ShowModalDialog(o, "Error", sError, String.Empty);
                }

                User u = GetUserMFA(HttpContext, sNickName, sCode);
                string sNarr = u.LoggedIn ? "Success" : "Failed.";
                return this.ShowModalDialog(o, sNarr, sNarr, String.Empty);

            }
            else if(o.Action == "Profile_SendVerificationEmail")
            {
                try
                {
                    User u = HttpContext.GetCurrentUser();
                    if (!u.LoggedIn)
                    {
                        return this.ShowModalDialog(o, "Error", "Sorry you must be logged in", String.Empty);
                    }

                    DACResult r = BBPAPI.ERCUtilities.SendVerificationEmail(u);

                    if (r.Error != String.Empty)
                    {
                        return this.ShowModalDialog(o, "Error", r.Error, String.Empty);
                    }
                    return this.ShowModalDialog(o, "Sent", "We sent you a verification E-mail.  Please check your inbox and as a last resort your junk folder. ", String.Empty);
                }catch(Exception)
                {
                    return this.ShowModalDialog(o, "Error", "Unable to send verification email [R001].", String.Empty);
                }
            }
            else if (o.Action == "Profile_AddTwoFactor")
            {
                string sMFASecret = GetFormData(o.FormData, "txtMFASecret");
                string sCode = GetFormData(o.FormData, "txtTwoFactorCode");

                // Verify MFA code first.
                string sError = String.Empty;
                TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
                bool isValid = twoFactor.ValidateTwoFactorPIN(sMFASecret, sCode);

                if (!isValid)
                    sError = "Sorry, the 2FA code you entered is not valid.  Please associate the QR code with your 2FA program first, then click the 2FA app and copy the code for this site, then paste the code in the Code textbox and try again.";
                User u = HttpContext.GetCurrentUser();

                if (u.LoggedIn == false)
                {
                    sError = "Sorry, you must be logged in.";
                }


                if (sError != String.Empty)
                {
                    return this.ShowModalDialog(o, "Error", sError, String.Empty);
                }
                // Save
                bool fOK = DB.Financial.UpdateUserMFA(sMFASecret, u.id);
                if (!fOK)
                {
                    return this.ShowModalDialog(o, "Error", "Unable to save.", String.Empty);
                }
                HttpContext.EraseUserCache();
                return this.ShowModalDialog(o, "Success", "Two Factor Enabled.", "location.reload();");
            }
            else if (o.Action == "Profile_Register")
            {
                string sError = String.Empty;
                string sNickName = GetFormData(o.FormData, "txtNickName");
                string sEmail = GetFormData(o.FormData, "txtEmailAddress");
                string sMFASecret = GetFormData(o.FormData, "txtMFASecret");
                string sCode = GetFormData(o.FormData, "txtTwoFactorCode");
                //string sUserID = GetFormData(o.FormData, "txtUserID");
                if (sNickName==String.Empty)
                {
                    sError = "Nick name must be populated.";
                }
                if (sEmail == String.Empty)
                    sError = "E-Mail must be populated.";
                if (!IsValidEmailAddress(sEmail))
                    sError = "E-Mail address is invalid.";
                // Check for duplicates
                double nNNCt = DB.GetUserCountByNickName(sNickName);
                if (nNNCt > 0)
                    sError = "Sorry, this Nickname is already in use.";

                double nECt = DB.GetUserCountByEmail(sEmail);
                if (nECt > 0)
                    sError = "Sorry, this e-mail address is already in use.";
                if (sMFASecret == String.Empty || sMFASecret.Length < 20)
                {
                    sError = "Sorry, 2FA seed is invalid.";
                }
                // Verify MFA code first.
                
                TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
                bool isValid = twoFactor.ValidateTwoFactorPIN(sMFASecret, sCode);
                
                if (!isValid)
                    sError = "Sorry, the 2FA code you entered is not valid.  Please associate the QR code with your 2FA program first, then click the 2FA app and copy the code for this site, then paste the code in the Code textbox and try again.";


                if (sError != String.Empty)
                {
                    return this.ShowModalDialog(o, "Error", sError, String.Empty);
                }


                // Save
                User u = new User();
                u.id = Guid.NewGuid().ToString();
                BBPAPI.ERCUtilities.SetMFAKey(u, sMFASecret);
                Encryption.KeyType k = GetKeyPairByGUID(IsTestNet(HttpContext), sMFASecret, String.Empty);
                u.BBPAddress = k.PubKey;
                u.NickName = sNickName;
                u.EmailAddress = sEmail;
                u.LoggedIn = true;
                u.Added = DateTime.Now;


                bool fSaved = PersistUser(IsTestNet(HttpContext), u);
                if (!fSaved)
                {
                    return this.ShowModalDialog(o, "Error", "Unable to save record", String.Empty);
                }
                string sBody = "Thank you for registering with Unchained!<br><br><ul><li>E-Mail address must be verified</ul><br>Thank you for using BiblePay.<br>";
                string d2 = DSQL.UI.MsgBoxJson(HttpContext, "Register", "Registration", sBody);
                return Json(d2);

            }
            else if (o.Action == "Profile_SendBBP")
            {
                string sToAddress = GetFormData(o.FormData, "txtSendToAddress");
                double nAmount = GetDouble(GetFormData(o.FormData, "txtAmountToSend"));
                string sPayload = "<XML>Send_BBP</XML>";
                Encryption.KeyType k = GetKeyPair(HttpContext, string.Empty);

                BMSCommon.Model.DACResult r0 = BBPAPI.Sanctuary.SendMoney(IsTestNet(HttpContext), k, nAmount, sToAddress, sPayload);
                string sResult = String.Empty;
                if (r0.TXID != String.Empty)
                {
                    sResult = "Sent " + nAmount.ToString() + " to " + sToAddress + " on TXID " + r0.TXID;
                    DSQL.UI.GetAvatarBalance(HttpContext, true);
                }
                else
                {
                    sResult = "Failed.  [" + r0.Error + "]";
                }

                string modal = DSQL.UI.GetModalDialog("Send BBP", sResult);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "Profile_ChangeChain")
            {
                if (GetChain(HttpContext) == "MAINNET")
                {
                    HttpContext.Session.SetString("Chain", "TESTNET");
                }
                else
                {
                    HttpContext.Session.SetString("Chain", "MAINNET");
                }
                string sNewChain = GetChain(HttpContext);
                string m = "location.href='profile/profile';"; // DSQL.UI.GetModalDialog("Switch Block Chain", "Chain has been switched to " + sNewChain, "location.href='le';");
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else
            {
                throw new Exception(String.Empty);
            }
        }
    }
}
