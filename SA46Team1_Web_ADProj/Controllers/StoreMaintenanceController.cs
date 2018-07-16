using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Store/StoreMaintenance")]
    public class StoreMaintenanceController : Controller
    {
        [Route("Categories")]
        public ActionResult Categories()
        {
            if (Session["MaintenanceCategoriesPage"].ToString() == "1")
            {
                return View("Categories");
            }
            else
            {
                Session["MaintenanceCategoriesPage"] = "1";
                return View("Categories2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayCategoryDetails()
        {
            //Session["SelectedPONumber"] = dataToBeSent;
            Session["MaintenanceCategoriesPage"] = "2";
            return RedirectToAction("Maintenance", "Store");
        }

        [Route("StoreBin")]
        public ActionResult StoreBin()
        {
            if (Session["MaintenanceStoreBinPage"].ToString() == "1")
            {
                return View("StoreBin");
            }
            else
            {
                Session["MaintenanceStoreBinPage"] = "1";
                return View("StoreBin2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayStoreBinDetails()
        {
            //Session["SelectedPONumber"] = dataToBeSent;
            Session["MaintenanceStoreBinPage"] = "2";
            return RedirectToAction("Maintenance", "Store");
        }

        [Route("Suppliers")]
        public ActionResult Suppliers()
        {
            if (Session["MaintenanceSuppliersPage"].ToString() == "1")
            {
                return View("Suppliers");
            }
            else
            {
                Session["MaintenanceSuppliersPage"] = "1";
                return View("Suppliers2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplaySupplierDetails()
        {
            //Session["SelectedPONumber"] = dataToBeSent;
            Session["MaintenanceSuppliersPage"] = "2";
            return RedirectToAction("Maintenance", "Store");
        }

        [Route("CollectionPoints")]
        public ActionResult CollectionPoints()
        {
            if (Session["MaintenanceCollectionPointsPage"].ToString() == "1")
            {
                return View("CollectionPoints");
            }
            else
            {
                Session["MaintenanceCollectionPointsPage"] = "1";
                return View("CollectionPoints2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayCollectionPointDetails()
        {
            //Session["SelectedPONumber"] = dataToBeSent;
            Session["MaintenanceCollectionPointsPage"] = "2";
            return RedirectToAction("Maintenance", "Store");
        }

        [Route("Departments")]
        public ActionResult Departments()
        {
            if (Session["MaintenanceDepartmentsPage"].ToString() == "1")
            {
                return View("Departments");
            }
            else
            {
                Session["MaintenanceDepartmentsPage"] = "1";
                return View("Departments2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayDepartmentDetails()
        {
            //Session["SelectedPONumber"] = dataToBeSent;
            Session["MaintenanceDepartmentsPage"] = "2";
            return RedirectToAction("Maintenance", "Store");
        }

        [Route("Items")]
        public ActionResult Items()
        {
            if (Session["MaintenanceItemsPage"].ToString() == "1")
            {
                return View("Items");
            }
            else
            {
                Session["MaintenanceItemsPage"] = "1";
                return View("Items2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayItemDetails()
        {
            //Session["SelectedPONumber"] = dataToBeSent;
            Session["MaintenanceItemsPage"] = "2";
            return RedirectToAction("Maintenance", "Store");
        }

    }
}
