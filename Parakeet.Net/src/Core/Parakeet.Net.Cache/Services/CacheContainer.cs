using System.Collections.Concurrent;

namespace Parakeet.Net.Cache
{
    /// <summary>
    /// 缓存容器
    /// </summary>
    /// <typeparam name="TMultilevelCache">多级缓存类类型</typeparam>
    /// <typeparam name="TData">缓存数据类型</typeparam>
    public class CacheContainer<TMultilevelCache, TData> : ICacheContainer<TMultilevelCache, TData> where TMultilevelCache : MultilevelCache<TData>
    {
        /// <summary>
        /// 线程安全字典  string--TMultilevelCache 
        /// </summary>
        protected readonly ConcurrentDictionary<string, TMultilevelCache> MultilevelCaches = new ConcurrentDictionary<string, TMultilevelCache>();

        /// <summary>
        /// 本地缓存最大缓存数
        /// </summary>
        public const int MaximumLocalCache = 1024;

        #region Implementation of ICacheContainer<in TMultilevelCache,out TData>

        /// <summary>
        /// 添加多级缓存实例
        /// </summary>
        /// <param name="multilevelCache">多级缓存实例</param>
        public void AddCache(TMultilevelCache multilevelCache)
        {
            //超过最大缓存数后清理掉本地访问不频繁的本地缓存
            if (MultilevelCaches.Count > MaximumLocalCache)
            {
                MultilevelCaches.RemoveAll(x => x.Value.IsExpire);
            }

            MultilevelCaches.TryAdd(multilevelCache.CacheKey, multilevelCache);
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="multilevelCacheFunc">当缓存key不存在时，尝试添加添加缓存实例</param>
        /// <returns></returns>
        public async Task<TData> GetCacheValue(string cacheKey, Func<TMultilevelCache> multilevelCacheFunc = null)
        {
            if (MultilevelCaches.TryGetValue(cacheKey, out var cache))
            {
                var data = await cache.GetCacheData();
                if (data == null)
                {
                    MultilevelCaches.TryRemove(cacheKey, out _);
                }
                else
                {
                    return data;
                }
            }

            if (multilevelCacheFunc == null)
            {
                return default(TData);
            }

            var multilevelCache = multilevelCacheFunc.Invoke();
            AddCache(multilevelCache);
            return await multilevelCache.GetCacheData();
        }


        /// <summary>
        /// 根据缓存key移除缓存对象
        /// </summary>
        /// <param name="cacheKey">缓存key</param>
        public void RemoveCache(string cacheKey)
        {
            MultilevelCaches.TryRemove(cacheKey, out _);
        }

        #endregion Implementation of ICacheContainer<in TMultilevelCache,out TData>
    }
}