using System;
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
            //Session variables for inner pages of Store tabs
            Session["StockAdjPage"] = "1";
            Session["SelectedPONumber"] = "0";
            Session["POListPage"] = "1";
            Session["GRListPage"] = "1";
            Session["ReqListPage"] = "1";
            Session["DisbursementListPage"] = "1";
            Session["MaintenanceItemsPage"] = "1";
            Session["MaintenanceCategoriesPage"] = "1";
            Session["MaintenanceCollectionPointsPage"] = "1";
            Session["MaintenanceDepartmentsPage"] = "1";
            Session["MaintenanceStoreBinPage"] = "1";
            Session["MaintenanceSuppliersPage"] = "1";
        }
    }
}
