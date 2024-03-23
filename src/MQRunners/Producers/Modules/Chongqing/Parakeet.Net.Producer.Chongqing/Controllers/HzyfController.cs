using Common;
using Common.Dtos;
using Common.Dtos.Hzyf;
using Common.Enums;
using Common.Extensions;
using Common.RabbitMQModule.Producers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Parakeet.Net.Controllers;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Parakeet.Net.Producer.Chongqing.Controllers
{
    /// <summary>
    /// 杭州宇泛（实名制/人脸识别/闸机）  测试
    /// </summary>
    [Route("chongqing/[controller]/[action]")]
    [Producer(Magics.CHONGQING, (int)DeviceType.Gate, exchange: "exchange.direct.gate")]
    public class HzyfController : IOTControllerBase
    {
        public HzyfController(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        /// <summary>
        /// 设备心跳 1分钟post一次
        /// </summary>
        [HttpPost]
        public async Task<object> Heartbeat(UfaceHeartbeatDto input)
        {
            var deviceKeyGroup = $"Device:Array:SerialNo:{input.DeviceKey}";

            #region 消息队列

            //注意，这里只需要判断deviceKey是否有任务，不进行消费
            var isExistTask = await RedisHelper.ExistsAsync(deviceKeyGroup);

            #endregion 消息队列

            var device = DevicePool[input.DeviceKey];
            //push到心跳消息队列，保存心跳数据到数据库  1分钟一次心跳
            var wrapperData = new WrapperData<HeartbeatDto>
            {
                Success = true,
                Code = 0,
                Data = new HeartbeatDto
                {
                    Id = GuidGenerator.Create(),
                    Device = new DeviceDto
                    {
                        SerialNo = input.DeviceKey,//设备返回的就是真正的serialNo
                        FakeNo = device?.FakeNo,
                    },
                    DeviceId = device.Id
                }
            };

            await PublishAsync(wrapperData, $"{Magics.CHONGQING}.{device.Type.ToInt()}.heartbeat");

            return new
            {
                Result = isExistTask//只有返回 true 时，设备才开始请求获取任务接口
            };
        }

        /// <summary>
        /// 设备心跳发现消息队列有未消费的数据时，就会post到这里
        /// </summary>
        [HttpPost]
        public async Task<string> GetTask(GetTaskDto input)
        {
            //从消息队列里面 根据 deviceKey去取任务，根据设备编号或任务名称，找到消息队列里面返回给设备的数据
            var deviceKeyGroup = $"Device:Array:SerialNo:{input.DeviceKey}";
            var item = await RedisHelper.RPopAsync(deviceKeyGroup);
            var json = JsonDocument.Parse(item);
            var interfaceName = json.RootElement.GetProperty("interfaceName").ToString();
            if (interfaceName.Contains("person/create") || interfaceName.Contains("face/create"))
            {
                var taskNo = json.RootElement.GetProperty("taskNo");//json["taskNo"]?.ToString();
                var taskJson = item.Replace("/create", "/update");
                await RedisHelper.SetAsync($"{taskNo}", taskJson, 3600);
                Logger.LogInformation($"将任务改为update放到消息队列中\n:{taskNo}");
            }
            Logger.LogDebug($"设备获取任务字符串\n:{item}");
            return item;
        }


        /// <summary>
        /// 设备任务处理结果 回调Api
        /// </summary>
        [HttpPost]
        public async Task<object> HandleResultFeedBack([FromBody] HandleFeedBackDto input)
        {
            var deviceKeyGroup = $"Device:Array:SerialNo:{input.DeviceKey}";
            var result = JsonConvert.DeserializeObject<ExecuteTaskResultDto<object>>(input.Result);
            var device = DevicePool[input.DeviceKey];
            var sequeceNo = Convert.ToInt64(input.TaskNo);
            Logger.LogInformation($"设备{device.FakeNo}处理结果返回信息：{input.Result},sequenceNo:{sequeceNo}");
            var json = JsonDocument.Parse(input.Result);
            json.RootElement.TryGetProperty("code", out var codeElement);
            json.RootElement.TryGetProperty("success", out var successElement);
            var code = codeElement.ToString();
            var success = bool.Parse(successElement.ToString());
            if (success == false && (code.Contains("LAN_EXP-3005") || code.Contains("LAN_EXP-4007")))
            {
                var taskJson = await RedisHelper.GetAsync($"{input.TaskNo}");
                if (taskJson.HasValue())
                {
                    await RedisHelper.LPushAsync(deviceKeyGroup, taskJson);
                    Logger.LogInformation($"设备{device.FakeNo}重新修改json任务为update：{taskJson}");
                    return new { Result = true };
                }
            }
            var wrapperData = new WrapperData<FeedbackDto>
            {
                Data = new FeedbackDto
                {
                    Id = GuidGenerator.Create(),
                    Device = new DeviceDto
                    {
                        FakeNo = device.FakeNo,
                        SerialNo = device.SerialNo,
                    },
                    Code = result.Success ? 0 : 1,
                    SequenceNo = sequeceNo,
                    RecordTime = DateTime.Now
                }
            };
            await PublishAsync(wrapperData, $"{Magics.CHONGQING}.{device.Type.ToInt()}.feedback");
            var isExistTask = await RedisHelper.ExistsAsync(deviceKeyGroup);
            return new { Result = isExistTask };//处理完毕一条任务，如果队列里面还有未消费的任务，返回true 设备继续GetTask处理下一条任务
        }

        /// <summary>
        /// 上传识别结果
        /// 设备每间隔10分钟从数据库中读取出回调失败(state:0) 的识别记录,再次向回调地址发送请求，
        /// 请求成功则更新数据库中对应的回调状态(state:1)
        /// </summary>
        [HttpPost]
        public async Task<string> UploadAttendance(UfaceIdentityDto input)
        {
            //产生gateRecord数据 push到消息队列
            var device = DevicePool[input.DeviceKey];
            //DateTime.Now.ToUnixTimeTicks(13);
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
                    //IdCard = input.PersonId,//在分流模块去赋值(应该在解析模块赋值)
                    PersonnelId = string.IsNullOrEmpty(input.PersonId) ? Guid.Empty.ToString("N") : input.PersonId,
                    PersonnelName = input.PersonId,
                    //时间戳转datetime
                    RecordTime = long.TryParse(input.Time, out var timeUnix)
                        ? timeUnix.FromUnixMillonSeconds()
                        : DateTime.TryParse(input.Time, out DateTime recordTime)
                            ? recordTime
                            : DateTime.Now
                }
            };
            await PublishAsync(wrapperData, $"{Magics.CHONGQING}.{device.Type.ToInt()}.record");

            Logger.LogInformation($"设备：【{input.DeviceKey}】上传识别结果," +
                                  $"人员Id：【{input.PersonId}】," +
                                  $"上传时间【{input.Time}】");
            //success=true  ====>设备数据库statge=1
            return JsonConvert.SerializeObject(new { result = 1, success = true });
        }

    }
}
