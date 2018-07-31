using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using SA46Team1_Web_ADProj.DAL;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class StoreController : Controller
    {
        private ItemsRepositoryImpl itemRepository;

        public StoreController()
        {
            this.itemRepository = new ItemsRepositoryImpl(new SSISdbEntities());
        }

        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Home()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                List<StockAdjustmentFullDetail> list = new List<StockAdjustmentFullDetail>();
                string role = (string)Session["Role"];
                string empId = (string)Session["LoginEmployeeID"];
                if(role == "Store Clerk")
                {
                    list = m.StockAdjustmentFullDetails.Where(x => (x.Status == "Approved" && x.NotificationStatus == "Unread")
                    || (x.Status == "Rejected" && x.NotificationStatus == "Unread")).ToList();
                }
                else
                if(role == "Store Supervisor")
                {
                    list = m.StockAdjustmentFullDetails.Where(x => x.Status == "Pending" 
                    && x.NotificationStatus == "Unread" && x.Amount < 250).ToList();
                }
                else
                {
                    list = m.StockAdjustmentFullDetails.Where(x => x.Status == "Pending" 
                    && x.NotificationStatus == "Unread" && x.Amount >= 250).ToList();
                }

                int UnreadPendingApprovalsCount = list.Count();
                Session["NoUnreadRequests"] = UnreadPendingApprovalsCount;
            }

            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public JsonResult Search(string search)
        {
            return new JsonResult { Data = itemRepository.GetItemById(search), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Inventory()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Disbursements()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Purchase()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Report()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Store Manager, Store Supervisor")]
        public ActionResult Maintenance()
        {
            return View();
        }

        [Authorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Notifications()
        {
            return View();
        }

        [Authorize(Roles = "Store Manager, Store Supervisor")]
        public ActionResult Approval()
        {
            return View();
        }

        public RedirectToRouteResult Test()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                #region Variables
                // List of StaffReq with Open & Approved
                List<StaffRequisitionHeader> staffReqHeadList = m.StaffRequisitionHeaders
                        .Where(x => x.Status == "Open" && x.ApprovalStatus == "Approved").ToList();
                // Prep List for Date sorting (within the week)
                List<StaffRequisitionHeader> SRHForTheWeek = new List<StaffRequisitionHeader>();
                
                // Loop through Open & Approve for Dept count & List of StaffReq within week
                Dictionary<Item, int> itemAndQty = new Dictionary<Item, int>();
                List<StaffRequisitionDetail> srdList = new List<StaffRequisitionDetail>();
                
                // ------------------------------------------ Might need to change this condition ---------------------------------------------
                List<StaffRequisitionDetail> srdWithBackOrders = m.StaffRequisitionDetails.Where(x => x.QuantityBackOrdered > 0).ToList();
                Dictionary<Item, List<string>> listOfItemsAndDeptAdded = new Dictionary<Item, List<string>>();
                List<string> deptCodes = new List<string>();
                List<StaffRequisitionDetail> allsrd = new List<StaffRequisitionDetail>();
                #endregion

                #region Prepare new StaffRequisitionDetails to add
                foreach (StaffRequisitionHeader srh in staffReqHeadList)
                {
                    // Convert Dates
                    DateTime date = (DateTime)srh.DateProcessed;
                    DateTime convertedDate = date.Date;
                    DateTime validDate = DateTime.Now.Date.AddDays(-7);
                    double dateCompare = (convertedDate - validDate).TotalDays;
                    // Only collate those new ones (>7 Days with Open means disbursed but not collected)
                    if (dateCompare < 7)
                    {
                        SRHForTheWeek.Add(srh);
                    }
                }

                // Only add StaffRequisitionDetails for the week into the list (Past week are for backorders only)
                foreach(StaffRequisitionHeader srhInWeek in SRHForTheWeek)
                {
                    // Find List of StaffReqDetails based on Header ID
                    StaffRequisitionDetail SRD = m.StaffRequisitionDetails.Where(x => x.FormID == srhInWeek.FormID).FirstOrDefault();
                    if (SRD != null && !srdList.Contains(SRD))
                    {
                        srdList.Add(SRD);
                    }
                }
                #endregion

                // Check if there's any entries to collate
                if (srdList != null && srdWithBackOrders != null)
                {
                    #region Create StockRetrievalHeader
                    // Create StockRetrievalHeader
                    StockRetrievalHeader newsrh = new StockRetrievalHeader();
                    int newSRH = m.StockRetrievalHeaders.Count() + 1;
                    string srhId = CommonLogic.SerialNo(newSRH, "StoR");
                    newsrh.ID = srhId;
                    newsrh.Date = DateTime.Now;
                    newsrh.Disbursed = 0;
                    m.StockRetrievalHeaders.Add(newsrh);
                    m.SaveChanges();
                    #endregion

                    #region Confirming how many Items & quantity to collate
                    // Collate items based on Backorder list
                    if (srdWithBackOrders != null)
                    {
                        foreach (StaffRequisitionDetail srdlist in srdWithBackOrders)
                        {
                            Item item = m.Items.Where(x => x.ItemCode == srdlist.ItemCode).FirstOrDefault();
                            if (!itemAndQty.ContainsKey(item))
                            {
                                itemAndQty.Add(item, srdlist.QuantityBackOrdered);
                            }
                            else
                            {
                                itemAndQty[item] += srdlist.QuantityBackOrdered;
                            }
                        }
                    }

                    // Collate items based on SRD
                    if (srdList != null)
                    {
                        foreach (StaffRequisitionDetail srdlist in srdList)
                        {
                            Item item = m.Items.Where(x => x.ItemCode == srdlist.ItemCode).FirstOrDefault();
                            if (!itemAndQty.ContainsKey(item))
                            {
                                itemAndQty.Add(item, srdlist.QuantityOrdered);
                            }
                            else
                            {
                                itemAndQty[item] += srdlist.QuantityOrdered;
                            }
                        }
                    }

                    // Check if quantity is sufficient
                    foreach (Item itemToCollate in itemAndQty.Keys.ToList())
                    {
                        if (itemToCollate.Quantity > itemAndQty[itemToCollate])
                        {
                            itemAndQty[itemToCollate] = itemToCollate.Quantity;
                        }
                    }
                    #endregion

                    #region Fulfill Backorders
                    // Fulfill all backorders first
                    if (srdWithBackOrders != null)
                    {
                        foreach (StaffRequisitionDetail retrievalListWithBO in srdWithBackOrders)
                        {
                            Item itemToRetrieve = m.Items.Where(x => x.ItemCode == retrievalListWithBO.ItemCode).FirstOrDefault();
                            int qtyAvailable = itemAndQty[itemToRetrieve];

                            // Only create entries with qty available
                            if (qtyAvailable > 0)
                            {
                                // Create quantity requested that is really available for that item
                                int qtyToAdd = 0;
                                if (qtyAvailable > retrievalListWithBO.QuantityBackOrdered)
                                {
                                    qtyToAdd = retrievalListWithBO.QuantityBackOrdered;
                                }
                                else
                                {
                                    qtyToAdd = qtyAvailable;
                                }

                                StockRetrievalDetail newsrd = new StockRetrievalDetail();
                                Bin bin = m.Bins.Where(x => x.ItemCode == itemToRetrieve.ItemCode).FirstOrDefault();
                                StaffRequisitionHeader srhDeptCode = m.StaffRequisitionHeaders
                                    .Where(x => x.FormID == retrievalListWithBO.FormID).FirstOrDefault();
                                DepartmentDetail dd = m.DepartmentDetails
                                    .Where(x => x.DepartmentCode == srhDeptCode.DepartmentCode).FirstOrDefault();
                                newsrd.Id = srhId;
                                newsrd.Bin = bin.Number;
                                newsrd.ItemCode = itemToRetrieve.ItemCode;
                                newsrd.QuantityRetrieved = qtyToAdd;
                                newsrd.CollectionPointID = dd.CollectionPointID;
                                newsrd.DepartmentCode = srhDeptCode.DepartmentCode;
                                newsrd.QuantityAdjusted = 0;
                                newsrd.Remarks = "";
                                m.StockRetrievalDetails.Add(newsrd);
                                m.SaveChanges();

                                //Remove quantity added from dictionary
                                itemAndQty[itemToRetrieve] -= qtyToAdd;

                                // Prepare list for iteration of repeated entries 
                                if (!deptCodes.Contains(srhDeptCode.DepartmentCode))
                                {
                                    deptCodes.Add(srhDeptCode.DepartmentCode);
                                }

                                if (!listOfItemsAndDeptAdded.ContainsKey(itemToRetrieve))
                                {
                                    listOfItemsAndDeptAdded.Add(itemToRetrieve, deptCodes);
                                }
                                else
                                {
                                    listOfItemsAndDeptAdded[itemToRetrieve] = deptCodes;
                                }

                                allsrd.Add(retrievalListWithBO);
                            }
                        }
                    }
                    #endregion

                    #region Fulfill new entries
                    // Create StockRetrievalDetails
                    foreach(StaffRequisitionDetail retrievalList in srdList)
                    {
                        Item itemRetrieved = m.Items.Where(x => x.ItemCode == retrievalList.ItemCode).FirstOrDefault();
                        Bin bin = m.Bins.Where(x => x.ItemCode == retrievalList.ItemCode).FirstOrDefault();
                        StaffRequisitionHeader srhDeptCode = m.StaffRequisitionHeaders
                            .Where(x => x.FormID == retrievalList.FormID).FirstOrDefault();
                        DepartmentDetail dd = m.DepartmentDetails
                            .Where(x => x.DepartmentCode == srhDeptCode.DepartmentCode).FirstOrDefault();
                        StockRetrievalDetail newsrd = new StockRetrievalDetail();
                        int qtyAvailable = itemAndQty[itemRetrieved];

                        // Only create entries with qty available
                        if(qtyAvailable > 0)
                        {
                            // Create quantity requested that is really available for that item
                            int qtyToAdd = 0;
                            if(qtyAvailable > retrievalList.QuantityOrdered)
                            {
                                qtyToAdd = retrievalList.QuantityOrdered;
                            }
                            else
                            {
                                qtyToAdd = qtyAvailable;
                            }

                            if (!listOfItemsAndDeptAdded[itemRetrieved].Contains(srhDeptCode.DepartmentCode))
                            {
                                // If its an entirely new entry (No backorders for that week)
                                newsrd.Id = srhId;
                                newsrd.Bin = bin.Number;
                                newsrd.ItemCode = retrievalList.ItemCode;
                                newsrd.QuantityRetrieved = qtyToAdd;
                                newsrd.CollectionPointID = dd.CollectionPointID;
                                newsrd.DepartmentCode = srhDeptCode.DepartmentCode;
                                newsrd.QuantityAdjusted = 0;
                                newsrd.Remarks = "";
                                m.StockRetrievalDetails.Add(newsrd);
                                allsrd.Add(retrievalList);
                            }
                            else
                            {
                                // If there's existing Back orders, we just update the quantity
                                StockRetrievalDetail existingSRD = m.StockRetrievalDetails
                                    .Where(x => x.Id == srhId && x.ItemCode == itemRetrieved.ItemCode
                                    && x.DepartmentCode == srhDeptCode.DepartmentCode).FirstOrDefault();
                                existingSRD.QuantityRetrieved += qtyToAdd;
                            }

                            m.SaveChanges();
                            itemAndQty[itemRetrieved] -= qtyToAdd;
                        }

                    }
                    #endregion

                    #region Create StockRetrievalReqForm
                    foreach(StaffRequisitionDetail allNewlyCreated in allsrd)
                    {
                        // New StockRetrievalReqForm
                        StockRetrievalReqForm srrf = new StockRetrievalReqForm();
                        srrf.ReqFormID = allNewlyCreated.FormID;
                        srrf.StockRetrievalID = srhId;
                        m.StockRetrievalReqForms.Add(srrf);
                        m.SaveChanges();
                    }
                    #endregion

                }







            }
            return RedirectToAction("Home", "Store");
        }

    }
}
