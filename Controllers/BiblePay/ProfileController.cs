using BBPAPI;
using BBPAPI.Model;
using BBPAPI.Utilities;
using BiblePay.BMS.DSQL;
using BiblePay.BMS.Extensions;
using BMSCommon;
using BMSCommon.Model;
using Google.Authenticator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static BiblePay.BMS.Controllers.AttachmentController;
using static BiblePay.BMS.DSQL.DOMItem;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UIWallet;
using static BMSCommon.Common;
using static BMSCommon.Encryption;

namespace BiblePay.BMS.Controllers
{
	public class ProfileController : Controller
    {

        public IActionResult Wallet()
        {
            //Encryption.KeyType k = GetKeyPair(HttpContext);
            ViewBag.BBPAddress = HttpContext.GetCurrentUser().GetPublicKey();

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
            User u = BBPAPI.Model.UserFunctions.GetUserByID(false, sUserID);
            if (u == null)
                sError = "Invalid User.";
            if (sError != String.Empty)
            {
                DSQL.UI.MsgBox(HttpContext, "Error", "Error", sError, true);
                return View();
            }
            u.EmailAddressVerified = 1;
            u.LoggedIn = true;
            bool fOK = BBPAPI.Interface.Repository.UpdateUserEmailAsVerified(sUserID);
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
            BBPKeyPair k = GetKeyPairByGUID(IsTestNet(HttpContext), ViewBag.MFASecret, String.Empty);
            string sSite = IsTestNet(HttpContext) ? "TEST unchained.biblepay.org" : "unchained.biblepay.org";
            var setupInfo = twoFactor.GenerateSetupCode(sSite, k.PubKey, ViewBag.MFASecret, false, 3);
            ViewBag.TwoFactorManualSetupCode = setupInfo.ManualEntryKey;
            ViewBag.TwoFactorQRImageUrl = setupInfo.QrCodeSetupImageUrl;
        }

