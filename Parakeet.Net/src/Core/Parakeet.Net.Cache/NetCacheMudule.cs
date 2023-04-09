using Parakeet.Net.Cache.Extensions;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Cache
{
    /// <summary>
    /// Redis缓存模块
    /// </summary>
    [DependsOn(typeof(NetDomainSharedModule),
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(AbpCachingModule)
        //typeof(AbpDistributedLockingModule),
        //typeof(AbpDistributedLockingAbstractionsModule)
        )]
    public class NetCacheMudule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //CsRedis ServiceStackRedis  StackExchangeRedis 三选一
            //context.Services.AddCsRedisCache();//CsRedis
            //context.Services.AddServiceStackRedisCache();//ServiceStackRedis
            //StackExchangeRedis 在AbpCachingStackExchangeRedisModule 模块种已经注册 只需要配置EasyCaching即可
            context.Services.AddStackExchangeRedisCache();
        }
    }
}
