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
    
    public partial class StockRetrievalReqForm
    {
        public int Id { get; set; }
        public string StockRetrievalID { get; set; }
        public string ReqFormID { get; set; }
    
        public virtual StaffRequisitionHeader StaffRequisitionHeader { get; set; }
        public virtual StockRetrievalHeader StockRetrievalHeader { get; set; }
    }
}