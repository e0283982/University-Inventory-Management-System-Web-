using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class StaffRequisitionDetailsRepositoryImpl : IStaffRequisitionDetailsRepository, IDisposable
    {
        private SSISdbEntities context;
        public StaffRequisitionDetailsRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<StaffRequisitionDetail> GetStaffRequisitionDetails()
        {
            return context.StaffRequisitionDetails.ToList();
        }

        public StaffRequisitionDetail GetStaffRequisitionDetailById(string formId, string itemCode)
        {
            return context.StaffRequisitionDetails.Where(x => x.FormID == formId && x.ItemCode == itemCode).First();
        }

        public void InsertStaffRequisitionDetail(StaffRequisitionDetail staffRequisitionDetail)
        {
            context.StaffRequisitionDetails.Add(staffRequisitionDetail);
        }

        public void DeleteStaffRequisitionDetail(string formId, string itemCode)
        {
            StaffRequisitionDetail staffRequisitionDetail = context.StaffRequisitionDetails.Where(x => x.FormID == formId && x.ItemCode == itemCode).First();
            context.StaffRequisitionDetails.Remove(staffRequisitionDetail);
        }

        public void UpdateStaffRequisitionDetail(StaffRequisitionDetail staffRequisitionDetail)
        {
            context.Entry(staffRequisitionDetail).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}