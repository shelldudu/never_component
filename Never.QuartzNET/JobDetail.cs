using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.QuartzNET
{
    /// <summary>
    /// job详情
    /// </summary>
    public struct JobDetail
    {
        /// <summary>
        /// 作业分组
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 作业名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 下次执行时间
        /// </summary>
        public DateTime? NextFireTime { get; set; }

        /// <summary>
        /// 上次执行时间
        /// </summary>
        public DateTime? PreviousFireTime { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 上次执行的异常信息
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 作业状态
        /// </summary>
        public TriggerState TriggerState { get; set; }

        /// <summary>
        /// 作业描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 时间间隔
        /// </summary>
        public string Interval { get; set; }
    }
}
