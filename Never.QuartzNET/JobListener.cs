using Never.IoC;
using Quartz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Never.QuartzNET
{
    /// <summary>
    ///
    /// </summary>
    internal class JobListener : IJobListener
    {
        #region field

        /// <summary>
        /// 健康报告
        /// </summary>
        private IJobHealthReport jobHealthReport = null;

        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationStartup startup = null;
        #endregion field

        #region ctor

        public JobListener(IApplicationStartup startup)
        {
            this.startup = startup;
        }

        #endregion ctor

        #region ijob

        public string Name
        {
            get
            {
                return "Job健康检查";
            }
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => { this.JobExecutionVetoed(context); }, cancellationToken);
        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => { this.JobToBeExecuted(context); }, cancellationToken);
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (jobException != null && context.JobDetail.JobDataMap.ContainsKey("jobMap"))
            {
                var table = context.JobDetail.JobDataMap["jobMap"] as Hashtable;
                table["Exception"] = jobException;
            }

            var scope = context.JobDetail.JobDataMap.Get("BeginLifetimeScope") as ILifetimeScope;
            if (scope != null)
                scope.Dispose();

            if (this.startup == null || this.startup.ServiceLocator == null)
                return;

            if (this.jobHealthReport == null)
                this.startup.ServiceLocator.TryResolve(ref this.jobHealthReport);

            if (this.jobHealthReport == null)
                return;

            var attributes = JobAttributeStorager.GetAttributes<JobDescriptorAttribute>(context.JobDetail.JobType);
            if (attributes == null)
            {
                return;
            }

            var jobId = context.JobDetail.JobDataMap.GetString("jobId");
            foreach (var attribute in attributes)
            {
                if (attribute == null)
                    continue;

                if (!string.IsNullOrEmpty(jobId))
                {
                    if (!jobId.Equals(attribute.Id))
                        continue;
                }

                var detail = new JobExcuteDetail()
                {
                    RunTime = DateTime.Now,
                    JobDescn = attribute.Descn,
                    JobName = attribute.Name,
                    JobId = attribute.Id,
                    JobType = context.JobDetail.JobType.Name,
                    Exception = jobException == null ? "" : jobException.GetInnerException().Message,
                    Heartbeat = attribute.Heartbeat,
                    MachineName = System.Environment.MachineName,
                    JobCronSchedule = attribute.CronSchedule
                };

                try
                {
                    this.jobHealthReport.Report(detail);
                }
                catch
                {
                }
            }
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => { this.JobWasExecuted(context, jobException); }, cancellationToken);
        }

        #endregion ijob
    }
}