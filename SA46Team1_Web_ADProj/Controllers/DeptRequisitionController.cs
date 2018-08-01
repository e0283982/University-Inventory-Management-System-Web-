using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Dept/DeptRequisition")]
    public class DeptRequisitionController : Controller
    {
        [CustomAuthorize(Roles = "Employee Representative, Employee")]
        [Route("NewReq")]
        public ActionResult NewReq()
        {

            Session["DeptReqTabIndex"] = "1";

            using (SSISdbEntities e = new SSISdbEntities()) {
                int SRcount = e.StaffRequisitionHeaders.Count() + 1;
                Session["currentFormId"] = CommonLogic.SerialNo(SRcount, "SR");

                Tuple<Item, StaffRequisitionDetail> tuple = new Tuple<Item, StaffRequisitionDetail>(new Item(), new StaffRequisitionDetail());
                List<String> tempList = (List<String>)Session["tempList"];

                if (tempList.Count==0)
                {
                    ViewBag.ItemsList = new SelectList((from s in e.Items.OrderBy(x => x.Description).ToList()
                                                        select new
                                                        {
                                                            ItemCode = s.ItemCode,
                                                            Description = s.Description + " (" + s.UoM + ")"
                                                        }),
                                                    "ItemCode", "Description", null);
                }
                else {

                    List<Item> newItemList = e.Items.Where(x => !tempList.Contains(x.ItemCode)).OrderBy(x => x.Description).ToList();
                    //update ddl to remove items
                    ViewBag.ItemsList = new SelectList((from s in newItemList
                                                            select new
                                                            {
                                                                ItemCode = s.ItemCode,
                                                                Description = s.Description + " (" + s.UoM + ")"
                                                            }),
                                                        "ItemCode", "Description", null);
                }
                

                TempData["RowIndexesToDiscard"] = new List<int>();

                return View(tuple);
            }

        }

        [CustomAuthorize(Roles = "Employee Representative, Employee")]
        [Route("BackOrders")]
        public ActionResult BackOrders()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Employee Representative, Employee")]
        [HttpPost]
        public RedirectToRouteResult DiscardSelBackorders(string[] itemCodes, string[] formIds)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                DAL.StaffRequisitionDetailsRepositoryImpl dal = new DAL.StaffRequisitionDetailsRepositoryImpl(e);
                DAL.StaffRequisitionRepositoryImpl dalHeader = new DAL.StaffRequisitionRepositoryImpl(e);

                int index = 0;

                foreach (string i in itemCodes) {
                    string formId = formIds[index];

                    //update SRD
                    StaffRequisitionDetail srd = new StaffRequisitionDetail();
                    srd = dal.GetStaffRequisitionDetailById(formId, itemCodes[index]);
                    srd.CancelledBackOrdered = srd.QuantityBackOrdered;
                    srd.QuantityBackOrdered = 0;
                    dal.UpdateStaffRequisitionDetail(srd);

                    //update SRH
                    StaffRequisitionHeader srh = new StaffRequisitionHeader();
                    srh = dalHeader.GetStaffRequisitionHeaderById(formId);
                    string stockRetrievalId = e.StockRetrievalReqForms.Where(x => x.ReqFormID == formId).Select(x=>x.StockRetrievalID).FirstOrDefault();
                    byte? disbursedStatus = e.StockRetrievalHeaders.Where(x => x.ID == stockRetrievalId).Select(x => x.Disbursed).FirstOrDefault();
                    bool backOrderStatus = false;
                    List<StaffRequisitionDetail> reqDetailsList = e.StaffRequisitionDetails.Where(x => x.FormID == formId).ToList();
                    foreach (StaffRequisitionDetail detail in reqDetailsList) {
                        if (detail.QuantityBackOrdered > 0) {
                            backOrderStatus = true;
                        }
                    }

                    switch (backOrderStatus) {
                        case true: //backorder exists for current SR
                            srh.Status = disbursedStatus == 1 ? "Outstanding" : "Open";
                            break;
                        case false:
                            srh.Status = "Cancelled";
                            break;
                    }

                    dalHeader.UpdateStaffRequisitionHeader(srh);

                    index++;
                    e.SaveChanges();
                }

            }

            Session["DeptReqTabIndex"] = "2";
            return RedirectToAction("Requisition", "Dept");
        }

        [CustomAuthorize(Roles = "Employee Representative, Employee")]
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
                srd.QuantityBackOrdered = item2.QuantityOrdered;
                srd.CancelledBackOrdered = 0;
                
                srd.Item = e.Items.Where(x => x.ItemCode == itemToAdd.ItemCode).FirstOrDefault();

                srd.Item.Description = itemToAdd.Description;
                srd.Item.UoM = itemToAdd.UoM;

                list.Add(srd);
                Session["newReqList"] = list;

                //add to list meant for already added items
                List<String> tempList = (List<String>)Session["tempList"];
                tempList.Add(itemToAdd.ItemCode);
                Session["tempList"] = tempList;

                List<Item> newItemList = new List<Item>();
              


                return RedirectToAction("Requisition", "Dept");
            }
        }

        [CustomAuthorize(Roles = "Employee Representative, Employee")]
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
                srh.EmployeeID = Session["LoginEmployeeID"].ToString();
                srh.DateRequested = System.DateTime.Now;
                srh.Status = "Open"; 
                srh.ApprovalStatus = "Pending"; 
                srh.DateProcessed = null; //to change to null (default)
                srh.Approver = e.Employees.Where(x => x.EmployeeID == srh.EmployeeID).Select(x => x.ReportsTo).FirstOrDefault();
                srh.NotificationStatus = "Unread";

                dalHeader.InsertStaffRequisitionHeader(srh);

                foreach (StaffRequisitionDetail srd in (List<StaffRequisitionDetail>)Session["newReqList"]) {
                    srd.Item = null;
                    dalDetails.InsertStaffRequisitionDetail(srd);
                }

                e.SaveChanges();
                Session["newReqList"] = new List<StaffRequisitionDetail>();
                
                int noUnreadRequests = (int)Session["NoUnreadRequests"];
                noUnreadRequests++;
                Session["NoUnreadRequests"] = noUnreadRequests;
            }

            Session["tempList"] = new List<String>();

            return RedirectToAction("Requisition", "Dept");
        }

        [CustomAuthorize(Roles = "Employee Representative, Employee")]
        [HttpPost]
        public RedirectToRouteResult DiscardNewItems(string data, int index)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                //data is item desc, index is list index
                List<StaffRequisitionDetail> list = (List<StaffRequisitionDetail>)Session["newReqList"];
                list.RemoveAt(index);
                Session["newReqList"] = list;

                //remove from list meant for already added items
                List<String> tempList = (List<String>)Session["tempList"];
                string itemCode = e.Items.Where(x => x.Description == data).Select(x => x.ItemCode).FirstOrDefault();
                tempList.Remove(itemCode);
                Session["tempList"] = tempList;

                return RedirectToAction("Requisition", "Dept");
            }
        }

        [CustomAuthorize(Roles = "Employee Representative, Employee")]
        [HttpPost]
        public RedirectToRouteResult EditNewOrderQty()
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                Session["newReqEditMode"] = true;

                return RedirectToAction("Requisition", "Dept");
            }
        }

        [CustomAuthorize(Roles = "Employee Representative, Employee")]
        [HttpPost]
        public RedirectToRouteResult ExitEditNewOrderQty(object[] arr, string[] arr1)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                //update temp list
                List<StaffRequisitionDetail> list = (List<StaffRequisitionDetail>)Session["newReqList"];
                foreach (StaffRequisitionDetail srd in list.ToList()) {
                    StaffRequisitionDetail srdTemp = srd;
                    srdTemp.QuantityOrdered = Int32.Parse(arr1[list.IndexOf(srd)]);
                    list[list.IndexOf(srd)] = srd;
                }

                Session["newReqList"] = list;
                Session["newReqEditMode"] = false;
                
                return RedirectToAction("Requisition", "Dept");
            }
        }

        [HttpPost]
        public RedirectToRouteResult ClearNewReqItems()
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                Session["newReqEditMode"] = false;

                //clear temp list
                List<StaffRequisitionDetail> list = (List<StaffRequisitionDetail>)Session["newReqList"];
                list.Clear();
                Session["newReqList"] = list;

                //remove from list meant for already added items
                List<String> tempList = (List<String>)Session["tempList"];
                tempList.Clear();
                Session["tempList"] = tempList;

                return RedirectToAction("Requisition", "Dept");
            }
        }

        [CustomAuthorize(Roles = "Employee Representative")]
        [Route("UpcomingDelivery")]
        public ActionResult UpcomingDelivery()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Employee Representative")]
        [Route("CollectionList")]
        public ActionResult CollectionList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                string deptCode = Session["DepartmentCode"].ToString();
                m.Configuration.ProxyCreationEnabled = false;
                List<DateTime> collectionDates = m.DisbursementHeaders.Where(x => x.DepartmentCode == deptCode && x.Status == "Open").Select(x => x.Date).ToList();
                DateTime displayedCollectionDate = new DateTime();
                
                foreach (DateTime dt in collectionDates)
                {
                    if (dt > displayedCollectionDate)
                    {
                        displayedCollectionDate = dt;
                    }
                }

                if (displayedCollectionDate == new DateTime())
                {
                    //all open disbursements belonging to dept are alr past
                }
                else {
                    List<DisbursementDetail> disbursementDetails = m.DisbursementDetails.Where(x => x.DisbursementHeader.DepartmentCode == deptCode && x.DisbursementHeader.Status == "Open").ToList();
                    ViewBag.CollectionPointDesc = m.DepartmentDetails.Where(x => x.DepartmentCode == deptCode).Select(x => x.CollectionPoint.CollectionPointDescription).FirstOrDefault();
                    ViewBag.CollectionTime = m.DepartmentDetails.Where(x => x.DepartmentCode == deptCode).Select(x => x.CollectionPoint.CollectionTime).FirstOrDefault();
                    ViewBag.ExpectedDelivery = displayedCollectionDate;
                }

               
            }


            return View();
        }

    }
}
