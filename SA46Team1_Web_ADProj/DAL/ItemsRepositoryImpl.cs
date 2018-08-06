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
    public class ItemsRepositoryImpl : IItemsRepository, IDisposable
    {
        private SSISdbEntities context;

        public ItemsRepositoryImpl(SSISdbEntities context)
        {
            this.context = context;
        }
        public IEnumerable<Item> GetItems()
        {
            return context.Items.ToList();
        }

        public IEnumerable<string> GetItemIds(string search)
        {
            return context.Items.Where(x => x.ItemCode.StartsWith(search, StringComparison.OrdinalIgnoreCase)).Select(x => x.ItemCode).ToList();
        }

        public Item GetItemById(string itemCode)
        {
            return context.Items.Find(itemCode);
        }

        public void InsertItem(Item item)
        {
            Item newItem = context.Items.Add(item);
        }

        public void DeleteItem(string itemCode)
        {
            Item item = context.Items.Find(itemCode);
            context.Items.Remove(item);
        }

        public void UpdateItem(Item item)
        {
            context.Entry(item).State = EntityState.Modified;
        }
        public void Save()
        {
            context.SaveChanges();
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