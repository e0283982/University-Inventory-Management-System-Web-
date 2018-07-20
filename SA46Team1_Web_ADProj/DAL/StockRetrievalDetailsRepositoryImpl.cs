using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class StockRetrievalDetailsRepositoryImpl : IStockRetrievalDetailsRepository, IDisposable
    {
        private SSISdbEntities context;
        public StockRetrievalDetailsRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<StockRetrievalDetail> GetStockRetrievalDetails()
        {
            return context.StockRetrievalDetails.ToList();
        }

        public StockRetrievalDetail GetStockRetrievalDetailById(string id, int binid)
        {
            return context.StockRetrievalDetails.Where(x => x.Id == id && x.Bin == binid).First();
        }

        public void InsertStockRetrievalDetail(StockRetrievalDetail stockRetrievalDetail)
        {
            context.StockRetrievalDetails.Add(stockRetrievalDetail);
        }

        public void DeleteStockRetrievalDetail(string id, int binid)
        {
           StockRetrievalDetail stockRetrievalDetail = context.StockRetrievalDetails.Where(x => x.Id == id && x.Bin == binid).First();
            context.StockRetrievalDetails.Remove(stockRetrievalDetail);
        }

        public void UpdateStockRetrievalDetail(StockRetrievalDetail stockRetrievalDetail)
        {
            context.Entry(stockRetrievalDetail).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}