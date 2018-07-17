using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IPOReceiptDetailsRepository : IDisposable
    {
        IEnumerable<POReceiptDetail> GetPOReceiptDetails();

        POReceiptDetail GetPOReceiptDetailById(int receiptNo, string itemCode);

        void InsertPOReceiptDetail(POReceiptDetail poReceiptDetail);

        void DeletePOReceiptDetail(int receiptNo, string itemCode);

        void UpdatePOReceiptDetail(POReceiptDetail poReceiptDetail);

        void Save();
    }
}
