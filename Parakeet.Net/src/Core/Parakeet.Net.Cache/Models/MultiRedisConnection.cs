using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Parakeet.Net.Cache
{
    /// <summary>
    /// 多redis连接
    /// </summary>
    public class MultiRedisConnection
    {
        /// <summary>
        /// redis连接名
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// redis连接字符串
        /// </summary>
        public string ConnectString { get; set; }

        /// <summary>
        /// 多redis连接
        /// </summary>
        public ConnectionMultiplexer Connections { get; set; }
    }
}
