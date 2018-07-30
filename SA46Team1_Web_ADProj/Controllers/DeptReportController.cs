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

            foreach (String s in arr)
            {
                DepartmentUsageReport dup = JsonConvert.DeserializeObject<DepartmentUsageReport>(s);
                allDeptUsage.Add(dup);
            }

            TempData["allDeptUsage"] = allDeptUsage;
            //TempData["dept"] = deptArray[0];
            //TempData["category"] = categoryArray[0];

            return RedirectToAction("Report", "Dept");

        }
    }
}