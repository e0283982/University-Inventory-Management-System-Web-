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
            //using(SSISdbEntities m = new SSISdbEntities())
            //{
            //    List<StaffRequisitionHeader> staffReqHeadList = m.StaffRequisitionHeaders
            //        .Where(x => x.Status == "Open" && x.ApprovalStatus == "Approved").ToList();
            //    List<StaffRequisitionHeader> staffRetrievalList = new List<StaffRequisitionHeader>();
            //    List<string> formIdList = new List<string>();
            //    List<string> itemCodeList = new List<string>();
            //    List<int> qtyList = new List<int>();
            //    List<string> TitemCodeList = new List<string>();
            //    List<int> TqtyList = new List<int>();
            //    int[] qtys = new int[] { };
            //    List<Department> deptList = new List<Department>();
            //    List<StaffRequisitionDetail> staffRequisitionDetailsList = new List<StaffRequisitionDetail>();
            //    // Only loop those with open & Approved
            //    foreach (StaffRequisitionHeader srh in staffReqHeadList)
            //    {
            //        // Convert Dates
            //        DateTime date = (DateTime) srh.DateProcessed;
            //        DateTime convertedDate = date.Date;
            //        Debug.WriteLine(date.ToString());
            //        DateTime validDate = DateTime.Now.AddDays(-7);
            //        Debug.WriteLine(validDate.ToString());
            //        int dateCompare = DateTime.Compare(validDate, convertedDate);
            //        Debug.WriteLine(dateCompare);

            //        // Only collate those new ones (>7 Days with Open means disbursed but not collected)
            //        if(dateCompare < 7)
            //        {
            //            formIdList.Add(srh.FormID);
            //        }

            //        Department department = m.Departments.Where(x => x.DepartmentCode == srh.DepartmentCode).FirstOrDefault();
            //        if (!deptList.Contains(department))
            //        {
            //            deptList.Add(department);
            //        }
            //    }

            //    int arrayCount = 0;
            //    // Search for All relevant StaffRequisitionDetail
            //    foreach(string s in formIdList)
            //    {
            //        StaffRequisitionDetail srd = m.StaffRequisitionDetails.Where(x => x.FormID == s).FirstOrDefault();
            //        staffRequisitionDetailsList.Add(srd);
            //        itemCodeList.Add(srd.ItemCode);
            //        qtys[arrayCount] = srd.QuantityBackOrdered;
            //        arrayCount++;
            //    }

            //    arrayCount = 0;

            //    // Collate Item code with values
            //    foreach(string s in itemCodeList)
            //    {
            //        if (!TitemCodeList.Contains(s))
            //        {
            //            // If new ItemCode
            //            TitemCodeList.Add(s);
            //            qtyList.Add(qtys[arrayCount]);
            //        }
            //        else
            //        {
            //            // If ItemCode exist, replace value & add more qty into the list
            //            int i = itemCodeList.IndexOf(s);
            //            int q = qtys[i] + qtys[arrayCount];
            //            qtyList.RemoveAt(i);
            //            qtyList.Insert(i, q);
            //        }
            //        arrayCount++;
            //    }

            //    arrayCount = 0;

            //    foreach(Department d in deptList)
            //    {
            //        // Create StockRetrievalHeader
            //        StockRetrievalHeader newsrh = new StockRetrievalHeader();
            //        int newSRH = m.StockRetrievalHeaders.Count() + 1;
            //        string srhId = "StoR-" + newSRH.ToString();
            //        newsrh.ID = srhId;
            //        newsrh.Date = DateTime.Now;
            //        newsrh.Disbursed = 0;
            //        m.SaveChanges();

            //        foreach(StaffRequisitionDetail s in staffRequisitionDetailsList)
            //        {
            //            StockRetrievalDetail srd = new StockRetrievalDetail();
            //            srd.Id = srhId;
            //            StaffRequisitionHeader sh = m.StaffRequisitionHeaders
            //                .Where(x => x.DepartmentCode == d.DepartmentCode && x.FormID == s.FormID).FirstOrDefault();
            //            if(sh.DepartmentCode == d.DepartmentCode)
            //            {
            //                srd.ItemCode = s.ItemCode;
            //                srd.QuantityRetrieved = 0;

            //            }

            //        }

            //    }
            //}
            return RedirectToAction("Home", "Store");
        }


    }
}
