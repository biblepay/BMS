using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace BiblePay.BMS.Controllers
{
   

    public class IntelController : Controller
    {

        [Route("Homey/About")]
        [Route("Homey/About/{id?}")]
        public IActionResult About(int? id)
        {
            return Ok("7");
        }

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
            e1.phone = "9725551212";
            e1.name = "Choo, Umbert";
            e1.amt = "$4000";
            Employee e2 = new Employee();
            e2.phone = "4125551212";
            e2.name = "Chahoo, J";
            e2.amt = "$7000";
            e.Add(e1);
            e.Add(e2);

            var model = e;
            //            ViewBag.BigTimeTable = e;

//            ViewBag.BigTimeTable = "   <tr>   <td>4125551212</td> <td>Alio, R J.</td>  "
  //              + "<td>03-13-19</td>           <td>adio.auctor@zip.edu</td>  <td>717</td>      <td>Timor-Leste</td>          <td>$7,007</td>   <td>1</td>    </tr>";
            return View(model);
        }

        public IActionResult AnalyticsDashboard() => View();
        public IActionResult Introduction() => View();
        //public IActionResult MarketingDashboard() => View();
        public IActionResult Privacy() => View();
    }
}
