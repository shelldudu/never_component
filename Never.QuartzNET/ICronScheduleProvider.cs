using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.QuartzNET
{
    /// <summary>
    /// 刻度提供者
    /// </summary>
    public interface ICronScheduleProvider
    {
        /// <summary>
        /// 获取提供者
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetCronSchedule(JobDescriptorAttribute attribute, Type type);
    }
}