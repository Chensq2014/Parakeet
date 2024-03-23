using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;
using Volo.Abp.Modularity.PlugIns;

namespace Parakeet.Net.ROServer
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            //CustomConfigurationManager.Init(env);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //var configuration = services.GetConfiguration();

            #region 客户端限流

            //// needed to load configuration from appsettings.json
            //services.AddOptions();
            //// needed to store rate limit counters and ip rules
            //services.AddMemoryCache();

            ////load general configuration from appsettings.json
            //services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            ////load ip rules from appsettings.json
            //services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

            //// inject counter and rules stores
            //services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            //services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            //services.AddHttpContextAccessor();

            //// configuration (resolvers, counter key builders)
            //services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            #endregion

            //services.AddGrpc();

            #region 自定义鉴权

            //services.AddSingleton<IAuthenticationHandler, GrpcAuthticateHandler>();
            //services.AddSingleton<IAuthorizationHandler, GrpcRequireHandler>();

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = "grpc";
            //    options.DefaultSignInScheme = "grpc";
            //    options.DefaultChallengeScheme = "grpc";
            //});
            //services.AddAuthenticationCore(options => options.AddScheme<GrpcAuthticateHandler>("grpc", "grpc"));

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("grpc", policyBuilder => policyBuilder.Requirements.Add(new GrpcRequirement()));
            //});

            #endregion 自定义鉴权

            #region MQ及缓存配置

            //services.AddCachePool();
            //services.AddRabbitMQ();

            #endregion

            ////自定义模块注册
            //services.ConfigureModuleServices(services.GetConfiguration());

            //服务注册配置等 交给启动Module
            services.AddApplication<ROServerModule>(options =>
            {
                //加载Plugins里的dll插件
                var baseDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var path = $"{baseDirectory}/Plugins";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                options.PlugInSources.Add(new FolderPlugInSource(path, SearchOption.AllDirectories));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            #region 交给启动模块AbpModule 初始化
            Log.Logger.Information($"{nameof(Startup)}默认配置管道开始.....");
            app.InitializeApplication();
            Log.Logger.Information($"{nameof(Startup)}默认配置管道结束.....");
            #endregion

            #region 非Abp启动代码

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //启用限流,需在UseMvc前面
            //app.UseIpRateLimiting();

            //app.UseRouting();

            #region 鉴权授权

            //app.UseAuthentication();
            //app.UseAuthorization();

            #endregion 鉴权授权

            #region 设置终结点

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGrpcService<GreeterService>();
            //    endpoints.MapGrpcService<ROService>();

            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            //    });
            //});
            #endregion


            #endregion

            ////自定义模块初始化
            //CustomModulePool.Instance.Initialize(app.ApplicationServices);
        }
    }
}
