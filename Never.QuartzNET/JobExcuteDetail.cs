using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.QuartzNET
{
    /// <summary>
    /// Job运行详情
    /// </summary>
    public struct JobExcuteDetail : IJobExcuteDetail, IEquatable<JobExcuteDetail>
    {
        /// <summary>
        /// 运行时间
        /// </summary>
        public DateTime RunTime { get; set; }

        /// <summary>
        /// Job类型
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// Job名字
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// Job的运行刻度
        /// </summary>
        public string JobCronSchedule { get; set; }

        /// <summary>
        /// Job描述
        /// </summary>
        public string JobDescn { get; set; }

        /// <summary>
        /// 运行异常
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 心跳，以毫秒为单位
        /// </summary>
        public int Heartbeat { get; set; }

        /// <summary>
        /// JobId
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// 机器名
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Equals(JobExcuteDetail other)
        {
            return this.JobType == other.JobType && this.JobName == other.JobName;
        }
    }
}