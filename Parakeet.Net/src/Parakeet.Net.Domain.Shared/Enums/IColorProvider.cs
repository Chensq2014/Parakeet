using System;
using System.ComponentModel;
using System.Drawing;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 定义枚举颜色接口
    /// </summary>
    [Description("枚举状态颜色")]
    public interface IColorProvider
    {
        /// <summary>
        /// 获取枚举值对应颜色
        /// </summary>
        /// <param name="enummer">枚举值</param>
        /// <returns></returns>
        [Description("获取枚举值对应颜色")]
        Color GetColor(int enummer);
        /// <summary>
        /// 获取项对应颜色
        /// </summary>
        /// <param name="source">枚举项</param>
        /// <returns></returns>
        [Description("获取枚举值对应颜色")]
        Color GetColor(Enum source);
    }
}
