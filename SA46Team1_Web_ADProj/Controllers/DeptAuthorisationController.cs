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
                List<string> roleList = new List<string>() { "Approver", "Representative"};
                ViewBag.RolesList = new SelectList(roleList, "Designation");

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
                string role = Request.Form["SelectNewEmpRole"].ToString();
                Employee HODemp = e.Employees.Where(x => x.DepartmentCode == deptCode && x.Designation == "Department Head" && x.Active==1).FirstOrDefault();

                if (emp != null) {
                    
                    if (role == "Approver")
                    {
                        //1. If delegated approver,
                        //Insert New Approval Delegation (and inactivate others of same department)
                        DAL.ApprovalDelegationRepositoryImpl dal1 = new DAL.ApprovalDelegationRepositoryImpl(e);
                        item1.EmployeeID = emp.EmployeeID;
                        item1.Active = 1;  
                        item1.DateAssigned = System.DateTime.Now;
                        item1.Id = e.ApprovalDelegations.ToList().Count() + 1;
                        

                        //(1.1 loop Approval Delegation - same dept)
                        List<string> employeeList = 
                            e.Employees.Where(x => x.DepartmentCode == deptCode && x.Active == 1).Select(x=>x.EmployeeID).ToList();
                        
                        List<ApprovalDelegation> pastActiveApprovalsOfDept =
                            e.ApprovalDelegations.Where(x => employeeList.Contains(x.EmployeeID)).ToList();

                        if (pastActiveApprovalsOfDept != null)
                        {
                            foreach (ApprovalDelegation ad in pastActiveApprovalsOfDept)
                            {
                                ad.Active = 0;
                                dal1.UpdateApprovalDelegation(ad);
                            }
                        }
                        
                        if (emp.Designation == "Employee Representative")
                        {
                            deptDetails.RepresentativeID = HODemp.EmployeeID; 
                            //assuming only HOD (not just any approver) can delegate roles
                        }

                        if (emp.Designation != "Department Head")
                        {
                            //(1.2 loop employee - same dept)
                            Employee prevApprover = e.Employees.Where(x => x.DepartmentCode == deptCode && x.Approver == 1).FirstOrDefault();
                            if (prevApprover != null)
                            {
                                prevApprover.Approver = 0;
                            }

                            emp.Designation = "Employee";
                            dal1.InsertApprovalDelegation(item1);
                        }

                        
                        //2. Update Employee role and approver flag
                        //3. Update department details (rep and approver id)
                        emp.Approver = 1;
                        deptDetails.ApproverID = emp.EmployeeID;
                    }

                    if (role == "Representative") {
                        Employee prevRep = e.Employees.Where(x => x.DepartmentCode == deptCode 
                            && x.EmployeeID==deptDetails.RepresentativeID).FirstOrDefault();

                        if (prevRep != null && prevRep.Designation!="Department Head")
                        {
                            prevRep.Designation = "Employee";
                        }

                        //inactivate any prev approver del
                        ApprovalDelegation ad = e.ApprovalDelegations.Where(x => x.EmployeeID == emp.EmployeeID && x.Active == 1).FirstOrDefault();
                        if (ad != null) {
                            ad.Active = 0;
                        }

                        //check if dept has no approver, default to dept's HOD
                        if (emp.Approver == 1)
                        {
                            deptDetails.ApproverID = HODemp.EmployeeID;
                        }

                        if (emp.Designation != "Department Head")
                        {
                            emp.Designation = "Employee Representative";
                            emp.Approver = 0;
                        }

                        deptDetails.RepresentativeID = emp.EmployeeID;
                       

                        //4. Update rep in Disbursement Header (with status 'Open')
                        List<DisbursementHeader> openDisbursements = e.DisbursementHeaders.Where(x => x.DepartmentCode == deptCode && x.Status == "Open").ToList();
                        foreach (DisbursementHeader d in openDisbursements)
                        {
                            d.RepresentativeID = emp.EmployeeID;
                        }
                    }

                    dal3.UpdateEmployee(emp); 
                    dal2.UpdateDepartmentDetail(deptDetails);
                   
                }
                
                e.SaveChanges();
            }

            return RedirectToAction("Authorisation", "Dept");
        }
        
    }
}
