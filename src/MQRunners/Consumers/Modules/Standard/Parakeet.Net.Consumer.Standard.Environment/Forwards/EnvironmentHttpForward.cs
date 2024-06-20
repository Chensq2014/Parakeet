using Common;
using Common.Cache;
using Common.Dtos;
using Common.Extensions;
using EasyCaching.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parakeet.Net.Caches;
using Parakeet.Net.Consumer.Forwards;
using Parakeet.Net.Consumer.Standard.Dtos;
using Parakeet.Net.Consumer.Standard.Interfaces;
using Parakeet.Net.Consumer.Standard.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TokenResultDto = Parakeet.Net.Consumer.Standard.Dtos.TokenResultDto;

namespace Parakeet.Net.Consumer.Standard.EnvironmentModule.Forwards
{
    /// <summary>
    /// standard人员注册转发器
    /// </summary>
    public class EnvironmentHttpForward : HttpForward<EnvironmentRecordDto>, IEnvironmentRecordHttpForward
    {
        protected readonly IConfiguration _configuration;
        private readonly TokenConfigDto _tokenConfigDto;
        protected readonly IEasyCachingProvider CachingProvider;
        private readonly ICacheContainer<MultilevelCache<TokenResultDto>, TokenResultDto> _cacheContainer;
        public EnvironmentHttpForward(
            IHttpClientFactory httpClientFactory,
            ILogger<EnvironmentRecordDto> logger, 
            KeySecretPool keySecretPool,
            IOptionsMonitor<TokenConfigDto> tokenOptionsMonitor,
            IConfiguration configuration) : base(httpClientFactory, logger,keySecretPool)
        {
            _configuration = configuration;
            _tokenConfigDto = tokenOptionsMonitor.CurrentValue;
        }

        protected override async Task<HttpContent> GetHttpContentAsync(WrapperData<EnvironmentRecordDto> wrapperData)
        {
            //var worker = wrapperData.Data.Worker;
            //var imageHost = _configuration.GetValue<string>("ImageHost");
            var keySecret = KeySecretPool[Magics.STANDARD, wrapperData.Data.DeviceId];
            var projectCode = wrapperData.Data.Device.Project?.Organization?.Code;
            if (string.IsNullOrWhiteSpace(keySecret?.ProjectKeySecret) || string.IsNullOrWhiteSpace(_tokenConfigDto.Password))
            {
                Logger.LogError($"[环境]设备[{wrapperData.Data.Device.FakeNo}]项目编码[{projectCode}]未设置加密钥,禁止转发...");
                return null;
            }

            #region environment

            var environment = new
            {
                UnifiedProjectCode = projectCode,                       //是   工程项目编码
                DataTimeStamp = wrapperData.Data.RecordTime.ToUnixTimeTicks(13),  //是   数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = wrapperData.Data.Device.Supplier.Name ?? "供应商",  //是   设备供应商
                DeviceSN = wrapperData.Data.Device.FakeNo,                              //是   设备序列号
                TSP = wrapperData.Data.TSP ?? 0,                                    //是   TSP（总悬浮颗粒物）//最大10位字符串
                Pressure = wrapperData.Data.Pressure ?? 0,                     //是   气压
                WindSpeed = wrapperData.Data.WindSpeed ?? 0,                    //是   风速
                //WindLevel = SmartUtility.ToWindGrade(wrapperData.Data.WindSpeed),                       //是   风级
                Humidity = wrapperData.Data.Humidity ?? 0,                     //是   湿度
                Temperature = wrapperData.Data.Temperature ?? 0,                  //是   温度
                Noise = wrapperData.Data.Noise ?? 0,                        //是   噪声
                PM25 = wrapperData.Data.PM2P5 ?? 0,                        //是   PM25
                PM10 = wrapperData.Data.PM10 ?? 0,                        //是   PM10
                WindDirection = wrapperData.Data.WindDirection ?? 0,  //是   风向（角度）风向值，整数，范围0°~359°, 单位：°
                //1:正北方 2：北东北方 3：东北方 4：东东北方 5：正东方 6：东东南方 7：东南方 8：南东南方
                //9：正南方 10：南西南方 11：西南方 12：西西南方 13：正西方 14：西西北方 15：西北方 16：北西北方
                Direction = wrapperData.Data.WindDirection ?? 12,                                          //是   风向方位范围：1～16
                Atmospheric = wrapperData.Data.Rainfall ?? 0,                                      //否   空气质量，精确到小数点后一位
                Date = wrapperData.Data.RecordTime.ToString("yyyy-MM-dd"),        //是   采集日期：yyyy - MM - dd
                Time = wrapperData.Data.RecordTime.ToString("HH:ss:mm"),          //是 采集时间：HH: ss: mm
                OtherField = "test"                                      //否   保留字段
            };

            #endregion

            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(environment,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };

            var content = new FormUrlEncodedContent(list);
            Logger.LogInformation($"[环境]设备[{wrapperData.Data.Device.FakeNo}]数据发送");
            return await Task.FromResult(content);
        }

        /// <summary>
        /// client request 扩展设置
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="fakeNo"></param>
        /// <param name="receiveUrl"></param>
        /// <returns></returns>
        protected override async Task<string> RequestExtends(HttpClient httpClient, string fakeNo = null,
            string receiveUrl = null)
        {
            //把token缓存到redis里面 缓存30分钟
            var cacheKey = string.Format("Environment", fakeNo);
            var tokenResult = await _cacheContainer.GetCacheValue(cacheKey, () =>
                new MultilevelCache<TokenResultDto>(CachingProvider, cacheKey, async () =>
                {
                    var keySecret = KeySecretPool[Magics.STANDARD,fakeNo];
                    _tokenConfigDto.AppKey = keySecret?.ProjectKeyId;
                    _tokenConfigDto.AppSecret = keySecret?.ProjectKeySecret;
                    return await TokenService.GetAccessToken(_tokenConfigDto);
                })
                {
                    RedisExpireTimeSpan = TimeSpan.FromMinutes(30)
                });
            if (tokenResult.Data.IsExpired())
            {
                tokenResult = await TokenService.GetAccessToken(_tokenConfigDto);
            }
            if (tokenResult.Data.IsExpired())
            {
                tokenResult = await TokenService.GetAccessToken(_tokenConfigDto);
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenResult?.Data.Access_token}");
            return await base.RequestExtends(httpClient, fakeNo, receiveUrl);
        }
    }
}
