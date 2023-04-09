using BiblePay.BMS.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OptionsShared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UIWallet;

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
        public IActionResult Projects() => View();
        public IActionResult Register() => View();
        public IActionResult Search() => View();
    }
}
