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
    public class POReceiptDetailsRepositoryImpl : IPOReceiptDetailsRepository, IDisposable
    {
        private SSISdbEntities context;
        public POReceiptDetailsRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<POReceiptDetail> GetPOReceiptDetails()
        {
            return context.POReceiptDetails.ToList();
        }

        public POReceiptDetail GetPOReceiptDetailById(string receiptNo, string itemCode)
        {
            return context.POReceiptDetails.Where(x => x.ReceiptNo == receiptNo && x.ItemCode == itemCode).First();
        }

        public void InsertPOReceiptDetail(POReceiptDetail poReceiptDetail)
        {
            context.POReceiptDetails.Add(poReceiptDetail);
        }

        public void DeletePOReceiptDetail(string receiptNo, string itemCode)
        {
            POReceiptDetail poReceiptDetail = context.POReceiptDetails.Where(x => x.ReceiptNo == receiptNo && x.ItemCode == itemCode).First();
            context.POReceiptDetails.Remove(poReceiptDetail);
        }

        public void UpdatePOReceiptDetail(POReceiptDetail poReceiptDetail)
        {
            context.Entry(poReceiptDetail).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}