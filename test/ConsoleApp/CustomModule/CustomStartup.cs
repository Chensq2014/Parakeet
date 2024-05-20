using Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp
{
    /// <summary>
    /// 自定义启动类
    /// </summary>
    public class CustomStartup : ICustomStartup
    {
        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }


        /// <summary>
        /// 模块名称
        /// </summary>
        public ICustomModule Module => new CustomModule();


        /// <summary>
        /// CustomStartup
        /// </summary>
        /// <param name="configuration"></param>
        public CustomStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddChongqingModules();
            ////必须加这句
            //ServiceProviderFactory.ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Configure
        /// </summary>
        public void Configure(IServiceProvider serviceProvider)
        {
            //var deviceRepository = ServiceProviderFactory.ServiceProvider.GetRequiredService<IChongqingDeviceMediatorRepository>();
            //var deviceServie = new DeviceMediatorService(deviceRepository);
            //var deviceMediators = deviceServie.Query(new DeviceMediatorQueryFilter()
            //{
            //    Area = new ChongqingModule().Name,
            //    PageNumber = 0,
            //    PageSize = 0
            //});

            //DeviceMediatorPool.Instance.Init(deviceMediators);

            //TODO DO SOME WORK
            //DO SOME WORK...
        }

    }
}
