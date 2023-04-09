using System.ComponentModel;

namespace Parakeet.Net.Enums
{

    /// <summary>
    /// 通知类型
    /// </summary>
    [Description("通知类型")]
    public enum NotifyType
    {
        /// <summary>
        /// 系统消息
        /// </summary>
        [Description("系统消息")]
        系统消息 = 0,
        /// <summary>
        /// 应用消息
        /// </summary>
        [Description("应用消息")]
        应用消息 = 10
    }
}
