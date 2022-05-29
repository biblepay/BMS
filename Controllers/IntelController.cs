using System;
using System.Collections.Generic;
using System.Net.Mail;
using BMSCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using static BiblePay.BMS.Controllers.BBPController;
using static BiblePay.BMS.DSQL.UI;
using static BMSCommon.WebRPC;

namespace BiblePay.BMS.Controllers
{
   
    public class IntelController : Controller
    {

        public class Employee
        {
            public string phone;
            public string name;
            public string date1;
            public string email;
            public string num1;
            public string name2;
            public string amt;
            public string dumb1;

        }
        
        public IActionResult MarketingDashboard()
        {
            List<Employee> e = new List<Employee>();
            Employee e1 = new Employee();
            Employee e2 = new Employee();
            e.Add(e1);
            e.Add(e2);
            var model = e;
            ViewBag.DynamicTable = DSQL.UI.GetBasicTable("t", "the new dynamic table");
            ViewBag.Notifications = DSQL.UI.GetNotifications();
            ViewBag.Accordian = DSQL.UI.GetAccordian("a1", "[Accordian] Collapse", "<br>A dynamic table inside a dynamic accordian.");
            return View(model);
        }
        public class ClientToServer
        {
            public string BBPAddress { get; set; }
            public string ERC20Signature { get; set; }
            public string ExtraData { get; set; }
            public string FormData { get; set; }
            public string Action { get; set; }
        }

       
        private static string Coalesce(string a,string b, string c, string d)
        {
            if (a != "")
                return a;
            if (b != "")
                return b;
            if (c != "")
                return c;
            if (d != "")
                return d;
            return "";
        }
        private static void UnivStoreAnswer(HttpContext h, string sFormData)
        {
            string sRadioA = GetFormData(sFormData, "radioAnswerA");
            string sRadioB = GetFormData(sFormData, "radioAnswerB");
            string sRadioC = GetFormData(sFormData, "radioAnswerC");
            string sRadioD = GetFormData(sFormData, "radioAnswerD");
            string sChosen = Coalesce(sRadioA, sRadioB, sRadioC, sRadioD);
            UnivFinalExamController.RecordAnswer(sChosen, h);
        }

