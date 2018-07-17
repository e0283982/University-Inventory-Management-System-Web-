using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IStockTakeHeaderRepository : IDisposable
    {
        IEnumerable<StockTakeHeader> GetStockTakeHeaders();

        StockTakeHeader GetStockTakeHeaderById(int stockTakeId);

        void InsertStockTakeHeader(StockTakeHeader stockTakeHeader);

        void DeleteStockTakeHeader(int stockTakeId);

        void UpdateStockTakeHeader(StockTakeHeader stockTakeHeader);
 
        void Save();
    }
}
