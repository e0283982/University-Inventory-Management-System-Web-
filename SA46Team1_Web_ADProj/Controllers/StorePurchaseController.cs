using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [Authorize(Roles = "Store Clerk, Store Manager")]
    [RoutePrefix("Store/StorePurchase")]
    public class StorePurchaseController : Controller
    {
        [Route("CreatePO")]
        public ActionResult CreatePO()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                Tuple<Item, PODetail> tuple = new Tuple<Item, PODetail>(new Item(), new PODetail());
                ViewBag.ItemsList = new SelectList((from s in m.Items.ToList()
                                                    select new
                                                    {
                                                        ItemCode = s.ItemCode,
                                                        Description = s.Description + " (" + s.UoM + ")"
                                                    }),
                                                    "ItemCode",
                                                    "Description",
                                                    null);
                return View(tuple);
            }
        }

        [HttpPost]
        public RedirectToRouteResult DeletePOItem(string data)
        {
            string itemCode = data;
            List<PODetail> poFullDetailsList = (List<PODetail>)Session["newPOList"];
            PODetail pod = new PODetail();
            foreach (PODetail p in poFullDetailsList)
            {
                if (p.ItemCode == itemCode)
                {
                    pod = p;
                }
            }
            poFullDetailsList.Remove(pod);
            Session["newPOList"] = poFullDetailsList;
            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public ActionResult SavePO(string[] arrQty, string[] arrSupplier)
        {
            List<PODetail> poDetailsList = (List<PODetail>)Session["newPOList"];
            List<Supplier> supplierList = new List<Supplier>();
            List<Item> itemAdded = new List<Item>();
            int arrayCount = 0;
            using (SSISdbEntities m = new SSISdbEntities())
            {
                // Checking for number of suppliers to Iterate & later used for creating no. of PO
                for(int i = 0; i < arrSupplier.Length; i++)
                {
                    string supCode = arrSupplier[i];
                    Supplier supplier = m.Suppliers.Where(x => x.SupplierCode == supCode).FirstOrDefault();
                    if (!supplierList.Contains(supplier))
                    {
                        supplierList.Add(supplier);
                    }
                }

                // Change Item Supplier in PODetails for retrieving in list and adding into database later
                foreach (PODetail p in poDetailsList)
                {
                    string coy = arrSupplier[arrayCount];
                    Supplier sup = m.Suppliers.Where(x => x.SupplierCode == coy).FirstOrDefault();
                    p.Item.Supplier1 = sup.SupplierCode;
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
                    newPOHeader.EmployeeID = (string) Session["LoginEmployeeID"];
                    // --------------------------------------------- IMPORTANT : Need to change this ------------------------------------------------------//
                    newPOHeader.Remarks = "";
                    // ------------------------------------------------------------------------------------------------------------------------------------//

                    newPOHeader.Status = "Open";
                    newPOHeader.TransactionType = "PO";
                    m.POHeaders.Add(newPOHeader);
                    m.SaveChanges();

                    // Loop through PODetails to add items based on selected supplier suppliers
                    foreach (PODetail pod in poDetailsList)
                    {
                        // Only add if the item is belonging to the supplier / PO
                        string supCode = pod.Item.Supplier1;
                        Supplier supplier = m.Suppliers.Where(x => x.SupplierCode == supCode).FirstOrDefault();
                        if (supplier == s)
                        {
                            // Only add if the item has not been added
                            if (!itemAdded.Contains(pod.Item))
                            {
                                PODetail poDetailToAdd = new PODetail();
                                float itemUnitPrice = m.SupplierPriceLists.Where(x => x.SupplierCode == s.SupplierCode
                                    && x.ItemCode == pod.Item.ItemCode).Select(y => y.UnitCost).FirstOrDefault();
                                poDetailToAdd.PONumber = poId;
                                poDetailToAdd.ItemCode = pod.ItemCode;
                                int qty = Convert.ToInt32(arrQty[poDetailsList.IndexOf(pod)]);
                                poDetailToAdd.QuantityOrdered = qty;
                                poDetailToAdd.QuantityBackOrdered = qty;
                                poDetailToAdd.QuantityDelivered = 0;
                                poDetailToAdd.UnitCost = itemUnitPrice;
                                poDetailToAdd.CancelledBackOrdered = 0;
                                m.PODetails.Add(poDetailToAdd);
                                m.SaveChanges();
                                itemAdded.Add(pod.Item);
                            }
                        }
                    }
                }
                Session["newPOList"] = new List<PODetail>();
                return View();
            }
        }

        [HttpPost]
        public RedirectToRouteResult AddPOItem(PODetail item2)
        {
            Item itemToAdd = new Item();
            bool existingItem = false;
            using (SSISdbEntities m = new SSISdbEntities())
            {
                // Get Item to add into Datatable
                string itemCode = Request.Form["SelectItemChose"].ToString();
                List<PODetail> poFullDetailsList = (List<PODetail>)Session["newPOList"];
                PODetail pod = new PODetail();
                PODetail existingPoD = new PODetail();
                pod.PONumber = (string)Session["newPoId"];

                // Checking if Item is already in list
                foreach (PODetail p in poFullDetailsList.ToList())
                {
                    // Iterate through whole list to check for itemcode
                    if (p.ItemCode == itemCode)
                    {
                        // Combine 2 same items (Adding new qty to exisiting qty)
                        existingItem = true;
                        // Create new item so that we can remove old qty & add back new qty
                        existingPoD = p;

// ---------------------------------- IMPORTANT : Need to change this qty based on JSON of array ---------------------------------------------//
                        existingPoD.QuantityOrdered += item2.QuantityOrdered;
//--------------------------------------------------------------------------------------------------------------------------------------------//

                        poFullDetailsList.Remove(p);
                        poFullDetailsList.Add(existingPoD);
                    }
                }

                // Execute when item is not in the list
                if (existingItem == false)
                {
                    // Creating the new item with these 
                    itemToAdd = m.Items.Where(x => x.ItemCode == itemCode).FirstOrDefault();
                    pod.ItemCode = itemToAdd.ItemCode;

// ---------------------------------- IMPORTANT : Need to change this qty based on JSON of array ---------------------------------------------//
                    pod.QuantityOrdered = item2.QuantityOrdered;
// -------------------------------------------------------------------------------------------------------------------------------------------//

                    pod.Item = m.Items.Where(x => x.ItemCode == itemToAdd.ItemCode).FirstOrDefault();

                    pod.Item.Description = itemToAdd.Description;
                    pod.Item.UoM = itemToAdd.UoM;
                    pod.Item.Supplier1 = itemToAdd.Supplier1;
                    pod.Item.AvgUnitCost = itemToAdd.AvgUnitCost;

                    poFullDetailsList.Add(pod);
                }

                Session["newPOList"] = poFullDetailsList;
                return RedirectToAction("Purchase", "Store");
            }
        }

        [Route("POList")]
        public ActionResult POList()
        {
            if (Session["POListPage"].ToString() == "1")
            {
                return View("POList");
            }
            else
            {
                Session["POListPage"] = "1";
                return View("DisplayPO");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayPO(FormCollection data)
        {
            Session["POListPage"] = "2";
            string poNumber = data["PONumber"];
            Session["poNumber"] = poNumber;
            using(SSISdbEntities m = new SSISdbEntities())
            {
                List<POFullDetail> poFullDetailList = m.POFullDetails.Where(x => x.PONumber == poNumber).ToList();
                Session["POItems"] = poFullDetailList;
            }
            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToPOList()
        {
            Session["POListPage"] = "1";

            return RedirectToAction("Purchase", "Store");
        }

        [Route("GoodsReceivedList")]
        public ActionResult GoodsReceivedList()
        {
            if (Session["GRListPage"].ToString() == "1")
            {
                @Session["BackToGRList"] = "false";
                return View("GoodsReceivedList");
            }
            else
            {
                Session["GRListPage"] = "1";
                return View("GoodsReceivedList2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayGR(FormCollection data)
        {
            Session["GRListPage"] = "2";
            string rNo = data["ReceiptNo"];
            Session["grId"] = rNo;
            return RedirectToAction("Purchase", "Store");
        }


        [HttpPost]
        public RedirectToRouteResult BackToGRList()
        {
            Session["GRListPage"] = "1";
            Session["BackToGRList"] = "true";

            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult EditMode()
        {
            Session["poDetailsEditMode"] = true;
            Session["POListPage"] = "2";
// ----------------------------- Should we add Viewbag for title to differentiate GR / edit ? -------------------------------------------
            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult SaveEdit(string[] arrQty)
        {
            Session["poDetailsEditMode"] = false;
            Session["POListPage"] = "2";
            List<POFullDetail> poFullDetailList = (List<POFullDetail>)Session["POItems"];
            string poNumber = (string)Session["poNumber"];
            int arrayCount = 0;
            using (SSISdbEntities m = new SSISdbEntities())
            {
                foreach(POFullDetail p in poFullDetailList)
                {
                    PODetail pod = m.PODetails.Where(x => x.ItemCode == p.ItemCode && x.PONumber == p.PONumber).FirstOrDefault();
                    pod.QuantityBackOrdered = Convert.ToInt32(arrQty[arrayCount]);
                    pod.QuantityOrdered = pod.QuantityBackOrdered;
                    m.SaveChanges();
                    arrayCount++;

                    p.QuantityOrdered = pod.QuantityBackOrdered;
                }
            }
            Session["POItems"] = poFullDetailList;
            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult ExitEditMode()
        {
            Session["poDetailsEditMode"] = false;
            Session["POListPage"] = "2";
            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public ActionResult CancelPO()
        {
            string poNumber = (string) Session["poNumber"];
            using(SSISdbEntities m = new SSISdbEntities())
            {
                POHeader poHeader = m.POHeaders.Where(x => x.PONumber == poNumber).FirstOrDefault();
                poHeader.Status = "Cancelled";
                m.SaveChanges();

                List<POFullDetail> poFullDetailsList = (List<POFullDetail>)Session["POItems"];
                List<PODetail> podItems = m.PODetails.Where(x => x.PONumber == poNumber).ToList();
                foreach(POFullDetail p in poFullDetailsList)
                {
                    foreach(PODetail pod in podItems)
                    {
                        if(pod.ItemCode == p.ItemCode)
                        {
                            pod.CancelledBackOrdered = pod.QuantityBackOrdered;
                            pod.QuantityBackOrdered = 0;
                            m.SaveChanges();
                        }
                        p.QuantityOrdered = 0;
                    }
                }
                Session["POItems"] = poFullDetailsList;
            }
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult GoodsReceipt(string[] arrQty)
        {
            // Need to check if whole PO backorder = 0 -> status = "Completed"
            // Update qty
            // Update POHeader status
            // Update PO Details
            // Create new PORH if null
            // POReceiptDetails
            List<POFullDetail> poFullDetailList = (List<POFullDetail>)Session["POItems"];
            string poNumber = (string)Session["poNumber"];
            int arrayCount = 0;
            float totalInventoryValue = 0;
            string receiptNo = null;

            using (SSISdbEntities m = new SSISdbEntities())
            {
                int newId = m.POReceiptHeaders.Count() + 1;
                receiptNo = "POR-" + newId.ToString();
                int countQty = 0;

                foreach (POFullDetail p in poFullDetailList)
                {
                    PODetail pod = m.PODetails.Where(x => x.ItemCode == p.ItemCode && x.PONumber == p.PONumber).FirstOrDefault();
                    int qty = Convert.ToInt32(arrQty[arrayCount]);
                    countQty += qty;

                    // Only execute if qty > 0, meaning there's good received.
                    if (qty > 0)
                    {
                        if (pod.QuantityBackOrdered == qty)
                        {
                            pod.QuantityBackOrdered = 0;
                        
                        }
                        else
                        {
                            pod.QuantityBackOrdered = pod.QuantityBackOrdered - qty;
                        }
                        pod.QuantityDelivered = qty;
                        m.SaveChanges();

                        totalInventoryValue += qty * p.UnitCost;

                        // Update Item on Hand
                        Item item = m.Items.Where(x => x.ItemCode == p.ItemCode).FirstOrDefault();
                        item.Quantity += qty;
                        m.SaveChanges();

                        // Update Item Transaction
                        ItemTransaction itemTransaction = new ItemTransaction();
                        itemTransaction.TransDateTime = DateTime.Now;
                        itemTransaction.DocumentRefNo = poNumber;
                        itemTransaction.ItemCode = item.ItemCode;
                        itemTransaction.TransactionType = "PO Receipt";
                        itemTransaction.Quantity = qty;
                        itemTransaction.UnitCost = pod.UnitCost;
                        itemTransaction.Amount = pod.UnitCost * qty;
                        m.ItemTransactions.Add(itemTransaction);
                        m.SaveChanges();

                        // Update POReceipt Details
                        POReceiptDetail pord = new POReceiptDetail();
                        pord.ReceiptNo = receiptNo;
                        pord.PONumber = poNumber;
                        pord.ItemCode = item.ItemCode;
                        pord.QuantityReceived = qty;
                        pord.UnitCost = pod.UnitCost;
                        pord.Amount = pod.UnitCost * qty;
                        m.POReceiptDetails.Add(pord);
                        m.SaveChanges();

                        p.QuantityOrdered = pod.QuantityBackOrdered;
                        p.QuantityDelivered = pod.QuantityDelivered;
                    }
                    arrayCount++;
                }

                int countInventory = 0;

                // Checking for completion of PO
                foreach(POFullDetail p in poFullDetailList)
                {
                    PODetail pod = m.PODetails.Where(x => x.ItemCode == p.ItemCode && x.PONumber == p.PONumber).FirstOrDefault();
                    countInventory += pod.QuantityBackOrdered;
                    totalInventoryValue += pod.UnitCost;
                }

                if(countQty == 0)
                {

                    // GR here
                    POReceiptHeader porh = new POReceiptHeader();
                    porh.ReceiptNo = receiptNo;
                    porh.PONumber = poNumber;
                    porh.DeliveryOrderNo = "Hardcode first";
                    porh.ReceivedDate = DateTime.Now;
                    porh.Remarks = null;
                    porh.TransactionType = "PO Receipt";
                    porh.TotalAmount = totalInventoryValue;
                    m.POReceiptHeaders.Add(porh);
                    m.SaveChanges();

                    // Completed PO
                    if (countInventory == 0)
                    {
                        // Update PO Header
                        POHeader poh = m.POHeaders.Where(x => x.PONumber == poNumber).FirstOrDefault();
                        poh.Status = "Completed";
                        m.SaveChanges();
                    }
                }
            }

            Session["POItems"] = poFullDetailList;
            Session["poDetailsEditMode"] = false;
            Session["GRListPage"] = "2";
            Session["grId"] = receiptNo;
            return RedirectToAction("Purchase", "Store");
        }
    }
}