        public IActionResult EmailMaintenance()
        {
            List<EmailAccount> l = BBPAPI.Interface.Repository.GetEmailAccounts(HttpContext.GetCurrentUser().GetPublicKey());
            if (l.Count > 0)
            {
                ViewBag.BBPAddress = l[0].BBPAddress;
                ViewBag.txtUserName = l[0].UserName;
                ViewBag.EmailAddress = l[0].UserName + "@biblepay.org";
                ViewBag.ButtonDisabled = "disabled";
            }
            else
            {
                ViewBag.EmailAddress = "Not Provisioned";
                ViewBag.BBPAddress = "?";
                ViewBag.ButtonDisabled = "";
            }
            return View();
        }
        public IActionResult Register()
        {
            SetupTwoFactorAuthentication();
            return View();
        }
        public IActionResult Profile()
        {
            User u = HttpContext.GetCurrentUser();
            ViewBag.NickName = u.NickName;
            if (String.IsNullOrEmpty(ViewBag.NickName))
                ViewBag.NickName = "Guest";
            bool fTestNet = IsTestNet(HttpContext);
            try
            {
                ViewBag.EmailAddress = BBPAPI.ERCUtilities.BindEmailAddressValue(u);
                BMSCommon.Common.Log(ViewBag.EmailAddress);
                ViewBag.EmailAddressVerified = u.EmailAddressVerified == 1 ? "Yes" : "No";
                ViewBag.BioURL = DSQL.UI.GetBioURL(HttpContext);
                ViewBag.Balance = DSQL.UI.GetAvatarBalance(HttpContext, false);
                ViewBag.ERC20Address = u.ERC20Address;
                if (String.IsNullOrEmpty(u.MFA) && u.LoggedIn)
                {
                    SetupTwoFactorAuthentication();
                }
            }
            catch (Exception ex1)
            {
                BMSCommon.Common.Log(ex1.Message);
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Profile(List<IFormFile> file)
        {
            User u = HttpContext.GetCurrentUser();
			if (file.Count < 1)
			{
				throw new Exception("no file");
			}

            for (int i = 0; i < file.Count;)
            {
				FreshUpload fresh = new FreshUpload(this, file[i]);
				if (fresh.NotAllowedError != null)
					return fresh.NotAllowedError;
                // Change the avatar (check the extension too)
                Pin p = new Pin();
                p = PinLogic.StoreFile(u, fresh.FullDiskFileName, fresh.StorjDestination, "");
                u.BioURL = p.URL;
                u.Updated = System.DateTime.Now;
                SetUser(u, HttpContext);
                bool f = BBPAPI.Interface.Repository.PersistUser(u);
				ViewBag.Message = "Sent " + file[0].FileName + " successfully";
				Response.Redirect("/profile/profile");
				return View();
            }
            throw new Exception("no file.");
        }


        [HttpPost]
        public async Task<JsonResult> ProcessDoCallback([FromBody] ClientToServer o)
        {
            ServerToClient returnVal = new ServerToClient();
            User u0 = GetUser(HttpContext);

            if (o.Action == "Profile_Save")
            {
                User u = GetUser(HttpContext);
                bool fTestNet = IsTestNet(HttpContext);
                BBPAPI.ERCUtilities.SetUserEmail2(u,GetFormData(o.FormData, "txtEmailAddress"));

                u.Updated = System.DateTime.Now;
                u.NickName = GetFormData(o.FormData, "txtNickName");
                
                if (String.IsNullOrEmpty(u.id))
                {
                    u.id = Guid.NewGuid().ToString();
                }
                bool f = BBPAPI.Interface.Repository.PersistUser(u);
                string sResult = f ? "Saved." : "Failed to save user record (are you logged in)?";
                if (!f)
                {
                    string modal = DSQL.UI.GetModalDialogJson("Save User Record", sResult, String.Empty);
                    return Json(modal);
                }
                else
                {

                    SetUser(u, HttpContext);
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
                string m = "location.href='profile/wallet';";
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
                string sMFASecret = "";// ?? DB.GetUserMFAByNickName(sNickName);
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
                bool fOK = false;// DB.Financial.UpdateUserMFA(sMFASecret, u.id);
                if (!fOK)
                {
                    return this.ShowModalDialog(o, "Error", "Unable to save.", String.Empty);
                }
                HttpContext.EraseUserCache();
                return this.ShowModalDialog(o, "Success", "Two Factor Enabled.", "location.reload();");
            }
            else if (o.Action == "Profile_Register")
            {
                // NOTE** This is only called from our MFA Profile_Register page which may or may not be used long term.

                string sError = String.Empty;
                string sNickName = GetFormData(o.FormData, "txtNickName");
                string sEmail = GetFormData(o.FormData, "txtEmailAddress");
                string sMFASecret = GetFormData(o.FormData, "txtMFASecret");
                string sCode = GetFormData(o.FormData, "txtTwoFactorCode");
                if (sNickName==String.Empty)
                {
                    sError = "Nick name must be populated.";
                }
                if (sEmail == String.Empty)
                    sError = "E-Mail must be populated.";
                if (!IsValidEmailAddress(sEmail))
                    sError = "E-Mail address is invalid.";
                // Check for duplicates
                double nNNCt = BBPAPI.Interface.Repository.GetUserCountByNickName(sNickName);
                if (nNNCt > 0)
                    sError = "Sorry, this Nickname is already in use.";

                double nECt = BBPAPI.Interface.Repository.GetUserCountByEmail(sEmail);
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
                BBPKeyPair k = GetKeyPairByGUID(IsTestNet(HttpContext), sMFASecret, String.Empty);
                u.BBPAddress = k.PubKey;
                u.NickName = sNickName;
                u.EmailAddress = sEmail;
                u.Added = DateTime.Now;
                u.Updated = DateTime.Now;   


                bool fSaved = BBPAPI.Interface.Repository.PersistUser(u);
                if (!fSaved)
                {
                    return this.ShowModalDialog(o, "Error", "Unable to save record", String.Empty);
                }
                string sBody = "Thank you for registering with Unchained!<br><br><ul><li>E-Mail address must be verified</ul><br>Thank you for using BiblePay.<br>";
                string d2 = DSQL.UI.MsgBoxJson(HttpContext, "Register", "Registration", sBody);
                return Json(d2);

            }
            else if (o.Action == "Profile_AddEmail")
            {
                // if user balance is too low, reject
                // if email is already associated reject
                double nBal = DSQL.UI.GetAvatarBalance(HttpContext, false).ToDouble();
                if (nBal < 1000)
                {
                    return this.ShowModalDialog(o, "Error", "BBP Balance must be > 1000.", String.Empty);
                }
                BBPKeyPair k = HttpContext.GetCurrentUser().GetKeyPair();
                List<EmailAccount> l = BBPAPI.Interface.Repository.GetEmailAccounts(k.PubKey);
                if (l.Count > 0)
                {
                    return this.ShowModalDialog(o, "Error", "Sorry, you already have an e-mail account.", String.Empty);
                }
                string sName = GetFormData(o.FormData, "txtUserName");
                if (sName.Length < 1 || sName==null || sName.Length > 64 || sName.Contains(" "))
                {
                    return this.ShowModalDialog(o, "Error", "Sorry, the email prefix must be valid.", String.Empty);
                }

                int nResp = BBPAPI.Interface.EMail.ProvisionBBPEmailService(sName, k.PrivKey).Result;
                if (nResp != 1)
                {
                    return this.ShowModalDialog(o, "Error", "Sorry, the email account could not be provisioned, Error [" + nResp.ToString() + "].", String.Empty);
                }

                string sMsg = "Your E-Mail account has been provisioned.  Thank you for using BIBLEPAY!";
                return this.ShowModalDialog(o, "Success", sMsg, "location.href='profile/emailmaintenance';");

            }
            else if (o.Action == "Profile_SendBBP")
            {
                string sToAddress = GetFormData(o.FormData, "txtSendToAddress");
                double nAmount = GetDouble(GetFormData(o.FormData, "txtAmountToSend"));
                string sPayload = "<XML>Send_BBP</XML>";
                SendMoneyRequest smr = new SendMoneyRequest();
                smr.PrivateKey = HttpContext.GetCurrentUser().GetPrivateKey();
                smr.nAmount = nAmount;
                smr.sToAddress = sToAddress;
                smr.sOptPayload = sPayload;
                smr.TestNet = IsTestNet(HttpContext);
                BMSCommon.Model.DACResult r0 = await BBPAPI.Interface.WebRPC.SendMoney(smr);
                string sResult = String.Empty;
                if (r0.TXID != String.Empty)
                {
                    sResult = "Sent " + nAmount.ToString() + " to " + sToAddress + " on TXID <label style='font-size:7px;'>" + r0.TXID + "</label>.";

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
