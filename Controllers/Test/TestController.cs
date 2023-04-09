using BiblePay.BMS.DSQL;
using BiblePay.BMS.Models;
using BMSCommon.Model;
using Google.Authenticator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using static BiblePay.BMS.DSQL.DOMItem;


namespace BiblePay.BMS.Controllers
{
    public class TestController : Controller
    {
        
        public static object ChangeType(object value, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                return Convert.ChangeType(value, Nullable.GetUnderlyingType(type));
            }
            string sSourceType = value.GetType().ToString();
            string sDestType = type.ToString();
            if (sDestType == "System.String" && sSourceType == "System.Guid")
            {
                return value.ToString();
            }
            return Convert.ChangeType(value, type);
        }

        public static void BindObject<T>(object item, ClientToServer cts)
        {

            Type t = typeof(T);
            var myFields = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in myFields)
            {
                string newValue = GetFormData(cts.FormData, prop.Name);
                if (prop != null && !String.IsNullOrEmpty(newValue))
                {
                    prop.SetValue(item, ChangeType(newValue, prop.FieldType));
                }
            }

            var myProps = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in myProps)
            {
                string newValue = GetFormData(cts.FormData, prop.Name);
                if (prop != null && !String.IsNullOrEmpty(newValue))
                {
                    prop.SetValue(item, ChangeType(newValue, prop.PropertyType), null);
                }

            }

        }

        public ActionResult TestPage()
        {
            Models.City c = new City();
            c.Name1 = "Satoshi";
            c.Id = 1;
            return View(c);
        }
        
        public ActionResult TestSection10()
        {
            ClientToServer cts = HttpContext.Session.GetObject<ClientToServer>("post10");
            City c = new City();
            if (cts != null)
            {
                c = new City();
                BindObject<City>(c, cts);
            }
            c.Zip++;
            return PartialView("_TestSection10", c);
        }


        public JsonResult Test10([FromBody] BMSCommon.Model.ClientToServer o)
        {
            HttpContext.Session.SetObject("post10", o);
            return this.RedirectToSection(o, "partial-load-section10", "/Test/TestSection10");
        }

        public ActionResult TestSection1()
        {
             ViewBag.txtName = HttpContext.Session.GetFormValue("txtName");
             if (ViewBag.txtName.Length < 1)
             {
                 ViewBag.txtError = "<font color=red>YOU MUST ENTER IT</font>";
             }
             else
             {
                 ViewBag.txtError = "";
             }
            return PartialView("_TestSection1");
        }

 
        public JsonResult PostSection1([FromBody] ClientToServer o)
        {
            return this.RedirectToSection(o, "partial-load-section1", "/Test/TestPage");
        }

        public ActionResult AssociateTwoFA()
        {
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            string sSecret = "1";
            var setupInfo = twoFactor.GenerateSetupCode("TEST_UNCHAINED",
                "you@biblepay.org", sSecret, false, 3);
            ViewBag.SetupCode = setupInfo.ManualEntryKey;
            ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            return View("TestPage");
        }

        public ActionResult TestSection2()
        {
            ViewBag.txtCity = this.GetFormValue("txtCity");
            return PartialView("_TestSection2");
        }

        public JsonResult PostSection2([FromBody] ClientToServer o)
        {
            return this.RedirectToSection(o, "partial-load-section2", "/Test/TestSection2");
        }

        public ActionResult TestSection3()
        {
            ViewBag.txtState = this.GetFormValue("txtState2");

            return PartialView("_TestSection3");
        }
        
        public JsonResult PostSection3([FromBody] BMSCommon.Model.ClientToServer o)
        {
            string sJS = this.RedirectToSectionJS("partial-load-section3", "/Test/TestSection3");
            return this.ShowModalDialog(o, GetFormData(o.FormData,"txtState2"), "The value of the dialog", sJS);
        }

        public JsonResult Check2FA([FromBody] ClientToServer o)
        {
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            string sSecret = "1";
            string sCode = GetFormData(o.FormData, "txtCode");
            bool isValid = twoFactor.ValidateTwoFactorPIN(sSecret, sCode);
            string sNarr = isValid ? "Success" : "Failed";
            return this.ShowModalDialog(o, "auth", sNarr, "");
        }

        [HttpPost]
        public JsonResult AutoCity(string Prefix)
        {
            List<City> ObjList = new List<City>()
            {
                new City {Id=1,Name1="Autumn" },
                new City {Id=2,Name1="Bobert" },
                new City {Id=3,Name1="Pune" },
                new City {Id=4,Name1="Delhi" },
                new City {Id=5,Name1="Dehradun" },
                new City {Id=6,Name1="Noida" },
                new City {Id=7,Name1="New Delhi" }

            };
            //Searching records from list using LINQ query  
            var Name = (from N in ObjList
                        where N.Name1.StartsWith(Prefix)
                        select new { N.Name1 });
            return Json(Name);//, JsonRequestBehavior.AllowGet);
        }
        
    }
}
