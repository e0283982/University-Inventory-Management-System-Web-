//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SA46Team1_Web_ADProj.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class StockAdjustmentDetail
    {
        public string RequestId { get; set; }
        public string ItemCode { get; set; }
        public int ItemQuantity { get; set; }
        public float Amount { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual StockAdjustmentHeader StockAdjustmentHeader { get; set; }
        [NotMapped]
        public List<StockAdjustmentDetail> StockAdjustmentDetails { get; set; }
    }
}
