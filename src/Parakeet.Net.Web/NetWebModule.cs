using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Common;
using Common.Dtos;
using Common.Enums;
using Common.EnumServices;
using Common.Extensions;
using Common.Storage;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Parakeet.Net.Localization;
using Parakeet.Net.Web.Menus;
using StackExchange.Redis;
using Microsoft.OpenApi.Models;
using Serilog;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Http.Client.Web;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.Security.Claims;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Nest;
using System.Net.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Localization.Resources.AbpUi;
using Volo.Abp.Localization;
using Microsoft.IdentityModel.Logging;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Volo.Abp.Data;
using Volo.Abp.Threading;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.ResponseCaching;
using Polly;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Common.Helpers;
using Exceptionless;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Logging;
using Parakeet.Net.Aop;
using Parakeet.Net.Extentions;
using Parakeet.Net.GrpcLessonServer;
using Parakeet.Net.GrpcService;
using Parakeet.Net.ServiceGroup;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Parakeet.Net.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Identity.Web;

namespace Parakeet.Net.Web;

[DependsOn(
    typeof(NetHttpApiClientModule),
    typeof(NetHttpApiModule),
    typeof(NetApplicationModule),
    typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule),
    typeof(AbpAspNetCoreMvcClientModule),
    typeof(AbpHttpClientWebModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpSettingManagementWebModule),
    typeof(AbpHttpClientIdentityModelWebModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
    )]
