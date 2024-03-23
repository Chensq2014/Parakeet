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

namespace Parakeet.Net.Register.Commands.Chongqing
{
    public class PersonRegisterCommand : BaseHandlerType, IReverseCommand
    {
        public string Name => EquipmentConstants.REGISTER_PERSON_COMMAND;
        public string Area => EquipmentConstants.CHONGQING;
        public string SupplierCode => EquipmentConstants.REGISTER;

        public override string HandlerType => $"{Area}_{SupplierCode}_{Name}";

        private readonly IProducerContainer _producerContainer;
        private readonly ILogger<PersonRegisterCommand> _logger;
        //允许10个线程调用，当1个线程进入就开启线程锁
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 10);

        public PersonRegisterCommand(IProducerContainer producerContainer, ILogger<PersonRegisterCommand> logger)
        {
            _producerContainer = producerContainer;
            _logger = logger;
        }

        public async Task<ResponseWrapper<string>> ExecuteAsync(DeviceDto device, string body)
        {
            await _semaphore.WaitAsync();//只允许当前线程执行完毕之后，下一线程才开始
            try
            {
                var commandBody = TextJsonConvert.DeserializeObject<PersonRegisterDto>(body);
                commandBody.PersonnelId ??= Guid.NewGuid().ToString("N");
                var worker = new WorkerDto
                {
                    Id = Guid.NewGuid(),
                    IdCard = commandBody.IdCard,
                    Name = commandBody.Name,
                    Gender = commandBody.Gender,
                    Nation = commandBody.Nation,
                    Birthday = commandBody.Birthday,
                    Address = commandBody.Address,
                    IssuedBy = commandBody.IssuedBy,
                    TermValidityStart = commandBody.TermValidityStart,
                    TermValidityEnd = commandBody.TermValidityEnd,
                    IdPhoto = commandBody.IdPhoto,
                    Photo = commandBody.Photo,
                    InfraredPhoto = commandBody.InfraredPhoto,
                    IdPhotoUrl = commandBody.IdPhotoUrl,
                    PhotoUrl = commandBody.PhotoUrl,
                    InfraredPhotoUrl = commandBody.InfraredPhotoUrl,
                    PhoneNumber = commandBody.PhoneNumber
                };

                _logger.LogDebug($"[重庆采集]设备[{device.FakeNo}]正在注册人员{worker.Name}_{commandBody.PersonnelId}...");

                var entity = new DeviceWorkerDto
                {
                    Id = Guid.NewGuid(),
                    Device = device,
                    //FakeNo = device.FakeNo,
                    //SerialNo = device.SerialNo,
                    IcCard = commandBody.IcCard,
                    PersonId = commandBody.PersonnelId,
                    PersonnelType = commandBody.PersonnelType.ToEnum<StaffType>(),
                    RegisterType = commandBody.RegisterType.ToEnum<RegType>(),
                    RecordTime = DateTime.Now,
                    //CreationTime = DateTime.Now,
                    //Name = commandBody.Name,
                    //IdCard = commandBody.IdCard,
                    WorkerNo = commandBody.WorkerNo,
                    PhoneNumber = commandBody.PhoneNumber,
                    GroupLeader = commandBody.GroupLeader,
                    Marital = commandBody.Marital,
                    PoliticsType = commandBody.PoliticsType,
                    IsJoin = commandBody.IsJoin,
                    JoinTime = commandBody.JoinTime,
                    Education = commandBody.Education,
                    HasBadMedicalHistory = commandBody.HasBadMedicalHistory,
                    IsSpecial = commandBody.IsSpecial,
                    CorpId = commandBody.CorpId,
                    CorpCode = commandBody.CorpCode,
                    CorpName = commandBody.CorpName,
                    WorkerTypeId = commandBody.WorkerTypeId,
                    WorkerTypeCode = commandBody.WorkerTypeCode,
                    WorkerTypeName = commandBody.WorkerTypeName,
                    WorkPostId = commandBody.WorkPostId,
                    WorkPostCode = commandBody.WorkPostCode,
                    WorkPostName = commandBody.WorkPostName,
                    WorkerGroupId = commandBody.WorkerGroupId,
                    WorkerGroupCode = commandBody.WorkerGroupCode,
                    WorkerGroupName = commandBody.WorkerGroupName,
                    WorkerId = worker.Id,
                    Worker = worker
                };

                var wrapperData = new WrapperData<DeviceWorkerDto>
                {
                    Data = entity,
                };
                //await Task.Delay(new Random().Next(1000, 3000));//在这里等待1-3s即可 自动下发的地方设置了等待1s，注册不需要，浪费性能
                //_logger.LogInformation($"grpc 注册数据的exchange{AppConstants.GATE_EXCHANGE},routKey:{Area}.{AppConstants.GATE}.{AppConstants.REGISTER_HANDLER_TYPE}");
                var producer = _producerContainer.GetProducer(Area, EquipmentConstants.GATE_EXCHANGE);
                await producer.PublishAsync(wrapperData,
                    $"{EquipmentConstants.CHONGQING}.{EquipmentConstants.GATE}.{EquipmentConstants.REGISTER_HANDLER_TYPE}"); //.ConfigureAwait(false);
                _logger.LogInformation(
                    $"[重庆采集]设备[{device.FakeNo}]执行{Name}命令完毕,人员[{commandBody.Name}]-[{commandBody.IdCard}]-[{commandBody.PersonnelId}]");

                return ResponseWrapper.Succeed($"设备[{device.FakeNo}]执行{Name}命令完毕");
            }
            catch (Exception ex)
            {
                _logger.LogError($"设备[{device.FakeNo}]执行{Name}命令失败:{ex.Message}");
                return ResponseWrapper.Error($"设备[{device.FakeNo}]执行{Name}命令失败:{ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
