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
    interface IApprovalDelegationRepository : IDisposable
    {
        IEnumerable<ApprovalDelegation> GetApprovalDelegation();

        ApprovalDelegation GetApprovalDelegationById(int id);

        void InsertApprovalDelegation(ApprovalDelegation approvalDelegation);

        void DeleteApprovalDelegation(int id);

        void UpdateApprovalDelegation(ApprovalDelegation approvalDelegation);

        void Save();
    }
}
