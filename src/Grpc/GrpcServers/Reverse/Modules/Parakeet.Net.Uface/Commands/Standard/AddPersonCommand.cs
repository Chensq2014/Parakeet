using Common.Dtos;
using Common.Entities;
using Common.Enums;
using Common.Extensions;
using Common.Interfaces;
using Common.RabbitMQModule.Producers;
using Microsoft.Extensions.Logging;
using Parakeet.Net.Equipment;
using Parakeet.Net.Uface.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Parakeet.Net.Uface.Commands.Standard
{
    public class AddPersonCommand : BaseHandlerType, IReverseCommand
    {
        public string Name => EquipmentConstants.ADD_PERSON_FACE_COMMAND;
        public string Area => EquipmentConstants.STANDARD;
        public string SupplierCode => EquipmentConstants.UFACE;

        public override string HandlerType => $"{Area}_{SupplierCode}_{Name}";
        private readonly ILogger<AddPersonCommand> _logger;
        private readonly IProducerContainer _producerContainer;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 10);

        public AddPersonCommand(ILogger<AddPersonCommand> logger, IProducerContainer producerContainer)
        {
            _logger = logger;
            _producerContainer = producerContainer;
        }

        /// <summary>
        /// 同时注册人员和注册图片
        /// </summary>
        /// <param name="device"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<ResponseWrapper<string>> ExecuteAsync(DeviceDto device, string body)
        {
            await _semaphore.WaitAsync();
            try
            {

                _logger.LogInformation($"[宇泛考勤]设备[{device.FakeNo}]正在执行{Name}命令");
                var input = TextJsonConvert.DeserializeObject<AddPersonDto>(body);

                var deviceGroup = $"Device:Array:SerialNo:{device.SerialNo}";
                UfaceExtensions.Delay();//执行前等待，防止并发ticks重复
                var now = DateTime.Now;
                var ticks = now.ToUnixTimeTicks(13);//13位时间戳精确到毫秒
                var addPerson = new
                {
                    taskNo = ticks.ToString(),
                    interfaceName = "person/create",//向设备注册人员 api名称
                    result = true,
                    person = new
                    {
                        name = input.Name,
                        iDNumber = input.IdCard,
                        id = input.PersonnelId,//非必填 只允许数字和英文字母 长度255
                        idCardNum = input.IcCard,//非必填
                                                 //facePermission = input.FacePermission,//非必填
                                                 //idCardPermission = input.IdCardPermission,//非必填
                                                 //faceAddCardPermission = input.FaceAddCardPermission,//非必填
                                                 //IDPermission = input.IDPermission//非必填
                    }
                };

                var taskJson = TextJsonConvert.SerializeObject(addPerson);
                if (!string.IsNullOrWhiteSpace(taskJson))
                {
                    #region 下发图片之后再发feedback
                    ////消息反馈,第一次反馈不要添加Type，因为当前还不知晓Type类型
                    //var entity = new FeedbackBase(Guid.NewGuid())
                    //{
                    //    FakeNo = device.FakeNo,
                    //    SerialNo = device.SerialNo,
                    //    Message = "",
                    //    HandlerType = PersonnelOperatorEnum.Register.ToInt(),
                    //    PersonnelId = input.PersonnelId,
                    //    SequenceNo = ticks,
                    //    RecordTime = DateTime.Now
                    //};

                    //var wrapperData = new WrapperData<FeedbackBase>
                    //{
                    //    Success = true,
                    //    Code = 0,
                    //    Data = entity
                    //};

                    //var producer = _producerContainer.GetProducer(Area, AppConstants.GATE_EXCHANGE);
                    //await producer.PublishAsync(wrapperData, $"{Area}.{AppConstants.GATE}.{AppConstants.FEEDBACK_HANDLER_TYPE}").ConfigureAwait(false);
                    //await Task.Delay(500);
                    #endregion

                    //将json字符串taskJson从左push到队列数组 deviceKeyGroup
                    await RedisHelper.LPushAsync(deviceGroup, taskJson);
                    var producer = _producerContainer.GetProducer(Area, EquipmentConstants.GATE_EXCHANGE);
                    var feedBackFirstTime = true;
                    foreach (var imageModel in input.Faces)
                    {
                        var imagBase64 = imageModel.Image?.Split(",").LastOrDefault();
                        var addPersonFace = new
                        {
                            taskNo = (++ticks).ToString(),
                            interfaceName = "face/create",//向设备已注册人员添加人脸的api名称
                            personId = input.PersonnelId,
                            faceId = imageModel.FaceId ?? input.PersonnelId,//GuidGenerator.Create().ToString("N"),
                            isEasyWay = false,
                            result = true,
                            imgBase64 = imagBase64
                        };
                        var addImageJson = TextJsonConvert.SerializeObject(addPersonFace);
                        if (!string.IsNullOrWhiteSpace(addImageJson) && feedBackFirstTime)
                        {
                            //消息反馈,第一次反馈不要添加Type，因为当前还不知晓Type类型
                            #region 添加成功图片时发送FeedBack
                            //消息反馈,第一次反馈不要添加Type，因为当前还不知晓Type类型
                            var entity = new FeedbackDto
                            {
                                Id = Guid.NewGuid(),
                                Device = device,
                                //FakeNo = device.FakeNo,
                                //SerialNo = device.SerialNo,
                                Message = "",
                                HandlerType = FeedbackHandlerType.Register,
                                PersonnelId = input.PersonnelId,
                                SequenceNo = ticks,
                                RecordTime = now
                            };
                            var wrapperData = new WrapperData<FeedbackDto>
                            {
                                Success = true,
                                Code = 0,
                                Data = entity
                            };
                            await producer.PublishAsync(wrapperData, $"{Area}.{EquipmentConstants.GATE}.{EquipmentConstants.FEEDBACK_HANDLER_TYPE}").ConfigureAwait(false);
                            #endregion
                            feedBackFirstTime = false;
                            //将json字符串taskJson从左push到队列数组 deviceKeyGroup
                            await RedisHelper.LPushAsync(deviceGroup, addImageJson);
                            _logger.LogInformation($"[宇泛考勤]设备[{device.FakeNo}]执行{addPersonFace.interfaceName}命令完毕,{input.Name}_{input.IdCard}_SequenceNo:{ticks}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"错误：{ex.Message}");
                return ResponseWrapper.Error(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
            return ResponseWrapper.Succeed($"设备[{device.FakeNo}]执行{Name}命令完毕");
        }
    }
}