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
            List<POFullDetail> itemList = new List<POFullDetail>();
            List<PODetail> poDetailsList = (List<PODetail>)Session["newPOList"];
            List<Supplier> supplierList = new List<Supplier>();
            List<Item> itemAdded = new List<Item>();
            int arrayCount = 0;
            using (SSISdbEntities m = new SSISdbEntities())
            {
                foreach (PODetail p in poDetailsList)
                {
                    // Adding <input> Qty & <select> Supplier into poDetailsList 
                    // Finalize PODetailsList
                    p.QuantityOrdered = Convert.ToInt32(arrQty[arrayCount]);
                    string coy = arrSupplier[arrayCount];
                    Supplier sup = m.Suppliers.Where(x => x.SupplierCode == coy).FirstOrDefault();
                    p.Item.Supplier = sup;

                    // Grouping Suppliers & Extract Items out of PODetailsList
                    string supCode = p.Item.Supplier.SupplierCode;
                    Supplier supplier = m.Suppliers.Where(x => x.SupplierCode == supCode).FirstOrDefault();
                    if (!supplierList.Contains(supplier))
                    {
                        supplierList.Add(supplier);
                    }

                    //POFullDetail f = new POFullDetail();
                    //f.ItemCode = p.ItemCode;
                    //f.QuantityOrdered = p.QuantityOrdered;
                    //f.UnitCost = p.UnitCost;
                    //itemList.Add(f);
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

// ---------------------------------- IMPORTANT : Need to change this based on Sesson Role ---------------------------------------------//
                    newPOHeader.EmployeeID = "E1";
// --------------------------------------------- IMPORTANT : Need to change this ------------------------------------------------------//
                    newPOHeader.Remarks = "";
// ------------------------------------------------------------------------------------------------------------------------------------//

                    newPOHeader.Status = "Open";
                    newPOHeader.TransactionType = "PO";
                    m.POHeaders.Add(newPOHeader);
                    m.SaveChanges();

                    // Loop through PODetails to check suppliers
                    foreach (PODetail pod in poDetailsList)
                    {
                        // Create PO Details, Line by line based on items
                        string supCode = pod.Item.Supplier.SupplierCode;
                        Supplier supplier = m.Suppliers.Where(x => x.SupplierCode == supCode).FirstOrDefault();
                        if (supplier.Equals(s))
                        {
// ---------------------------------- IMPORTANT : Not sure this is 100% working ---------------------------------------------//
                            if (!itemAdded.Contains(pod.Item))
                            {
                                // Need to double check after changing UI
                                PODetail poDetailToAdd = new PODetail();
                                float itemUnitPrice = m.SupplierPriceLists.Where(x => x.SupplierCode == supplier.SupplierCode
                                    && x.ItemCode == pod.Item.ItemCode).Select(y => y.UnitCost).FirstOrDefault();
                                poDetailToAdd.PONumber = poId;
                                poDetailToAdd.ItemCode = pod.ItemCode;
                                poDetailToAdd.QuantityOrdered = pod.QuantityOrdered;
                                poDetailToAdd.QuantityBackOrdered = pod.QuantityOrdered;
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
            }
            Session["newPOList"] = new List<PODetail>();
            return View();
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
                }
            }
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
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult GoodsReceipt()
        {
            return null;
        }
    }
}
