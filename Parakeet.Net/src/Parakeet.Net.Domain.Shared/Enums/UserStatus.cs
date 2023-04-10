using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 用户状态(激活/禁用)
    /// </summary>
    [Description("用户状态(激活/禁用)")]
    public enum UserStatus
    {
        /// <summary>
        /// 激活
        /// </summary>
        [Description("激活")]
        Active = 0,
        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        Disable = 10,
        /// <summary>
        /// 封号/锁定
        /// </summary>
        [Description("锁定")]
        Locking = 20,
    }
}
