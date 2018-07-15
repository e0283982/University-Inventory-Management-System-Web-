using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class StoreMaintenanceController : Controller
    {
        public ActionResult Categories()
        {
            return View();
        }

        public ActionResult StoreBin()
        {
            return View();
        }

        public ActionResult Suppliers()
        {
            return View();
        }

        public ActionResult CollectionPoints()
        { 
            return View();
        }

        public ActionResult Departments()
        {
            return View();
        }

        public ActionResult Items()
        {
            return View();
        }
        
    }
}
