using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IPOHeaderRepository : IDisposable
    {
        IEnumerable<POHeader> GetPOHeader();

        POHeader GetPOHeaderById(string poNumber);

        void InsertPOHeader(POHeader poHeader);

        void DeletePOHeader(string poNumber);

        void UpdatePOHeader(POHeader poHeader);

        void Save();

    }
}
