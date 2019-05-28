using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.QuartzNET
{
    /// <summary>
    /// Job运行详情
    /// </summary>
    public interface IJobExcuteDetail
    {
        /// <summary>
        /// 运行时间
        /// </summary>
        DateTime RunTime { get; }

        /// <summary>
        /// Job类型
        /// </summary>
        string JobType { get; }

        /// <summary>
        /// Job名字
        /// </summary>
        string JobName { get; }

        /// <summary>
        /// Job的运行刻度
        /// </summary>
        string JobCronSchedule { get; }

        /// <summary>
        /// Job描述
        /// </summary>
        string JobDescn { get; }

        /// <summary>
        /// 运行异常
        /// </summary>
        string Exception { get; }

        /// <summary>
        /// 心跳，以毫秒为单位
        /// </summary>
        int Heartbeat { get; set; }

        /// <summary>
        /// JobId
        /// </summary>
        string JobId { get; set; }

        /// <summary>
        /// 机器名
        /// </summary>
        string MachineName { get; }
    }
}