using Never.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.QuartzNET
{
    /// <summary>
    /// 工作者
    /// </summary>
    public struct JobWorker : Never.Security.IWorker
    {
        /// <summary>
        /// Gets the worker identifier.
        /// </summary>
        /// <value>
        /// The worker identifier.
        /// </value>
        public long WorkerId
        {
            get
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the name of the worker.
        /// </summary>
        /// <value>
        /// The name of the worker.
        /// </value>
        public string WorkerName
        {
            get
            {
                return "job";
            }
        }

        /// <summary>
        /// 创建新对象
        /// </summary>
        public static JobWorker New
        {
            get
            {
                return new JobWorker();
            }
        }
    }
}