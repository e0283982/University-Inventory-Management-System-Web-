using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using SA46Team1_Web_ADProj.Models;
namespace SA46Team1_Web_ADProj.Controllers
{

    [RoutePrefix("Dept/DeptApproval")]
    public class DeptApprovalController : Controller
    {
        [CustomAuthorize(Roles = "Department Head")]
        [Route("Approval")]
        public ActionResult Approval()
        {
            if (Session["ReqApprovalPage"].ToString() == "1")
            {
                return View("Approval");
            }
            else
            {
                Session["ReqApprovalPage"] = "1";
                return View("Approval2");
            }
        }

        [CustomAuthorize(Roles = "Department Head")]
        [HttpPost]
        //[Route("DisplayApprovalDetails")]
        public RedirectToRouteResult DisplayApprovalDetails(string ReqFormId)
        {
            Session["ReqApprovalPage"] = "2";
            Session["ReviewNewRequisitionId"] = ReqFormId;

            using (SSISdbEntities e = new SSISdbEntities())
            {
                DAL.StaffRequisitionRepositoryImpl dal = new DAL.StaffRequisitionRepositoryImpl(e);
                StaffRequisitionHeader srh = dal.GetStaffRequisitionHeaderById(ReqFormId);
                TempData["RequisitionRequstor"] = e.Employees.Where(x => x.EmployeeID == srh.EmployeeID).Select(x => x.EmployeeName).First();
                TempData["RequisitionDateReq"] = srh.DateRequested;
            }

            return RedirectToAction("Approval", "Dept");
        }

        [CustomAuthorize(Roles = "Department Head")]
        //[HttpPost]
        public RedirectToRouteResult BackToApprovalList()
        {
            Session["ReqApprovalPage"] = "1";
            return RedirectToAction("Approval", "Dept");
        }

        [CustomAuthorize(Roles = "Department Head")]
        [HttpPost]
        public RedirectToRouteResult Approve()
        {
            //update staff req
            using (SSISdbEntities e = new SSISdbEntities()) {
                DAL.StaffRequisitionRepositoryImpl dal = new DAL.StaffRequisitionRepositoryImpl(e);

                StaffRequisitionHeader srh = dal.GetStaffRequisitionHeaderById(Session["ReviewNewRequisitionId"].ToString());
                srh.ApprovalStatus = "Approved";
                srh.Approver = Session["UserId"].ToString();
                srh.DateProcessed = System.DateTime.Now;
                
                if (srh.NotificationStatus == "Unread")
                {
                    int noUnreadRequests = (int)Session["NoUnreadRequests"];
                    noUnreadRequests--;
                    Session["NoUnreadRequests"] = noUnreadRequests;
                    srh.NotificationStatus = "Read";
                }

                dal.UpdateStaffRequisitionHeader(srh);
                e.SaveChanges();

            }
            return RedirectToAction("Approval", "Dept");
        }

        [CustomAuthorize(Roles = "Department Head")]
        [HttpPost]
        public RedirectToRouteResult Reject()
        {
            //update staff req
            using (SSISdbEntities e = new SSISdbEntities())
            {
                DAL.StaffRequisitionRepositoryImpl dal = new DAL.StaffRequisitionRepositoryImpl(e);

                StaffRequisitionHeader srh = dal.GetStaffRequisitionHeaderById(Session["ReviewNewRequisitionId"].ToString());
                srh.ApprovalStatus = "Rejected";
                srh.Approver = Session["UserId"].ToString();
                srh.DateProcessed = System.DateTime.Now;
                srh.Status = "Cancelled";
                
                if (srh.NotificationStatus == "Unread")
                {
                    int noUnreadRequests = (int)Session["NoUnreadRequests"];
                    noUnreadRequests--;
                    Session["NoUnreadRequests"] = noUnreadRequests;
                    srh.NotificationStatus = "Read";
                }

                dal.UpdateStaffRequisitionHeader(srh);
                e.SaveChanges();

            }
            return RedirectToAction("Approval", "Dept");
        }
    }
}
