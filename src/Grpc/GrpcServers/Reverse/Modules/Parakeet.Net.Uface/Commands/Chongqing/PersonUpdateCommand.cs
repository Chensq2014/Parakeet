using Common.Dtos;
using Common.Entities;
using Common.Enums;
using Common.Extensions;
using Common.Interfaces;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using Parakeet.Net.Equipment;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Parakeet.Net.Uface.Commands.Chongqing
{
    public class PersonUpdateCommand : BaseHandlerType, IReverseCommand
    {
        private readonly ILogger _logger;
        //private readonly IProducerContainer _producerContainer;
        //private readonly TenantConnectStringPool _tenantConnectStringPool;
        //允许10个线程调用，当1个线程进入就开启线程锁
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 10);

        public string Name => EquipmentConstants.UPDATE_PERSON_COMMAND;
        public string Area => EquipmentConstants.CHONGQING;
        public string SupplierCode => EquipmentConstants.UFACE;

        public override string HandlerType => $"{Area}_{SupplierCode}_{Name}";

        public PersonUpdateCommand(
            //TenantConnectStringPool tenantDbConnectionStringPool,
            //IProducerContainer producerContainer, 
            ILogger<PersonUpdateCommand> logger)
        {
            _logger = logger;
            //_producerContainer = producerContainer;
            //_tenantConnectStringPool = tenantDbConnectionStringPool;
            //_semaphore = new SemaphoreSlim(1,1);
        }

        /// <summary>
        /// 同时注册人员和注册图片
        /// </summary>
        /// <param name="device"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<ResponseWrapper<string>> ExecuteAsync(DeviceDto device, string body)
        {
            _logger.LogInformation($"[杭州宇泛]设备[{device.FakeNo}]正在执行{Name}命令");
            await _semaphore.WaitAsync();
            try
            {
                if (device.Type != DeviceType.Gate)
                {
                    return ResponseWrapper.Error($"[杭州宇泛]设备[{device.FakeNo}]非闸机设备,无法进行下发操作", 500);
                }

                //string key = string.Format("keyPrefix", device.FakeNo);
                //var latestOnline = await RedisHelper.GetAsync<DateTime?>(key);
                //if (latestOnline == null || (DateTime.Now - latestOnline.Value).TotalMinutes > 5)
                //{
                //    return ResponseWrapper.Error($"[杭州宇泛]设备[{device.FakeNo}]执行{Name}命令失败,设备已掉线", 405);
                //}

                var input = TextJsonConvert.DeserializeObject<AddPersonDto>(body);

                var deviceGroup = $"Device:Array:SerialNo:{device.SerialNo}";
                var now = DateTime.Now;
                var ticks = now.ToUnixTimeTicks(13);
                var updatePersonTask = new
                {
                    taskNo = ticks.ToString(),
                    interfaceName = "person/update",
                    result = true,
                    person = new
                    {
                        id = input.PersonnelId,//必填 只允许数字和英文字母 长度255
                        name = input.Name,
                        idNumber = input.IdCard,
                        idCardNum = input.IcCard,
                    }
                };

                var updatePersonTaskJson = TextJsonConvert.SerializeObject(updatePersonTask);
                if (!string.IsNullOrWhiteSpace(updatePersonTaskJson))
                {
                    using (var connection = new NpgsqlConnection("connectString"))//_tenantConnectStringPool[device.AreaTenantId.Value, true].Value)
                    {
                        connection.Open();
                        //将更新人员任务添加至队列
                        await RedisHelper.LPushAsync(deviceGroup, updatePersonTaskJson);
                        _logger.LogInformation($"[杭州宇泛]设备[{device.FakeNo}]执行{Name}命令,{input.Name}_{input.IdCard}_SequenceNo:{ticks}");

                        //await Task.Delay(1000);

                        //循环人脸图片
                        foreach (var imageModel in input.Faces)
                        {
                            ticks++;
                            var entity = new FeedbackDto
                            {
                                Id = Guid.NewGuid(),
                                Device = device,
                                //FakeNo = device.FakeNo,
                                //SerialNo = device.SerialNo,
                                Message = "",
                                //此处使用Register而非Update,否则无法找到对应的HandlerType更新
                                HandlerType = FeedbackHandlerType.Register,
                                PersonnelId = input.PersonnelId,
                                SequenceNo = ticks,
                                RecordTime = now
                            };

                            connection.Execute("INSERT INTO \"T_Feedbacks\" (\"Id\", \"SerialNo\", \"CreationTime\", \"IsDeleted\", \"RecordTime\", \"Type\", \"Message\", \"FakeNo\", \"SequenceNo\", \"PersonnelId\", \"HandlerType\", \"Code\") VALUES (@Id,@SerialNo,@CreationTime,@IsDeleted,@RecordTime,@Type,@Message,@FakeNo,@SequenceNo,@PersonnelId,@HandlerType,@Code)", entity);

                            var imagBase64 = imageModel.Image.Split(",").LastOrDefault();
                            var addPersonFace = new
                            {
                                taskNo = ticks.ToString(),
                                interfaceName = "face/update",
                                personId = input.PersonnelId,
                                faceId = imageModel.FaceId ?? input.PersonnelId,
                                isEasyWay = true,//非必传，默认 false：严格检测照片质量；true：宽松检测照片质量
                                result = true,
                                imgBase64 = imagBase64
                            };

                            var addImageJson = TextJsonConvert.SerializeObject(addPersonFace);
                            if (!string.IsNullOrWhiteSpace(addImageJson))
                            {
                                //将人脸更新任务添加至队列
                                await RedisHelper.LPushAsync(deviceGroup, addImageJson);
                                _logger.LogInformation($"[杭州宇泛]设备[{device.FakeNo}]执行{addPersonFace.interfaceName}命令,{input.Name}_{input.IdCard}_SequenceNo:{ticks}");
                            }
                        }
                    }

                    return ResponseWrapper.Succeed($"[杭州宇泛]设备[{device.FakeNo}]执行{Name}命令成功");
                }

                _logger.LogError($"[杭州宇泛]设备{device.FakeNo}序列化updatePersonTaskJson-{input.Name}失败");

                return ResponseWrapper.Succeed($"设备[{device.FakeNo}]执行{Name}命令完毕");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"设备[{device.FakeNo}]执行{Name}命令失败:{ex.Message}");
                return ResponseWrapper.Error($"设备[{device.FakeNo}]执行{Name}命令失败:{ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
