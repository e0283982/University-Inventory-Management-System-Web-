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
                string id = (string)Session["LoginEmployeeID"];
                m.Configuration.ProxyCreationEnabled = false;
                List<StaffRequisitionHeader> srhList = new List<StaffRequisitionHeader>();
                    Employee emp = m.Employees.Where(x => x.EmployeeID == id).FirstOrDefault();
                    if (emp.Designation == "Department Head")
                    {
                        srhList = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == emp.DepartmentCode
                        && x.ApprovalStatus == "Pending" && x.NotificationStatus == "Unread").ToList();
                    }
                    else
                    {
                        srhList = m.StaffRequisitionHeaders
                            .Where(x => x.DepartmentCode == emp.DepartmentCode && x.EmployeeID == emp.EmployeeID
                            && (x.ApprovalStatus == "Approved" || x.ApprovalStatus == "Rejected") && x.NotificationStatus == "Unread").ToList();
                    }

                    if (srhList == null)
                    {
                        srhList = new List<StaffRequisitionHeader>();
                    }

                int deptUnreadPendingApprovalsCount = srhList.Count();

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
