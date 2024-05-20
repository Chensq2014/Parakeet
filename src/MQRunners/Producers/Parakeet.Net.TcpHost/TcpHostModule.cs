using Common.TcpMudule;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Producer;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Parakeet.Net.TcpHost
{
    [DependsOn(typeof(ProducerModule), 
        typeof(TcpCoreMudule),
        typeof(AbpAutofacModule))]
    public class TcpHostModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            //context.Services.AddLicense();//私有化部署 读取mac地址等信息 与配置文件中的信息做匹配【暂未使用】
            //context.Services.AddAreaTenantFreeSql<ChongqingShareModule>(AppConstants.DB_AREA, (codeFirst) => { });
            //context.Services.AddTcpServer();//TcpCoreMudule

            ////配置自定义模块 StartUp中的ConfigureServices  向容器中注册服务
            //context.Services.ConfigureModuleServices(configuration);

            context.Services.AddHostedService(provider => new TcpHostService(provider));
            base.ConfigureServices(context);
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            //context.ServiceProvider.GetService<T>()
            base.OnApplicationInitialization(context);
        }
    }
}
