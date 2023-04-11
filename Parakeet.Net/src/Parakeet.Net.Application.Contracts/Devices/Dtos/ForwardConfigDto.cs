using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Parakeet.Net.Devices.Dtos
{
    /// <summary>
    /// 转发服务配置信息
    /// </summary>
    public class ForwardConfigDto
    {
        /// <summary>
        /// 转发服务器接收数据的url
        /// </summary>
        public string TargetUrl { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 分组名称 同一个分组获取同一个HttpClient实例 作用域单例
        /// </summary>
        public string HttpClientName { get; set; }

        /// <summary>
        /// 转发方法默认HttpMethod.Post
        /// </summary>
        public HttpMethod HttpMethod = HttpMethod.Post;
    }
}
