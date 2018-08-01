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
    
    public partial class StockRetrievalDetail
    {
        public string Id { get; set; }
        public int Bin { get; set; }
        public string ItemCode { get; set; }
        public int QuantityRetrieved { get; set; }
        public string CollectionPointID { get; set; }
        public int QuantityAdjusted { get; set; }
        public string Remarks { get; set; }
        public Nullable<byte> Collected { get; set; }
    
        public virtual CollectionPoint CollectionPoint { get; set; }
        public virtual Item Item { get; set; }
        public virtual StockRetrievalHeader StockRetrievalHeader { get; set; }
    }
}
