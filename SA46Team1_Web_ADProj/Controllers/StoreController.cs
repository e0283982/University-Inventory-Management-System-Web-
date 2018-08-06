using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using SA46Team1_Web_ADProj.DAL;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class StoreController : Controller
    {
        private ItemsRepositoryImpl itemRepository;

        public StoreController()
        {
            this.itemRepository = new ItemsRepositoryImpl(new SSISdbEntities());
        }

        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Home()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<StockAdjustmentFullDetail> list = new List<StockAdjustmentFullDetail>();
                string role = (string)Session["Role"];
                string empId = (string)Session["LoginEmployeeID"];
                if(role == "Store Clerk")
                {
                    list = m.StockAdjustmentFullDetails.Where(x => (x.Status == "Approved" && x.NotificationStatus == "Unread")
                    || (x.Status == "Rejected" && x.NotificationStatus == "Unread")).ToList();
                }
                else
                if(role == "Store Supervisor")
                {
                    list = m.StockAdjustmentFullDetails.Where(x => x.Status == "Pending" 
                    && x.NotificationStatus == "Unread" && x.Amount < 250).ToList();
                }
                else
                {
                    list = m.StockAdjustmentFullDetails.Where(x => x.Status == "Pending" 
                    && x.NotificationStatus == "Unread" && x.Amount >= 250).ToList();
                }

                int UnreadPendingApprovalsCount = list.Count();
                Session["NoUnreadRequests"] = UnreadPendingApprovalsCount;
            }

            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public JsonResult Search(string search)
        {
            return new JsonResult { Data = itemRepository.GetItemById(search), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Inventory()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Disbursements()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Purchase()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Report()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Store Manager, Store Supervisor")]
        public ActionResult Maintenance()
        {
            return View();
        }

        [Authorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Notifications()
        {
            return View();
        }

        [Authorize(Roles = "Store Manager, Store Supervisor")]
        public ActionResult Approval()
        {
            return View();
        }

        public RedirectToRouteResult Test()
        {
         
            return RedirectToAction("Home", "Store");
        }

    }
}
