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
    public class StockAdjustmentRepositoryImpl : IStockAdjustmentRepository, IDisposable
    {
        private SSISdbEntities context;
        public StockAdjustmentRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<StockAdjustmentHeader> GetStockAdjustmentHeaders()
        {
            return context.StockAdjustmentHeaders.ToList();
        }

        public StockAdjustmentHeader GetStockAdjustmentHeaderById(int requestid)
        {
            return context.StockAdjustmentHeaders.Find(requestid);
        }

        public void InsertStockAdjustmentHeader(StockAdjustmentHeader stockAdjustmentHeader)
        {
            context.StockAdjustmentHeaders.Add(stockAdjustmentHeader);
        }

        public void DeleteStockAdjustmentHeader(int requestid)
        {
            StockAdjustmentHeader stockAdjustment = context.StockAdjustmentHeaders.Find(requestid);
            context.StockAdjustmentHeaders.Remove(stockAdjustment);
        }

        public void UpdateStockAdjustmentHeader(StockAdjustmentHeader stockAdjustmentHeader)
        {
            context.Entry(stockAdjustmentHeader).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}