using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SA46Team1_Web_ADProj
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                base.HandleUnauthorizedRequest(filterContext);
            else
            {
                // handle controller access
                filterContext.Result = new ViewResult { ViewName = "Unauthorized" };
                filterContext.HttpContext.Response.StatusCode = 403;
                //handle menu links
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.HttpContext.Response.StatusCode = 403;
            }
        }
    }
}