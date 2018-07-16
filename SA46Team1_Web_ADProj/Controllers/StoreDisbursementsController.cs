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

        [HttpPost]
        public RedirectToRouteResult DisplayReqDetails()
        {
            //Session["SelectedPONumber"] = dataToBeSent;
            Session["ReqListPage"] = "2";
            return RedirectToAction("Disbursements", "Store");
        }

        [Route("Requisition")]
        public ActionResult Requisition()
        {
            if (Session["ReqListPage"].ToString() == "1")
            {
                return View("Requisition");
            }
            else
            {
                Session["ReqListPage"] = "1";
                return View("Requisition2");
            }
        }

        [Route("Retrieval")]
        public ActionResult Retrieval()
        {
            return View();
        }
        
    }
}
