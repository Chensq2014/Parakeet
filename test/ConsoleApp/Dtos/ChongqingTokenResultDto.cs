using System;
using System.Collections.Generic;

namespace ConsoleApp.Dtos
{
    /// <summary>
    /// 重庆tokenResult
    /// </summary>
    public class ChongqingTokenResultDto
    {
        /// <summary>
        /// controls
        /// </summary>
        public List<string> Controls { get; set; } = new List<string>();
        /// <summary>
        /// token
        /// </summary>
        public CustomDto Custom { get; set; } = new CustomDto();
        /// <summary>
        /// 状态
        /// </summary>
        public StatusDto Status { get; set; } = new StatusDto();
    }

    /// <summary>
    /// CustomDto
    /// </summary>
    public class CustomDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Access_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Refresh_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Jsessionid { get; set; }

        /// <summary>
        /// 过期时间戳(秒) 默认12小时
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

    /// <summary>
    /// StatusDto
    /// </summary>
    public class StatusDto
    {
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// text
        /// </summary>
        public string Text { get; set; }
    }
}
