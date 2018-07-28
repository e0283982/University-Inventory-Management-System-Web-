using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj.Models
{
    public class NewRequisitionModel
    {

        public string EmployeeId { get; set; }   
        public string ItemDescription { get; set; }
        public int OrderedQuantity { get; set; }
        public int RequisitionSize { get; set; }
        public int RequisitionId { get; set; }

    }
}