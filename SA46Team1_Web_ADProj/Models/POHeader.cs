
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
    
public partial class POHeader
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public POHeader()
    {

        this.PODetails = new HashSet<PODetail>();

        this.POReceiptHeaders = new HashSet<POReceiptHeader>();

    }


    public string PONumber { get; set; }

    public System.DateTime Date { get; set; }

    public string SupplierCode { get; set; }

    public string ContactName { get; set; }

    public string DeliverTo { get; set; }

    public string EmployeeID { get; set; }

    public string Remarks { get; set; }

    public string Status { get; set; }

    public string TransactionType { get; set; }



    public virtual Employee Employee { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PODetail> PODetails { get; set; }

    public virtual Supplier Supplier { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<POReceiptHeader> POReceiptHeaders { get; set; }

}

}
