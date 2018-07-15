using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserModel model)
        {
            ViewBag.IsValid = true;

            if (model.Username != null)
            {
                if (model.Username.Equals("Dept"))
                {
                    return RedirectToAction("Home", "Dept", new { area = "" });
                }
                else if (model.Username.Equals("Store"))
                {
                    return RedirectToAction("Home", "Store", new { area = "" });
                }
                else
                {
                    ViewBag.IsValid = false;
                }
            }
            
            return View("Login");
        }

        [HttpPost]
        public ActionResult Logout()
        {

            return View("Login");
        }


    }
}
