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
    interface IDisbursementDetailsRepository : IDisposable
    {
        IEnumerable<DisbursementDetail> GetDisbursementDetails();

        DisbursementDetail GetDisbursementDetailById(string id, string itemCode);

        void InsertDisbursementDetail(DisbursementDetail disbursementDetail);

        void DeleteDisbursementDetail(string id, string itemCode);

        void UpdateDisbursementDetail(DisbursementDetail disbursementDetail);

        void Save();
    }
}
