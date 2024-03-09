using Common.Cache;
using Common.Storage;
using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Common.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;

namespace Parakeet.Net.Caches
{
    /// <summary>
    /// 许可证高速缓存池，调用前请先确保单例注入LicensePool 
    /// </summary>
    public class LicensePool
    {
        protected readonly ICacheContainer<MultilevelCache<License>, License> CacheContainer;
        protected readonly IEasyCachingProvider CachingProvider;
        //protected readonly IFreeSql FreeSql;//使用freesql进行了数据查询 需要单例注入
        private readonly IRepository<License, Guid> _licenseRepository;//使用efcore 进行查询 注入IRepository仓储

        public LicensePool(ICacheContainer<MultilevelCache<License>, License> cacheContainer,
            IEasyCachingProvider cachingProvider, 
            //IFreeSql freeSql,
            IRepository<License, Guid> licenseRepository)
        {
            CacheContainer = cacheContainer;
            CachingProvider = cachingProvider;
            _licenseRepository = licenseRepository;
            //FreeSql = freeSql;
        }


        /// <summary>
        /// 根据AppId获取许可证
        /// </summary>
        /// <param name="appId">许可证Id</param>
        /// <param name="appKey">许可证key</param>
        /// <returns></returns>
        public License this[string appId,string appKey]
        {
            get
            {
                var cacheKey = string.Format(CacheKeys.LicenseAppIdAppKey, appId,appKey);
                return AsyncHelper.RunSync(() => CacheContainer.GetCacheValue(cacheKey, () => new MultilevelCache<License>(CachingProvider, cacheKey, async () =>
                {
                    #region IFreeSql

                    //var entity = await FreeSql.Select<License>()
                    //    .IncludeMany(x => x.LicenseResources)
                    //    .Where(x => !x.IsDeleted && x.AppId == appId && x.AppKey == appKey)
                    //    .WhereCascade(x => !x.IsDeleted)
                    //    .FirstAsync();

                    ////IFreeSql需要排除循环引用
                    //foreach (var extended in entity?.LicenseResources)
                    //{
                    //    extended.License = null;
                    //}

                    #endregion

                    #region efCore

                    var entity = await (await _licenseRepository.GetQueryableAsync())
                        .Include(x => x.LicenseResources)
                        .Where(x => x.AppId == appId && x.AppKey == appKey)
                        .FirstOrDefaultAsync();

                    #endregion

                    return entity;
                })));
            }
        }

        /// <summary>
        /// 根据Name获取许可证
        /// </summary>
        /// <param name="name">许可证名称</param>
        /// <returns></returns>
        public License GetByName(string name)
        {
            var cacheKey = string.Format(CacheKeys.LicenseName, name);
            return AsyncHelper.RunSync(() => CacheContainer.GetCacheValue(cacheKey, () => new MultilevelCache<License>(CachingProvider, cacheKey, async () =>
            {
                #region IFreeSql

                //var entity = await FreeSql.Select<License>()
                //    .Where(x => !x.IsDeleted && x.Name == name)
                //    .WhereCascade(x => !x.IsDeleted)
                //    .IncludeMany(x => x.LicenseResources)
                //    .FirstAsync();

                ////IFreeSql需要排除循环引用
                //foreach (var extended in entity?.LicenseResources)
                //{
                //    extended.License = null;
                //}

                #endregion

                #region efCcore

                var entity = await (await _licenseRepository.GetQueryableAsync())
                    .Include(x => x.LicenseResources)
                    .Where(x => x.Name == name)
                    .FirstOrDefaultAsync();

                #endregion

                return entity;
            })));
        }


        /// <summary>
        /// 根据Id获取License 许可证
        /// </summary>
        /// <param name="id">许可证Id</param>
        /// <returns></returns>
        public License this[Guid id]
        {
            get
            {
                var cacheKey = string.Format(CacheKeys.LicenseId, id);
                return AsyncHelper.RunSync(() => CacheContainer.GetCacheValue(cacheKey, () => new MultilevelCache<License>(CachingProvider, cacheKey, async () =>
                {
                    #region FreeSql

                    //var entity = await FreeSql.Select<License>()
                    //    .Where(x => !x.IsDeleted&& x.Id == id)
                    //    .IncludeMany(x => x.LicenseResources)
                    //    .FirstAsync();

                    //foreach (var resource in entity?.LicenseResources)
                    //{
                    //    resource.License = null;
                    //}
                    #endregion

                    #region efcore

                    var entity = await (await _licenseRepository.GetQueryableAsync())
                        .Include(x => x.LicenseResources)
                        .Where(x => x.Id == id)
                        .FirstAsync();

                    #endregion

                    return entity;
                })));
            }
        }
    }
}
