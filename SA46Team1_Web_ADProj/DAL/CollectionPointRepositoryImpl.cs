using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class CollectionPointRepositoryImpl : ICollectionPointRepository, IDisposable
    {

        private SSISdbEntities context;
        public CollectionPointRepositoryImpl(SSISdbEntities context)
        {
            this.context = context;
        }

        public void DeleteCollectionPoint(string collectionId)
        {
            CollectionPoint collectionPoint = context.CollectionPoints.Find(collectionId);
            context.CollectionPoints.Remove(collectionPoint);
        }

        public CollectionPoint GetCollectionPointById(string collectionId)
        {
            return context.CollectionPoints.Find(collectionId);
        }

        public IEnumerable<CollectionPoint> GetCollectionPoints()
        {
            return context.CollectionPoints.ToList();
        }

        public void InsertCollectionPoint(CollectionPoint collectionPoint)
        {
            context.CollectionPoints.Add(collectionPoint);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void UpdateCollectionPoint(CollectionPoint collectionPoint)
        {
            context.Entry(collectionPoint).State = EntityState.Modified;
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