using Common.Entities;
using FreeSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Parakeet.Net.Caches;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Common.Extensions
{
    /// <summary>
    /// 领域层 容器注册扩展
    /// </summary>
    public static class DomainServiceCollectionExtensions
    {
        /// <summary>
        /// 使用高速缓存(含redis缓存)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbConnectionString"></param>
        /// <param name="redisConnectionString"></param>
        public static void AddCachePool(this IServiceCollection services, string dbConnectionString = "", string redisConnectionString = "")
        {
            dbConnectionString = dbConnectionString.IsNullOrEmpty() ? services.GetConfiguration().GetConnectionString(CommonConsts.ConnectionStringName) : dbConnectionString;
            if (dbConnectionString.HasValue())
            {
                //var freeSql = new FreeSqlBuilder().UseConnectionString(DataType.MySql, dbConnectionString).Build();
                var freeSql = new FreeSqlBuilder().UseConnectionString(DataType.PostgreSQL, dbConnectionString).Build();
                // freeSql = new FreeSqlBuilder().UseConnectionString(DataType.SqlServer, dbConnectionString).Build();
                ConfigFluentApi(freeSql.CodeFirst);
                services.TryAddSingleton(freeSql);
            }
            services.AddStackExchangeRedisCache(options => { });
            //if (redisConnectionString.HasValue())
            //{
            //    services.AddCsRedisCache(redisConnectionString);
            //}
            //services.AddServiceStackRedisCache();

            services.TryAddSingleton<DevicePool>();
            services.TryAddSingleton<KeySecretPool>();
            services.TryAddSingleton<LicensePool>();
            services.TryAddSingleton<PacketHandlerPool>();
        }


        /// <summary>
        /// IFreeSql codeFirst的流式api
        /// </summary>
        /// <param name="codeFirst">代码优先接口(IFreeSql)</param>
        private static void ConfigFluentApi(ICodeFirst codeFirst)
        {
            codeFirst.ConfigEntity<Device>(x =>
            {
                x.Name(GetTableName<Device>());
                x.Navigate(y => y.Supplier, "SupplierId");
                x.Navigate(y => y.AreaTenant, "AreaTenantId");
                x.Navigate(y => y.Project, "ProjectId");
                x.Navigate(y => y.Threshold, "ThresholdId");
                x.Navigate(y => y.Sequence, "SequenceId");
                x.Navigate(y => y.Extends, "DeviceId");
            });

            codeFirst.ConfigEntity<DeviceExtend>(x =>
            {
                x.Name(GetTableName<DeviceExtend>());
                x.Navigate(y => y.Device, "DeviceId");
            });

            codeFirst.ConfigEntity<DeviceMediator>(x =>
            {
                x.Name(GetTableName<DeviceMediator>());
                x.Navigate(y => y.Device, "DeviceId");
                x.Navigate(y => y.Mediator, "MediatorId");
            });

            codeFirst.ConfigEntity<DeviceSequence>(x =>
            {
                x.Name(GetTableName<DeviceSequence>());
                x.Navigate(y => y.Device, "DeviceId");
            });

            codeFirst.ConfigEntity<Supplier>(x =>
            {
                x.Name(GetTableName<Supplier>());
                x.Navigate(y => y.Devices, "DeviceId");
            });

            codeFirst.ConfigEntity<AreaTenant>(x =>
            {
                x.Name(GetTableName<AreaTenant>());
                x.Navigate(y => y.Devices, "DeviceId");
            });
            codeFirst.ConfigEntity<Threshold>(x =>
            {
                x.Name(GetTableName<Threshold>());
                x.Navigate(y => y.Devices, "DeviceId");
            });
            codeFirst.ConfigEntity<Project>(x =>
            {
                x.Name(GetTableName<Project>());
                x.Navigate(y => y.Devices, "DeviceId");
            });
            codeFirst.ConfigEntity<Mediator>(x => { x.Name(GetTableName<Mediator>()); });

            codeFirst.ConfigEntity<TenantDbConnectionString>(x =>
            {
                x.Navigate(y => y.AreaTenant, "AreaTenantId");
                x.Name(GetTableName<TenantDbConnectionString>());
            });
            codeFirst.ConfigEntity<License>(x =>
            {
                x.Name(GetTableName<License>());
                x.Navigate(y => y.LicenseResources, "LicenseId");
            });
            codeFirst.ConfigEntity<LicenseResource>(x =>
            {
                x.Name(GetTableName<LicenseResource>());
                x.Navigate(y => y.License, "LicenseId");
            });
            //codeFirst.ConfigEntity<PacketHandler>(x =>
            //{
            //    x.Name(GetTableName<PacketHandler>());
            //});

            //codeFirst.ConfigEntity<KeySecret>(x =>
            //{
            //    x.Name(GetTableName<KeySecret>());
            //});

            codeFirst.ConfigEntity<Worker>(x =>
            {
                x.Name(GetTableName<Worker>());
                x.Property(t => t.IdPhoto).DbType("text");
                x.Property(t => t.Photo).DbType("text");
                x.Property(t => t.InfraredPhoto).DbType("text");
            });
            codeFirst.ConfigEntity<WorkerType>(x =>
            {
                x.Name(GetTableName<WorkerType>());
            });
            codeFirst.ConfigEntity<DeviceWorker>(x =>
            {
                x.Name(GetTableName<DeviceWorker>());
                x.Navigate(t => t.AreaTenant, "AreaTenantId");
                x.Navigate(y => y.Worker, "WorkerId");
            });
        }


        /// <summary>
        /// 获取表名
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <returns></returns>
        public static string GetTableName<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? $"{type.Name}s";
            return tableName;
        }


        /// <summary>
        /// CodeFirstActions 字典
        /// </summary>
        public static Dictionary<Type, List<Action<ICodeFirst>>> CodeFirstActions = new Dictionary<Type, List<Action<ICodeFirst>>>();

        /// <summary>
        /// 添加区域租户FreeSql CodeFirst 服务
        /// </summary>
        /// <typeparam name="TModule"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="areaCode"></param>
        /// <param name="configFluentApi"></param>
        /// <param name="useMaster"></param>
        /// <returns></returns>
        public static IServiceCollection AddTenantFreeSql<TModule>(this IServiceCollection serviceCollection, string areaCode, Action<ICodeFirst> configFluentApi, bool useMaster = true) where TModule : AbpModule
        {
            if (!CodeFirstActions.ContainsKey(typeof(TModule)))
            {
                CodeFirstActions.Add(typeof(TModule), new List<Action<ICodeFirst>>());
            }
            CodeFirstActions[typeof(TModule)].Add(configFluentApi);
            serviceCollection.AddSingleton(provider =>
            {
                var year = DateTime.Now.Year;
                var tenant = provider.GetService<IFreeSql>().Select<AreaTenant>()
                    .IncludeMany(x => x.TenantDbConnectionStrings)
                    .WhereCascade(x => !x.IsDeleted)
                    .Where(x => x.AreaCode == areaCode)
                    .First();
                Check.NotNull(tenant, nameof(tenant), $"无法在数据库中找到区域租户[{areaCode}],请先在数据中进行配置");
                var connectionString = tenant.TenantDbConnectionStrings.FirstOrDefault(x => x.IsMaster == useMaster && x.Date?.Year == year);
                Check.NotNull(connectionString, nameof(connectionString), $"区域租户[{areaCode}],未配置{year}年的{(useMaster ? "Master" : "Slave")}数据库连接字符串");
                var freeSql = new FreeSqlBuilder()
                    .UseConnectionString(DataType.PostgreSQL, connectionString.Value)
                    .Build<TModule>();
                foreach (var codeFirstAction in CodeFirstActions[typeof(TModule)])
                {
                    codeFirstAction(freeSql.CodeFirst);
                }

                return freeSql;
            });

            return serviceCollection;
        }
    }
}
