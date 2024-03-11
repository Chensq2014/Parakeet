using Common.Events;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Parakeet.Net.EventHandlers
{
    /// <summary>
    /// 这两个接口的类型，将其自动注册为订阅者 ILocalEventHandler/IDistributedEventHandler
    /// </summary>
    public class RemoveCacheEventHandler : ILocalEventHandler<RemoveCacheEvent>, ITransientDependency
    {
        private readonly IDistributedCache _cacheManager;

        /// <summary>
        /// 依赖注入分布式缓存容器 IDistributedCache
        /// </summary>
        /// <param name="cacheManager"></param>
        public RemoveCacheEventHandler(IDistributedCache cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 异步方法
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public async Task HandleEventAsync(RemoveCacheEvent eventData)
        {
            var cache = _cacheManager.Get(eventData.CacheName);
            if (cache is null)
            {
                return;
            }
            await _cacheManager.RemoveAsync(eventData.Key);
            //if (eventData.Key.HasValue())
            //{
            //    await _cacheManager.RemoveAsync(eventData.Key);
            //}
            //else
            //{
            //    await _cacheManager.RemoveAsync(eventData.Key);
            //}
        }
        
        ///// <summary>
        ///// 同步方法 分布式缓存没有同步接口
        ///// </summary>
        ///// <param name="eventData"></param>
        //public void HandleEvent(RemoveCacheEvent eventData)
        //{
        //    var cache = _cacheManager.Get(eventData.CacheName);
        //    if (cache is null)
        //    {
        //        return;
        //    }
        //    if (string.IsNullOrWhiteSpace(eventData.Key))
        //    {
        //        _cacheManager.Remove(eventData.Key);
        //    }
        //}
    }
}
