using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreDisbursements")]
    public class StoreDisbursementsController : Controller
    {
        [Route("Disbursement")]
        public ActionResult Disbursement()
        {
            if (Session["DisbursementListPage"].ToString() == "1")
            {
                @Session["BackToDisbursementList"] = "false";
                return View("Disbursement");
            }
            else
            {
                Session["DisbursementListPage"] = "1";
                return View("Disbursement2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayDisbursementDetails()
        {
            Session["DisbursementListPage"] = "2";
            return RedirectToAction("Disbursements", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToDisbursementList()
        {
            Session["DisbursementListPage"] = "1";
            Session["BackToDisbursementList"] = "true";

            return RedirectToAction("Disbursements", "Store");
        }


        [Route("Requisition")]
        public ActionResult Requisition()
        {
            //Session["DisbursementListPage"] = "0";

            if (Session["ReqListPage"].ToString() == "1")
            {
                return View("Requisition");
            }
            else
            {
                Session["ReqListPage"] = "1";
                return View("Requisition2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayReqDetails()
        {
            Session["ReqListPage"] = "2";
            return RedirectToAction("Disbursements", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToRequisitionsList()
        {
            Session["ReqListPage"] = "1";

            return RedirectToAction("Disbursements", "Store");
        }

        [Route("Retrieval")]
        public ActionResult Retrieval()
        {
            //Use disbursementlistpage equals to 1 to wire the tab
            Session["RetrievalListPage"] = "1";

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                int id = m.StockRetrievalHeaders.Count();
                string reqId = "StoR-" + id;
                Session["RetrievalId"] = "StoR-" + id;
                ViewBag.IdCount = id;
                ViewBag.Disbursed = m.StockRetrievalHeaders.Where(x => x.ID == reqId).First().Disbursed;                
            }

            var tuple = new Tuple<StockRetrievalDetail, Item>(new StockRetrievalDetail(), new Item());

            return View(tuple);
        }

        //New Stuff
        [HttpPost]
        public RedirectToRouteResult DisburseItems()
        {
            Session["RetrievalListPage"] = "2";
            
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                string id = (string) Session["RetrievalId"];
                StockRetrievalHeader srh = m.StockRetrievalHeaders.Where(x => x.ID == id).First();
                srh.Disbursed = 1;

                //To check whether there is stock adjustment for the item
                List<StockRetrievalDetail> itemsRetrieved = m.StockRetrievalDetails.Where(x => x.Id == id).ToList<StockRetrievalDetail>();

                bool stockAdjustmentHeaderCreated = false;

                foreach (StockRetrievalDetail srd in itemsRetrieved)
                {
                    DateTime localDate = DateTime.Now;

                    if (srd.QuantityAdjusted > 0)
                    {
                        
                        if (!stockAdjustmentHeaderCreated)
                        {
                            //To Create Stock Adjustment Header
                            stockAdjustmentHeaderCreated = true;

                            StockAdjustmentHeader sah = new StockAdjustmentHeader();
                            int stockAdjustmentHeaderId = m.StockAdjustmentHeaders.Count() + 1;
                            sah.RequestId = "SA-" + stockAdjustmentHeaderId;
                            
                            sah.DateRequested = localDate;

                            //TODO, Temporary put the requestor as E1
                            sah.Requestor = "E1";

                            //Approver default to Store Supervisor first
                            string supervisorId = m.Employees.Where(x => x.Designation == "Store Supervisor").Select(x => x.EmployeeID).FirstOrDefault();

                            sah.Approver = supervisorId;

                            sah.TransactionType = "Stock Adjustment";

                            m.StockAdjustmentHeaders.Add(sah);
                            m.SaveChanges();                            
                        }

                        //To Create Stock Adjustment Details
                        int stockAdjustmentDetailId = m.StockAdjustmentHeaders.Count();
                        StockAdjustmentDetail sad = new StockAdjustmentDetail();
                        sad.RequestId = "SA-" + stockAdjustmentDetailId;
                        sad.ItemCode = srd.ItemCode;
                        sad.ItemQuantity = srd.QuantityAdjusted;

                        float itemUnitCost = m.Items.Where(x => x.ItemCode == sad.ItemCode).Select(x => x.AvgUnitCost).FirstOrDefault();
                        sad.Amount = itemUnitCost * sad.ItemQuantity;

                        //Temporary, put it as missing first then will update the dialog box to include missing/damaged
                        sad.Remarks = "Damaged";
                        sad.Status = "Pending";

                        m.StockAdjustmentDetails.Add(sad);

                        //To change the approver to manager
                        if(sad.Amount > 250)
                        {
                            StockAdjustmentHeader stockAdjustmentHeader = m.StockAdjustmentHeaders.Where(x => x.RequestId == sad.RequestId).FirstOrDefault();

                            string managerId = m.Employees.Where(x => x.Designation == "Store Manager").Select(x => x.EmployeeID).FirstOrDefault();

                            stockAdjustmentHeader.Approver = managerId;

                            m.SaveChanges();
                        }

                        //To add the item transactions
                        ItemTransaction itemTransaction = new ItemTransaction();
                        itemTransaction.TransDateTime = localDate;
                        itemTransaction.DocumentRefNo = sad.RequestId;
                        itemTransaction.ItemCode = sad.ItemCode;
                        itemTransaction.TransactionType = "Stock Adjustment";
                        itemTransaction.Quantity = sad.ItemQuantity;
                        itemTransaction.UnitCost = itemUnitCost;
                        itemTransaction.Amount = sad.Amount;

                        m.ItemTransactions.Add(itemTransaction);

                        //To update the quantity of the item table
                        Item itemAdjusted = m.Items.Where(x => x.ItemCode == itemTransaction.ItemCode).FirstOrDefault();
                        itemAdjusted.Quantity -= itemTransaction.Quantity;


                        m.SaveChanges();
                    }                    

                }





                //Creating list of new disbursements

                List<String> reqFormIDList = m.StockRetrievalReqForms.Where(x => x.StockRetrievalID == id).Select(x => x.ReqFormID).ToList<String>();

                foreach(String reqFormID in reqFormIDList)
                {
                    DisbursementHeader newDH = new DisbursementHeader();

                    int count = m.DisbursementHeaders.Count() + 1;
                    string disId = "DH-" + count;
                    newDH.Id = disId;

                    newDH.Status = "Open";
                    
                    newDH.RequisitionFormID = reqFormID;

                    DateTime localDate = DateTime.Now;
                    newDH.Date = localDate;
                    newDH.DepartmentCode = m.StaffRequisitionHeaders.Where(x => x.FormID == reqFormID).FirstOrDefault().DepartmentCode;                    
                    newDH.CollectionPointID = m.DepartmentDetails.Where(x => x.DepartmentCode == newDH.DepartmentCode).FirstOrDefault().CollectionPointID;
                    newDH.RepresentativeID = m.DepartmentDetails.Where(x => x.DepartmentCode == newDH.DepartmentCode).FirstOrDefault().RepresentativeID;

                    //Temporary
                    //newDH.Amount = 100;
                    //m.DisbursementHeaders.Add(newDH);

                    float totalAmount = 0f;

                    //To create disbursement details, case of no adjustment first

                    List<StaffRequisitionDetail> staffRequisitionDetailsList = m.StaffRequisitionDetails.Where(x => x.FormID == reqFormID).ToList<StaffRequisitionDetail>();

                    foreach(StaffRequisitionDetail srd in staffRequisitionDetailsList)
                    {
                        DisbursementDetail newDD = new DisbursementDetail();
                        newDD.Id = disId;
                        newDD.ItemCode = srd.ItemCode;
                        newDD.QuantityOrdered = srd.QuantityOrdered;

                        //QuantityReceived will be the same as Quantity Ordered unless there are adjustments
                        newDD.QuantityReceived = srd.QuantityOrdered;

                        float itemUnitCost = m.Items.Where(x => x.ItemCode == newDD.ItemCode).Select(x => x.AvgUnitCost).FirstOrDefault();
                        newDD.UnitCost = itemUnitCost;

                        newDD.UoM = m.Items.Where(x => x.ItemCode == newDD.ItemCode).Select(x => x.UoM).FirstOrDefault();
                        newDD.QuantityAdjusted = 0;
                        newDD.TransactionType = "Disbursement";

                        float amount = itemUnitCost * newDD.QuantityReceived;
                        totalAmount += amount;

                        m.DisbursementDetails.Add(newDD);


                        //TODO: to update the item database


                    }

                    newDH.Amount = totalAmount;
                    m.DisbursementHeaders.Add(newDH);

                    //To update the status of requisition
                    //Status would be outstanding when the item is disbursed
                    //Status would only change to completed when the receiver has acknowledged receipt of the item
                    StaffRequisitionHeader staffRequisitionHeader = m.StaffRequisitionHeaders.Where(x => x.FormID == reqFormID).FirstOrDefault();
                    staffRequisitionHeader.Status = "Outstanding";

                    m.SaveChanges();
                } 
                







            }


            




            return RedirectToAction("Disbursements", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult AdjustItem(StockRetrievalDetail item1, Item item2)
        {
            Session["RetrievalListPage"] = "2";

            string itemCode;

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                itemCode = m.Items.Where(x => x.Description == item2.Description).FirstOrDefault().ItemCode;

                StockRetrievalDetail srd = m.StockRetrievalDetails.Where(x => x.ItemCode == itemCode && x.Id == item1.Id).FirstOrDefault();

                int qtyAdjusted = item1.QuantityAdjusted;

                srd.QuantityAdjusted = srd.QuantityAdjusted + qtyAdjusted;
                srd.QuantityRetrieved = srd.QuantityRetrieved - qtyAdjusted;

                m.SaveChanges();

            }

            return RedirectToAction("Disbursements", "Store");
        }



    }
}
