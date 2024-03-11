using Common.Locks;
using Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace Parakeet.Net.Locks
{
    public static class LockExtensions
    {
        /// <summary>
        /// 注册业务锁
        /// </summary>
        /// <param name="service">服务集合</param>
        public static void AddLock(this IServiceCollection service)
        {
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、添加单例默认线程锁DefaultLock ....");
            service.TryAddScoped<ILock, DefaultLock>();
        }
    }
}
