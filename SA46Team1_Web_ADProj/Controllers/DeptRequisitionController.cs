using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Dept/DeptRequisition")]
    public class DeptRequisitionController : Controller
    {
        [Route("NewReq")]
        public ActionResult NewReq()
        {
            return View();
        }

        [Route("BackOrders")]
        public ActionResult BackOrders()
        {
            return View();
        }

     
    }
}
