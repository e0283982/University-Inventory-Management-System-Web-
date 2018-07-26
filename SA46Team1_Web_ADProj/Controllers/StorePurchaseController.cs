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
        public RedirectToRouteResult DeletePOItem(FormCollection data)
        {
            string itemCode = data["0"];
            List<PODetail> poFullDetailsList = (List<PODetail>)Session["newPOList"];
            PODetail pod = new PODetail();
            foreach(PODetail p in poFullDetailsList)
            {
                if(p.ItemCode == itemCode)
                {
                    pod = p;
                }
            }
            poFullDetailsList.Remove(pod);
            Session["newPOList"] = poFullDetailsList;
            return RedirectToAction("Purchase", "Store");
        }

        [HttpPost]
        public ActionResult SavePO()
        {
            List<POFullDetail> itemList = new List<POFullDetail>();
            List<PODetail> poDetailsList = (List<PODetail>) Session["newPOList"];
            List<Supplier> supplierList = new List<Supplier>();
            List<Item> itemAdded = new List<Item>();

            using (SSISdbEntities m = new SSISdbEntities())
            {
                // Grouping Suppliers & Extract Items out of PODetailsList
                foreach (PODetail p in poDetailsList)
                {
                    Supplier supplier = m.Suppliers.Where(x => x.SupplierCode == p.Item.Supplier1).FirstOrDefault();
                    if (!supplierList.Contains(supplier))
                    {
                        supplierList.Add(supplier);
                    }
                    POFullDetail f = new POFullDetail();
                    f.ItemCode = p.ItemCode;
                    f.QuantityOrdered = p.QuantityOrdered;
                    f.UnitCost = p.UnitCost;
                    itemList.Add(f);
                }

                // Each Supplier iterates once such that only 1 PO is created for each of them
                foreach (Supplier s in supplierList)
                {
                    // Create new PO based on supplier
                    int count = m.POHeaders.Count();
                    string poId = "PO-" + count.ToString();
                    
                    POHeader newPOHeader = new POHeader();
                    newPOHeader.PONumber = poId;
                    newPOHeader.Date = DateTime.Now;
                    newPOHeader.SupplierCode = s.SupplierCode;
                    newPOHeader.ContactName = s.ContactName;
                    newPOHeader.DeliverTo = "Logic University";
                    newPOHeader.EmployeeID = "E1";
                    newPOHeader.Remarks = "";
                    newPOHeader.Status = "Open";
                    newPOHeader.TransactionType = "PO";
                    m.POHeaders.Add(newPOHeader);
                    m.SaveChanges();

                    // Loop through PODetails to check suppliers
                    foreach (PODetail pod in poDetailsList)
                    {
                        // Create PO Details, Line by line based on items
                        Supplier supplier = m.Suppliers.Where(x => x.SupplierCode == pod.Item.Supplier1).FirstOrDefault();
                        if (supplier.Equals(s))
                        {
                                if (!itemAdded.Contains(pod.Item))
                                {
                                    // Need to double check after changing UI
                                    PODetail poDetailToAdd = new PODetail();
                                    float itemUnitPrice = m.SupplierPriceLists.Where(x => x.SupplierCode == supplier.SupplierCode 
                                        && x.ItemCode == pod.Item.ItemCode).Select(y=> y.UnitCost).FirstOrDefault();
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
                string itemCode = Request.Form["SelectItemChose"].ToString();
                List<PODetail> poFullDetailsList = (List<PODetail>)Session["newPOList"];
                PODetail pod = new PODetail();
                PODetail existingPoD = new PODetail();
                pod.PONumber = (string)Session["newPoId"];
                    foreach(PODetail p in poFullDetailsList.ToList())
                    {
                        if(p.ItemCode == itemCode)
                        {
                            existingItem = true;
                            existingPoD = p;
                            existingPoD.QuantityOrdered += item2.QuantityOrdered;
                            poFullDetailsList.Remove(p);
                            poFullDetailsList.Add(existingPoD);
                        }
                    }
                if(existingItem == false)
                {
                    itemToAdd = m.Items.Where(x => x.ItemCode == itemCode).FirstOrDefault();
                    pod.ItemCode = itemToAdd.ItemCode;
                    pod.QuantityOrdered = item2.QuantityOrdered;
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
            Session["poNumber"] = data["PONumber"];
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

    }
}
