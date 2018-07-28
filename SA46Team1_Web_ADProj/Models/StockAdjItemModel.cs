using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj.Models
{
    public class StockAdjItemModel
    {
        public string ItemCode { get; set; }

        public string Reason { get; set; }

        public string ItemDesc { get; set; }

        public string UOM { get; set; }

        public int AdjQty { get; set; }

        public double AdjCost { get; set; }

        public float AvgUnitCost { get; set; }

    }
}