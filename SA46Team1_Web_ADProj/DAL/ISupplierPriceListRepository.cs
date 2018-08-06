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
    interface ISupplierPriceListRepository : IDisposable
    {
        IEnumerable<SupplierPriceList> GetSupplierPriceLists();

        SupplierPriceList GetSupplierPriceListById(string supplierCode, string itemCode);

        void InsertSupplierPriceList(SupplierPriceList supplierPriceList);

        void DeleteSupplierPriceList(string supplierCode, string itemCode);

        void UpdateSupplierPriceList(SupplierPriceList supplierPriceList);

        void Save();

    }
}
