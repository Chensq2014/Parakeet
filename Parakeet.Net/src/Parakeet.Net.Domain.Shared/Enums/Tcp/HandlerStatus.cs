using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 处理状态
    /// </summary>
    [Description("处理状态")]
    public enum HandlerStatus
    {
        /// <summary>
        /// 中断
        /// </summary>
        [Description("中断")]
        Break = 0,

        /// <summary>
        /// 继续
        /// </summary>
        [Description("继续")]
        Continue = 1,

        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 2
    }
}
