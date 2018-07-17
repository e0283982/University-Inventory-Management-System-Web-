using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Dept/DeptRequisitionHistory")]
    public class DeptRequisitionHistoryController : Controller
    {
        [Route("Overview")]
        public ActionResult Overview()
        {
            return View();
        }

        [Route("CollectionList")]
        public ActionResult CollectionList()
        {
            return View();
        }

     
    }
}
