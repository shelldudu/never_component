using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.QuartzNET
{
    /// <summary>
    /// 内存保存
    /// </summary>
    public class MemoryHealthReport : IJobHealthReport, IDisposable
    {
        #region filed

        /// <summary>
        ///
        /// </summary>
        private readonly List<MemoryJobExcuteDetail> memory = null;

        private int capacity = 500;

        #endregion filed

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryHealthReport"/> class.
        /// </summary>
        public MemoryHealthReport() : this(500)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryHealthReport"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        protected MemoryHealthReport(int capacity)
        {
            this.capacity = capacity < 500 ? 500 : capacity;
            this.memory = new List<MemoryJobExcuteDetail>(this.capacity);
        }

        #endregion ctor

        #region IJobHealthReport

        /// <summary>
        /// 报告该Job的运行情况
        /// </summary>
        /// <param name="detail"></param>
        public virtual void Report(IJobExcuteDetail detail)
        {
            if (detail == null)
                return;

            var job = new MemoryJobExcuteDetail()
            {
                JobType = detail.JobType,
                JobName = detail.JobName,
                Exception = detail.Exception,
                Heartbeat = detail.Heartbeat,
                JobDescn = detail.JobDescn,
                RunTime = detail.RunTime,
                MachineName = detail.MachineName,
                JobId = detail.JobId,
                JobCronSchedule = detail.JobCronSchedule,
            };

            var index = memory.IndexOf(job);
            if (index >= 0)
            {
                var item = memory[index];
                item.Exception = detail.Exception;
                item.RunTime = detail.RunTime;
                return;
            }

            memory.Add(job);
        }

        /// <summary>
        /// ctor出错的报告
        /// </summary>
        /// <param name="detail">The detail.</param>
        public virtual void CtorErrorReport(IJobExcuteDetail detail)
        {
            if (detail == null)
                return;

            var job = new MemoryJobExcuteDetail()
            {
                JobType = detail.JobType,
                JobName = detail.JobName,
                Exception = detail.Exception,
                Heartbeat = detail.Heartbeat,
                JobDescn = detail.JobDescn,
                RunTime = detail.RunTime,
                MachineName = detail.MachineName,
                JobId = detail.JobId,
                JobCronSchedule = detail.JobCronSchedule,
            };

            var index = memory.IndexOf(job);
            if (index >= 0)
            {
                var item = memory[index];
                item.Exception = detail.Exception;
                return;
            }

            memory.Add(job);
        }

        /// <summary>
        /// 注册Job的运行情况
        /// </summary>
        public virtual void Register(IScheduler scheduler, IJobDetail job, ITrigger trigger, JobDescriptorAttribute attribute, IJobExcuteDetail detail)
        {
            this.Report(detail);
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public virtual PagedData<IJobExcuteDetail> Search(PagedSearch search)
        {
            var query = (from detail in this.memory
                         select (new JobExcuteDetail()
                         {
                             JobType = detail.JobType,
                             JobName = detail.JobName,
                             Exception = detail.Exception,
                             Heartbeat = detail.Heartbeat,
                             JobDescn = detail.JobDescn,
                             RunTime = detail.RunTime,
                             MachineName = detail.MachineName,
                             JobId = detail.JobId,
                             JobCronSchedule = detail.JobCronSchedule,
                         }) as IJobExcuteDetail).Skip(search.StartIndex).Take(search.PageSize).ToArray();

            return new PagedData<IJobExcuteDetail>(search.PageNow, search.PageSize, memory.Count, query);
        }

        #endregion IJobHealthReport

        #region dis

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposed"></param>
        protected virtual void Dispose(bool disposed)
        {
            if (!disposed)
                return;
        }

        #endregion dis

        #region util

        /// <summary>
        /// Job运行详情
        /// </summary>
        private class MemoryJobExcuteDetail : IJobExcuteDetail, IEquatable<MemoryJobExcuteDetail>
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
            /// Job的运行刻度
            /// </summary>
            public string JobCronSchedule { get; set; }

            /// <summary>
            /// 机器名
            /// </summary>
            public string MachineName { get; set; }

            /// <summary>
            /// Equalses the specified other.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns></returns>
            public bool Equals(MemoryJobExcuteDetail other)
            {
                return this.JobType == other.JobType && this.JobName == other.JobName;
            }
        }

        #endregion util
    }
}