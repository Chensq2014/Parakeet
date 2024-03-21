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
        ////���ַ�ʽ��ʱ��(Ӧ�ڲ����ص�����µ���UseExceptionless)��
        ////������ExceptionlessClient ����ע������IServiceCollection AddExceptionlessʱ����appKey��url 
        //app.UseExceptionless("Gum3CWHNe4uKf7CYGT1CIBEKRx17FreeOywYTIDr");

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
        
        #region ��headerд��һ�������Id
        //���һ���м��,��鲢ȷ����response header����һ��correlationId�������Id��
        //request: requestû��header,�򷵻�һ��Guid.NewGuid().ToString("N")��
        //����ȡHeaders��X-Correlation-Id�������ǿ��򷵻أ���������һ������header����һ��ͷ���򷵻ش�ͷ
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�����һ���м��,��鲢ȷ����response header����һ��correlationId�������Id�� request: requestû��header,�򷵻�һ��Guid.NewGuid().ToString(N) ����ȡHeaders��X-Correlation-Id�������ǿ��򷵻أ���������һ������header����һ��ͷ���򷵻ش�ͷ....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseCorrelationId();

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
        //app.UseSession();

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

        #region ���⻧
        //���⻧
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�����⻧....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        if (CommonConsts.MultiTenancyEnabled)
        {
            app.UseMultiTenancy();
        }

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
}
