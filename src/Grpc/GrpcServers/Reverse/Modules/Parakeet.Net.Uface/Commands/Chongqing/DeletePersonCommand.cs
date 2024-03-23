using Common.Dtos;
using Common.Entities;
using Common.Enums;
using Common.Extensions;
using Common.Interfaces;
using Common.RabbitMQModule.Producers;
using Microsoft.Extensions.Logging;
using Parakeet.Net.Equipment;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Parakeet.Net.Uface.Commands.Chongqing
{
    /// <summary>
    /// 从设备删除人员
    /// </summary>
    public class DeletePersonCommand : BaseHandlerType, IReverseCommand
    {
        private readonly ILogger<DeletePersonCommand> _logger;

        private readonly IProducerContainer _producerContainer;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 10);
        public string Name => EquipmentConstants.DELETE_PERSON_COMMAND;
        public string Area => EquipmentConstants.CHONGQING;
        public string SupplierCode => EquipmentConstants.UFACE;
        public override string HandlerType => $"{Area}_{SupplierCode}_{Name}";

        public DeletePersonCommand(ILogger<DeletePersonCommand> logger, IProducerContainer producerContainer)
        {
            _logger = logger;
            _producerContainer = producerContainer;
        }
        public async Task<ResponseWrapper<string>> ExecuteAsync(DeviceDto device, string body)
        {
            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation($"[宇泛]设备[{device.SerialNo}]正在执行{Name}命令");

                var input = TextJsonConvert.DeserializeObject<DeletePersonDto>(body);
                var id = string.Join(",", input.PersonnelIds);
                var deviceGroup = $"device_{device.SerialNo}";
                //UfaceExtensions.Delay();//执行前等待，防止并发ticks重复 使用信号量代替
                var now = DateTime.Now;
                var ticks = now.ToUnixTimeTicks(13);//13位时间戳精确到毫秒
                var deletePerson = new
                {
                    taskNo = ticks.ToString(),
                    interfaceName = "person/delete",//向设备注册人员 api名称
                    id = id,
                    result = true
                };
                var taskJson = TextJsonConvert.SerializeObject(deletePerson);
                if (taskJson.HasValue())
                {
                    //消息反馈,第一次反馈不要添加Type，因为当前还不知晓Type类型
                    var entity = new FeedbackDto
                    {
                        Id = Guid.NewGuid(),
                        Device = device,
                        //FakeNo = device.FakeNo,
                        //SerialNo = device.SerialNo,
                        Message = "",
                        HandlerType = FeedbackHandlerType.Delete,
                        PersonnelId = id,
                        SequenceNo = ticks,
                        RecordTime = now
                    };

                    var wrapperData = new WrapperData<FeedbackDto>
                    {
                        Success = true,
                        Code = 0,
                        Data = entity,
                    };

                    var producer = _producerContainer.GetProducer(Area, EquipmentConstants.GATE_EXCHANGE);
                    await producer.PublishAsync(wrapperData, $"{Area}.{EquipmentConstants.GATE}.{EquipmentConstants.FEEDBACK_HANDLER_TYPE}").ConfigureAwait(false);
                    await Task.Delay(1000);
                    //将json字符串taskJson从左push到队列数组 deviceKeyGroup
                    await RedisHelper.LPushAsync(deviceGroup, taskJson);
                }
                _logger.LogInformation($"[宇泛]设备[{device.SerialNo}]执行{Name}命令完毕,人员:[{id}]已推送");
            }
            catch (Exception e)
            {
                _logger.LogError($"错误：{e.Message}");
                return ResponseWrapper.Error(e.Message);
            }
            finally
            {
                _semaphore.Release();
            }
            return ResponseWrapper.Succeed($"设备[{device.FakeNo}]执行{Name}命令完毕");
        }
    }
}
