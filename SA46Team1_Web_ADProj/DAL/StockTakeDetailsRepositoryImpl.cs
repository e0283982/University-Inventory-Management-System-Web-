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
    public class StockTakeDetailsRepositoryImpl : IStockTakeDetailsRepository, IDisposable
    {
        private SSISdbEntities context;
        public StockTakeDetailsRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<StockTakeDetail> GetStockTakeDetails()
        {
            return context.StockTakeDetails.ToList();
        }

        public StockTakeDetail GetStockTakeDetailById(string stockTakeId, string itemCode)
        {
            return context.StockTakeDetails.Where(x => x.StockTakeID == stockTakeId && x.ItemCode == itemCode).First();
        }

        public void InsertStockTakeDetail(StockTakeDetail stockTakeDetail)
        {
            context.StockTakeDetails.Add(stockTakeDetail);
        }

        public void DeleteStockTakeDetail(string stockTakeId, string itemCode)
        {
            StockTakeDetail stockTakeDetail = context.StockTakeDetails.Where(x => x.StockTakeID == stockTakeId && x.ItemCode == itemCode).First();
            context.StockTakeDetails.Remove(stockTakeDetail);
        }

        public void UpdateStockTakeDetail(StockTakeDetail stockTakeDetail)
        {
            context.Entry(stockTakeDetail).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}