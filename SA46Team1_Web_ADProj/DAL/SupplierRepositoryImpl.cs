using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class SupplierRepositoryImpl : ISupplierRepository, IDisposable
    {

        private SSISdbEntities context;
        public SupplierRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<Supplier> GetSuppliers()
        {
            return context.Suppliers.ToList();
        }

        public Supplier GetSupplierById(string supplierId)
        {
            return context.Suppliers.Find(supplierId);
        }

        public void InsertSupplier(Supplier supplier)
        {
            context.Suppliers.Add(supplier);
        }

        public void DeleteSupplier(string supplierId)
        {
            Supplier supplier = context.Suppliers.Find(supplierId);
            context.Suppliers.Remove(supplier);
        }

        public void UpdateSupplier(Supplier supplier)
        {
            context.Entry(supplier).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}