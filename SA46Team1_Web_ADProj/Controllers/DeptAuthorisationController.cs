using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Dept/DeptAuthorisation")]
    public class DeptAuthorisationController : Controller
    {
        [Route("RoleDelegation")]
        public ActionResult RoleDelegation()
        {
            return View();
        }

     
    }
}
