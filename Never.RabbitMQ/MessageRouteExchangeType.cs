using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.RabbitMQ
{
    /// <summary>
    /// 交换机类型
    /// </summary>
    public enum MessageRouteExchangeType
    {
        /// <summary>
        /// fanout
        /// </summary>
        Fanout = 0,

        /// <summary>
        /// direct
        /// </summary>
        Direct = 1,

        /// <summary>
        /// topic
        /// </summary>
        Topic = 2,
    }
}