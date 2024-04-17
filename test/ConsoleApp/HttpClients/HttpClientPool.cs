using System;
using System.Collections.Generic;
using Serilog;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Dtos;
using Newtonsoft.Json;
using Common.Dtos;
using Common.Extensions;
using RestSharp;

namespace ConsoleApp.HttpClients
{
    /// <summary>
    /// 湖南HttpClient单例
    /// </summary>
    public class HttpClientPool
    {
        /// <summary>
        /// 静态字段
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly TokenResultDto tokenResult = new TokenResultDto();
        private static readonly ChongqingTokenResultDto chongqingTokenResult = new ChongqingTokenResultDto();
        private static readonly SichuanTokenResultDto sichuanTokenResult = new SichuanTokenResultDto { Data = new SichuanTokenResultDataDto() };

        public static HttpClient Instance()
        {
            Log.Logger.Debug($"{nameof(HttpClient)}只被构造一次");
            //_httpClient.BaseAddress = new Uri("http://39.108.81.42");
            return _httpClient;
        }

        /// <summary>
        /// 测试client请求代码
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> GetAsync()
        {
            //client.DefaultRequestHeaders.Add("Host", data.Mediator.Host);
            //client.DefaultRequestHeaders.Add("Method", "Post");
            //client.DefaultRequestHeaders.Add("KeepAlive", "false"); //防止http连接保持
            var url = $"http://www.baidu.com";
            var response = await _httpClient.GetAsync(url);

            Log.Logger.Debug($"request:{ response != null && response.StatusCode is HttpStatusCode.OK}");
            return response != null && response.StatusCode is HttpStatusCode.OK;
        }


        /// <summary>
        /// 获取token方法 缓存一个token 发现失败再执行刷新token  刷新token的api文档还没呢,只有直接请求的api？
        /// </summary>
        /// <param name="serialNo"></param>
        /// <param name="baseUrl"></param>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        public static async Task<TokenResultDto> GetAccessToken(string serialNo, string baseUrl = null, string apiUrl = null)
        {
            //如果没有accessToken或者token过期(12h) 才重新请求token
            if (string.IsNullOrWhiteSpace(tokenResult.AccessToken) || tokenResult.IsExpired())
            {
                var tokenClient = new RestClient(baseUrl);
                var tokenRequest = new RestRequest(apiUrl, Method.Get);
                tokenRequest.AddParameter("devKey", serialNo);
                var tokenResponse = await tokenClient.ExecuteAsync(tokenRequest);
                var token = JsonConvert.DeserializeObject<ApiBaseRequestResponse>(tokenResponse.Content);
                if (token.Errcode != 0)
                {
                    //logger.LogError("请求token失败,请检查设备所在项目devKey是否可用...");
                    return null;
                }
                tokenResult.CreateTime = DateTime.Now;
                tokenResult.AccessToken = token.Content;
            }
            //else if (tokenResult.IsExpired())
            //{
            //    var refreshUrl = $@"/jgapi/devkey/verify";
            //    await RefreshToken(tokenResult.AccessToken, baseUrl, refreshUrl);
            //}
            return tokenResult;
        }


        /// <summary>
        /// 获取token方法 缓存一个token 发现失败再执行刷新token  刷新token的api文档还没呢,只有直接请求的api？
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="baseUrl"></param>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        public static async Task<ChongqingTokenResultDto> GetChongqingAccessToken(string appKey, string appSecret, string baseUrl = null, string apiUrl = null)
        {
            //DateTime.Now.Subtract(CreateTime) > TimeSpan.FromSeconds(ExpiresIn)
            //如果没有accessToken或者token过期(12h) 才重新请求token
            if (string.IsNullOrWhiteSpace(chongqingTokenResult.Custom?.Access_token) || chongqingTokenResult.Custom.IsExpired())
            {
                #region restClient

                //var tokenClient = new RestClient(baseUrl);
                //var tokenRequest = new RestRequest(apiUrl, Method.POST);
                ////tokenRequest.AddJsonBody(new
                ////{
                ////    client_id=appKey,
                ////    client_secret= appSecret,
                ////    grant_type="client_credentials"
                ////});
                //tokenRequest.AddParameter("client_id", appKey);
                //tokenRequest.AddParameter("client_secret", appSecret);
                //tokenRequest.AddParameter("grant_type", "client_credentials");

                //var tokenResponse = await tokenClient.ExecuteAsync(tokenRequest);
                //var token = JsonConvert.DeserializeObject<ChongqingTokenResultDto>(tokenResponse.Content);
                //if (token.Status.Code == 0)
                //{
                //    //logger.LogError("请求token失败,请检查设备所在项目devKey是否可用...");
                //    Console.WriteLine($"请求token失败,请检查设备所在项目devKey是否可用...");
                //    return null;
                //}
                #endregion

                #region _httpClient

                var input = new
                {
                    client_id = appKey,
                    client_secret = appSecret,
                    grant_type = "client_credentials"
                };
                Console.WriteLine($"Json序列化input:");
                Console.WriteLine($"{Newtonsoft.Json.JsonConvert.DeserializeObject<object>(JsonConvert.SerializeObject(input))}");
                //var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);

                Console.WriteLine($"请求地址:{baseUrl}{apiUrl}");
                var list = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("client_id", input.client_id),
                    new KeyValuePair<string, string>("client_secret", input.client_secret),
                    new KeyValuePair<string, string>("grant_type", input.grant_type),
                };
                var content = new FormUrlEncodedContent(list);
                //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
                var result = await _httpClient.PostAsync($"{baseUrl}{apiUrl}", content);
                var msg = await result.Content.ReadAsStringAsync();
                Console.WriteLine($"token请求结果:\n{Newtonsoft.Json.JsonConvert.DeserializeObject<object>(msg)}");
                if (!result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"{baseUrl}{apiUrl}请求失败！");
                }
                //TextJsonConvert.DeserializeObject<ChongqingTokenResultDto>(msg);
                var token = JsonConvert.DeserializeObject<ChongqingTokenResultDto>(msg);
                #endregion

