﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [Authorize(Roles = "Department Head, Employee Representative, Employee")]
    public class DeptController : Controller
    {
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Requisition()
        {
            return View();
        }

        public ActionResult RequisitionHistory()
        {
            return View();
        }

        public ActionResult Approval()
        {
            return View();
        }
    
        public ActionResult Authorisation()
        {
            return View();
        }

        public ActionResult Notifications()
        {
            return View();
        }

        public ActionResult CollectionPoint()
        {
            return View();
        }

    }
}
