using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using Common.Storage;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parakeet.Net.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.OpenApi.Models;
using Parakeet.Net.Extentions;
using Parakeet.Net.Localization;
using Serilog;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Swashbuckle;
using Volo.Abp.VirtualFileSystem;
using Microsoft.AspNetCore.ResponseCaching;
using System.Threading;
using Volo.Abp.Data;
using Volo.Abp.IdentityServer.Jwt;
using Volo.Abp.Threading;
using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Claims;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Logging;
using Parakeet.Net.Aop;
using Parakeet.Net.GrpcLessonServer;
using Parakeet.Net.ServiceGroup;
using Volo.Abp.AutoMapper;
using Parakeet.Net.GrpcService;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using System.Net.Http;
using Exceptionless;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Logging;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;

namespace Parakeet.Net;

[DependsOn(
    typeof(NetHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(NetApplicationModule),
    typeof(NetEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
public class NetHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} Start PreConfigureServices ....");
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            //将嵌入式文件注册到虚拟文件系统.
            options.AddAssemblyResource(
                typeof(NetResource),
                typeof(NetDomainModule).Assembly,
                typeof(NetDomainSharedModule).Assembly,
                typeof(NetApplicationModule).Assembly,
                typeof(NetApplicationContractsModule).Assembly,
                typeof(NetHttpApiHostModule).Assembly
            );
        });
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} End PreConfigureServices ....");
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} Start ConfigureServices ....");

        ConfigureConventionalControllers();
        ConfigureDataProtection(context);
        ConfigureDistributedLocking(context);

        ConfigureAuthentication(context);//配置鉴权
        ConfigureAuthorization(context);//配置授权
        ConfigureAutoMapper(); //在应用层NetCoreApplicationModule已配置
        ConfigureCors(context); //配置跨域
        ConfigureCache(context); //配置缓存和Redis
        ConfigureSwaggerServices(context); //配置swagger
        ConfigureVirtualFileSystem(context); //配置虚拟文件系统 这里建议注释掉

        ConfigureGrpcs(context);

        //ConfigureDbContext(context); //配置PGSql 底层模块也会配置 谁后执行就听谁的:当然是外层的Module最后执行


        ConfigureHsts(context);
        ConfigureHttpPolly(context);
        ConfigureCompressServices(context);
        ConfigureLocalizationServices();//本地语言化
        ConfigureAbpAntiForgerys();
        ConfigureFileUploadOptions();

        //将 AlwaysAllowAuthorizationService 注册到 依赖注入 系统中
        //绕过授权服务. 通常用于在需要禁用授权系统的集成测试中
        context.Services.AddAlwaysAllowAuthorization();

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} End ConfigureServices ....");
    }

    #region 所有配置
    
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

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<NetDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Parakeet.Net.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<NetDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Parakeet.Net.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<NetApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Parakeet.Net.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<NetApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Parakeet.Net.Application"));
                options.FileSets.ReplaceEmbeddedByPhysical<NetHttpApiHostModule>(hostingEnvironment.ContentRootPath);
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            //options.ConventionalControllers.Create(typeof(NetApplicationModule).Assembly);
            options.ConventionalControllers.Create(typeof(NetApplicationModule).Assembly, option =>
            {
                //Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(AbpAspNetCoreMvcOptions)}.ConventionalControllers.Create{nameof(ConventionalControllerSetting)}    ConventionalControllers.Create 动态api RootPath配置....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                //它始终以 /api开头
                //路由路径. 默认值为"/app", 可以进行如下配置
                option.RootPath = "parakeet"; // api/parakeet/[controller]/[Action]
                //option.UrlControllerNameNormalizer = (contet)=> "AreaTest";
                ////通过提供TypePedicate选项进一步过滤类以成为API控制器
                //options.TypePredicate = type => true;//如果你不想将此类型公开为API控制器, 则可以在类型检查时返回false.
                //option.TypePredicate = type => type.Name.EndsWith("AppService");//以AppService结尾的才可以反射为动态controller
                //option.ControllerTypes.Add(typeof(CustomAppService));//自定义的AppService没有继承IAppService接口的可以手动添加也可以动态映射出api
            });
        });
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
    }

    /// <summary>
    /// Authentication认证 通过添加AddAuthentication
    /// 注册了AuthenticationService, AuthenticationHandlerProvider,
    /// 配置权限使用IdentityServer，IdentityServer服务器地址，注册到 ApiName到IdentityServer服务器
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        ////通过添加AddAuthentication注册了AuthenticationService, AuthenticationHandlerProvider,AuthenticationSchemeProvider这三个对象
        ////通过AddAuthentication返回的AuthenticationBuilder通过AddJwtBearer（或者AddCookie）
        ////来指定Scheme类型和需要验证的参数,并指定了AuthenticationHandler
        //context.Services.AddAuthentication("Bearer")////注册Scheme：Bearer/Cookie
        //    .AddIdentityServerAuthentication(options =>
        //    {
        //        //可以使用http://spider.dev.spdio.com/为idserver服务
        //        options.Authority = configuration["AuthServer:Authority"];
        //        options.RequireHttpsMetadata = false;
        //        options.ApiName = configuration["AuthServer:ApiName"] ?? "parakeet";
        //    });
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata");
                options.Audience = "parakeet";
            });

        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
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
            options.AddMaps<NetHttpApiHostModule>(true);
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
        ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("Parakeet");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "Parakeet-Protection-Keys");
        }
    }

    private void ConfigureDistributedLocking(
        ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = ConnectionMultiplexer.Connect(context.Services.GetConfiguration()["Redis:Configuration"]!);
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });
    }

    //private void ConfigureUrls(ServiceConfigurationContext context)
    //{
    //    Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、配置{nameof(AppUrlOptions)}....ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
    //    var configuration = context.Services.GetConfiguration();
    //    Configure<AppUrlOptions>(options =>
    //    {
    //        Log.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(AppUrlOptions)}  RootUrl={configuration["App:SelfUrl"]}....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
    //        options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
    //    });
    //}

    /// <summary>
    /// 配置跨域
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureCors(ServiceConfigurationContext context)
    {
        context.Services.AddCors(options =>
        {
            // options.AddPolicy(CommonConsts.AppName, builder =>
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(context.Services.GetConfiguration()["App:CorsOrigins"]?
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

            //shared 模块已添加
            //options.Languages.Add(new LanguageInfo("en", "en", "English"));
            //options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
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
            options.Limits.MaxRequestHeadersTotalSize = 500 * 1024 * 1024;
        });

        // 当托管在IIS/IIS Express下时，也要调整IIS转发到Kestrel的请求大小限制
        Configure<IISServerOptions>(options =>
        {
            // 这里通常不需要额外配置，因为IIS的限制主要在web.config中设置
            options.MaxRequestBodySize = int.MaxValue; // 设置为int.MaxValue会绕过默认限制，允许最大的请求大小
        });

        // 如果需要在应用层控制上传大小，可以使用IFormFile中间件
        Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = long.MaxValue; // 设置允许的最大多部件表单数据大小
        });
    }

    #endregion


    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} Start PostConfigureServices ....");
        base.PostConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} End PostConfigureServices ....");
    }


    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} Start OnPreApplicationInitialization ....");
        base.OnPreApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} End OnPreApplicationInitialization ....");
    }


    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} Start OnApplicationInitialization ....");

        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        var configuration = context.GetConfiguration();

        //添加一个中间件，检查并确定给response header返回一个correlationId
        //request: request没有header,则返回一个Guid.NewGuid().ToString("N")，
        //若有取Headers【X-Correlation-Id】，若非空则返回，否则生成一个，给header设置一个头，则返回此头
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

        #region 全局异常处理 可在业务系统没有捕获到框架的最外层捕获全局异常 前后端分离，这个可以不配置
        ////应尽早在管道中调用异常处理委托，这样就能捕获在后续管道发生的异常
        ////先把异常处理的中间件写在最前面，这样方可捕获稍后调用中发生的任何异常。
        //Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、全局异常处理 可在业务系统没有捕获到框架的最外层捕获全局异常 先把异常处理的中间件写在最前面，这样方可捕获稍后调用中发生的任何异常....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

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
        //else
        //{
        //    //其它环境指定错误处理动作(返回固定提示错误页)，不让客户直接看到异常堆栈信息
        //    app.UseErrorPage();
        //}
        #endregion


        #region 使用HTTP错误代码页

        ////UseStatusCodePagesWithReExecute 拦截 404 状态代码并重新执行将其指向 URL 的管道即我们的(/Error/404)中
        ////整个请求流经 Http 管道并由 MVC 中间件处理，该中间件返回 NotFound 视图 HTML 的状态代码 依然是 200
        ////当响应流出到客户端时，它会通过 UseStatusCodePagesWithReExecute 中间件，该中间件会使用 HTML 响应，将 200 状态代码替换为原始的 404 状态代码
        ////因为它只是重新执行管道而不发出重定向请求，所以我们还在地址栏中保留原始请求地址而不会变为/Home/Error/404

        ////app.UseStatusCodePages();
        ////app.UseStatusCodePages(async context =>
        ////    {
        ////        context.HttpContext.Response.ContentType = "text/plain";
        ////        await context.HttpContext.Response.WriteAsync(
        ////            "Status code page, status code: " +
        ////            context.HttpContext.Response.StatusCode);
        ////    });
        //app.UseStatusCodePagesWithReExecute("/Home/Error{0}");//占位符{0}在这代表Response.StatusCode 
        #endregion

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
        ////Cookie 策略中间件 (UseCookiePolicy) 使应用符合欧盟一般数据保护条例 (GDPR) 规定
        //Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、策略中间件 (UseCookiePolicy) 使应用符合欧盟一般数据保护条例 (GDPR) 规定....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        
        //app.UseCookiePolicy();
        #endregion

        #region 会话中间件
        //会话中间件 (UseSession) 建立和维护会话状态(内部直接设置cookie  key value相关 设置，更新有效期的方式达到存储seesion会话)。 
        //如果应用使用会话状态，请在 Cookie 策略中间件之后和 MVC 中间件之前调用会话中间件

        //Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、会话中间件 (UseSession) 建立和维护会话状态(内部直接设置cookie  key value相关 设置，更新有效期的方式达到存储seesion会话)。 如果应用使用会话状态，请在 Cookie 策略中间件之后和 MVC 中间件之前调用会话中间件....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseSession();
        
        #region 标准Middleware
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
        app.UseCors(CommonConsts.AppName);//与configure中跨域名称要一致

        //指定使用中间件AuthenticationMiddleware 构建认证管道 --context.User = authenticateResult.Principal;
        //创建HttpContext.User，Abp Vnext提供的ICurrentUser就是来源HttpContext.User?Thead.ClaimsPrincipals
        app.UseAuthentication();//鉴权，检测有没有登录，登录的是谁，赋值给CurrentUser
        app.UseJwtTokenMiddleware("Bearer");

        #region 高速缓存和压缩排序
        //高速缓存和压缩排序是特定于场景的，存在多个有效的排序
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、高速缓存和压缩排序是特定于场景的，存在多个有效的排序....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        #endregion
        app.UseResponseCaching();//使用ResponseCaching缓存

        #region 压缩
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、压缩....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        app.UseResponseCompression();//压缩
        #endregion

        //虚拟文件中间件用于向客户端/浏览器提供嵌入式(js, css, image ...)文件,
        //就像 wwwroot 文件夹中的物理(静态)文件一样. 在静态文件中间件之后添加它
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

        app.UseUnitOfWork();
        app.UseDynamicClaims();

        //将IdentityServer中间件添加到HTTP管道 (允许IdentityServer开始拦截路由并处理请求)
        //配置文件中如果配置了数组，那么这个中间件会报错？这个bug
        app.UseIdentityServer();

        //在Startup类中的Configure方法通过添加UseAuthentication注册认证中间件
        //通过AuthenticationSchemeProvider获取正确的Scheme，
        //在AuthenticationService中通过Scheme和AuthenticationHandlerProvider获取正确的AuthenticationHandler，
        //最后通过对应的AuthenticationHandler的AuthenticateAsync方法进行认证流程
        //(主要是从Request.Headers里面获取Authorization的Bearer出来解析，再在AddJwtBearer中传入的委托参数JwtBearerOptions的TokenValidationParameters属性作为依据进行对比来进行认证是否通过与否)
        app.UseAuthorization();//授权

        //获取RequestLocalizationOptions，并执行LocalizationOptions的委托  IAbpRequestLocalizationOptionsProvider.InitLocalizationOptions
        //创建RequestLocalizationMiddleware，导入LocalizationOptions和_loggerFactory
        app.UseAbpRequestLocalization();

        app.UseSwagger();

        app.UseAbpSwaggerUI(options =>
        {
            //var configuration = context.GetConfiguration();
            options.SwaggerEndpoint(configuration["App:SwaggerEndpoint"] ?? "/swagger/v1/swagger.json", "Parakeet API");

            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes("parakeet");
        });

        //判断请求是否需要写审计log  =》可配置，不允许匿名访问而且用户没有认证 不审计，
        //get请求不写审记日志，需要的话，创建一个IAuditLogSaveHandle
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();

        ////首先经过路由规则的匹配，找到最符合条件的的IRouter,然后调用IRouter.RouteAsync来设置RouteContext.Handler，最后把请求交给RouteContext.Handler来处理。
        ////在MVC中提供了两个IRouter实现，分别如下：MvcAttributeRouteHandler,MvcRouteHandler
        //app.UseMvcWithDefaultRouteAndArea();
        //前后端分离 不需要默认设置route
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        //ExtraSeedData(context);//ExtraSeedData默认数据生成 第一次运行即可 一般不需要在这里执行

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} End OnApplicationInitialization ....");

    }
    
    /// <summary>
    /// 初始化数据到数据库
    /// </summary>
    /// <param name="context"></param>
    private void ExtraSeedData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(NetHttpApiHostModule)}  SeedData初始化数据执行 ....线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

            using var scope = context.ServiceProvider.CreateScope();
            await scope.ServiceProvider
                .GetRequiredService<IDataSeeder>()
                .SeedAsync();
        });
    }
    
    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} Start OnPostApplicationInitialization ....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        base.OnPostApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} End OnPostApplicationInitialization ....Configure中的组装管道流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        Log.Warning($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"..............................................OnApplicationShutdown 反序最先执行............................................");
        Log.Warning($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} Start OnApplicationShutdown ....线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        base.OnApplicationShutdown(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(NetHttpApiHostModule)} End OnApplicationShutdown ....线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
    }
}
