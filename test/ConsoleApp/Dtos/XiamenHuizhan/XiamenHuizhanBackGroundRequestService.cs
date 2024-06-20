using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Volo.Abp;

namespace ConsoleApp.Dtos.XiamenHuizhan
{
    /// <summary>
    /// 厦门会展请求服务 url会根据设备编码等规则变化，使用httpClient动态请求
    /// </summary>
    public class XiamenHuizhanBackGroundRequestService : BackgroundService
    {
        //private readonly ILogger _logger;
        private Timer _timer;
        private bool _init;

        /// <summary>
        /// 构造函数
        /// </summary>
        public XiamenHuizhanBackGroundRequestService()
        {

        }

        #region 一标段

        /// <summary>
        /// 一标段环境设备实时数据请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> SectionOneEnvironmentRealTimeRequest(PostParameterDto input)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Cookie", $"satoken=f9f4183d-5b7b-4c9b-bda7-2401def00833");
            input.Host = "http://123.56.74.49";
            input.Port = 6000;
            var deviceId = "870699588397301760";
            input.Api = $"/devices/{deviceId}/data/latest";
            Console.WriteLine($"input：{JsonConvert.DeserializeObject(JsonConvert.SerializeObject(input))}");
            var result = await client.GetAsync($"{input.FullUrl}");
            if (!result.IsSuccessStatusCode)
            {
                throw new UserFriendlyException($"请求{input.FullUrl}失败");
            }
            var content = await result.Content.ReadAsStringAsync();
            var returnObj = JsonConvert.DeserializeObject<SectionOneEnvironmentReturnData>(content);
            var environment = new EnvironmentCommon(Guid.NewGuid())
            {
                SerialNo = returnObj.Data.DeviceId,
                Pm10 = returnObj.Data.Data.Pm10Rtd,
                Pm10Flag = returnObj.Data.Data.Pm10Flag,
                Pm25 = returnObj.Data.Data.Pm25Rtd,
                Pm25Flag = returnObj.Data.Data.Pm25Flag,
                WindSpeed = returnObj.Data.Data.WsRtd,
                WindSpeedFlag = returnObj.Data.Data.WsFlag,
                Temperature = returnObj.Data.Data.TemRtd,
                TemperatureFlag = returnObj.Data.Data.TemFlag,
                Noise = returnObj.Data.Data.B03Rtd,
                NoiseFlag = returnObj.Data.Data.B03Flag,
                Humidity = returnObj.Data.Data.RhRtd,
                HumidityFlag = returnObj.Data.Data.RhFlag,
                WindDirection = returnObj.Data.Data.WdRtd,
                WdFlag = returnObj.Data.Data.WdFlag,
                Pressure = returnObj.Data.Data.PaRtd,
                PaFlag = returnObj.Data.Data.PaFlag,
                Tsp = returnObj.Data.Data.TspRtd,
                TspFlag = returnObj.Data.Data.TspFlag,
                RecordTime = returnObj.Data.Data.DateTimeFormat
            };
            //存入数据库
            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// 一标段考勤历史记录请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> SectionOneGateHistoryRequest(PostParameterDto input)
        {
            using var client = new HttpClient();
            input.Host = "http://123.56.74.49";
            input.Port = 6000;
            var now = DateTime.Now;
            var deviceId = "887344297819504640";
            input.Api = $"/services/execute/GetProjectAttendanceListNew?deviceId={deviceId}";
            input.Content = new StringContent(JsonConvert.SerializeObject(new
            {
                pageSize = 1000,
                pageIndex = input.Next,
                begin = now.AddDays(-1),
                end = now
            }), Encoding.UTF8);
            Console.WriteLine($"input：{JsonConvert.DeserializeObject(JsonConvert.SerializeObject(input))}");

            //client.DefaultRequestHeaders.Add("Cookie", $"satoken=f9f4183d-5b7b-4c9b-bda7-2401def00833");
            var result = await client.PostAsync($"{input.FullUrl}", input.Content);
            if (!result.IsSuccessStatusCode)
            {
                throw new UserFriendlyException($"请求{input.FullUrl}失败");
            }
            var content = await result.Content.ReadAsStringAsync();
            var returnObj = JsonConvert.DeserializeObject<SectionOneGateReturnData>(content);
            var gates = returnObj.Data.Data.Select(m => new GateCommon(Guid.NewGuid())
            {
                SerialNo = m.SerialNumber,
                ProjectSysNo = m.ProjectSysNo,
                SubContractorSysNo = m.SubContractorSysNo,
                SubContractorName = m.SubContractorName,
                TeamSysNo = m.TeamSysNo,
                TeamName = m.TeamName,
                WorkTypeName = m.TeamName,
                WorkerNo = m.WorkerSysNo,
                PersonnelId = m.WorkerSysNo,
                PersonnelName = m.WorkerName,
                IdCard = m.IDCardNumber,
                InOutType = m.Type.ToString(),
                Gender = m.Gender.ToString(),
                Photo = m.Image,
                PhotoUrl = m.Image,
                CheckChannel = m.CheckChannel,
                FaceSimilarity = m.FaceSimilarity,
                RecordTime = m.InDate ?? m.Time
            }).ToList();
            if (gates.Min(m => m.RecordTime) > now.AddMonths(-1))
            {
                //继续请求
                input.Next++;
                await SectionOneGateHistoryRequest(input);
            }
            //历史考勤记录 查出1个月内的数据存入数据库，然后自动任务每10分钟查一次10分钟内产生的历史数据入库
            foreach (var gate in gates)
            {

            }
            return result.IsSuccessStatusCode;
        }



