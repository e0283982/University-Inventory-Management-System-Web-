using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IStockRetrievalRepository : IDisposable
    {
        IEnumerable<StockRetrievalHeader> GetStockRetrievalHeaders();

        StockRetrievalHeader GetStockRetrievalHeaderById(int id);

        void InsertStockRetrievalHeader(StockRetrievalHeader stockRetrievalHeader);

        void DeleteStockRetrievalHeader(int id);

        void UpdateStockRetrievalHeader(StockRetrievalHeader stockRetrievalHeader);

        void Save();

    }
}
