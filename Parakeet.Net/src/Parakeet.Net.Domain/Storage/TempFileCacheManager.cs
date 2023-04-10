using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using Parakeet.Net.Cache;
using Volo.Abp.Caching;

namespace Parakeet.Net.Storage
{
    /// <summary>
    /// 缓存文件二进制流
    /// </summary>
    public class TempFileCacheManager : ITempFileCacheManager
    {
        private readonly IDistributedCache<byte[]> _cacheManager;
        public TempFileCacheManager(IDistributedCache<byte[]> cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 设置缓存二进制流
        /// </summary>
        /// <param name="token"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task SetFile(string token, byte[] content)
        {
            await _cacheManager.SetAsync(token, content, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = new TimeSpan(0, 8, 0, 30)
            });
        }

        /// <summary>
        /// 获取文件二进制流
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<byte[]> GetFile(string token)
        {
            return await _cacheManager.GetAsync(token);
        }
    }
}
