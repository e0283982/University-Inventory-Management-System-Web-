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
        public RedirectToRouteResult MsgClicked(string ReqFormId, string ItemCode)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                StockAdjustmentDetail sad = e.StockAdjustmentDetails
                    .Where(x => x.RequestId == ReqFormId && x.ItemCode == ItemCode).FirstOrDefault();
                if (sad.NotificationStatus == "Unread")
                {
                    int noUnreadRequests = (int)Session["NoUnreadRequests"];
                    noUnreadRequests--;
                    Session["NoUnreadRequests"] = noUnreadRequests;
                }

                sad.NotificationStatus = "Read";
                e.SaveChanges();
            }
            Session["StoreInventoryTabIndex"] = 3;
            return RedirectToAction("StoreInventory", "StockAdj");
        }
    }
}