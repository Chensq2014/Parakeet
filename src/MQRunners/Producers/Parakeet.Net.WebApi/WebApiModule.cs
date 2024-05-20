using Common.Enums;
using Common.RabbitMQModule.Core;
using Common.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Parakeet.Net.Producer;
using Parakeet.Net.WebApi.Extentions;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.Net.WebApi
{
    [DependsOn(typeof(ProducerModule))]
    public class WebApiModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //context.Services.AddControllers();
            ConfigureSwaggerServices(context);
            base.ConfigureServices(context);
        }

        /// <summary>
        /// 配置Swagger
        /// </summary>
        /// <param name="context"></param>
        private void ConfigureSwaggerServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(WebApiModule)} Start  ConfigureServices ....");
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、ConfigureSwaggerServices...ConfigureServices中的流程日志线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            var apiSecurityScheme = new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            };


            //注册Swagger生成器，定义一个和多个Swagger 文档
            context.Services.AddSwaggerGen(
                options =>
                {
                    //swagger 多版本支持
                    Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddSwaggerGen配置{nameof(SwaggerGenOptions)} :{apiSecurityScheme.Name} 注册Swagger生成器，定义一个和多个Swagger 文档 多版本支持....ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                    typeof(VersionType).GetEnumNames().ToList().ForEach(v =>
                    {
                        options.SwaggerDoc(v, new OpenApiInfo { Title = $"NetCore WebAPI {v}", Version = v });
                    });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                    //以下部分属于自定义扩展 添加资源文件及...
                    //options.OperationFilter<SwaggerFileUploadFilter>(); //支持swagger上传文件(包装一下header) 新版本默认支持
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
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(WebApiModule)} End  ConfigureServices ....");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(WebApiModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration(); //env.GetAppConfiguration();

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、 获取IRabbitMQEventBusContainer 然后AutoRegister 自动注册并绑定当前模块的生产/消费者....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(WebApiModule).Assembly });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSwagger(option =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、UseSwagger {nameof(SwaggerOptions)}....");
                //option.RouteTemplate = "api-doc/{documentName}/swagger.json";//配置后，你的最终访问路径就就是 /api-doc/index.html
            });
            app.UseSwaggerUI(options =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、UseSwaggerUI {nameof(SwaggerUIOptions)}....");
                typeof(VersionType).GetEnumNames().ToList().ForEach(v =>
                {
                    //configuration["App:SwaggerEndpoint"]="/swagger/v1/swagger.json";
                    options.SwaggerEndpoint(string.Format(configuration["App:SwaggerEndpoint"], v) ?? $"/swagger/{v}/swagger.json", $"小鹦鹉工作室 NetCore WebAPI {v}");
                });
                //options.InjectJavascript("/swagger/ui/zh_CN.js"); // 加载中文包 无效
                //options.ShowExtensions();
                //options.DocExpansion(DocExpansion.None);
                //options.RoutePrefix = "api-doc";
            });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            app.UseConfiguredEndpoints();
            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(WebApiModule)} End  OnApplicationInitialization ....");
        }
    }
}
