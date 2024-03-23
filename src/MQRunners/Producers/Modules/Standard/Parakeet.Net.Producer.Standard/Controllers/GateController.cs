using Common;
using Common.Dtos;
using Common.Entities;
using Common.Enums;
using Common.Extensions;
using Common.RabbitMQModule.Producers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parakeet.Net.Caches;
using Parakeet.Net.Controllers;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Parakeet.Net.Producer.Standard.Controllers
{
    /// <summary>
    /// 考勤设备 考勤、心跳记录
    /// </summary>
    [Route("api/standard/[controller]/[action]")]//, ApiController
    [Producer(Magics.STANDARD, (int)DeviceType.Gate, exchange: "exchange.direct.gate")]
    public class GateController : IOTControllerBase
    {
        private readonly IRepository<Device, Guid> _devicereRepository;
        public GateController(IServiceProvider serviceProvider, IRepository<Device, Guid> devicereRepository) : base(serviceProvider)
        {
            _devicereRepository = devicereRepository;
        }

        /// <summary>
        /// 上传考勤识别结果
        /// </summary>
        [HttpPost]
        public async Task<ResponseWrapper<string>> Record([FromBody] GateRecordInputDto input)
        {
            Logger.LogInformation($"正在处理【闸机考勤】数据,设备编号【{input.SerialNo}】");

            //产生gateRecord数据 push到消息队列
            var device = DevicePool.GetByFakeNo(input.SerialNo);
            //device = await _devicereRepository.FirstOrDefaultAsync(m => m.FakeNo == input.SerialNo);
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "闸机考勤数据接收成功"
            };
            if (device == null)
            {
                response.Code = 404;
                response.Success = false;
                response.Data = "设备不存在";
                return response;
            }

            input.RecordTime ??= DateTime.UtcNow;
            var wrapperData = new WrapperData<GateRecordDto>
            {
                Success = true,
                Code = 0,
                Data = new GateRecordDto
                {
                    Id = GuidGenerator.Create(),
                    //todo:autoMapper
                    Device = new DeviceDto
                    {
                        FakeNo = device.FakeNo,
                        SerialNo = device.SerialNo
                    },
                    DeviceId = device.Id,
                    IdCard = input.IdCard,
                    InOrOut = input.InOrOut,
                    PersonnelId = input.PersonnelId,//.Value.ToString("N"),
                    PersonnelName = input.PersonnelName,
                    Photo = input.Photo,
                    PhotoUrl = input.PhotoUrl,
                    WorkerNo = input.WorkerNo,
                    RecordTime = input.RecordTime.Value
                }
            };
            await PublishAsync(wrapperData, $"{Magics.STANDARD}.{device.Type.ToInt()}.{nameof(Record).ToLower()}");

            Logger.LogInformation($"设备：【{input.SerialNo}】上传识别结果," +
                                  $"人员Id：【{input.PersonnelName}_{input.PersonnelId}】," +
                                  $"上传时间【{input.RecordTime}】");
            //success=true  ====>设备数据库statge=1
            return response;
        }

        /// <summary>
        /// 设备心跳
        /// </summary>
        [HttpPost]
        public async Task<ResponseWrapper<string>> Heartbeat([FromBody] HeartbeatInputDto input)
        {
            Logger.LogInformation($"正在处理【闸机心跳】数据,设备编号【{input.SerialNo}】");

            var device = GetDeviceByFakeNo(input.SerialNo);
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "闸机心跳数据接收成功"
            };
            if (device == null)
            {
                response.Code = 404;
                response.Success = false;
                response.Data = "设备不存在";
                return response;
            }
            var wrapper = new WrapperData<HeartbeatDto>
            {
                Data = new HeartbeatDto
                {
                    Id = GuidGenerator.Create(),
                    //todo:autoMapper
                    Device = new DeviceDto
                    {
                        FakeNo = device.FakeNo,
                        SerialNo = device.SerialNo
                    },
                    DeviceId = device.Id,
                    RecordTime = input.RecordTime ?? DateTime.Now
                }
            };

            await PublishAsync(wrapper, $"{Magics.STANDARD}.{device.Type.ToInt()}.{nameof(Heartbeat).ToLower()}");

            return response;
        }

    }
}
