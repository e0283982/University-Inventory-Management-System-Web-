using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class DisbursementDetailsRepositoryImpl : IDisbursementDetailsRepository, IDisposable
    {
        private SSISdbEntities context;
        public DisbursementDetailsRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<DisbursementDetail> GetDisbursementDetails()
        {
            return context.DisbursementDetails.ToList();
        }

        public DisbursementDetail GetDisbursementDetailById(string id, string itemCode)
        {
            return context.DisbursementDetails.Where(x => x.Id == id && x.ItemCode == itemCode).First();
        }

        public void InsertDisbursementDetail(DisbursementDetail disbursementDetail)
        {
            context.DisbursementDetails.Add(disbursementDetail);
        }

        public void DeleteDisbursementDetail(string id, string itemCode)
        {
            DisbursementDetail disbursementDetail = context.DisbursementDetails.Where(x => x.Id == id && x.ItemCode == itemCode).First();
            context.DisbursementDetails.Remove(disbursementDetail);
        }

        public void UpdateDisbursementDetail(DisbursementDetail disbursementDetail)
        {
            context.Entry(disbursementDetail).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
