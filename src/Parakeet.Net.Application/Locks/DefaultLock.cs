using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using Common;
using Common.Events;
using Common.Locks;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace Parakeet.Net.Locks
{
    /// <summary>
    /// 默认业务锁
    /// </summary>
    public class DefaultLock: ILock
    {
        /// <summary>
        /// 锁定标识 唯一
        /// </summary>
        private string _key;

        /// <summary>
        /// redis分布式缓存
        /// </summary>
        private readonly IDistributedCache _distributedCache;

        /// <summary>
        /// 事件
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        /// 初始化业务锁
        /// </summary>
        /// <param name="distributedCache">缓存</param>
        public DefaultLock(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            EventBus = NullDistributedEventBus.Instance;
        }

        #region 异步方法

        /// <summary>
        /// 锁定，成功锁定返回true，false代表之前已被锁定
        /// </summary>
        /// <param name="key">锁定标识</param>
        /// <param name="expiration">锁定时间间隔</param>
        /// <returns></returns>
        public async Task<bool> LockAsync(string key, TimeSpan? expiration = null)
        {
            _key = key;
            if (await _distributedCache.GetAsync(key) != null)
            {
                return false;
            }

            await _distributedCache.SetStringAsync(key,"1",new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
            return true;
        }

        /// <summary>
        /// 解除锁定
        /// </summary>
        public async Task UnLockAsync()
        {
            await _distributedCache.RemoveAsync(_key);
            //用remove事件去处理
            await EventBus.PublishAsync(new RemoveCacheEvent(CommonConsts.UserLocker,_key));
        }

        #endregion

        #region 同步方法

        /// <summary>   
        /// 锁定，成功锁定返回true，false代表之前已被锁定
        /// </summary>
        /// <param name="key">锁定标识</param>
        /// <param name="expiration">锁定时间间隔</param>
        /// <returns></returns>
        public bool Lock(string key, TimeSpan? expiration = null)
        {
            _key = key;
            if (_distributedCache.GetAsync(key) != null)
            {
                return false;
            }

            _distributedCache.SetStringAsync(key, "1",new DistributedCacheEntryOptions
             {
                 AbsoluteExpirationRelativeToNow = expiration
             });
            return true;
        }

        /// <summary>
        /// 解除锁定
        /// </summary>
        public void UnLock()
        {
            //1、通过Icache去处理
            //_distributedCache.Remove(_key);
            //也可以使用remove事件去处理
            EventBus.PublishAsync(new RemoveCacheEvent(CommonConsts.UserLocker,_key));
        }

        #endregion
    }
}
