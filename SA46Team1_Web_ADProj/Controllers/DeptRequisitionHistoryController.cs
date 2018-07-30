using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [CustomAuthorize(Roles = "Employee Representative, Employee")]
    [RoutePrefix("Dept/DeptRequisitionHistory")]
    public class DeptRequisitionHistoryController : Controller
    {
        [Route("Overview")]
        public ActionResult Overview()
        {
            if (Session["ReqHistoryPage"].ToString() == "1")
            {
                Session["existingReqEditMode"] = false;
                return View("Overview");
            }
            else
            {
                Session["ReqHistoryPage"] = "1";
                return View("Overview2");
            }
        }

        [HttpPost]
        public ActionResult DisplayReqHistoryDetails(FormCollection data)
        {
            string deptCode = Session["DepartmentCode"].ToString();
            string formId = data["FormID"];
            ReqHistoryModel model;

            using (SSISdbEntities e = new SSISdbEntities()) {
                StaffRequisitionHeader srh = e.StaffRequisitionHeaders.Where(x=>x.FormID==formId).FirstOrDefault();

                string repId = e.DepartmentDetails.Where(x => x.DepartmentCode == deptCode).Select(x => x.RepresentativeID).FirstOrDefault();
                string repName = e.Employees.Where(x => x.EmployeeID == repId).Select(x => x.EmployeeName).FirstOrDefault();
                string approverName = e.Employees.Where(x => x.EmployeeID == srh.Approver).Select(x => x.EmployeeName).FirstOrDefault();
                string approvalStatus = srh.ApprovalStatus;
                string status = srh.Status;
                DateTime requestDate = srh.DateRequested;

                model = new ReqHistoryModel();
                model.ApprovalStatus = approvalStatus;
                model.ApproverName = approverName;
                model.RepName = repName;
                model.RequestDate = requestDate;
                model.Status = status;

                Session["CurrentReqHistory"] = model;
            }
            

            Session["ReqHistoryPage"] = "2";
            Session["id"] = data["FormID"];
            return RedirectToAction("RequisitionHistory", "Dept");
        }

        [HttpPost]
        public ActionResult DisplayReqHistoryDetails2(string formId)
        {
            TempData["goBackToDelivery"] = true;
            Session["DeptReqTabIndex"] = "3";

            string deptCode = Session["DepartmentCode"].ToString();
            ReqHistoryModel model;

            using (SSISdbEntities e = new SSISdbEntities())
            {
                StaffRequisitionHeader srh = e.StaffRequisitionHeaders.Where(x => x.FormID == formId).FirstOrDefault();

                string repId = e.DepartmentDetails.Where(x => x.DepartmentCode == deptCode).Select(x => x.RepresentativeID).FirstOrDefault();
                string repName = e.Employees.Where(x => x.EmployeeID == repId).Select(x => x.EmployeeName).FirstOrDefault();
                string approverName = e.Employees.Where(x => x.EmployeeID == srh.Approver).Select(x => x.EmployeeName).FirstOrDefault();
                string approvalStatus = srh.ApprovalStatus;
                DateTime requestDate = srh.DateRequested;

                model = new ReqHistoryModel();
                model.ApprovalStatus = approvalStatus;
                model.ApproverName = approverName;
                model.RepName = repName;
                model.RequestDate = requestDate;

                Session["CurrentReqHistory"] = model;
            }


            Session["ReqHistoryPage"] = "2";
            Session["id"] = formId;
            return RedirectToAction("RequisitionHistory", "Dept");
        }


        [HttpPost]
        public RedirectToRouteResult BackToReqHistoryList()
        {
            Session["ReqHistoryPage"] = "1";
            return RedirectToAction("RequisitionHistory", "Dept");
        }

        [HttpPost]
        public RedirectToRouteResult EditExisitingOrderQty()
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                Session["existingReqEditMode"] = true;
                Session["ReqHistoryPage"] = "2";

                return RedirectToAction("RequisitionHistory", "Dept");
            }
        }

        [HttpPost]
        public RedirectToRouteResult ExitEditExistingOrderQty(object[] arr, string[] arr1)
        {
            string formId = Session["id"].ToString();
            
            Session["existingReqEditMode"] = false;

            //update details of current req history
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<RequisitionHistoryDetail> reqDetailsList = m.RequisitionHistoryDetails.Where(x => x.FormID == formId).ToList<RequisitionHistoryDetail>();
                DAL.StaffRequisitionDetailsRepositoryImpl dal = new DAL.StaffRequisitionDetailsRepositoryImpl(m);
                StaffRequisitionDetail srd = new StaffRequisitionDetail();

                foreach (RequisitionHistoryDetail rhd in reqDetailsList)
                {
                    string itemCode = m.Items.Where(x => x.Description == rhd.Description).Select(x => x.ItemCode).FirstOrDefault();
                    srd = m.StaffRequisitionDetails.Where(x => x.FormID == formId && x.ItemCode == itemCode).FirstOrDefault();

                    int index = reqDetailsList.IndexOf(rhd);
                    srd.QuantityOrdered = Int32.Parse(arr1[index]);

                    dal.UpdateStaffRequisitionDetail(srd);
                }

                m.SaveChanges();

            }

            Session["ReqHistoryPage"] = "2";
            
            return RedirectToAction("RequisitionHistory", "Dept");
            
        }

        [HttpPost]
        public RedirectToRouteResult DiscardSelReqItems(string data, int index)
        {
            string formId = Session["id"].ToString();

            Session["existingReqEditMode"] = false;

            //update details of current req history
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                DAL.StaffRequisitionDetailsRepositoryImpl dal = new DAL.StaffRequisitionDetailsRepositoryImpl(m);
                string itemCode = m.Items.Where(x => x.Description == data).Select(x => x.ItemCode).FirstOrDefault();
                dal.DeleteStaffRequisitionDetail(formId,itemCode);

                int noOfItemsInRequest = m.StaffRequisitionDetails.Where(x => x.FormID == formId).ToList().Count();

                if (noOfItemsInRequest == 1) {
                    DAL.StaffRequisitionRepositoryImpl dalHeader = new DAL.StaffRequisitionRepositoryImpl(m);
                    StaffRequisitionHeader srh = m.StaffRequisitionHeaders.Where(x => x.FormID == formId).FirstOrDefault();
                    srh.Status = "Withdrawn"; //to add in list of constants
                    dalHeader.UpdateStaffRequisitionHeader(srh);

                    if (srh.NotificationStatus == "Unread") {
                        int noUnreadRequests = (int)Session["NoUnreadRequests"];
                        noUnreadRequests--;
                        Session["NoUnreadRequests"] = noUnreadRequests;
                    }
                    
                }

                m.SaveChanges();

            }

            Session["ReqHistoryPage"] = "2";

            return RedirectToAction("RequisitionHistory", "Dept");

        }
    }
}
