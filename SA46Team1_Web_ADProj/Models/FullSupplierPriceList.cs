using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj.Models
{
    public class FullSupplierPriceList
    {
        public string SupplierCode { get; set; }
        public string ItemCode { get; set; }
        public float UnitCost { get; set; }
        public string UoM { get; set; }
        public byte Active { get; set; }
        public string ItemDesc { get; set; }
    }
}