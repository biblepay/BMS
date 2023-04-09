using BBPAPI;
using BBPAPI.Model;
using BMSCommon;
using BMSCommon.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static BBPAPI.ERCUtilities;
using static BiblePay.BMS.DSQL.DOMItem;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UI;
using static BMSCommon.Common;
using static BMSCommon.Extensions;


namespace BiblePay.BMS.Controllers
{
    public class PhoneController : Controller
    {
        public static string States = "Alabama, Alaska, Arizona, Arkansas, California, Colorado, Connecticut, Delaware, Florida, Georgia, Hawaii, Idaho, Illinois, Indiana, Iowa, Kansas, Kentucky, Louisiana, Maine, Maryland, Massachusetts, Michigan, Minnesota, Mississippi, Missouri, Montana, Nebraska, Nevada, New Hampshire, New Jersey, New Mexico, New York, North Carolina, North Dakota, Ohio, Oklahoma, Oregon, Pennsylvania, Rhode Island, South Carolina, South Dakota, Tennessee, Texas, Utah, Vermont, Virginia, Washington, West Virginia, Wisconsin, Wyoming";
        public static string StateAbbrev = "AL, AK, AZ, AR, CA, CO, CT, DE, FL, GA, HI, ID, IL, IN, IA, KS, KY, LA, ME, MD, MA, MI, MN, MS, MO, MT, NE, NV, NH, NJ, NM, NY, NC, ND, OH, OK, OR, PA, RI, SC, SD, TN, TX, UT, VT, VA, WA, WV, WI, WY";

        private JsonResult Navigate(string sURL)
        {
            ServerToClient returnVal = new ServerToClient();
            string m = "location.href='" + sURL + "';";
            returnVal.returnbody = m;
            returnVal.returntype = "javascript";
            string o1 = JsonConvert.SerializeObject(returnVal);
            return Json(o1);
        }

        private JsonResult Javascript(string sJS)
        {
            ServerToClient returnVal = new ServerToClient();
            returnVal.returnbody = sJS;
            returnVal.returntype = "javascript";
            string o1 = JsonConvert.SerializeObject(returnVal);
            return Json(o1);
        }
        
        public string CallHistoryReport()
        {
            PhoneUser pu = GetPhoneUser();
            DataTable dt1 = DB.PhoneProcs.GetCallHistoryReport((long)GetDouble(pu.BBPPhoneID));
            string sHTML = "<table width=100%><tr><th>Added<th>From Number<th>To Number<th>Charge<th>TXID<th>Bill</tr>";
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                DataRow dr = dt1.Rows[i];
                string sRow = "<tr><td>" + dr["Added"].ToMilitaryTime()
                    + "<td>" + dr["fromnumber"].ToString() + "<td>" + dr["tonumber"].ToString()
                    + "<td>" + dr["amountbbp"].ToCurrencyBBP() 
                    + "<td style='font-size:4px;'>" + dr["TXID"].ToString() + "<td>" 
                    + dr["amountbilled"].ToCurrencyBBP() + "</tr>\r\n";
                sHTML += sRow;
            }
            sHTML += "</table>";
            return sHTML;
        }

        public IActionResult CallHistory()
        {
            ViewBag.CallHistoryReport = CallHistoryReport();
            return View();
        }
        public class PhoneUser
        {
            public string BBPPhoneID { get; set; }
            public string BBPUN { get; set; }
            public string BBPPK { get; set; }
            public string BBPPW { get; set; }
            public string BBPAddress { get; set; }
            public double Balance { get; set; }
            public string PhoneNumber { get; set; }
            public PhoneUser()
            {

            }

        }
        private PhoneUser GetPhoneUser()
        {
            PhoneUser pu = new PhoneUser();
            string sParam1 = HttpContext.Session.GetString("bbpaddress") ?? String.Empty;
            string sParam2 = HttpContext.Session.GetString("bbppk") ?? String.Empty;
            DataTable dt1 = DB.PhoneProcs.GetPhoneUser(sParam1, sParam2);
            if (dt1.Rows.Count > 0)
            {
                pu.BBPPhoneID = dt1.Rows[0]["id"].ToString() ?? String.Empty;
                pu.BBPUN = dt1.Rows[0]["username"].ToString() ?? String.Empty;
                pu.BBPPW = dt1.Rows[0]["userpassword"].ToString() ?? String.Empty;
                pu.PhoneNumber = dt1.Rows[0]["PhoneNumber"].ToString() ?? String.Empty;
            }
            pu.Balance = QueryAddressBalance(false, sParam1);
            pu.BBPAddress = sParam1;
            pu.BBPPK = sParam2;
            return pu;
        }

