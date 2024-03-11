using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Storage;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Parakeet.Net.BackgroundWorks
{
    /// <summary>
    /// 清理临时数据的后台定时任务 (单例)
    /// 只需要继承自PeriodicBackgroundWorkerBase或者AsyncPeriodicBackgroundWorkerBase即可 
    ///   //abp继承自PeriodicBackgroundWorkerBase的定时任务 需要程序启动后在OnApplicationInitialization里面这样调用
    /// context.AddBackgroundWorker<ClearTempDataBackgroundWorker>();
    /// </summary>
    public class ClearTempDataBackgroundWorker : PeriodicBackgroundWorkerBase//AsyncPeriodicBackgroundWorkerBase
    {
        //private readonly IInteractiveAppService _interactiveAppService;//依赖注入AppService

        /// <summary>
        /// 一小时/天的 间隔时间段毫秒数
        /// </summary>
        public const int IntervalSeconds = 86400000;//1h:3600000;//12h 43200000//1d：86400000 

        /// <summary>
        /// IInteractiveAppService interactiveAppService, 
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="serviceScopeFactory"></param>
        public ClearTempDataBackgroundWorker(AbpTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer,serviceScopeFactory)
        {
            timer.Period = IntervalSeconds;//一小时/天执行一次 
            //Logger?.LogError($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ClearTempDataBackgroundWorker)} 后台任务被构造完毕 {timer.Period}毫秒执行一次 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// 每隔IntervalSeconds 时间后定时调度的方法
        /// </summary>
        protected override void DoWork(PeriodicBackgroundWorkerContext workerContext)
        {
            //每周末执行1-2次
            var dayOfWeek = DateTime.Now.DayOfWeek;
            if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
            {
                Logger?.LogError($"{{0}}", $"{CacheKeys.LogCount++}、定时清理数据开始 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                //清理数据逻辑
                //_interactiveAppService.ClearDemoData().Wait();
                Logger?.LogError($"{{0}}", $"{CacheKeys.LogCount++}、定时清理数据完成 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
