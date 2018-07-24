using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj.Models
{
    public class DeptFullDetailsModel
    {
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public string ContactName { get; set; }
        public int TelephoneNo { get; set; }
        public Nullable<int> FaxNo { get; set; }
        public string CollectionPointName { get; set; }
        public string RepresentativeID { get; set; }
        public string ApproverID { get; set; }
        public string RepresentativeName { get; set; }
        public string ApproverName { get; set; }
        public byte Active { get; set; }
        
    }
}