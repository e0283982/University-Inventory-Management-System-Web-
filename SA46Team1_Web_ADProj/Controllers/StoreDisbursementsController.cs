using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreDisbursements")]
    public class StoreDisbursementsController : Controller
    {
        [Route("Disbursement")]
        public ActionResult Disbursement()
        {
            return View();
        }

        [Route("Requisition")]
        public ActionResult Requisition()
        {
            return View();
        }

        [Route("Retrieval")]
        public ActionResult Retrieval()
        {
            return View();
        }
        
    }
}
