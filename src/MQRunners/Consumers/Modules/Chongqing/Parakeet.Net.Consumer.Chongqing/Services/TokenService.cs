using Newtonsoft.Json;
using Parakeet.Net.Consumer.Chongqing.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parakeet.Net.Consumer.Chongqing.Services
{
    /// <summary>
    /// 获取Chongqing token
    /// </summary>
    public class TokenService
    {
        /// <summary>
        /// 获取token方法 缓存一个token 发现失败再执行刷新token
        /// </summary>
        /// <param name="tokenConfigDto">appKey appSecret baseUrl apiUrl</param>
        /// <returns></returns>
        public static async Task<TokenResultDto> GetAccessToken(TokenConfigDto tokenConfigDto)
        {
            //如果没有accessToken或者token过期 才重新请求token
            var tokenResult = new TokenResultDto();
            if (string.IsNullOrWhiteSpace(tokenResult.Custom?.Access_token) || tokenResult.Custom.IsExpired())
            {
                #region restClient

                //var tokenClient = new RestClient(tokenConfigDto.HostPortString);
                //var tokenRequest = new RestRequest(tokenConfigDto.Uri, Method.POST);
                ////tokenRequest.AddJsonBody(new
                ////{
                ////    client_id=appKey,
                ////    client_secret= appSecret,
                ////    grant_type="client_credentials"
                ////});
                //tokenRequest.AddParameter("client_id", tokenConfigDto.AppKey);
                //tokenRequest.AddParameter("client_secret", tokenConfigDto.AppSecret);
                //tokenRequest.AddParameter("grant_type", tokenConfigDto.GrantType?? "client_credentials");

                //var tokenResponse = await tokenClient.ExecuteAsync(tokenRequest);
                //var token = JsonConvert.DeserializeObject<TokenResultDto>(tokenResponse.Content);
                //if (token.Status.Code == 0)
                //{
                //    //logger.LogError("请求token失败,请检查设备所在项目devKey是否可用...");
                //    Console.WriteLine($"请求token失败,请检查设备appKey是否可用...");
                //    return null;
                //}

                #endregion

                #region _httpClient

                var httpClient = new HttpClient();

                var input = new
                {
                    client_id = tokenConfigDto.AppKey,
                    client_secret = tokenConfigDto.AppSecret,
                    grant_type = tokenConfigDto.GrantType ?? "client_credentials"
                };

                var list = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("client_id", input.client_id),
                    new KeyValuePair<string, string>("client_secret", input.client_secret),
                    new KeyValuePair<string, string>("grant_type", input.grant_type),
                };
                var content = new FormUrlEncodedContent(list);
                //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
                var result = await httpClient.PostAsync($"{tokenConfigDto.Url}", content);
                var msg = await result.Content.ReadAsStringAsync();
                //Console.WriteLine($"token请求结果:{msg}");
                if (!result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"{tokenConfigDto.Url}请求失败！");
                    return tokenResult;
                }

                tokenResult = JsonConvert.DeserializeObject<TokenResultDto>(msg);
                #endregion
            }



            return tokenResult;
        }
    }
}
