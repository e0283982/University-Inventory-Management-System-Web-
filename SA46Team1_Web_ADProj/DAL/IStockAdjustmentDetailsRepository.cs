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
    interface IStockAdjustmentDetailsRepository : IDisposable
    {
        IEnumerable<StockAdjustmentDetail> GetStockAdjustmentDetails();

        StockAdjustmentDetail GetStockAdjustmentDetailById(string requestid, string itemcode);

        void InsertStockAdjustmentDetail(StockAdjustmentDetail stockAdjustmentDetail);

        void DeleteStockAdjustmentDetail(string requestid, string itemcode);

        void UpdateStockAdjustmentDetail(StockAdjustmentDetail stockAdjustmentDetail);

        void Save();
    }
}
