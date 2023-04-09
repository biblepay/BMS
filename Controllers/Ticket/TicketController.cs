using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using BiblePay.BMS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using BMSCommon.Model;
using static BiblePay.BMS.DSQL.DOMItem;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.ControllerExtensions;
using static BiblePay.BMS.DSQL.UI;
using static BMSCommon.Extensions;
using BiblePay.BMS.Extensions;
using System.Linq;
using BBPAPI.Model;
using BBPAPI;

namespace BiblePay.BMS.Controllers
{
    public class TicketController : Controller
    {
        private static int GetMaxTicketNumber()
        {
            List<Ticket> lTickets = BBPAPI.DB.GetDatabaseObjectsAsAdmin<Ticket>("ticket");
            int r = lTickets.Max(a => a.TicketNumber);
            return r;
        }
        private static List<Ticket> GetTicketList(string sUser, string sID)
        {
            List<Ticket> lTickets = BBPAPI.DB.GetDatabaseObjectsAsAdmin<Ticket>("ticket");
            if (sID == String.Empty)
            {
                lTickets = lTickets.Where(s => s.AssignedTo == sUser && s.Version > 1).ToList();
            }
            else
            {
                lTickets = lTickets.Where(s => s.id == sID).ToList();
            }
            foreach (Ticket t in lTickets)
            {
                t.BioURL = GetAvatarPictureWithName(false, t.ERC20Address);
                t.AssignedToBioURL = GetAvatarPictureWithName(false, t.AssignedTo);
            }
            return lTickets;
        }
        public static string GetTicketListReport(string sUser, string sID)
        {
      
            string html = "<table class=saved>";
            html +=  "<tr><th>Ticket #<th>Name<th>Assigned To<th>Updated</tr>";
            List<Ticket> lTickets = GetTicketList(sUser, sID);
            foreach (Ticket t in lTickets)
            {
                string sAnchor = "<a href='ticket/ticketview?id=" + t.id + "'>" + t.TicketNumber + "</a>";
                string sRow = "<tr><td>" + sAnchor
                        + "<td>" + t.Title
                        + "<td>" + t.AssignedToBioURL
                        + "<td>" + t.Updated.ToMilitaryTime() + "</tr>";
                html += sRow;
            }
            html += "</table>";
            return html;
        }

        public static string GetTicketHistory(string sParentID)
        {
            string html = "<table class=saved width=100%>";
            html += "<tr><th>Assigned To<th>Updated<th>Disposition</tr>";
            List<TicketHistory> lTH = DB.GetDatabaseObjectsAsAdmin<TicketHistory>("tickethistory");
                lTH = lTH.Where(s => s.ParentID == sParentID && s.AssignedTo != null).ToList();
            foreach (TicketHistory th in lTH)
            {
                th.AssignedToBioURL = GetAvatarPictureWithName(false, th.AssignedTo);
                string sRow = "<tr><td>" + th.AssignedToBioURL
                        + "<td>" + th.Added.ToMilitaryTime()
                        + "<td>" + th.Disposition + "</tr>";
                sRow += "<tr><td colspan=3> " + th.Notes + "</td></tr>";
                html += sRow;
            }
            html += "</table>";
            return html;
        }



