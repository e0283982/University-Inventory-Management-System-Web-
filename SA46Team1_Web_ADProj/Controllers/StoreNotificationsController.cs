using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreNotifications")]
    public class StoreNotificationsController : Controller
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
            using (SSISdbEntities e = new SSISdbEntities())
            {
                DAL.StaffRequisitionRepositoryImpl dal = new DAL.StaffRequisitionRepositoryImpl(e);
                StaffRequisitionHeader srh = e.StaffRequisitionHeaders.Where(x => x.FormID == ReqFormId).FirstOrDefault();

                if (srh.NotificationStatus == "Unread")
                {
                    int noUnreadRequests = (int)Session["NoUnreadRequests"];
                    noUnreadRequests--;
                    Session["NoUnreadRequests"] = noUnreadRequests;
                }

                srh.NotificationStatus = "Read";

                dal.UpdateStaffRequisitionHeader(srh);
                e.SaveChanges();
            }

            return RedirectToAction("Approval", "Dept");
        }
    }
}