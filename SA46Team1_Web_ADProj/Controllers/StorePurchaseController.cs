using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class StorePurchaseController : Controller
    {
        public ActionResult CreatePO()
        {
            return View();
        }

        public ActionResult POList()
        {
            return View();
        }

        public ActionResult GoodsReceivedList()
        { 
            return View();
        }
        
    }
}
