using Never.IoC;
using Never.Serialization;
using Never.Startups;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Never.QuartzNET
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 启动QuartzNet定时工作支持
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseQuartzNET(this ApplicationStartup startup)
        {
            return UseQuartzNET<MemoryHealthReport>(startup, true, null);
        }

        /// <summary>
        /// 启动QuartzNet定时工作支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="throwExceptionWhenSchedulerIsNull"></param>
        /// <param name="cronScheduleProvider">刻度提供器</param>
        /// <returns></returns>
        public static ApplicationStartup UseQuartzNET(this ApplicationStartup startup, bool throwExceptionWhenSchedulerIsNull, Func<ICronScheduleProvider> cronScheduleProvider)
        {
            return UseQuartzNET<MemoryHealthReport>(startup, throwExceptionWhenSchedulerIsNull, cronScheduleProvider);
        }

        /// <summary>
        /// 启动QuartzNet定时工作支持
        /// </summary>
        /// <typeparam name="TJobHealthReport">The type of the job health report.</typeparam>
        /// <param name="startup">The startup.</param>
        /// <returns></returns>
        public static ApplicationStartup UseQuartzNET<TJobHealthReport>(this ApplicationStartup startup)
            where TJobHealthReport : IJobHealthReport
        {
            return UseQuartzNET<TJobHealthReport>(startup, true, null);
        }

        /// <summary>
        /// 启动QuartzNet定时工作支持
        /// </summary>
        /// <typeparam name="TJobHealthReport">The type of the job health report.</typeparam>
        /// <param name="startup">The startup.</param>
        /// <param name="throwExceptionWhenSchedulerIsNull"></param>
        /// <param name="cronScheduleProvider">刻度提供器</param>
        /// <returns></returns>
        public static ApplicationStartup UseQuartzNET<TJobHealthReport>(this ApplicationStartup startup, bool throwExceptionWhenSchedulerIsNull, Func<ICronScheduleProvider> cronScheduleProvider)
            where TJobHealthReport : IJobHealthReport
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseQuartzNET"))
                return startup;

            startup.ServiceRegister.RegisterType<TJobHealthReport, IJobHealthReport>(string.Empty, ComponentLifeStyle.Singleton);
            var service = new StartupService(startup, cronScheduleProvider) { throwExceptionWhenSchedulerIsNull = throwExceptionWhenSchedulerIsNull };
            startup.RegisterStartService(service);
            startup.RegisterStartService(true, (x) => { service.Scheduler.Start(); });
            JobStatusManager.startupService = service;
            startup.Items["UseQuartzNET"] = "t";
            return startup;
        }
    }
}