public class NetWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(NetResource),
                typeof(NetDomainModule).Assembly,
                typeof(NetDomainSharedModule).Assembly,
                typeof(NetApplicationModule).Assembly,
                typeof(NetApplicationContractsModule).Assembly,
                typeof(NetWebModule).Assembly
            );
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} Start ConfigureServices ....");
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        ConfigureBundles();//mvc前端配置压缩的资源bundles
        ConfigureCache(context);//配置缓存和Redis 在domain模块已配置
        ConfigureFilters(context);//mvc的全局filter统一配置 AddMvc
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureDistributedLocking(context, configuration);
        ConfigureUrls(configuration);
        ConfigureAuthentication(context);//配置鉴权
        ConfigureAuthorization(context);//配置授权
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices(configuration);
        ConfigureGrpcs(context);
        //ConfigureMultiTenancy();//放在NetMultiTenancyModule里面
        ConfigureSwaggerServices(context);//配置swagger

        ConfigCookie(context);
        ConfigSession(context);
        ConfigureCors(context); //配置跨域
        ConfigureHsts(context);
        ConfigureHttpPolly(context);
        ConfigureCompressServices(context);
        ConfigureLocalizationServices();
        ConfigureAbpAntiForgerys();
        ConfigureFileUploadOptions();

        if (!hostingEnvironment.IsDevelopment())
        {
            //https 需要增加443端口
            //context.Services.AddHttpsRedirection(option => option.HttpsPort = 443);
            //ConfigurHsts(context);
        }
        //context.Services.AddControllersWithViews()//反射收集dll-控制器--action--PartManager
        //    .AddNewtonsoftJson();

        Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、AddDirectoryBrowser:配置允许指定的目录浏览 AddRazorPages:支持Razor 的Pages view模式....ConfigureServices中的流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        context.Services.AddDirectoryBrowser();//允许指定的目录浏览


        //Configure<AbpAuditingOptions>(options =>
        //{
        //    //options.IsEnabledForGetRequests = true;
        //    options.ApplicationName = "parakeet";
        //});
        //Configure<AbpBackgroundJobOptions>(options =>
        //{
        //    options.IsJobExecutionEnabled = false;
        //});

        //AddControllersWithViews/AddRazorPages/AddControllersCore-->AddMvcCore/AddAuthorization(包含了授权的服务注册)
        //AddAuthorization-->AddAuthorizationCore、AddAuthorizationPolicyEvaluator
        //AddAuthorizationCore-->注册IAuthorizationService、IAuthorizationHandler....
        context.Services.AddRazorPages(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddRazorPages (参数委托)配置{nameof(RazorPagesOptions)},【MVC流程日志：1、ConfigreServices最后的流程AddRazorPages，开始MVC资源及配置】AddRazorPages-->AddMvcCore/-->ApplicationPartManager(一次性加载当前及关联的所有终结点(Action/Controller)所在程序集dll，准备好控制器提供器，完成MVC处理动作初始化，包括Controller-Action,为中间件UseRouting 匹配路由做准备)...ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

        })//支持Razor 的Pages view模式
          //.AddNewtonsoftJson(options =>
          //{
          //    Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddNewtonsoftJson {nameof(MvcNewtonsoftJsonOptions)} 设置 返回数据给前端序列化时key为驼峰样式,时区 日期格式 空对象处理，忽略循环引用等配置...ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
          //    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); //序列化时key为驼峰样式
          //    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
          //    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
          //    options.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
          //    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//忽略循环引用
          //})
            .AddJsonOptions(options =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddJsonOptions {nameof(JsonOptions)} 设置 返回数据给前端序列化时key为驼峰样式,时区 日期格式 空对象处理，忽略循环引用等配置...ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                //options.JsonSerializerOptions.MaxDepth = 2;
                //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;//序列化后驼峰命名规则
                //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            })
            .AddRazorRuntimeCompilation();//修改cshtml后能自动编译
                                          //context.Services.AddControllers(options =>
                                          //    {
                                          //        var jsonInputFormatter = options.InputFormatters
                                          //            .OfType<Microsoft.AspNetCore.Mvc.Formatters.NewtonsoftJsonInputFormatter>()
                                          //            .First();
                                          //        jsonInputFormatter.SupportedMediaTypes.Add("multipart/form-data; boundary=*");
                                          //    }
                                          //);

        //AddControllersWithViews-->AddControllersCore->AddMvcCore/AddAuthorization(包含了授权的服务注册)
        //context.Services.AddControllersWithViews(
        //    options =>
        //    {
        //        options.Filters.Add<CustomExceptionFilterAttribute>();//全局注册
        //        options.Filters.Add<CustomGlobalActionFilterAttribute>();
        //    })
        //    .AddNewtonsoftJson(options =>
        //        {
        //            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); //序列化时key为驼峰样式
        //            options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
        //            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        //            options.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
        //            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//忽略循环引用
        //        })
        //    .AddRazorRuntimeCompilation();//修改cshtml后能自动编译


        #region 参数验证自定义和mvc过滤器

        ////禁用ModelState默认行为 覆盖ModelState管理的默认行为
        //context.Services.Configure<ApiBehaviorOptions>(options =>
        //{
        //    //第一种方案：符合Framework时代的风格，需要额外在指定覆盖原有的模型验证
        //    //options.SuppressModelStateInvalidFilter = true;
        //    //官方建议做法，符合Core时代的风格，只需复写InvalidModelStateResponseFactory委托即可
        //    options.InvalidModelStateResponseFactory = (context) =>
        //    {
        //        var error = context.ModelState.GetValidationSummary();
        //        return new JsonResult(Result.FromError($"参数验证不通过：{error.ToString()}", ResultCode.InvalidParams));
        //    };
        //});

        ////老版本需要账号密码登录设置的全局过滤器 使用identityserver后不再需要
        //context.Services.AddMvc(o =>
        //{
        //    //o.Filters.Add<CustomExceptionFilterAttribute>();//异常处理全局filter
        //    //o.Filters.Add(typeof(CustomGlobalActionFilterAttribute));//全局注册filter
        //    o.Filters.Add(typeof(CustomAuthorityActionFilterAttribute)); //Mvc5 全局权限验证 (项目搬迁)
        //});

        #endregion

        //ConfigureDbContext(context);//可以单独配置本Module的dbcontext及连接字符串


        //ConfigurePlugins(context);


        #region HttpClient请求Policy配置
        ConfigureHttpPolly(context);
        #endregion

        #region 标准中间件注册option套路

        ////context.Services.AddBrowserFilter();//无option注册
        //context.Services.AddBrowserFilter(option =>
        //{
        //    //option.EnableIE = false;
        //    //option.EnableEdge = false;
        //    //option.EnableChorme = false;
        //    option.EnableFirefox = true;
        //});//option委托1

        //context.Services.AddBrowserFilter(option =>
        //{
        //    option.EnableIE = true;
        //    option.EnableEdge = true;
        //    option.EnableChorme = true;
        //    option.EnableFirefox = true;
        //});//option委托2

        //context.Services.AddMiddlewareFactory();
        //context.Services.AddInheritedMiddleware();

        #endregion

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} End ConfigureServices ....");
        //设置一个全局的解析Provider提供器，使用的地方不再依赖注入 待测试

    }

    #region 所有配置项

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpBundlingOptions)} ScriptBundles:{LeptonXLiteThemeBundles.Styles.Global},StyleBundles:{LeptonXLiteThemeBundles.Scripts.Global}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.ScriptBundles
                //.Add(CustomerConsts.GlobalBundleName, bundle =>
                .Configure(LeptonXLiteThemeBundles.Styles.Global, bundle =>
                {
                    bundle.AddFiles(
                        "/js/devextreme/cldr.min.js",
                        "/js/devextreme/cldr/event.min.js",
                        "/js/devextreme/cldr/supplemental.min.js",
                        "/js/devextreme/cldr/unresolved.min.js",
                        "/js/devextreme/globalize.min.js",
                        "/js/devextreme/globalize/message.min.js",
                        "/js/devextreme/globalize/number.min.js",
                        "/js/devextreme/globalize/currency.min.js",
                        "/js/devextreme/globalize/date.min.js",
                        "/js/devextreme/jszip.min.js",
                        "/js/devextreme/dx.all.js",
                        "/js/devextreme/vectormap-data/world.js",//加;去"use strict"
                        "/js/devextreme/aspnet/dx.aspnet.mvc.js",//加;去"use strict"
                        "/js/devextreme/aspnet/dx.aspnet.data.js",//加;去"use strict"
                        "/js/devextreme/localization/dx.messages.zh.js",//去"use strict"
                        "/js/devextreme/localization/supplemental.js",
                        "/js/devextreme/localization/zh.js",
                        "/plugins/devextreme/dx.extension.js",
                        "/plugins/devextreme/dx.page.extension.js",
                        "/plugins/devextreme/common.js",
                        "/plugins/drags/dxDragFiles.js",
                        "/js/site.js"
                    );
                });

            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles(
                        "/css/devextreme/dx.common.css",
                        "/css/devextreme/dx.light.css",
                        "/css/default.css",
                        "/css/site.min.css",
                        "/global-styles.css"
                        );
                }
            );
        });
    }

    /// <summary>
    /// 配置分布式缓存，Redis缓存连接字符串,前缀等
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureCache(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureCache...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        //var configuration = context.Services.GetConfiguration();
        //var hostingEnvironment = context.Services.GetHostingEnvironment();

        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "net:";
        });

        #region Redis 缓存已放入领域层


        #region Caching.CSRedis 跟StackExchangeRedis 做的事情一样 

        //Log.Logger.Information($"配置Caching.CSRedis(跟StackExchangeRedis 做的事情一样)....");
        //Microsoft.Extensions.Caching.Redis--CSRedisCore   
        //Microsoft.AspNetCore.DataProtection.StackExchangeRedis  使用这个便于配置



        //缓存已放入领域层先执行,这里是外层模块后执行
        //context.Services.AddStackExchangeRedisCache();
        //context.Services.AddCachePool();//包含 context.Services.AddCsRedisCache();
        //context.Services.ConfigAbpDistributeCacheOptions();

        #endregion


        #region 老版本

        //Configuration.Caching.UseRedis(options =>
        //{
        //    options.ConnectionString = configuration["Abp:RedisCache:ConnectionStrings"];
        //    options.DatabaseId = Convert.ToInt32(configuration["Abp:RedisCache:DatabaseId"]);
        //});
        //Configuration.Caching.ConfigureAll(cache =>
        //{
        //    //缓存默认过期时间设置为2h
        //    cache.DefaultSlidingExpireTime = TimeSpan.FromHours(2);
        //});
        ////用户个性签名验证码等缓存5分钟
        //Configuration.Caching.Configure(CustomerConsts.PersonalCaches, cache =>
        //{
        //    cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(5);
        //});

        #endregion

        #endregion

        context.Services.AddResponseCaching(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddResponseCaching参数委托配置{nameof(ResponseCachingOptions)} (客户端同一请求应答缓存,暂测无效)....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        });

        //if (!hostingEnvironment.IsDevelopment())
        //{
        //    var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
        //    context.Services
        //        .AddDataProtection()
        //        .PersistKeysToStackExchangeRedis(redis, "Parakeet-Protection-Keys");
        //}
    }


    private void ConfigureFilters(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureFilters...ConfigureServices中的流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        //mvc全局过滤器
        context.Services.AddMvc(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddMvc配置{nameof(MvcOptions)} mvc全局过滤器....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】  ");
            //options.Filters.Add(typeof(AreaAttribute));
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、配置{nameof(AppUrlOptions)}....ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        Configure<AppUrlOptions>(options =>
        {
            Log.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AppUrlOptions)}  RootUrl={configuration["App:SelfUrl"]}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureMultiTenancy()
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureMultiTenancy...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        Configure<AbpMultiTenancyOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpMultiTenancyOptions)} 是否启用多租户配置:{CommonConsts.MultiTenancyEnabled}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.IsEnabled = CommonConsts.MultiTenancyEnabled;
        });
    }


    /// <summary>
    /// Authentication认证
    /// 通过添加AddAuthentication注册了AuthenticationService, AuthenticationHandlerProvider,AuthenticationSchemeProvider
    /// 
    /// 配置权限使用IdentityServer，IdentityServer服务器地址，
    /// 注册到 ApiName到IdentityServer服务器
    ///
    ///
    /// jwt   https://jwt.io/   debugger
    /// 通过加密算法来建立信任 HS256(对称) RS256(非对称)
    /// 其实还是那套东西
    /// 1、登录写入凭证
    /// 2、鉴权就是找出用户
    /// 3、授权就是判断权限
    /// 4、退出就是清理凭证
    /// 
    /// 鉴权授权里面，是通过AuthenticationHttpContextExtensions的5个方法—
    /// SignInAsync, AuthenticateAsync,ForbidAsync, ChallengeAsync, SignOutAsync【AuthenticationService的5个方法】
    /// -其实最终还是要写cookie/session/信息
    /// 
    ///     都是调用的IAuthenticationService，ConfigureService注册
    ///     AuthenticationCoreServiceCollectionExtensions.AddAuthenticationCore
    ///         IAuthenticationService
    ///         IAuthenticationHandlerProvider
    ///         IAuthenticationSchemeProvider
    /// 
    /// 5个方法默认就在AuthenticationService，找handler完成处理
    ///     造轮子
    /// 
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureAuthentication...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        var configuration = context.Services.GetConfiguration();

        #region 客户端模式

        //context.Services.AddIdentityServer() //定义处理规则
        //    .AddDeveloperSigningCredential() //默认的开发者证书--临时证书--生产环境为了保证token不失效，证书是不变的
        //    .AddInMemoryClients(ClientInitConfig.GetClients())
        //    .AddInMemoryApiResources(ClientInitConfig.GetApiResources());

        #endregion

        #region 客户端 密码模式 注意 如果要给第三方使用token 就不要再AddCookie()【如果AddCookie 第三方Header里面就必须传递cookie】

        ////通过添加AddAuthentication注册了AuthenticationService, AuthenticationHandlerProvider,AuthenticationSchemeProvider这三个对象
        ////通过AddAuthentication返回的AuthenticationBuilder通过AddJwtBearer（或者AddCookie）
        ////来指定Scheme类型和需要验证的参数,并指定了AuthenticationHandler
        ////Bearer (scheme 策略名称)
        ////IdentityServerAuthenticationDefaults.AuthenticationScheme CookieAuthenticationDefaults.AuthenticationScheme
        //context.Services.AddAuthentication(options =>
        //{
        //    //注册Scheme：Bearer/Cookie
        //    //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //    //options.DefaultChallengeScheme = "oidc";//OpenIdConnectDefaults.AuthenticationScheme
        //})
        //    //.AddJwtBearer(options =>//AddIdentityServerAuthentication已替换AddJwtBearer
        //    //    {
        //    //        options.Authority = context.Services.GetConfiguration()["AuthServer:Authority"];
        //    //        options.RequireHttpsMetadata = false;
        //    //        //options.Audience = configuration["AuthServer:ApiName"] ?? "parakeet";
        //    //        options.TokenValidationParameters = new TokenValidationParameters
        //    //        {
        //    //            ValidateAudience = false
        //    //        };
        //    //    })
        //    //.AddOpenIdConnect("oidc", options =>
        //    //{
        //    //    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //    //    options.Authority = context.Services.GetConfiguration()["AuthServer:Authority"];
        //    //    options.RequireHttpsMetadata = false;
        //    //    options.ClientId = "DataCenterWebApp";
        //    //    options.SaveTokens = true;
        //    //})
        //    .AddIdentityServerAuthentication(options =>//定义校验规则如何校验
        //    {
        //        Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddAuthentication AddIdentityServerAuthentication配置{nameof(IdentityServerAuthenticationOptions)} idserver4服务器授权地址:{configuration["AuthServer:Authority"]}Api:{configuration["AuthServer:ApiName"] ?? "parakeet"}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        //        //idserver4授权地址(应该是获取公钥,还有自身api资源名称需要注册到ids4)
        //        //声明自身服务器api资源名称(parakeet) 注册到ids4...(客户端请求ids时，ids返回token中就带有这个(parakeet作用域)信息)
        //        options.Authority = configuration["AuthServer:Authority"];
        //        options.RequireHttpsMetadata = false;
        //        options.ApiName = configuration["AuthServer:ApiName"] ?? "parakeet";
        //        options.JwtBackChannelHandler = new HttpClientHandler
        //        {
        //            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        //        };
        //    });

        #endregion

        #region 最基础认证--自定义Handler 只要Scheme【解决方案不同】 就可以多个Scheme
        //////services.AddAuthentication().AddCookie();-->
        //////AddAuthentication()-->services.AddAuthenticationCore();//本质
        //context.Services.AddAuthenticationCore(options => options.AddScheme<CustomHandler>("CustomScheme", "DemoScheme"));
        #endregion

        #region cookie方式验证 初始化登录地址与无权限时跳转地址

        ////cookie方式验证 初始化登录地址与无权限时跳转地址
        ////context.Services.AddAuthentication(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme);
        //context.Services.AddAuthentication(options =>
        //    {
        //        //默认解决方案名称
        //        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;//Scheme不能少
        //        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //        options.DefaultChallengeScheme = "Account/Login";
        //    })
        //    .AddCookie(authenticationScheme: CookieAuthenticationDefaults.AuthenticationScheme, configureOptions: options =>
        //    {
        //        //这里可以使用context.Services.BuildServiceProvider()
        //        //是因为这是内部委托，已经是Build之后才会执行的代码了
        //        Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddCookie配置{nameof(NetCoreWebModule)} AddAuthentication....AddCookie中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        //        options.SessionStore = context.Services.BuildServiceProvider().GetRequiredService<ITicketStore>();
        //        options.LoginPath = new PathString("/Account/Login");//登录地址
        //        options.AccessDeniedPath = new PathString("/Account/Login");//未授权跳转地址

        //        options.Events = new CookieAuthenticationEvents()
        //        {
        //            //扩展事件
        //            OnSignedIn = new Func<CookieSignedInContext, Task>(
        //                async ctx =>
        //                {
        //                    Console.WriteLine($"{ctx.Request.Path} is OnSignedIn");
        //                    await Task.CompletedTask;
        //                }),
        //            OnSigningIn = async ctx =>
        //            {
        //                Console.WriteLine($"{ctx.Request.Path} is OnSigningIn");
        //                await Task.CompletedTask;
        //            },
        //            OnSigningOut = async ctx =>
        //            {
        //                Console.WriteLine($"{ctx.Request.Path} is OnSigningOut");
        //                await Task.CompletedTask;
        //            }
        //        };
        //    });

        //AddCookie 里面ITicketStore需先注入容器
        //context.Services.AddScoped<ITicketStore, MemoryCacheTicketStore>();
        ////context.Services.AddAuthorization(options =>
        ////{
        ////    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        ////        .RequireAuthenticatedUser()
        ////        .Build();
        ////});
        #endregion

        #region 回退策略FallbackPolicy 默认为 null

        //// 未提供IAuthorizeData时CombineAsync(IAuthorizationPolicyProvider, IEnumerable<IAuthorizeData>)使用的回退授权策略。
        //// 因此，如果资源没有IAuthorizeData实例，AuthorizationMiddleware 将使用回退策略。
        //// 如果资源具有任何IAuthorizeData， 则将评估它们而不是回退策略。默认情况下，回退策略为 null，
        //// 除非您的管道中有 AuthorizationMiddleware，否则通常不会产生任何影响。
        //// 默认IAuthorizationService不以任何方式使用它。
        //context.Services.AddAuthorization(options =>
        //{
        //    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        //        .RequireAuthenticatedUser()
        //        .Build();
        //});
        #endregion


        #region AddAuthentication 只允许配置一次并且连续链式调用，保证在同一个builder里面包含所有配置(方案等命名空间一致)

        //注意 如果多次AddAuthentication 就会创建多个builder造成冲突或命名空间不一致

        context.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";//OpenIdConnectDefaults.AuthenticationScheme;//
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignInScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
                options.CheckTokenExpiration();
            })
            .AddAbpOpenIdConnect("oidc", options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata");
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                options.ClientId = configuration["AuthServer:ClientId"];
                options.ClientSecret = EncodingEncryptHelper.DEncrypt(configuration["AuthServer:ClientSecret"]);

                options.UsePkce = true;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("roles");
                options.Scope.Add("email");
                options.Scope.Add("phone");
                options.Scope.Add("parakeet");
            })
            .AddMicrosoftIdentityWebApp(context.Services.GetConfiguration(), CommonConsts.AzureAdSectionName, OpenIdConnectDefaults.AuthenticationScheme);
        /*
        * This configuration is used when the AuthServer is running on the internal network such as docker or k8s.
        * Configuring the redirecting URLs for internal network and the web
        * The login and the logout URLs are configured to redirect to the AuthServer real DNS for browser.
        * The token acquired and validated from the the internal network AuthServer URL.
        */
        if (configuration.GetValue<bool>("AuthServer:IsContainerized"))
        {
            context.Services.Configure<OpenIdConnectOptions>("oidc", options =>
            {
                options.TokenValidationParameters.ValidIssuers = new[]
                {
                        configuration["AuthServer:MetaAddress"]!.EnsureEndsWith('/'),
                        configuration["AuthServer:Authority"]!.EnsureEndsWith('/')
                    };

                options.MetadataAddress = configuration["AuthServer:MetaAddress"]!.EnsureEndsWith('/') +
                                        ".well-known/openid-configuration";

                var previousOnRedirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;
                options.Events.OnRedirectToIdentityProvider = async ctx =>
                {
                    // Intercept the redirection so the browser navigates to the right URL in your host
                    ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"]!.EnsureEndsWith('/') + "connect/authorize";

                    if (previousOnRedirectToIdentityProvider != null)
                    {
                        await previousOnRedirectToIdentityProvider(ctx);
                    }
                };
                var previousOnRedirectToIdentityProviderForSignOut = options.Events.OnRedirectToIdentityProviderForSignOut;
                options.Events.OnRedirectToIdentityProviderForSignOut = async ctx =>
                {
                    // Intercept the redirection for signout so the browser navigates to the right URL in your host
                    ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"]!.EnsureEndsWith('/') + "connect/logout";

                    if (previousOnRedirectToIdentityProviderForSignOut != null)
                    {
                        await previousOnRedirectToIdentityProviderForSignOut(ctx);
                    }
                };
            });
        }

        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });



        #endregion
    }


    /// <summary>
    /// 授权
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureAuthorization(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureAuthentication...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        var configuration = context.Services.GetConfiguration();

        #region 最基础认证--自定义Handler 只要Scheme【解决方案不同】 就可以多个Scheme
        ////services.AddAuthentication().AddCookie();-->
        ////services.AddAuthenticationCore();//本质
        //context.Services.AddAuthenticationCore(options => options.AddScheme<CustomHandler>("CustomScheme", "DemoScheme"));
        #endregion


        #region 授权:基于角色||策略授权

        ////定义一个共用的policy
        //var qqEmailPolicy = new AuthorizationPolicyBuilder().AddRequirements(new QQEmailRequirement("qqEmailPolicy")).Build();

        context.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy",
                policyBuilder => policyBuilder
                .RequireRole("admin")//Claim的Role是Admin
                .RequireUserName("parakeet")//Claim的Name是parakeet
                .RequireClaim(ClaimTypes.Email)//必须有某个Cliam
                //.Combine(qqEmailPolicy)
                );//内置

            options.AddPolicy("UserPolicy",
                policyBuilder => policyBuilder.RequireAssertion(ctx =>
                    ctx.User.HasClaim(c => c.Type == ClaimTypes.Role)
                && ctx.User.Claims.First(c => c.Type.Equals(ClaimTypes.Role)).Value == "admin")
           //.Combine(qqEmailPolicy)
           );//自定义
             //policy层面  没有Requirements

            //options.AddPolicy("QQEmail", policyBuilder => policyBuilder.Requirements.Add(new QQEmailRequirement("QQEmail")));
            //options.AddPolicy("DoubleEmail", policyBuilder => policyBuilder.Requirements.Add(new DoubleEmailRequirement()));
        });

        ////父类为AuthorizationHandler<TRequirement>:IAuthorizationHandler，所以可以多次单例注册
        ////可以试试 servicepProvider.GetService<IAuthorizationHandler>()能获取多少
        //context.Services.AddSingleton<IAuthorizationHandler, OtherMailHandler>();
        //context.Services.AddSingleton<IAuthorizationHandler, QQMailHandler>();

        #endregion


        //代码发布后，如通过代理服务器nginx等进行https->http中转,转发原始请求的Header参数配置
        context.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
            //options.KnownProxies.Add(IPAddress.Parse("47.65.1.1"));//添加受信任的代理服务器
        });

    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<NetWebModule>(true);
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureVirtualFileSystem...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、 Configure配置{nameof(AbpVirtualFileSystemOptions)} 虚拟文件系统:{nameof(NetDomainSharedModule)}{nameof(NetDomainModule)}{nameof(NetApplicationContractsModule)}{nameof(NetApplicationModule)}{nameof(NetWebModule)}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                // "YourRootNameSpace" 是项目的根命名空间名字. 如果你的项目的根命名空间名字为空,则无需传递此参数.
                //options.FileSets.AddEmbedded<MyModule>("YourRootNameSpace");
                options.FileSets.ReplaceEmbeddedByPhysical<NetDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Parakeet.Net.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<NetApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Parakeet.Net.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<NetDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Parakeet.Net.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<NetApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Parakeet.Net.Application"));
                options.FileSets.ReplaceEmbeddedByPhysical<NetWebModule>(hostingEnvironment.ContentRootPath);
            });
        }
    }

    /// <summary>
    /// 配置前端导航菜单
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureNavigationServices(IConfiguration configuration)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureNavigationServices...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        Configure<AbpNavigationOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpNavigationOptions)} NetCoreMenuContributor:{nameof(NetMenuContributor)}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.MenuContributors.Add(new NetMenuContributor(configuration));
        });

        Configure<AbpToolbarOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpToolbarOptions)} NetToolbarContributor:{nameof(NetToolbarContributor)}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.Contributors.Add(new NetToolbarContributor());
        });
    }


    /// <summary>
    /// 配置GRPC 集中管理
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureGrpcs(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureGrpcs...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        var logger = context.Services.GetRequiredServiceLazy<ILogger<CustomClientLoggerInterceptor>>();
        context.Services.AddGrpcClient<CustomMath.CustomMathClient>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddGrpcClient配置{nameof(GrpcClientFactoryOptions)} ....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.Address = new Uri("https://localhost:5001");
            options.Interceptors.Add(new CustomClientLoggerInterceptor(logger));
        });
        //.ConfigureChannel(grpcOptions =>
        //{
        //    //HttpClient --443 代替grpc-https://localhost:5001
        //    grpcOptions.HttpClient=new HttpClient(new HttpClientHandler
        //    {
        //        ServerCertificateCustomValidationCallback = (msg,cert,chain,error)=>true//忽略证书
        //    });
        //});

        context.Services
            .AddGrpcClient<Lesson.LessonClient>(options =>
            {
                options.Address = new Uri("https://localhost:5002");
                options.Interceptors.Add(new CustomClientLoggerInterceptor(logger));
            })
            .ConfigureChannel(grpcOptions =>
            {
                var callCredentials = CallCredentials.FromInterceptor(async (ctx, metaData) =>
                {
                    var token = await JWTTokenHelper.GetJWTToken();//todo:即时获取--待加缓存
                    Log.Logger.Information($"token:{token}");
                    metaData.Add("Authorization", $"Bearer {token}");
                });
                //请求都带上token
                grpcOptions.Credentials = ChannelCredentials.Create(new SslCredentials(), callCredentials);
            });
    }


    private static void ConfigureSwaggerServices(ServiceConfigurationContext context)
    {
        var apiSecurityScheme = new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey
        };
        var configuration = context.Services.GetConfiguration();
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"]!,
            new Dictionary<string, string>
            {
                {"parakeet", "Parakeet API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Parakeet API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);

                options.IncludeXmlCommentFiles()
                    .AddSecurityDefinition("bearerAuth", apiSecurityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearerAuth"
                            }
                        },
                        new List<string>()
                    }
                });

                ////api起冲突时默认使用第一个
                //options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
    }
    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("Parakeet");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "Parakeet-Protection-Keys");
        }
    }

    private void ConfigureDistributedLocking(
        ServiceConfigurationContext context,
        IConfiguration configuration)
    {
        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });
    }

    /// <summary>
    /// 忽略AbpAntiForgerys 
    /// </summary>
    private void ConfigureAbpAntiForgerys()
    {
        //忽略接口调用Header的RequestVerificationToken验证
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidateIgnoredHttpMethods.Add("POST");
            options.AutoValidateIgnoredHttpMethods.Add("PUT");
            options.AutoValidateIgnoredHttpMethods.Add("DELETE");
        });
    }

    /// <summary>
    /// 配置文件上传 
    /// </summary>
    private void ConfigureFileUploadOptions()
    {
        // 配置Kestrel自身对上传文件大小的限制
        Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = null; // 设置为null则不限制大小，也可设置为具体的大小（如5GB）

            //单位：字节 From testing, the MaxRequestHeadersTotalSize can be set to 1000M. If set to 1GB, the api will be started failed.
            options.Limits.MaxRequestHeadersTotalSize = 900 * 1024 * 1024;//900MB 按照字节为单位计算的
        });

        // 当托管在IIS/IIS Express下时，也要调整IIS转发到Kestrel的请求大小限制
        Configure<IISServerOptions>(options =>
        {
            // 这里通常不需要额外配置，因为IIS的限制主要在web.config中设置
            options.MaxRequestBodySize = long.MaxValue; // 设置为long.MaxValue会绕过默认限制，允许最大的请求大小
        });

        // 如果需要在应用层控制上传大小，可以使用IFormFile中间件
        Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = long.MaxValue; // 设置允许的最大多部件表单数据大小
        });
    }

    /// <summary>
    /// 配置cookie
    /// </summary>
    /// <param name="context"></param>
    private void ConfigCookie(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigCookie...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        //context.Services.ConfigureExternalCookie(options => { });
        //context.Services.ConfigureApplicationCookie(options => { });
        context.Services.Configure<CookiePolicyOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(CookiePolicyOptions)} 可跨站点cookie Cookie.SameSite:{SameSiteMode.Lax.DisplayName()}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.MinimumSameSitePolicy = SameSiteMode.Lax;//可跨站点cookie
            //options.MinimumSameSitePolicy = SameSiteMode.None;
            //options.OnAppendCookie = cookieContext =>
            //    UserAgentExtention.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            //options.OnDeleteCookie = cookieContext =>
            //    UserAgentExtention.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });
    }

    /// <summary>
    /// 配置Session
    /// </summary>
    /// <param name="context"></param>
    private void ConfigSession(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigSession...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        context.Services.AddSession(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddSession 配置{nameof(SessionOptions)}Session Cookie 可跨站点cookie Chrome新版本对SameSite有要求Cookie.SameSite必须为:{SameSiteMode.Lax.DisplayName()}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.IdleTimeout = TimeSpan.FromHours(2);
            options.Cookie.HttpOnly = true;
            options.Cookie.Name = "ops_cookie";
            options.Cookie.IsEssential = true;
            //您可能只想通过安全连接设置应用程序cookie：
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;//CookieSecurePolicy.None;
            options.Cookie.SameSite = SameSiteMode.Lax;//可跨站点cookie Chrome新版本对SameSite有要求
            //options.Cookie.SameSite = SameSiteMode.None;
        });
    }

    /// <summary>
    /// 配置跨域
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureCors(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureCors...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        var configuration = context.Services.GetConfiguration();
        context.Services.AddCors(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddCors配置跨域{nameof(Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions)}....这里是ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            //跨域默认名称 与中间件
            options.AddPolicy(CommonConsts.AppName, builder =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、CorsOptions.AddPolicy配置跨域AddPolicy Name：{CommonConsts.AppName},CorsOrigins:{configuration["App:CorsOrigins"]}....这里是ConfigureServices中的 {options.GetType().Name}_AddPolicy_{builder.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                builder
                    .WithOrigins(configuration["App:CorsOrigins"]?
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.RemovePostFix("/"))
                        .ToArray() ?? Array.Empty<string>())
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    /// <summary>
    /// 配置Hsts
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureHsts(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureHsts...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        // Configure Hsts
        context.Services.AddHsts(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddHsts 配置{nameof(HstsOptions)}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(180);
        });
    }

    /// <summary>
    /// 配置HttpPolly
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureHttpPolly(ServiceConfigurationContext context)
    {
        // Configure HttpPolly
        Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureHttpPolly....ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

        //如果出站请求为 HTTP GET，则应用 10 秒超时。 其他所有 HTTP 方法应用 30 秒超时
        var timeout = Polly.Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(10));
        var longTimeout = Polly.Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(30));
        var registry = context.Services.AddPolicyRegistry();

        registry.Add("regular", timeout);
        registry.Add("long", longTimeout);
        context.Services.AddHttpClient("regularTimeoutHandler")
            .AddPolicyHandlerFromRegistry("regular");
        context.Services.AddHttpClient("longTimeoutHandler")
            .AddPolicyHandlerFromRegistry("long");

        //context.Services.AddHttpClient("conditionalpolicy")
        //    // Run some code to select a policy based on the request
        //    .AddPolicyHandler(request =>
        //        request.Method == HttpMethod.Get ? timeout : longTimeout);
        context.Services.AddHttpClient("extendedhandlerlifetime")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// 配置文件压缩
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureCompressServices(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureCompressServices...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        context.Services.AddResponseCompression(options =>
        {
            Log.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddResponseCompression 配置{nameof(ResponseCompressionOptions)}...ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        });
    }

    /// <summary>
    /// 语言本地化
    /// </summary>
    private void ConfigureLocalizationServices()
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureLocalizationServices...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        Configure<AbpLocalizationOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AbpLocalizationOptions)} zh-Hans  简体中文 en English....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            options.Resources
                .Get<NetResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );

            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
        });
    }

    #endregion



    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} Start PostConfigureServices ....");
        base.PostConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} End PostConfigureServices ....");
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} Start OnPreApplicationInitialization ....");
        base.OnPreApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} End OnPreApplicationInitialization ....");
    }


    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} Start OnApplicationInitialization ....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        var configuration = context.GetConfiguration();


        #region 向header写入一个相关性Id
        //添加一个中间件,检查并确定给response header返回一个correlationId（相关性Id）
        //request: request没有header,则返回一个Guid.NewGuid().ToString("N")，
        //若有取Headers【X-Correlation-Id】，若非空则返回，否则生成一个，给header设置一个头，则返回此头
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、添加一个中间件,检查并确定给response header返回一个correlationId（相关性Id） request: request没有header,则返回一个Guid.NewGuid().ToString(N) 若有取Headers【X-Correlation-Id】，若非空则返回，否则生成一个，给header设置一个头，则返回此头....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseCorrelationId();

        //这种方式过时了(应在不重载的情况下调用UseExceptionless)，
        //先配置ExceptionlessClient 依赖注入容器IServiceCollection AddExceptionless时设置appKey，url 
        app.UseExceptionless("Gum3CWHNe4uKf7CYGT1CIBEKRx17FreeOywYTIDr");

        #region ForwardedHeader中间件允许 请求中的Header参数通过Nginx等中间代理

        //允许 请求中的Header参数通过Nginx等中间代理：如http代理https的请求，需要把https的请求的header内容传递给nginx代理后的http请求
        //nginx设置中，proxy_set_header X-Forwarded - For  X-Forwarded - Proto
        //server {
        //    listen        80;
        //    server_name example.com *.example.com;
        //    location / {
        //        proxy_pass http://127.0.0.1:5000;
        //        proxy_http_version 1.1;
        //        proxy_set_header Upgrade $http_upgrade;
        //        proxy_set_header Connection keep - alive;
        //        proxy_set_header Host $host;
        //        proxy_cache_bypass $http_upgrade;
        //        proxy_set_header X-Forwarded - For $proxy_add_x_forwarded_for;
        //        proxy_set_header X-Forwarded - Proto $scheme;
        //    }
        //}
        #endregion
        app.UseForwardedHeaders();

        #region 全局异常处理 可在业务系统没有捕获到框架的最外层捕获全局异常
        //应尽早在管道中调用异常处理委托，这样就能捕获在后续管道发生的异常
        //先把异常处理的中间件写在最前面，这样方可捕获稍后调用中发生的任何异常。
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、全局异常处理 可在业务系统没有捕获到框架的最外层捕获全局异常 先把异常处理的中间件写在最前面，这样方可捕获稍后调用中发生的任何异常....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        if (env.IsDevelopment())
        {
            //开发环境的话，打印程序出现异常 的堆栈信息
            app.UseDeveloperExceptionPage();

            //显示PII信息然后就可以在异常信息中看到更加完整的信息了, 方便开发调试...
            IdentityModelEventSource.ShowPII = true;

            #region  通用全局异常处理 自定义错误处理扩展(打印堆栈信息)

            //app.UseExceptionHandler(error =>
            //{
            //    error.Run(async ctx =>
            //    {
            //        ctx.Response.StatusCode = 500;
            //        ctx.Response.ContentType = "text/html";
            //        await ctx.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
            //        await ctx.Response.WriteAsync("Error!<br/><br/>\r\n");
            //        var exceptionHandlerPathFeather = ctx.Features.Get<IExceptionHandlerFeature>();
            //        if (exceptionHandlerPathFeather?.Error is FileNotFoundException)
            //        {
            //            await ctx.Response.WriteAsync("File error thrown!<br/><br/>\r\n");
            //        }

            //        await ctx.Response.WriteAsync("<a href=\"/\">Home</a><br/><br/>\r\n");
            //        await ctx.Response.WriteAsync("</body></html>\r\n");
            //    });
            //});
            #endregion
        }
        else
        {
            //其它环境指定错误处理动作(返回固定提示错误页)，不让客户直接看到异常堆栈信息
            app.UseErrorPage();
        }

        #region 使用HTTP错误代码页

        //UseStatusCodePagesWithReExecute 拦截 404 状态代码并重新执行将其指向 URL 的管道即我们的(/Error/404)中
        //整个请求流经 Http 管道并由 MVC 中间件处理，该中间件返回 NotFound 视图 HTML 的状态代码 依然是 200
        //当响应流出到客户端时，它会通过 UseStatusCodePagesWithReExecute 中间件，该中间件会使用 HTML 响应，将 200 状态代码替换为原始的 404 状态代码
        //因为它只是重新执行管道而不发出重定向请求，所以我们还在地址栏中保留原始请求地址而不会变为/Home/Error/404

        //app.UseStatusCodePages();
        //app.UseStatusCodePages(async context =>
        //    {
        //        context.HttpContext.Response.ContentType = "text/plain";
        //        await context.HttpContext.Response.WriteAsync(
        //            "Status code page, status code: " +
        //            context.HttpContext.Response.StatusCode);
        //    });
        #endregion
        app.UseStatusCodePagesWithReExecute("/Home/Error{0}");//占位符{0}在这代表Response.StatusCode 

        #region ABP框架语言本地化中间件
        //获取RequestLocalizationOptions，并执行LocalizationOptions的委托  IAbpRequestLocalizationOptionsProvider.InitLocalizationOptions
        //创建RequestLocalizationMiddleware，导入LocalizationOptions和_loggerFactory
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、创建RequestLocalizationMiddleware，导入LocalizationOptions和_loggerFactory....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseAbpRequestLocalization();

        #region HTTP 严格传输安全协议 (HSTS) 中间件
        //HTTP 严格传输安全协议 (HSTS) 中间件 (UseHsts) 添加 Strict-Transport-Security 标头
        #endregion
        //app.UseHsts();

        #region 将 HTTP 请求重定向到 HTTPS  开发环境一般不用
        //将 HTTP 请求重定向到 HTTPS  开发环境一般不用
        #endregion
        //app.UseHttpsRedirection();

        #region Cookie 策略中间件
        //Cookie 策略中间件 (UseCookiePolicy) 使应用符合欧盟一般数据保护条例 (GDPR) 规定
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、策略中间件 (UseCookiePolicy) 使应用符合欧盟一般数据保护条例 (GDPR) 规定....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseCookiePolicy();

        #region 会话中间件
        //会话中间件 (UseSession) 建立和维护会话状态(内部直接设置cookie  key value相关 设置，更新有效期的方式达到存储seesion会话)。 
        //如果应用使用会话状态，请在 Cookie 策略中间件之后和 MVC 中间件之前调用会话中间件

        //Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、会话中间件 (UseSession) 建立和维护会话状态(内部直接设置cookie  key value相关 设置，更新有效期的方式达到存储seesion会话)。 如果应用使用会话状态，请在 Cookie 策略中间件之后和 MVC 中间件之前调用会话中间件....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseSession();

        ////自定义Use中间件逻辑
        //app.Use(next =>
        //{
        //    Log.Logger.Warning($"{{0}}", $"{CacheKeys.LogCount++}、【MVC流程日志：3、Use作用域内返回RequestDelegate前的代码,属于Build中间件组装流程时会被reverse反序执行一次【注意Build方法在startup调用configure之后，所以此条日志显示在Startup Configure方法日志结束之后】 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

        //    return new RequestDelegate(async ctx =>
        //    {
        //        //ctx.Response.OnStarting(async () =>
        //        //{
        //        //    Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、ctx.Response.OnStarting ....");
        //        //});

        //        Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、【MVC流程日志：4、请求进入中间件执行RequestDelegate委托中代码，下一步将进入---EndpointRoutingMiddleware中间件内部开始路由匹配】线程Id：【{Thread.CurrentThread.ManagedThreadId}】.....");
        //        await next.Invoke(ctx);//执行下一个中间逻辑 (CustomLogMiddleware)
        //        Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、【MVC流程日志：5、请求被中间件EndpointRoutingMiddleware(next.Invoke)处理后，回到当前RequestDelegate委托闭环】 线程Id：【{Thread.CurrentThread.ManagedThreadId}】....");
        //    });
        //});
        ////插入一个自定义中间件，invoke方法内部含日志输出，可以清晰的看到中间件委托处理请求执行的先后顺序
        ////app.UseCustomLog();
        /// 
        #region 标准Middleware
        ////玩法1---Use传递---Add就无操作---IOptions<BrowserFilterOptions> options就是Use指定传递的 
        ////玩法2---Use不传递---靠Add实现---IOptions<BrowserFilterOptions> options就是IOC生成的
        ////玩法3---都传递---Use和Add都传递--Add为准1  Use为准2   叠加3---结果是2，以Use为准，原因是对象只会是UseMiddleware传递的值，就不会再找IOC了---但是合理吗？----可以升级注入IConfigureOptions<BrowserFilterOptions>，然后叠加生效

        //app.UseBrowserFilter(new BrowserFilterOptions
        //{
        //    EnableIE = false,
        //    EnableFirefox = false
        //});//玩法1

        //app.UseBrowserFilter();//玩法2

        //app.UseBrowserFilter(option =>
        //{
        //    option.EnableEdge = true;
        //});//玩法3

        #endregion

        #region 用于路由请求的路由中间件

        //请求进入时：根据(app.UseEndPoint组装中间件时写入的路由规则)完成路由匹配，找到EndPoint(请求时才会完成路由匹配找EndPoint)
        //找EndPoint匹配规则在app.UseEndPoint(组装中间件逻辑里，所以规则会比找EndPoint(app.UseRouting内部委托)先执行)
        //UseRouting、UseEndPonts方法是组装中间件时执行--程序启动时 组装中间件执行的顺序
        //1、UseRouting早于-->2、UseEndPonts(组装中间件时完成路由规则)
        //3、UseRouting里面的委托:EndPointRoutingMiddleware是请求发生时才执行
        //4、请求发生时EndPointRoutingMiddleware调用next.invoke()前 找EndPoint【找endpoint具从partManger加载所有程序集 controller action 终结点去找】
        //5、会根据2、UseEndPoint组装中间件时设置的路由规则找到真正的EndPoint,
        //6、UseEndPonts里面的委托:EndPointMiddleware也是请求发生时才执行，
        //7、请求发生时上一步EndPointEndPointRoutingMiddleware找到真正的EndPoint,
        //8、然后调用next.invoke交给EndPointMiddleware执行EndPoint.执行EndPoint之后，
        //9、再回到EndPointEndPointRoutingMiddleware中间件请求后逻辑(中间件走完终点逐件往回走思想)。

        //执行顺序：组装中间件>(早于)请求调用时  两个中间件的组装早于-->两个中间件请求发生时
        //组装中间件时：1、UseRouting(方法内部委托前没干什么事儿)-->2、UseEndPonts(方法委托前配置了路由规则)-->
        //3、请求调用时：EndPointRoutingMiddleware委托next.invoke前(根据已经配置好的路由规则完成路由匹配，找到EndPoint)-->
        //4、【EndPointMiddleware 执行终结点委托前-->5、EndPointMiddleware 执行终结点委托后】-->
        //6、EndPointRoutingMiddleware委托next.invoke后
        //总结：UseRouting>UseEndPonts>EndPointRoutingMiddleware前>EndPointMiddleware执行(前,中,后)>EndPointRoutingMiddleware后
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、请求进入时：app.UseRouting根据(app.UseEndPoint组装中间件时写入的路由规则)完成路由匹配，找到EndPoint(请求时才会完成路由匹配找EndPoint)....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseRouting();

        #region 跨域策略
        //跨域的策略名称与configure中跨域名称要一致，跨域中间件必须要在UseRouting 之后
        //在UseAuthentication UseAuthorization UseResponseCaching之前
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、跨域的策略名称与configure中跨域名称要一致，跨域中间件必须要在UseRouting 之后 UseAuthentication UseAuthorization UseResponseCaching之前....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseCors(CommonConsts.AppName);

        #region 身份验证 解析信息，读取token 创建HttpContext.User 给context.User赋值
        //身份验证中间件 (UseAuthentication) 尝试对用户进行身份验证，然后才会允许用户访问安全资源
        //鉴权 解析信息，读取token AuthAppBuilderExtensions.UseAuthentication->指定使用中间件AuthenticationMiddleware->
        //构建认证管道 --context.User = authenticateResult.Principal;
        //创建HttpContext.User，Abp Vnext提供的ICurrentUser就是来源HttpContext.User?Thead.ClaimsPrincipals
        //鉴权 解析信息，读取token 检测有没有登录，登录的是谁 完成用户信息获取(解析赋值给context.User)

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、身份验证中间件 (UseAuthentication)鉴权 解析信息，读取token 检测有没有登录，登录的是谁 完成用户信息获取(解析完赋值给context.User AbpVnext提供的ICurrentUser就是来源HttpContext.User?Thead.ClaimsPrincipals)....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseAuthentication();

        #region //使用jwt格式的token 中间件
        //使用jwt格式的token 中间件

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、使用jwt格式的token 中间件....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        //app.UseJwtTokenMiddleware();

        #region 高速缓存和压缩排序
        //高速缓存和压缩排序是特定于场景的，存在多个有效的排序
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、高速缓存和压缩排序是特定于场景的，存在多个有效的排序....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseResponseCaching();//使用ResponseCaching缓存

        #region 压缩
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、压缩....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        app.UseResponseCompression();//压缩
        #endregion

        #region 静态文件中间件
        //静态文件中间件如果放caching与compress之后 表示以允许缓存压缩的静态文件
        //等同于 使用 wwwroot 文件夹中的物理(静态)(js, css, image ...)文件
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、静态文件 等同于 使用 wwwroot 文件夹中的物理(静态)(js, css, image ...)文件 此中间件如果放caching与compress之后 表示以允许缓存压缩的静态文件....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseStaticFiles(new StaticFileOptions//提供静态文件StaticFileOptions.FileProvider
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
            RequestPath = "/StaticFiles",
            OnPrepareResponse = ctx =>
            {
                // using Microsoft.AspNetCore.Http;
                ctx.Context.Response.Headers.Append(
                    "Cache-Control", $"public, max-age=604800");
            }
        });

        #region 启用静态文件url地址浏览
        //启用静态文件url地址浏览
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、启用静态文件url地址浏览....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseDirectoryBrowser(new DirectoryBrowserOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "images")),
            RequestPath = "/MyImages"
        });

        #region 多租户 放在NetMultiTenancyModule里面

        ////多租户 放在NetMultiTenancyModule模块里面
        //Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、多租户....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

        //if (CommonConsts.MultiTenancyEnabled)
        //{
        //    app.UseMultiTenancy();
        //}
        #endregion


        #region 将IdentityServer中间件添加到HTTP管道 (允许IdentityServer开始拦截路由并处理请求)
        //将IdentityServer中间件添加到HTTP管道 (允许IdentityServer开始拦截路由并处理请求)
        //配置文件中如果配置了数组，那么这个中间件会报错？这个bug
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、将IdentityServer中间件添加到HTTP管道 (允许IdentityServer开始拦截路由并处理请求)....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        //app.UseIdentityServer();

        ////自定义安全策略及自定义中间件
        //app.UseSecurePolicy();
        //app.UseInheritedMiddleware();

        app.UseDynamicClaims();

        #region 用于授权用户访问安全资源的授权中间件

        //Claim：信息
        //    ClaimsIdentity：身份
        //    ClaimsPrincipal：一个人可以有多个身份
        //    AuthenticationTicket：用户票据
        //    加密一下---写入cookie
        //用于授权用户访问安全资源的授权中间件 //授权，检测用户的具体权限
        //在Startup类中的Configure方法通过添加UseAuthentication注册认证中间件
        //通过AuthenticationSchemeProvider获取正确的Scheme，
        //在AuthenticationService中通过Scheme和AuthenticationHandlerProvider获取正确的AuthenticationHandler，
        //最后通过对应的AuthenticationHandler的AuthenticateAsync方法进行认证流程
        //(主要是从Request.Headers里面获取Authorization的Bearer出来解析，再在AddJwtBearer中传入的委托参数JwtBearerOptions的TokenValidationParameters属性作为依据进行对比来进行认证是否通过与否)
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、授权，检测用户的具体权限....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、在Startup类中的Configure方法通过添加UseAuthentication注册认证中间件");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、通过AuthenticationSchemeProvider获取正确的Scheme");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、在AuthenticationService中通过Scheme和AuthenticationHandlerProvider获取正确的AuthenticationHandler");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、最后通过对应的AuthenticationHandler的AuthenticateAsync方法进行认证流程");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、(主要是从Request.Headers里面获取Authorization的Bearer出来解析，再在AddJwtBearer中传入的委托参数JwtBearerOptions的TokenValidationParameters属性作为依据进行对比来进行认证是否通过与否)");
        #endregion
        app.UseAuthorization();

        #region swagger/swagger-ui中间件 在线的 API 文档生成与测试工具
        //swagger/swagger-ui中间件
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、swagger/swagger-ui中间件....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseSwagger(option =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、UseSwagger {nameof(SwaggerOptions)}....");
            //option.RouteTemplate = "api-doc/{documentName}/swagger.json";//配置后，你的最终访问路径就就是 /api-doc/index.html
        });
        app.UseSwaggerUI(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、UseSwaggerUI {nameof(SwaggerUIOptions)}....");
            EnumContext.Instance.GetEnumTypeItemKeyNameDescriptions(new InputNameDto { Name = nameof(VersionType) })
                .ForEach(v =>
                {
                    //configuration["App:SwaggerEndpoint"]="/swagger/v1/swagger.json";
                    options.SwaggerEndpoint(string.Format(context.GetConfiguration()["App:SwaggerEndpoint"] ?? @$"/swagger/{v.ItemDescription}/swagger.json", v.ItemDescription), $"小鹦鹉工作室 NetCore API {v.ItemDescription}");
                });
            //typeof(VersionType).GetEnumNames().ToList().ForEach(v =>
            //{
            //    //configuration["App:SwaggerEndpoint"]="/swagger/v1/swagger.json";
            //    options.SwaggerEndpoint(string.Format(configuration["App:SwaggerEndpoint"] ?? @$"/swagger/{v}/swagger.json", v), $"小鹦鹉工作室 NetCore API {v}");
            //});
            //options.InjectJavascript("/swagger/ui/zh_CN.js"); // 加载中文包 无效
            //options.ShowExtensions();
            //options.DocExpansion(DocExpansion.None);
            //options.RoutePrefix = "api-doc";
        });
        #region ABP审计日志中间件

        //判断请求是否需要写审计log  =》可配置，不允许匿名访问而且用户没有认证 不审计，
        //get请求不写审记日志，需要的话，创建一个IAuditLogSaveHandle
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、判断请求是否需要写审计log ->可配置，不允许匿名访问而且用户没有认证 不审计 get请求不写审记日志，如需要的话，需创建一个IAuditLogSaveHandle....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseAuditing();

        app.UseAbpSerilogEnrichers();

        #region 路由规则，执行终结点中间件,新版本已替换为 UseConfiguredEndpoints

        ////首先经过路由规则的匹配，找到最符合条件的的IRouter,然后调用IRouter.RouteAsync来设置RouteContext.Handler，
        ////最后把请求交给RouteContext.Handler来处理。
        ////在MVC中提供了两个IRouter实现，分别如下：MvcAttributeRouteHandler,MvcRouteHandler
        ////设置路由规则
        //app.UseEndpoints(endpoints =>
        //{
        //    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        //    endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
        //    endpoints.MapRazorPages();
        //    //additionalConfigurationAction?.Invoke(endpoints);
        //});
        #endregion

        //app.UseMvcWithDefaultRouteAndArea();//已过时 使用UseConfiguredEndpoints替代
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、首先经过路由规则的匹配，找到最符合条件的的IRouter,然后调用IRouter.RouteAsync来设置RouteContext.Handler,把请求交给RouteContext.Handler来处理 在MVC中提供了两个IRouter实现，分别如下：MvcAttributeRouteHandler,MvcRouteHandler....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        app.UseConfiguredEndpoints(x =>
        {
            Log.Logger.Warning($"{{0}}", $"{CacheKeys.LogCount++}、【MVC流程日志：2、UseConfiguredEndpoints_{nameof(IEndpointConventionBuilder)} 此委托会直接执行，属于组装中间件流程,目的是在组装中间件流程过程中先将路由规则放入，根据ConfigreService已经加载的终结点程序集dll，完成所有EndPoint的转化，准备EndPoint数据源(内部UseEndpoints-->MapControllerRoute)，这样在Http请求时，UseRouting 请求进入---EndpointRoutingMiddleware从EndPoint数据源中才能完成Endpoint的查找 再然后，请求到EndpointMiddleware负责执行Endpoint(处理请求)】线程Id：【{Thread.CurrentThread.ManagedThreadId}】....】【注意：此日志的打印顺序为中间件的最早，因为此中间件为最后一个，反序就是第一个，再加上内部逻辑(路由规则)被直接invoke】");
        });


        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、SeedData默认数据生成 第一次运行即可....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        //ExtraSeedData(context);//ExtraSeedData默认数据生成 第一次运行即可 一般不需要在这里执行

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} End OnApplicationInitialization ....线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

    }


    /// <summary>
    /// 初始化数据到数据库
    /// </summary>
    /// <param name="context"></param>
    private void ExtraSeedData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(NetWebModule)}  SeedData初始化数据执行 ....线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

            using var scope = context.ServiceProvider.CreateScope();
            await scope.ServiceProvider
                .GetRequiredService<IDataSeeder>()
                .SeedAsync();
        });
    }


    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} Start OnPostApplicationInitialization ....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        base.OnPostApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} End OnPostApplicationInitialization ....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        Log.Warning($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"..............................................OnApplicationShutdown 反序最先执行............................................");
        Log.Warning($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} Start OnApplicationShutdown ....线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        base.OnApplicationShutdown(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetWebModule)} End OnApplicationShutdown ....线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
    }


}
