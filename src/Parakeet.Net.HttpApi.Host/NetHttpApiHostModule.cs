using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
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
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
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
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureConventionalControllers();
        ConfigureAuthentication(context, configuration);
        ConfigureCache(configuration);
        ConfigureVirtualFileSystem(context);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureDistributedLocking(context, configuration);
        ConfigureCors(context, configuration);
        ConfigureSwaggerServices(context, configuration);
    }

    private void ConfigureCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "net:"; });
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

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata");
                options.Audience = "Parakeet";
            });

        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
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

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
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

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();

        if (CommonConsts.MultiTenancyEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Parakeet API");

            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes("parakeet");
        });

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
