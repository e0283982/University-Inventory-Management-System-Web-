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
    class ItemTransactionRepositoryImpl : IItemTransactionRepository, IDisposable
    {
        private SSISdbEntities context;
        public ItemTransactionRepositoryImpl(SSISdbEntities context)
        {
            this.context = context;
        }

        public IEnumerable<ItemTransaction> GetItemTransactions()
        {
            return context.ItemTransactions.ToList();
        }
        public void DeleteItemTransaction(DateTime date, string DocumentRefNo)
        {
            ItemTransaction itemTransaction = context.ItemTransactions.Where(x => x.TransDateTime == date && x.DocumentRefNo == DocumentRefNo).First();
            context.ItemTransactions.Remove(itemTransaction);
        }
        public ItemTransaction GetItemTransactionByID(DateTime date, string DocumentRefNo)
        {
            return context.ItemTransactions.Where(x => x.TransDateTime == date && x.DocumentRefNo == DocumentRefNo).First();
        }
        public void InsertItemTransaction(ItemTransaction itemTransaction)
        {
            context.ItemTransactions.Add(itemTransaction);
        }
        public void Save()
        {
            context.SaveChanges();
        }
        public void UpdateItemTransaction(ItemTransaction itemTransaction)
        {
            context.Entry(itemTransaction).State = EntityState.Modified;
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
