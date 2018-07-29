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

        [Authorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Home()
        {
            return View();
        }
        [Authorize(Roles = "Store Clerk, Store Manager")]
        public JsonResult Search(string search)
        {
            return new JsonResult { Data = itemRepository.GetItemById(search), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Inventory()
        {
            return View();
        }
        [Authorize(Roles = "Store Clerk, Store Manager")]
        public ActionResult Disbursements()
        {
            return View();
        }
        [Authorize(Roles = "Store Clerk, Store Manager")]
        public ActionResult Purchase()
        {
            return View();
        }
        [Authorize(Roles = "Store Clerk, Store Manager")]
        public ActionResult Report()
        {
            return View();
        }

        [Authorize(Roles = "Store Manager")]
        public ActionResult Maintenance()
        {
            return View();
        }
        
        public RedirectToRouteResult Test()
        {
            using(SSISdbEntities m = new SSISdbEntities())
            {
                // List of StaffReq with Open & Approved
                List<StaffRequisitionHeader> staffReqHeadList = m.StaffRequisitionHeaders
                        .Where(x => x.Status == "Open" && x.ApprovalStatus == "Approved").ToList();
                List<Department> deptList = new List<Department>();

                // Prep List for Date sorting (within the week)
                List<StaffRequisitionHeader> finalSRH = new List<StaffRequisitionHeader>();

                // Loop through Open & Approve for Dept count & List of StaffReq within week
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
                        finalSRH.Add(srh);
                        // To know how many StoR IDs to create
                        Department department = m.Departments.Where(x => x.DepartmentCode == srh.DepartmentCode).FirstOrDefault();
                        if (!deptList.Contains(department))
                        {
                            deptList.Add(department);
                        }
                    }
                }

                // Creating entries based on No. of Dept
                List<StaffRequisitionHeader> deptSRH;
                foreach(Department dept in deptList)
                {
                    // Refresh List for every department, therefore creating different entry in database based on department
                    deptSRH = new List<StaffRequisitionHeader>();

                    // New StaffReq list based on Department
                    foreach(StaffRequisitionHeader deptSRHList in finalSRH)
                    {
                        StaffRequisitionHeader dSRH = m.StaffRequisitionHeaders
                            .Where(x => x.DepartmentCode == dept.DepartmentCode && deptSRHList.FormID == x.FormID).FirstOrDefault();
                        if(dSRH != null)
                        {
                            deptSRH.Add(dSRH);
                        }
                    }

                    // Create StockRetrievalHeader
                    StockRetrievalHeader newsrh = new StockRetrievalHeader();
                    int newSRH = m.StockRetrievalHeaders.Count() + 1;
                    string srhId = "StoR-" + newSRH.ToString();
                    newsrh.ID = srhId;
                    newsrh.Date = DateTime.Now;
                    newsrh.Disbursed = 0;
                    m.StockRetrievalHeaders.Add(newsrh);
                    m.SaveChanges();

                    // Adding database entries based on Department requisition
                    foreach (StaffRequisitionHeader deptSRHFinalList in deptSRH)
                    {
                        // For each new StoR id, create entries based on item
                        List<StaffRequisitionDetail> staffRd = m.StaffRequisitionDetails.Where(x => x.FormID == deptSRHFinalList.FormID).ToList();
                        foreach(StaffRequisitionDetail sRd in staffRd)
                        {
                            StockRetrievalDetail newsrd = new StockRetrievalDetail();
                            Bin bin = m.Bins.Where(x => x.ItemCode == sRd.ItemCode).FirstOrDefault();
                            DepartmentDetail dd = m.DepartmentDetails.Where(x => x.DepartmentCode == dept.DepartmentCode).FirstOrDefault();
                            newsrd.Id = srhId;
                            newsrd.Bin = bin.Number;
                            newsrd.ItemCode = sRd.ItemCode;
                            newsrd.QuantityRetrieved = 0;
                            newsrd.CollectionPointID = dd.CollectionPointID;
                            newsrd.DepartmentCode = dept.DepartmentCode;
                            newsrd.QuantityAdjusted = 0;
                            newsrd.Remarks = "";
                            m.StockRetrievalDetails.Add(newsrd);
                            m.SaveChanges();
                        }

                        // New StockRetrievalReqForm based on Dept
                        StockRetrievalReqForm srrf = new StockRetrievalReqForm();
                        srrf.ReqFormID = deptSRHFinalList.FormID;
                        srrf.StockRetrievalID = srhId;
                        m.StockRetrievalReqForms.Add(srrf);
                        m.SaveChanges();
                    }
                }

            }
            return RedirectToAction("Home", "Store");
        }


    }
}
