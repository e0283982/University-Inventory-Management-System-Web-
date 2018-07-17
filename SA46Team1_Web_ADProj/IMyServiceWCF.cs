using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Services;

namespace SA46Team1_Web_ADProj
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMyServiceWCF" in both code and config file together.
    [ServiceContract]
    public interface IMyServiceWCF
    {
        [OperationContract]
        [WebGet(UriTemplate = "/InventoryOverviewWCF", ResponseFormat = WebMessageFormat.Json)]
        List<String> ListInventory();

        [OperationContract]
        [WebGet(UriTemplate = "/InventoryWCFList", ResponseFormat = WebMessageFormat.Json)]
        List<InventoryOverview> InventoryList();
    }
}
