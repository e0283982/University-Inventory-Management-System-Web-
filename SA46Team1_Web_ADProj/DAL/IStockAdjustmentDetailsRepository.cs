using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IStockAdjustmentDetailsRepository : IDisposable
    {
        IEnumerable<StockAdjustmentDetail> GetStockAdjustmentDetails();

        StockAdjustmentDetail GetStockAdjustmentDetailById(int requestid, string itemcode);

        void InsertStockAdjustmentDetail(StockAdjustmentDetail stockAdjustmentDetail);

        void DeleteStockAdjustmentDetail(int requestid, string itemcode);

        void UpdateStockAdjustmentDetail(StockAdjustmentDetail stockAdjustmentDetail);

        void Save();
    }
}
