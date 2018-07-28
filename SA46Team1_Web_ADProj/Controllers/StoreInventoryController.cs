using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.DAL;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreInventory")]
    public class StoreInventoryController : Controller
    {
        [Authorize(Roles = "Store Clerk, Store Manager")]
        [Route("Overview")]
        public ActionResult Overview()
        {
            if (Session["InventoryOverviewPage"].ToString() == "1")
            {
            
                return View("Overview");
            }
            else
            {
                Session["InventoryOverviewPage"] = "1";
                return View("Overview2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayItemDetails(string maintenanceItemCode)
        {
            Session["InventoryOverviewPage"] = "2";
            Session["MaintenanceItemCode"] = maintenanceItemCode;

            return RedirectToAction("Inventory", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToInventoryOverviewList()
        {
            Session["InventoryOverviewPage"] = "1";

            return RedirectToAction("Inventory", "Store");
        }

        [Authorize(Roles = "Store Clerk, Store Manager")]
        [Route("Reorder")]
        public ActionResult Reorder()
        {
            List<ReorderList> reorderLists;
            using (SSISdbEntities m = new SSISdbEntities())
            {
                reorderLists = m.ReorderLists.ToList();
                Session["ReorderList"] = reorderLists;
            }
            return View();
        }

        [Authorize(Roles = "Store Clerk, Store Manager")]
        [HttpPost]
        public ActionResult AddToPO(string[] arr1, string[] arrSupplier)
        {
            List<ReorderList> poDetailsList = (List<ReorderList>)Session["ReorderList"];
            List<Supplier> supplierList = new List<Supplier>();
            List<Item> itemAdded = new List<Item>();
            int arrayCount = 0;
            using (SSISdbEntities m = new SSISdbEntities())
            {
                // Checking for number of suppliers to Iterate & later used for creating no. of PO
                for (int i = 0; i < arrSupplier.Length; i++)
                {
                    string supCode = arrSupplier[i];
                    Supplier supplier = m.Suppliers.Where(x => x.CompanyName == supCode).FirstOrDefault();
                    if (!supplierList.Contains(supplier))
                    {
                        supplierList.Add(supplier);
                    }
                }

                // Change Item Supplier in PODetails for retrieving in list and adding into database later
                foreach (ReorderList p in poDetailsList)
                {
                    string coy = arrSupplier[arrayCount];
                    Supplier sup = m.Suppliers.Where(x => x.CompanyName == coy).FirstOrDefault();
                    p.s1 = sup.CompanyName;
                    arrayCount++;
                }

                // Each Supplier iterates once such that only 1 PO is created for each of them
                foreach (Supplier s in supplierList)
                {
                    // Create new PO based on supplier
                    int count = m.POHeaders.Count() + 1;
                    string poId = "PO-" + count.ToString();

                    POHeader newPOHeader = new POHeader();
                    newPOHeader.PONumber = poId;
                    newPOHeader.Date = DateTime.Now;
                    newPOHeader.SupplierCode = s.SupplierCode;
                    newPOHeader.ContactName = s.ContactName;
                    newPOHeader.DeliverTo = "Logic University";
                    newPOHeader.EmployeeID = (string)Session["LoginEmployeeID"];
                    // --------------------------------------------- IMPORTANT : Need to change this ------------------------------------------------------//
                    newPOHeader.Remarks = "";
                    // ------------------------------------------------------------------------------------------------------------------------------------//

                    newPOHeader.Status = "Open";
                    newPOHeader.TransactionType = "PO";
                    m.POHeaders.Add(newPOHeader);
                    m.SaveChanges();

                    // Loop through PODetails to add items based on selected supplier suppliers
                    foreach (ReorderList pod in poDetailsList)
                    {
                        // Only add if the item is belonging to the supplier / PO
                        string supName = pod.s1;
                        Supplier supplier = m.Suppliers.Where(x => x.CompanyName == supName).FirstOrDefault();
                        if (supplier == s)
                        {
                            // Only add if the item has not been added
                            Item i = m.Items.Where(x => x.ItemCode == pod.ItemCode).FirstOrDefault();
                            if (!itemAdded.Contains(i))
                            {
                                PODetail poDetailToAdd = new PODetail();
                                float itemUnitPrice = m.SupplierPriceLists.Where(x => x.SupplierCode == s.SupplierCode
                                    && x.ItemCode == pod.ItemCode).Select(y => y.UnitCost).FirstOrDefault();
                                poDetailToAdd.PONumber = poId;
                                poDetailToAdd.ItemCode = pod.ItemCode;
                                int qty = Convert.ToInt32(arr1[poDetailsList.IndexOf(pod)]);
                                poDetailToAdd.QuantityOrdered = qty;
                                poDetailToAdd.QuantityBackOrdered = qty;
                                poDetailToAdd.QuantityDelivered = 0;
                                poDetailToAdd.UnitCost = itemUnitPrice;
                                poDetailToAdd.CancelledBackOrdered = 0;
                                m.PODetails.Add(poDetailToAdd);
                                m.SaveChanges();
                                itemAdded.Add(i);
                            }
                        }
                    }
                }
                Session["newPOList"] = new List<PODetail>();
                return View();
            }
        }

        [Authorize(Roles = "Store Clerk")]
        [Route("StockAdj")]
        public ActionResult StockAdj()
        {
            if (Session["StockAdjPage"].ToString() == "1")
            {
                return View("StockAdj");
            }
            else
            {
                if(TempData["ItemList"] == null)
                {
                    List<StockAdjustmentDetail> sadList = new List<StockAdjustmentDetail>();
                    StockAdjustmentDetail sad = new StockAdjustmentDetail();
                    sad.ItemCode = "C001";
                    sad.RequestId = "SA/1000";
                    sad.ItemQuantity = 100;
                    sad.Amount = 100;
                    sad.Remarks = "Damaged";
                    sadList.Add(sad);

                    sad.StockAdjustmentDetails = sadList;

                    Session["StockAdjPage"] = "1";
                    return View("StockAdj2", sad);
                }
                else
                {

                    StockAdjustmentDetail sad = TempData["ItemList"] as StockAdjustmentDetail;

                    Session["StockAdjPage"] = "1";
                    return View("StockAdj2", sad);
                }                
            }
        }
        [Authorize(Roles = "Store Clerk")]
        [HttpPost]
        public RedirectToRouteResult CreateNewStockAdj()
        {                        
            Session["StockAdjPage"] = "2";            
            return RedirectToAction("Inventory", "Store");
        }

        [Authorize(Roles = "Store Clerk")]
        [HttpPost]
        public RedirectToRouteResult AddNewItem(StockAdjustmentDetail stockAdjustmentDetail)
        {
            StockAdjustmentDetail sad = new StockAdjustmentDetail();
            sad.ItemCode = "C002";
            sad.RequestId = "SA/2000";
            sad.ItemQuantity = 1100;
            sad.Amount = 1010;
            sad.Remarks = "Damaged";
            //sad.Reason = "nil";

            stockAdjustmentDetail.StockAdjustmentDetails.Add(sad);

            TempData["ItemList"] = stockAdjustmentDetail;

            Session["StockAdjPage"] = "2";
            return RedirectToAction("Inventory", "Store");
        }

        [Authorize(Roles = "Store Clerk")]
        [HttpPost]
        public RedirectToRouteResult SubmitNewStockAdj(StockAdjustmentDetail stockAdjustmentDetail)
        {
            StockAdjustmentHeader sah = new StockAdjustmentHeader();
            sah.DateRequested = DateTime.Now;
            sah.Requestor = "E1";
            //sah.Status = "Pending";
            sah.TransactionType = "Stock Adjustment";

            using(SSISdbEntities m = new SSISdbEntities())
            {
                m.StockAdjustmentHeaders.Add(sah);
                m.SaveChanges();
            }

            using (SSISdbEntities m = new SSISdbEntities())
            {
                foreach(StockAdjustmentDetail sad in stockAdjustmentDetail.StockAdjustmentDetails)
                {
                    StockAdjustmentDetail sadDb = new StockAdjustmentDetail();
                    sadDb.RequestId = sah.RequestId;
                    sadDb.ItemCode = sad.ItemCode;
                    sadDb.ItemQuantity = sad.ItemQuantity;
                    sadDb.Remarks = sad.Remarks;
                    //sadDb.Reason = sad.Reason;
                    //Temporary
                    sadDb.Amount = 1000;

                    m.StockAdjustmentDetails.Add(sadDb);
                    m.SaveChanges();

                }

            }

            return RedirectToAction("Inventory", "Store");
        }

        [Authorize(Roles = "Store Manager")]
        [Route("StockTake")]
        public ActionResult StockTake()
        {
            return View();
        }

        [Authorize(Roles = "Store Manager")]
        [HttpPost]
        public ActionResult StockTakeUpdate(StockTakeList[] arr, string[] arr1)
        {
            int count = 0;
            
            string transType = "Stock Take";
            List<StockTakeList> list = new List<StockTakeList>();
            for (int i = 0; i < arr.Length; i++)
            {
                list.Add(arr[i]);
            }

            using (SSISdbEntities m = new SSISdbEntities())
            {
                int itemTransCount = m.StockTakeHeaders.Count() + 1;
                string itemTransactionId = "ST-" + itemTransCount.ToString();
                
                // Update StockTakeHeader Table
                StockTakeHeader stockTakeHeader = new StockTakeHeader();
                stockTakeHeader.StockTakeID = itemTransactionId;
                stockTakeHeader.Date = DateTime.Now;
                stockTakeHeader.TransactionType = transType;
                m.StockTakeHeaders.Add(stockTakeHeader);
                m.SaveChanges();

                foreach (StockTakeList l in list)
                {
                    Item item = m.Items.Where(x => x.ItemCode == l.ItemCode).FirstOrDefault();
                    int itemQty = Convert.ToInt32(arr1[count]);
                    float avgCost = item.AvgUnitCost;
                    int qtyOnHand = item.Quantity;
                    int qtyAdjusted = itemQty - qtyOnHand;
                    float totalAmt = avgCost * (float) qtyAdjusted;
                    string itemcode = l.ItemCode;

                    if(qtyAdjusted != 0)
                    {
                        // Update Item Table
                        item.Quantity = itemQty;

                        // Update ItemTransaction Table
                        ItemTransaction itemTransaction = new ItemTransaction();
                        itemTransaction.TransDateTime = DateTime.Now;
                        itemTransaction.DocumentRefNo = itemTransactionId;
                        itemTransaction.ItemCode = itemcode;
                        itemTransaction.TransactionType = transType;
                        itemTransaction.Quantity = qtyAdjusted;
                        itemTransaction.UnitCost = avgCost;
                        itemTransaction.Amount = totalAmt;
                        m.ItemTransactions.Add(itemTransaction);
                        m.SaveChanges();
                    }

                    // Update StockTakeDetails Table
                    StockTakeDetail stockTakeDetail = new StockTakeDetail();
                    stockTakeDetail.StockTakeID = itemTransactionId;
                    stockTakeDetail.ItemCode = itemcode;
                    stockTakeDetail.QuantityOnHand = qtyOnHand;
                    stockTakeDetail.QuantityCounted = itemQty;
                    stockTakeDetail.QuantityAdjusted = qtyAdjusted;
                    m.StockTakeDetails.Add(stockTakeDetail);
                    m.SaveChanges();
                    count++;
                }
            }
            return View();
        }
        
    }
}
