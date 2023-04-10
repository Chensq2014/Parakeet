using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 区域状态
    /// </summary>
    [Description("区域状态")]
    public enum AreaStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Active = 0,
        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        Deactive = 1
    }
}
