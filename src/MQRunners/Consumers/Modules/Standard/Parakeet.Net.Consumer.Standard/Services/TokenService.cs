using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parakeet.Net.Consumer.Standard.Dtos;

namespace Parakeet.Net.Consumer.Standard.Services
{
    public class TokenService
    {
        //private static readonly TokenResultDto tokenResult = new TokenResultDto{Data = new TokenResultDataDto()};

        /// <summary>
        /// 获取token方法
        /// </summary>
        /// <param name="tokenConfigDto">account password appKey appSecret baseUrl apiUrl</param>
        /// <returns></returns>
        public static async Task<TokenResultDto> GetAccessToken(TokenConfigDto tokenConfigDto)
        {
            //DateTime.Now.Subtract(CreateTime) > TimeSpan.FromSeconds(ExpiresIn)
            //如果没有accessToken或者token过期 才重新请求token
            var tokenResult = new TokenResultDto { Data = new TokenResultDataDto() };
            if (string.IsNullOrWhiteSpace(tokenResult?.Data.Access_token) || tokenResult.Data.IsExpired())
            {
                #region _httpClient

                var httpClient = new HttpClient();

                var input = new
                {
                    account = tokenConfigDto.Account,
                    password = tokenConfigDto.Password,
                    app_key = tokenConfigDto.AppKey,
                    app_secret = tokenConfigDto.AppSecret
                };
                var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8);
                //content.Headers.Add("ContentType", "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await httpClient.PostAsync($"{tokenConfigDto.Url}", content);
                var msg = await result.Content.ReadAsStringAsync();

                if (!result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"{tokenConfigDto.Url}请求token失败！");
                    return tokenResult;
                }
                tokenResult = JsonConvert.DeserializeObject<TokenResultDto>(msg);
                #endregion

                #region 请求结果保存到redis 此处用静态变量存放
                //tokenResult.Data.CreateTime = DateTime.Now;
                //tokenResult.Data.Access_token = token.Access_token;
                //tokenResult.Data.Expires_in = token.Expires_in;

                #endregion

            }

            return tokenResult;
        }
    }
}
