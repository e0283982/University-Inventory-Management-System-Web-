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
                Session["MaintenanceBackFlagPage"] = "0";
                return View("Categories");
            }
            else
            {
                Session["MaintenanceCategoriesPage"] = "1";
                return View("Categories2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayCategoryDetails(string maintenanceCategoryId)
        {
            Session["MaintenanceCategoriesPage"] = "2";
            Session["MaintenanceCategoryId"] = maintenanceCategoryId;

            return RedirectToAction("Maintenance", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToCategoriesMaintenanceList()
        {
            Session["MaintenanceBackFlagPage"] = "1";
            Session["MaintenanceCategoriesPage"] = "1";

            return RedirectToAction("Maintenance", "Store");
        }

        [Route("StoreBin")]
        public ActionResult StoreBin()
        {
            if (Session["MaintenanceStoreBinPage"].ToString() == "1")
            {
                Session["MaintenanceBackFlagPage"] = "0";
                return View("StoreBin");
            }
            else
            {
                Session["MaintenanceStoreBinPage"] = "1";
                return View("StoreBin2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayStoreBinDetails(string maintenanceBinId)
        {
            Session["MaintenanceStoreBinPage"] = "2";
            Session["MaintenanceBinId"] = maintenanceBinId;

            return RedirectToAction("Maintenance", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToBinsMaintenanceList()
        {
            Session["MaintenanceBackFlagPage"] = "2";
            Session["MaintenanceStoreBinPage"] = "1";

            return RedirectToAction("Maintenance", "Store");
        }


        [Route("Suppliers")]
        public ActionResult Suppliers()
        {
            if (Session["MaintenanceSuppliersPage"].ToString() == "1")
            {
                Session["MaintenanceBackFlagPage"] = "0";
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
            Session["MaintenanceSuppliersPage"] = "2";
            return RedirectToAction("Maintenance", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToSuppliersMaintenanceList()
        {
            Session["MaintenanceBackFlagPage"] = "4";
            Session["MaintenanceSuppliersPage"] = "1";

            return RedirectToAction("Maintenance", "Store");
        }

        [Route("CollectionPoints")]
        public ActionResult CollectionPoints()
        {
            if (Session["MaintenanceCollectionPointsPage"].ToString() == "1")
            {
                Session["MaintenanceBackFlagPage"] = "0";
                return View("CollectionPoints");
            }
            else
            {
                Session["MaintenanceCollectionPointsPage"] = "1";
                return View("CollectionPoints2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayCollectionPointDetails(string maintenanceCollectionPtCode)
        {
            Session["MaintenanceCollectionPointsPage"] = "2";
            Session["MaintenanceCollectionPtCode"] = maintenanceCollectionPtCode;

            return RedirectToAction("Maintenance", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToCollectionPointsMaintenanceList()
        {
            Session["MaintenanceBackFlagPage"] = "3";
            Session["MaintenanceCollectionPointsPage"] = "1";

            return RedirectToAction("Maintenance", "Store");
        }

        [Route("Departments")]
        public ActionResult Departments()
        {
            if (Session["MaintenanceDepartmentsPage"].ToString() == "1")
            {
                Session["MaintenanceBackFlagPage"] = "0";
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
            Session["MaintenanceDepartmentsPage"] = "2";
            return RedirectToAction("Maintenance", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToDeptsMaintenanceList()
        {
            Session["MaintenanceBackFlagPage"] = "5";
            Session["MaintenanceDepartmentsPage"] = "1";
            return RedirectToAction("Maintenance", "Store");
        }

        [Route("Items")]
        public ActionResult Items()
        {
            if (Session["MaintenanceItemsPage"].ToString() == "1")
            {
                Session["MaintenanceBackFlagPage"] = "0";
                return View("Items");
            }
            else
            {
                Session["MaintenanceItemsPage"] = "1";
                return View("Items2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayItemDetails(string maintenanceItemCode)
        {
            Session["MaintenanceItemsPage"] = "2";
            Session["MaintenanceItemCode"] = maintenanceItemCode;
            return RedirectToAction("Maintenance", "Store");
        }

    }
}
