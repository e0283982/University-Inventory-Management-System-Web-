using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SA46Team1_Web_ADProj.Models;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.DAL
{
    public class StockAdjustmentDetailsRepositoryImpl : IStockAdjustmentDetailsRepository, IDisposable
    {
        private SSISdbEntities context;
        public StockAdjustmentDetailsRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<StockAdjustmentDetail> GetStockAdjustmentDetails()
        {
            return context.StockAdjustmentDetails.ToList();
        }

        public StockAdjustmentDetail GetStockAdjustmentDetailById(string requestid, string itemcode)
        {
            return context.StockAdjustmentDetails.Where(x => x.RequestId == requestid && x.ItemCode == itemcode).First();
        }

        public void InsertStockAdjustmentDetail(StockAdjustmentDetail stockAdjustmentDetail)
        {
            context.StockAdjustmentDetails.Add(stockAdjustmentDetail);
        }

        public void DeleteStockAdjustmentDetail(string requestid, string itemcode)
        {
            StockAdjustmentDetail stockAdjustmentDetail =  context.StockAdjustmentDetails.Where(x => x.RequestId == requestid && x.ItemCode == itemcode).First();
            context.StockAdjustmentDetails.Remove(stockAdjustmentDetail);
        }

        public void UpdateStockAdjustmentDetail(StockAdjustmentDetail stockAdjustmentDetail)
        {
            context.Entry(stockAdjustmentDetail).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}