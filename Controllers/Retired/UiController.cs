using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BiblePay.BMS.Controllers
{
    public class UiController : Controller
    {

       
        public IActionResult Accordion()
        {
            ViewBag.Accordian = DSQL.UI.GetAccordian("a1", "This is an accordian", "<br>BBP<br><br><br>Now is the time");
            return View();
        }
        public IActionResult Alerts()
        {
            return View();
        }
        //public IActionResult Badges() => View();
        //public IActionResult Breadcrumbs() => View();
        //public IActionResult ButtonGroup() => View();
        //public IActionResult Buttons() => View();
        //public IActionResult Cards() => View();
        //public IActionResult Carousel() => View();
        //public IActionResult Collapse() => View();
        //public IActionResult Dropdowns() {
        //    return View();
        //}
        //public IActionResult ListFilter() => View();
        public IActionResult Modal() => View();
        public IActionResult Navbars() => View();
        public IActionResult Pagination() => View();
        //public IActionResult Panels() => View();
        //public IActionResult ProgressBars() => View();
        //public IActionResult Tooltips() => View();
        //public IActionResult TooltipsPopovers() => View();
    }
}
