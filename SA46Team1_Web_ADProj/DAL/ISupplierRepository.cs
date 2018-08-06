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
    interface ISupplierRepository : IDisposable
    {
        IEnumerable<Supplier> GetSuppliers();

        Supplier GetSupplierById(string supplierId);

        void InsertSupplier(Supplier supplier);

        void DeleteSupplier(string supplierId);

        void UpdateSupplier(Supplier supplier);

        void Save();
    }
}
