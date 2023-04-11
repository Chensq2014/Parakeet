using System;
using System.Collections.Generic;
using System.Text;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 缓存token
    /// </summary>
    public class TokenResultDto
    {
        /// <summary>
        /// access_token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// token创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 过期时间戳(秒) 默认12小时
        /// </summary>
        public long ExpiresIn { get; set; } = 3600 * 12;

        public bool IsExpired()
        {
            return DateTime.Now.Subtract(CreateTime) > TimeSpan.FromSeconds(ExpiresIn);
        }
    }
}
