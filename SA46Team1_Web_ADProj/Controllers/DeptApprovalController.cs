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

        //[HttpPost]
        public RedirectToRouteResult BackToApprovalList()
        {
            Session["ReqApprovalPage"] = "1";
            return RedirectToAction("Approval", "Dept");
        }

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

                dal.UpdateStaffRequisitionHeader(srh);
                e.SaveChanges();

                if (srh.NotificationStatus == "Unread")
                {
                    int noUnreadRequests = (int)Session["NoUnreadRequests"];
                    noUnreadRequests--;
                    Session["NoUnreadRequests"] = noUnreadRequests;
                }

            }
            return RedirectToAction("Approval", "Dept");
        }

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

                dal.UpdateStaffRequisitionHeader(srh);
                e.SaveChanges();

                if (srh.NotificationStatus == "Unread")
                {
                    int noUnreadRequests = (int)Session["NoUnreadRequests"];
                    noUnreadRequests--;
                    Session["NoUnreadRequests"] = noUnreadRequests;
                }

            }
            return RedirectToAction("Approval", "Dept");
        }
    }
}
