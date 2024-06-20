using AspNetCoreRateLimit;
using Common.Extensions;
using Common.Storage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parakeet.Net.GrpcEFCore;
using Parakeet.Net.ROServer.Handlers;
using Parakeet.Net.ROServer.Services;
using Serilog;
using System.Threading;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Parakeet.Net.ROServer
{
    [DependsOn(typeof(GrpcEFCoreModule),
        typeof(AbpAspNetCoreMvcModule)
        )]
    public class ROServerModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} Start PreConfigureServices ....");
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} End PreConfigureServices ....");
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} Start ConfigureServices ....");
            var configuration = context.Services.GetConfiguration();

            #region AutoMapper

            //添加对AutoMapper的支持
            //context.Services.AddAutoMapper();
            Configure<AbpAutoMapperOptions>(options =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、Configure配置{nameof(ROServerModule)} AbpAutoMapperOptions...ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                options.AddMaps<ROServerModule>();//options.AddProfile<ReverseDtoMapperProfile>();
            });

            #endregion

            #region grpc配置

            context.Services.AddGrpc();
            #endregion

            #region 自定义鉴权,授权替换系统默认接口实现

            context.Services.AddSingleton<IAuthenticationHandler, GrpcAuthticateHandler>();
            context.Services.AddSingleton<IAuthorizationHandler, GrpcRequireHandler>();

            context.Services.AddAuthentication(options =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、默认授权解决方案名称(1个解决方案(Scheme)->包含多个策略(policy) 1个策略policy->包含多个RequireMents 一个RequireMent包含多个Handlers处理程序  1个处理程序Handler->自定义代码)...ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                options.DefaultAuthenticateScheme = "grpc";
                options.DefaultSignInScheme = "grpc";
                options.DefaultChallengeScheme = "grpc";
            });
            context.Services.AddAuthenticationCore(options => options.AddScheme<GrpcAuthticateHandler>("grpc", "grpc"));

            context.Services.AddAuthorization(options =>
            {
                options.AddPolicy("grpc", policyBuilder => policyBuilder.Requirements.Add(new GrpcRequirement()));
            });

            #endregion 自定义鉴权

            #region 客户端限流
            //// needed to load configuration from appsettings.json  Build()方法时就已经干了这个事了。
            //context.Services.AddOptions();

            // needed to store rate limit counters and ip rules
            context.Services.AddMemoryCache();

            //load general configuration from appsettings.json
            context.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            //load ip rules from appsettings.json
            context.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            context.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            context.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            context.Services.AddHttpContextAccessor();

            // configuration (resolvers, counter key builders)
            context.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            #endregion

            //context.Services.AddCachePool();//若当前Module未引用NetCoreDomainModule模块,需要单独调用扩展方法执行
            //context.Services.AddRabbitMQ();//使用AbpModule时，间接引用RabbitMQModule,MQ模块会自动执行此扩展方法
            context.Services.ConfigureModuleServices(configuration);//加载自定义插件 并且注册插件中的服务到DI容器
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} End ConfigureServices ....");
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} Start PostConfigureServices ....");
            base.PostConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} End PostConfigureServices ....");
        }

        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} Start OnPreApplicationInitialization  初始化模块RabbitMQEventBusContainer....");
            base.OnPreApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} End OnPreApplicationInitialization 初始化模块RabbitMQEventBusContainer完毕....");
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} Start OnApplicationInitialization ....");

            var app = context.GetApplicationBuilder(); //context.ServiceProvider.GetRequiredService<IObjectAccessor<IApplicationBuilder>>().Value;//
            var env = context.GetEnvironment();//context.ServiceProvider.GetRequiredService<IWebHostEnvironment>();//
            //var configuration = context.GetConfiguration(); //context.ServiceProvider.GetRequiredService<IConfiguration>();//env.GetAppConfiguration();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //ModulePool.Instance.Initialize(app.ApplicationServices);

            //启用限流,需在UseMvc前面
            app.UseIpRateLimiting();

            //插件模块已经添加了生产者到生产者集合并且已经调用autoRegister方法，当前模块如果没用MQ可以不写
            //app.ApplicationServices.GetRequiredService<IRabbitMQEventBusContainer>();
            //var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            //rabbitEventBusContainer.AutoRegister(new[] { typeof(ROServerModule).Assembly });

            app.UseRouting();

            #region 鉴权授权

            app.UseAuthentication();
            app.UseAuthorization();

            #endregion 鉴权授权

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ROService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} End OnApplicationInitialization ....");
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} Start OnPostApplicationInitialization ....");
            base.OnPostApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} End OnPostApplicationInitialization ....");
        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} Start OnApplicationShutdown ....");
            base.OnApplicationShutdown(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ROServerModule)} End OnApplicationShutdown ....");
        }
    }
}
