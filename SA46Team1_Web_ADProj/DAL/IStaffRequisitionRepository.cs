using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IStaffRequisitionRepository : IDisposable
    {
        IEnumerable<StaffRequisitionHeader> GetStaffRequisitionHeaders();

        StaffRequisitionHeader GetStaffRequisitionHeaderById(string formId);

        void InsertStaffRequisitionHeader(StaffRequisitionHeader staffRequisitionHeader);

        void DeleteStaffRequisitionHeader(string formId);

        void UpdateStaffRequisitionHeader(StaffRequisitionHeader staffRequisitionHeader);

        void Save();
    }
}
