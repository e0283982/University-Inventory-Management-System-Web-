using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class SupplierPriceListRepositoryImpl : ISupplierPriceListRepository, IDisposable
    {
        private SSISdbEntities context;
        public SupplierPriceListRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<SupplierPriceList> GetSupplierPriceLists()
        {
            return context.SupplierPriceLists.ToList();
        }

        public SupplierPriceList GetSupplierPriceListById(string supplierCode, string itemCode)
        {
            return context.SupplierPriceLists.Where(x => x.SupplierCode == supplierCode && x.ItemCode == itemCode).First();
        }

        public void InsertSupplierPriceList(SupplierPriceList supplierPriceList)
        {
            context.SupplierPriceLists.Add(supplierPriceList);
        }

        public void DeleteSupplierPriceList(string supplierCode, string itemCode)
        {
            SupplierPriceList supplierPriceList = context.SupplierPriceLists.Where(x => x.SupplierCode == supplierCode && x.ItemCode == itemCode).First();
            context.SupplierPriceLists.Remove(supplierPriceList);
        }

        public void UpdateSupplierPriceList(SupplierPriceList supplierPriceList)
        {
            context.Entry(supplierPriceList).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}