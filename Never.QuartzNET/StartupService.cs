using Never.Exceptions;
using Never.Startups;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.QuartzNET
{
    /// <summary>
    /// 启动支持主题
    /// </summary>
    internal class StartupService : Startups.IStartupService
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly ISchedulerFactory schedFactory = null;

        /// <summary>
        ///
        /// </summary>
        private readonly static Type jobType = typeof(IJob);

        /// <summary>
        ///
        /// </summary>
        private readonly Type attributeType = typeof(JobDescriptorAttribute);

        /// <summary>
        /// 是否跑出异常
        /// </summary>
        internal bool throwExceptionWhenSchedulerIsNull;

        /// <summary>
        /// 任务
        /// </summary>
        public IScheduler Scheduler = null;

        /// <summary>
        ///
        /// </summary>
        private readonly Func<ICronScheduleProvider> cronScheduleProvider = null;

        #endregion field

        #region ctor

        public StartupService(ApplicationStartup startup, Func<ICronScheduleProvider> cronScheduleProvider)
        {
            this.cronScheduleProvider = cronScheduleProvider;
            this.schedFactory = new StdSchedulerFactory();
        }

        #endregion ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        void IStartupService.OnStarting(Startups.StartupContext context)
        {
            this.RegisterAllJob(context).GetAwaiter().GetResult();
        }


        private async Task RegisterAllJob(Startups.StartupContext context)
        {
            Scheduler = await this.schedFactory.GetScheduler();
            Scheduler.ListenerManager.AddJobListener(new JobListener(context.ApplicationStartup));
            Scheduler.JobFactory = new JobFactory(context.ApplicationStartup);

            foreach (var assembly in context.FilteringAssemblyProvider.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type == null || type.IsAbstract || !type.IsClass || type.IsInterface || type.IsGenericTypeDefinition)
                        continue;

                    if (!jobType.IsAssignableFrom(type))
                        continue;

                    var attributes = type.GetCustomAttributes(this.attributeType, false) as IEnumerable<JobDescriptorAttribute>;
                    if (attributes == null || !attributes.Any())
                        throw new Exception(string.Format("对象{0}没有JonDescriptorAttribute特性，不可绑定Job", type.Name));

                    if (attributes.Count() > 1)
                    {
                        if (attributes.Any(o => o.Id.IsNullOrEmpty()))
                            throw new Exception(string.Format("对象{0}的JonDescriptorAttribute特性存在多个，并且\"{1}\"没有填写Id，不可绑定Job", type.Name, attributes.FirstOrDefault().Descn));

                        if (attributes.Select(o => o.Id).Distinct().Count() != attributes.Count())
                            throw new Exception(string.Format("对象{0}的JonDescriptorAttribute特性总数{1}个，里面Id总数{2}个，两者不等，请填写每个Job的Id", type.Name, attributes.Count(), attributes.Select(o => o.Id).Distinct().Count()));
                    }

                    /*注册IoC*/
                    context.ApplicationStartup.ServiceRegister.RegisterType(type, type, string.Empty, IoC.ComponentLifeStyle.Scoped);
                    JobAttributeStorager.Add(type, attributes);
                    var provider = this.cronScheduleProvider == null ? null : this.cronScheduleProvider();

                    foreach (var attribute in attributes)
                    {
                        var croschedule = this.GeCronSchedule(provider, attribute, type);
                        if (croschedule.IsNullOrEmpty())
                            throw new Exception(string.Format("特性{0}的运行刻度为空", attribute.Name));

                        var name = Guid.NewGuid().ToString();
                        var group = Guid.NewGuid().ToString();
                        var triggerName = Guid.NewGuid().ToString();
                        IJobDetail job = JobBuilder.Create(type).UsingJobData("jobId", attribute.Id).WithIdentity(name, group).Build();
                        ITrigger trigger = TriggerBuilder.Create().WithIdentity(triggerName, group).WithCronSchedule(croschedule).StartNow().Build();

                        await Scheduler.ScheduleJob(job, trigger);
                    }
                }
            }
        }

        void Processing(IApplicationStartup application, Type type, IScheduler scheduler)
        {
            if (type == null || type.IsAbstract || !type.IsClass || type.IsInterface || type.IsGenericTypeDefinition)
                return;

            if (!jobType.IsAssignableFrom(type))
                return;

            var attributes = type.GetCustomAttributes(this.attributeType, false) as IEnumerable<JobDescriptorAttribute>;
            if (attributes == null || !attributes.Any())
                throw new Exception(string.Format("对象{0}没有JonDescriptorAttribute特性，不可绑定Job", type.Name));

            if (attributes.Count() > 1)
            {
                if (attributes.Any(o => o.Id.IsNullOrEmpty()))
                    throw new Exception(string.Format("对象{0}的JonDescriptorAttribute特性存在多个，并且\"{1}\"没有填写Id，不可绑定Job", type.Name, attributes.FirstOrDefault().Descn));

                if (attributes.Select(o => o.Id).Distinct().Count() != attributes.Count())
                    throw new Exception(string.Format("对象{0}的JonDescriptorAttribute特性总数{1}个，里面Id总数{2}个，两者不等，请填写每个Job的Id", type.Name, attributes.Count(), attributes.Select(o => o.Id).Distinct().Count()));
            }

            /*注册IoC*/
            application.ServiceRegister.RegisterType(type, type, string.Empty, IoC.ComponentLifeStyle.Scoped);
            JobAttributeStorager.Add(type, attributes);
            var provider = this.cronScheduleProvider == null ? null : this.cronScheduleProvider();
            foreach (var attribute in attributes)
            {
                var croschedule = this.GeCronSchedule(provider, attribute, type);
                if (croschedule.IsNullOrEmpty())
                    throw new Exception(string.Format("特性{0}的运行刻度为空", attribute.Name));

                var name = attribute.Name;
                var group = type.Assembly.GetName().Name;
                var triggerName = Guid.NewGuid().ToString();
                IJobDetail job = JobBuilder.Create(type).SetJobData(new JobDataMap()
                {
                    new KeyValuePair<string, object>("jobId",attribute.Id),
                    new KeyValuePair<string, object>("jobMap",new Hashtable())
                }).WithIdentity(name, group).Build();

                ITrigger trigger = TriggerBuilder.Create().WithIdentity(triggerName, group).WithCronSchedule(croschedule).StartNow().Build();

                scheduler.ScheduleJob(job, trigger).Wait();
            }
        }

        private string GeCronSchedule(ICronScheduleProvider provider, JobDescriptorAttribute attribute, Type type)
        {
            if (provider == null)
                return attribute.CronSchedule;

            var cron = provider.GetCronSchedule(attribute, type);
            if (cron.IsNotNullOrEmpty())
            {
                attribute.CronSchedule = cron;
                return cron;
            }

            return attribute.CronSchedule;
        }

        /// <summary>
        ///
        /// </summary>
        int IStartupService.Order
        {
            get { return 102; }
        }
    }
}