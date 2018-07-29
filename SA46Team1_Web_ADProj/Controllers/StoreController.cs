using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using SA46Team1_Web_ADProj.DAL;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class StoreController : Controller
    {
        private ItemsRepositoryImpl itemRepository;

        public StoreController()
        {
            this.itemRepository = new ItemsRepositoryImpl(new SSISdbEntities());
        }

        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Home()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager")]
        public JsonResult Search(string search)
        {
            return new JsonResult { Data = itemRepository.GetItemById(search), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CustomAuthorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Inventory()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager")]
        public ActionResult Disbursements()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager")]
        public ActionResult Purchase()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Store Clerk, Store Manager")]
        public ActionResult Report()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Store Manager")]
        public ActionResult Maintenance()
        {
            return View();
        }

    }
}
