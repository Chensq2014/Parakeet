using Common;
using Common.Storage;
using Localization.Resources.AbpUi;
using Parakeet.Net.Localization;
using Serilog;
using System.Threading;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Parakeet.Net;

[DependsOn(
    typeof(NetApplicationModule),
    typeof(NetApplicationContractsModule),
    typeof(AbpAccountHttpApiModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpTenantManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule)
    )]
public class NetHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiModule)} Start ConfigureServices ....");
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(AbpAspNetCoreMvcOptions)}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            //如果你的应用程序服务类名称为BookAppService.那么它将变为/book.
            //    如果要自定义命名, 则设置UrlControllerNameNormalizer选项. 它是一个委托允许你自定义每个控制器/服务的名称.
            //    如果该方法具有 'id'参数, 则会在路由中添加'/{id}'.
            //    如有必要,它会添加操作名称. 操作名称从服务上的方法名称获取并标准化;
            //删除'Async'后缀. 如果方法名称为'GetPhonesAsync',则变为GetPhones.
            //    删除HTTP method前缀. 基于的HTTP method删除GetList,GetAll,Get,Put,Update,Delete,Remove,Create,Add,Insert,Post和Patch前缀, 因此GetPhones变为Phones, 因为Get前缀和GET请求重复.
            //将结果转换为camelCase.
            //    如果生成的操作名称为空,则它不会添加到路径中.否则它会被添加到路由中(例如'/phones').对于GetAllAsync方法名称，它将为空,因为GetPhonesAsync方法名称将为phone.
            //    可以通过设置UrlActionNameNormalizer选项来自定义.It's an action delegate that is called for every method.
            //    如果有另一个带有'Id'后缀的参数,那么它也会作为最终路线段添加到路线中(例如'/phoneId').

            //Application的service生成的api/[AppName](配置文件中读) /[controller]/[Action]......
            options.ConventionalControllers.Create(typeof(NetApplicationModule).Assembly, option =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(AbpAspNetCoreMvcOptions)}.ConventionalControllers.Create{nameof(ConventionalControllerSetting)}    ConventionalControllers.Create 动态api RootPath配置....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                //它始终以 /api开头
                //路由路径. 默认值为"/app", 可以进行如下配置
                option.RootPath = "parakeet"; // api/parakeet/[controller]/[Action]
                                              //option.UrlControllerNameNormalizer = (contet)=> "AreaTest";
                                              ////通过提供TypePedicate选项进一步过滤类以成为API控制器
                                              //options.TypePredicate = type => true;//如果你不想将此类型公开为API控制器, 则可以在类型检查时返回false.
                                              //option.TypePredicate = type => type.Name.EndsWith("AppService");//以AppService结尾的才可以反射为动态controller
                                              //option.ControllerTypes.Add(typeof(CustomAppService));//自定义的AppService没有继承IAppService接口的可以手动添加也可以动态映射出api
            });

            ////配置DFS的API 路由前缀
            //options.ConventionalControllers.Create(typeof(DFSApplicationContractsModule).Assembly,option =>
            //{
            //    option.RootPath = "parakeet/dfs"; // api/parakeet/[controller]/[Action]
            //});
        });

        ////定义自定义设置值提供程序后,需要将其显式注册到 AbpSettingOptions
        //Configure<AbpSettingOptions>(options =>
        //{
        //    options.ValueProviders.Add<CustomSettingValueProvider>();
        //});

        ////使用HuNan别名的httClient工厂来构造 HuNan的httpClient 添加重试、熔断器、超时等策略
        //var timeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
        //context.Services.AddHttpClient(CustomerConsts.AppName, c =>
        //{
        //    Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddHttpClient配置{nameof(HttpClient)} :使用{CustomerConsts.AppName}别名的httClient工厂来构造 HuNan的httpClient 添加重试、熔断器、超时等策略 ....ConfigureServices中的{c.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        //    c.BaseAddress = new Uri(CustomConfigurationManager.Configuration["AuthServer:Authority"]); //http://www.parakeet.vip 设置默认Uri:80 端口号和api在client端设置
        //    c.DefaultRequestHeaders.Add("Accept", "application/json");
        //})
        //    .AddPolicyHandler(request => timeout)
        //    .AddTransientHttpErrorPolicy(p => p.RetryAsync(3));

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiModule)} End ConfigureServices ....");
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<NetResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
