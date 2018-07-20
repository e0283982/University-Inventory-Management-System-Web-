using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Dept/DeptRequisition")]
    public class DeptRequisitionController : Controller
    {

        [Route("NewReq")]
        public ActionResult NewReq()
        {
            using (SSISdbEntities e = new SSISdbEntities()) {
                string SRcount = (e.StaffRequisitionHeaders.Count()+1).ToString();
                Session["currentFormId"]= "SR" + SRcount;

                Tuple<Item, StaffRequisitionDetail> tuple = new Tuple<Item, StaffRequisitionDetail>(new Item(), new StaffRequisitionDetail());

                //ViewBag.ItemsList = new SelectList(e.Items.ToList(), "ItemCode", "Description");
                ViewBag.ItemsList = new SelectList((from s in e.Items.ToList()
                                                    select new
                                                    {
                                                        ItemCode = s.ItemCode,
                                                        Description = s.Description + " (" + s.UoM + ")"
                                                    }),
                                                    "ItemCode",
                                                    "Description",
                                                    null);

                TempData["RowIndexesToDiscard"] = new List<int>();

                return View(tuple);
            }

        }

        [Route("BackOrders")]
        public ActionResult BackOrders()
        {
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult DiscardSelBackorders(string[] deleteItemCodes, string[] deleteReqId)
        {
            //Session["DeptReqTabIndex"] = "1";

            using (SSISdbEntities e = new SSISdbEntities())
            {
                DAL.StaffRequisitionDetailsRepositoryImpl dal = new DAL.StaffRequisitionDetailsRepositoryImpl(e);
                int index = 0;

                foreach (string i in deleteItemCodes) {
                    StaffRequisitionDetail srd = new StaffRequisitionDetail();
                    srd=
                        dal.GetStaffRequisitionDetailById(deleteReqId[index], deleteItemCodes[index]);
                    srd.CancelledBackOrdered = srd.QuantityBackOrdered;
                    srd.QuantityBackOrdered = 0;
                    dal.UpdateStaffRequisitionDetail(srd);

                    index++;
                    e.SaveChanges();
                }

            }

            return RedirectToAction("Requisition", "Dept");
        }

        [HttpPost]
        [Route("NewReq/AddNewReqItem")]
        public RedirectToRouteResult AddNewReqItem(Item item1, StaffRequisitionDetail item2)
        {
            Item itemToAdd = new Item();

            using (SSISdbEntities e = new SSISdbEntities())
            {
                string itemCode = Request.Form["SelectItemDesc"].ToString();
                itemToAdd= e.Items.Where(x => x.ItemCode == itemCode).FirstOrDefault();

                List<StaffRequisitionDetail> list = new List<StaffRequisitionDetail>();
                list = (List<StaffRequisitionDetail>)Session["newReqList"];
                StaffRequisitionDetail srd = new StaffRequisitionDetail();
                srd.ItemCode = itemToAdd.ItemCode;
                srd.FormID = Session["currentFormId"].ToString();
                srd.QuantityOrdered = item2.QuantityOrdered;
                srd.QuantityDelivered = 0;
                srd.QuantityBackOrdered = 0;
                //srd.CancelledBackOrdered = 0;

                srd.Item = e.Items.Where(x => x.ItemCode == itemToAdd.ItemCode).FirstOrDefault();

                srd.Item.Description = itemToAdd.Description;
                srd.Item.UoM = itemToAdd.UoM;

                list.Add(srd);
                Session["newReqList"] = list;

                return RedirectToAction("Requisition", "Dept");
            }
        }

        [HttpPost]
        //[Route("NewReq/SubmitNewRequestForm")]
        public RedirectToRouteResult SubmitNewRequestForm()
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                DAL.StaffRequisitionDetailsRepositoryImpl dalDetails = new DAL.StaffRequisitionDetailsRepositoryImpl(e);
                DAL.StaffRequisitionRepositoryImpl dalHeader = new DAL.StaffRequisitionRepositoryImpl(e);

                StaffRequisitionHeader srh = new StaffRequisitionHeader();
                srh.FormID = Session["currentFormId"].ToString();
                srh.DepartmentCode = Session["DepartmentCode"].ToString();
                srh.EmployeeID = Session["UserId"].ToString();
                srh.DateRequested = System.DateTime.Now;
                srh.Status = "Pending"; //to check
                srh.ApprovalStatus = "Pending";
                srh.DateProcessed = System.DateTime.Now;
                srh.Approver = e.Employees.Where(x => x.EmployeeID == srh.EmployeeID).Select(x => x.ReportsTo).FirstOrDefault();

                dalHeader.InsertStaffRequisitionHeader(srh);

                foreach (StaffRequisitionDetail srd in (List<StaffRequisitionDetail>)Session["newReqList"]) {
                    srd.Item = null;
                    dalDetails.InsertStaffRequisitionDetail(srd);
                }

                e.SaveChanges();
                Session["newReqList"] = new List<StaffRequisitionDetail>();
            }

            return RedirectToAction("Requisition", "Dept");
        }

        [HttpPost]
        public RedirectToRouteResult DiscardNewItems(int[] rowIndexToDelete)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                List<StaffRequisitionDetail> list = (List<StaffRequisitionDetail>)Session["newReqList"];
                list.RemoveAll(x=> rowIndexToDelete.Contains(list.IndexOf(x)));
                Session["newReqList"] = list;

                return RedirectToAction("Requisition", "Dept");
            }
        }


    }
}
