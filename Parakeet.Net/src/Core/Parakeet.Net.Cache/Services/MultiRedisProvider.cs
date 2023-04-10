using StackExchange.Redis;
using System.Collections.Concurrent;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Cache
{
    /// <summary>
    /// 多redis连接提供器
    /// </summary>
    [ExposeServices(typeof(IMultiRedisProvider))]
    public class MultiRedisProvider : IMultiRedisProvider, ISingletonDependency
    {
        /// <summary>
        /// 多redis客户端
        /// </summary>
        protected ConcurrentDictionary<string, MultiRedisConnection> _redisClients = new ConcurrentDictionary<string, MultiRedisConnection>(Environment.ProcessorCount * 2, 919);

        /// <summary>
        /// 默认连接名
        /// </summary>
        public string DefaultConnectionName => "default";

        /// <summary>
        /// 填充真实key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> FillKeyAsync(string key)
        {
            //if (EgmpEnvironment.Instance.DeployModel == EgmpDeployModel.Single)
            //{
            //    return key;
            //}
            //else
            //{
            //    var tenant = _tenantEnvironmentInstanceProvider.GetCurrentTenant();
            //    if (tenant == null)
            //    {
            //        throw new Exception("未获取到租户环境");
            //    }

            //    if (key.StartsWith(tenant.WebSiteUrl.ToString(), StringComparison.OrdinalIgnoreCase))
            //    {
            //        return key;
            //    }

            //    if (string.IsNullOrEmpty(AppVersionNo.VersionNo))
            //    {
            //        return $"{tenant.WebSiteUrl}:{tenant.EnvType}:{key}";
            //    }
            //    else
            //    {
            //        return $"{tenant.WebSiteUrl}:{tenant.EnvType}:{AppVersionNo.VersionNo}:{key}";
            //    }
            //}
            return await Task.FromResult(key);
        }

        /// <summary>
        /// 多个连接根据租户建立客户端
        /// </summary>
        /// <param name="redisConnections"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task InitializeAsync(IList<(string ConnectionName, string ConnectionString)> redisConnections)
        {
            if (redisConnections?.Any() == false)
            {
                throw new ArgumentNullException($"连接字符串不能为空");
            }

            _redisClients = new ConcurrentDictionary<string, MultiRedisConnection>(Environment.ProcessorCount * 2, 919);
            foreach (var item in redisConnections)
            {
                _redisClients.TryAdd(item.ConnectionString, new MultiRedisConnection
                {
                    ConnectionName = item.ConnectionName,
                    ConnectString = item.ConnectionString,
                    Connections = await ConnectionMultiplexer.ConnectAsync(item.ConnectionString)
                });
            }
        }

        /// <summary>
        /// 获取当前RedisClient
        /// </summary>
        /// <returns></returns>
        public async Task<IDatabase> GetCurrentRedisClientAsync(int dbNum = 0)
        {
            //if (EgmpEnvironment.Instance.DeployModel == EgmpDeployModel.Single)
            //{
            //    return await GetDefaultRedisClientAsync();
            //}
            //else
            //{
            //    var tenant = _tenantEnvironmentInstanceProvider.GetCurrentTenant();
            //    if (tenant == null)
            //    {
            //        return await GetDefaultRedisClientAsync();
            //    }
            //    var connectname = string.Empty;
            //    if (string.IsNullOrEmpty(AppVersionNo.VersionNo))
            //    {
            //        connectname = $"{tenant.WebSiteUrl}:{tenant.EnvType}";
            //    }
            //    else
            //    {
            //        connectname = $"{tenant.WebSiteUrl}:{tenant.EnvType}:{AppVersionNo.VersionNo}";
            //    }

            //    return await GetRedisClientAsync(connectname);
            //}
            return await GetDefaultRedisClientAsync(dbNum);
        }
        
        /// <summary>
        /// 获取默认的database
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IDatabase> GetDefaultRedisClientAsync(int dbNum = 0)
        {
            if (_redisClients.TryGetValue(DefaultConnectionName, out var connect))
            {
                return await Task.FromResult(connect.Connections.GetDatabase(dbNum));
            }

            throw new ArgumentException("redis实例为空");
        }


        /// <summary>
        /// 获取redis客户端
        /// </summary>
        /// <param name="connectName"></param>
        /// <param name="dbNum"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IDatabase> GetRedisClientAsync(string connectName, int dbNum = 0)
        {
            if (_redisClients.TryGetValue(connectName, out var connect))
            {
                return connect.Connections.GetDatabase(dbNum);
            }

            return await GetDefaultRedisClientAsync();
        }
        
    }
}
