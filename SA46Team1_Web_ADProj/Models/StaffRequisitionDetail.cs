
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
    
public partial class StaffRequisitionDetail
{

    public string FormID { get; set; }

    public string ItemCode { get; set; }

    public int QuantityOrdered { get; set; }

    public int QuantityDelivered { get; set; }

    public int QuantityBackOrdered { get; set; }

    public int CancelledBackOrdered { get; set; }



    public virtual Item Item { get; set; }

    public virtual StaffRequisitionHeader StaffRequisitionHeader { get; set; }

}

}
