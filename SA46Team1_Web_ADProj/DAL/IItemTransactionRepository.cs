using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IItemTransactionRepository: IDisposable
    {
        IEnumerable<ItemTransaction> GetItemTransactions();
        ItemTransaction GetItemTransactionByID(DateTime date, string DocumentRefNo);      
        void InsertItemTransaction(ItemTransaction itemTransaction);
        void DeleteItemTransaction(DateTime date, string DocumentRefNo);
        void UpdateItemTransaction(ItemTransaction itemTransaction);
        void Save();
    }
}
