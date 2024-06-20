using System;
using System.Collections.Generic;

namespace ConsoleApp.Dtos
{
    /// <summary>
    /// iot_ops idserver4 返回token格式
    /// </summary>
    public class IotOpsResultDto
    {
        /// <summary>
        /// token_type
        /// </summary>
        public string Token_type { get; set; }

        /// <summary>
        /// scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// access_token
        /// </summary>
        public string Access_token { get; set; }

        /// <summary>
        /// expires_in 过期时间戳(秒) 访问令牌超时时间（以秒为单位），缺省24小时 
        /// </summary>
        public long Expires_in { get; set; }

        /// <summary>
        /// errors
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// token创建时间 默认DateTime.Now更新token后再更新它为当前DateTime.Now
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否过期
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return DateTime.Now.Subtract(CreateTime) > TimeSpan.FromSeconds(Expires_in);
        }
    }
}
