using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.DAL
{
    interface IRolesRepository : IDisposable
    {

        IEnumerable<Role> GetRoles();

        Role GetRoleByDesignation(string designation);

        void InsertRole(Role role);

        void Save();

    }
}