        public IActionResult PhoneService()
        {
            string s1 = Request.Query["param1"].ToString() ?? String.Empty;
            string s2 = Request.Query["param2"].ToString() ?? String.Empty;
            string sParam1 = Encryption.Base64Decode(System.Net.WebUtility.UrlDecode(s1));
            string sParam2 = Encryption.Base64Decode(System.Net.WebUtility.UrlDecode(s2));
            HttpContext.Session.SetString("bbpaddress", sParam1);
            HttpContext.Session.SetString("bbppk", sParam2);
            PhoneUser pu = GetPhoneUser();
            ViewBag.PhoneUser = pu;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ProcessDoCallback([FromBody] ClientToServer o)
        {
            //BMSCommon.Model.ServerToClient returnVal = new ServerToClient();
            User u0 = GetUser(HttpContext);
            if (o.Action == "phone_addnumber")
            {
                // Make a new Phone User Here
                PhoneUser pu = GetPhoneUser();
                if (pu.Balance < 1000)
                {
                    return Json(DSQL.UI.GetModalDialogJson("Provision", "Phone balance too low.  You can top up your balance by sending BBP to your Phone Address."));
                }

                if (GetDouble(pu.BBPPhoneID) > 0)
                {
                    return Json(DSQL.UI.GetModalDialogJson("Provision", "Phone already provisioned."));
                }
                string sPhoneNumber = GetFormData(o.FormData, "ddPhoneNum");
                int nRegionID = 0;
                // Verify this User-BBP address is not in use already....
                long nNewUserID = 0;
                try
                {
                    nNewUserID = await DB.PhoneProcs.AddNewPhoneUser(pu.BBPAddress, pu.BBPPK);
                }
                catch(Exception ex1)
                {
                    Log("ProvisionNewUser::" + ex1.Message);
                    return Json(DSQL.UI.GetModalDialogJson("Provision", "Failed during selection of username"));
                }
                // At this point save the phone number;
                bool fSuccess = BBPAPI.DB.PhoneProcs.InsertPhoneUser(nNewUserID, pu.BBPAddress, pu.BBPPK);
                if (!fSuccess)
                {
                    return Json(DSQL.UI.GetModalDialogJson("Provision", "Failed during attachment of user ID."));
                }

                nRegionID = (int)GetDouble(GetFormData(o.FormData, "ddRegion"));
                string sCountryState = GetFormData(o.FormData, "ddState");
                long nOK = await DB.PhoneProcs.AttachPhoneNumber(nRegionID, sPhoneNumber, sCountryState, pu.BBPAddress);
                if (nOK == 0)
                {
                    return Json(DSQL.UI.GetModalDialogJson("Provision", "Failed during attachment of new phone number"));
                }
                
                bool f1000 = BBPAPI.DB.PhoneProcs.SetPhoneUserPhoneNumber(nNewUserID, sPhoneNumber);
                if (!f1000)
                {
                    return Json(DSQL.UI.GetModalDialogJson("Provision", "Failed during creation of new phone number"));
                }
                bool f1002=DB.PhoneProcs.SetPhoneRulesCreated(nNewUserID);
                if (!f1002)
                {
                    return Json(DSQL.UI.GetModalDialogJson("Provision", "Failed during setting of rules creation."));
                }
                string sResult = "Your BBP Phone has been created!  Your phone number is " + sPhoneNumber + ".  To use your phone simply click the Phone tab.  You can also launch the phone application from your BBP wallet (click Phone from the left menu).  Thank you for using BIBLEPAY and God Bless you!";
                string modal = DSQL.UI.GetModalDialogJson("Provision Phone Service", sResult, "location.reload();");
                return Json(modal);
            }
            else if (o.Action == "phone_changestate")
            {
                HttpContext.Session.SetString("ddState", GetFormData(o.FormData, "ddState"));
                HttpContext.Session.SetString("ddRegion", "");
                string js = "$('#partial-load-provision').load('/Phone/AddPhoneNumber');";
                return Javascript(js);
            }
            else if (o.Action == "phone_changeregion")
            {
                HttpContext.Session.SetString("ddRegion", GetFormData(o.FormData, "ddRegion"));
                string js = "$('#partial-load-provision').load('/Phone/AddPhoneNumber');";
                return Javascript(js);
            }
            else
            {
                throw new Exception("Unknown Option");
            }
            
        }
        
        public async Task<ActionResult> AddPhoneNumber()
        {

            List<DropDownItem> ddRegions = new List<DropDownItem>();
            List<DropDownItem> ddPhoneNums = new List<DropDownItem>();
            List<DropDownItem> ddStates = new List<DropDownItem>();
            string[] vStates = States.Split(",");
            string[] vStateAbbrev = StateAbbrev.Split(",");
            string sRegionID = HttpContext.Session.GetString("ddRegion") ?? String.Empty;
            string sState = HttpContext.Session.GetString("ddState") ?? String.Empty;
            if (String.IsNullOrEmpty(sState))
            {
                sState = "AL";
            }
            ddRegions = await DB.PhoneProcs.GetRegions(sState.Trim());
            if (String.IsNullOrEmpty(sRegionID))
            {
                sRegionID = ddRegions[0].key;
            }

            if (ddRegions.Count > 0 )
            {
                ddPhoneNums = await DB.PhoneProcs.GetPhoneNumbersForRegion((long)GetDouble(sRegionID.Trim()), sState.Trim());
            }

            for (int i = 0; i < vStates.Length; i++)
            {
                ddStates.Add(new DropDownItem(vStateAbbrev[i].Trim(), vStates[i].Trim()));
            }
            ViewBag.ddState = ListToHTMLSelect(ddStates, sState);
			ViewBag.ddRegion = ListToHTMLSelect(ddRegions, sRegionID);
            ViewBag.ddPhoneNum = ListToHTMLSelect(ddPhoneNums, "Selected");
            return View();
        }

    }
}
