using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Storage;

namespace Parakeet.Net.BackgroundWorks
{
    /// <summary>
    /// 自定义后台任务 BackgroundService基于IHostedService
    /// </summary>
    public class CustomBackgroundJob : BackgroundService
    {
        private IServiceProvider _serviceProvider;
        private IServiceScopeFactory _serviceScopeFactory;
        public CustomBackgroundJob(IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = serviceScopeFactory;
        }

        private Timer _timer;
        //    =new Timer(m =>
        //{
        //    //定期执行逻辑
        //    Log.Logger.Information($"定期执行逻辑:{nameof(CustomBackgroundJob)}执行{DateTime.Now:yyyy/MM/dd}{m?.ToString()}");
        //}, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

        /// <summary>
        /// 持续运行的后台任务执行方法
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(async m =>
            {
                //定期执行逻辑
                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、定期执行逻辑:{nameof(CustomBackgroundJob)} BackgroundService执行{DateTime.Now:yyyy/MM/dd}{m?.ToString()} 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

                //var httpContext =_serviceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;
                //using  var scope = _serviceScopeFactory.CreateScope();
                //var httpContext = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;

                await Task.CompletedTask;
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            await Task.CompletedTask;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            _timer?.Dispose();
        }
    }
}
