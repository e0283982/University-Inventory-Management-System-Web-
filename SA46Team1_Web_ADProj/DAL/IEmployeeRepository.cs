using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IEmployeeRepository : IDisposable
    {
        IEnumerable<Employee> GetEmployees();

        Employee GetEmployeeById(string employeeId);

        void InsertEmployee(Employee employee);

        void DeleteEmployee(string employeeId);

        void UpdateEmployee(Employee employee);

        Employee FindEmployeeEmailId(string emailId);

        void Save();

    }
}
