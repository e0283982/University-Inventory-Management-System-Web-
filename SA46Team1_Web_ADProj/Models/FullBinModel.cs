using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj.Models
{
    public class FullBinModel
    {
        public int Number { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }

        public byte Active { get; set; }
        public string Location { get; set; }

    }
}