        #endregion

        #region 二标段


        /// <summary>
        /// 二标段环境设备实时数据请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> SectionTwoEnvironmentRealTimeRequest(PostParameterDto input)
        {
            using var client = new HttpClient();
            input.Host = "http://47.107.103.35";
            input.Port = 8010;
            input.Api = "/openApi/data/realtime";
            Console.WriteLine($"input：{JsonConvert.DeserializeObject(JsonConvert.SerializeObject(input))}");
            input.Content = new StringContent(JsonConvert.SerializeObject(new
            {
                SN = "MjAyMTA2MDIwMzEwMDAwMw=="
            }), Encoding.UTF8);
            var result = await client.PostAsync($"{input.FullUrl}", input.Content);
            if (!result.IsSuccessStatusCode)
            {
                throw new UserFriendlyException($"请求{input.FullUrl}失败");
            }
            var content = await result.Content.ReadAsStringAsync();
            var returnObj = JsonConvert.DeserializeObject<SectionTwoEnvironmentReturnData>(content);
            var eCommon = new EnvironmentCommon
            {
                SerialNo = returnObj.Content.DeviceId,
                Pm10 = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a34002")?.Data,
                Pm10Flag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a34002")?.Flag,
                Pm25 = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a34004")?.Data,
                Pm25Flag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a34004")?.Flag,
                WindSpeed = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01007")?.Data,
                WindSpeedFlag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01007")?.Flag,
                Temperature = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01001")?.Data,
                TemperatureFlag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01001")?.Flag,
                Noise = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a50001")?.Data,
                NoiseFlag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a50001")?.Flag,
                Humidity = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01002")?.Data,
                HumidityFlag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01002")?.Flag,
                WindDirection = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01008")?.Data,
                WdFlag = returnObj.Content.RealtimeData.Find(m => m.Sensor == "a01008")?.Flag,
                RecordTime = returnObj.Content.DateTime
            };
            //存入数据库
            return result.IsSuccessStatusCode;
        }


