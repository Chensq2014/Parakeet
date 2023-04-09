using StackExchange.Redis;

namespace Parakeet.Net.Cache
{
    /// <summary>
    /// 多redis连接提供器
    /// </summary>
    public interface IMultiRedisProvider
    {
        /// <summary>
        /// 默认连接名
        /// </summary>
        public string DefaultConnectionName { get; }

        /// <summary>
        /// 填充真实key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<string> FillKeyAsync(string key);

        /// <summary>
        /// 多个连接根据租户建立客户端
        /// </summary>
        /// <param name="redisConnections"></param>
        /// <returns></returns>
        public Task InitializeAsync(IList<(string ConnectionName, string ConnectionString)> redisConnections);

        /// <summary>
        /// 获取当前RedisClient
        /// </summary>
        /// <returns></returns>
        public Task<IDatabase> GetCurrentRedisClientAsync(int dbNum = 0);

        /// <summary>
        /// 获取默认的database
        /// </summary>
        /// <returns></returns>
        public Task<IDatabase> GetDefaultRedisClientAsync(int dbNum = 0);

        /// <summary>
        /// 获取redis客户端
        /// </summary>
        /// <param name="connectName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Task<IDatabase> GetRedisClientAsync(string connectName, int dbNum = 0);
    }
}
