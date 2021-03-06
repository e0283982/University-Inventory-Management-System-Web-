﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json;
using SA46Team1_Web_ADProj.Models;

//-----------------------------------------------------------------
//   Author: Peh Guek Lan
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.Controllers
{    
    [RoutePrefix("Dept/DeptReport")]
    public class DeptReportController : Controller
    {

        [Route("RptDepartmentUsage")]
        public ActionResult RptDepartmentUsage()
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                string deptCode = Session["DepartmentCode"].ToString();
                ViewBag.Department = e.Departments.Where(x => x.DepartmentCode == deptCode).Select(x => x.DepartmentName).FirstOrDefault();
                List<String> categoryList = e.Categories.Select(x => x.CategoryName).ToList();
                categoryList.Insert(0,"All");
                ViewBag.CategoryList = new SelectList(categoryList,
                                                 null);

                TempData["RowIndexesToDiscard"] = new List<int>();

                return View();
            }
        }

        public ActionResult ExportRptDepartmentUsage()
        {
            List<DepartmentUsageReport> allDeptUsage = (List<DepartmentUsageReport>)TempData["allDeptUsage"];
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

            foreach (String s in arr)
            {
                DepartmentUsageReport dup = JsonConvert.DeserializeObject<DepartmentUsageReport>(s);
                allDeptUsage.Add(dup);
            }

            TempData["allDeptUsage"] = allDeptUsage;

            return RedirectToAction("Report", "Dept");

        }
    }
}
