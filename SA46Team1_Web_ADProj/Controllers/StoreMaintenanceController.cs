using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreMaintenance")]
    public class StoreMaintenanceController : Controller
    {
        [Route("Categories")]
        public ActionResult Categories()
        {
            return View();
        }

        [Route("StoreBin")]
        public ActionResult StoreBin()
        {
            return View();
        }

        [Route("Suppliers")]
        public ActionResult Suppliers()
        {
            return View();
        }

        [Route("CollectionPoints")]
        public ActionResult CollectionPoints()
        { 
            return View();
        }

        [Route("Departments")]
        public ActionResult Departments()
        {
            return View();
        }

        [Route("Items")]
        public ActionResult Items()
        {
            return View();
        }
        
    }
}
