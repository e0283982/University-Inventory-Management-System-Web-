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
    
    public partial class StockAdjustmentHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StockAdjustmentHeader()
        {
            this.StockAdjustmentDetails = new HashSet<StockAdjustmentDetail>();
        }
    
        public int RequestId { get; set; }
        public System.DateTime DateRequested { get; set; }
        public string Requestor { get; set; }
        public string Approver { get; set; }
        public Nullable<System.DateTime> DateProcessed { get; set; }
        public string Status { get; set; }
        public string TransactionType { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockAdjustmentDetail> StockAdjustmentDetails { get; set; }
    }
}