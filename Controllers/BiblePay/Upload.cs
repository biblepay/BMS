using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BiblePay.BMS.Controllers
{
    public class Upload : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult UploadFile()
        {
            ViewBag.Message = "none";

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> UploadFile(List<IFormFile> file)
        {
            try
            {
                if (file.Count > 0)
                {
                    for (int i = 0; i < file.Count; i++)
                    {
                        string _FileName = Path.GetFileName(file[i].FileName);
                        string sGuid = Guid.NewGuid().ToString() + ".dat";

                        string sDestFN = Path.Combine(Path.GetTempPath(), sGuid);
                        using (var stream = new FileStream(sDestFN, System.IO.FileMode.Create))
                        {
                            await file[i].CopyToAsync(stream);
                        }

                    }
                }
                ViewBag.Message = "Sent " + file[0].FileName + " successfully";
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }

    }
}
