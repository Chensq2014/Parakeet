using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Parakeet.Net.Controllers
{
    /// <summary>
    /// testController测试
    /// </summary>
    [Route("/api/parakeet/test/[action]")]
    public class TestController : AbpController
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceProvider _serviceProvider;

        public TestController(
            IServiceScopeFactory serviceScopeFactory, 
            IServiceProvider serviceProvider)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _serviceProvider = serviceProvider;
        }

        #region 判断serviceProvider来源
        
        /// <summary> 
        /// 这个框架对controller注册 _scopeFactory 有区别
        /// 新建一个netcore项目做测试
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="scopeFactory"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<bool[]> JudgeScopeCtrl([FromServices]IServiceProvider serviceProvider,[FromServices]IServiceScopeFactory scopeFactory)
        {
            var result = new[]
            {
                ReferenceEquals(_serviceProvider,HttpContext.RequestServices),
                ReferenceEquals(serviceProvider,HttpContext.RequestServices),
                ReferenceEquals(_serviceProvider,serviceProvider),
                ReferenceEquals(_serviceScopeFactory,scopeFactory)
            };
#pragma warning disable CS4014
            Task.Run(async () =>
#pragma warning restore CS4014
            {
                try
                {
                    //_serviceProvider/serviceProvider会被释放 使用_serviceScopeFactory 【测试失败】
                    //IServiceScopeFactory是单例
                    using var scope = _serviceScopeFactory.CreateScope();
                    var sleep = 10 * 1000;
                    Console.WriteLine($"等老子一哈{sleep}ms！");
                    await Task.Delay(sleep);
                    //var locationRepository =
                    //    scope.ServiceProvider.GetRequiredService<IRepository<LocationArea, Guid>>();
                    ////var context = scope.ServiceProvider.GetService<NetCoreDbContext>();
                    ////var location=await context.LocationAreas.FirstOrDefaultAsync();
                    //var location = await locationRepository.FirstOrDefaultAsync();
                    //Console.WriteLine($"查询数据：{location.Name}");

                    var factory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
                    var provider = factory.CreateScope().ServiceProvider;
                    foreach (var propertyInfo in provider.GetType().GetProperties())
                    {
                        Console.WriteLine($"{propertyInfo.Name}");
                    }
                    Console.WriteLine($"_serviceScopeFactory是单例注入获取的唯一实例，它跟随应用程序容器生命周期");
                    Console.WriteLine($"所以只要程序不停止，它都活着，不受当前请求结束的约束，从_serviceScopeFactory随时都可以再创建CreateScope()获取新作用域ServiceProvider,从而获取接口实例");


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Console.WriteLine($"休息好了！");
                }
            });
            
            Console.WriteLine($"返回数据");
            return await Task.FromResult(result);
        }
        #endregion
    }
}
