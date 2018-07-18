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
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetEmployeeList")]
        public List<Employee> GetEmployeeList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.Employees.ToList<Employee>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetItemCodeList")]
        public List<String> GetItemCodeList()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.InventoryOverviews.Select(x => x.ItemCode).ToList<String>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetInventoryOverviewList/{itemcode}")]
        public List<InventoryOverview> GetInventoryOverviewList(string itemcode)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.InventoryOverviews.Where(x => x.ItemCode == itemcode).ToList<InventoryOverview>();
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GetGoodsReceivedLists/{itemcode}")]
        public List<GoodsReceivedList> GetGoodsReceivedLists(string itemcode)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                return m.GoodsReceivedLists.Where(x => x.ItemCode == itemcode).ToList<GoodsReceivedList>();
            }
        }
    }
}