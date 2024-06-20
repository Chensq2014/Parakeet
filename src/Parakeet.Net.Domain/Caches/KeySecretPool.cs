using Common.Cache;
using Common.Entities;
using Common.Storage;
using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;

namespace Parakeet.Net.Caches
{
    /// <summary>
    /// 设备区域转发密钥高速缓存池，调用前请先确保单例注入DeviceKeySecretPool 
    /// </summary>
    public class KeySecretPool
    {
        protected readonly ICacheContainer<MultilevelCache<DeviceKeySecret>, DeviceKeySecret> CacheContainer;
        protected readonly IEasyCachingProvider CachingProvider;
        //protected readonly IFreeSql FreeSql;//使用freesql进行了数据查询 需要单例注入
        private readonly IRepository<DeviceKeySecret, Guid> _deviceKeySecretRepository;//使用efcore 进行查询 注入IRepository仓储

        public KeySecretPool(ICacheContainer<MultilevelCache<DeviceKeySecret>, DeviceKeySecret> cacheContainer,
            IEasyCachingProvider cachingProvider,
            //IFreeSql freeSql,
            IRepository<DeviceKeySecret, Guid> deviceKeySecretRepository)
        {
            CacheContainer = cacheContainer;
            CachingProvider = cachingProvider;
            _deviceKeySecretRepository = deviceKeySecretRepository;
            //FreeSql = freeSql;
        }


        /// <summary>
        /// 根据密钥Id获取DeviceKeySecret 设备区域转发密钥
        /// </summary>
        /// <param name="id">设备区域转发密钥Id</param>
        /// <returns></returns>
        public DeviceKeySecret this[Guid id]
        {
            get
            {
                var cacheKey = string.Format(CacheKeys.DeviceKeySecretId, id);
                return AsyncHelper.RunSync(() => CacheContainer.GetCacheValue(cacheKey, () => new MultilevelCache<DeviceKeySecret>(CachingProvider, cacheKey, async () =>
                {
                    #region FreeSql

                    //var entity = await FreeSql.Select<DeviceKeySecret>()
                    //    .Where(x => !x.IsDeleted&& x.Id == id)
                    //    .Include(x => x.Device)
                    //    .FirstAsync();

                    //foreach (var keySecret in entity?.Device.KeySecrets)
                    //{
                    //    keySecret.Device = null;//待测试
                    //}
                    #endregion

                    #region efcore

                    var entity = await (await _deviceKeySecretRepository.GetQueryableAsync())
                        .Include(x => x.Device)
                        .Where(x => x.Id == id)
                        .FirstAsync();

                    #endregion

                    return entity;
                })));
            }
        }

        /// <summary>
        /// 根据设备区域编码获取DeviceKeySecret 设备区域转发密钥
        /// </summary>
        /// <param name="area">区域编码</param>
        /// <param name="fakeNo">设备编码</param>
        /// <returns></returns>
        public DeviceKeySecret this[string area, string fakeNo]
        {
            get
            {
                var cacheKey = string.Format(CacheKeys.DeviceKeySecretAreaFakeNo, area, fakeNo);
                return AsyncHelper.RunSync(() => CacheContainer.GetCacheValue(cacheKey, () => new MultilevelCache<DeviceKeySecret>(CachingProvider, cacheKey, async () =>
                {
                    #region FreeSql

                    //return await FreeSql.Select<DeviceKeySecret>()
                    //    .Where(x => !x.IsDeleted)
                    //    .Where(x => x.Area == areaCode && x.Device.FakeNo == fakeNo)
                    //    .FirstAsync();
                    #endregion

                    #region efcore

                    var entity = await (await _deviceKeySecretRepository.GetQueryableAsync())
                        .Include(x => x.Device)
                        .Where(x => x.Area == area && x.Device.FakeNo == fakeNo)
                        .FirstAsync();

                    #endregion

                    return entity;
                })));
            }
        }

        /// <summary>
        /// 根据区域码+设备Id 获取DeviceKeySecret 设备区域转发密钥
        /// </summary>
        /// <param name="area">区域编码</param>
        /// <param name="deviceId">设备Id</param>
        /// <returns></returns>
        public DeviceKeySecret this[string area, Guid deviceId]
        {
            get
            {
                var cacheKey = string.Format(CacheKeys.DeviceKeySecretAreaDeviceId, area, deviceId);
                return AsyncHelper.RunSync(() => CacheContainer.GetCacheValue(cacheKey, () => new MultilevelCache<DeviceKeySecret>(CachingProvider, cacheKey, async () =>
                {
                    #region FreeSql

                    //return await FreeSql.Select<DeviceKeySecret>()
                    //    .Where(x => !x.IsDeleted)
                    //    .Where(x => x.Area == areaCode && x.DeviceId == deviceId)
                    //    .FirstAsync();
                    #endregion

                    #region efcore

                    var entity = await  (await _deviceKeySecretRepository.GetQueryableAsync())
                        .Include(x => x.Device)
                        .Where(x => x.Area == area && x.DeviceId == deviceId)
                        .FirstAsync();

                    #endregion

                    return entity;
                })));
            }
        }
    }
}
