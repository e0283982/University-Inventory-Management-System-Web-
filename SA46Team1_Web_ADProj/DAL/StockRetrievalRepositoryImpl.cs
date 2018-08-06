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
    public class StockRetrievalRepositoryImpl : IStockRetrievalRepository, IDisposable
    {
        private SSISdbEntities context;
        public StockRetrievalRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<StockRetrievalHeader> GetStockRetrievalHeaders()
        {
            return context.StockRetrievalHeaders.ToList();
        }

        public StockRetrievalHeader GetStockRetrievalHeaderById(int id)
        {
            return context.StockRetrievalHeaders.Find(id);
        }

        public void InsertStockRetrievalHeader(StockRetrievalHeader stockRetrievalHeader)
        {
            context.StockRetrievalHeaders.Add(stockRetrievalHeader);
        }

        public void DeleteStockRetrievalHeader(int id)
        {
            StockRetrievalHeader stockRetrievalHeader = context.StockRetrievalHeaders.Find(id);
            context.StockRetrievalHeaders.Remove(stockRetrievalHeader);
        }

        public void UpdateStockRetrievalHeader(StockRetrievalHeader stockRetrievalHeader)
        {
            context.Entry(stockRetrievalHeader).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}