using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;


namespace SA46Team1_Web_ADProj.DAL
{
    interface IItemsRepository : IDisposable
    {
        IEnumerable<Item> GetItems();
        Item GetItemById(string itemCode);
        void InsertItem(Item item);
        void DeleteItem(string itemCode);
        void UpdateItem(Item item);
        void Save();
    }
}
