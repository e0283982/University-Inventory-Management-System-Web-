using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj.Models
{
    public class StockAdjustmentModel
    {
        public string RequestorId { get; set; }
        public string ItemDescription { get; set; }
        public int AdjustedQuantity { get; set; }
        public string Remarks { get; set; }

    }
}