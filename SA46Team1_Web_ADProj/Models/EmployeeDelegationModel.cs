using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj.Models
{
    public class EmployeeDelegationModel
    {
        public string EmpId { get; set; }

        public string EmpName { get; set; }

        public string Role { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime FromDate { get; set; }

    }
}