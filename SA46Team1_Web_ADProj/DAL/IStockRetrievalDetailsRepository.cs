using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IStockRetrievalDetailsRepository : IDisposable
    {
        IEnumerable<StockRetrievalDetail> GetStockRetrievalDetails();

        StockRetrievalDetail GetStockRetrievalDetailById(int id, int binid);

        void InsertStockRetrievalDetail(StockRetrievalDetail stockRetrievalDetail);

        void DeleteStockRetrievalDetail(int id, int binid);

        void UpdateStockRetrievalDetail(StockRetrievalDetail stockRetrievalDetail);

        void Save();
    }
}
