using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Dept/DeptNotifications")]
    public class DeptNotificationsController : Controller
    {
        [Route("Read")]
        public ActionResult Read()
        {
            return View("ReadMsgs");
        }

        [Route("Unread")]
        public ActionResult Unread()
        {
            return View("UnreadMsgs");
        }

        [Route("All")]
        public ActionResult All()
        {
            return View("AllMsgs");
        }

        [HttpPost]
        public RedirectToRouteResult MsgClicked(string ReqFormId)
        {
            using (SSISdbEntities e = new SSISdbEntities()) {
                DAL.StaffRequisitionRepositoryImpl dal = new DAL.StaffRequisitionRepositoryImpl(e);
                StaffRequisitionHeader srh = e.StaffRequisitionHeaders.Where(x => x.FormID == ReqFormId).FirstOrDefault();
                srh.NotificationStatus = "Read";

                dal.UpdateStaffRequisitionHeader(srh);
                e.SaveChanges();
            }

            return RedirectToAction("Approval", "Dept");
        }
    }
}
