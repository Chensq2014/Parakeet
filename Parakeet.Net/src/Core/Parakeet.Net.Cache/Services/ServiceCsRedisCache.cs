using CSRedis;
using Parakeet.Net.Interfaces;

namespace Parakeet.Net.Cache.Services
{
    /// <summary>
    /// CsRedisCache
    /// </summary>
    public class ServiceCsRedisCache : CSRedisClient, IHandlerType
    {
        /// <summary>
        /// CsRedis
        /// </summary>
        public string HandlerType => "CsRedis";


        public ServiceCsRedisCache(string connectionString) : base(connectionString)
        {
        }

        public ServiceCsRedisCache(string connectionString, string[] sentinels, bool readOnly = false) : base(connectionString, sentinels, readOnly)
        {
        }

        public ServiceCsRedisCache(string connectionString, string[] sentinels, bool readOnly, SentinelMasterConverter convert) : base(connectionString, sentinels, readOnly, convert)
        {
        }

        /// <summary>
        /// 创建redis分区访问类，通过 KeyRule 对 key 进行分区，连接对应的 connectionString
        /// </summary>
        /// <param name="nodeRule">按key分区规则，返回值格式：127.0.0.1:6379/13，默认方案(null)：取key哈希与节点数取模</param>
        /// <param name="connectionStrings">127.0.0.1[:6379],password=123456,defaultDatabase=13,poolsize=50,ssl=false,writeBuffer=10240,prefix=key前辍</param>

        public ServiceCsRedisCache(Func<string, string> nodeRule, params string[] connectionStrings) : base(nodeRule, connectionStrings)
        {

        }

        protected ServiceCsRedisCache(Func<string, string> nodeRule, string[] sentinels, bool readOnly, SentinelMasterConverter convert = null, params string[] connectionStrings) : base(nodeRule, sentinels, readOnly, convert, connectionStrings)
        {
        }

        ///// <summary>
        ///// ChangeDb
        ///// </summary>
        ///// <param name="dbNum"></param>
        //public void ChangeDb(int dbNum)
        //{
        //    RedisHelper.Nodes.
        //}
    }
}
