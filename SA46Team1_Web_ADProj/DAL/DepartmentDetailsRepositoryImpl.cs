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
    public class DepartmentDetailsRepositoryImpl : IDepartmentDetailsRepository, IDisposable
    {

        private SSISdbEntities context;
        public DepartmentDetailsRepositoryImpl(SSISdbEntities context)
        {
            this.context = context;
        }
        public void DeleteDepartmentDetail(string code)
        {
            DepartmentDetail departmentDetail = context.DepartmentDetails.Find(code);
            context.DepartmentDetails.Remove(departmentDetail);
        }

        public DepartmentDetail GetDepartmentDetailById(string code)
        {
            return context.DepartmentDetails.Find(code);
        }
        public IEnumerable<DepartmentDetail> GetDepartmentDetails()
        {
            return context.DepartmentDetails.ToList();
        }
        public void InsertDepartmentDetail(DepartmentDetail departmentDetail)
        {
            context.DepartmentDetails.Add(departmentDetail);
        }
        public void Save()
        {
            context.SaveChanges();
        }

        public void UpdateDepartmentDetail(DepartmentDetail departmentDetail)
        {
            context.Entry(departmentDetail).State = EntityState.Modified;
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