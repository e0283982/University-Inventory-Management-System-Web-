using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class EmployeeRepositoryImpl : IEmployeeRepository, IDisposable
    {
        private SSISdbEntities context;
        public EmployeeRepositoryImpl(SSISdbEntities context)
        {
            this.context = context;
        }

        public void DeleteEmployee(string employeeId)
        {
            Employee employee = context.Employees.Find(employeeId);
            context.Employees.Remove(employee);
        }

        public Employee GetEmployeeById(string employeeId)
        {
            return context.Employees.Find(employeeId);
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return context.Employees.ToList();
        }

        public void InsertEmployee(Employee employee)
        {
            context.Employees.Add(employee);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void UpdateEmployee(Employee employee)
        {
            context.Entry(employee).State = EntityState.Modified;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}