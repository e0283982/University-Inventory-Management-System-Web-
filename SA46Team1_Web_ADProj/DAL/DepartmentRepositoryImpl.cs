using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SA46Team1_Web_ADProj.Models;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.DAL
{   
    public class DepartmentRepositoryImpl : IDepartmentRepository, IDisposable
    {

        private SSISdbEntities context;
        public DepartmentRepositoryImpl(SSISdbEntities context)
        {
            this.context = context;
        }

        public void DeleteDepartment(string code)
        {
            Department department = context.Departments.Find(code);
            context.Departments.Remove(department);
        }
        public Department GetDepartmentById(string code)
        {
            return context.Departments.Find(code);
        }

        public IEnumerable<Department> GetDepartments()
        {
            return context.Departments.ToList();
        }

        public void InsertDepartment(Department department)
        {
            context.Departments.Add(department);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void UpdateDepartment(Department department)
        {
            context.Entry(department).State = EntityState.Modified;
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