using Common.Dtos;
using Common.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Parakeet.Net.BackgroundWorks
{
    /// <summary>
    /// 事件消费者后台任务
    /// </summary>

    public class EventConsumerBackgroundWorker : AsyncPeriodicBackgroundWorkerBase
    {
        /// <summary>
        /// 一小时/天的 间隔时间段毫秒数
        /// </summary>
        public const int IntervalSeconds = 3600000;//1h:3600000;//12h 43200000//1d：86400000 

        /// <summary>
        /// IInteractiveAppService interactiveAppService, 
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="serviceScopeFactory"></param>
        public EventConsumerBackgroundWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory, IOptionsMonitor<BackGroundWorkerOptionDto> backGroundOptions) : base(timer, serviceScopeFactory)
        {
            timer.Period = backGroundOptions?.CurrentValue.EventConsumerInterval > 0
                ? backGroundOptions.CurrentValue.EventConsumerInterval
                : IntervalSeconds;//一小时行一次
            //Logger?.LogError($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(EventBackgroundWorker)} 后台任务被构造完毕 {timer.Period}毫秒执行一次 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            Logger.LogDebug($"间隔{Timer.Period}ms 定时执行 {nameof(EventConsumerBackgroundWorker)} 事件处理任务");

            try
            {
                var bgOptions = workerContext.ServiceProvider.GetRequiredService<IOptionsMonitor<BackGroundWorkerOptionDto>>()?.CurrentValue;
                if (bgOptions?.IsEnable == true && bgOptions.IsEnableEvent)
                {
                    //获取服务 执行具体逻辑
                    var testAppService = workerContext.ServiceProvider.GetRequiredService<ITestAppService>();
                    var result = await testAppService.ConsumerEvent();
                    var messages = result.Where(x => x.Code >= 0).SelectMany(y => y.Messages).ToList();
                    if (messages.Any())
                    {
                        Logger.LogInformation($"本次消费处理完毕：{string.Join(";\n", messages)}");
                    }
                }
                else
                {
                    Logger.LogDebug($"后台任务总开关:{bgOptions?.IsEnable}_Event任务开关:{bgOptions?.IsEnableEvent}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            Logger.LogInformation($"{nameof(EventConsumerBackgroundWorker)}启动Start");
            return base.StartAsync(cancellationToken);
        }


        public override Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            Logger.LogInformation($"{nameof(EventConsumerBackgroundWorker)}意外Stop");
            return base.StopAsync(cancellationToken);
        }

    }
}
