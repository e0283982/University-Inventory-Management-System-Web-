using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

namespace SA46Team1_Web_ADProj.Controllers
{
    [RoutePrefix("Dept/DeptAuthorisation")]
    public class DeptAuthorisationController : Controller
    {
        [Authorize(Roles ="Department Head")]
        [Route("RoleDelegation")]
        public ActionResult RoleDelegation()
        {
            using (SSISdbEntities e = new SSISdbEntities())
            {
                ViewBag.RolesList = new SelectList(e.Roles.Select(x=>x.Designation).ToList(), "Designation");

                var tuple = new Tuple<ApprovalDelegation, Employee>(new ApprovalDelegation(), new Employee());

                return View(tuple);
            }
        }
        [Authorize(Roles = "Department Head")]
        [HttpPost]
        [Route("RoleDelegation/SubmitNewDelegation")]
        public RedirectToRouteResult SubmitNewDelegation(ApprovalDelegation item1, Employee item2)
        {
            using(SSISdbEntities e = new SSISdbEntities())
            {
                DAL.DepartmentDetailsRepositoryImpl dal2 = new DAL.DepartmentDetailsRepositoryImpl(e);
                string deptCode = Session["DepartmentCode"].ToString();
                DepartmentDetail deptDetails = e.DepartmentDetails.Where(x => x.DepartmentCode == deptCode && x.Active == 1).FirstOrDefault();

                DAL.EmployeeRepositoryImpl dal3 = new DAL.EmployeeRepositoryImpl(e);

                Employee emp = new Employee();
                emp = e.Employees.Where(x => x.EmployeeName == item2.EmployeeName).FirstOrDefault();
                if (emp != null) {
                    DAL.ApprovalDelegationRepositoryImpl dal1 = new DAL.ApprovalDelegationRepositoryImpl(e);
                    item1.EmployeeID = emp.EmployeeID;
                    item1.Active = 1;
                    item1.DateAssigned = System.DateTime.Now;

                    //todo: update other role del records belonging to this employee to inactive
                    List<ApprovalDelegation> pastApprovalsByEmp =
                        e.ApprovalDelegations.Where(x => x.EmployeeID == item1.EmployeeID).ToList();

                    foreach (ApprovalDelegation ad in pastApprovalsByEmp)
                    {
                        ad.Active = 0;
                        dal1.UpdateApprovalDelegation(ad);
                    }

                    dal1.InsertApprovalDelegation(item1); //ok

                    
                    string role = Request.Form["SelectNewEmpRole"].ToString();
                    emp.Designation = role;
                    dal3.UpdateEmployee(emp);

                    if (deptDetails != null)
                    {
                        deptDetails.RepresentativeID = item1.EmployeeID;
                        deptDetails.ApproverID = emp.EmployeeID;
                        dal2.UpdateDepartmentDetail(deptDetails); //ok
                    }
                    else
                    {
                        //no active dept details exist
                    }

                }

                e.SaveChanges();
            }

            return RedirectToAction("Authorisation", "Dept");
        }
        
    }
}
