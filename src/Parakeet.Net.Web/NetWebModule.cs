using System;
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

namespace Parakeet.Net.Web;

[DependsOn(
    typeof(NetHttpApiClientModule),
    typeof(NetHttpApiModule),
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
                typeof(NetDomainSharedModule).Assembly,
                typeof(NetApplicationContractsModule).Assembly,
                typeof(NetWebModule).Assembly
            );
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        ConfigureBundles();
        ConfigureCache();
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureDistributedLocking(context, configuration);
        ConfigureUrls(configuration);
        ConfigureAuthentication(context, configuration);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices(configuration);
        ConfigureMultiTenancy();
        ConfigureSwaggerServices(context.Services);

        ConfigCookie(context);
        ConfigureHsts(context);
        ConfigureHttpPolly(context);
        ConfigureCompressServices(context);
        ConfigureLocalizationServices();
        ConfigureAbpAntiForgerys();

        //context.Services.AddBrowserFilter();
        //context.Services.AddMiddlewareFactory();
        //context.Services.AddInheritedMiddleware();
        //context.Services.AddSession();
        //Configure<AbpAuditingOptions>(options =>
        //{
        //    //options.IsEnabledForGetRequests = true;
        //    options.ApplicationName = "parakeet";
        //});
        //Configure<AbpBackgroundJobOptions>(options =>
        //{
        //    options.IsJobExecutionEnabled = false;
        //});

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

    private void ConfigureCache()
    {
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "net:";
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureMultiTenancy()
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = CommonConsts.MultiTenancyEnabled;
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookies";
            options.DefaultChallengeScheme = "oidc";
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
                options.ClientSecret = configuration["AuthServer:ClientSecret"];

                options.UsePkce = true;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("roles");
                options.Scope.Add("email");
                options.Scope.Add("phone");
                options.Scope.Add("parakeet");
            });
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
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<NetDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Parakeet.Net.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<NetApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Parakeet.Net.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<NetWebModule>(hostingEnvironment.ContentRootPath);
            });
        }
    }

    private void ConfigureNavigationServices(IConfiguration configuration)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new NetMenuContributor(configuration));
        });

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new NetToolbarContributor());
        });
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                EnumContext.Instance.GetEnumTypeItemKeyNameDescriptions(new InputNameDto { Name = nameof(VersionType) })
                    .ForEach(v =>
                    {
                        options.SwaggerDoc($"{v.ItemDescription}", new OpenApiInfo { Title = "Parakeet API", Version = $"{v.ItemDescription}" });
                    });
                //options.SwaggerDoc($"{VersionType.V1.DisplayName()}", new OpenApiInfo { Title = "Parakeet API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
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
        ////这种方式过时了(应在不重载的情况下调用UseExceptionless)，
        ////先配置ExceptionlessClient 依赖注入容器IServiceCollection AddExceptionless时设置appKey，url 
        //app.UseExceptionless("Gum3CWHNe4uKf7CYGT1CIBEKRx17FreeOywYTIDr");

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
        
        #region 向header写入一个相关性Id
        //添加一个中间件,检查并确定给response header返回一个correlationId（相关性Id）
        //request: request没有header,则返回一个Guid.NewGuid().ToString("N")，
        //若有取Headers【X-Correlation-Id】，若非空则返回，否则生成一个，给header设置一个头，则返回此头
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、添加一个中间件,检查并确定给response header返回一个correlationId（相关性Id） request: request没有header,则返回一个Guid.NewGuid().ToString(N) 若有取Headers【X-Correlation-Id】，若非空则返回，否则生成一个，给header设置一个头，则返回此头....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseCorrelationId();

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
        //app.UseSession();

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

        #region 多租户
        //多租户
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、多租户....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        if (CommonConsts.MultiTenancyEnabled)
        {
            app.UseMultiTenancy();
        }

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
}
