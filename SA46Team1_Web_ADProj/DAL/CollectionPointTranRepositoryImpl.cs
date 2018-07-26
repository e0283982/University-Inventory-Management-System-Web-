using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class CollectionPointTranRepositoryImpl
        //: ICollectionPointTransRepository, IDisposable
    {
        private SSISdbEntities context;

        //public CollectionPointTranRepositoryImpl(SSISdbEntities context)
        //{
        //    this.context = context;
        //}

        //public void DeleteCollectionPointTran(string departmentCode, string collectionId)
        //{
        //    CollectionPointTran collectionPointTran = context.CollectionPointTrans.Where(x => x.DepartmentCode == departmentCode
        //                                                && x.CollectionPointID == collectionId).First();
        //    context.CollectionPointTrans.Remove(collectionPointTran);
        //}        

        //public CollectionPointTran GetCollectionPointTranById(string departmentCode, string collectionId)
        //{
        //    return context.CollectionPointTrans.Where(x => x.DepartmentCode == departmentCode
        //                                                && x.CollectionPointID == collectionId).First();
        //}

        //public IEnumerable<CollectionPointTran> GetCollectionPointTrans()
        //{
        //    return context.CollectionPointTrans.ToList();
        //}

        //public void InsertCollectionPointTran(CollectionPointTran collectionPointTran)
        //{
        //    context.CollectionPointTrans.Add(collectionPointTran);
        //}

        //public void Save()
        //{
        //    context.SaveChanges();
        //}

        //public void UpdateCollectionPointTran(CollectionPointTran collectionPointTran)
        //{
        //    context.Entry(collectionPointTran).State = EntityState.Modified;
        //}

        //private bool disposed = false;
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!this.disposed)
        //    {
        //        if (disposing)
        //        {
        //            context.Dispose();
        //        }
        //    }
        //    this.disposed = true;
        //}
        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
    }
}