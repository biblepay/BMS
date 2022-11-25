using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BiblePay.BMS.Models;
using static BMSCommon.CryptoUtils;
using BiblePay.BMS.Extensions;

namespace BiblePay.BMS.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            User uGlobal = HttpContext.GetCurrentUser();

            var items = NavigationModel.BuildNavigation(uGlobal.ERC20Address, false);
            
            return View(items);
        }
    }
}
