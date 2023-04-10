using Parakeet.Net.Enums;
using System;
using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// ajax返回的Message类
    /// </summary>
    [Serializable, Description("ajax返回的Message类")]
    public class Message
    {
        /// <summary>
        /// 请求状态
        /// </summary>
        [Description("请求状态")]
        public bool Status { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        [Description("返回消息")]
        public string Msg { get; set; }

        /// <summary>
        /// 返回对象
        /// </summary>
        [Description("返回对象")]
        public object Data { get; set; }

        /// <summary>
        /// Exception错误信息
        /// </summary>
        [Description("Exception错误信息")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        [Description("Url")]
        public string Url { get; set; }

        /// <summary>
        /// 错误状态码 int类型
        /// </summary>
        [Description("错误状态码")]
        public ErrorCode? ErrorCode { get; set; }
    }
}
