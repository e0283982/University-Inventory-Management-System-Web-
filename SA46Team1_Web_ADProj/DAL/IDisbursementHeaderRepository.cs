using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IDisbursementHeaderRepository : IDisposable
    {
        IEnumerable<DisbursementHeader> GetDisbursementHeaders();

        DisbursementHeader GetDisbursementHeaderById(string id);

        void InsertDisbursementHeader(DisbursementHeader disbursementHeader);

        void DeleteDisbursementHeader(string id);

        void UpdateDisbursementHeader(DisbursementHeader disbursementHeader);

        void Save();
    }
}
