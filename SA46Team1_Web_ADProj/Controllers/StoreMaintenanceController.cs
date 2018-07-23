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

        [HttpPost]
        [Route("Categories/AddNewCategory")]
        public RedirectToRouteResult AddNewCategory(Category cat)
        {
            using (SSISdbEntities e = new SSISdbEntities()) {

                string categoryCount = (e.Categories.Count() + 1).ToString();
                cat.CategoryID = "C" + categoryCount;
                cat.Active = 1;

                DAL.CategoryRepositoryImpl dal = new DAL.CategoryRepositoryImpl(e);
                dal.InsertCategory(cat);
                e.SaveChanges();

                return RedirectToAction("Maintenance", "Store");
            }
        }

        [Route("StoreBin")]
        public ActionResult StoreBin()
        {
            if (Session["MaintenanceStoreBinPage"].ToString() == "1")
            {
                using (SSISdbEntities e = new SSISdbEntities()) {
                    ViewBag.ItemsList = new SelectList((from s in e.Items.ToList()
                                                        select new
                                                        {
                                                            ItemCode = s.ItemCode,
                                                            Description = s.Description + " (" + s.UoM + ")"
                                                        }),
                                                    "ItemCode",
                                                    "Description",
                                                    null);
                }
                
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


        [HttpPost]
        [Route("Departments/AddNewDept")]
        public RedirectToRouteResult AddNewBin(Bin bin)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                string itemCode = Request.Form["SelectBinItem"].ToString();
                int binCount = e.Bins.Count() + 1;

                bin.Number = binCount;
                bin.ItemCode = itemCode;
                bin.Active = 1;

                DAL.BinRepositoryImpl dal = new DAL.BinRepositoryImpl(e);
                dal.InsertBin(bin);

                e.SaveChanges();

                return RedirectToAction("Maintenance", "Store");
            }
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
        public RedirectToRouteResult DisplaySupplierDetails(string maintenanceSupplierCode)
        {
            Session["MaintenanceSuppliersPage"] = "2";
            Session["MaintenanceSupplierCode"] = maintenanceSupplierCode;

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

        [HttpPost]
        [Route("CollectionPoints/AddNewCollectionPoint")]
        public RedirectToRouteResult AddNewCollectionPoint(CollectionPoint cp)
        {
            //Item itemToAdd = new Item();

            using (SSISdbEntities e = new SSISdbEntities())
            {

                string collectionPtCount = (e.CollectionPoints.Count() + 1).ToString();
                cp.CollectionPointID = "CP" + collectionPtCount;
                cp.Active = 1;

                DAL.CollectionPointRepositoryImpl dal = new DAL.CollectionPointRepositoryImpl(e);
                dal.InsertCollectionPoint(cp);
                e.SaveChanges();

                return RedirectToAction("Maintenance", "Store");
            }
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
        public RedirectToRouteResult DisplayDepartmentDetails(string maintenanceDeptCode)
        {
            Session["MaintenanceDepartmentsPage"] = "2";
            Session["MaintenanceDeptCode"] = maintenanceDeptCode;

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
                using (SSISdbEntities e = new SSISdbEntities())
                {
                    List<String> UOMList = new List<string>()
                    {
                        "Box","Dozen","Each","Packet","Set"
                    };
                    ViewBag.SupplierList = new SelectList(e.Suppliers.ToList(), "SupplierCode", "CompanyName");
                    ViewBag.UOMList = new SelectList(UOMList, "UoM");
                    ViewBag.CategoryList = new SelectList(e.Categories.ToList(), "CategoryID", "CategoryName");

                }
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

        [HttpPost]
        [Route("Items/AddNewItem")]
        public RedirectToRouteResult AddNewItem(Item item)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                string category = Request.Form["SelectItemCategory"].ToString();
                string UoM = Request.Form["SelectItemUOM"].ToString();
                string supplier1 = Request.Form["SelectSupplier1"].ToString();
                string supplier2 = Request.Form["SelectSupplier2"].ToString();
                string supplier3 = Request.Form["SelectSupplier3"].ToString();

                string itemFirstChar = item.Description.Substring(0, 1).ToUpper();
                int countWithItemFirstChar = e.Items.Where(x => x.Description.Substring(0, 1).ToUpper() == itemFirstChar).Count() +1;
                
                switch (countWithItemFirstChar.ToString().Length)
                {
                    case 1:
                        item.ItemCode = itemFirstChar + "00"+countWithItemFirstChar;
                        break;
                    case 2:
                        item.ItemCode = itemFirstChar + "0" + countWithItemFirstChar;
                        break;
                    case 3:
                        item.ItemCode = itemFirstChar + countWithItemFirstChar;
                        break;
                }

                item.CategoryID = category;
                item.UoM = UoM;
                item.Supplier1 = supplier1;
                item.Supplier2 = supplier2;
                item.Supplier3 = supplier3;
                item.Active = 1;
                item.AvgUnitCost = 0;

                DAL.ItemsRepositoryImpl dal = new DAL.ItemsRepositoryImpl(e);
                dal.InsertItem(item);

                e.SaveChanges();

                return RedirectToAction("Maintenance", "Store");
            }
        }
    }
}
