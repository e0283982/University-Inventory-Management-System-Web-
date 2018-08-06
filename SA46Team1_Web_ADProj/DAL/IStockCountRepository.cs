using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.DAL
{
    interface IStockCountRepository : IDisposable
    {
        IEnumerable<StockCount> GetStockCounts();

        StockCount GetStockCountByYearMonthItem(int year, int month, string itemcode);

        void InsertStockCount(StockCount stockCount);

        void UpdateStockCount(StockCount stockCount);
    }
}
