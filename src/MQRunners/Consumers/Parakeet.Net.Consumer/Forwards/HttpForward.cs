using Common.Devices.Dtos;
using Common.Dtos;
using Common.Extensions;
using Common.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Parakeet.Net.Caches;
using Parakeet.Net.Consumer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parakeet.Net.Consumer.Forwards
{
    /// <summary>
    /// 转发公共处理类
    /// </summary>
    /// <typeparam name="TDeviceRecord"></typeparam>
    public abstract class HttpForward<TDeviceRecordDto> : IHttpForward<TDeviceRecordDto> where TDeviceRecordDto : DeviceRecordDto
    {
        protected readonly IHttpClientFactory HttpClientFactory;
        protected readonly ILogger<TDeviceRecordDto> Logger;
        protected readonly KeySecretPool KeySecretPool;//给所有子类用
        protected ForwardConfigDto ForwardConfigDto;//给子类构造函数调用父类公共方法赋值
        private bool _initialized;
        public Action<HttpResponseMessage> HttpResponseAction;

        protected HttpForward(IHttpClientFactory httpClientFactory, ILogger<TDeviceRecordDto> logger, KeySecretPool keySecretPool)
        {
            HttpClientFactory = httpClientFactory;
            Logger = logger;
            KeySecretPool = keySecretPool;
        }

        #region IHttpForward CreateClient接口

        /// <summary>
        /// 根据ClientName创建并获取httpClient,作用域内ClientName相同的返回同一实例
        /// </summary>
        /// <returns></returns>
        public async Task<HttpClient> CreateClient()
        {
            var httpClient = HttpClientFactory.CreateClient(ForwardConfigDto.HttpClientName);
            return await Task.FromResult(httpClient);
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化【转发服务配置】
        /// </summary>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public Task Init(Action<ForwardConfigDto> configAction)
        {
            if (!_initialized)
            {
                //使用默认转发服务配置,configAction委托提供给子类扩展转发服务配置
                ForwardConfigDto = new ForwardConfigDto();
                configAction(ForwardConfigDto);
                _initialized = true;
            }
            return Task.CompletedTask;
        }

        #endregion

        #region BatchPush 批量转发处理

        /// <summary>
        /// 批量转发
        /// </summary>
        /// <param name="wrapperDataList">封装的数据源集合</param>
        /// <param name="maximumConcurrent">最大并发推送数量，默认为50</param>
        /// <param name="maximumMilliseconds">最大等待时间（毫秒），超过此时间后不再等待所有请求结果，默认为10秒</param>
        /// <returns></returns>
        public async Task BatchPush(List<WrapperData<TDeviceRecordDto>> wrapperDataList, int maximumConcurrent = 50, int maximumMilliseconds = 10000)
        {
            var bufferCount = wrapperDataList.Count % maximumConcurrent == 0 ? wrapperDataList.Count / maximumConcurrent : (wrapperDataList.Count / maximumConcurrent + 1);

            for (var count = 0; count < bufferCount; count++)
            {
                var tasks = new List<Task>();
                var wrapperDataItems = wrapperDataList.Skip(count * maximumConcurrent).Take(maximumConcurrent).ToList();
                foreach (var wrapperDataItem in wrapperDataItems)
                {
                    tasks.Add(Task.Factory.StartNew(
                        async () =>
                        {
                            await Push(wrapperDataItem);
                        }));
                }
                await Task.WhenAny(tasks.WhenAll(), Task.Delay(maximumMilliseconds));
            }
        }

        #endregion

        #region Push 转发处理方法

        /// <summary>
        /// 转发处理方法
        /// </summary>
        /// <param name="wrapperData">数据源</param>
        /// <param name="content">httpoCntent</param>
        /// <returns></returns>
        public virtual async Task Push(WrapperData<TDeviceRecordDto> wrapperData, HttpContent content = null)
        {
            try
            {
                if (wrapperData?.Data == null)
                {
                    Logger.LogWarning($"空数据,将不进行任何操作");
                    return;
                }
                if (!wrapperData.Data.Device.Mediators.Any(m => m.Forward))
                {
                    Logger.LogWarning($"设备:{wrapperData.Data?.Device?.FakeNo}未配置任何转发器");
                    return;
                }

                var area = wrapperData?.Data?.Device?.AreaTenant?.AreaCode;
                var defaultUrl = wrapperData?.Data?.Device?.Mediators?.FirstOrDefault(m => m.Mediator.Area == area)?.Mediator.Url;
                var dictionary = content is null ? await GetHttpContentsAsync(wrapperData) : new Dictionary<string, HttpContent> { { defaultUrl, content } };
                if (dictionary.Any())
                {
                    foreach (var keyValuePair in dictionary)
                    {
                        Logger.LogDebug($"====================>设备{wrapperData?.Data?.Device?.FakeNo}转发数据到:{keyValuePair.Key}");

                        var result = await Request(wrapperData.Data.Device.FakeNo, keyValuePair.Key, keyValuePair.Value);
                        if (!result.Success)
                        {
                            await RedisHelper.RPushAsync(CacheKeys.DeviceForwardRecord, new ForwardErrorRecordDto
                            {
                                Area = wrapperData.Data.Device.Mediators.FirstOrDefault()?.Mediator.Area,
                                Content = JsonConvert.SerializeObject(wrapperData),
                                Device = wrapperData.Data.Device,
                                DeviceId = wrapperData.Data.DeviceId,
                                RecordTime = wrapperData.Data.RecordTime,
                                Code = result.Code,
                                Result = result.Message
                            });
                        }

                        Logger.LogDebug($"{wrapperData?.Data?.Device?.FakeNo}_{keyValuePair.Key}_{result?.Message}");
                    }
                }
                else
                {
                    Logger.LogWarning($"设备:{wrapperData.Data?.Device?.FakeNo}没有转发Content 无法进行转发");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"设备{wrapperData?.Data?.Device?.FakeNo}转发产生未知错误");

                await RedisHelper.RPushAsync(CacheKeys.DeviceForwardRecord, new ForwardErrorRecordDto
                {
                    Area = wrapperData.Data.Device.Mediators.FirstOrDefault()?.Mediator.Area,
                    Content = JsonConvert.SerializeObject(wrapperData),
                    DeviceId = wrapperData.Data.DeviceId,
                    Device = wrapperData.Data.Device,
                    RecordTime = wrapperData.Data.RecordTime,
                    Result = $"设备{wrapperData?.Data?.Device?.FakeNo}产生未知错误未知错误Exception{ex};Detail:{ex.StackTrace}",
                    Code = 400
                });
            }
        }

        #endregion

        #region 获取Dictionary<string, HttpContent> 扩展

        /// <summary>
        /// 转发多处 提供子类扩展
        /// </summary>
        /// <param name="wrapperData"></param>
        protected virtual async Task<Dictionary<string, HttpContent>> GetHttpContentsAsync(WrapperData<TDeviceRecordDto> wrapperData)
        {
            var dictionary = new Dictionary<string, HttpContent>();
            var content = await GetHttpContentAsync(wrapperData);
            if (content != null)
            {
                foreach (var deviceMediator in wrapperData.Data.Device.Mediators)
                {
                    //var area = wrapperData?.Data?.Device?.AreaTenant?.AreaCode;
                    var url = deviceMediator.Mediator.Url;
                    if (url is null)
                    {
                        Logger.LogWarning($"设备类型{wrapperData?.Data?.Device?.Type.ToInt()}未配置转发器");
                        continue;
                    }
                    dictionary.Add(url, content);
                }
            }
            return dictionary;
        }

        /// <summary>
        /// 获取默认content
        /// </summary>
        /// <param name="wrapperData"></param>
        /// <returns></returns>
        protected virtual Task<HttpContent> GetHttpContentAsync(WrapperData<TDeviceRecordDto> wrapperData)
        {
            return Task.FromResult(GetHttpContent(wrapperData));
        }

        /// <summary>
        /// 获取默认content
        /// </summary>
        /// <param name="wrapperData"></param>
        /// <returns></returns>
        protected virtual HttpContent GetHttpContent(WrapperData<TDeviceRecordDto> wrapperData)
        {
            return new StringContent("");
        }


        #endregion

        #region Request Http请求及返回结果扩展

        /// <summary>
        /// 支持多个content转发
        /// </summary>
        /// <param name="receiveUrl"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected virtual async Task<ResponseWrapper> Request(string fakeNo, string receiveUrl, HttpContent content)
        {
            using var httpClient = await CreateClient();
            var fullUrl = await RequestExtends(httpClient, fakeNo, receiveUrl);

            var result = new ResponseWrapper();
            try
            {
                HttpResponseMessage httpResponseMessage;
                if (ForwardConfigDto.HttpMethod == HttpMethod.Post)
                {
                    httpResponseMessage = await httpClient.PostAsync(fullUrl, content);
                }
                else if (ForwardConfigDto.HttpMethod == HttpMethod.Get)
                {
                    httpResponseMessage = await httpClient.GetAsync(fullUrl);
                }
                else
                {
                    result = ResponseWrapper.Error("设备{fakeNo}暂不支持Post和Get以外的请求方式", 405);
                    throw new NotSupportedException("设备{fakeNo}暂不支持Post和Get以外的请求方式");
                }

                if (HttpResponseAction != null)
                {
                    HttpResponseAction(httpResponseMessage);
                }
                else
                {
                    if (!httpResponseMessage.IsSuccessStatusCode)
                    {
                        result = ResponseWrapper.Error($"设备{fakeNo}转发到远程Url=>{fullUrl}失败", 400);
                        Logger.LogError($"设备{fakeNo}转发到远程Url=>{fullUrl}失败");
                    }
                }

                var response = await httpResponseMessage.Content.ReadAsStringAsync();
                result.Message = response;

                await HandlerResult(result);

                if (result.Success || result.Code == 0 || result.Code == 200)
                {
                    Logger.LogInformation($"设备{fakeNo}=================>{fullUrl}转发结果:{response}");
                }
                else
                {
                    throw new Exception(result.Message);
                }

                httpResponseMessage.Dispose();
            }
            catch (HttpRequestException e)
            {
                result = ResponseWrapper.Error($"设备{fakeNo}推送消息到{fullUrl}失败！异常信息：{e.Message}, 堆栈信息：{e.StackTrace}");
                Logger.LogWarning($"设备{fakeNo}推送消息到{fullUrl}失败！", e.Message);
                await Task.Delay(100);
            }
            catch (Exception e)
            {
                result = ResponseWrapper.Error($"设备{fakeNo}推送消息到{fullUrl}失败！异常信息：{e.Message}, 堆栈信息：{e.StackTrace}");
                Logger.LogWarning($"设备{fakeNo}推送消息到{fullUrl}失败！", e.Message);
                await Task.Delay(100);
            }

            return result;
        }

        /// <summary>
        /// 设置DefaultRequestHeaders
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="fakeNo">设备转发编码</param>
        /// <param name="receiveUrl"></param>
        protected virtual async Task<string> RequestExtends(HttpClient httpClient, string fakeNo = null, string receiveUrl = null)
        {
            return await Task.FromResult(receiveUrl);
        }

        /// <summary>
        /// 返回结果扩展处理
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        protected virtual Task<ResponseWrapper> HandlerResult(ResponseWrapper wrapper)
        {
            //wrapper = JsonConvert.DeserializeObject<ResponseWrapper>(wrapper.Message);
            return Task.FromResult(wrapper);
        }
        #endregion

    }
}
