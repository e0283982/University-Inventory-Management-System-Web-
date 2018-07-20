using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj.Models
{
    public class BackOrderModel
    {
        public string ItemCode { get; set; }

        public string ReqId { get; set; }

        public string ItemDesc { get; set; }

        public string UOM { get; set; }

        public int OutstandingQty { get; set; }

    }
}