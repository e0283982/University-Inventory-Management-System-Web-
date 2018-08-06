using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SA46Team1_Web_ADProj.Models;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.DAL
{
    public class RolesRepositoryImpl :  IRolesRepository, IDisposable
    {
        private SSISdbEntities context;
        public RolesRepositoryImpl(SSISdbEntities context)
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

        public IEnumerable<Role> GetRoles()
        {
            return context.Roles.ToList();
        }

        public Role GetRoleByDesignation(string designation)
        {
            return context.Roles.Find(designation);
        }

        public void InsertRole(Role role)
        {
            context.Roles.Add(role);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}