using Never.IoC;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.QuartzNET
{
    internal class JobFactory : IJobFactory
    {
        /// <summary>
        /// 健康报告
        /// </summary>
        private IJobHealthReport jobHealthReport = null;

        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationStartup startup = null;

        public JobFactory(IApplicationStartup startup)
        {
            this.startup = startup;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {


            try
            {
                var scope = ContainerContext.Current.ServiceLocator.BeginLifetimeScope();
                var job = scope.ResolveOptional(bundle.JobDetail.JobType) as IJob;
                bundle.JobDetail.JobDataMap.Add("BeginLifetimeScope", scope);
                return job;
            }
            catch (Exception ex)
            {
                if (this.startup == null || this.startup.ServiceLocator == null)
                    throw new Exception(string.Format("构造对象{0}出错", bundle.JobDetail.JobType.Name), ex);

                if (this.jobHealthReport == null)
                    this.startup.ServiceLocator.TryResolve(ref this.jobHealthReport);

                if (this.jobHealthReport != null)
                {
                    var attributes = JobAttributeStorager.GetAttributes<JobDescriptorAttribute>(bundle.JobDetail.JobType);
                    if (attributes != null)
                    {
                        foreach (var attribute in attributes)
                        {
                            if (attribute == null)
                                continue;

                            var detail = new JobExcuteDetail()
                            {
                                RunTime = DateTime.Now,
                                JobDescn = attribute.Descn,
                                JobName = attribute.Name,
                                JobId = attribute.Id,
                                JobType = bundle.JobDetail.JobType.Name,
                                Exception = ex.GetFullMessage(),
                                Heartbeat = attribute.Heartbeat,
                                MachineName = System.Environment.MachineName,
                                JobCronSchedule = attribute.CronSchedule
                            };

                            try
                            {
                                this.jobHealthReport.CtorErrorReport(detail);
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                throw new Exception(string.Format("构造对象{0}出错", bundle.JobDetail.JobType.Name), ex);
            }
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}