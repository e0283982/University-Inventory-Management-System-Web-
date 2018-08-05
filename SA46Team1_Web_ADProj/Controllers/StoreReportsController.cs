using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using SA46Team1_Web_ADProj.Models;
using System.IO;
using Newtonsoft.Json;

namespace SA46Team1_Web_ADProj.Controllers
{
    [CustomAuthorize(Roles = "Store Manager, Store Supervisor, Store Clerk")]
    [RoutePrefix("Store/StoreReports")]
    public class StoreReportsController : Controller
    {
        [Route("RptInventory")]
        public ActionResult RptInventory()
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                ViewBag.ItemsList = new SelectList((from s in e.Items.ToList()
                                                    select new
                                                    {
                                                        ItemCode = s.ItemCode,
                                                        Description = s.Description + " (" + s.UoM + ")"
                                                    }),
                                                "ItemCode",
                                                "Description",
                                                null);

                ViewBag.CategoryList = new SelectList((from s in e.Categories.ToList()
                                                       select new
                                                       {
                                                           CategoryID = s.CategoryID,
                                                           CategoryName = s.CategoryName
                                                       }),
                                                 "CategoryID",
                                                 "CategoryName",
                                                 null);

                TempData["RowIndexesToDiscard"] = new List<int>();

                return View();
            }
        }

        public ActionResult ExportRptInventory()
        {
            List<InventoryValuationReport> allRptInventory = (List<InventoryValuationReport>)TempData["allRptInventory"];
            if (allRptInventory == null) {
                using (SSISdbEntities e = new SSISdbEntities()) {
                    allRptInventory = e.InventoryValuationReports.ToList();
                }
            }
            
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RptInventoryValuation.rpt"));
            rd.SetDataSource(allRptInventory);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "InventoryValuation.pdf");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ExportRptInventory2(String[] arr)
        {

            List<InventoryValuationReport> allRptInventory = new List<InventoryValuationReport>();

            foreach (String s in arr)
            {
                InventoryValuationReport rp = JsonConvert.DeserializeObject<InventoryValuationReport>(s);
                allRptInventory.Add(rp);
            }

            TempData["allRptInventory"] = allRptInventory;

            return RedirectToAction("Report", "Store");

        }

        [Route("RptReorder")]
        public ActionResult RptReorder()
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {

                ViewBag.ItemsList = new SelectList((from s in e.Items.ToList()
                                                    select new
                                                    {
                                                        ItemCode = s.ItemCode,
                                                        Description = s.Description + " (" + s.UoM + ")"
                                                    }),
                                                 "ItemCode",
                                                 "Description",
                                                 null);

                ViewBag.CategoryList = new SelectList((from s in e.Categories.ToList()
                                                       select new
                                                       {
                                                           CategoryID = s.CategoryID,
                                                           CategoryName = s.CategoryName
                                                       }),
                                                 "CategoryID",
                                                 "CategoryName",
                                                 null);

                TempData["RowIndexesToDiscard"] = new List<int>();
                return View();
            }
        }

        public ActionResult ExportRptReorder()
        {
            List<ReorderReport> allRptReorders = (List<ReorderReport>)TempData["allRptReorders"];
            if (allRptReorders == null)
            {
                using (SSISdbEntities e = new SSISdbEntities())
                {
                    allRptReorders = e.ReorderReports.ToList();
                }
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RptReorder.rpt"));
            rd.SetDataSource(allRptReorders);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "ReorderReport.pdf");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public RedirectToRouteResult ExportRptReorder2(String[] arr)
        {

            List<ReorderReport> allRptReorders = new List<ReorderReport>();

            foreach (String s in arr)
            {
                ReorderReport rp = JsonConvert.DeserializeObject<ReorderReport>(s);
                allRptReorders.Add(rp);
            }

            TempData["allRptReorders"] = allRptReorders;

            return RedirectToAction("Report", "Store");

        }

        [Route("RptDepartmentUsage")]
        public ActionResult RptDepartmentUsage()
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                ViewBag.DepartmentList = new SelectList((from s in e.Departments.ToList()
                                                         select new
                                                         {
                                                             DepartmentCode = s.DepartmentCode,
                                                             DepartmentName = s.DepartmentName
                                                         }),
                                                "DepartmentCode",
                                                "DepartmentName",
                                                null);

                ViewBag.CategoryList = new SelectList((from s in e.Categories.ToList()
                                                       select new
                                                       {
                                                           CategoryID = s.CategoryID,
                                                           CategoryName = s.CategoryName
                                                       }),
                                                 "CategoryID",
                                                 "CategoryName",
                                                 null);

                TempData["RowIndexesToDiscard"] = new List<int>();

                return View();
            }
        }

        public ActionResult ExportRptDepartmentUsage()
        {
            List<DepartmentUsageReport> allDeptUsage = (List<DepartmentUsageReport>)TempData["allDeptUsage"];
            //string dept = (string) TempData["dept"];
            //string category = (string)TempData["category"];

            //List<DepartmentUsageReport> list = allDeptUsage.Where(x => x.DepartmentName == dept && x.CategoryName == category).ToList();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RptDepartmentUsage.rpt"));
            rd.SetDataSource(allDeptUsage);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "DepartmentUsageReport.pdf");
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        [HttpPost]
        public RedirectToRouteResult ExportRptDepartmentUsage2(String[] arr)
        {
            List<DepartmentUsageReport> allDeptUsage = new List<DepartmentUsageReport>();

            foreach (String s in arr) {
                DepartmentUsageReport dup = JsonConvert.DeserializeObject<DepartmentUsageReport>(s);
                allDeptUsage.Add(dup);
            }

            TempData["allDeptUsage"] = allDeptUsage;
            //TempData["dept"] = deptArray[0];
            //TempData["category"] = categoryArray[0];

            return RedirectToAction("Report", "Store");

        }
    }

}

