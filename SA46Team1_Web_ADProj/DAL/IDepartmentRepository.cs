using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.DAL
{
    interface IDepartmentRepository : IDisposable
    {
        IEnumerable<Department> GetDepartments();

        Department GetDepartmentById(string code);

        void InsertDepartment(Department department);

        void DeleteDepartment(string code);

        void UpdateDepartment(Department department);

        void Save();

    }
}
