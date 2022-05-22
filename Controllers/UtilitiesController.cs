using Microsoft.AspNetCore.Mvc;

namespace BiblePay.BMS.Controllers
{
    public class UtilitiesController : Controller
    {
        public IActionResult Borders() => View();
        public IActionResult Sizing() => View();
    }
}
