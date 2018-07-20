using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IPOReceiptDetailsRepository : IDisposable
    {
        IEnumerable<POReceiptDetail> GetPOReceiptDetails();

        POReceiptDetail GetPOReceiptDetailById(string receiptNo, string itemCode);

        void InsertPOReceiptDetail(POReceiptDetail poReceiptDetail);

        void DeletePOReceiptDetail(string receiptNo, string itemCode);

        void UpdatePOReceiptDetail(POReceiptDetail poReceiptDetail);

        void Save();
    }
}
