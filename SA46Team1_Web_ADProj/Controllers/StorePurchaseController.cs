using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [CustomAuthorize(Roles = "Store Manager, Store Supervisor, Store Clerk")]
    [RoutePrefix("Store/StorePurchase")]
    public class StorePurchaseController : Controller //tabs: PO list, Create PO, Goods Received
    {
        /************Action methods belonging to Store Purchase - Create PO *******************/

        [Route("CreatePO")]
        public ActionResult CreatePO()
        {
            Session["StorePurchaseTabIndex"] = "2";

            using (SSISdbEntities m = new SSISdbEntities())
            {
                List<POFullDetail> poFullDetailsList = (List<POFullDetail>)Session["newPOList"];

                double grossTotal = 0;
                double netTotal = 0;
                double gst = 0;

                foreach (POFullDetail p in poFullDetailsList) {
                    var itemTotal=0F;
                    if (p.CompanyName == p.Supplier1Code)
                    {
                        itemTotal = p.Supplier1UnitCost * p.QuantityOrdered;
                    }
                    else if (p.CompanyName == p.Supplier2Code)
                    {
                        itemTotal = p.Supplier2UnitCost * p.QuantityOrdered;
                    }
                    else if (p.CompanyName == p.Supplier3Code)
                    {
                        itemTotal = p.Supplier3UnitCost * p.QuantityOrdered;
                    }

                    grossTotal += itemTotal;
                }

                TempData["grossTotal"] = Math.Round(grossTotal,2, MidpointRounding.AwayFromZero);

                netTotal = grossTotal * 1.07;
                TempData["netTotal"] = Math.Round(netTotal,2, MidpointRounding.AwayFromZero);

                gst = grossTotal * 0.07;
                TempData["gst"]= Math.Round(gst, 2, MidpointRounding.AwayFromZero);

                Tuple<Item, POFullDetail> tuple = new Tuple<Item, POFullDetail>(new Item(), new POFullDetail());

                List<String> tempList = (List<String>)Session["tempList"];

                if (tempList.Count == 0)
                {
                    ViewBag.ItemsList = new SelectList((from s in m.Items.OrderBy(x => x.Description).ToList()
                                                        select new
                                                        {
                                                            ItemCode = s.ItemCode,
                                                            Description = s.Description + " (" + s.UoM + ")"
                                                        }),
                                                    "ItemCode", "Description", null);
                }
                else
                {

                    List<Item> newItemList = m.Items.Where(x => !tempList.Contains(x.ItemCode)).OrderBy(x => x.Description).ToList();
                    //update ddl to remove items
                    ViewBag.ItemsList = new SelectList((from s in newItemList
                                                        select new
                                                        {
                                                            ItemCode = s.ItemCode,
                                                            Description = s.Description + " (" + s.UoM + ")"
                                                        }),
                                                        "ItemCode", "Description", null);
                }

                return View(tuple);
            }
        }

        [HttpPost]
        public RedirectToRouteResult DeletePOItem(string data)
        {
            string itemCode = data;
            List<POFullDetail> poFullDetailsList = (List<POFullDetail>)Session["newPOList"];
            POFullDetail pod = new POFullDetail();
            foreach (POFullDetail p in poFullDetailsList)
            {
                if (p.ItemCode == itemCode)
                {
                    pod = p;
                }
            }
            poFullDetailsList.Remove(pod);

            using (SSISdbEntities e = new SSISdbEntities()) {
                //remove from list meant for already added items
                List<String> tempList = (List<String>)Session["tempList"];
                tempList.Remove(pod.ItemCode);
                Session["tempList"] = tempList;
            }
            

            if (poFullDetailsList.Count == 0)
            {
                poFullDetailsList = new List<POFullDetail>();
            }
            Session["newPOList"] = poFullDetailsList;


            return RedirectToAction("CreatePO", "StorePurchase");
        }

        [HttpPost]
        public RedirectToRouteResult SavePO(string[] arrQty, string[] arrSupplier, string taData)
        {
            // To ensure no empty Entries
            int enteredQty = 0;
            if(arrQty.Count() > 0)
            {
                for (int i = 0; i < arrQty.Length; i++)
                {
                    enteredQty += Convert.ToInt32(arrQty[i]);
                }
            }


            if (enteredQty > 0)
            {
                List<POFullDetail> poDetailsList = (List<POFullDetail>)Session["newPOList"];
                List<Supplier> supplierList = new List<Supplier>();
                List<Item> itemAdded = new List<Item>();
                int arrayCount = 0;
                using (SSISdbEntities m = new SSISdbEntities())
                {
                    // Checking for number of suppliers to Iterate & later used for creating no. of PO
                    for (int i = 0; i < arrSupplier.Length; i++)
                    {
                        string supCode = arrSupplier[i];
                        Supplier supplier = m.Suppliers.Where(x => x.SupplierCode == supCode).FirstOrDefault();
                        if (!supplierList.Contains(supplier))
                        {
                            supplierList.Add(supplier);
                        }
                    }

                    // Change Item Supplier in PODetails for retrieving in list and adding into database later
                    foreach (POFullDetail p in poDetailsList)
                    {
                        string coy = arrSupplier[arrayCount];
                        Supplier sup = m.Suppliers.Where(x => x.SupplierCode == coy).FirstOrDefault();
                        p.CompanyName = sup.CompanyName;
                        arrayCount++;
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
                        newPOHeader.Remarks = taData;
                        // ------------------------------------------------------------------------------------------------------------------------------------//

                        newPOHeader.Status = "Open";
                        newPOHeader.TransactionType = "PO";
                        m.POHeaders.Add(newPOHeader);
                        m.SaveChanges();

                        // Loop through PODetails to add items based on selected supplier suppliers
                        foreach (POFullDetail pod in poDetailsList)
                        {
                            // Only add if the item is belonging to the supplier / PO
                            string supCode = m.Suppliers.Where(x=>x.CompanyName==pod.CompanyName).Select(x=>x.SupplierCode).FirstOrDefault();
                            Supplier supplier = m.Suppliers.Where(x => x.SupplierCode == supCode).FirstOrDefault();
                            if (supplier == s)
                            {
                                // Only add if the item has not been added
                               // if (!itemAdded.Contains(pod.Item))
                                //{
                                    PODetail poDetailToAdd = new PODetail();
                                    float itemUnitPrice = m.SupplierPriceLists.Where(x => x.SupplierCode == s.SupplierCode
                                        && x.ItemCode == pod.ItemCode).Select(y => y.UnitCost).FirstOrDefault();
                                    poDetailToAdd.PONumber = poId;
                                    poDetailToAdd.ItemCode = pod.ItemCode;
                                    int qty = Convert.ToInt32(arrQty[poDetailsList.IndexOf(pod)]);
                                    poDetailToAdd.QuantityOrdered = qty;
                                    poDetailToAdd.QuantityBackOrdered = qty;   //this?
                                    poDetailToAdd.QuantityDelivered = 0;
                                    poDetailToAdd.UnitCost = itemUnitPrice;
                                    poDetailToAdd.CancelledBackOrdered = 0;
                                    m.PODetails.Add(poDetailToAdd);
                                    m.SaveChanges();

                                    Item item = new Item();
                                    item = m.Items.Where(x => x.ItemCode == pod.ItemCode && pod.CompanyName == supplier.CompanyName).FirstOrDefault();
                                    itemAdded.Add(item);
                              //  }
                            }
                        }
                    }
                }
            }

            //add to list meant for already added items
            List<String> tempList = (List<String>)Session["tempList"];
            tempList.Clear();
            Session["tempList"] = tempList;

            Session["newPOList"] = new List<POFullDetail>();

            Session["POListPage"] = "1";

            return RedirectToAction("POList", "StorePurchase");
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
                List<POFullDetail> poFullDetailsList = (List<POFullDetail>)Session["newPOList"];
                POFullDetail pod = new POFullDetail();
                POFullDetail existingPoD = new POFullDetail();
                pod.PONumber = (string)Session["newPoId"];

                // Checking if Item is already in list
                foreach (POFullDetail p in poFullDetailsList.ToList())
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

                    //pod.Item = m.Items.Where(x => x.ItemCode == itemToAdd.ItemCode).FirstOrDefault();

                    pod.Description = itemToAdd.Description;
                    pod.UoM = itemToAdd.UoM;
                    pod.CompanyName = itemToAdd.Supplier1; //default ddl at supplier 1's
                    pod.UnitCost = itemToAdd.AvgUnitCost;
                    pod.Supplier1Code = itemToAdd.Supplier1;
                    pod.Supplier2Code = itemToAdd.Supplier2;
                    pod.Supplier3Code = itemToAdd.Supplier3;
                    pod.Supplier1UnitCost = m.SupplierPriceLists.Where(x => x.SupplierCode == itemToAdd.Supplier1 && x.ItemCode == itemToAdd.ItemCode).Select(x => x.UnitCost).FirstOrDefault();
                    pod.Supplier2UnitCost = m.SupplierPriceLists.Where(x => x.SupplierCode == itemToAdd.Supplier2 && x.ItemCode == itemToAdd.ItemCode).Select(x => x.UnitCost).FirstOrDefault();
                    pod.Supplier3UnitCost = m.SupplierPriceLists.Where(x => x.SupplierCode == itemToAdd.Supplier3 && x.ItemCode == itemToAdd.ItemCode).Select(x => x.UnitCost).FirstOrDefault();

                    poFullDetailsList.Add(pod);
                }

                Session["newPOList"] = poFullDetailsList;

                //add to list meant for already added items
                List<String> tempList = (List<String>)Session["tempList"];
                tempList.Add(itemToAdd.ItemCode);
                Session["tempList"] = tempList;

                return RedirectToAction("Purchase", "Store");
            }
        }

        [HttpPost]
        public RedirectToRouteResult EditNewPOQty(string data, int index)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                List<POFullDetail> poFullDetailsList = (List<POFullDetail>)Session["newPOList"];
                POFullDetail item = poFullDetailsList.ElementAt(index);
                item.QuantityOrdered = Int32.Parse(data);
                Session["newAdjList"] = poFullDetailsList;

                return RedirectToAction("Purchase", "Store");
            }
        }

        [HttpPost]
        public RedirectToRouteResult EditNewPOSupplier(string data, int index)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                List<POFullDetail> poFullDetailsList = (List<POFullDetail>)Session["newPOList"];
                POFullDetail item = poFullDetailsList.ElementAt(index);
                item.CompanyName = data;
                SupplierPriceList spl = e.SupplierPriceLists.Where(y => y.SupplierCode == item.CompanyName).FirstOrDefault();
                item.UnitCost = spl.UnitCost;
                Session["newAdjList"] = poFullDetailsList;

                return RedirectToAction("Purchase", "Store");
            }
        }

        /************Action methods belonging to Store Purchase - PO List *******************/


        [Route("POList")]
        public ActionResult POList()
        {
            Session["StorePurchaseTabIndex"] = "1";

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
        public ActionResult DisplayPO(FormCollection data)
        {
            Session["POListPage"] = "2";
            string poNumber = data["PONumber"];
            Session["poNumber"] = poNumber;
            using(SSISdbEntities m = new SSISdbEntities())
            {
                List<POFullDetail> poFullDetailList = m.POFullDetails.Where(x => x.PONumber == poNumber).ToList();
                POHeader ph = m.POHeaders.Where(x => x.PONumber == poNumber).FirstOrDefault();
                Session["PORemarks"] = ph.Remarks;

                Session["POItems"] = poFullDetailList;
                Session["poStatus"] = m.POHeaders.Where(x => x.PONumber == poNumber).Select(x => x.Status).FirstOrDefault();
            }

            return View();
        }

        [HttpPost]
        public RedirectToRouteResult BackToPOList()
        {
            Session["POListPage"] = "1";

            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult EditMode()
        {
            Session["poDetailsEditMode"] = true;
            // ----------------------------- Should we add Viewbag for title to differentiate GR / edit ? -------------------------------------------
            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult GREditMode()
        {
            Session["grEditMode"] = true;
            // ----------------------------- Should we add Viewbag for title to differentiate GR / edit ? -------------------------------------------
            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult EditPOQtyOrdered(string data, int index)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                List<POFullDetail> poFullDetailList = (List<POFullDetail>)Session["POItems"];
                POFullDetail item = poFullDetailList.ElementAt(index);
                item.QuantityOrdered = Int32.Parse(data);
                Session["POItems"] = poFullDetailList;

                return RedirectToAction("Requisition", "Dept");
            }
        }


        [HttpPost]
        public RedirectToRouteResult SaveEdit(string[] arrQty, string taData)
        {
// --------------------- Validation required ----------------------
            Session["poDetailsEditMode"] = false;

            List<POFullDetail> poFullDetailList = (List<POFullDetail>)Session["POItems"];
            string poNumber = (string)Session["poNumber"];
            int arrayCount = 0;
            using (SSISdbEntities m = new SSISdbEntities())
            {
                foreach (POFullDetail p in poFullDetailList)
                {
                    PODetail pod = m.PODetails.Where(x => x.ItemCode == p.ItemCode && x.PONumber == p.PONumber).FirstOrDefault();
                    pod.QuantityBackOrdered = Convert.ToInt32(arrQty[arrayCount]);
                    pod.QuantityOrdered = pod.QuantityBackOrdered;
                    m.SaveChanges();
                    arrayCount++;

                    p.QuantityOrdered = pod.QuantityBackOrdered;
                }
                if (taData != null)
                {
                    POHeader ph = m.POHeaders.Where(x => x.PONumber == poNumber).FirstOrDefault();
                    ph.Remarks = taData;
                    m.SaveChanges();
                    Session["PORemarks"] = taData;
                }
            }
            Session["POItems"] = poFullDetailList;
            Session["POListPage"] = "2";

            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult ExitEditMode()
        {
            Session["poDetailsEditMode"] = false;
            Session["grEditMode"] = false;
            Session["POListPage"] = "2";

            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public ActionResult CancelPO()
        {
            string poNumber = (string)Session["poNumber"];
            using (SSISdbEntities m = new SSISdbEntities())
            {
                POHeader poHeader = m.POHeaders.Where(x => x.PONumber == poNumber).FirstOrDefault();
                poHeader.Status = "Cancelled";
                m.SaveChanges();

                List<POFullDetail> poFullDetailsList = (List<POFullDetail>)Session["POItems"];
                List<PODetail> podItems = m.PODetails.Where(x => x.PONumber == poNumber).ToList();
                foreach (POFullDetail p in poFullDetailsList)
                {
                    foreach (PODetail pod in podItems)
                    {
                        if (pod.ItemCode == p.ItemCode)
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
            Session["poStatus"] = "Cancelled";
            Session["POListPage"] = "2";

            return RedirectToAction("Purchase", "Store");
        }

        /************Action methods belonging to Store Purchase - GR *******************/


        [Route("GoodsReceivedList")]
        public ActionResult GoodsReceivedList()
        {
            Session["StorePurchaseTabIndex"] = "3";

            if (Session["GRListPage"].ToString() == "1")
            {
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
            using (SSISdbEntities e = new SSISdbEntities()) {
                Session["poNumber"] = e.GoodsReceivedLists.Where(x => x.ReceiptNo == rNo).Select(x => x.PONumber).FirstOrDefault();
            }

            return RedirectToAction("Purchase", "Store");
        }
        
        [HttpPost]
        public RedirectToRouteResult BackToGRList()
        {
            Session["GRListPage"] = "1";

            return RedirectToAction("Purchase", "Store");
        }
        
        
        [HttpPost]
        public ActionResult GoodsReceipt(string[] arrQty)
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
            bool invalid = false;

            int c = 0;

            try
            {
                using (SSISdbEntities m = new SSISdbEntities())
                {
                    // Validation
                    foreach (POFullDetail p in poFullDetailList)
                    {
                        if ((p.QuantityOrdered - p.QuantityDelivered) < Convert.ToInt32(arrQty[c]) || Convert.ToInt32(arrQty[c]) < 0)
                        {
                            invalid = true;
                        }
                        c++;
                    }

                    if (invalid == false)
                    {
                        int newId = m.POReceiptHeaders.Count() + 1;
                        receiptNo = CommonLogic.SerialNo(newId, "POR");
                        int countQty = 0;
                        int countInventory = 0;

                        // Tabulate all qty, to ensure total is not 0 (Prevent empty entries)
                        for (int i = 0; i < arrQty.Length; i++)
                        {
                            countQty += Convert.ToInt32(arrQty[i]);
                        }

                        // Create New PO Receipt Header
                        if (countQty > 0)
                        {
                            // Preparation for database update
                            foreach (POFullDetail p in poFullDetailList)
                            {
                                PODetail pod = m.PODetails.Where(x => x.ItemCode == p.ItemCode && x.PONumber == p.PONumber).FirstOrDefault();
                                totalInventoryValue += pod.UnitCost;
                            }

                            // Create PO Receipt Header first (Due to Foreign Key exceptions)
                            POReceiptHeader porh = new POReceiptHeader();
                            porh.ReceiptNo = receiptNo;
                            porh.PONumber = poNumber;
                            // ---------------------------------------------------- Need to change this --------------------------------------
                            porh.DeliveryOrderNo = "Hardcode first";
                            porh.ReceivedDate = DateTime.Now;
                            porh.Receiver = (string)Session["LoginEmployeeID"];
                            porh.Remarks = "";
                            porh.TransactionType = "PO Receipt";
                            porh.TotalAmount = totalInventoryValue;
                            m.POReceiptHeaders.Add(porh);
                            m.SaveChanges();
                            TempData["ReceiptNo"] = receiptNo;

                            // Adding into Database based on Items
                            foreach (POFullDetail p in poFullDetailList)
                            {
                                PODetail pod = m.PODetails.Where(x => x.ItemCode == p.ItemCode && x.PONumber == p.PONumber).FirstOrDefault();
                                int qty = Convert.ToInt32(arrQty[arrayCount]);

                                // Only execute if qty > 0, meaning there's good received.
                                if (qty > 0 && (pod.QuantityOrdered - pod.QuantityDelivered) != 0)
                                {
                                    // Update PO Details Table
                                    if (pod.QuantityBackOrdered == qty)
                                    {
                                        pod.QuantityBackOrdered = 0;

                                    }
                                    else
                                    {
                                        pod.QuantityBackOrdered = pod.QuantityBackOrdered - qty;
                                    }
                                    pod.QuantityDelivered += qty;
                                    m.SaveChanges();

                                    //totalInventoryValue += qty * p.UnitCost;

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

                            // Checking for completion of PO 
                            foreach (POFullDetail p in poFullDetailList)
                            {
                                PODetail pod = m.PODetails.Where(x => x.ItemCode == p.ItemCode && x.PONumber == p.PONumber).FirstOrDefault();
                                countInventory += pod.QuantityBackOrdered;
                            }


                            POHeader poh = m.POHeaders.Where(x => x.PONumber == poNumber).FirstOrDefault();
                            // Completed PO
                            if (countInventory == 0)
                            {
                                poh.Status = "Completed";
                            }
                            else
                            {
                                poh.Status = "Outstanding";
                            }
                            m.SaveChanges();
                            Session["GRListPage"] = "2";
                            Session["poStatus"] = "Completed";
                        }
                    }
                    else
                    {
                        // ---------------------------- Need some sort of error message here -------------------------------------------
                        ViewBag.ErrorAmt = "Invalid Quantity!";
                        Session["StorePurchaseTabIndex"] = "2";
                        return View("DisplayPO");
                    }
                }
            }
            catch(Exception)
            {
                TempData["ErrorMsg"] = "Please enter a valid number!";
                Session["StorePurchaseTabIndex"] = "2";
                return View("DisplayPO");
            }
            
            Session["POItems"] = poFullDetailList;
            Session["poDetailsEditMode"] = false;
            Session["grId"] = receiptNo;
            Session["StorePurchaseTabIndex"] = "3";

            return RedirectToAction("DisplayGR", "StorePurchase");

        }

        public ActionResult ExportPO()
        {
            List<POFullDetail> poFullDetailList = (List<POFullDetail>)Session["POItems"];

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "Frm_PurchaseOrder.rpt"));
            rd.SetDataSource(poFullDetailList);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Frm_PurchaseOrder.pdf");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        

    }
}
