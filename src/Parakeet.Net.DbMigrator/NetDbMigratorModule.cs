using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using Parakeet.Net.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.MultiTenancy.ConfigurationStore;

namespace Parakeet.Net.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(NetEntityFrameworkCoreModule),
    typeof(NetApplicationContractsModule)
    )]
public class NetDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "net:"; });

        //ConfigConnectionOptions(context.Services);
    }

    protected void ConfigConnectionOptions(IServiceCollection services)
    {

        var configureation = services.GetConfiguration();//还无法获取到ConnectionStrings节点
        services.Configure<AbpDbConnectionOptions>(options =>
        {
            //var items=configureation.GetValue<List<string>>("ConnectionStrings");
            //foreach(var item in items)
            //{
            //    Console.WriteLine($"{item}");
            //}

            var portalConn = services.GetConfiguration()["ConnectionStrings:Portal"];
            //options.ConnectionStrings.Default = "default-value";
            options.ConnectionStrings[CommonConsts.PortalConnectionStringName] = portalConn;
            //options.ConnectionStrings["Saas"] = "Saas-default-value";
            //options.ConnectionStrings["Admin"] = "Admin-default-value";

            //options.Databases.Configure("Saas", database =>
            //{
            //    database.MappedConnections.Add("Saas1");
            //    database.MappedConnections.Add("Saas2");
            //    database.IsUsedByTenants = false;
            //});

            //options.Databases.Configure("Admin", database =>
            //{
            //    database.MappedConnections.Add("Admin1");
            //    database.MappedConnections.Add("Admin2");
            //});
        });

        //services.Configure<AbpDefaultTenantStoreOptions>(options =>
        //{
        //    options.Tenants = new[]
        //    {
        //            new TenantConfiguration(_tenant1Id, "tenant1")
        //            {
        //                ConnectionStrings =
        //                {
        //                    { ConnectionStrings.DefaultConnectionStringName, "tenant1-default-value"},
        //                    {"db1", "tenant1-db1-value"},
        //                    {"Admin", "tenant1-Admin-value"}
        //                }
        //            },
        //            new TenantConfiguration(_tenant2Id, "tenant2")
        //    };
        //});
    }
}
