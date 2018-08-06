using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//-----------------------------------------------------------------
//   Author: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.Controllers
{
    [CustomAuthorize]
    public class ErrorController : Controller
    {
        public ActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View();
        }
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}