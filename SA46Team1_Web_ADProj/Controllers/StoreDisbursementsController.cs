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

                //To check whether all items have been retrieved
                List<StockRetrievalDetail> stockRetrievalDetailsList = m.StockRetrievalDetails.Where(x => x.Id == reqId).ToList<StockRetrievalDetail>();
                bool allItemCollected = true;

                foreach (StockRetrievalDetail srd in stockRetrievalDetailsList)
                {
                    if(srd.Collected == 0)
                    {
                        allItemCollected = false;
                    }
                }

                StockRetrievalHeader stockRetrievalHeader = m.StockRetrievalHeaders.Where(x => x.ID == reqId).FirstOrDefault();
                if (allItemCollected)
                {
                    stockRetrievalHeader.AllItemsRetrieved = 1;
                    m.SaveChanges();
                }
                else
                {
                    stockRetrievalHeader.AllItemsRetrieved = 0;
                    m.SaveChanges();
                }

                ViewBag.AllItemsRetrieved = stockRetrievalHeader.AllItemsRetrieved;

                

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

                //Create distinct disbursement headers
                List<String> disbHeaderDeptCodes = new List<String>();               

                //Take from stock retrieval details
                List<StockRetrievalDetail> stockRetrievalDetails = m.StockRetrievalDetails.Where(x => x.Id == id).ToList<StockRetrievalDetail>();

                foreach (StockRetrievalDetail srd in stockRetrievalDetails)
                {
                    //To take care that in case stock adjustment makes the quantity retrieved to be 0
                    if(srd.QuantityRetrieved > 0)
                    {
                        String deptCode = m.DepartmentDetails.Where(x => x.CollectionPointID == srd.CollectionPointID).FirstOrDefault().DepartmentCode;
                        disbHeaderDeptCodes.Add(deptCode);
                    }
                }

                //Make it distinct so that only one disbursement header is created
                disbHeaderDeptCodes = disbHeaderDeptCodes.Distinct().ToList();

                foreach (String deptCode in disbHeaderDeptCodes)
                {
                    DisbursementHeader newDH = new DisbursementHeader();

                    int count = m.DisbursementHeaders.Count() + 1;
                    string disId = CommonLogic.SerialNo(count, "DH");
                    newDH.Id = disId;
                    newDH.Status = "Open";
                    newDH.Date = DateTime.Now;
                    newDH.Amount = 0; //Put 0 first and then to be calculated after all the disbursement details is done
                    newDH.StockRetrievalId = id;
                    newDH.DepartmentCode = deptCode;
                    newDH.CollectionPointID = m.DepartmentDetails.Where(x => x.DepartmentCode == deptCode).FirstOrDefault().CollectionPointID;
                    newDH.RepresentativeID = m.DepartmentDetails.Where(x => x.DepartmentCode == newDH.DepartmentCode).FirstOrDefault().RepresentativeID;

                    float totalAmount = 0f;

                    //Create disbursement details, since one collection point is for one dept, then the entire stock retrieved would be assigned to that dept
                    foreach (StockRetrievalDetail srd in stockRetrievalDetails)
                    {
                        String deptCodeDH = m.DepartmentDetails.Where(x => x.CollectionPointID == srd.CollectionPointID).Select(x => x.DepartmentCode).FirstOrDefault();

                        //Only disbursed if quantity retrieved is more than 0
                        if(srd.QuantityRetrieved > 0 && deptCodeDH.Equals(newDH.DepartmentCode))
                        {
                            DisbursementDetail newDD = new DisbursementDetail();
                            newDD.Id = disId;
                            newDD.ItemCode = srd.ItemCode;

                            int quantityOrdered = 0;

                            foreach(String reqF in reqFormIDList)
                            {
                                StaffRequisitionDetail staffReqDet = m.StaffRequisitionDetails.Where(x => x.FormID == reqF && x.ItemCode == newDD.ItemCode).FirstOrDefault();

                                int reqQtyOrdered = 0;

                                if (staffReqDet != null)
                                {
                                    reqQtyOrdered = staffReqDet.QuantityOrdered;
                                }
                                
                                quantityOrdered = quantityOrdered + reqQtyOrdered;
                            }


                            newDD.QuantityOrdered = quantityOrdered;
                            newDD.QuantityReceived = srd.QuantityRetrieved;

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
