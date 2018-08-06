using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.DAL
{
    public class StaffRequisitionRepositoryImpl : IStaffRequisitionRepository, IDisposable
    {
        private SSISdbEntities context;
        public StaffRequisitionRepositoryImpl(SSISdbEntities context)
        {
            this.context = context;
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

        public IEnumerable<StaffRequisitionHeader> GetStaffRequisitionHeaders()
        {
            return context.StaffRequisitionHeaders.ToList();
        }

        public StaffRequisitionHeader GetStaffRequisitionHeaderById(string formId)
        {
            return context.StaffRequisitionHeaders.Find(formId);
        }

        public void InsertStaffRequisitionHeader(StaffRequisitionHeader staffRequisitionHeader)
        {
            context.StaffRequisitionHeaders.Add(staffRequisitionHeader);
        }

        public void DeleteStaffRequisitionHeader(string formId)
        {
            StaffRequisitionHeader staffRequisitionHeader = context.StaffRequisitionHeaders.Find(formId);
            context.StaffRequisitionHeaders.Remove(staffRequisitionHeader);
        }

        public void UpdateStaffRequisitionHeader(StaffRequisitionHeader staffRequisitionHeader)
        {
            context.Entry(staffRequisitionHeader).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}