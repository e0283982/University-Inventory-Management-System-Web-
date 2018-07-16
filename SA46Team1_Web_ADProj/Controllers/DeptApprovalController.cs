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
            return View();
        }

    }
}
