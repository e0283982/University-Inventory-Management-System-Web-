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
    public class StockTakeHeaderRepositoryImpl : IStockTakeHeaderRepository, IDisposable
    {
        private SSISdbEntities context;
        public StockTakeHeaderRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<StockTakeHeader> GetStockTakeHeaders()
        {
            return context.StockTakeHeaders.ToList();
        }

        public StockTakeHeader GetStockTakeHeaderById(int stockTakeId)
        {
            return context.StockTakeHeaders.Find(stockTakeId);
        }

        public void InsertStockTakeHeader(StockTakeHeader stockTakeHeader)
        {
            context.StockTakeHeaders.Add(stockTakeHeader);
        }

        public void DeleteStockTakeHeader(int stockTakeId)
        {
            StockTakeHeader stockTakeHeader = context.StockTakeHeaders.Find(stockTakeId);
            context.StockTakeHeaders.Remove(stockTakeHeader);
        }

        public void UpdateStockTakeHeader(StockTakeHeader stockTakeHeader)
        {
            context.Entry(stockTakeHeader).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}