
namespace Parakeet.Net.Cache
{
    /// <summary>
    /// 缓存容器
    /// </summary>
    /// <typeparam name="TMultilevelCache">多级缓存类类型</typeparam>
    /// <typeparam name="TData">缓存数据类型</typeparam>
    public interface ICacheContainer<in TMultilevelCache, TData> where TMultilevelCache : MultilevelCache<TData>
    {
        /// <summary>
        /// 添加多级缓存实例
        /// </summary>
        /// <param name="multilevelCache"></param>
        void AddCache(TMultilevelCache multilevelCache);

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="multilevelCache">当缓存key不存在时，尝试添加添加缓存实例</param>
        /// <returns></returns>
        Task<TData> GetCacheValue(string cacheKey, Func<TMultilevelCache> multilevelCache = null);

        /// <summary>
        /// 根据key移除缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        void RemoveCache(string cacheKey);
    }
}
