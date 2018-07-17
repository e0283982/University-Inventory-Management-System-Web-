﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SA46Team1_Web_ADProj
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start()
        {
            //Session variables for Application
            Session["Role"] = "Dept";

            //Session variables for inner pages of DEPT tabs

            //Session variables for Dept - Requisition History
            Session["ReqHistoryPage"] = "1";

            //Session variables for Dept - Requisition Approval
            Session["ReqApprovalPage"] = "1";

            //Session variables for inner pages of STORE tabs

            //Session variables for Store - Inventory
            Session["StockAdjPage"] = "1";
            Session["SelectedPONumber"] = "0";

            //Session variables for Store - Purchase
            Session["POListPage"] = "1";

            Session["GRListPage"] = "1";
            Session["BackToGRList"] = "false";

            //Session variables for Store - Disbursement
            Session["ReqListPage"] = "1";

            Session["DisbursementListPage"] = "1";
            Session["BackToDisbursementList"] = "false";

            //Session variables for Store - Maintenance
            Session["MaintenanceBackFlagPage"] = "0";

            Session["MaintenanceItemsPage"] = "1";
            Session["MaintenanceCategoriesPage"] = "1";
            Session["MaintenanceCollectionPointsPage"] = "1";
            Session["MaintenanceDepartmentsPage"] = "1";
            Session["MaintenanceStoreBinPage"] = "1";
            Session["MaintenanceSuppliersPage"] = "1";
        }
    }
}
