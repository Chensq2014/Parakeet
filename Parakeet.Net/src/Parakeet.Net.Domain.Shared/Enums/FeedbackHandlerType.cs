using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 设备处理类型
    /// </summary>
    [Description("设备处理类型")]
    public enum FeedbackHandlerType
    {
        /// <summary>
        /// 注册
        /// </summary>
        [Description("注册")]
        Register = 1,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 2,
        /// <summary>
        /// 更新
        /// </summary>
        [Description("更新")]
        Update = 3
    }
}
