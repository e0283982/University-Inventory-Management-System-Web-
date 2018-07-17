using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.DAL
{
    public class ApprovalDelegationRepositoryImpl : IApprovalDelegationRepository, IDisposable
    {
        private SSISdbEntities context;
        public ApprovalDelegationRepositoryImpl(SSISdbEntities context)
        {
            this.context = context;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<ApprovalDelegation> GetApprovalDelegation()
        {
            return context.ApprovalDelegations.ToList();
        }

        public ApprovalDelegation GetApprovalDelegationById(int id)
        {
            return context.ApprovalDelegations.Find(id);
        }

        public void InsertApprovalDelegation(ApprovalDelegation approvalDelegation)
        {
            context.ApprovalDelegations.Add(approvalDelegation);
        }

        public void DeleteApprovalDelegation(int id)
        {
            ApprovalDelegation approvalDelegation = context.ApprovalDelegations.Find(id);
            context.ApprovalDelegations.Remove(approvalDelegation);
        }

        public void UpdateApprovalDelegation(ApprovalDelegation approvalDelegation)
        {
            context.Entry(approvalDelegation).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}