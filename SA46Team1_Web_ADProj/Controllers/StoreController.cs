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

        [Authorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Home()
        {
            return View();
        }
        [Authorize(Roles = "Store Clerk, Store Manager")]
        public JsonResult Search(string search)
        {
            return new JsonResult { Data = itemRepository.GetItemById(search), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Inventory()
        {
            return View();
        }
        [Authorize(Roles = "Store Clerk, Store Manager")]
        public ActionResult Disbursements()
        {
            return View();
        }
        [Authorize(Roles = "Store Clerk, Store Manager")]
        public ActionResult Purchase()
        {
            return View();
        }
        [Authorize(Roles = "Store Clerk, Store Manager")]
        public ActionResult Report()
        {
            return View();
        }

        [Authorize(Roles = "Store Manager")]
        public ActionResult Maintenance()
        {
            return View();
        }

    }
}
