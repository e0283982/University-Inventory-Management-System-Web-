using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [CustomAuthorize(Roles = "Store Manager, Store Supervisor, Store Clerk")]
    [RoutePrefix("Store/StoreDisbursements")]
    public class StoreDisbursementsController : Controller //Tabs: Requisition, Retrieval, Disbursement
    {
        /************Action methods belonging to Store Disbursements - Disbursement*******************/
        [Route("Disbursement")]
        public ActionResult Disbursement()
        {
            Session["StoreDisbursementTabIndex"] = "3";

            if (Session["DisbursementListPage"].ToString() == "1")
            {
                return View("Disbursement");
            }
            else
            {
                Session["DisbursementListPage"] = "1";
                return View("Disbursement2");
            }
        }

        [HttpPost]
        public ActionResult DisplayDisbursementDetails(string storeDisbursementFormId)
        {
            Session["DisbursementListPage"] = "2";
            Session["storeDisbursementFormId"] = storeDisbursementFormId;

            return RedirectToAction("Disbursements", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToDisbursementList()
        {
            Session["DisbursementListPage"] = "1";

            return RedirectToAction("Disbursements", "Store");
        }

        /************Action methods belonging to Store Disbursements - Requisition *******************/

        [Route("Requisition")]
        public ActionResult Requisition()
        {
            Session["StoreDisbursementTabIndex"] = "1";

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
        public ActionResult DisplayReqDetails(string storeReqFormId)
        {
            Session["ReqListPage"] = "2";
            Session["storeReqFormId"] = storeReqFormId;

            return RedirectToAction("Disbursements", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToRequisitionsList()
        {
            Session["ReqListPage"] = "1";

            return RedirectToAction("Disbursements", "Store");
        }

        /************Action methods belonging to Store Disbursements - Retrieval*******************/


        [Route("Retrieval")]
        public ActionResult Retrieval()
        {
            Session["StoreDisbursementTabIndex"] = "2";
            
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                int id = m.StockRetrievalHeaders.Count();
                string reqId = CommonLogic.SerialNo(id, "StoR");
                Session["RetrievalId"] = reqId;
                ViewBag.IdCount = reqId;
                ViewBag.Disbursed = m.StockRetrievalHeaders.Where(x => x.ID == reqId).First().Disbursed;                
            }

            var tuple = new Tuple<StockRetrievalDetail, Item>(new StockRetrievalDetail(), new Item());

            return View(tuple);
        }

        [HttpPost]
        public RedirectToRouteResult DisburseItems()
        {
            
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                string id = (string) Session["RetrievalId"];
                StockRetrievalHeader srh = m.StockRetrievalHeaders.Where(x => x.ID == id).First();
                srh.Disbursed = 1;
                
                List<StockRetrievalDetail> itemsRetrieved = m.StockRetrievalDetails.Where(x => x.Id == id).ToList();

                bool stockAdjustmentHeaderCreated = false;

                foreach (StockRetrievalDetail srd in itemsRetrieved)
                {
                    DateTime localDate = DateTime.Now;

                    if (srd.QuantityAdjusted > 0)
                    {
                        //To check whether there is stock adjustment header for the item
                        if (!stockAdjustmentHeaderCreated)
                        {
                            //To Create Stock Adjustment Header
                            stockAdjustmentHeaderCreated = true;

                            StockAdjustmentHeader sah = new StockAdjustmentHeader();
                            int stockAdjustmentHeaderId = m.StockAdjustmentHeaders.Count() + 1;
                            sah.RequestId = CommonLogic.SerialNo(stockAdjustmentHeaderId, "SA");
                            
                            sah.DateRequested = localDate;

                            //TODO, Temporary put the requestor as E1
                            sah.Requestor = (string) Session["LoginEmployeeID"];                            

                            sah.TransactionType = "Stock Adjustment";

                            m.StockAdjustmentHeaders.Add(sah);
                            m.SaveChanges();                            
                        }

                        //To Create Stock Adjustment Details
                        int stockAdjustmentDetailId = m.StockAdjustmentHeaders.Count();
                        StockAdjustmentDetail sad = new StockAdjustmentDetail();
                        sad.RequestId = CommonLogic.SerialNo(stockAdjustmentDetailId, "SA");
                        sad.ItemCode = srd.ItemCode;
                        sad.ItemQuantity = srd.QuantityAdjusted;

                        float itemUnitCost = m.Items.Where(x => x.ItemCode == sad.ItemCode).Select(x => x.AvgUnitCost).FirstOrDefault();
                        sad.Amount = itemUnitCost * sad.ItemQuantity;

                        sad.Remarks = srd.Remarks;
                        sad.Status = "Pending";

                        m.StockAdjustmentDetails.Add(sad);                        

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

                //To order by id so the earlier id will mean that the req form was submitted earlier
                List<String> reqFormIDList = m.StockRetrievalReqForms.OrderBy(x => x.Id).Where(x => x.StockRetrievalID == id).Select(x => x.ReqFormID).ToList<String>();

                //List<String> reqFormIDList = m.StockRetrievalReqForms.Where(x => x.StockRetrievalID == id).Select(x => x.ReqFormID).ToList<String>();

                foreach (String reqFormID in reqFormIDList)
                {
                    DisbursementHeader newDH = new DisbursementHeader();

                    int count = m.DisbursementHeaders.Count() + 1;
                    string disId = CommonLogic.SerialNo(count, "DH");
                    newDH.Id = disId;
                    newDH.Status = "Open";
                    
                    //------------------------------- TO CHANGE ----------------------------
                    //To change to stock retrieval id(local variable id) instead of staff req id
                    newDH.StockRetrievalId = id;
                    
                    newDH.DepartmentCode = m.StaffRequisitionHeaders.Where(x => x.FormID == reqFormID).FirstOrDefault().DepartmentCode;                    
                    newDH.CollectionPointID = m.DepartmentDetails.Where(x => x.DepartmentCode == newDH.DepartmentCode).FirstOrDefault().CollectionPointID;
                    newDH.RepresentativeID = m.DepartmentDetails.Where(x => x.DepartmentCode == newDH.DepartmentCode).FirstOrDefault().RepresentativeID;

                    float totalAmount = 0f;

                    //To create disbursement details, case of no adjustment first

                    List<StaffRequisitionDetail> staffRequisitionDetailsList = m.StaffRequisitionDetails.Where(x => x.FormID == reqFormID).ToList<StaffRequisitionDetail>();

                    foreach(StaffRequisitionDetail srd in staffRequisitionDetailsList)
                    {
                        
                        Item itemRequested = m.Items.Where(x => x.ItemCode == srd.ItemCode).FirstOrDefault();

                        //Unable to fulfill any item
                        if(itemRequested.Quantity <= 0)
                        {
                            srd.QuantityDelivered = 0;
                            srd.QuantityBackOrdered = srd.QuantityOrdered;

                        }
                        else
                        {
                           
                            DisbursementDetail newDD = new DisbursementDetail();
                            newDD.Id = disId;
                            newDD.ItemCode = srd.ItemCode;
                            newDD.QuantityOrdered = srd.QuantityOrdered;

                            //QuantityReceived will be the same as Quantity Ordered as item table will be able to fulfill all the items
                            if (itemRequested.Quantity > srd.QuantityOrdered)
                            {
                                newDD.QuantityReceived = srd.QuantityOrdered;
                                srd.QuantityDelivered = newDD.QuantityReceived;
                            }
                            else
                            {
                                //QuantityReceived will be the same as item table quantity as that is all that is left
                                newDD.QuantityReceived = itemRequested.Quantity;
                                srd.QuantityDelivered = newDD.QuantityReceived;

                                //There would be quantity backordered in this case
                                srd.QuantityBackOrdered = srd.QuantityOrdered - srd.QuantityDelivered;
                            }                           

                            float itemUnitCost = m.Items.Where(x => x.ItemCode == newDD.ItemCode).Select(x => x.AvgUnitCost).FirstOrDefault();
                            newDD.UnitCost = itemUnitCost;

                            newDD.UoM = m.Items.Where(x => x.ItemCode == newDD.ItemCode).Select(x => x.UoM).FirstOrDefault();
                            newDD.QuantityAdjusted = 0;
                            newDD.TransactionType = "Disbursement";

                            float amount = itemUnitCost * newDD.QuantityReceived;
                            totalAmount += amount;

                            m.DisbursementDetails.Add(newDD);

                            //To add the item transactions
                            DateTime localDate = DateTime.Now;
                            newDH.Date = localDate;

                            ItemTransaction itemTransaction = new ItemTransaction();
                            itemTransaction.TransDateTime = localDate;
                            itemTransaction.DocumentRefNo = newDD.Id;
                            itemTransaction.ItemCode = newDD.ItemCode;
                            itemTransaction.TransactionType = "Stock Disbursement";
                            itemTransaction.Quantity = newDD.QuantityReceived;
                            itemTransaction.UnitCost = itemUnitCost;
                            itemTransaction.Amount = newDD.QuantityReceived * itemUnitCost;

                            m.ItemTransactions.Add(itemTransaction);

                            //To update the quantity of the item table
                            Item itemDisbursed = m.Items.Where(x => x.ItemCode == itemTransaction.ItemCode).FirstOrDefault();
                            itemDisbursed.Quantity -= itemTransaction.Quantity;

                            
                        }                       

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

            string itemCode;

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                itemCode = m.Items.Where(x => x.Description == item2.Description).FirstOrDefault().ItemCode;

                StockRetrievalDetail srd = m.StockRetrievalDetails.Where(x => x.ItemCode == itemCode && x.Id == item1.Id).FirstOrDefault();

                int qtyAdjusted = item1.QuantityAdjusted;

                srd.QuantityAdjusted = srd.QuantityAdjusted + qtyAdjusted;
                srd.QuantityRetrieved = srd.QuantityRetrieved - qtyAdjusted;

                srd.Remarks = item1.Remarks;

                m.SaveChanges();

            }

            return RedirectToAction("Disbursements", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult UpdateItemCollection(int bin, string collectionPointDescription)
        {
            string retId = (String) Session["RetrievalId"];

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;

                string collectionPointId = m.CollectionPoints.Where(x => x.CollectionPointDescription == collectionPointDescription).Select(x => x.CollectionPointID).FirstOrDefault();

                StockRetrievalDetail stockRetrievalDetail = m.StockRetrievalDetails.Where(x => x.Id == retId && x.Bin == bin && x.CollectionPointID == collectionPointId).FirstOrDefault();

                if(stockRetrievalDetail.Collected == 0)
                {
                    stockRetrievalDetail.Collected = 1;
                }
                else
                {
                    stockRetrievalDetail.Collected = 0;
                }

                m.SaveChanges();
            }


            return RedirectToAction("Disbursements", "Store");
        }



    }
}
