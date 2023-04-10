namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// ServiceStack.Redis 缓存配置
    /// </summary>
    public class RedisConnOptions
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; set; } = $"127.0.0.1";
        
        /// <summary>
        /// 端口号 默认6379
        /// </summary>
        public int Port { get; set; } = 6379;

        /// <summary>
        /// Database 0-15 默认0
        /// </summary>
        public int DatabaseId { get; set; } = 0;

    }
}
