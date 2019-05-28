using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.QuartzNET
{
    /// <summary>
    /// 注册Job
    /// </summary>
    public interface IJobRegister
    {
        /// <summary>
        /// 注册Job的运行情况
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="job"></param>
        /// <param name="attribute"></param>
        /// <param name="trigger"></param>
        /// <param name="detail"></param>
        void Register(IScheduler scheduler, IJobDetail job, ITrigger trigger, JobDescriptorAttribute attribute, IJobExcuteDetail detail);
    }
}