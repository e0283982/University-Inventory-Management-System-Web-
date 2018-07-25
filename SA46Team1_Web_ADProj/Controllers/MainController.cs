using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using SA46Team1_Web_ADProj.DAL;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class MainController : Controller
    {
        IEmployeeRepository emplRepo;
        public MainController()
        {
            emplRepo = new EmployeeRepositoryImpl(new SSISdbEntities());
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserModel user)
        {
            if (new AppUserManager(new UserStore<Employee>(new SSISdbEntities())).IsValid(user.Username, user.Password))
            {
                //Auth success
                Employee employee = emplRepo.FindEmployeeEmailId(user.Username);
                var ident = new ClaimsIdentity(
                new[] {                   
                    new Claim(ClaimTypes.NameIdentifier, employee.EmployeeEmail),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                    new Claim(ClaimTypes.Name,employee.EmployeeName),
                      new Claim(ClaimTypes.Role, employee.Designation),
                      },
                DefaultAuthenticationTypes.ApplicationCookie);
                HttpContext.GetOwinContext().Authentication.SignIn(
                new AuthenticationProperties { IsPersistent = false }, ident);

                switch (employee.Designation)
                {               
                    case "Department Head":
                        return RedirectToAction("Home", "Dept", new { area = "" });
                    case "Employee":
                        return RedirectToAction("Home", "Dept", new { area = "" });
                    case "Employee Representative":
                        return RedirectToAction("Home", "Dept", new { area = "" });
                    case "Store Clerk":
                        return RedirectToAction("Home", "Store", new { area = "" });                     
                    case "Store Manager":
                        return RedirectToAction("Home", "Store", new { area = "" });
                    case "Store Supervisor":
                        return RedirectToAction("Home", "Store", new { area = "" });     
                    default:
                        return View("Login");                        
                }
            }
            else
            {
                //auth failed
                return View("Login");
            }               
        }
        [HttpPost]
        public ActionResult Logout()
        {         
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return View("Login");
        }
    }
}
