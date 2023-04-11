using System;
using System.Net.Http;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// httpClient请求参数对象
    /// </summary>
    public class RequestParameterDto
    {
        /// <summary>
        /// 主机/域名
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; } = 80;

        /// <summary>
        /// Api
        /// </summary>
        public string Api { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string FullUrl => $"{Host}{(Port == 80 || Port == 443 ? "" : $":{Port}")}{Api}";

        /// <summary>
        /// 请求内容
        /// </summary>
        public HttpContent Content { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        public HttpMethod HttpMethod = HttpMethod.Post;

        /// <summary>
        /// access_token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 请求下一页参数
        /// </summary>

        public long Next { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 请求结果
        /// </summary>
        public HttpResponseMessage Response { get; set; }

        /// <summary>
        /// 封装请求参数对象
        /// </summary>
        public object RequestObj { get; set; }

        /// <summary>
        /// 请求是否返回对象
        /// </summary>
        public bool ReturnObj { get; set; }
    }
}
