using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Dept/DeptApproval")]
    public class DeptApprovalController : Controller
    {
        [Route("Approval")]
        public ActionResult Approval()
        {
            if (Session["ReqApprovalPage"].ToString() == "1")
            {
                return View("Approval");
            }
            else
            {
                Session["ReqApprovalPage"] = "1";
                return View("Approval2");
            }
        }

        [HttpPost]
        //[Route("DisplayApprovalDetails")]
        public RedirectToRouteResult DisplayApprovalDetails(string ReqFormId)
        {
            Session["ReqApprovalPage"] = "2";
            TempData["ReviewNewRequisitionId"] = ReqFormId;

            return RedirectToAction("Approval", "Dept");
        }

        [HttpPost]
        public RedirectToRouteResult BackToApprovalList()
        {
            Session["ReqApprovalPage"] = "1";
            return RedirectToAction("Approval", "Dept");
        }
    }
}
