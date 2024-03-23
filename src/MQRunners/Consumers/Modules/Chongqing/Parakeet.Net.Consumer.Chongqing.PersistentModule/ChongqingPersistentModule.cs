using Common;
using Common.Entities;
using Common.Extensions;
using Common.RabbitMQModule.Core;
using Common.Storage;
using FreeSql;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Consumer.Chongqing.PersistentModule
{
    /// <summary>
    /// Chongqing存储模块
    /// </summary>
    [DependsOn(typeof(ChongqingModule))]
    public class ChongqingPersistentModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingPersistentModule)} Start  ConfigureServices ....");

            #region 注入区域FreeSql 以便consumer依赖注入进行批量插入

            context.Services.AddTenantFreeSql<ChongqingPersistentModule>(Magics.CHONGQING, ConfigFluentApi);

            #endregion
            base.ConfigureServices(context);

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingPersistentModule)} End  ConfigureServices ....");
        }


        private void ConfigFluentApi(ICodeFirst codeFirst)
        {
            codeFirst.ConfigEntity<EnvironmentBase>(x =>
            {
                x.Name(DomainServiceCollectionExtensions.GetTableName<EnvironmentBase>());
            });
            codeFirst.ConfigEntity<GateBase>(x =>
            {
                x.Name(DomainServiceCollectionExtensions.GetTableName<GateBase>());
            });
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingPersistentModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、 获取IRabbitMQEventBusContainer 然后AutoPersistent 自动注册并绑定当前模块的生产/消费者....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(ChongqingPersistentModule).Assembly });

            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingPersistentModule)} End  OnApplicationInitialization ....");
        }
    }
}
