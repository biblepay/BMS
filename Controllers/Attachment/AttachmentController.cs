using BBPAPI;
using BMSCommon.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BiblePay.BMS.Controllers
{
    public class AttachmentController : Controller
    {

        [HttpPost]
        public async Task<IActionResult> UploadFileAttachment(List<IFormFile> file)
        {
            string sParentID = Request.Query["parentid"].ToString() ?? String.Empty;
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
                            Attachment a = new Attachment();
                            a.id = Guid.NewGuid().ToString();
                            a.FileName = fi.Name;

                            string sGuid = a.id + fi.Extension;
                            string sDestFN = Path.Combine(Path.GetTempPath(), sGuid);
                            using (var stream = new FileStream(sDestFN, System.IO.FileMode.Create))
                            {
                                await file[i].CopyToAsync(stream);
                            }

                            string sFullDest = "upload/photos/" + sGuid;
                            a.Version = 2;
                            a.URL = await StorjIO.StorjUpload(sDestFN, sFullDest, String.Empty);
                            a.ParentID = sParentID;
                            // Store the attachment object
                            DB.OperationProcs.StoreAttachment(a);
                            ServerToClient returnVal = new ServerToClient();
                            returnVal.returnbody = String.Empty;
                            returnVal.returntype = "uploadsuccessredirect";
                            returnVal.returnurl = a.URL;
                            string o1 = JsonConvert.SerializeObject(returnVal);
                            return Json(o1);
                        }
                        else
                        {
                            string modal = DSQL.UI.GetModalDialog("Save Attachment", "Extension not allowed");
                            ServerToClient returnVal = new ServerToClient();
                            returnVal.returntype = "modal";
                            returnVal.returnbody = modal;
                            string o1 = JsonConvert.SerializeObject(returnVal);
                            return Json(o1);
                        }
                    }
                }
                else
                {
                    return View();
                }
                ViewBag.Message = "Sent " + file[0].FileName + " successfully";
                Response.Redirect("/attachment/attachmentlist");
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }


        public static string GetAttachmentGallery(string sParentID)
        {
            string html = "<div class='fs-lg fw-300 p-5 bg-white border-faded rounded mb-g'><div class='row js-list-filter' id='al1'>";
            List<Attachment> l = DB.GetDatabaseObjectsAsAdmin<Attachment>("attachment");
            l = l.Where(s => s.Version >= 2 && s.ParentID == sParentID).ToList();

            string sMyCDN = "https://localhost:8440";

            for (int i = 0; i < l.Count; i++)
            {
                string sURL = sMyCDN + "/wwwroot/" +   l[i].URL;
                string sBgImg = sMyCDN + "/wwwroot/" + l[i].URL; // /img/demo/gallery/thumb/1.jpg

                string sItem = "<div class='col-xl-4'><div class='card border shadow-0 mb-g shadow-sm-hover' style='min-height:201px;'>"
                          + "<a _target=blank href='" + sURL + "' class='text-center px-3 py-4 d-flex position-relative height-10 border'>"
                     + "   <img style='width:200px;height:200px;' src='" + sBgImg + "'/>"
                     + "   </a> </div></div>\r\n";
                html += sItem;
            }
            html += "</div></div>";
            return html;
        }
        public IActionResult AttachmentList()
        {
            string sParentID = Request.Query["parentid"].ToString() ?? String.Empty;
            ViewBag.Attachments = GetAttachmentGallery(sParentID);
            return View();
        }
    }
}
