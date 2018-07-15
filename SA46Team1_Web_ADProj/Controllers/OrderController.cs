using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class OrderController : Controller
    {
        public ActionResult Overview()
        {
            ViewData["Message"] = "Your Order overview description page.";

            return View();
        }

        public ActionResult Delivery()
        {
            ViewData["Message"] = "Your Delivery description page.";

            return View();
        }

        public ActionResult Requests()
        {
            ViewData["Message"] = "Your requests page.";

            return View();
        }

        public ActionResult Requisitions()
        {
            ViewData["Message"] = "Your Requisitions page.";

            return View();
        }

       
    }
}
