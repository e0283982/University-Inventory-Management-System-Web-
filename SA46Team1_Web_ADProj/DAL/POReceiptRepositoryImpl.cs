using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class POReceiptRepositoryImpl : IPOReceiptRepository, IDisposable
    {
        private SSISdbEntities context;
        public POReceiptRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<POReceiptHeader> GetPOReceiptHeaders()
        {
            return context.POReceiptHeaders.ToList();
        }

        public POReceiptHeader GetPOReceiptHeaderById(int receiptNo)
        {
            return context.POReceiptHeaders.Find(receiptNo);
        }

        public void InsertPOReceiptHeader(POReceiptHeader poReceiptHeader)
        {
            context.POReceiptHeaders.Add(poReceiptHeader);
        }

        public void DeletePOReceiptHeader(int receiptNo)
        {
            POReceiptHeader poReceiptHeader = context.POReceiptHeaders.Find(receiptNo);
            context.POReceiptHeaders.Remove(poReceiptHeader);
        }

        public void UpdatePOReceiptHeader(POReceiptHeader poReceiptHeader)
        {
            context.Entry(poReceiptHeader).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}