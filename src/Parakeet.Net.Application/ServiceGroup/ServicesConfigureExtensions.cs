using Common.Dtos;
using Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Filters;
using Parakeet.Net.LocationAreas;
using Parakeet.Net.ServiceGroup.AlibabaSdk;
using Parakeet.Net.ServiceGroup.Esign;
using Parakeet.Net.ServiceGroup.JianWei;
using Parakeet.Net.ServiceGroup.Sign;
using Serilog;

namespace Parakeet.Net.ServiceGroup
{
    /// <summary>
    /// IServiceCollection 静态扩展 配置微服务或节点类
    /// </summary>
    public static class ServicesConfigureExtensions
    {
        /// <summary>
        /// 配置微服务http调用的公共静态方法
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterHttpsApi(this IServiceCollection services)
        {
            var configuration = services.GetConfiguration();

            #region 配置反射类+微服务及节点
            ////配置反射类
            //services.Configure<DemoConfig>(configuration.GetSection("DemoConfig"));
            //将指定节点Json格式数据转为一个ESignOption对象
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、配置ESignOption -->App:MicroServices:CertificateServer:Option:{configuration.GetSection("App:MicroServices:CertificateServer:Option")}");
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、配置SignOption -->App:MicroServices:SignServer:Option:{configuration.GetSection("App:MicroServices:SignServer:Option")}");
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、配置RecordConfig -->RecordConfig:{configuration.GetSection("RecordConfig")}");
            services.Configure<ESignOption>(configuration.GetSection("App:MicroServices:CertificateServer:Option"));
            services.Configure<SignOption>(configuration.GetSection("App:MicroServices:SignServer:Option"));
            services.Configure<ChongqingJianWeiOption>(configuration.GetSection("App:MicroServices:ChongqingJianWei:Option"));
            services.Configure<GatewayKeySecretOptionDto>(configuration.GetSection("App:MicroServices:ROClient:Option"));
            services.Configure<ROClientOptionDto>(configuration.GetSection("App:MicroServices:ROClient:Option"));
            services.Configure<AlibabaSdkOption>(configuration.GetSection("App:MicroServices:AlibabaSdk:Option"));
            services.Configure<RecordConfig>(configuration.GetSection("RecordConfig"));


            services.Configure<WeixinOptionDto>(configuration.GetSection(WeixinOptionDto.ConfigKey));

            ////使用WebApiClient扩展调用微服务接口前配置Header的Token,扩展自ApiActionFilter属性
            ////加入全局过滤器，微服务接口调用api前都要经过此属性设置Header...
            //var defaultServiceTokenAttribute = new DefaultServiceTokenAttribute();//应该是所有服务共用token
            ////DFS分布式文件服务接口访问Host,时间,全局过滤Header配置
            //Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、分布式文件服务接口访问Host,时间,全局过滤Header配置 Host:{configuration.GetValue<string>("RemoteServices:DFS:BaseUrl")}");
            //services.AddHttpApi<IFileAppService>()
            //    .ConfigureHttpApiConfig(c =>
            //    {
            //        c.HttpHost = new Uri(configuration.GetValue<string>("RemoteServices:DFS:BaseUrl"));
            //        c.FormatOptions.DateTimeFormat = CommonConsts.DateTimeFormatString;
            //        c.GlobalFilters.Add(defaultServiceTokenAttribute);
            //    });

            ////重庆建委接口url，过滤器配置
            //services.AddHttpApi<IChongqingJianWeiApi>()
            //    .ConfigureHttpApiConfig(c =>
            //    {
            //        Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、重庆建委1.0接口IChongqingJianWeiApi，host 过滤器等配置 Host:{configuration.GetSection("App:MicroServices:ChongqingJianWei:ServerRootAddress")}");
            //        c.HttpHost = new Uri(configuration.GetValue<string>("App:MicroServices:ChongqingJianWei:ServerRootAddress"));
            //        c.FormatOptions.DateTimeFormat = CommonConsts.DateTimeFormatString;
            //        c.GlobalFilters.Add(new ChongqingJianWeiAppKeyAttribute());
            //    });

            ////金格签章接口url，过滤器配置 服务端是金格的免费测试接口，可以使用
            //services.AddHttpApi<ISignerApi>()
            //    .ConfigureHttpApiConfig(c =>
            //    {
            //        Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、金格签章接口ISignerApi，host 过滤器等配置 Host:{configuration.GetSection("App:MicroServices:SignServer:ServerRootAddress")}");
            //        c.HttpHost = new Uri(configuration.GetValue<string>("App:MicroServices:SignServer:ServerRootAddress"));
            //        c.FormatOptions.DateTimeFormat = CommonConsts.DateTimeFormatString;
            //        //c.GlobalFilters.Add(defaultServiceTokenAttribute);//金格api请求可以不在Header添加token
            //        c.GlobalFilters.Add(new SignAppKeyAttribute());
            //    });

            ////E签章接口url，过滤器配置 这个服务端需要一个docker来发布 暂时禁用
            //services.AddHttpApi<IESignApi>()
            //    .ConfigureHttpApiConfig(c =>  {
            //        c.HttpHost = new Uri(configuration.GetValue<string>("App:MicroServices:ESignServer:ServerRootAddress"));
            //        c.FormatOptions.DateTimeFormat = CommonConsts.DateTimeFormatString;
            //        c.GlobalFilters.Add(defaultServiceTokenAttribute);
            //        c.GlobalFilters.Add(new ESignAppKeyAttribute());
            //    });

            #region 扩展写法 没必要
            //void ConfigOption(HttpApiConfig config,List<ApiActionFilterAttribute> attributes, string nodeAddress)
            //{
            //    config.HttpHost = new Uri(configuration.GetValue<string>("RemoteServices:DFS:BaseUrl"));
            //    config.FormatOptions.DateTimeFormat = CommonConsts.DateTimeFormatString;
            //    attributes.ForEach(config.GlobalFilters.Add);
            //}
            //E签章接口url，过滤器配置
            //context.Services.AddHttpApi<IESignApi>()
            //    .ConfigureHttpApiConfig(c => ConfigOption(c, 
            //        new List<ApiActionFilterAttribute>
            //        {
            //            defaultServiceTokenAttribute,
            //            new ESignAppKeyAttribute()
            //        },
            //        "App:MicroServices:ESignServer:ServerRootAddress"));
            #endregion

            #endregion


            #region 过滤器
            //// MVC全局过滤器
            //services.AddMvc(
            //    //o.Filters.Add<CustomExceptionFilterAttribute>();//异常处理全局filter
            //    //o.Filters.Add(typeof(CustomGlobalActionFilterAttribute));//全局注册filter
            //    //options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())
            //);

            //如果不把这个放到全局过滤器中，就要手动去注入
            //注册异常过滤器 它里面的构造函数可以依赖注入 用容器生成的这个实例
            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、注册全局自定义异常过滤器，监控第三方api错误消息");
            services.AddScoped(typeof(CustomExceptionFilterAttribute));


            #endregion


            return services;
        }

    }
}