        /// <summary>
        /// 2标段考勤历史记录请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> SectionTwoGateHistoryRequest(PostParameterDto input)
        {
            using var client = new HttpClient();
            var now = DateTime.Now;
            input.Host = "https://glm.glodon.com/api/open";
            input.Port = 443;
            //这里根据appid、secret计算出签名sign 放在url里面或者post参数里面请求接口
            var secret = "bd1b6322217dfbeb96de3696d1a387fa";
            var gateInput = new
            {
                appid = "0a9416fd6baa4595a1268ad1f52034b0",
                //secret = "bd1b6322217dfbeb96de3696d1a387fa",
                //sign = "",//调用一个方法计算sign
                beginDate = $"{now.AddDays(-1):yyyy-MM-dd}",
                endDate = $"{now:yyyy-MM-dd}",
                inOutType = "IN",//IN进，OUT出
                projectId = "490952877121536",//租户id：1438415,
                startId = input.Next,
                pageSize = 1000
            };

            var sign = Utilities.GetXiamenSectionTwoSign(secret, gateInput);

            input.Api = $"/attendance/cardV2?appid={gateInput.appid}&sign={sign}&beginDate={gateInput.beginDate}&endDate={gateInput.endDate}&inOutType={gateInput.inOutType}&projectId={gateInput.projectId}&startId={gateInput.startId}&pageSize={gateInput.pageSize}";

            Console.WriteLine($"input：{JsonConvert.DeserializeObject(JsonConvert.SerializeObject(input))}");
            var result = await client.GetAsync($"{input.FullUrl}");
            if (!result.IsSuccessStatusCode)
            {
                throw new UserFriendlyException($"请求{input.FullUrl}失败");
            }
            var content = await result.Content.ReadAsStringAsync();
            var returnObj = JsonConvert.DeserializeObject<SectionTwoGateReturnData>(content);
            var gates = returnObj.Data.Select(m => new GateCommon(Guid.NewGuid())
            {

                SerialNo = m.DeviceCode,
                ProjectSysNo = m.ProjectWorkerId?.ToString(),
                SubContractorSysNo = m.CompanyId?.ToString(),
                SubContractorName = m.CompanyName,
                CompanyId = m.CompanyId?.ToString(),
                CompanyName = m.CompanyName,
                TeamSysNo = m.TeamId?.ToString(),
                TeamName = m.TeamName,
                WorkTypeName = m.WorkTypeName,
                WorkTypeCode = m.WorkTypeCode,
                WorkerNo = m.WorkerId?.ToString(),
                PersonnelId = m.Id.ToString(),
                PersonnelName = m.WorkerName,
                IdCard = m.Identification,
                InOutType = m.InOutType,
                //Gender = m.Gender.ToString(),
                Photo = m.ScanPhoto,
                PhotoUrl = m.ScanPhoto,
                CheckChannel = m.ClockType,
                RecordTime = m.Date
            }).ToList();
            if (gates.Min(m => m.RecordTime) > now.AddMonths(-1))
            {
                //继续请求
                input.Next = returnObj.Data.LastOrDefault()?.Id ?? ++input.Next;//input.Next++;
                await SectionTwoGateHistoryRequest(input);
            }
            //历史考勤记录 查出1个月内的数据存入数据库，然后自动任务每10分钟查一次10分钟内产生的历史数据入库
            foreach (var gate in gates)
            {
                
            }
            return result.IsSuccessStatusCode;
        }



        #endregion


        /// <summary>
        /// 定期执行逻辑
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_init)
            {
                //等待10分钟600秒
                await Task.Delay(100000, stoppingToken);
                _init = true;
            }

            _timer = new Timer(async func =>
            {
                try
                {

                    //一标段环境 
                    await SectionOneEnvironmentRealTimeRequest(new PostParameterDto { }).ConfigureAwait(false);
                    //二标段环境
                    await SectionTwoEnvironmentRealTimeRequest(new PostParameterDto { }).ConfigureAwait(false);
                    //一标段考勤历史记录
                    await SectionOneGateHistoryRequest(new PostParameterDto { }).ConfigureAwait(false);
                    //二标段考勤历史记录
                    await SectionTwoGateHistoryRequest(new PostParameterDto { }).ConfigureAwait(false);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"数据模拟规则调用api出现错误{ex.Message}", ex);

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

            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));

            await Task.CompletedTask;
        }
    }
}
