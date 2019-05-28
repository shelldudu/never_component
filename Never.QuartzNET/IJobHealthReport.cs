using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.QuartzNET
{
    /// <summary>
    /// Job工作报告
    /// </summary>
    public interface IJobHealthReport : IJobRegister
    {
        /// <summary>
        /// 报告该Job的运行情况
        /// </summary>
        /// <param name="detail"></param>
        void Report(IJobExcuteDetail detail);

        /// <summary>
        /// ctor出错的报告
        /// </summary>
        /// <param name="detail">The detail.</param>
        void CtorErrorReport(IJobExcuteDetail detail);
    }
}