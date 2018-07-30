using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [CustomAuthorize(Roles = "Department Head, Employee Representative, Employee")]
    public class DeptController : Controller
    {
        public ActionResult Home()
        {
            string deptCode = Session["DepartmentCode"].ToString();
            //initialise number of pending approvals belonging to dept here
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<StaffRequisitionHeader> list = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == deptCode && x.ApprovalStatus != "Approved" && x.NotificationStatus == "Unread").OrderBy(x => x.FormID).ToList<StaffRequisitionHeader>();
                int deptUnreadPendingApprovalsCount = list.Count();

                Session["NoUnreadRequests"] = deptUnreadPendingApprovalsCount;
            }

            return View();
        }

        public ActionResult Requisition()
        {
            return View();
        }

        public ActionResult RequisitionHistory()
        {
            return View();
        }

        public ActionResult Approval()
        {
            return View();
        }
    
        public ActionResult Authorisation()
        {
            return View();
        }

        public ActionResult Notifications()
        {
            return View();
        }

        public ActionResult CollectionPoint()
        {
            return View();
        }

        public ActionResult Report()
        {
            return View();
        }
    }
}
