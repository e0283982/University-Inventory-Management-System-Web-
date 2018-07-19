using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreInventoryManager")]
    public class StoreManagerInventoryController : Controller
    {
        public StoreManagerInventoryController()
        {
            
        }


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

        [Route("ReviewAdj")]
        public ActionResult ReviewAdj()
        {
            return View();
        }

        [HttpPost]
        [Route("Approve")]
        public RedirectToRouteResult Approve()
        {
            string s = Request.Form["RequestId"].ToString();
            return RedirectToAction("Inventory","Store");
        }
        [Route("Reject")]
        public RedirectToRouteResult Reject()
        {
            string s = Request.Form["Description"].ToString();
            return RedirectToAction("Inventory", "Store");
        }



    }
}
