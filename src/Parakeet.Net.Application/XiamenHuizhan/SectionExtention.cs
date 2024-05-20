using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Common.Dtos;

namespace Parakeet.Net.XiamenHuizhan
{
    /// <summary>
    /// 扩展
    /// </summary>
    public class SectionExtention
    {
        #region 厦门会展一标段

        /// <summary>
        /// 一标段环境设备实时数据请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<SectionOneEnvironmentReturnData> SectionOneEnvironmentRealTimeRequest(RequestParameterDto input)
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
                //throw new UserFriendlyException($"请求{input.FullUrl}失败");
                return null;
            }
            var content = await result.Content.ReadAsStringAsync();
            var returnObj = JsonConvert.DeserializeObject<SectionOneEnvironmentReturnData>(content);
            return returnObj;
        }


        /// <summary>
        /// 一标段考勤历史记录请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<SectionOneGateReturnData> SectionOneGateHistoryRequest(RequestParameterDto input)
        {
            using var client = new HttpClient();
            input.Host = "http://123.56.74.49";
            input.Port = 6000;
            var deviceId = "887344297819504640";
            input.Api = $"/services/execute/GetProjectAttendanceListNew?deviceId={deviceId}";
            input.Content = new StringContent(JsonConvert.SerializeObject(new
            {
                pageSize = 100,
                pageIndex = input.Next,
                begin = input.StartDate ?? DateTime.Now.AddMinutes(-10),//请求10分钟以前考勤历史数据记录，因为每1分钟请求一次
                end = input.EndDate ?? DateTime.Now
            }), Encoding.UTF8);
            Console.WriteLine($"input：{JsonConvert.DeserializeObject(JsonConvert.SerializeObject(input))}");

            client.DefaultRequestHeaders.Add("Cookie", $"satoken=f9f4183d-5b7b-4c9b-bda7-2401def00833");
            var result = await client.PostAsync($"{input.FullUrl}", input.Content);
            if (!result.IsSuccessStatusCode)
            {
                //throw new UserFriendlyException($"请求{input.FullUrl}失败");
                return null;
            }
            var content = await result.Content.ReadAsStringAsync();
            var returnObj = JsonConvert.DeserializeObject<SectionOneGateReturnData>(content);

            return returnObj;
        }


        #endregion

        #region 厦门会展二标段

        /// <summary>
        /// 二标段环境设备实时数据请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<SectionTwoEnvironmentReturnData> SectionTwoEnvironmentRealTimeRequest(RequestParameterDto input)
        {
            using var client = new HttpClient();
            input.Host = "http://47.107.103.35";
            input.Port = 8010;
            input.Api = "/openApi/data/realtime";
            Console.WriteLine($"input：{JsonConvert.DeserializeObject(JsonConvert.SerializeObject(input))}");
            input.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("SN","MjAyMTA2MDIwMzEwMDAwMw==")
            });
            var result = await client.PostAsync($"{input.FullUrl}", input.Content);
            if (!result.IsSuccessStatusCode)
            {
                //throw new UserFriendlyException($"请求{input.FullUrl}失败");
                return null;
            }
            var content = await result.Content.ReadAsStringAsync();
            var returnObj = JsonConvert.DeserializeObject<SectionTwoEnvironmentReturnData>(content);

            return returnObj;
        }


        /// <summary>
        /// 2标段考勤入历史记录请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<SectionTwoGateReturnData> SectionTwoGateHistoryRequest(RequestParameterDto input)
        {
            using var client = new HttpClient();
            input.Host = "https://glm.glodon.com/api/open";
            input.Port = 443;
            //这里根据appid、secret计算出签名sign 放在url里面或者post参数里面请求接口
            var secret = "bd1b6322217dfbeb96de3696d1a387fa";
            var gateInput = new
            {
                appid = "0a9416fd6baa4595a1268ad1f52034b0",
                //secret = "bd1b6322217dfbeb96de3696d1a387fa",
                //sign = "",//调用一个方法计算sign
                beginDate = $"{input.StartDate ?? DateTime.Now.AddMinutes(-10):yyyy-MM-dd}",//10分钟请求一次，请求10分钟以前进出记录
                endDate = $"{input.EndDate ?? DateTime.Now:yyyy-MM-dd}",
                //inOutType = input.InOutType,//"IN",//IN进，OUT出
                inOutType = "IN",//"IN",//IN进，OUT出
                projectId = "490952877121536",//租户id：1438415,
                startId = input.Next,
                pageSize = 100
            };

            var sign = GetXiamenSectionTwoSign(secret, gateInput);

            input.Api = $"/attendance/cardV2?appid={gateInput.appid}&sign={sign}&beginDate={gateInput.beginDate}&endDate={gateInput.endDate}&inOutType={gateInput.inOutType}&projectId={gateInput.projectId}&startId={gateInput.startId}&pageSize={gateInput.pageSize}";

            Console.WriteLine($"input：{JsonConvert.DeserializeObject(JsonConvert.SerializeObject(input))}");
            var result = await client.GetAsync($"{input.FullUrl}");
            if (!result.IsSuccessStatusCode)
            {
                //throw new UserFriendlyException($"请求{input.FullUrl}失败");
                Console.WriteLine($"请求{input.FullUrl}失败");
                return null;
            }
            var content = await result.Content.ReadAsStringAsync();
            var returnObj = JsonConvert.DeserializeObject<SectionTwoGateReturnData>(content);
            return returnObj;
        }

        #endregion

        #region 签名
        /// <summary>
        /// 获取厦门会展二标段劳务实名制网站加密
        /// </summary>
        /// <param name="appSecret">密钥</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        private static string GetXiamenSectionTwoSign(string appSecret, object input)
        {
            var propInfos = input.GetType().GetProperties().ToList();
            propInfos.Sort((m, n) => string.CompareOrdinal(m.Name, n.Name));
            var beforeMd5 = new StringBuilder();
            beforeMd5.Append(appSecret);
            foreach (var propertyInfo in propInfos)
            {
                beforeMd5.Append(propertyInfo.Name);
                beforeMd5.Append(propertyInfo.GetValue(input));
            }
            beforeMd5.Append(appSecret);
            var md5 = new MD5CryptoServiceProvider();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(beforeMd5.ToString()));
            var sign = ConvertBinaryToHexValueString(bytes);
            return sign;
        }

        /// <summary>
        /// 转换二进制位16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string ConvertBinaryToHexValueString(IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder();
            foreach (var _byte in bytes)
            {
                sb.Append($"{_byte:X2}");
            }
            return sb.ToString();
        }
        #endregion
    }
}
