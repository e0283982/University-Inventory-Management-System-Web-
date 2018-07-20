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

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                int id = m.StockRetrievalHeaders.Count();
                string reqId = "StoR-" + id;
                Session["RetrievalId"] = "StoR-" + id;
                ViewBag.IdCount = id;
                ViewBag.Disbursed = m.StockRetrievalHeaders.Where(x => x.ID == reqId).First().Disbursed;                
            }

            //ViewBag.Disbursed = 0;

            //ViewBag.IdCount = 2;

            return View();
        }

        //New Stuff
        [HttpPost]
        public RedirectToRouteResult DisburseItems()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.Configuration.ProxyCreationEnabled = false;
                string id = (string) Session["RetrievalId"];
                StockRetrievalHeader srh = m.StockRetrievalHeaders.Where(x => x.ID == id).First();
                srh.Disbursed = 1;
                m.SaveChanges();

            }            

            return RedirectToAction("Disbursements", "Store");
        }



    }
}
