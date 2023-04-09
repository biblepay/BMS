using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static BiblePay.BMS.DSQL.DOMItem;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UI;
using static BiblePay.BMS.DSQL.UIWallet;
using static BMSCommon.BitcoinSync;
using BMSCommon.Model;


namespace BiblePay.BMS.Controllers
{
    public class TimelineController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> UploadFileTimeline(List<IFormFile> file)
        {
            try
            {
                if (file.Count > 0)
                {
                    for (int i = 0; i < file.Count;)
                    {
                        string _FileName = Path.GetFileName(file[i].FileName);
                        bool fOK = DSQL.UI.IsAllowableExtension(_FileName);
                        if (fOK)
                        {
                            FileInfo fi = new FileInfo(_FileName);
                            string sGuid = Guid.NewGuid().ToString() + "" + fi.Extension;
                            string sDestFN = Path.Combine(Path.GetTempPath(), sGuid);
                            using (var stream = new FileStream(sDestFN, System.IO.FileMode.Create))
                            {
                                await file[i].CopyToAsync(stream);
                            }

                            string sURL = await BBPAPI.IPFS.UploadIPFS(sDestFN, "upload/photos/" + sGuid, GlobalSettings.GetCDN());
                            ServerToClient returnVal = new ServerToClient();
                            returnVal.returnbody = "";
                            returnVal.returntype = "uploadsuccess";
                            returnVal.returnurl = sURL;
                            string o1 = JsonConvert.SerializeObject(returnVal);
                            return Json(o1);
                        }
                        else
                        {
                            string modal = DSQL.UI.GetModalDialog("Save Timeline Image", "Extension not allowed");
                            BMSCommon.Model.ServerToClient returnVal = new ServerToClient();
                            returnVal.returntype = "modal";
                            returnVal.returnbody = modal;
                            string o1 = JsonConvert.SerializeObject(returnVal);
                            return Json(o1);
                        }
                    }
                }
                ViewBag.Message = "Sent " + file[0].FileName + " successfully";
                Response.Redirect("/bbp/timeline");
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }

        public IActionResult Timeline()
        {
            ViewBag.Timeline = DSQL.UI.GetTimelinePostDiv(HttpContext, "main");
            return View();
        }




    }
}
