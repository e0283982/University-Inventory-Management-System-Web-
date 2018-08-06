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
    public class PODetailsRepositoryImpl : IPODetailsRepository, IDisposable
    {

        private SSISdbEntities context;
        public PODetailsRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<PODetail> GetPODetails()
        {
            return context.PODetails.ToList();
        }

        public PODetail GetPODetailById(string poNumber, string itemCode)
        {
            return context.PODetails.Where(x => x.PONumber == poNumber && x.ItemCode == itemCode).First();
        }

        public void InsertPODetail(PODetail poDetail)
        {
            context.PODetails.Add(poDetail);
        }

        public void DeletePODetail(string poNumber, string itemCode)
        {
            PODetail poDetail = context.PODetails.Where(x => x.PONumber == poNumber && x.ItemCode == itemCode).First();
            context.PODetails.Remove(poDetail);
        }

        public void UpdatePODetail(PODetail poDetail)
        {
            context.Entry(poDetail).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}