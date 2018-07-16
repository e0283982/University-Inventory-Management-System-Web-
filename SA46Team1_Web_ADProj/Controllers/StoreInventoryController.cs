using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreInventory")]
    public class StoreInventoryController : Controller
    {
        [Route("Overview")]
        public ActionResult Overview()
        {
            return View();
        }

        [Route("Reorder")]
        public ActionResult Reorder()
        {
            return View();
        }

        [Route("StockAdj")]
        public ActionResult StockAdj()
        {
            if (Session["StockAdjPage"].ToString() == "1")
            {
                return View("StockAdj");
            }
            else
            {
                Session["StockAdjPage"] = "1";
                return View("StockAdj2");
            }
        }

        [HttpPost]
        //[Route("StockAdj")]
        public RedirectToRouteResult CreateNewStockAdj()
        {
            Session["StockAdjPage"] = "2";
            return RedirectToAction("Inventory", "Store");
        }

        [Route("StockTake")]
        public ActionResult StockTake()
        {
            return View();
        }
        
    }
}
