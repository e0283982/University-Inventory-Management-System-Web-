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
    
    public partial class DepartmentDetail
    {
        public string DepartmentCode { get; set; }
        public string ContactID { get; set; }
        public int TelephoneNo { get; set; }
        public Nullable<int> FaxNo { get; set; }
        public string CollectionPointID { get; set; }
        public string RepresentativeID { get; set; }
        public string ApproverID { get; set; }
        public byte Active { get; set; }
    
        public virtual CollectionPoint CollectionPoint { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        public virtual Employee Employee2 { get; set; }
    }
}
