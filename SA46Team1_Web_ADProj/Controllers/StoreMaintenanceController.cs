using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SA46Team1_Web_ADProj.Models;

//-----------------------------------------------------------------
//   Author: Ong Wei Ting
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.Controllers
{
    [CustomAuthorize(Roles = "Store Manager, Store Supervisor")]
    [RoutePrefix("Store/StoreMaintenance")]
    public class StoreMaintenanceController : Controller //Tabs: Items, Category, Bin, Suppliers
    {
        /************Action methods belonging to Store Maintenance - Item *******************/

        [Route("Items")]
        public ActionResult Items()
        {
            Session["MaintenanceTabIndex"] = "1";

            if (Session["MaintenanceItemsPage"].ToString() == "1")
            {
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

                using (SSISdbEntities e = new SSISdbEntities())
                {
                    List<Supplier> list = e.Suppliers.ToList();
                    Dictionary<String, String> dic = new Dictionary<string, string>();

                    dic = list.ToDictionary(x => x.SupplierCode, x => x.CompanyName);
                    e.Configuration.ProxyCreationEnabled = false;

                    var json = JsonConvert.SerializeObject(dic);
                    ViewBag.SupplierList = json;
                }

                return View("Items2");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplayItemDetails(string maintenanceItemCode)
        {
            Session["MaintenanceItemsPage"] = "2";
            Session["MaintenanceItemCode"] = maintenanceItemCode;

            using (SSISdbEntities e = new SSISdbEntities())
            {
                TempData["ReorderLvl"] = e.Items.Where(x => x.ItemCode == maintenanceItemCode).Select(x => x.ReOrderLevel).FirstOrDefault();
                TempData["ReorderQty"] = e.Items.Where(x => x.ItemCode == maintenanceItemCode).Select(x => x.ReOrderQuantity).FirstOrDefault();

                //also pass var if category has item qty >0
                int countItemsWithQtyNotZero = e.Items.Where(x => x.ItemCode == maintenanceItemCode && x.Quantity > 0).ToList().Count();

                if (countItemsWithQtyNotZero > 0)
                {
                    TempData["countItemsWithQtyNotZero"] = true;
                }
                else
                {
                    TempData["countItemsWithQtyNotZero"] = false;
                }
            }

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
                string itemFirstChar = "Z";
                int countWithItemFirstChar = e.Items.Where(x => x.ItemCode.Substring(0, 1).ToUpper() == itemFirstChar).Count() + 1;

                switch (countWithItemFirstChar.ToString().Length)
                {
                    case 1:
                        item.ItemCode = itemFirstChar + "00" + countWithItemFirstChar;
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

        [HttpPost]
        [Route("Items/EditItem")]
        public RedirectToRouteResult EditItem(String[] suppliers, String[] desc, String[] status, string[] reorderlvl, string[] reorderqty)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                try
                {
                    string itemCode = Session["MaintenanceItemCode"].ToString();
                    Item item = e.Items.Where(x => x.ItemCode == itemCode).FirstOrDefault();
                    DAL.ItemsRepositoryImpl dal = new DAL.ItemsRepositoryImpl(e);
                    if (status != null)
                    {
                        if (status[0].ToLower() == "true")
                        {
                            item.Active = 1;
                        }
                        else
                        {
                            item.Active = 0;
                        }
                    }
                    item.ReOrderLevel = Convert.ToInt32(reorderlvl[0]);
                    item.ReOrderQuantity = Convert.ToInt32(reorderqty[0]);
                    item.Description = desc[0];
                    item.Supplier1 = suppliers[0];
                    item.Supplier2 = suppliers[1];
                    item.Supplier3 = suppliers[2];

                    dal.UpdateItem(item);
                    e.SaveChanges();

                    Session["MaintenanceItemsPage"] = "1";
                    return RedirectToAction("Maintenance", "Store");
                }
                catch(FormatException fe)
                {
                    Debug.WriteLine(fe.Message);
                }catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return null;
            }
        }

        [HttpPost]
        public RedirectToRouteResult BackToItemsMaintenanceList()
        {
            Session["MaintenanceItemsPage"] = "1";

            return RedirectToAction("Maintenance", "Store");
        }

        /************Action methods belonging to Store Maintenance - Category *******************/

        [Route("Categories")]
        public ActionResult Categories()
        {
            Session["MaintenanceTabIndex"] = "2";

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
        public RedirectToRouteResult DisplayCategoryDetails(string maintenanceCategoryId)
        {
            Session["MaintenanceCategoriesPage"] = "2";
            Session["MaintenanceCategoryId"] = maintenanceCategoryId;

            //also pass var if category has item qty >0
            using (SSISdbEntities e = new SSISdbEntities()) {
                int countItemsWithQtyNotZero = e.Items.Where(x => x.CategoryID == maintenanceCategoryId && x.Quantity > 0).ToList().Count();

                if (countItemsWithQtyNotZero > 0)
                {
                    TempData["countItemsWithQtyNotZero"] = true;
                }
                else {
                    TempData["countItemsWithQtyNotZero"] = false;
                }

            }

            return RedirectToAction("Maintenance", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToCategoriesMaintenanceList()
        {
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

        [HttpPost]
        [Route("Categories/EditCategory")]
        public RedirectToRouteResult EditCategory(Category[] arr)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                DAL.CategoryRepositoryImpl dal = new DAL.CategoryRepositoryImpl(e);
                dal.UpdateCategory(arr[0]);
                e.SaveChanges();

                Session["MaintenanceCategoriesPage"] = "1";

                return RedirectToAction("Maintenance", "Store");
            }
        }

        /************Action methods belonging to Store Maintenance - Store Bin *******************/

        [Route("StoreBin")]
        public ActionResult StoreBin()
        {
            Session["MaintenanceTabIndex"] = "3";

            if (Session["MaintenanceStoreBinPage"].ToString() == "1")
            {
                using (SSISdbEntities e = new SSISdbEntities()) {
                    List<String> binnedItemCodes = e.Bins.Where(x => x.Active == 1).Select(x => x.ItemCode).ToList();

                    ViewBag.ItemsList = new SelectList((from s in e.Items.Where(x=> !binnedItemCodes.Contains(x.ItemCode)).ToList()
                                                        select new
                                                        {
                                                            ItemCode = s.ItemCode,
                                                            Description = s.Description + " (" + s.UoM + ")"
                                                        }),
                                                    "ItemCode",
                                                    "Description",
                                                    null);
                }
                
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


            int binNumber = Int32.Parse(maintenanceBinId);

            using (SSISdbEntities e = new SSISdbEntities()) {
                string itemCode = e.Bins.Where(x => x.Number == binNumber).Select(x => x.ItemCode).FirstOrDefault();
                int countItemsWithQtyNotZero = e.Items.Where(x => x.ItemCode == itemCode && x.Quantity > 0).ToList().Count();
                int num = Convert.ToInt32(maintenanceBinId);
                Bin bin = e.Bins.Where(x => x.Number == num).FirstOrDefault();
                Session["MaintenanceBinId"] = bin.Location;
                if (countItemsWithQtyNotZero > 0)
                {
                    TempData["countItemsWithQtyNotZero"] = true;
                }
                else
                {
                    TempData["countItemsWithQtyNotZero"] = false;
                }
            }
            return RedirectToAction("Maintenance", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToBinsMaintenanceList()
        {
            Session["MaintenanceStoreBinPage"] = "1";

            return RedirectToAction("Maintenance", "Store");
        }


        [HttpPost]
        [Route("StoreBin/AddNewBin")]
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

        [HttpPost]
        [Route("StoreBin/EditBin")]
        public RedirectToRouteResult EditBin(FullBinModel[] arr)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                DAL.BinRepositoryImpl dal = new DAL.BinRepositoryImpl(e);
                FullBinModel model = arr[0];
                Bin bin = new Bin();
                bin.Number = model.Number;
                bin.Active = 1;
                bin.Location = model.Location;
                bin.ItemCode = e.Items.Where(x => x.Description == model.ItemDesc).Select(x => x.ItemCode).FirstOrDefault();

                dal.UpdateBin(bin);
                e.SaveChanges();

                Session["MaintenanceStoreBinPage"] = "1";

                return RedirectToAction("Maintenance", "Store");
            }
        }

        /************Action methods belonging to Store Maintenance - Suppliers *******************/


        [Route("Suppliers")]
        public ActionResult Suppliers()
        {
            Session["MaintenanceTabIndex"] = "4";

            if (Session["MaintenanceSuppliersPage"].ToString() == "1")
            {
                return View("Suppliers");
            }
            else if (Session["MaintenanceSuppliersPage"].ToString() == "2")
            {
                Session["MaintenanceSuppliersPage"] = "1";
                return View("Suppliers2");
            }
            else
            {
                Session["MaintenanceSuppliersPage"] = "1";
                return View("Suppliers3");
            }
        }

        [HttpPost]
        public RedirectToRouteResult DisplaySupplierDetails(string maintenanceSupplierCode)
        {
            Session["MaintenanceSuppliersPage"] = "2";
            Session["MaintenanceSupplierCode"] = maintenanceSupplierCode;

            return RedirectToAction("Maintenance", "Store");
        }
        
        public RedirectToRouteResult DisplaySupplierPriceList()
        {
            Session["MaintenanceSuppliersPage"] = "3";

            return RedirectToAction("Maintenance", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult UpdateSupplierPriceList(String[] arrItemCodes, String[] arrStatus, String[] price)
        {
            try
            {
                Session["MaintenanceSuppliersPage"] = "2";
                String supplierCode = Session["MaintenanceSupplierCode"].ToString();
                if(arrItemCodes.Count() > 0 && arrStatus.Count() > 0 && price.Count() > 0)
                {
                    using (SSISdbEntities e = new SSISdbEntities())
                    {
                        DAL.SupplierPriceListRepositoryImpl dal = new DAL.SupplierPriceListRepositoryImpl(e);

                        int index = 0;
                        foreach (string s in arrItemCodes)
                        {
                            SupplierPriceList spl = e.SupplierPriceLists.Where(x => x.SupplierCode == supplierCode && x.ItemCode == s).FirstOrDefault();
                            spl.UnitCost = float.Parse(price[index]);
                            if (arrStatus[index] == "true")
                            {
                                spl.Active = 1;
                            }
                            else
                            {
                                spl.Active = 0;
                            }

                            dal.UpdateSupplierPriceList(spl);
                            index++;
                        }

                        e.SaveChanges();
                    }

                }
                else
                {
                    Debug.Write("Empty input detected.");
                }

            }
            catch (FormatException fe)
            {
                Debug.WriteLine(fe.Message);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
            return RedirectToAction("Maintenance", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToSupplierDetails()
        {
            Session["MaintenanceSuppliersPage"] = "2";

            return RedirectToAction("Maintenance", "Store");
        }

        [HttpPost]
        public RedirectToRouteResult BackToSuppliersMaintenanceList()
        {
            Session["MaintenanceSuppliersPage"] = "1";

            return RedirectToAction("Maintenance", "Store");
        }

        [HttpPost]
        [Route("Suppliers/AddNewSupplier")]
        public RedirectToRouteResult AddNewSupplier(Supplier supplier)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                supplier.Active = 1;
                DAL.SupplierRepositoryImpl dal = new DAL.SupplierRepositoryImpl(e);
                dal.InsertSupplier(supplier);

                e.SaveChanges();

                return RedirectToAction("Maintenance", "Store");
            }
        }

        [HttpPost]
        [Route("Suppliers/EditSupplier")]
        public RedirectToRouteResult EditSupplier(Supplier[] arr)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                DAL.SupplierRepositoryImpl dal = new DAL.SupplierRepositoryImpl(e);
                dal.UpdateSupplier(arr[0]);
                e.SaveChanges();

                Session["MaintenanceSuppliersPage"] = "1";

                return RedirectToAction("Maintenance", "Store");
            }
        }

        /************Action methods belonging to Store Maintenance - (obsolete) *******************/


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

        [HttpPost]
        [Route("CollectionPoints/EditCollectionPoint")]
        public RedirectToRouteResult EditCollectionPoint(CollectionPoint[] arr)
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                DAL.CollectionPointRepositoryImpl dal = new DAL.CollectionPointRepositoryImpl(e);
                dal.UpdateCollectionPoint(arr[0]);
                e.SaveChanges();

                Session["MaintenanceCollectionPointsPage"] = "1";

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
        
    }
}
