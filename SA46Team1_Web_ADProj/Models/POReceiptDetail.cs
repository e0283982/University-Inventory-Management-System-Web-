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
    
    public partial class POReceiptDetail
    {
        public int ReceiptNo { get; set; }
        public string PONumber { get; set; }
        public string ItemCode { get; set; }
        public int QuantityReceived { get; set; }
        public float UnitCost { get; set; }
        public float Amount { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual POReceiptHeader POReceiptHeader { get; set; }
    }
}