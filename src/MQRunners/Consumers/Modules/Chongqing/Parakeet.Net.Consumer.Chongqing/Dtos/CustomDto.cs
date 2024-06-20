using System;

namespace Parakeet.Net.Consumer.Chongqing.Dtos
{
    public class CustomDto
    {
        /// <summary>
        /// Access_token
        /// </summary>
        public string Access_token { get; set; }
        /// <summary>
        /// Refresh_token
        /// </summary>
        public string Refresh_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Jsessionid { get; set; }

        /// <summary>
        /// 过期时间戳(秒)
        /// </summary>
        public string Expires_in { get; set; }

        /// <summary>
        /// token创建时间 默认DateTime.Now更新token后再更新它为当前DateTime.Now
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public bool IsExpired()
        {
            return DateTime.Now.Subtract(CreateTime) > TimeSpan.FromSeconds(Convert.ToInt64(Expires_in));
        }
    }
}
