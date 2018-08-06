using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.DAL
{
    interface IPODetailsRepository : IDisposable
    {
        IEnumerable<PODetail> GetPODetails();

        PODetail GetPODetailById(string poNumber, string itemCode);

        void InsertPODetail(PODetail poDetail);

        void DeletePODetail(string poNumber, string itemCode);

        void UpdatePODetail(PODetail poDetail);

        void Save();

    }
}
