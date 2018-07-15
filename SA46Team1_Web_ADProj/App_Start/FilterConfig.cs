using System.Web;
using System.Web.Mvc;

namespace SA46Team1_Web_ADProj
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
