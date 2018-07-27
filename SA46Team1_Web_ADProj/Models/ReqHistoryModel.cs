using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj.Models
{
    public class ReqHistoryModel
    {
        public string ApprovalStatus { get; set; }

        public DateTime RequestDate { get; set; }

        public string RepName { get; set; }

        public string ApproverName { get; set; }

        public string Status { get; set; }
    }
}