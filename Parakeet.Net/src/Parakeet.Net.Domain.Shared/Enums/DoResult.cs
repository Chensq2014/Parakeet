using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 请求状态
    /// </summary>
    [Description("请求状态")]
    public enum DoResult
    {
        /// <summary>
        /// 请求失败
        /// </summary>
        [Description("请求失败")]
        Failed = 0,
        /// <summary>
        /// 请求成功
        /// </summary>
        [Description("请求成功")]
        Success = 10,
        /// <summary>
        /// 请求超时
        /// </summary>
        [Description("请求超时")]
        OverTime = 20,
        /// <summary>
        /// 未授权
        /// </summary>
        [Description("未授权")]
        NoAuthorization = 30,
        /// <summary>
        /// 其它错误
        /// </summary>
        [Description("其它错误")]
        Other = 100
    }
}
