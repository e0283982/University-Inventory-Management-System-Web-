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

        public JsonResult Search(string search)
        {
            return new JsonResult { Data = itemRepository.GetItemById(search), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize(Roles = "Store Clerk, Store Manager, Store Supervisor")]
        public ActionResult Inventory()
        {
            return View();
        }

        public ActionResult Disbursements()
        {
            return View();
        }

        public ActionResult Purchase()
        {
            return View();
        }

        public ActionResult Report()
        {
            return View();
        }

        public ActionResult Maintenance()
        {
            return View();
        }
        


    }
}
