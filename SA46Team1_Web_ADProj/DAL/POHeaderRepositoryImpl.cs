using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class POHeaderRepositoryImpl : IPOHeaderRepository, IDisposable
    {
        private SSISdbEntities context;
        public POHeaderRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<POHeader> GetPOHeader()
        {
            return context.POHeaders.ToList();
        }

        public POHeader GetPOHeaderById(string poNumber)
        {
            return context.POHeaders.Find(poNumber);
        }

        public void InsertPOHeader(POHeader poHeader)
        {
            context.POHeaders.Add(poHeader);
        }

        public void DeletePOHeader(string poNumber)
        {
            POHeader poHeader = context.POHeaders.Find(poNumber);
            context.POHeaders.Remove(poHeader);
        }

        public void UpdatePOHeader(POHeader poHeader)
        {
            context.Entry(poHeader).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}