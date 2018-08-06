using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

//-----------------------------------------------------------------
//   Author: Ong Wei Ting
//-----------------------------------------------------------------


namespace SA46Team1_Web_ADProj.Controllers
{
    public class DeptController : Controller
    {
        [CustomAuthorize(Roles = "Department Head, Employee Representative, Employee")]
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
                    if(emp.Approver == 1)
                {
                    List<StaffRequisitionHeader> s = srhList = m.StaffRequisitionHeaders
                        .Where(x => x.DepartmentCode == emp.DepartmentCode && x.EmployeeID == emp.EmployeeID
                        && (x.ApprovalStatus == "Approved" || x.ApprovalStatus == "Rejected") && x.NotificationStatus == "Unread").ToList();
                    srhList = m.StaffRequisitionHeaders.Where(x => x.DepartmentCode == emp.DepartmentCode
                    && x.ApprovalStatus == "Pending" && x.NotificationStatus == "Unread").ToList();
                    srhList.AddRange(s);
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
                Session["NoUnreadRequestsEmployee"] = deptUnreadPendingApprovalsCount;
            }

            return View();
        }

        [CustomAuthorize(Roles = "Employee Representative, Employee")]
        [Restrict("Approver")]
        public ActionResult Requisition()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Employee Representative, Employee")]
        [Restrict("Approver")]
        public ActionResult RequisitionHistory()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Department Head, Approver")]
        public ActionResult Approval()
        {
                return View();
        }

        [CustomAuthorize(Roles = "Department Head")]
        public ActionResult Authorisation()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Department Head, Employee Representative, Employee")]
        public ActionResult Notifications()
        {
            return View();
        }

        public ActionResult CollectionPoint()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Department Head")]
        public ActionResult Report()
        {
            return View();
        }
    }
}
