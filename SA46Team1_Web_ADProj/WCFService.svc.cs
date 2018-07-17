using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WCFService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WCFService.svc or WCFService.svc.cs at the Solution Explorer and start debugging.
    public class WCFService : IWCFService
    {
        public List<String> ListItem()
        {
            return HWTempData.Test2();

        }

        public List<StockAdjustmentOverview> StockAdjustmentList()
        {
            //Temporary placeholder to make the requestID = 1
            string requestorID = "E1";
            return HWTempData.GetStockAdjustmentOverviewList(requestorID);
           
           
        }
        
        

    }
}