        [HttpPost]
        public JsonResult ProcessDoCallback([FromBody] ClientToServer o)
        {
            //ServerToClient returnVal = new ServerToClient();
            User u0 = GetUser(HttpContext);

            if (o.Action == "ticket_submit")
            {
                Ticket t = new Ticket();
                t.Added = DateTime.Now;
                t.Updated = DateTime.Now;
                t.AssignedTo = GetFormData(o.FormData, "ddAssignTo");
                t.Disposition = GetFormData(o.FormData, "ddDisposition");
                t.Description = GetFormData(o.FormData, "txtDescription");
                t.Title = GetFormData(o.FormData, "txtName");
                t.TicketNumber = GetMaxTicketNumber() + 1;
                t.ERC20Address = HttpContext.GetCurrentUser().ERC20Address;
                t.id = Guid.NewGuid().ToString();
                t.Version = 2;
                string sErr = String.Empty;
                if (t.Title == String.Empty)
                {
                    sErr = "Title must be populated.";
                }
                else if (t.Description == String.Empty)
                {
                    sErr = "Description must be populated.";
                }
                if (sErr != String.Empty)
                {
                    return Json(MsgBoxJson(HttpContext, "Ticket Error", "Ticket Error", sErr));
                }

                bool f20 = DB.StoreDataAsAdmin<Ticket>("options.ticket", t, t.id);
                return this.ReturnURL("ticket/ticketlist");
            }
            else if (o.Action == "ticket_edit")
            {
                string sID = GetFormData(o.FormData, "txtID");
                List<Ticket> tList = GetTicketList(String.Empty, sID);
                if (tList.Count > 0)
                {
                    Ticket t = tList[0];
                    t.Updated = DateTime.Now;
                    t.AssignedTo = GetFormData(o.FormData, "ddAssignTo");
                    t.Disposition = GetFormData(o.FormData, "ddDisposition");
                    if (t.Disposition == String.Empty)
                    {
                        return Json(MsgBoxJson(HttpContext, "Error", "Error", "Disposition must be populated."));
                    }
                    else if (t.AssignedTo == String.Empty)
                    {
                        return Json(MsgBoxJson(HttpContext, "Error", "Error", "Assigned to must be populated."));

                    }
                    bool f19 = DB.StoreDataAsAdmin<Ticket>("options.ticket", t, t.id);
                    TicketHistory th = new TicketHistory();
                    th.ParentID = t.id;
                    th.id = Guid.NewGuid().ToString();
                    th.Added = DateTime.Now;
                    th.AssignedTo = GetFormData(o.FormData, "ddAssignTo");
                    th.Disposition = GetFormData(o.FormData, "ddDisposition");
                    th.Updated = DateTime.Now;
                    th.Notes = GetFormData(o.FormData, "txtNotes");
                    bool f22 = DB.StoreDataAsAdmin<TicketHistory>("options.tickethistory", th, th.id);
                }
                return this.ReturnURL("ticket/ticketlist");
            }

            else
            {
                throw new Exception("");
            }
        }


        public async Task<IActionResult> TicketView()
        {

            ViewBag.TicketID = Request.Query["id"].ToString() ?? String.Empty;
            List<Ticket> l = await GetTicketList(String.Empty, ViewBag.TicketID);
            if (l.Count == 0)
            {
                return this.ReturnURL("ticket/ticketlist");
            }
            TicketModel model = GetModel(l[0]);
            ViewBag.Ticket = l[0];
            ViewBag.TicketHistory = await GetTicketHistory(ViewBag.TicketID);
            return View(model);
        }

        private TicketModel GetModel(BMSCommon.Model.Ticket t)
        {
            TicketModel model = new TicketModel();
            model.DispositionList.Add(new SelectListItem { Text = "Change Approval", Value = "ChangeApproval" });
            model.DispositionList.Add(new SelectListItem { Text = "Programming", Value = "Programming" });
            model.DispositionList.Add(new SelectListItem { Text = "Code Review", Value = "CodeReview" });
            model.DispositionList.Add(new SelectListItem { Text = "Release To Production", Value = "ReleaseToProduction" });
            if (t.Disposition != null)
            {
                model.DispositionList.FirstOrDefault(c => c.Value == t.Disposition).Selected = true;
            }

            // Assign To
            List<SelectListItem> li = new List<SelectListItem>();
            List<User> l = DB.GetDatabaseObjectsAsAdmin<User>("user");
            foreach (User u in l)
            {
                if (!String.IsNullOrEmpty(u.NickName))
                {
                    SelectListItem li1 = new SelectListItem();
                    li1.Text = u.NickName;
                    li1.Value = u.ERC20Address;
                    li.Add(li1);
                }
            }
            model.AssignedToList = li;

            if (t.AssignedTo != null)
            model.AssignedToList.FirstOrDefault(c => c.Value == t.AssignedTo).Selected = true;
            return model;
        }

        public IActionResult TicketAdd()
        {
            TicketModel model = GetModel(new Ticket());
            return View(model);
        }

        public IActionResult TicketList()
        {
            ViewBag.TicketList = GetTicketListReport(HttpContext.GetCurrentUser().ERC20Address,String.Empty);
            return View();
        }

    }
}
