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
    public class StockCountRepositoryImpl : IStockCountRepository, IDisposable
    {
        private SSISdbEntities context;
        public StockCountRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<StockCount> GetStockCounts()
        {
            return context.StockCounts.ToList();
        }

        public StockCount GetStockCountByYearMonthItem(int year, int month, string itemcode)
        {
            return context.StockCounts.Where(x => x.Year == year && x.Month == month && x.ItemCode == itemcode).First();
        }

        public void InsertStockCount(StockCount stockCount)
        {
            context.StockCounts.Add(stockCount);
        }

        public void UpdateStockCount(StockCount stockCount)
        {
            context.Entry(stockCount).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}