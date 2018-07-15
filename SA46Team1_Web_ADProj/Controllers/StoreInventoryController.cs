using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class StoreInventoryController : Controller
    {
        public ActionResult Overview()
        {
            return View();
        }

        public ActionResult Reorder()
        {
            return View();
        }

        public ActionResult StockAdj()
        {
           
            return View("StockAdj");
            
        }

        public ActionResult StockTake()
        {
            return View();
        }
        
    }
}
