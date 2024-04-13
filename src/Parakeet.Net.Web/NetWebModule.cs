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
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} Start ConfigureServices ....");
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        ConfigureBundles();//mvcǰ������ѹ������Դbundles
        ConfigureCache(context);//���û����Redis ��domainģ��������
        ConfigureFilters(context);//mvc��ȫ��filterͳһ���� AddMvc
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureDistributedLocking(context, configuration);
        ConfigureUrls(configuration);
        ConfigureAuthentication(context);//���ü�Ȩ
        ConfigureAuthorization(context);//������Ȩ
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices(configuration);
        ConfigureGrpcs(context);
        //ConfigureMultiTenancy();//����NetMultiTenancyModule����
        ConfigureSwaggerServices(context);//����swagger

        ConfigCookie(context);
        ConfigSession(context);
        ConfigureCors(context); //���ÿ���
        ConfigureHsts(context);
        ConfigureHttpPolly(context);
        ConfigureCompressServices(context);
        ConfigureLocalizationServices();
        ConfigureAbpAntiForgerys();
        ConfigureFileUploadOptions();

        if (!hostingEnvironment.IsDevelopment())
        {
            //https ��Ҫ����443�˿�
            //context.Services.AddHttpsRedirection(option => option.HttpsPort = 443);
            //ConfigurHsts(context);
        }
        //context.Services.AddControllersWithViews()//�����ռ�dll-������--action--PartManager
        //    .AddNewtonsoftJson();

        Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}��AddDirectoryBrowser:��������ָ����Ŀ¼��� AddRazorPages:֧��Razor ��Pages viewģʽ....ConfigureServices�е�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        context.Services.AddDirectoryBrowser();//����ָ����Ŀ¼���


        //Configure<AbpAuditingOptions>(options =>
        //{
        //    //options.IsEnabledForGetRequests = true;
        //    options.ApplicationName = "parakeet";
        //});
        //Configure<AbpBackgroundJobOptions>(options =>
        //{
        //    options.IsJobExecutionEnabled = false;
        //});

        //AddControllersWithViews/AddRazorPages/AddControllersCore-->AddMvcCore/AddAuthorization(��������Ȩ�ķ���ע��)
        //AddAuthorization-->AddAuthorizationCore��AddAuthorizationPolicyEvaluator
        //AddAuthorizationCore-->ע��IAuthorizationService��IAuthorizationHandler....
        context.Services.AddRazorPages(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddRazorPages (����ί��)����{nameof(RazorPagesOptions)},��MVC������־��1��ConfigreServices��������AddRazorPages����ʼMVC��Դ�����á�AddRazorPages-->AddMvcCore/-->ApplicationPartManager(һ���Լ��ص�ǰ�������������ս��(Action/Controller)���ڳ���dll��׼���ÿ������ṩ�������MVC��������ʼ��������Controller-Action,Ϊ�м��UseRouting ƥ��·����׼��)...ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");

        })//֧��Razor ��Pages viewģʽ
          //.AddNewtonsoftJson(options =>
          //{
          //    Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddNewtonsoftJson {nameof(MvcNewtonsoftJsonOptions)} ���� �������ݸ�ǰ�����л�ʱkeyΪ�շ���ʽ,ʱ�� ���ڸ�ʽ �ն���������ѭ�����õ�����...ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
          //    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); //���л�ʱkeyΪ�շ���ʽ
          //    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
          //    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
          //    options.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
          //    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//����ѭ������
          //})
            .AddJsonOptions(options =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddJsonOptions {nameof(JsonOptions)} ���� �������ݸ�ǰ�����л�ʱkeyΪ�շ���ʽ,ʱ�� ���ڸ�ʽ �ն���������ѭ�����õ�����...ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
                //options.JsonSerializerOptions.MaxDepth = 2;
                //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;//���л����շ���������
                //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            })
            .AddRazorRuntimeCompilation();//�޸�cshtml�����Զ�����
                                          //context.Services.AddControllers(options =>
                                          //    {
                                          //        var jsonInputFormatter = options.InputFormatters
                                          //            .OfType<Microsoft.AspNetCore.Mvc.Formatters.NewtonsoftJsonInputFormatter>()
                                          //            .First();
                                          //        jsonInputFormatter.SupportedMediaTypes.Add("multipart/form-data; boundary=*");
                                          //    }
                                          //);

        //AddControllersWithViews-->AddControllersCore->AddMvcCore/AddAuthorization(��������Ȩ�ķ���ע��)
        //context.Services.AddControllersWithViews(
        //    options =>
        //    {
        //        options.Filters.Add<CustomExceptionFilterAttribute>();//ȫ��ע��
        //        options.Filters.Add<CustomGlobalActionFilterAttribute>();
        //    })
        //    .AddNewtonsoftJson(options =>
        //        {
        //            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); //���л�ʱkeyΪ�շ���ʽ
        //            options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
        //            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        //            options.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
        //            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//����ѭ������
        //        })
        //    .AddRazorRuntimeCompilation();//�޸�cshtml�����Զ�����


        #region ������֤�Զ����mvc������

        ////����ModelStateĬ����Ϊ ����ModelState�����Ĭ����Ϊ
        //context.Services.Configure<ApiBehaviorOptions>(options =>
        //{
        //    //��һ�ַ���������Frameworkʱ���ķ����Ҫ������ָ������ԭ�е�ģ����֤
        //    //options.SuppressModelStateInvalidFilter = true;
        //    //�ٷ���������������Coreʱ���ķ��ֻ�踴дInvalidModelStateResponseFactoryί�м���
        //    options.InvalidModelStateResponseFactory = (context) =>
        //    {
        //        var error = context.ModelState.GetValidationSummary();
        //        return new JsonResult(Result.FromError($"������֤��ͨ����{error.ToString()}", ResultCode.InvalidParams));
        //    };
        //});

        ////�ϰ汾��Ҫ�˺������¼���õ�ȫ�ֹ����� ʹ��identityserver������Ҫ
        //context.Services.AddMvc(o =>
        //{
        //    //o.Filters.Add<CustomExceptionFilterAttribute>();//�쳣����ȫ��filter
        //    //o.Filters.Add(typeof(CustomGlobalActionFilterAttribute));//ȫ��ע��filter
        //    o.Filters.Add(typeof(CustomAuthorityActionFilterAttribute)); //Mvc5 ȫ��Ȩ����֤ (��Ŀ��Ǩ)
        //});

        #endregion

        //ConfigureDbContext(context);//���Ե������ñ�Module��dbcontext�������ַ���


        //ConfigurePlugins(context);


        #region HttpClient����Policy����
        ConfigureHttpPolly(context);
        #endregion

        #region ��׼�м��ע��option��·

        ////context.Services.AddBrowserFilter();//��optionע��
        //context.Services.AddBrowserFilter(option =>
        //{
        //    //option.EnableIE = false;
        //    //option.EnableEdge = false;
        //    //option.EnableChorme = false;
        //    option.EnableFirefox = true;
        //});//optionί��1

        //context.Services.AddBrowserFilter(option =>
        //{
        //    option.EnableIE = true;
        //    option.EnableEdge = true;
        //    option.EnableChorme = true;
        //    option.EnableFirefox = true;
        //});//optionί��2

        //context.Services.AddMiddlewareFactory();
        //context.Services.AddInheritedMiddleware();

        #endregion

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} End ConfigureServices ....");
        //����һ��ȫ�ֵĽ���Provider�ṩ����ʹ�õĵط���������ע�� ������

    }

    #region ����������

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��Configure����{nameof(AbpBundlingOptions)} ScriptBundles:{LeptonXLiteThemeBundles.Styles.Global},StyleBundles:{LeptonXLiteThemeBundles.Scripts.Global}....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
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
                        "/js/devextreme/vectormap-data/world.js",//��;ȥ"use strict"
                        "/js/devextreme/aspnet/dx.aspnet.mvc.js",//��;ȥ"use strict"
                        "/js/devextreme/aspnet/dx.aspnet.data.js",//��;ȥ"use strict"
                        "/js/devextreme/localization/dx.messages.zh.js",//ȥ"use strict"
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
    /// ���÷ֲ�ʽ���棬Redis���������ַ���,ǰ׺��
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureCache(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureCache...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        //var configuration = context.Services.GetConfiguration();
        //var hostingEnvironment = context.Services.GetHostingEnvironment();

        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "net:";
        });

        #region Redis �����ѷ��������


        #region Caching.CSRedis ��StackExchangeRedis ��������һ�� 

        //Log.Logger.Information($"����Caching.CSRedis(��StackExchangeRedis ��������һ��)....");
        //Microsoft.Extensions.Caching.Redis--CSRedisCore   
        //Microsoft.AspNetCore.DataProtection.StackExchangeRedis  ʹ�������������



        //�����ѷ����������ִ��,���������ģ���ִ��
        //context.Services.AddStackExchangeRedisCache();
        //context.Services.AddCachePool();//���� context.Services.AddCsRedisCache();
        //context.Services.ConfigAbpDistributeCacheOptions();

        #endregion


        #region �ϰ汾

        //Configuration.Caching.UseRedis(options =>
        //{
        //    options.ConnectionString = configuration["Abp:RedisCache:ConnectionStrings"];
        //    options.DatabaseId = Convert.ToInt32(configuration["Abp:RedisCache:DatabaseId"]);
        //});
        //Configuration.Caching.ConfigureAll(cache =>
        //{
        //    //����Ĭ�Ϲ���ʱ������Ϊ2h
        //    cache.DefaultSlidingExpireTime = TimeSpan.FromHours(2);
        //});
        ////�û�����ǩ����֤��Ȼ���5����
        //Configuration.Caching.Configure(CustomerConsts.PersonalCaches, cache =>
        //{
        //    cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(5);
        //});

        #endregion

        #endregion

        context.Services.AddResponseCaching(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddResponseCaching����ί������{nameof(ResponseCachingOptions)} (�ͻ���ͬһ����Ӧ�𻺴�,�ݲ���Ч)....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
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
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureFilters...ConfigureServices�е�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        //mvcȫ�ֹ�����
        context.Services.AddMvc(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddMvc����{nameof(MvcOptions)} mvcȫ�ֹ�����....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��  ");
            //options.Filters.Add(typeof(AreaAttribute));
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}������{nameof(AppUrlOptions)}....ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        Configure<AppUrlOptions>(options =>
        {
            Log.Error($"{{0}}", $"{CacheKeys.LogCount++}��Configure����{nameof(AppUrlOptions)}  RootUrl={configuration["App:SelfUrl"]}....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureMultiTenancy()
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureMultiTenancy...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        Configure<AbpMultiTenancyOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��Configure����{nameof(AbpMultiTenancyOptions)} �Ƿ����ö��⻧����:{CommonConsts.MultiTenancyEnabled}....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
            options.IsEnabled = CommonConsts.MultiTenancyEnabled;
        });
    }


    /// <summary>
    /// Authentication��֤
    /// ͨ�����AddAuthenticationע����AuthenticationService, AuthenticationHandlerProvider,AuthenticationSchemeProvider
    /// 
    /// ����Ȩ��ʹ��IdentityServer��IdentityServer��������ַ��
    /// ע�ᵽ ApiName��IdentityServer������
    ///
    ///
    /// jwt   https://jwt.io/   debugger
    /// ͨ�������㷨���������� HS256(�Գ�) RS256(�ǶԳ�)
    /// ��ʵ�������׶���
    /// 1����¼д��ƾ֤
    /// 2����Ȩ�����ҳ��û�
    /// 3����Ȩ�����ж�Ȩ��
    /// 4���˳���������ƾ֤
    /// 
    /// ��Ȩ��Ȩ���棬��ͨ��AuthenticationHttpContextExtensions��5��������
    /// SignInAsync, AuthenticateAsync,ForbidAsync, ChallengeAsync, SignOutAsync��AuthenticationService��5��������
    /// -��ʵ���ջ���Ҫдcookie/session/��Ϣ
    /// 
    ///     ���ǵ��õ�IAuthenticationService��ConfigureServiceע��
    ///     AuthenticationCoreServiceCollectionExtensions.AddAuthenticationCore
    ///         IAuthenticationService
    ///         IAuthenticationHandlerProvider
    ///         IAuthenticationSchemeProvider
    /// 
    /// 5������Ĭ�Ͼ���AuthenticationService����handler��ɴ���
    ///     ������
    /// 
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureAuthentication...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        var configuration = context.Services.GetConfiguration();

        #region �ͻ���ģʽ

        //context.Services.AddIdentityServer() //���崦�����
        //    .AddDeveloperSigningCredential() //Ĭ�ϵĿ�����֤��--��ʱ֤��--��������Ϊ�˱�֤token��ʧЧ��֤���ǲ����
        //    .AddInMemoryClients(ClientInitConfig.GetClients())
        //    .AddInMemoryApiResources(ClientInitConfig.GetApiResources());

        #endregion

        #region �ͻ��� ����ģʽ ע�� ���Ҫ��������ʹ��token �Ͳ�Ҫ��AddCookie()�����AddCookie ������Header����ͱ��봫��cookie��

        ////ͨ�����AddAuthenticationע����AuthenticationService, AuthenticationHandlerProvider,AuthenticationSchemeProvider����������
        ////ͨ��AddAuthentication���ص�AuthenticationBuilderͨ��AddJwtBearer������AddCookie��
        ////��ָ��Scheme���ͺ���Ҫ��֤�Ĳ���,��ָ����AuthenticationHandler
        ////Bearer (scheme ��������)
        ////IdentityServerAuthenticationDefaults.AuthenticationScheme CookieAuthenticationDefaults.AuthenticationScheme
        //context.Services.AddAuthentication(options =>
        //{
        //    //ע��Scheme��Bearer/Cookie
        //    //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //    //options.DefaultChallengeScheme = "oidc";//OpenIdConnectDefaults.AuthenticationScheme
        //})
        //    //.AddJwtBearer(options =>//AddIdentityServerAuthentication���滻AddJwtBearer
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
        //    .AddIdentityServerAuthentication(options =>//����У��������У��
        //    {
        //        Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddAuthentication AddIdentityServerAuthentication����{nameof(IdentityServerAuthenticationOptions)} idserver4��������Ȩ��ַ:{configuration["AuthServer:Authority"]}Api:{configuration["AuthServer:ApiName"] ?? "parakeet"}....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        //        //idserver4��Ȩ��ַ(Ӧ���ǻ�ȡ��Կ,��������api��Դ������Ҫע�ᵽids4)
        //        //�������������api��Դ����(parakeet) ע�ᵽids4...(�ͻ�������idsʱ��ids����token�оʹ������(parakeet������)��Ϣ)
        //        options.Authority = configuration["AuthServer:Authority"];
        //        options.RequireHttpsMetadata = false;
        //        options.ApiName = configuration["AuthServer:ApiName"] ?? "parakeet";
        //        options.JwtBackChannelHandler = new HttpClientHandler
        //        {
        //            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        //        };
        //    });

        #endregion

        #region �������֤--�Զ���Handler ֻҪScheme�����������ͬ�� �Ϳ��Զ��Scheme
        //////services.AddAuthentication().AddCookie();-->
        //////AddAuthentication()-->services.AddAuthenticationCore();//����
        //context.Services.AddAuthenticationCore(options => options.AddScheme<CustomHandler>("CustomScheme", "DemoScheme"));
        #endregion

        #region cookie��ʽ��֤ ��ʼ����¼��ַ����Ȩ��ʱ��ת��ַ

        ////cookie��ʽ��֤ ��ʼ����¼��ַ����Ȩ��ʱ��ת��ַ
        ////context.Services.AddAuthentication(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme);
        //context.Services.AddAuthentication(options =>
        //    {
        //        //Ĭ�Ͻ����������
        //        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;//Scheme������
        //        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //        options.DefaultChallengeScheme = "Account/Login";
        //    })
        //    .AddCookie(authenticationScheme: CookieAuthenticationDefaults.AuthenticationScheme, configureOptions: options =>
        //    {
        //        //�������ʹ��context.Services.BuildServiceProvider()
        //        //����Ϊ�����ڲ�ί�У��Ѿ���Build֮��Ż�ִ�еĴ�����
        //        Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddCookie����{nameof(NetCoreWebModule)} AddAuthentication....AddCookie�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        //        options.SessionStore = context.Services.BuildServiceProvider().GetRequiredService<ITicketStore>();
        //        options.LoginPath = new PathString("/Account/Login");//��¼��ַ
        //        options.AccessDeniedPath = new PathString("/Account/Login");//δ��Ȩ��ת��ַ

        //        options.Events = new CookieAuthenticationEvents()
        //        {
        //            //��չ�¼�
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

        //AddCookie ����ITicketStore����ע������
        //context.Services.AddScoped<ITicketStore, MemoryCacheTicketStore>();
        ////context.Services.AddAuthorization(options =>
        ////{
        ////    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        ////        .RequireAuthenticatedUser()
        ////        .Build();
        ////});
        #endregion

        #region ���˲���FallbackPolicy Ĭ��Ϊ null

        //// δ�ṩIAuthorizeDataʱCombineAsync(IAuthorizationPolicyProvider, IEnumerable<IAuthorizeData>)ʹ�õĻ�����Ȩ���ԡ�
        //// ��ˣ������Դû��IAuthorizeDataʵ����AuthorizationMiddleware ��ʹ�û��˲��ԡ�
        //// �����Դ�����κ�IAuthorizeData�� ���������Ƕ����ǻ��˲��ԡ�Ĭ������£����˲���Ϊ null��
        //// �������Ĺܵ����� AuthorizationMiddleware������ͨ����������κ�Ӱ�졣
        //// Ĭ��IAuthorizationService�����κη�ʽʹ������
        //context.Services.AddAuthorization(options =>
        //{
        //    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        //        .RequireAuthenticatedUser()
        //        .Build();
        //});
        #endregion


        #region AddAuthentication ֻ��������һ�β���������ʽ���ã���֤��ͬһ��builder���������������(�����������ռ�һ��)

        //ע�� ������AddAuthentication �ͻᴴ�����builder��ɳ�ͻ�������ռ䲻һ��

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
    /// ��Ȩ
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureAuthorization(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureAuthentication...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        var configuration = context.Services.GetConfiguration();

        #region �������֤--�Զ���Handler ֻҪScheme�����������ͬ�� �Ϳ��Զ��Scheme
        ////services.AddAuthentication().AddCookie();-->
        ////services.AddAuthenticationCore();//����
        //context.Services.AddAuthenticationCore(options => options.AddScheme<CustomHandler>("CustomScheme", "DemoScheme"));
        #endregion


        #region ��Ȩ:���ڽ�ɫ||������Ȩ

        ////����һ�����õ�policy
        //var qqEmailPolicy = new AuthorizationPolicyBuilder().AddRequirements(new QQEmailRequirement("qqEmailPolicy")).Build();

        context.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy",
                policyBuilder => policyBuilder
                .RequireRole("admin")//Claim��Role��Admin
                .RequireUserName("parakeet")//Claim��Name��parakeet
                .RequireClaim(ClaimTypes.Email)//������ĳ��Cliam
                //.Combine(qqEmailPolicy)
                );//����

            options.AddPolicy("UserPolicy",
                policyBuilder => policyBuilder.RequireAssertion(ctx =>
                    ctx.User.HasClaim(c => c.Type == ClaimTypes.Role)
                && ctx.User.Claims.First(c => c.Type.Equals(ClaimTypes.Role)).Value == "admin")
           //.Combine(qqEmailPolicy)
           );//�Զ���
             //policy����  û��Requirements

            //options.AddPolicy("QQEmail", policyBuilder => policyBuilder.Requirements.Add(new QQEmailRequirement("QQEmail")));
            //options.AddPolicy("DoubleEmail", policyBuilder => policyBuilder.Requirements.Add(new DoubleEmailRequirement()));
        });

        ////����ΪAuthorizationHandler<TRequirement>:IAuthorizationHandler�����Կ��Զ�ε���ע��
        ////�������� servicepProvider.GetService<IAuthorizationHandler>()�ܻ�ȡ����
        //context.Services.AddSingleton<IAuthorizationHandler, OtherMailHandler>();
        //context.Services.AddSingleton<IAuthorizationHandler, QQMailHandler>();

        #endregion


        //���뷢������ͨ�����������nginx�Ƚ���https->http��ת,ת��ԭʼ�����Header��������
        context.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
            //options.KnownProxies.Add(IPAddress.Parse("47.65.1.1"));//��������εĴ��������
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
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureVirtualFileSystem...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}�� Configure����{nameof(AbpVirtualFileSystemOptions)} �����ļ�ϵͳ:{nameof(NetDomainSharedModule)}{nameof(NetDomainModule)}{nameof(NetApplicationContractsModule)}{nameof(NetApplicationModule)}{nameof(NetWebModule)}....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
                // "YourRootNameSpace" ����Ŀ�ĸ������ռ�����. ��������Ŀ�ĸ������ռ�����Ϊ��,�����贫�ݴ˲���.
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
    /// ����ǰ�˵����˵�
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureNavigationServices(IConfiguration configuration)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureNavigationServices...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        Configure<AbpNavigationOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��Configure����{nameof(AbpNavigationOptions)} NetCoreMenuContributor:{nameof(NetMenuContributor)}....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
            options.MenuContributors.Add(new NetMenuContributor(configuration));
        });

        Configure<AbpToolbarOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��Configure����{nameof(AbpToolbarOptions)} NetToolbarContributor:{nameof(NetToolbarContributor)}....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
            options.Contributors.Add(new NetToolbarContributor());
        });
    }


    /// <summary>
    /// ����GRPC ���й���
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureGrpcs(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureGrpcs...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        var logger = context.Services.GetRequiredServiceLazy<ILogger<CustomClientLoggerInterceptor>>();
        context.Services.AddGrpcClient<CustomMath.CustomMathClient>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddGrpcClient����{nameof(GrpcClientFactoryOptions)} ....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
            options.Address = new Uri("https://localhost:5001");
            options.Interceptors.Add(new CustomClientLoggerInterceptor(logger));
        });
        //.ConfigureChannel(grpcOptions =>
        //{
        //    //HttpClient --443 ����grpc-https://localhost:5001
        //    grpcOptions.HttpClient=new HttpClient(new HttpClientHandler
        //    {
        //        ServerCertificateCustomValidationCallback = (msg,cert,chain,error)=>true//����֤��
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
                    var token = await JWTTokenHelper.GetJWTToken();//todo:��ʱ��ȡ--���ӻ���
                    Log.Logger.Information($"token:{token}");
                    metaData.Add("Authorization", $"Bearer {token}");
                });
                //���󶼴���token
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

                ////api���ͻʱĬ��ʹ�õ�һ��
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
    /// ����AbpAntiForgerys 
    /// </summary>
    private void ConfigureAbpAntiForgerys()
    {
        //���Խӿڵ���Header��RequestVerificationToken��֤
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidateIgnoredHttpMethods.Add("POST");
            options.AutoValidateIgnoredHttpMethods.Add("PUT");
            options.AutoValidateIgnoredHttpMethods.Add("DELETE");
        });
    }

    /// <summary>
    /// �����ļ��ϴ� 
    /// </summary>
    private void ConfigureFileUploadOptions()
    {
        // ����Kestrel������ϴ��ļ���С������
        Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = null; // ����Ϊnull�����ƴ�С��Ҳ������Ϊ����Ĵ�С����5GB��

            //��λ���ֽ� From testing, the MaxRequestHeadersTotalSize can be set to 1000M. If set to 1GB, the api will be started failed.
            options.Limits.MaxRequestHeadersTotalSize = 900 * 1024 * 1024;//900MB �����ֽ�Ϊ��λ�����
        });

        // ���й���IIS/IIS Express��ʱ��ҲҪ����IISת����Kestrel�������С����
        Configure<IISServerOptions>(options =>
        {
            // ����ͨ������Ҫ�������ã���ΪIIS��������Ҫ��web.config������
            options.MaxRequestBodySize = long.MaxValue; // ����Ϊlong.MaxValue���ƹ�Ĭ�����ƣ��������������С
        });

        // �����Ҫ��Ӧ�ò�����ϴ���С������ʹ��IFormFile�м��
        Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = long.MaxValue; // ������������ಿ�������ݴ�С
        });
    }

    /// <summary>
    /// ����cookie
    /// </summary>
    /// <param name="context"></param>
    private void ConfigCookie(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigCookie...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        //context.Services.ConfigureExternalCookie(options => { });
        //context.Services.ConfigureApplicationCookie(options => { });
        context.Services.Configure<CookiePolicyOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��Configure����{nameof(CookiePolicyOptions)} �ɿ�վ��cookie Cookie.SameSite:{SameSiteMode.Lax.DisplayName()}....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
            options.MinimumSameSitePolicy = SameSiteMode.Lax;//�ɿ�վ��cookie
            //options.MinimumSameSitePolicy = SameSiteMode.None;
            //options.OnAppendCookie = cookieContext =>
            //    UserAgentExtention.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            //options.OnDeleteCookie = cookieContext =>
            //    UserAgentExtention.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });
    }

    /// <summary>
    /// ����Session
    /// </summary>
    /// <param name="context"></param>
    private void ConfigSession(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigSession...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        context.Services.AddSession(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddSession ����{nameof(SessionOptions)}Session Cookie �ɿ�վ��cookie Chrome�°汾��SameSite��Ҫ��Cookie.SameSite����Ϊ:{SameSiteMode.Lax.DisplayName()}....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
            options.IdleTimeout = TimeSpan.FromHours(2);
            options.Cookie.HttpOnly = true;
            options.Cookie.Name = "ops_cookie";
            options.Cookie.IsEssential = true;
            //������ֻ��ͨ����ȫ��������Ӧ�ó���cookie��
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;//CookieSecurePolicy.None;
            options.Cookie.SameSite = SameSiteMode.Lax;//�ɿ�վ��cookie Chrome�°汾��SameSite��Ҫ��
            //options.Cookie.SameSite = SameSiteMode.None;
        });
    }

    /// <summary>
    /// ���ÿ���
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureCors(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureCors...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        var configuration = context.Services.GetConfiguration();
        context.Services.AddCors(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddCors���ÿ���{nameof(Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions)}....������ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
            //����Ĭ������ ���м��
            options.AddPolicy(CommonConsts.AppName, builder =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��CorsOptions.AddPolicy���ÿ���AddPolicy Name��{CommonConsts.AppName},CorsOrigins:{configuration["App:CorsOrigins"]}....������ConfigureServices�е� {options.GetType().Name}_AddPolicy_{builder.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
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
    /// ����Hsts
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureHsts(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureHsts...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        // Configure Hsts
        context.Services.AddHsts(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddHsts ����{nameof(HstsOptions)}....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(180);
        });
    }

    /// <summary>
    /// ����HttpPolly
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureHttpPolly(ServiceConfigurationContext context)
    {
        // Configure HttpPolly
        Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureHttpPolly....ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");

        //�����վ����Ϊ HTTP GET����Ӧ�� 10 �볬ʱ�� �������� HTTP ����Ӧ�� 30 �볬ʱ
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
    /// �����ļ�ѹ��
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureCompressServices(ServiceConfigurationContext context)
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureCompressServices...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        context.Services.AddResponseCompression(options =>
        {
            Log.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddResponseCompression ����{nameof(ResponseCompressionOptions)}...ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        });
    }

    /// <summary>
    /// ���Ա��ػ�
    /// </summary>
    private void ConfigureLocalizationServices()
    {
        Log.Information($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureLocalizationServices...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        Configure<AbpLocalizationOptions>(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��Configure����{nameof(AbpLocalizationOptions)} zh-Hans  �������� en English....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
            options.Resources
                .Get<NetResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );

            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "��������"));
        });
    }

    #endregion



    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} Start PostConfigureServices ....");
        base.PostConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} End PostConfigureServices ....");
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} Start OnPreApplicationInitialization ....");
        base.OnPreApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} End OnPreApplicationInitialization ....");
    }


    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} Start OnApplicationInitialization ....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");

        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        var configuration = context.GetConfiguration();


        #region ��headerд��һ�������Id
        //���һ���м��,��鲢ȷ����response header����һ��correlationId�������Id��
        //request: requestû��header,�򷵻�һ��Guid.NewGuid().ToString("N")��
        //����ȡHeaders��X-Correlation-Id�������ǿ��򷵻أ���������һ������header����һ��ͷ���򷵻ش�ͷ
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�����һ���м��,��鲢ȷ����response header����һ��correlationId�������Id�� request: requestû��header,�򷵻�һ��Guid.NewGuid().ToString(N) ����ȡHeaders��X-Correlation-Id�������ǿ��򷵻أ���������һ������header����һ��ͷ���򷵻ش�ͷ....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseCorrelationId();

        //���ַ�ʽ��ʱ��(Ӧ�ڲ����ص�����µ���UseExceptionless)��
        //������ExceptionlessClient ����ע������IServiceCollection AddExceptionlessʱ����appKey��url 
        app.UseExceptionless("Gum3CWHNe4uKf7CYGT1CIBEKRx17FreeOywYTIDr");

        #region ForwardedHeader�м������ �����е�Header����ͨ��Nginx���м����

        //���� �����е�Header����ͨ��Nginx���м������http����https��������Ҫ��https�������header���ݴ��ݸ�nginx������http����
        //nginx�����У�proxy_set_header X-Forwarded - For  X-Forwarded - Proto
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

        #region ȫ���쳣���� ����ҵ��ϵͳû�в��񵽿�ܵ�����㲶��ȫ���쳣
        //Ӧ�����ڹܵ��е����쳣����ί�У��������ܲ����ں����ܵ��������쳣
        //�Ȱ��쳣������м��д����ǰ�棬�������ɲ����Ժ�����з������κ��쳣��
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��ȫ���쳣���� ����ҵ��ϵͳû�в��񵽿�ܵ�����㲶��ȫ���쳣 �Ȱ��쳣������м��д����ǰ�棬�������ɲ����Ժ�����з������κ��쳣....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        if (env.IsDevelopment())
        {
            //���������Ļ�����ӡ��������쳣 �Ķ�ջ��Ϣ
            app.UseDeveloperExceptionPage();

            //��ʾPII��ϢȻ��Ϳ������쳣��Ϣ�п���������������Ϣ��, ���㿪������...
            IdentityModelEventSource.ShowPII = true;

            #region  ͨ��ȫ���쳣���� �Զ����������չ(��ӡ��ջ��Ϣ)

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
            //��������ָ����������(���ع̶���ʾ����ҳ)�����ÿͻ�ֱ�ӿ����쳣��ջ��Ϣ
            app.UseErrorPage();
        }

        #region ʹ��HTTP�������ҳ

        //UseStatusCodePagesWithReExecute ���� 404 ״̬���벢����ִ�н���ָ�� URL �Ĺܵ������ǵ�(/Error/404)��
        //������������ Http �ܵ����� MVC �м���������м������ NotFound ��ͼ HTML ��״̬���� ��Ȼ�� 200
        //����Ӧ�������ͻ���ʱ������ͨ�� UseStatusCodePagesWithReExecute �м�������м����ʹ�� HTML ��Ӧ���� 200 ״̬�����滻Ϊԭʼ�� 404 ״̬����
        //��Ϊ��ֻ������ִ�йܵ����������ض��������������ǻ��ڵ�ַ���б���ԭʼ�����ַ�������Ϊ/Home/Error/404

        //app.UseStatusCodePages();
        //app.UseStatusCodePages(async context =>
        //    {
        //        context.HttpContext.Response.ContentType = "text/plain";
        //        await context.HttpContext.Response.WriteAsync(
        //            "Status code page, status code: " +
        //            context.HttpContext.Response.StatusCode);
        //    });
        #endregion
        app.UseStatusCodePagesWithReExecute("/Home/Error{0}");//ռλ��{0}�������Response.StatusCode 

        #region ABP������Ա��ػ��м��
        //��ȡRequestLocalizationOptions����ִ��LocalizationOptions��ί��  IAbpRequestLocalizationOptionsProvider.InitLocalizationOptions
        //����RequestLocalizationMiddleware������LocalizationOptions��_loggerFactory
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}������RequestLocalizationMiddleware������LocalizationOptions��_loggerFactory....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseAbpRequestLocalization();

        #region HTTP �ϸ��䰲ȫЭ�� (HSTS) �м��
        //HTTP �ϸ��䰲ȫЭ�� (HSTS) �м�� (UseHsts) ��� Strict-Transport-Security ��ͷ
        #endregion
        //app.UseHsts();

        #region �� HTTP �����ض��� HTTPS  ��������һ�㲻��
        //�� HTTP �����ض��� HTTPS  ��������һ�㲻��
        #endregion
        //app.UseHttpsRedirection();

        #region Cookie �����м��
        //Cookie �����м�� (UseCookiePolicy) ʹӦ�÷���ŷ��һ�����ݱ������� (GDPR) �涨
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�������м�� (UseCookiePolicy) ʹӦ�÷���ŷ��һ�����ݱ������� (GDPR) �涨....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseCookiePolicy();

        #region �Ự�м��
        //�Ự�м�� (UseSession) ������ά���Ự״̬(�ڲ�ֱ������cookie  key value��� ���ã�������Ч�ڵķ�ʽ�ﵽ�洢seesion�Ự)�� 
        //���Ӧ��ʹ�ûỰ״̬������ Cookie �����м��֮��� MVC �м��֮ǰ���ûỰ�м��

        //Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}���Ự�м�� (UseSession) ������ά���Ự״̬(�ڲ�ֱ������cookie  key value��� ���ã�������Ч�ڵķ�ʽ�ﵽ�洢seesion�Ự)�� ���Ӧ��ʹ�ûỰ״̬������ Cookie �����м��֮��� MVC �м��֮ǰ���ûỰ�м��....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseSession();

        ////�Զ���Use�м���߼�
        //app.Use(next =>
        //{
        //    Log.Logger.Warning($"{{0}}", $"{CacheKeys.LogCount++}����MVC������־��3��Use�������ڷ���RequestDelegateǰ�Ĵ���,����Build�м����װ����ʱ�ᱻreverse����ִ��һ�Ρ�ע��Build������startup����configure֮�����Դ�����־��ʾ��Startup Configure������־����֮�� �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");

        //    return new RequestDelegate(async ctx =>
        //    {
        //        //ctx.Response.OnStarting(async () =>
        //        //{
        //        //    Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��ctx.Response.OnStarting ....");
        //        //});

        //        Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}����MVC������־��4����������м��ִ��RequestDelegateί���д��룬��һ��������---EndpointRoutingMiddleware�м���ڲ���ʼ·��ƥ�䡿�߳�Id����{Thread.CurrentThread.ManagedThreadId}��.....");
        //        await next.Invoke(ctx);//ִ����һ���м��߼� (CustomLogMiddleware)
        //        Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}����MVC������־��5�������м��EndpointRoutingMiddleware(next.Invoke)����󣬻ص���ǰRequestDelegateί�бջ��� �߳�Id����{Thread.CurrentThread.ManagedThreadId}��....");
        //    });
        //});
        ////����һ���Զ����м����invoke�����ڲ�����־��������������Ŀ����м��ί�д�������ִ�е��Ⱥ�˳��
        ////app.UseCustomLog();
        /// 
        #region ��׼Middleware
        ////�淨1---Use����---Add���޲���---IOptions<BrowserFilterOptions> options����Useָ�����ݵ� 
        ////�淨2---Use������---��Addʵ��---IOptions<BrowserFilterOptions> options����IOC���ɵ�
        ////�淨3---������---Use��Add������--AddΪ׼1  UseΪ׼2   ����3---�����2����UseΪ׼��ԭ���Ƕ���ֻ����UseMiddleware���ݵ�ֵ���Ͳ�������IOC��---���Ǻ�����----��������ע��IConfigureOptions<BrowserFilterOptions>��Ȼ�������Ч

        //app.UseBrowserFilter(new BrowserFilterOptions
        //{
        //    EnableIE = false,
        //    EnableFirefox = false
        //});//�淨1

        //app.UseBrowserFilter();//�淨2

        //app.UseBrowserFilter(option =>
        //{
        //    option.EnableEdge = true;
        //});//�淨3

        #endregion

        #region ����·�������·���м��

        //�������ʱ������(app.UseEndPoint��װ�м��ʱд���·�ɹ���)���·��ƥ�䣬�ҵ�EndPoint(����ʱ�Ż����·��ƥ����EndPoint)
        //��EndPointƥ�������app.UseEndPoint(��װ�м���߼�����Թ�������EndPoint(app.UseRouting�ڲ�ί��)��ִ��)
        //UseRouting��UseEndPonts��������װ�м��ʱִ��--��������ʱ ��װ�м��ִ�е�˳��
        //1��UseRouting����-->2��UseEndPonts(��װ�м��ʱ���·�ɹ���)
        //3��UseRouting�����ί��:EndPointRoutingMiddleware��������ʱ��ִ��
        //4��������ʱEndPointRoutingMiddleware����next.invoke()ǰ ��EndPoint����endpoint�ߴ�partManger�������г��� controller action �ս��ȥ�ҡ�
        //5�������2��UseEndPoint��װ�м��ʱ���õ�·�ɹ����ҵ�������EndPoint,
        //6��UseEndPonts�����ί��:EndPointMiddlewareҲ��������ʱ��ִ�У�
        //7��������ʱ��һ��EndPointEndPointRoutingMiddleware�ҵ�������EndPoint,
        //8��Ȼ�����next.invoke����EndPointMiddlewareִ��EndPoint.ִ��EndPoint֮��
        //9���ٻص�EndPointEndPointRoutingMiddleware�м��������߼�(�м�������յ����������˼��)��

        //ִ��˳����װ�м��>(����)�������ʱ  �����м������װ����-->�����м��������ʱ
        //��װ�м��ʱ��1��UseRouting(�����ڲ�ί��ǰû��ʲô�¶�)-->2��UseEndPonts(����ί��ǰ������·�ɹ���)-->
        //3���������ʱ��EndPointRoutingMiddlewareί��next.invokeǰ(�����Ѿ����úõ�·�ɹ������·��ƥ�䣬�ҵ�EndPoint)-->
        //4����EndPointMiddleware ִ���ս��ί��ǰ-->5��EndPointMiddleware ִ���ս��ί�к�-->
        //6��EndPointRoutingMiddlewareί��next.invoke��
        //�ܽ᣺UseRouting>UseEndPonts>EndPointRoutingMiddlewareǰ>EndPointMiddlewareִ��(ǰ,��,��)>EndPointRoutingMiddleware��
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}���������ʱ��app.UseRouting����(app.UseEndPoint��װ�м��ʱд���·�ɹ���)���·��ƥ�䣬�ҵ�EndPoint(����ʱ�Ż����·��ƥ����EndPoint)....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseRouting();

        #region �������
        //����Ĳ���������configure�п�������Ҫһ�£������м������Ҫ��UseRouting ֮��
        //��UseAuthentication UseAuthorization UseResponseCaching֮ǰ
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}������Ĳ���������configure�п�������Ҫһ�£������м������Ҫ��UseRouting ֮�� UseAuthentication UseAuthorization UseResponseCaching֮ǰ....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseCors(CommonConsts.AppName);

        #region �����֤ ������Ϣ����ȡtoken ����HttpContext.User ��context.User��ֵ
        //�����֤�м�� (UseAuthentication) ���Զ��û����������֤��Ȼ��Ż������û����ʰ�ȫ��Դ
        //��Ȩ ������Ϣ����ȡtoken AuthAppBuilderExtensions.UseAuthentication->ָ��ʹ���м��AuthenticationMiddleware->
        //������֤�ܵ� --context.User = authenticateResult.Principal;
        //����HttpContext.User��Abp Vnext�ṩ��ICurrentUser������ԴHttpContext.User?Thead.ClaimsPrincipals
        //��Ȩ ������Ϣ����ȡtoken �����û�е�¼����¼����˭ ����û���Ϣ��ȡ(������ֵ��context.User)

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�������֤�м�� (UseAuthentication)��Ȩ ������Ϣ����ȡtoken �����û�е�¼����¼����˭ ����û���Ϣ��ȡ(�����긳ֵ��context.User AbpVnext�ṩ��ICurrentUser������ԴHttpContext.User?Thead.ClaimsPrincipals)....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseAuthentication();

        #region //ʹ��jwt��ʽ��token �м��
        //ʹ��jwt��ʽ��token �м��

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��ʹ��jwt��ʽ��token �м��....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        //app.UseJwtTokenMiddleware();

        #region ���ٻ����ѹ������
        //���ٻ����ѹ���������ض��ڳ����ģ����ڶ����Ч������
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�����ٻ����ѹ���������ض��ڳ����ģ����ڶ����Ч������....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseResponseCaching();//ʹ��ResponseCaching����

        #region ѹ��
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��ѹ��....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        app.UseResponseCompression();//ѹ��
        #endregion

        #region ��̬�ļ��м��
        //��̬�ļ��м�������caching��compress֮�� ��ʾ��������ѹ���ľ�̬�ļ�
        //��ͬ�� ʹ�� wwwroot �ļ����е�����(��̬)(js, css, image ...)�ļ�
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}����̬�ļ� ��ͬ�� ʹ�� wwwroot �ļ����е�����(��̬)(js, css, image ...)�ļ� ���м�������caching��compress֮�� ��ʾ��������ѹ���ľ�̬�ļ�....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseStaticFiles(new StaticFileOptions//�ṩ��̬�ļ�StaticFileOptions.FileProvider
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

        #region ���þ�̬�ļ�url��ַ���
        //���þ�̬�ļ�url��ַ���
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�����þ�̬�ļ�url��ַ���....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseDirectoryBrowser(new DirectoryBrowserOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "images")),
            RequestPath = "/MyImages"
        });

        #region ���⻧ ����NetMultiTenancyModule����

        ////���⻧ ����NetMultiTenancyModuleģ������
        //Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�����⻧....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");

        //if (CommonConsts.MultiTenancyEnabled)
        //{
        //    app.UseMultiTenancy();
        //}
        #endregion


        #region ��IdentityServer�м����ӵ�HTTP�ܵ� (����IdentityServer��ʼ����·�ɲ���������)
        //��IdentityServer�м����ӵ�HTTP�ܵ� (����IdentityServer��ʼ����·�ɲ���������)
        //�����ļ���������������飬��ô����м���ᱨ�����bug
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}����IdentityServer�м����ӵ�HTTP�ܵ� (����IdentityServer��ʼ����·�ɲ���������)....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        //app.UseIdentityServer();

        ////�Զ��尲ȫ���Լ��Զ����м��
        //app.UseSecurePolicy();
        //app.UseInheritedMiddleware();

        app.UseDynamicClaims();

        #region ������Ȩ�û����ʰ�ȫ��Դ����Ȩ�м��

        //Claim����Ϣ
        //    ClaimsIdentity�����
        //    ClaimsPrincipal��һ���˿����ж�����
        //    AuthenticationTicket���û�Ʊ��
        //    ����һ��---д��cookie
        //������Ȩ�û����ʰ�ȫ��Դ����Ȩ�м�� //��Ȩ������û��ľ���Ȩ��
        //��Startup���е�Configure����ͨ�����UseAuthenticationע����֤�м��
        //ͨ��AuthenticationSchemeProvider��ȡ��ȷ��Scheme��
        //��AuthenticationService��ͨ��Scheme��AuthenticationHandlerProvider��ȡ��ȷ��AuthenticationHandler��
        //���ͨ����Ӧ��AuthenticationHandler��AuthenticateAsync����������֤����
        //(��Ҫ�Ǵ�Request.Headers�����ȡAuthorization��Bearer��������������AddJwtBearer�д����ί�в���JwtBearerOptions��TokenValidationParameters������Ϊ���ݽ��жԱ���������֤�Ƿ�ͨ�����)
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}����Ȩ������û��ľ���Ȩ��....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}����Startup���е�Configure����ͨ�����UseAuthenticationע����֤�м��");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��ͨ��AuthenticationSchemeProvider��ȡ��ȷ��Scheme");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}����AuthenticationService��ͨ��Scheme��AuthenticationHandlerProvider��ȡ��ȷ��AuthenticationHandler");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�����ͨ����Ӧ��AuthenticationHandler��AuthenticateAsync����������֤����");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��(��Ҫ�Ǵ�Request.Headers�����ȡAuthorization��Bearer��������������AddJwtBearer�д����ί�в���JwtBearerOptions��TokenValidationParameters������Ϊ���ݽ��жԱ���������֤�Ƿ�ͨ�����)");
        #endregion
        app.UseAuthorization();

        #region swagger/swagger-ui�м�� ���ߵ� API �ĵ���������Թ���
        //swagger/swagger-ui�м��
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��swagger/swagger-ui�м��....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseSwagger(option =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��UseSwagger {nameof(SwaggerOptions)}....");
            //option.RouteTemplate = "api-doc/{documentName}/swagger.json";//���ú�������շ���·���;��� /api-doc/index.html
        });
        app.UseSwaggerUI(options =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��UseSwaggerUI {nameof(SwaggerUIOptions)}....");
            EnumContext.Instance.GetEnumTypeItemKeyNameDescriptions(new InputNameDto { Name = nameof(VersionType) })
                .ForEach(v =>
                {
                    //configuration["App:SwaggerEndpoint"]="/swagger/v1/swagger.json";
                    options.SwaggerEndpoint(string.Format(context.GetConfiguration()["App:SwaggerEndpoint"] ?? @$"/swagger/{v.ItemDescription}/swagger.json", v.ItemDescription), $"С���Ĺ����� NetCore API {v.ItemDescription}");
                });
            //typeof(VersionType).GetEnumNames().ToList().ForEach(v =>
            //{
            //    //configuration["App:SwaggerEndpoint"]="/swagger/v1/swagger.json";
            //    options.SwaggerEndpoint(string.Format(configuration["App:SwaggerEndpoint"] ?? @$"/swagger/{v}/swagger.json", v), $"С���Ĺ����� NetCore API {v}");
            //});
            //options.InjectJavascript("/swagger/ui/zh_CN.js"); // �������İ� ��Ч
            //options.ShowExtensions();
            //options.DocExpansion(DocExpansion.None);
            //options.RoutePrefix = "api-doc";
        });
        #region ABP�����־�м��

        //�ж������Ƿ���Ҫд���log  =�������ã��������������ʶ����û�û����֤ ����ƣ�
        //get����д�����־����Ҫ�Ļ�������һ��IAuditLogSaveHandle
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}���ж������Ƿ���Ҫд���log ->�����ã��������������ʶ����û�û����֤ ����� get����д�����־������Ҫ�Ļ����贴��һ��IAuditLogSaveHandle....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseAuditing();

        app.UseAbpSerilogEnrichers();

        #region ·�ɹ���ִ���ս���м��,�°汾���滻Ϊ UseConfiguredEndpoints

        ////���Ⱦ���·�ɹ����ƥ�䣬�ҵ�����������ĵ�IRouter,Ȼ�����IRouter.RouteAsync������RouteContext.Handler��
        ////�������󽻸�RouteContext.Handler������
        ////��MVC���ṩ������IRouterʵ�֣��ֱ����£�MvcAttributeRouteHandler,MvcRouteHandler
        ////����·�ɹ���
        //app.UseEndpoints(endpoints =>
        //{
        //    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        //    endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
        //    endpoints.MapRazorPages();
        //    //additionalConfigurationAction?.Invoke(endpoints);
        //});
        #endregion

        //app.UseMvcWithDefaultRouteAndArea();//�ѹ�ʱ ʹ��UseConfiguredEndpoints���
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�����Ⱦ���·�ɹ����ƥ�䣬�ҵ�����������ĵ�IRouter,Ȼ�����IRouter.RouteAsync������RouteContext.Handler,�����󽻸�RouteContext.Handler������ ��MVC���ṩ������IRouterʵ�֣��ֱ����£�MvcAttributeRouteHandler,MvcRouteHandler....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        app.UseConfiguredEndpoints(x =>
        {
            Log.Logger.Warning($"{{0}}", $"{CacheKeys.LogCount++}����MVC������־��2��UseConfiguredEndpoints_{nameof(IEndpointConventionBuilder)} ��ί�л�ֱ��ִ�У�������װ�м������,Ŀ��������װ�м�����̹������Ƚ�·�ɹ�����룬����ConfigreService�Ѿ����ص��ս�����dll���������EndPoint��ת����׼��EndPoint����Դ(�ڲ�UseEndpoints-->MapControllerRoute)��������Http����ʱ��UseRouting �������---EndpointRoutingMiddleware��EndPoint����Դ�в������Endpoint�Ĳ��� ��Ȼ������EndpointMiddleware����ִ��Endpoint(��������)���߳�Id����{Thread.CurrentThread.ManagedThreadId}��....����ע�⣺����־�Ĵ�ӡ˳��Ϊ�м�������磬��Ϊ���м��Ϊ���һ����������ǵ�һ�����ټ����ڲ��߼�(·�ɹ���)��ֱ��invoke��");
        });


        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��SeedDataĬ���������� ��һ�����м���....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        //ExtraSeedData(context);//ExtraSeedDataĬ���������� ��һ�����м��� һ�㲻��Ҫ������ִ��

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} End OnApplicationInitialization ....�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");

    }


    /// <summary>
    /// ��ʼ�����ݵ����ݿ�
    /// </summary>
    /// <param name="context"></param>
    private void ExtraSeedData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��{nameof(NetWebModule)}  SeedData��ʼ������ִ�� ....�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");

            using var scope = context.ServiceProvider.CreateScope();
            await scope.ServiceProvider
                .GetRequiredService<IDataSeeder>()
                .SeedAsync();
        });
    }


    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} Start OnPostApplicationInitialization ....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        base.OnPostApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} End OnPostApplicationInitialization ....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        Log.Warning($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"..............................................OnApplicationShutdown ��������ִ��............................................");
        Log.Warning($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} Start OnApplicationShutdown ....�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        base.OnApplicationShutdown(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetWebModule)} End OnApplicationShutdown ....�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
    }


}
