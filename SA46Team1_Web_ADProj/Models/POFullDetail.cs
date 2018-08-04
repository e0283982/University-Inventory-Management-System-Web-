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

    public partial class POFullDetail
    {
        public string PONumber { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public float UnitCost { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityDelivered { get; set; }
        public string UoM { get; set; }
        public System.DateTime Date { get; set; }
        public string EmployeeName { get; set; }
        public string Status { get; set; }

        [NotMapped]
        public string Supplier1Code { get; set; }

        [NotMapped]
        public string Supplier2Code { get; set; }

        [NotMapped]
        public string Supplier3Code { get; set; }

        [NotMapped]
        public float Supplier1UnitCost { get; set; }

        [NotMapped]
        public float Supplier2UnitCost { get; set; }

        [NotMapped]
        public float Supplier3UnitCost { get; set; }
    }
}
