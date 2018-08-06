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
    interface IDepartmentDetailsRepository : IDisposable
    {
        IEnumerable<DepartmentDetail> GetDepartmentDetails();

        DepartmentDetail GetDepartmentDetailById(string code);

        void InsertDepartmentDetail(DepartmentDetail departmentDetail);

        void DeleteDepartmentDetail(string code);

        void UpdateDepartmentDetail(DepartmentDetail departmentDetail);

        void Save();

    }
}
