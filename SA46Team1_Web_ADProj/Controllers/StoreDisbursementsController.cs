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
            if (Session["DisbursementListPage"].ToString() == "1")
            {
                @Session["BackToDisbursementList"] = "false";
                return View("Disbursement");
            }
            else
            {
                Session["DisbursementListPage"] = "1";
                return View("Disbursement2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayDisbursementDetails()
        {
            Session["DisbursementListPage"] = "2";
            return RedirectToAction("Disbursements", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToDisbursementList()
        {
            Session["DisbursementListPage"] = "1";
            Session["BackToDisbursementList"] = "true";

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

        [HttpPost]
        public RedirectToRouteResult DisplayReqDetails()
        {
            Session["ReqListPage"] = "2";
            return RedirectToAction("Disbursements", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToRequisitionsList()
        {
            Session["ReqListPage"] = "1";

            return RedirectToAction("Disbursements", "Store");
        }

        [Route("Retrieval")]
        public ActionResult Retrieval()
        {
            return View();
        }
        
    }
}
