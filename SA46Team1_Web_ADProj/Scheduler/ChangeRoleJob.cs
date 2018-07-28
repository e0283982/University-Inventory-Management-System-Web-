using Quartz;
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