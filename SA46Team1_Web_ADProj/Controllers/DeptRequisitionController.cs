using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class DeptRequisitionController : Controller
    {
        public ActionResult NewReq()
        {
            return View();
        }

        public ActionResult BackOrders()
        {
            return View();
        }

     
    }
}
