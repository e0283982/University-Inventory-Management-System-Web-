using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StorePurchase")]
    public class StorePurchaseController : Controller
    {
        [Route("CreatePO")]
        public ActionResult CreatePO()
        {
            return View();
        }

        [Route("POList")]
        public ActionResult POList()
        {
            return View();
        }

        [Route("GoodsReceivedList")]
        public ActionResult GoodsReceivedList()
        { 
            return View();
        }
        
    }
}
