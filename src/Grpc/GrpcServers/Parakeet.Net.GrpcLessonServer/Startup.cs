using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Parakeet.Net.GrpcLessonServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<GrpcLessonModule>();
        }

        /// <summary>
        /// 配置管道
        /// 基于IApplicationBuilderFactory创建IApplicationBuilder对象
        /// 基于IStartupFilter的管道构建
        /// 调用IApplicationBuilder对象的Build方法完成完整的管道
        /// </summary>
        /// <param name="app"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            ILogger<Startup> logger = loggerFactory.CreateLogger<Startup>();
            logger.LogDebug($"{nameof(Startup)}默认配置管道开始.....");
            //app.InitializeApplication();//todo:grpc 需要启动模块引用AbpAspNetCoreMvcModule才可以这样用

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            #region 鉴权授权
            app.UseAuthentication();//鉴权 User赋值
            app.UseAuthorization();//判断User是否有claims等
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<LessonService>();

                endpoints.MapGet("/", async ctx =>
                {
                    await ctx.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

            Log.Logger.Information($"{nameof(Startup)}默认配置管道结束.....");
        }
    }
}
