using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

//-----------------------------------------------------------------
//   Authors: Chirag Shetty
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj.DAL
{    
    public class AuthRepository : IDisposable
        {
            private SSISdbEntities _ctx;

            private AppUserManager _userManager;

            public AuthRepository()
            {
                _ctx = new SSISdbEntities();
                _userManager = new AppUserManager(new UserStore<Employee>(_ctx));
            }

            public IdentityUser FindUser(string username, string password)
            {
                IdentityUser user =  _userManager.Find(username, password);
                return user;
            }

            public void Dispose()
            {
                _ctx.Dispose();
                _userManager.Dispose();
            }
        }
    }
