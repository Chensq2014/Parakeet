using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 错误码枚举
    /// </summary>
    [Description("错误码枚举")]
    public enum ErrorCode
    {
        /// <summary>
        /// 没有找到
        /// </summary>
        [Description("没有找到")]
        NotFound = 404,

        /// <summary>
        /// 已被锁定
        /// </summary>
        [Description("已被锁定")]
        CacheLocked = 1000,

        /// <summary>
        /// 已存在
        /// </summary>
        [Description("已存在")]
        Exist = 1001
    }
}
