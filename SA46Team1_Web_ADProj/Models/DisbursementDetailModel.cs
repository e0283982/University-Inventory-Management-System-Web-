using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj.Models
{
    public class DisbursementDetailModel
    {
        public string DisbursementId { get; set; }
        public string ItemDescription { get; set; }
        public int QuantityReceived { get; set; }
        public int QuantityAdjusted { get; set; }
    


    }
}