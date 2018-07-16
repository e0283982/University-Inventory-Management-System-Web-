using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWCFService" in both code and config file together.
    [ServiceContract]
    public interface IWCFService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/Item", ResponseFormat = WebMessageFormat.Json)]
        List<String> ListItem();

        [OperationContract]
        [WebGet(UriTemplate = "/StockAdjustmentList", ResponseFormat = WebMessageFormat.Json)]
        List<StockAdjustmentOverview> StockAdjustmentList();

    }
    
   
}
