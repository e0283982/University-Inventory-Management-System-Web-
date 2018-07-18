using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj
{
    //This is just a temporary class for Hendri to get data
    public class HWTempData
    {

        public static List<String> Test2()
        {            
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.StockAdjustmentDetails.Select(x => x.ItemCode).ToList<String>();
            }
        }

        public static List<StockAdjustmentOverview> GetStockAdjustmentOverviewList(string requestorId)
        {
            using (SSISdbEntities m = new SSISdbEntities())
            {
                return m.StockAdjustmentOverviews.Where(x => x.Requestor == requestorId).ToList<StockAdjustmentOverview>();
            }
        }

        //To delete
        public static List<Employee> GetAllEmployees()
        {
            string id = "E1";
            using (SSISdbEntities m = new SSISdbEntities())
            {                
                return m.Employees.Where(x => x.EmployeeID == id).ToList<Employee>();
            }

        }

        public static void InsertStockAdjustment(StockAdjustmentHeader stockAdjustmentHeader)
        {

            using (SSISdbEntities m = new SSISdbEntities())
            {
                m.StockAdjustmentHeaders.Add(stockAdjustmentHeader);
                m.SaveChanges();
            }



        }
        

    }
}