                #region 请求结果保存到redis 此处用静态变量存放

                chongqingTokenResult.Custom.CreateTime = DateTime.Now;
                chongqingTokenResult.Custom.Access_token = token.Custom.Access_token;
                chongqingTokenResult.Custom.Refresh_token = token.Custom.Refresh_token;
                chongqingTokenResult.Custom.Jsessionid = token.Custom.Jsessionid;
                chongqingTokenResult.Custom.Expires_in = token.Custom.Expires_in;

                #endregion
            }
            //else if (tokenResult.IsExpired())
            //{
            //    var refreshUrl = $@"/jgapi/devkey/verify";
            //    await RefreshToken(tokenResult.AccessToken, baseUrl, refreshUrl);
            //}
            return chongqingTokenResult;
        }


        /// <summary>
        /// 根据用户名密码获取idServer4 返回的token
        /// </summary>
        /// <param name="tokenUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="grantType"></param>
        /// <param name="scope"></param>
        /// <returns>token jwt格式</returns>
        public static async Task<string> GetIotOpsToken(string tokenUrl, string userName, string password, string clientId, string clientSecret, string grantType = "password", string scope = null)
        {
            try
            {
                var input = new
                {
                    username = userName,
                    password = password,
                    client_id = clientId,
                    client_secret = clientSecret,
                    grant_type = grantType,
                    scope = scope
                };
                Console.WriteLine($"Json序列化input:");
                Console.WriteLine($"{JsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input))}");
                //var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);

                Console.WriteLine($"请求token地址:{tokenUrl}");
                var list = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("username", input.username),
                    new KeyValuePair<string, string>("password", input.password),
                    new KeyValuePair<string, string>("client_id", input.client_id),
                    new KeyValuePair<string, string>("client_secret", input.client_secret),
                    new KeyValuePair<string, string>("grant_type", input.grant_type),
                    new KeyValuePair<string, string>("scope", input.scope)
                };
                var content = new FormUrlEncodedContent(list);
                //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
                var result = await _httpClient.PostAsync($"{tokenUrl}", content);
                var tokenString = await result.Content.ReadAsStringAsync();
                Console.WriteLine($"token请求结果:\n{JsonConvert.DeserializeObject<object>(tokenString)}");
                if (!result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"{tokenUrl}请求失败！");
                }
                return tokenString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }


        /// <summary>
        /// 获取token方法 缓存一个token 发现失败再执行刷新token  刷新token的api文档还没呢,只有直接请求的api？
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="password"></param>
        /// <param name="baseUrl"></param>
        /// <param name="apiUrl"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task<SichuanTokenResultDto> GetSichuanAccessToken(string appKey, string appSecret, string account, string password, string baseUrl = null, string apiUrl = null)
        {
            //DateTime.Now.Subtract(CreateTime) > TimeSpan.FromSeconds(ExpiresIn)
            //如果没有accessToken或者token过期(12h) 才重新请求token
            if (string.IsNullOrWhiteSpace(sichuanTokenResult.Data.Access_token) || sichuanTokenResult.Data.IsExpired())
            {
                #region _httpClient

                var input = new
                {
                    account = account,
                    password = password,
                    app_key = appKey,
                    app_secret = appSecret
                };
                Console.WriteLine($"Json序列化input:");
                Console.WriteLine($"{JsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input))}");
                var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);
                //content.Headers.Add("ContentType", "application/json; charset=utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                Console.WriteLine($"请求地址:{baseUrl}{apiUrl}");
                var result = await _httpClient.PostAsync($"{baseUrl}{apiUrl}", content);
                var msg = await result.Content.ReadAsStringAsync();
                Console.WriteLine($"token请求结果:\n{JsonConvert.DeserializeObject<object>(msg)}");
                if (!result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"{baseUrl}{apiUrl}请求失败！");
                }
                var token = JsonConvert.DeserializeObject<SichuanTokenResultDto>(msg)?.Data;
                #endregion

                #region 请求结果保存到redis 此处用静态变量存放

                sichuanTokenResult.Data.CreateTime = DateTime.Now;
                sichuanTokenResult.Data.Access_token = token.Access_token;
                sichuanTokenResult.Data.Expires_in = token.Expires_in;
                #endregion
            }
            return sichuanTokenResult;
        }


    }
}
