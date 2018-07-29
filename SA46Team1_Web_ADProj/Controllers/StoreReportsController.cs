using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using SA46Team1_Web_ADProj.Models;
using System.IO;
using System.Web.Helpers;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreReports")]
    public class StoreReportsController : Controller
    {
        [Route("Report1")]
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
            List<InventoryValuationReport> allInventory = new List<InventoryValuationReport>();
            using (SSISdbEntities dc = new SSISdbEntities())
            {
                allInventory = dc.InventoryValuationReports.ToList();
            }
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RptInventoryValuation.rpt"));
            rd.SetDataSource(allInventory);

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

        [Route("Report2")]
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
            List<ReorderReport> allReorder = new List<ReorderReport>();
            using (SSISdbEntities dc = new SSISdbEntities())
            {
                allReorder = dc.ReorderReports.ToList();
            }
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RptReorder.rpt"));
            rd.SetDataSource(allReorder);

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

        [Route("Report6")]
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
            List<DepartmentUsageReport> allDeptUsage = new List<DepartmentUsageReport>();
            using (SSISdbEntities dc = new SSISdbEntities())
            {
                allDeptUsage = dc.DepartmentUsageReports.ToList();
            }
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

        [Route("Report3")]
        public ActionResult Report3()
        {          
           return View();
        }
        //[Route("Report1")]
        //public ActionResult Report1()
        //{
        //    return View();
        //}

        //[Route("Report2")]
        //public ActionResult Report2()
        //{
        //    return View();
        //}

        //[Route("Report3")]
        //public ActionResult Report3()
        //{
        //    return View();
        //}

        [Route("Report4")]
        public ActionResult Report4()
        {
            return View();
        }

        [Route("Report5")]
        public ActionResult Report5()
        {
            return View();
        }

    }
}
