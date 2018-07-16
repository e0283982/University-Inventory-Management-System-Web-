using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreReports")]
    public class StoreReportsController : Controller
    {
        [Route("Report1")]
        public ActionResult Report1()
        {
            return View();
        }

        [Route("Report2")]
        public ActionResult Report2()
        {
            return View();
        }

        [Route("Report3")]
        public ActionResult Report3()
        {
            return View();
        }

        [Route("Report4")]
        public ActionResult Report4()
        {
            return View();
        }

        [Route("Report5")]
        public ActionResult Report5()
        {
            return View();
        }

    }
}
