using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
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
    /// 工作状态更新
    /// </summary>
    public static class JobStatusManager
    {
        /// <summary>
        /// 服务
        /// </summary>
        internal static StartupService startupService;

        /// <summary>
        /// 作业调度
        /// </summary>
        public static IScheduler Scheduler { get { return startupService?.Scheduler; } }

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        public static void Shutdown()
        {
            if (startupService != null && startupService.Scheduler.IsStarted)
                startupService.Scheduler.Shutdown();
        }

        /// <summary>
        /// Startup this instance.
        /// </summary>
        public static void Startup()
        {
            if (startupService != null && !startupService.Scheduler.IsStarted)
                startupService.Scheduler.Start();
        }


        /// <summary>
        /// 获取所有Job
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<JobDetail>> GetAllJobAsync()
        {
            if (startupService == null || startupService.Scheduler == null)
                return await Task.FromResult(new List<JobDetail>());

            var jobKeyList = new List<JobKey>();
            var jobDetailList = new List<JobDetail>();

            var groupNames = await Scheduler.GetJobGroupNames();
            foreach (var groupName in groupNames.OrderBy(t => t))
            {
                jobKeyList.AddRange(await Scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)));
            }

            foreach (var jobKey in jobKeyList.OrderBy(t => t.Group))
            {
                var jobDetail = await Scheduler.GetJobDetail(jobKey);
                var triggersList = await Scheduler.GetTriggersOfJob(jobKey);
                var triggers = triggersList.AsEnumerable().FirstOrDefault();

                var interval = string.Empty;
                if (triggers is SimpleTriggerImpl)
                    interval = (triggers as SimpleTriggerImpl)?.RepeatInterval.ToString();
                else
                    interval = (triggers as CronTriggerImpl)?.CronExpressionString;

                var exceptionMessage = default(Exception);
                if (jobDetail.JobDataMap["jobMap"] is Hashtable jobMap && jobMap.ContainsKey("Exception"))
                    exceptionMessage = jobMap["Exception"] as Exception;

                jobDetailList.Add(new JobDetail()
                {
                    Group = jobKey.Group,
                    Name = jobKey.Name,
                    TypeName = jobDetail.JobType.Name,
                    Exception = exceptionMessage,
                    TriggerState = await Scheduler.GetTriggerState(triggers.Key),
                    PreviousFireTime = triggers.GetPreviousFireTimeUtc()?.LocalDateTime,
                    NextFireTime = triggers.GetNextFireTimeUtc()?.LocalDateTime,
                    BeginTime = triggers.StartTimeUtc.LocalDateTime,
                    Interval = interval,
                    EndTime = triggers.EndTimeUtc?.LocalDateTime,
                    Description = jobDetail.Description,
                });
                continue;
            }

            return jobDetailList;
        }
    }
}