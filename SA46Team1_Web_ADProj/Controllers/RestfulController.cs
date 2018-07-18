using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Data.Entity;


namespace SA46Team1_Web_ADProj.Controllers
{
    [System.Web.Mvc.RoutePrefix("api/Restful")]
    public class RestfulController : ApiController
    {
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("/EmployeeList")]
        public List<Employee> GetEmployeesList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.Employees.ToList<Employee>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("/AdjustmentOverView")]
        public List<StockAdjustmentHeader> GetStockAdjustmentOverview()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.StockAdjustmentHeaders.ToList();
            }
        }

    }
}