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
    /// 设备高速缓存池，调用前请先确保单例注入DevicePool 
    /// </summary>
    public class DevicePool
    {
        protected readonly ICacheContainer<MultilevelCache<Device>, Device> CacheContainer;
        protected readonly IEasyCachingProvider CachingProvider;
        //protected readonly IFreeSql FreeSql;//使用freesql进行了数据查询 需要单例注入
        private readonly IRepository<Device, Guid> _deviceRepository;//使用efcore 进行查询

        public DevicePool(ICacheContainer<MultilevelCache<Device>, Device> cacheContainer, 
            //IFreeSql freeSql,
            IRepository<Device, Guid> deviceRepository,
            IEasyCachingProvider cachingProvider)
        {
            CacheContainer = cacheContainer;
            CachingProvider = cachingProvider;
            //FreeSql = freeSql;
            _deviceRepository = deviceRepository;
        }


        /// <summary>
        /// 根据SerialNo获取设备
        /// </summary>
        /// <param name="serialNo">设备序列号</param>
        /// <returns></returns>
        public Device this[string serialNo]
        {
            get
            {
                var cacheKey = string.Format(CacheKeys.DeviceDevicePoolSerialNo, serialNo);
                return AsyncHelper.RunSync(() => CacheContainer.GetCacheValue(cacheKey, () => new MultilevelCache<Device>(CachingProvider, cacheKey, async () =>
                {
                    #region IFreeSql

                    //var entity = await FreeSql.Select<Device>()
                    //    .Include(x => x.Supplier)
                    //    .Include(x => x.Sequence)
                    //    .Include(x => x.Threshold)
                    //    .Include(x => x.Project)
                    //    //.IncludeMany(x => x.Mediators)
                    //    .IncludeMany(x => x.Extends)
                    //    .Where(x => !x.IsDeleted && x.SerialNo == serialNo)
                    //    .WhereCascade(x => !x.IsDeleted)
                    //    .FirstAsync();

                    ////IFreeSql需要排除循环引用
                    //foreach (var extend in entity?.Extends)
                    //{
                    //    extend.Device = null;
                    //}

                    #endregion

                    #region efCore

                    var entity = await (await _deviceRepository.GetQueryableAsync())
                        .Include(x => x.Sequence)
                        .Include(x => x.Supplier)
                        .Include(x => x.Mediators)
                        .Include(x => x.Extends)
                        .ThenInclude(y => y.Device)
                        .Where(x => x.SerialNo == serialNo)
                        .FirstOrDefaultAsync();

                    #endregion

                    return entity;
                })));
            }
        }

        /// <summary>
        /// 根据fakeNo获取设备
        /// </summary>
        /// <param name="fakeNo">设备编码</param>
        /// <returns></returns>
        public Device GetByFakeNo(string fakeNo)
        {
            var cacheKey = string.Format(CacheKeys.DeviceDevicePoolFakeNo, fakeNo);
            return AsyncHelper.RunSync(() => CacheContainer.GetCacheValue(cacheKey, () => new MultilevelCache<Device>(CachingProvider, cacheKey, async () =>
            {
                #region IFreeSql

                //var entity = await FreeSql.Select<Device>()
                //    .Where(x => !x.IsDeleted && x.FakeNo == fakeNo)
                //    .WhereCascade(x => !x.IsDeleted)
                //    .Include(x => x.Supplier)
                //    .Include(x => x.Sequence)
                //    .Include(x => x.Threshold)
                //    .Include(x => x.Project)
                //    .IncludeMany(x => x.Mediators)
                //    .IncludeMany(x => x.Extends)
                //    .FirstAsync();

                ////IFreeSql需要排除循环引用
                //if (entity != null)
                //{
                //    foreach (var extended in entity.Extends)
                //    {
                //        extended.Device = null;
                //    }
                //    foreach (var mediator in entity.Mediators)
                //    {
                //        mediator.Device = null;
                //    }
                //}

                #endregion

                #region efCore

                var entity = await (await _deviceRepository.GetQueryableAsync())
                    .Include(x => x.Sequence)
                    .Include(x => x.Supplier)
                    .Include(x => x.Mediators)
                    .Include(x => x.Extends)
                    .ThenInclude(y => y.Device)
                    .Where(x => x.FakeNo == fakeNo)
                    .FirstOrDefaultAsync();

                #endregion

                return entity;
            })));
        }
    }
}
