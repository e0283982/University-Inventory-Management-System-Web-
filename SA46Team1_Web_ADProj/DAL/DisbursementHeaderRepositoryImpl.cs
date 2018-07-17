using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class DisbursementHeaderRepositoryImpl : IDisbursementHeaderRepository, IDisposable
    {
        private SSISdbEntities context;
        public DisbursementHeaderRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<DisbursementHeader> GetDisbursementHeaders()
        {
            return context.DisbursementHeaders.ToList();
        }

        public DisbursementHeader GetDisbursementHeaderById(string id)
        {
            return context.DisbursementHeaders.Find(id);
        }

        public void InsertDisbursementHeader(DisbursementHeader disbursementHeader)
        {
            context.DisbursementHeaders.Add(disbursementHeader);
        }

        public void DeleteDisbursementHeader(string id)
        {
            DisbursementHeader disbursementHeader = context.DisbursementHeaders.Find(id);
            context.DisbursementHeaders.Remove(disbursementHeader);
        }

        public void UpdateDisbursementHeader(DisbursementHeader disbursementHeader)
        {
            context.Entry(disbursementHeader).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}