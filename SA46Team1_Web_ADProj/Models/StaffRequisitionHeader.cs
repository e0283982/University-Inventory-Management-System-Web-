
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
    
public partial class StaffRequisitionHeader
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public StaffRequisitionHeader()
    {

        this.DisbursementHeaders = new HashSet<DisbursementHeader>();

        this.StaffRequisitionDetails = new HashSet<StaffRequisitionDetail>();

        this.StockRetrievalHeaders = new HashSet<StockRetrievalHeader>();

    }


    public string FormID { get; set; }

    public string DepartmentCode { get; set; }

    public string EmployeeID { get; set; }

    public System.DateTime DateRequested { get; set; }

    public string Approver { get; set; }

    public System.DateTime DateProcessed { get; set; }

    public string Status { get; set; }

    public string ApprovalStatus { get; set; }

    public string Remarks { get; set; }

    public string NotificationStatus { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DisbursementHeader> DisbursementHeaders { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<StaffRequisitionDetail> StaffRequisitionDetails { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<StockRetrievalHeader> StockRetrievalHeaders { get; set; }

}

}
