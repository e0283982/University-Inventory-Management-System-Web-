using Quartz;
using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SA46Team1_Web_ADProj.Scheduler
{
    public class ChangeRoleJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                using (SSISdbEntities m = new SSISdbEntities())
                {
                    // Get only those that are active
                    List<ApprovalDelegation> adList = m.ApprovalDelegations.Where(x => x.Active == 1).ToList();

                    foreach (ApprovalDelegation a in adList)
                    {
                        DateTime dateTo = a.ToDate.Date;
                        DateTime today = DateTime.Now.Date;

                        // Only execute if Date is less than or equal to Today
                        if (dateTo <= today)
                        {
                            // Remove approving rights
                            ApprovalDelegation ad = m.ApprovalDelegations.Where(x => x.Id == a.Id).FirstOrDefault();
                            ad.Active = 0;
                            Employee e = m.Employees.Where(x => x.EmployeeID == ad.EmployeeID).FirstOrDefault();
                            e.Approver = 0;

                            // Transfer approving rights back to Mgr
                            Employee mgr = m.Employees.Where(x => x.EmployeeID == e.ReportsTo).FirstOrDefault();
                            mgr.Approver = 1;
                            m.SaveChanges();
                        }
                    }
                }
            }
            catch (NotImplementedException e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        // To Test Cron works, put this in MyScheduler class & watch the print out in Output (Debug):
        //ITrigger trigger = TriggerBuilder.Create()
        //.WithCronSchedule("* * * * * ?")
        //.Build();
        //Debug.WriteLine("test2 success");
    }
}