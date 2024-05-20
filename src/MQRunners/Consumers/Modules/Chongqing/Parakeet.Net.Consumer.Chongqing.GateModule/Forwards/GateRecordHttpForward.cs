using Common;
using Common.Cache;
using Common.Dtos;
using Common.Extensions;
using EasyCaching.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parakeet.Net.Caches;
using Parakeet.Net.Consumer.Chongqing.Dtos;
using Parakeet.Net.Consumer.Chongqing.Interfaces;
using Parakeet.Net.Consumer.Chongqing.Services;
using Parakeet.Net.Consumer.Forwards;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TokenResultDto = Parakeet.Net.Consumer.Chongqing.Dtos.TokenResultDto;

namespace Parakeet.Net.Consumer.Chongqing.GateModule.Forwards
{
    /// <summary>
    /// 考勤
    /// </summary>
    public class GateRecordHttpForward : HttpForward<GateRecordDto>, IGateRecordHttpForward
    {
        protected readonly IEasyCachingProvider CachingProvider;
        protected readonly IConfiguration _configuration;
        private readonly ICacheContainer<MultilevelCache<TokenResultDto>, TokenResultDto> _cacheContainer;
        private readonly TokenConfigDto _tokenConfigDto;

        public GateRecordHttpForward(
            IHttpClientFactory httpClientFactory,
            ILogger<GateRecordDto> logger,
            KeySecretPool keySecretPool,
            IConfiguration configuration,
            ICacheContainer<MultilevelCache<TokenResultDto>, TokenResultDto> cacheContainer,
            IOptionsMonitor<TokenConfigDto> tokenOptionsMonitor,
            IEasyCachingProvider cachingProvider) : base(httpClientFactory, logger, keySecretPool)
        {
            _configuration = configuration;
            _tokenConfigDto = tokenOptionsMonitor.CurrentValue;
            CachingProvider = cachingProvider;
            _cacheContainer = cacheContainer;
        }

        /// <summary>
        /// httpClient 请求添加accessToken
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="fakeNo"></param>
        /// <param name="receiveUrl"></param>
        /// <returns></returns>
        protected override async Task<string> RequestExtends(HttpClient httpClient, string fakeNo = null,
            string receiveUrl = null)
        {
            //把token缓存到redis里面 缓存30分钟
            var cacheKey = $"{fakeNo}";
            var tokenResult = await _cacheContainer.GetCacheValue(cacheKey, () =>
                new MultilevelCache<TokenResultDto>(CachingProvider, cacheKey, async () =>
                {
                    var keySecret = KeySecretPool[Magics.CHONGQING, fakeNo];
                    _tokenConfigDto.AppKey = keySecret?.ProjectKeyId;
                    _tokenConfigDto.AppSecret = keySecret?.ProjectKeySecret;
                    return await TokenService.GetAccessToken(_tokenConfigDto);
                })
                {
                    RedisExpireTimeSpan = TimeSpan.FromMinutes(30)
                });

            if (tokenResult.Custom.IsExpired())
            {
                tokenResult = await TokenService.GetAccessToken(_tokenConfigDto);
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenResult?.Custom.Access_token}");
            return await base.RequestExtends(httpClient, fakeNo, receiveUrl);
        }


        protected override async Task<HttpContent> GetHttpContentAsync(WrapperData<GateRecordDto> data)
        {
            var serialNo = data.Data.Device.SerialNo;
            var item = data.Data;
            var personnelId = item.PersonnelId;
            //var workerNo = item.WorkerNo ?? _workerDevicePool[data.Data.FakeNo, item.PersonnelName, item.IdCard]?.WorkerNo;
            var workerNo = data.Data.WorkerNo;
            if (string.IsNullOrEmpty(workerNo))
            {
                Logger.LogError($"[重庆考勤]设备{serialNo}:WorkerNo is null:PersonnelId:{personnelId},不存在此人消息,禁止转发");
                return null;
            }

            var projectCode = data.Data.Device.Project?.Organization?.Code;
            if (string.IsNullOrWhiteSpace(projectCode))
            {
                Logger.LogError($"[重庆闸机实名制]设备[{data.Data.Device.FakeNo}]未设置项目编码,禁止转发...");
                return null;
            }

            var fullImageUrl = string.Empty;
            try
            {
                var imageHost = _configuration.GetValue<string>("ImageHost");
                //fullImageUrl = ImageUrlToBase64Helper.TryConvertFullUrl(Logger, imageHost, item.PhotoUrl);
                await Task.Delay(100);
            }
            catch (Exception e)
            {
                Logger.LogError($"图片[{item.PhotoUrl}]拼接url失败", e);
                return null;
            }

            var user = new
            {
                UnifiedProjectCode = projectCode,                        //C50 必填   工程项目编码
                DataTimeStamp = data.Data.RecordTime.ToUnixTimeTicks(13),        //N20 必填   数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = data.Data.Device.Supplier.Name, //C20 必填   设备供应商
                DeviceSN = item.Device.FakeNo,                                  //C20 必填 设备序列号
                IdCardNumber = item.IdCard,                              //C18 必填   证件号码（证件号和人员ID不能同时为空） 
                WorkerId = item.WorkerNo,                                //C50 必填   人员ID（证件号和人员ID不能同时为空，且不能为0）
                //C10 必填   考勤方式：
                //Face：人脸识别 Eye：虹膜识别 Finger：指纹识别 Hand：掌纹识别 IDCard：身份证识别 RnCard：实名卡
                //Error：异常清退 Manuel：一键开闸 ExitChannel：应急通道 QRCode：二维码识别 App：APP考勤 Other：其他方式
                //Mode = item. ?? "Face",
                Direction = 0,                                           //C1  必填 进出方向(1:进场，0：出场)
                Image = fullImageUrl,//C50 必填   考勤图片URL 
                Lat = item.Device.LocationArea.Latitude,     //C20 否   经度
                Lng = item.Device.LocationArea.Longitude,    //C20 否 纬度
                AttendanceDate = item.RecordTime.ToString("yyyy-MM-dd HH:ss:mm"),//C50 必填   考勤时间(格式为 yyyy - MM - dd HH: mm:ss）
                OtherField = "test" //C200 否   扩展字段
            };

            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(user,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };

            var content = new FormUrlEncodedContent(list);
            Logger.LogInformation($"[重庆考勤]设备[{data.Data.Device.FakeNo}]数据发送");
            Logger.LogInformation($"[SerialNo:{serialNo}_PersonnelId:{personnelId}]_workerId:{user.WorkerId}_photo:{user.Image};");

            return content;
        }
    }
}