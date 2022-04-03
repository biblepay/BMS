using BiblePay.BMS;
using BiblePay.BMS.DSQL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblePay.BMS
{

   public class GhettoController : Controller
    {
        public IActionResult Details(int id)
        {

            return Ok("5");
        }
    }

    public class HomeController : Controller
    {
        [Route("")]
        [Route("Home")]
        [Route("Home/Index")]
        [Route("Home/Index/{id?}")]
        public IActionResult Index(int? id)
        {
            return Ok("6");
        }

        [Route("Home/About")]
        [Route("Home/About/{id?}")]
        public IActionResult About(int? id)
        {
            return Ok("7");
        }
    }



}
