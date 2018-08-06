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
    interface IStaffRequisitionDetailsRepository : IDisposable
    {
        IEnumerable<StaffRequisitionDetail> GetStaffRequisitionDetails();

        StaffRequisitionDetail GetStaffRequisitionDetailById(string formId, string itemCode);

        void InsertStaffRequisitionDetail(StaffRequisitionDetail staffRequisitionDetail);

        void DeleteStaffRequisitionDetail(string formId, string itemCode);

        void UpdateStaffRequisitionDetail(StaffRequisitionDetail staffRequisitionDetail);

        void Save();
    }
}
