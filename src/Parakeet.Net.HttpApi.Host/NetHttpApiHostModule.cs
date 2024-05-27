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
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} Start PreConfigureServices ....");
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            //��Ƕ��ʽ�ļ�ע�ᵽ�����ļ�ϵͳ.
            options.AddAssemblyResource(
                typeof(NetResource),
                typeof(NetDomainModule).Assembly,
                typeof(NetDomainSharedModule).Assembly,
                typeof(NetApplicationModule).Assembly,
                typeof(NetApplicationContractsModule).Assembly,
                typeof(NetHttpApiHostModule).Assembly
            );
        });
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} End PreConfigureServices ....");
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} Start ConfigureServices ....");

        ConfigureConventionalControllers();
        ConfigureDataProtection(context);
        ConfigureDistributedLocking(context);

        ConfigureAuthentication(context);//���ü�Ȩ
        ConfigureAuthorization(context);//������Ȩ
        ConfigureAutoMapper(); //��Ӧ�ò�NetCoreApplicationModule������
        ConfigureCors(context); //���ÿ���
        ConfigureCache(context); //���û����Redis
        ConfigureSwaggerServices(context); //����swagger
        ConfigureVirtualFileSystem(context); //���������ļ�ϵͳ ���ｨ��ע�͵�

        ConfigureGrpcs(context);

        //ConfigureDbContext(context); //����PGSql �ײ�ģ��Ҳ������ ˭��ִ�о���˭��:��Ȼ������Module���ִ��


        ConfigureHsts(context);
        ConfigureHttpPolly(context);
        ConfigureCompressServices(context);
        ConfigureLocalizationServices();//�������Ի�
        ConfigureAbpAntiForgerys();
        ConfigureFileUploadOptions();

        //�� AlwaysAllowAuthorizationService ע�ᵽ ����ע�� ϵͳ��
        //�ƹ���Ȩ����. ͨ����������Ҫ������Ȩϵͳ�ļ��ɲ�����
        context.Services.AddAlwaysAllowAuthorization();

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} End ConfigureServices ....");
    }

    #region ��������
    
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
                //Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��{nameof(AbpAspNetCoreMvcOptions)}.ConventionalControllers.Create{nameof(ConventionalControllerSetting)}    ConventionalControllers.Create ��̬api RootPath����....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
                //��ʼ���� /api��ͷ
                //·��·��. Ĭ��ֵΪ"/app", ���Խ�����������
                option.RootPath = "parakeet"; // api/parakeet/[controller]/[Action]
                //option.UrlControllerNameNormalizer = (contet)=> "AreaTest";
                ////ͨ���ṩTypePedicateѡ���һ���������Գ�ΪAPI������
                //options.TypePredicate = type => true;//����㲻�뽫�����͹���ΪAPI������, ����������ͼ��ʱ����false.
                //option.TypePredicate = type => type.Name.EndsWith("AppService");//��AppService��β�Ĳſ��Է���Ϊ��̬controller
                //option.ControllerTypes.Add(typeof(CustomAppService));//�Զ����AppServiceû�м̳�IAppService�ӿڵĿ����ֶ����Ҳ���Զ�̬ӳ���api
            });
        });
        //������Ӧ�ó������������ΪBookAppService.��ô������Ϊ/book.
        //    ���Ҫ�Զ�������, ������UrlControllerNameNormalizerѡ��. ����һ��ί���������Զ���ÿ��������/���������.
        //    ����÷������� 'id'����, �����·�������'/{id}'.
        //    ���б�Ҫ,������Ӳ�������. �������ƴӷ����ϵķ������ƻ�ȡ����׼��;
        //ɾ��'Async'��׺. �����������Ϊ'GetPhonesAsync',���ΪGetPhones.
        //    ɾ��HTTP methodǰ׺. ���ڵ�HTTP methodɾ��GetList,GetAll,Get,Put,Update,Delete,Remove,Create,Add,Insert,Post��Patchǰ׺, ���GetPhones��ΪPhones, ��ΪGetǰ׺��GET�����ظ�.
        //�����ת��ΪcamelCase.
        //    ������ɵĲ�������Ϊ��,����������ӵ�·����.�������ᱻ��ӵ�·����(����'/phones').����GetAllAsync�������ƣ�����Ϊ��,��ΪGetPhonesAsync�������ƽ�Ϊphone.
        //    ����ͨ������UrlActionNameNormalizerѡ�����Զ���.It's an action delegate that is called for every method.
        //    �������һ������'Id'��׺�Ĳ���,��ô��Ҳ����Ϊ����·�߶���ӵ�·����(����'/phoneId').

        //Application��service���ɵ�api/[AppName](�����ļ��ж�) /[controller]/[Action]......
    }

    /// <summary>
    /// Authentication��֤ ͨ�����AddAuthentication
    /// ע����AuthenticationService, AuthenticationHandlerProvider,
    /// ����Ȩ��ʹ��IdentityServer��IdentityServer��������ַ��ע�ᵽ ApiName��IdentityServer������
    /// </summary>
    /// <param name="context"></param>
    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        ////ͨ�����AddAuthenticationע����AuthenticationService, AuthenticationHandlerProvider,AuthenticationSchemeProvider����������
        ////ͨ��AddAuthentication���ص�AuthenticationBuilderͨ��AddJwtBearer������AddCookie��
        ////��ָ��Scheme���ͺ���Ҫ��֤�Ĳ���,��ָ����AuthenticationHandler
        //context.Services.AddAuthentication("Bearer")////ע��Scheme��Bearer/Cookie
        //    .AddIdentityServerAuthentication(options =>
        //    {
        //        //����ʹ��http://spider.dev.spdio.com/Ϊidserver����
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

                ////api���ͻʱĬ��ʹ�õ�һ��
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
    //    Log.Information($"{{0}}", $"{CacheKeys.LogCount++}������{nameof(AppUrlOptions)}....ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
    //    var configuration = context.Services.GetConfiguration();
    //    Configure<AppUrlOptions>(options =>
    //    {
    //        Log.Error($"{{0}}", $"{CacheKeys.LogCount++}��Configure����{nameof(AppUrlOptions)}  RootUrl={configuration["App:SelfUrl"]}....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
    //        options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
    //    });
    //}

    /// <summary>
    /// ���ÿ���
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

            //shared ģ�������
            //options.Languages.Add(new LanguageInfo("en", "en", "English"));
            //options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "��������"));
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
            options.Limits.MaxRequestHeadersTotalSize = 500 * 1024 * 1024;
        });

        // ���й���IIS/IIS Express��ʱ��ҲҪ����IISת����Kestrel�������С����
        Configure<IISServerOptions>(options =>
        {
            // ����ͨ������Ҫ�������ã���ΪIIS��������Ҫ��web.config������
            options.MaxRequestBodySize = int.MaxValue; // ����Ϊint.MaxValue���ƹ�Ĭ�����ƣ��������������С
        });

        // �����Ҫ��Ӧ�ò�����ϴ���С������ʹ��IFormFile�м��
        Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = long.MaxValue; // ������������ಿ�������ݴ�С
        });
    }

    #endregion


    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} Start PostConfigureServices ....");
        base.PostConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} End PostConfigureServices ....");
    }


    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} Start OnPreApplicationInitialization ....");
        base.OnPreApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} End OnPreApplicationInitialization ....");
    }


    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} Start OnApplicationInitialization ....");

        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        var configuration = context.GetConfiguration();

        //���һ���м������鲢ȷ����response header����һ��correlationId
        //request: requestû��header,�򷵻�һ��Guid.NewGuid().ToString("N")��
        //����ȡHeaders��X-Correlation-Id�������ǿ��򷵻أ���������һ������header����һ��ͷ���򷵻ش�ͷ
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

        #region ȫ���쳣���� ����ҵ��ϵͳû�в��񵽿�ܵ�����㲶��ȫ���쳣 ǰ��˷��룬������Բ�����
        ////Ӧ�����ڹܵ��е����쳣����ί�У��������ܲ����ں����ܵ��������쳣
        ////�Ȱ��쳣������м��д����ǰ�棬�������ɲ����Ժ�����з������κ��쳣��
        //Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��ȫ���쳣���� ����ҵ��ϵͳû�в��񵽿�ܵ�����㲶��ȫ���쳣 �Ȱ��쳣������м��д����ǰ�棬�������ɲ����Ժ�����з������κ��쳣....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");

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
        //else
        //{
        //    //��������ָ����������(���ع̶���ʾ����ҳ)�����ÿͻ�ֱ�ӿ����쳣��ջ��Ϣ
        //    app.UseErrorPage();
        //}
        #endregion


        #region ʹ��HTTP�������ҳ

        ////UseStatusCodePagesWithReExecute ���� 404 ״̬���벢����ִ�н���ָ�� URL �Ĺܵ������ǵ�(/Error/404)��
        ////������������ Http �ܵ����� MVC �м���������м������ NotFound ��ͼ HTML ��״̬���� ��Ȼ�� 200
        ////����Ӧ�������ͻ���ʱ������ͨ�� UseStatusCodePagesWithReExecute �м�������м����ʹ�� HTML ��Ӧ���� 200 ״̬�����滻Ϊԭʼ�� 404 ״̬����
        ////��Ϊ��ֻ������ִ�йܵ����������ض��������������ǻ��ڵ�ַ���б���ԭʼ�����ַ�������Ϊ/Home/Error/404

        ////app.UseStatusCodePages();
        ////app.UseStatusCodePages(async context =>
        ////    {
        ////        context.HttpContext.Response.ContentType = "text/plain";
        ////        await context.HttpContext.Response.WriteAsync(
        ////            "Status code page, status code: " +
        ////            context.HttpContext.Response.StatusCode);
        ////    });
        //app.UseStatusCodePagesWithReExecute("/Home/Error{0}");//ռλ��{0}�������Response.StatusCode 
        #endregion

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
        ////Cookie �����м�� (UseCookiePolicy) ʹӦ�÷���ŷ��һ�����ݱ������� (GDPR) �涨
        //Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�������м�� (UseCookiePolicy) ʹӦ�÷���ŷ��һ�����ݱ������� (GDPR) �涨....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        
        //app.UseCookiePolicy();
        #endregion

        #region �Ự�м��
        //�Ự�м�� (UseSession) ������ά���Ự״̬(�ڲ�ֱ������cookie  key value��� ���ã�������Ч�ڵķ�ʽ�ﵽ�洢seesion�Ự)�� 
        //���Ӧ��ʹ�ûỰ״̬������ Cookie �����м��֮��� MVC �м��֮ǰ���ûỰ�м��

        //Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}���Ự�м�� (UseSession) ������ά���Ự״̬(�ڲ�ֱ������cookie  key value��� ���ã�������Ч�ڵķ�ʽ�ﵽ�洢seesion�Ự)�� ���Ӧ��ʹ�ûỰ״̬������ Cookie �����м��֮��� MVC �м��֮ǰ���ûỰ�м��....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseSession();
        
        #region ��׼Middleware
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
        app.UseCors(CommonConsts.AppName);//��configure�п�������Ҫһ��

        //ָ��ʹ���м��AuthenticationMiddleware ������֤�ܵ� --context.User = authenticateResult.Principal;
        //����HttpContext.User��Abp Vnext�ṩ��ICurrentUser������ԴHttpContext.User?Thead.ClaimsPrincipals
        app.UseAuthentication();//��Ȩ�������û�е�¼����¼����˭����ֵ��CurrentUser
        app.UseJwtTokenMiddleware("Bearer");

        #region ���ٻ����ѹ������
        //���ٻ����ѹ���������ض��ڳ����ģ����ڶ����Ч������
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�����ٻ����ѹ���������ض��ڳ����ģ����ڶ����Ч������....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        #endregion
        app.UseResponseCaching();//ʹ��ResponseCaching����

        #region ѹ��
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��ѹ��....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        app.UseResponseCompression();//ѹ��
        #endregion

        //�����ļ��м��������ͻ���/������ṩǶ��ʽ(js, css, image ...)�ļ�,
        //���� wwwroot �ļ����е�����(��̬)�ļ�һ��. �ھ�̬�ļ��м��֮�������
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

        app.UseUnitOfWork();
        app.UseDynamicClaims();

        //��IdentityServer�м����ӵ�HTTP�ܵ� (����IdentityServer��ʼ����·�ɲ���������)
        //�����ļ���������������飬��ô����м���ᱨ�����bug
        app.UseIdentityServer();

        //��Startup���е�Configure����ͨ�����UseAuthenticationע����֤�м��
        //ͨ��AuthenticationSchemeProvider��ȡ��ȷ��Scheme��
        //��AuthenticationService��ͨ��Scheme��AuthenticationHandlerProvider��ȡ��ȷ��AuthenticationHandler��
        //���ͨ����Ӧ��AuthenticationHandler��AuthenticateAsync����������֤����
        //(��Ҫ�Ǵ�Request.Headers�����ȡAuthorization��Bearer��������������AddJwtBearer�д����ί�в���JwtBearerOptions��TokenValidationParameters������Ϊ���ݽ��жԱ���������֤�Ƿ�ͨ�����)
        app.UseAuthorization();//��Ȩ

        //��ȡRequestLocalizationOptions����ִ��LocalizationOptions��ί��  IAbpRequestLocalizationOptionsProvider.InitLocalizationOptions
        //����RequestLocalizationMiddleware������LocalizationOptions��_loggerFactory
        app.UseAbpRequestLocalization();

        app.UseSwagger();

        app.UseAbpSwaggerUI(options =>
        {
            //var configuration = context.GetConfiguration();
            options.SwaggerEndpoint(configuration["App:SwaggerEndpoint"] ?? "/swagger/v1/swagger.json", "Parakeet API");

            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes("parakeet");
        });

        //�ж������Ƿ���Ҫд���log  =�������ã��������������ʶ����û�û����֤ ����ƣ�
        //get����д�����־����Ҫ�Ļ�������һ��IAuditLogSaveHandle
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();

        ////���Ⱦ���·�ɹ����ƥ�䣬�ҵ�����������ĵ�IRouter,Ȼ�����IRouter.RouteAsync������RouteContext.Handler���������󽻸�RouteContext.Handler������
        ////��MVC���ṩ������IRouterʵ�֣��ֱ����£�MvcAttributeRouteHandler,MvcRouteHandler
        //app.UseMvcWithDefaultRouteAndArea();
        //ǰ��˷��� ����ҪĬ������route
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        //ExtraSeedData(context);//ExtraSeedDataĬ���������� ��һ�����м��� һ�㲻��Ҫ������ִ��

        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} End OnApplicationInitialization ....");

    }
    
    /// <summary>
    /// ��ʼ�����ݵ����ݿ�
    /// </summary>
    /// <param name="context"></param>
    private void ExtraSeedData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��{nameof(NetHttpApiHostModule)}  SeedData��ʼ������ִ�� ....�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");

            using var scope = context.ServiceProvider.CreateScope();
            await scope.ServiceProvider
                .GetRequiredService<IDataSeeder>()
                .SeedAsync();
        });
    }
    
    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} Start OnPostApplicationInitialization ....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        base.OnPostApplicationInitialization(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} End OnPostApplicationInitialization ....Configure�е���װ�ܵ�������־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        Log.Warning($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"..............................................OnApplicationShutdown ��������ִ��............................................");
        Log.Warning($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} Start OnApplicationShutdown ....�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
        base.OnApplicationShutdown(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(NetHttpApiHostModule)} End OnApplicationShutdown ....�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
    }
}
