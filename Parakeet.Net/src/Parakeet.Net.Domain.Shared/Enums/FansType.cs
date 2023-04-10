using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 粉丝类型
    /// </summary>
    [Description("粉丝/人员类型")]
    public enum FansType
    {
        /// <summary>
        /// 白名单
        /// </summary>
        [Description("白名单")]
        白名单 = 0,
        /// <summary>
        /// 访客
        /// </summary>
        [Description("访客")]
        访客 = 10,
        /// <summary>
        /// 陌生人
        /// </summary>
        [Description("陌生人")]
        陌生人 = 20,
        /// <summary>
        /// 陌生人
        /// </summary>
        [Description("黑名单")]
        黑名单 = 30
    }
}
