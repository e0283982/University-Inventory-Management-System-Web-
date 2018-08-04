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
    public class StoreInventoryController : Controller //Tabs: Inventory, Reorder, Stock adj, Stock take
    {
        /************Action methods belonging to Store Inventory - Inventory*******************/

        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        [Route("Overview")]
        public ActionResult Overview()
        {
            Session["StoreInventoryTabIndex"] = "1";

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

        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        [HttpPost]
        public ActionResult DisplayItemDetails(string maintenanceItemCode)
        {
            Session["InventoryOverviewPage"] = "2";
            Session["MaintenanceItemCode"] = maintenanceItemCode;

            return View("Overview2");
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        [HttpPost]
        public RedirectToRouteResult BackToInventoryOverviewList()
        {
            Session["InventoryOverviewPage"] = "1";

            return RedirectToAction("Inventory", "Store");
        }

        /************Action methods belonging to Store Inventory - Reorder *******************/

        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        [Route("Reorder")]
        public ActionResult Reorder()
        {
            Session["StoreInventoryTabIndex"] = "2";

            List<ReorderList> reorderLists;
            using (SSISdbEntities m = new SSISdbEntities())
            {
                reorderLists = m.ReorderLists.ToList();
                Session["ReorderList"] = reorderLists;
            }
            return View();
        }

        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        [HttpPost]
        public RedirectToRouteResult AddToPO(string[] arr1, string[] arr2, string[] arrSupplier)
        {
            try
            {
                int enteredQty = 0;
                if (arr1 != null)
                {
                    for (int i = 0; i < arr1.Length; i++)
                    {
                        enteredQty += Convert.ToInt32(arr1[i]);
                    }
                }


                if (enteredQty > 0)
                {
                    List<ReorderList> poDetailsList = (List<ReorderList>)Session["ReorderList"];
                    List<Supplier> supplierList = new List<Supplier>();
                    List<Item> itemAdded = new List<Item>();
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
                            if (arr2.Contains(p.Description))
                            {
                                int index = arr2.ToList().FindIndex(x => x == p.Description);
                                string coy = arrSupplier[index];
                                Supplier sup = m.Suppliers.Where(x => x.CompanyName == coy).FirstOrDefault();
                                p.s1 = sup.CompanyName;
                            }

                        }

                        // Each Supplier iterates once such that only 1 PO is created for each of them
                        foreach (Supplier s in supplierList)
                        {
                            // Create new PO based on supplier
                            int count = m.POHeaders.Count() + 1;
                            string poId = CommonLogic.SerialNo(count, "PO");

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

                                        int index = arr2.ToList().FindIndex(x => x == pod.Description);
                                        if (index != -1)
                                        {
                                            int qty = Convert.ToInt32(arr1[index]);
                                            poDetailToAdd.QuantityOrdered = qty;
                                            poDetailToAdd.QuantityBackOrdered = qty;
                                            poDetailToAdd.QuantityDelivered = 0;
                                            poDetailToAdd.UnitCost = itemUnitPrice;
                                            poDetailToAdd.CancelledBackOrdered = 0;
                                            m.PODetails.Add(poDetailToAdd);
                                        }


                                        m.SaveChanges();
                                        itemAdded.Add(i);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    TempData["ErrorMsg"] = "You have not included any quantity";
                }
                Session["newPOList"] = new List<PODetail>();
            }
            catch (Exception e)
            {
                TempData["ErrorMsg"] = "Please key in a valid quantity";
            }


            return RedirectToAction("Inventory", "Store");
        }

        /************Action methods belonging to Store Inventory - Stock Adjustment *******************/

        [CustomAuthorize(Roles = "Store Clerk")]
        [Route("StockAdj")]
        public ActionResult StockAdj()
        {
            Session["StoreInventoryTabIndex"] = "3";

            if (Session["StockAdjPage"].ToString() == "1")
            {
                return View("StockAdj");
            }
            else
            {
                using (SSISdbEntities e = new SSISdbEntities())
                {

                    ViewBag.AdjustmentReasons = new SelectList(new List<String>() { "Damaged", "Missing" }, null);
                    List<String> tempList = (List<String>)Session["tempList"];

                    if (tempList.Count == 0)
                    {
                        ViewBag.ItemsList = new SelectList((from s in e.Items.OrderBy(x => x.Description).ToList()
                                                            select new
                                                            {
                                                                ItemCode = s.ItemCode,
                                                                Description = s.Description + " (" + s.UoM + ")"
                                                            }),
                                                        "ItemCode", "Description", null);
                    }
                    else
                    {

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
                }

                Session["StockAdjPage"] = "1";
                return View("StockAdj2");

            }
        }

        [HttpPost]
        public RedirectToRouteResult BackToStockAdjList()
        {
            Session["StockAdjPage"] = "1";

            return RedirectToAction("Inventory", "Store");
        }

        [CustomAuthorize(Roles = "Store Clerk")]
        [HttpPost]
        [Route("StockAdj/AddNewAdjItem")]
        public RedirectToRouteResult AddNewAdjItem(StockAdjItemModel item)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                string itemCode = Request.Form["SelectItemDesc"].ToString();
                string itemDesc = e.Items.Where(x => x.ItemCode == itemCode).Select(x => x.Description).FirstOrDefault();
                float avgUnitCost = e.Items.Where(x => x.ItemCode == itemCode).Select(x => x.AvgUnitCost).FirstOrDefault();
                string reason = Request.Form["SelectAdjReason"].ToString();

                List<StockAdjItemModel> list = new List<StockAdjItemModel>();
                list = (List<StockAdjItemModel>)Session["newAdjList"];
                StockAdjItemModel sad = new StockAdjItemModel();
                sad.ItemCode = itemCode;
                sad.ItemDesc = itemDesc;
                sad.AdjQty = item.AdjQty;
                sad.Reason = reason;
                sad.AdjCost = (avgUnitCost * item.AdjQty);
                sad.AvgUnitCost = avgUnitCost;

                list.Add(sad);
                Session["newAdjList"] = list;

                //add to list meant for already added items
                List<String> tempList = (List<String>)Session["tempList"];
                tempList.Add(itemCode);
                Session["tempList"] = tempList;

                Session["StockAdjPage"] = "2";

                return RedirectToAction("Inventory", "Store");
            }
        }

        [CustomAuthorize(Roles = "Store Clerk")]
        [HttpPost]
        public RedirectToRouteResult EditNewAdjQty(string data, int index)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                //edit sad item on qty text change
                //data is item desc, index is list index
                List<StockAdjItemModel> list = (List<StockAdjItemModel>)Session["newAdjList"];
                StockAdjItemModel item = list.ElementAt(index);
                item.AdjQty = Int32.Parse(data);
                item.AdjCost = Int32.Parse(data) * item.AvgUnitCost;
                Session["newAdjList"] = list;

                Session["StockAdjPage"] = "2";


                return RedirectToAction("Requisition", "Dept");
            }
        }

        [CustomAuthorize(Roles = "Store Clerk")]
        [HttpPost]
        public RedirectToRouteResult DiscardNewAdjItem(string data, int index)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                //data is item desc, index is list index
                List<StockAdjItemModel> list = (List<StockAdjItemModel>)Session["newAdjList"];
                list.RemoveAt(index);
                Session["newAdjList"] = list;

                Session["StockAdjPage"] = "2";

                //add to list meant for already added items
                List<String> tempList = (List<String>)Session["tempList"];
                int tempListIndex = tempList.FindIndex(x=>x==data);
                tempList.RemoveAt(tempListIndex);
                Session["tempList"] = tempList;

                return RedirectToAction("Inventory", "Store");
            }
        }

        [HttpPost]
        public RedirectToRouteResult ClearNewAdjItems()
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                //clear temp list
                List<StockAdjItemModel> list = (List<StockAdjItemModel>)Session["newAdjList"];
                list.Clear();
                Session["newAdjList"] = list;

                //remove from list meant for already added items
                List<String> tempList = (List<String>)Session["tempList"];
                tempList.Clear();
                Session["tempList"] = tempList;

                Session["StockAdjPage"] = "2";


                return RedirectToAction("Inventory", "Store");
            }
        }


        [CustomAuthorize(Roles = "Store Clerk")]
        [HttpPost]
        public RedirectToRouteResult SubmitNewAdj()
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                //data is item desc, index is list index
                int adjCount = e.StockAdjustmentHeaders.ToList().Count() + 1;
                string newAdjHeaderId = CommonLogic.SerialNo(adjCount, "SA");

                StockAdjustmentHeader sah = new StockAdjustmentHeader();
                sah.RequestId = newAdjHeaderId;
                sah.DateRequested = DateTime.Now;
                sah.Requestor = Session["LoginEmployeeID"].ToString();
                sah.DateProcessed = null;
                sah.TransactionType = "Stock Adjustment";

                DAL.StockAdjustmentRepositoryImpl dal = new DAL.StockAdjustmentRepositoryImpl(e);
                dal.InsertStockAdjustmentHeader(sah);

                DAL.StockAdjustmentDetailsRepositoryImpl dalDetails = new DAL.StockAdjustmentDetailsRepositoryImpl(e);

                //insert SAH details
                foreach (StockAdjItemModel detail in (List<StockAdjItemModel>)Session["NewAdjList"])
                {
                    StockAdjustmentDetail sad = new StockAdjustmentDetail();
                    sad.RequestId = newAdjHeaderId;
                    sad.ItemCode = detail.ItemCode;
                    sad.Amount = float.Parse(detail.AdjCost.ToString());
                    sad.ItemQuantity = detail.AdjQty;
                    sad.Remarks = detail.Reason;
                    sad.Status = "Pending";
                    sad.NotificationStatus = "Unread";

                    dalDetails.InsertStockAdjustmentDetail(sad);

                    //Create new Item Transaction
                    ItemTransaction itemTransaction = new ItemTransaction();
                    itemTransaction.TransDateTime = DateTime.Now;
                    itemTransaction.DocumentRefNo = sad.RequestId;
                    itemTransaction.ItemCode = sad.ItemCode;
                    itemTransaction.TransactionType = "Stock Adjustment";
                    itemTransaction.Quantity = sad.ItemQuantity;

                    float itemUnitCost = e.Items.Where(x => x.ItemCode == sad.ItemCode).Select(x => x.AvgUnitCost).FirstOrDefault();

                    itemTransaction.UnitCost = itemUnitCost;
                    itemTransaction.Amount = itemTransaction.Quantity * itemTransaction.UnitCost;
                    e.ItemTransactions.Add(itemTransaction);

                    //Update Item Quantity
                    Item item = e.Items.Where(x => x.ItemCode == sad.ItemCode).FirstOrDefault();
                    item.Quantity = item.Quantity - sad.ItemQuantity;

                }

                e.SaveChanges();

                Session["NewAdjList"] = new List<StockAdjItemModel>();

                return RedirectToAction("Inventory", "Store");
            }
        }

        [CustomAuthorize(Roles = "Store Clerk")]
        [HttpPost]
        public RedirectToRouteResult CreateNewStockAdj()
        {
            Session["StockAdjPage"] = "2";

            return RedirectToAction("Inventory", "Store");
        }

        //[HttpPost]
        //public RedirectToRouteResult AddNewItem(StockAdjustmentDetail stockAdjustmentDetail)
        //{
        //    StockAdjustmentDetail sad = new StockAdjustmentDetail();
        //    sad.ItemCode = "C002";
        //    sad.RequestId = "SA/2000";
        //    sad.ItemQuantity = 1100;
        //    sad.Amount = 1010;
        //    sad.Remarks = "Damaged";
        //    //sad.Reason = "nil";

        //    stockAdjustmentDetail.StockAdjustmentDetails.Add(sad);

        //    TempData["ItemList"] = stockAdjustmentDetail;

        //    Session["StockAdjPage"] = "2";
        //    return RedirectToAction("Inventory", "Store");
        //}

        //[CustomAuthorize(Roles = "Store Clerk")]
        //[HttpPost]
        //public RedirectToRouteResult SubmitNewStockAdj(StockAdjustmentDetail stockAdjustmentDetail)
        //{
        //    StockAdjustmentHeader sah = new StockAdjustmentHeader();
        //    sah.DateRequested = DateTime.Now;
        //    sah.Requestor = (string) Session["LoginEmployeeID"];
        //    //sah.Status = "Pending";
        //    sah.TransactionType = "Stock Adjustment";

        //    using(SSISdbEntities m = new SSISdbEntities())
        //    {
        //        m.StockAdjustmentHeaders.Add(sah);
        //        m.SaveChanges();

        //        foreach(StockAdjustmentDetail sad in stockAdjustmentDetail.StockAdjustmentDetails)
        //        {
        //            StockAdjustmentDetail sadDb = new StockAdjustmentDetail();
        //            sadDb.RequestId = sah.RequestId;
        //            sadDb.ItemCode = sad.ItemCode;
        //            sadDb.ItemQuantity = sad.ItemQuantity;
        //            sadDb.Remarks = sad.Remarks;
        //            //sadDb.Reason = sad.Reason;
        //            //Temporary
        //            sadDb.Amount = 1000;

        //            m.StockAdjustmentDetails.Add(sadDb);
        //            m.SaveChanges();

        //        }

        //    }

        //    return RedirectToAction("Inventory", "Store");
        //}

        /************Action methods belonging to Store Inventory - Stock Take *******************/

        [CustomAuthorize(Roles = "Store Manager, Store Supervisor")]
        [Route("StockTake")]
        public ActionResult StockTake()
        {
            Session["StoreInventoryTabIndex"] = "4";

            return View();
        }

        [CustomAuthorize(Roles = "Store Manager, Store Supervisor")]
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
                string itemTransactionId = CommonLogic.SerialNo(itemTransCount, "ST");

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
                    float totalAmt = avgCost * (float)qtyAdjusted;
                    string itemcode = l.ItemCode;

                    if (qtyAdjusted != 0)
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
