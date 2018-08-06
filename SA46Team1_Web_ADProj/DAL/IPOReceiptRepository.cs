using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.DAL
{
    interface IPOReceiptRepository: IDisposable
    {
        IEnumerable<POReceiptHeader> GetPOReceiptHeaders();

        POReceiptHeader GetPOReceiptHeaderById(int receiptNo);

        void InsertPOReceiptHeader(POReceiptHeader poReceiptHeader);

        void DeletePOReceiptHeader(int receiptNo);

        void UpdatePOReceiptHeader(POReceiptHeader poReceiptHeader);

        void Save();

    }
}
