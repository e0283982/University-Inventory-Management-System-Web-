using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MyServiceWCF" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MyServiceWCF.svc or MyServiceWCF.svc.cs at the Solution Explorer and start debugging.
    public class MyServiceWCF : IMyServiceWCF
    {
        public List<GoodsReceivedList> GoodsRecList()
        {
            string itemcode = "C001";
            return JWTempData.GoodsReceivedLists(itemcode);
        }

        public List<InventoryOverview> InventoryList()
        {
            //Temporary placeholder to make the requestID = 1
            string itemcode = "C001";
            return JWTempData.InventoryOverviewList(itemcode);
        }

        public List<string> ListInventory()
        {
            return JWTempData.Test2();
        }
    }
}
