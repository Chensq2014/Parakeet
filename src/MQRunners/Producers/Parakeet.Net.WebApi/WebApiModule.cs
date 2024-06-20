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
        /// ����Swagger
        /// </summary>
        /// <param name="context"></param>
        private void ConfigureSwaggerServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(WebApiModule)} Start  ConfigureServices ....");
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��ConfigureSwaggerServices...ConfigureServices�е�������־�߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
            var apiSecurityScheme = new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            };


            //ע��Swagger������������һ���Ͷ��Swagger �ĵ�
            context.Services.AddSwaggerGen(
                options =>
                {
                    //swagger ��汾֧��
                    Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��AddSwaggerGen����{nameof(SwaggerGenOptions)} :{apiSecurityScheme.Name} ע��Swagger������������һ���Ͷ��Swagger �ĵ� ��汾֧��....ConfigureServices�е�{options.GetType().Name}ί����־ �߳�Id����{Thread.CurrentThread.ManagedThreadId}��");
                    typeof(VersionType).GetEnumNames().ToList().ForEach(v =>
                    {
                        options.SwaggerDoc(v, new OpenApiInfo { Title = $"NetCore WebAPI {v}", Version = v });
                    });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                    //���²��������Զ�����չ �����Դ�ļ���...
                    //options.OperationFilter<SwaggerFileUploadFilter>(); //֧��swagger�ϴ��ļ�(��װһ��header) �°汾Ĭ��֧��
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
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(WebApiModule)} End  ConfigureServices ....");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(WebApiModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration(); //env.GetAppConfiguration();

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�� ��ȡIRabbitMQEventBusContainer Ȼ��AutoRegister �Զ�ע�Ტ�󶨵�ǰģ�������/������....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(WebApiModule).Assembly });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSwagger(option =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��UseSwagger {nameof(SwaggerOptions)}....");
                //option.RouteTemplate = "api-doc/{documentName}/swagger.json";//���ú�������շ���·���;��� /api-doc/index.html
            });
            app.UseSwaggerUI(options =>
            {
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}��UseSwaggerUI {nameof(SwaggerUIOptions)}....");
                typeof(VersionType).GetEnumNames().ToList().ForEach(v =>
                {
                    //configuration["App:SwaggerEndpoint"]="/swagger/v1/swagger.json";
                    options.SwaggerEndpoint(string.Format(configuration["App:SwaggerEndpoint"], v) ?? $"/swagger/{v}/swagger.json", $"С���Ĺ����� NetCore WebAPI {v}");
                });
                //options.InjectJavascript("/swagger/ui/zh_CN.js"); // �������İ� ��Ч
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
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(WebApiModule)} End  OnApplicationInitialization ....");
        }
    }
}
