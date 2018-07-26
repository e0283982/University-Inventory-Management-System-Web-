using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Linq;
using System.Threading.Tasks;

namespace SA46Team1_Web_ADProj.Models
{
    public class AppUserManager : UserManager<Employee>
    {
        public AppUserManager(IUserStore<Employee> store)
            : base(store)
        {
        }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            var manager = new AppUserManager(
                new UserStore<Employee>(context.Get<SSISdbEntities>()));

            return manager;
        }
        public bool IsValid(string username, string password)
        {
            using (SSISdbEntities context = new SSISdbEntities())
            {
                return context.Users.Any(x => x.EmployeeEmail == username && x.Password == password);
            }
        }
        public string GetRole(string username)
        {
            using (SSISdbEntities context = new SSISdbEntities())
            {
                return context.Users.Where(x => x.EmployeeEmail == username).Select(x => x.Designation).FirstOrDefault();
            }
        }
        public IdentityUser Find(string username, string password)
        {
            using (SSISdbEntities context = new SSISdbEntities())
            {
                return context.Employees.Where(x => x.EmployeeEmail == username && x.Password == password).FirstOrDefault();
            }
        }
    }
}