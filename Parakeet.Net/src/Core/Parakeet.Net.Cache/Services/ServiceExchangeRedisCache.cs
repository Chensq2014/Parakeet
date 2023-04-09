using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Parakeet.Net.Helper;
using Parakeet.Net.Interfaces;
using StackExchange.Redis;
using Volo.Abp.Caching.StackExchangeRedis;

namespace Parakeet.Net.Cache.Services
{
    /// <summary>
    /// ServiceExchangeRedisCache
    /// </summary>
    public class ServiceExchangeRedisCache : AbpRedisCache, IHandlerType
    {
        /// <summary>
        /// ServiceExchange
        /// </summary>
        public string HandlerType => "ServiceExchange";

        private readonly IMultiRedisProvider _multiRedisProvider;
        public ServiceExchangeRedisCache(IOptions<RedisCacheOptions> optionsAccessor, 
            IMultiRedisProvider multiRedisProvider) : base(optionsAccessor)
        {
            _multiRedisProvider = multiRedisProvider;
        }

        /// <summary>
        /// 初始化redis
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void InitMutiRedis(IServiceProvider serviceProvider)
        {
            var redisconnect = EnvironmentHelper.ExchangeRedisConn;

            //可以添加多个连接 默认情况下只读取单个
            _multiRedisProvider.InitializeAsync(new List<(string ConnectionName, string ConnectionString)>()
            {
                (_multiRedisProvider.DefaultConnectionName,redisconnect)
                //(mutiRedisProvider.DefaultConnectionName,redisconnect),
                //(mutiRedisProvider.DefaultConnectionName,redisconnect),
                //(mutiRedisProvider.DefaultConnectionName,redisconnect),
            });
        }

        /// <summary>
        /// ChangeDb
        /// </summary>
        /// <param name="dbNum"></param>
        public async Task<IDatabase?> ChangeDb(int dbNum)
        {
            //RedisDatabase.Database 当前redis数据库dbNum
            (await _multiRedisProvider.GetDefaultRedisClientAsync()).Multiplexer.GetDatabase(dbNum);
            
            return RedisDatabase.Multiplexer?.GetDatabase(dbNum);
        }
    }
}
