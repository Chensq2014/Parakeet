using Common.Dtos;
using Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Parakeet.Net.BackgroundWorks
{
    /// <summary>
    /// 数据模拟规则后台任务服务
    /// </summary>
    public class DeviceAnalogRuleBackgroundTaskService : BackgroundService
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private bool _init;

        private readonly IDeviceAnalogRuleAppService _deviceAnalogRuleAppService;
        //private readonly IServiceScopeFactory _serviceScopeFactory;
        public DeviceAnalogRuleBackgroundTaskService(
            ILogger<DeviceAnalogRuleBackgroundTaskService> logger,
            //IServiceScopeFactory serviceScopeFactory,
            IDeviceAnalogRuleAppService deviceAnalogRuleAppService)
        {
            _logger = logger;
            _deviceAnalogRuleAppService = deviceAnalogRuleAppService;
            //_serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_init)
            {
                await Task.Delay(10000, stoppingToken);
                _init = true;
            }

            _timer = new Timer(async func =>
            {
                //using var scope = _serviceScopeFactory.CreateScope();
                //var httpContextFactory =
                //    new DefaultHttpContextFactory(scope.ServiceProvider);
                //var context = httpContextFactory.Create(new FeatureCollection())?.Request?.HttpContext;

                var rules = await _deviceAnalogRuleAppService.GetFilterListAsync(new GetDeviceAnalogRuleBaseInput
                {
                    IsEnabled = true
                });
                var group = rules.ToList().GroupBy(m => m.Device.Type);
                foreach (var g in group)
                {
                    foreach (var rule in g.ToList())
                    {
                        if (DateTime.Now - rule.LastSendTime > rule.Period)
                        {
                            try
                            {
                                //发送一次数据模拟
                                //await StandardApi(rule).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"数据模拟规则调用api出现错误{ex.Message}", ex);

                                //await RedisHelper.RPushAsync(CacheKeys.DeviceForwardRecord, new ForwardRecord(Guid.NewGuid())
                                //{
                                //    Area = rule.Device.ParentArea,
                                //    Code = 500,
                                //    SerialNo = rule.Device.SerialNo,
                                //    FakeNo = rule.Device.FakeNo,
                                //    RecordTime = DateTime.Now,
                                //    Content = JsonConvert.SerializeObject(rule),
                                //    Result = $"{rule.DeviceName}【{rule.Device.FakeNo}】模拟数据失败,详细信息：{ex.Message}-{ex.StackTrace}",
                                //});
                            }
                        }
                    }
                }
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            await Task.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            _timer?.Dispose();  
        }
    }
}
