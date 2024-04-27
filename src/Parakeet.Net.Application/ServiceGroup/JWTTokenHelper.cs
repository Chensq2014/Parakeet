using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.Dtos;

namespace Parakeet.Net.ServiceGroup
{
    public class JWTTokenHelper
    {
        // ReSharper disable once ArrangeModifiersOrder
        public async static Task<string> GetJWTToken()
        {
            //string result = await PostWebQuest();
            string result = await PostClient();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<JWTTokenResult>(result).Token;
        }


        #region HttpClient实现Post请求
        /// <summary>
        /// HttpClient实现Post请求
        /// </summary>
        // ReSharper disable once ArrangeModifiersOrder
        private async static Task<string> PostClient()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                {"Name","Chensq" },
                {"Password","123456" }
            };
            string url = "https://localhost:5000/api/Authentication/Login?name=Chensq&password=123456";
            HttpClientHandler handler = new HttpClientHandler();
            using (var http = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(dic);
                var response = await http.PostAsync(url, content);
                Console.WriteLine(response.StatusCode); //确保HTTP成功状态值
                return await response.Content.ReadAsStringAsync();
            }
        }
        #endregion

        #region  HttpWebRequest实现post请求
        /// <summary>
        /// HttpWebRequest实现post请求
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once ArrangeModifiersOrder
        private async static Task<string> PostWebQuest()
        {
            var user = new
            {
                Name = "Chensq",
                Password = "123456"
            };
            string url = "http://localhost:9527/api/Authentication/Login?name=Eleven&password=123456";
            var postData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Timeout = 30 * 1000;//设置30s的超时
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.118 Safari/537.36";
            request.ContentType = "application/json";
            request.Method = "POST";
            byte[] data = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = data.Length;
            Stream postStream = request.GetRequestStream();
            postStream.Write(data, 0, data.Length);
            postStream.Close();

            using (var res = request.GetResponse() as HttpWebResponse)
            {
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                    return await reader.ReadToEndAsync();
                }
                else
                {
                    throw new Exception($"请求异常{res.StatusCode}");
                }
            }
        }
        #endregion
    }
}
