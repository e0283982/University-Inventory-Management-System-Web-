using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{

    public class BinRepositoryImpl : IBinRepository, IDisposable
    {

        private SSISdbEntities context;

        public BinRepositoryImpl(SSISdbEntities context)
        {
            this.context = context;
        }
        public void DeleteBin(int binId)
        {
            Bin bin = context.Bins.Find(binId);
            context.Bins.Remove(bin);
        }
        public Bin GetBinById(int binId)
        {
            return context.Bins.Find(binId);
        }
        public IEnumerable<Bin> GetBins()
        {
            return context.Bins.ToList();
        }

        public void InsertBin(Bin bin)
        {
            context.Bins.Add(bin);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void UpdateBin(Bin bin)
        {
            context.Entry(bin).State = EntityState.Modified;
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
    }
}