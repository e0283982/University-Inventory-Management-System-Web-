using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class StoreController : Controller
    {
        public ActionResult Home(string submit)
        {
            return View();
        }

        public ActionResult Inventory()
        {
            return View();
        }

        public ActionResult Disbursements()
        {
            return View();
        }

        public ActionResult Purchase()
        {
            return View();
        }

        public ActionResult Report()
        {
            return View();
        }

        public ActionResult Maintenance()
        {
            return View();
        }
        
    }
}
