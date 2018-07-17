using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj
{
    //This is just a temporary class for Hendri to get data
    public class JWTempData
    {
        public static List<String> Test2()
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.InventoryOverviews.Select(x => x.ItemCode).ToList<String>();
            }
        }

        public static List<InventoryOverview> InventoryOverviewList(string itemcode)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.InventoryOverviews.Where(x => x.ItemCode == itemcode).ToList<InventoryOverview>();
            }
        }
    }
}