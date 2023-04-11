using System;

namespace Parakeet.Net.Dtos
{
    public class WeixinAccessTokenReturnDto : WebClientResultBase
    {
        /// <summary>
        /// 应用token
        /// </summary>
        public string Access_token { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public long Expires_in { get; set; }

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
