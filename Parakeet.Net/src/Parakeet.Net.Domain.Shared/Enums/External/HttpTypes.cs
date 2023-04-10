using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 区域状态
    /// </summary>
    [Description("区域状态")]
    public enum HttpTypes
    {
        /// <summary>
        /// Https安全协议网络请求
        /// </summary>
        [Description("HTTPS")]
        HTTPS = 0,
        /// <summary>
        /// Http标准
        /// </summary>
        [Description("HTTP")]
        HTTP = 1
    }
}
