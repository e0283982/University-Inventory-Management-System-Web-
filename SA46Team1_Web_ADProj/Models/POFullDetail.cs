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
    
    public partial class POFullDetail
    {
        public string PONumber { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public int QuantityOrdered { get; set; }
        public string UoM { get; set; }
        public float UnitCost { get; set; }
        public Nullable<float> Total { get; set; }
        public System.DateTime Date { get; set; }
        public string EmployeeName { get; set; }
        public System.DateTime ReceivedDate { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
    }
}