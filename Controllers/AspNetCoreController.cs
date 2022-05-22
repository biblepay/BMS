using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiblePay.BMS.Controllers
{
    [AllowAnonymous]
    public class AspNetCoreController : Controller
    {

        public IActionResult Welcome()
        {
            return View();
        }

        public IActionResult Interactive() => View();
        public IActionResult Editions() => View();
        public IActionResult Faq() => View();
    }
}
