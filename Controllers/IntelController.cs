using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static BiblePay.BMS.DSQL.UI;


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
        public struct ServerToClient
        {
            public string returnbody;
            public string returntype;
            public string returnurl;
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
                bool f = BMSCommon.CryptoUtils.PersistUser(u);
                string sResult = f ? "Saved." : "Failed to save user record.";
                string modal = DSQL.UI.GetModalDialog("Save User Record", sResult);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "PortfolioBuilder_ToggleMode")
            {
                if (HttpContext.Session.GetString("PortfolioBuilderLeaderboardMode") == "Summary")
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
            else if (o.Action == "nft_add")
            {
                BBPController.btnSubmit_Click(HttpContext, o.FormData, "create");
                return Json("");
            }
            else if (o.Action == "profile_logout")
            {
                string m = "setCookie('erc20signature', '', 30);setCookie('erc20address','',30);location.href='/gospel/about';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Profile_SendBBP")
            {
                string sToAddress = GetFormData(o.FormData, "txtSendToAddress");
                double nAmount = BMSCommon.Common.GetDouble(GetFormData(o.FormData, "txtAmountToSend"));
                string sPayload = "<XML>Test</XML>";
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
