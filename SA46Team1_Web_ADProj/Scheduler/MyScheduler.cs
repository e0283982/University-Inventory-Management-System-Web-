using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SA46Team1_Web_ADProj.Scheduler
{
    public class MyScheduler
    {
        public void Start()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = schedFact.GetScheduler().Result;
            sched.Start();

            // Execute Codes in ChangeRoleJob
            IJobDetail job = JobBuilder.Create<ChangeRoleJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

            // Trigger Daily at 12am
            ITrigger trigger = TriggerBuilder.Create()
            //.WithCronSchedule("	0 0 0 1/1 * ? *") // Every day 12am
            .WithCronSchedule("	0 0/2 * 1/1 * ? *") // Every 2 Minute
            .Build();

            // Execute Codes in CreateDisbursementListJob
            IJobDetail job2 = JobBuilder.Create<CreateRetrievalListJob>()
                    .WithIdentity("job2", "group2")
                    .Build();

            // Trigger Every Thursday at 12am
            ITrigger trigger2 = TriggerBuilder.Create()
            .WithCronSchedule("0 0 0 ? * THU *")
            .Build();

            // Schedule the job using the job and trigger 
            sched.ScheduleJob(job, trigger);
            sched.ScheduleJob(job2, trigger2);
        }
    }
}