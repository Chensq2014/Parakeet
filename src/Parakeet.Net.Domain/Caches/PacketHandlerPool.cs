using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Cache;
using Common.Entities;
using Common.Storage;
using Volo.Abp.Threading;

namespace Parakeet.Net.Caches
{
    /// <summary>
    /// tcp包头 高速缓存池，调用前请先确保单例注入PacketHandlerPool
    /// </summary>
    public class PacketHandlerPool
    {
        protected readonly ICacheContainer<MultilevelCache<List<PacketHandler>>, List<PacketHandler>> CacheContainer;
        protected readonly IEasyCachingProvider CachingProvider;
        //protected readonly IFreeSql FreeSql;//使用freesql进行了数据查询 需要单例注入
        //private readonly IRepository<PacketHandler, Guid> _packetHandlerRepository;//使用efcore 进行查询 注入IRepository仓储

        public PacketHandlerPool(
            ICacheContainer<MultilevelCache<List<PacketHandler>>, List<PacketHandler>> cacheContainer,
            IEasyCachingProvider cachingProvider
            //IFreeSql freeSql,
            //IRepository<PacketHandler, Guid> packetHandlerRepository
            )
        {
            CacheContainer = cacheContainer;
            CachingProvider = cachingProvider;
            //_packetHandlerRepository = packetHandlerRepository;
            //FreeSql = freeSql;
        }

        public List<PacketHandler> this[string header]
        {
            get
            {
                var cacheKey = string.Format(CacheKeys.DevicePacketHandlerPoolHeader, header);

                return AsyncHelper.RunSync(() => CacheContainer.GetCacheValue(cacheKey, () => new MultilevelCache<List<PacketHandler>>(CachingProvider, cacheKey, async () =>
                {
                    #region FreeSql

                    //var entities = await FreeSql.Select<PacketHandler>()
                    //    .Where(x => x.Header == header)
                    //    .WhereCascade(x => !x.IsDeleted)
                    //    .ToListAsync();
                    #endregion

                    #region efCore

                    //var entities = await (await _packetHandlerRepository.GetQueryableAsync())
                    //    .Where(x => x.Header == header)
                    //    .ToListAsync();

                    #endregion

                    #region Custom

                    var entities = new List<PacketHandler>
                    {
                        new PacketHandler(Guid.NewGuid())
                        {
                            Header = "test",
                            Handler="test"
                        }
                    };
                    
                    await Task.Delay(100);

                    #endregion

                    return entities;
                })
                {
                    RedisExpireTimeSpan = TimeSpan.FromDays(1)
                }));
            }
        }

    }
}
