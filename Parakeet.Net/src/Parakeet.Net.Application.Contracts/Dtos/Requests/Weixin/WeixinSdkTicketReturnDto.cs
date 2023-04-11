using System;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 企业微信自建/三方应用获取微信Sdk Ticket返回dto
    /// </summary>
    public class WeixinSdkTicketReturnDto: WebClientResultBase
    {
        /// <summary>
        /// ticket
        /// </summary>
        public string Ticket { get; set; }

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
