using Quartz;
using Quartz.Impl;
using SA46Team1_Web_ADProj.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SA46Team1_Web_ADProj.Scheduler;
using System.Diagnostics;

namespace SA46Team1_Web_ADProj
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Scheduler using Quartz.Net (See codes in MyScheduler Class & the relevant Jobs
            MyScheduler sch = new MyScheduler();
            sch.Start();
        }

        protected void Session_Start()
        {
            //Session variables for Application
            Session["Role"] = "Dept";
            Session["UserId"] = "E4";
            Session["DepartmentCode"] = "COMM";
            Session["access-token"] = "test";
            Session["NoUnreadRequests"] = 0;

            //Session variable for PO
            Session["newPOList"] = new List<Models.POFullDetail>();
            Session["poDetailsEditMode"] = false;
            Session["grEditMode"] = true;

            // Session variable for Reorders
            Session["ReorderList"] = new List<Models.ReorderList>();

            //Session variables for inner pages of DEPT tabs
            Session["newReqList"] = new List<Models.StaffRequisitionDetail>();
            Session["newReqEditMode"] = false;
            Session["tempList"] = new List<String>();

            Session["ReviewNewRequisitionId"] = "";
            Session["DeptReqTabIndex"] = "0";
            Session["currentFormId"] = "";
            //Session variables for Dept - Requisition History
            Session["ReqHistoryPage"] = "1";
            Session["CurrentReqHistory"] = new Models.ReqHistoryModel();
            Session["existingReqEditMode"] = false;

            //Session variables for Dept - Requisition Approval
            Session["ReqApprovalPage"] = "1";

            //Session variables for inner pages of STORE tabs
            Session["StoreInventoryTabIndex"] = "1";
            Session["StorePurchaseTabIndex"] = "1";

            //Session variables for Store - Inventory
            Session["StockAdjPage"] = "1";
            Session["SelectedPONumber"] = "0";
            Session["InventoryOverviewPage"] = "1";
            Session["newAdjList"] = new List<Models.StockAdjItemModel>();


            //Session variables for Store - Purchase
            Session["POListPage"] = "1";
            
            Session["GRListPage"] = "1";
            Session["BackToGRList"] = "false";

            //Session variables for Store - Disbursement
            Session["ReqListPage"] = "1";
            Session["storeReqFormId"] = "";

            Session["DisbursementListPage"] = "1";
            Session["BackToDisbursementList"] = "false";
            Session["storeDisbursementFormId"] = "";

            Session["RetrievalListPage"] = "1";

            //Session variables for Store - Maintenance
            Session["MaintenanceBackFlagPage"] = "0";

            Session["MaintenanceItemsPage"] = "1";
            Session["MaintenanceItemCode"] = "";

            Session["MaintenanceCategoriesPage"] = "1";
            Session["MaintenanceCategoryId"] = "";

            Session["MaintenanceCollectionPointsPage"] = "1";
            Session["MaintenanceCollectionPtCode"] = "";

            Session["MaintenanceDepartmentsPage"] = "1";
            Session["MaintenanceDeptCode"] = "";

            Session["MaintenanceStoreBinPage"] = "1";
            Session["MaintenanceBinId"] = "";

            Session["MaintenanceSuppliersPage"] = "1";
            Session["MaintenanceSupplierCode"] = "";

        }
    }
}
