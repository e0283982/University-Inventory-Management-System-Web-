using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IStockTakeDetailsRepository : IDisposable
    {

        IEnumerable<StockTakeDetail> GetStockTakeDetails();

        StockTakeDetail GetStockTakeDetailById(int stockTakeId, string itemCode);

        void InsertStockTakeDetail(StockTakeDetail stockTakeDetail);

        void DeleteStockTakeDetail(int stockTakeId, string itemCode);

        void UpdateStockTakeDetail(StockTakeDetail stockTakeDetail);

        void Save();
    }
}