        [HttpPost]
        public JsonResult ProcessDoCallback([FromBody] ClientToServer o)
        {
            ServerToClient returnVal = new ServerToClient();

            if (o.Action == "Profile_Save")
            {
                BMSCommon.CryptoUtils.User u = DSQL.UI.GetUser(HttpContext);
                bool fTestNet = DSQL.UI.IsTestNet(HttpContext);

                u.EmailAddress = GetFormData(o.FormData, "txtEmailAddress");
                u.Updated = System.DateTime.Now.ToString();
                u.NickName = GetFormData(o.FormData, "txtNickName");
                u.ERC20Address = GetFormData(o.FormData, "txtERC20Address");
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
                DSQL.UI.SetUser(u, HttpContext);
                bool f = BMSCommon.CryptoUtils.PersistUser(IsTestNet(HttpContext), u);
                string sResult = f ? "Saved." : "Failed to save user record.";
                string modal = DSQL.UI.GetModalDialogJson("Save User Record", sResult);
                return Json(modal);
            }
            else if (o.Action == "PortfolioBuilder_ToggleMode")
            {
                if (HttpContext.Session.GetString("PortfolioBuilderLeaderboardMode") != "Detail")
                {
                    HttpContext.Session.SetString("PortfolioBuilderLeaderboardMode", "Detail");
                }
                else
                {
                    HttpContext.Session.SetString("PortfolioBuilderLeaderboardMode", "Summary");
                }

                string m = "location.href='/bbp/portfoliobuilderleaderboard';"; 
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Profile_Authenticate")
            {
                BMSCommon.CryptoUtils.User u = DSQL.UI.GetUser(HttpContext);
                
                return Json("");
            }
            else if (o.Action == "Profile_Authenticate_Full")
            {
                BMSCommon.CryptoUtils.User u = DSQL.UI.GetUser(HttpContext);

                string m = "location.href='/page/profile';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "nft_create")
            {
                string sRedir = BBPController.btnSubmitNFT_Click(HttpContext, o.FormData, "create");

                return Json(sRedir);
            }
            else if (o.Action == "nft_editme")
            {
                string sRedir = BBPController.btnSubmitNFT_Click(HttpContext, o.FormData, "edit");
                return Json(sRedir);
            }
            else if (o.Action == "nft_buy")
            {
                dynamic oExtra = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sNFTID = oExtra.nftid.Value;
                double nBuyItNowAmount = oExtra.Amount.Value;
                string sChain = IsTestNet(HttpContext) ? "test" : "main";

                NFT n = BMSCommon.NFT.GetNFT(sChain,sNFTID);
                string sError = "";

                if (n==null)
                {
                    sError = "NFT not found";
                    string s1 = MsgBoxJson(HttpContext, "Error", "Error", "NFT not found.");
                    return Json(s1);
                }
                if (n.OwnerERC20Address == "" || n.OwnerBBPAddress == "" || n.OwnerERC20Address==null)
                {
                    sError += "NFT ERC 20 address is not populated; invalid nft.";
                }

                bool fValid = BMSCommon.WebRPC.ValidateBiblepayAddress(IsTestNet(HttpContext), n.OwnerBBPAddress);
                if (!fValid)
                {
                    sError += "Owner BBP address is not valid. ";
                }

                if (sError != "")
                {
                    string s1 = MsgBoxJson(HttpContext, "Error", "Error", "NFT not found.");
                    return Json(s1);
                }


                double nAmount = BMSCommon.Common.GetDouble(GetFormData(o.FormData, "txtAmountToSend"));
                if (nAmount <= 0)
                {
                    sError += "Buy amount must be > 0.";
                }

                if (n.LowestAcceptableAmount() <= 0)
                {
                    sError += "Lowest acceptable amount is too low.";
                }
                string sPayload = "<XML>BuyNFT</XML>";

                BMSCommon.WebRPC.DACResult r0 = DSQL.UI.SendBBP(HttpContext, n.OwnerBBPAddress, nAmount, sPayload);
                if (r0.TXID != "")
                {
                    // Transfer the actual NFT
                    n.TXID = r0.TXID;
                    n.Action = "buy";
                    n.OwnerERC20Address = GetUser(HttpContext).ERC20Address;
                    n.OwnerBBPAddress = GetUser(HttpContext).BBPAddress;
                    n.Marketable = 0;
                    n.time = BMSCommon.Common.UnixTimestamp();
                    n.Save(IsTestNet(HttpContext));
                    string s2 = MsgBoxJson(HttpContext, "Success", "Success", "You have successfully purchased this NFT on TXID " + r0.TXID + ".  ");
                    return Json(s2);
                }
                else
                {
                    string s3 = MsgBoxJson(HttpContext, "Error", "Error", "Purchase error. ");
                    return Json(s3);
                }
            }
            else if (o.Action == "profile_logout")
            {
                string m = "setCookie('erc20signature', '', 30);setCookie('erc20address','',30);location.href='/gospel/about';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "nft_edit")
            {
                dynamic a = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sID = a.nftid.Value;
                string m = "location.href='/bbp/nftadd?mode=edit&id=" + sID + "';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Profile_SendBBP")
            {
                string sToAddress = GetFormData(o.FormData, "txtSendToAddress");
                double nAmount = BMSCommon.Common.GetDouble(GetFormData(o.FormData, "txtAmountToSend"));
                string sPayload = "<XML>Send_BBP</XML>";
                BMSCommon.WebRPC.DACResult r0 = DSQL.UI.SendBBP(HttpContext, sToAddress, nAmount, sPayload);
                string sResult = "";
                if (r0.TXID != "")
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
            else if (o.Action == "Univ_Grade")
            {
                string sResults = BiblePay.BMS.Controllers.UnivFinalExamController.ShowResults(HttpContext);
                string modal = DSQL.UI.GetModalDialog("Final Exam Results", sResults);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "MemorizeScriptures_Grade")
            {
                string s = MemorizeScripturesController.btnGrade_Click(HttpContext, o.FormData);
                string modal = DSQL.UI.GetModalDialog("Results", s);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "scratch_submit")
            {
                string sScratchID = GetFormData(o.FormData, "txtScratchID");
                string sScratchValue = GetFormData(o.FormData, "txtScratchValue");
                string sError = "";
                if (sScratchID == "" || sScratchValue == "")
                {
                    sError = "Values must be populated";
                }
                else
                {
                    BMSCommon.Pricing.SetKeyValue("scratch_" + sScratchID, sScratchValue);
                }
                string sNarr = sError != "" ? sError : "Successfully saved.  Your data will be saved for 10 minutes and then erased.  "
                    + "NOTE: Once you access the data, we will rewrite the value with 'accessed' so that it cannot be accessed again (it can only be accessed once).";
                string modal = DSQL.UI.GetModalDialog("Results", sNarr);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "turnkey_submit")
            {
                string sError = "";
                if (!GetUser(HttpContext).LoggedIn)
                    sError = "Sorry, you must be logged in first.";
                string txtIP = GetFormData(o.FormData, "txtIPAddress");
                string txtSancName = GetFormData(o.FormData, "txtSanctuaryName");
                string txtConfig = GetFormData(o.FormData, "txtConfiguration");
                string txtRootPassword = GetFormData(o.FormData, "txtRootPassword");
                if (txtIP == "")
                    sError += "<br>Sorry, IP Address must be populated.";
                if (txtSancName == "")
                    sError += "<br>Sorry, Sanctuary Name must be populated.";
                if (txtConfig == "")
                    sError += "<br>Sorry, Configuration must be populated.";
                if (txtRootPassword == "")
                    sError += "<br>Sorry, root password must be populated.";
                if (GetUser(HttpContext).EmailAddress=="" || GetUser(HttpContext).EmailAddress==null)
                {
                    sError += "<br>Sorry, your e-mail address must be populated first.";
                }
                if (sError != "")
                {
                    string d = MsgBoxJson(HttpContext, "Turnkey Provision", "Error", sError);
                    return Json(d);
                }
                string sql = "Insert into turnkeysanctuary (id,erc20address,added,ip,sanctuaryname,configuration,rootpassword) values (uuid(),@erc20address,now(),@ip,@sanctuaryname,@config,@rootpassword);";
                MySqlCommand m1 = new MySqlCommand(sql);

                m1.Parameters.AddWithValue("@erc20address", GetUser(HttpContext).ERC20Address);
                m1.Parameters.AddWithValue("@ip", txtIP);
                m1.Parameters.AddWithValue("@sanctuaryname", txtSancName);
                m1.Parameters.AddWithValue("@config", txtConfig);
                m1.Parameters.AddWithValue("@rootpassword", txtRootPassword);

                bool fIns = BMSCommon.Database.ExecuteNonQuery(false, m1, "");
                if (!fIns)
                {
                    string d = MsgBoxJson(HttpContext, "Turnkey Provision", "Error", "Sorry, we were unable to provision your sanctuary.  Please e-mail contact@biblepay.org with the Config information and we will manually deal with this or fix the issue.  Thank you for using BiblePay.  ");
                    return Json(d);
                }
                string sNarr = "Congratulations.  Your sanctuary provisioning request has been received.  <br><br>"
                    +"Most sanctuaries are created within 24 hours.  We will reach out to you via E-mail once completed.  <br><br>Thank you for using BiblePay!";

                string d1 = MsgBoxJson(HttpContext, "Turnkey Provisioner", "Success", sNarr);

                MailMessage mm = new MailMessage();
                MailAddress mTo = new MailAddress("rob@biblepay.org", "Rob Andrews");
                mm.To.Add(mTo);
                mm.Subject = "New Sanctuary Provisioning Request";
                mm.Body = "Dear Team BiblePay, \r\nWe have received a new turnkey sanctuary provisioning request.  \r\n";
                mm.Body += " for " + txtIP + " for SanctuaryName: " + txtSancName + "\r\n\r\n Sincerely Yours, \r\nBiblePay BMS";

                mm.IsBodyHtml = false;

                DACResult dr1 = BMSCommon.API.SendMail(false, mm);

                return Json(d1);
            }
            else if (o.Action == "proposal_add")
            {
                string sRedir = BBPController.btnSubmitProposal_Click(HttpContext, o.FormData);
                return Json(sRedir);
            }
            else if (o.Action == "show_modal")
            {
                //Extra Data
                dynamic msg = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string modal = DSQL.UI.GetModalDialog("Information", msg.Narr.Value);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "MemorizeScriptures_Switch")
            {
                MemorizeScripturesController.btnSwitchMode_Click(HttpContext);
                string m = "location.href='memorizescriptures/memorizescriptures';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "MemorizeScriptures_Next")
            {
                MemorizeScripturesController.btnNextScripture_Click(HttpContext,o.FormData);
                string m = "location.href='memorizescriptures/memorizescriptures';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "test_1")
            {
                // MessagePage
                DSQL.UI.MsgBox(HttpContext, "BBP Long Process", "Error occurred", "object not set to an object.", false);
                string m = "location.href='bbp/messagepage';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Univ_Next")
            {
                string test = Request.Query["test"];
                UnivStoreAnswer(HttpContext, o.FormData);
                UnivFinalExamController.btnNext_Click(HttpContext);
                string m = "location.href='univfinalexam/univfinalexam';"; 
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Univ_Back")
            {
                string test = Request.Query["test"];
                UnivStoreAnswer(HttpContext, o.FormData);
                UnivFinalExamController.btnBack_Click(HttpContext);
                string m = "location.href='univfinalexam/univfinalexam';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Univ_Switch")
            {
                string test = Request.Query["test"];
                UnivFinalExamController.btnSwitch_Click(HttpContext);
                string m = "location.href='univfinalexam/univfinalexam';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Profile_ChangeChain")
            {
                if (DSQL.UI.GetChain(HttpContext) == "MAINNET")
                {
                    HttpContext.Session.SetString("Chain", "TESTNET");
                }
                else
                {
                    HttpContext.Session.SetString("Chain", "MAINNET");
                }
                string sNewChain = DSQL.UI.GetChain(HttpContext);
                string m = "location.href='page/profile';"; // DSQL.UI.GetModalDialog("Switch Block Chain", "Chain has been switched to " + sNewChain, "location.href='page/profile';");
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
           
            if (false)
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddDays(7));
            }
            return Json("");

        }


        public IActionResult AnalyticsDashboard() => View();
        public IActionResult Introduction() => View();
        
        public IActionResult Privacy() => View();
    }
}
