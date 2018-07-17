using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IStockAdjustmentRepository : IDisposable
    {
        IEnumerable<StockAdjustmentHeader> GetStockAdjustmentHeaders();

        StockAdjustmentHeader GetStockAdjustmentHeaderById(int requestid);

        void InsertStockAdjustmentHeader(StockAdjustmentHeader stockAdjustmentHeader);

        void DeleteStockAdjustmentHeader(int requestid);

        void UpdateStockAdjustmentHeader(StockAdjustmentHeader stockAdjustmentHeader);

        void Save();

    }
}
