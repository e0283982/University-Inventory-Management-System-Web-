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
            if (Session["ReqHistoryPage"].ToString() == "1")
            {
                return View("Overview");
            }
            else
            {
                Session["ReqHistoryPage"] = "1";
                return View("Overview2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayReqHistoryDetails(FormCollection data)
        {
            Session["ReqHistoryPage"] = "2";
            Session["id"] = data["FormID"];
            return RedirectToAction("RequisitionHistory", "Dept");
        }

        [HttpPost]
        public RedirectToRouteResult BackToReqHistoryList()
        {
            Session["ReqHistoryPage"] = "1";
            return RedirectToAction("RequisitionHistory", "Dept");
        }

        [Route("CollectionList")]
        public ActionResult CollectionList()
        {
            return View();
        }
    }
}
