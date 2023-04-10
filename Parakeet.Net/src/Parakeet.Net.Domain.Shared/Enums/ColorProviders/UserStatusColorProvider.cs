using System;
using System.ComponentModel;
using System.Drawing;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 状态颜色控制
    /// </summary>
    [Description("状态颜色控制")]
    public class UserStatusColorProvider : IColorProvider
    {
        /// <summary>
        /// 获取枚举值对应颜色
        /// </summary>
        /// <param name="source">枚举值</param>
        /// <returns></returns>
        [Description("获取枚举值对应颜色")]
        public Color GetColor(Enum source)
        {
            switch (source)
            {
                case UserStatus.Active:
                    return Color.Gold;
                case UserStatus.Disable:
                    return Color.Tomato;
                default:
                    return Color.Transparent;
            }
        }
        /// <summary>
        /// 获取枚举值对应颜色
        /// </summary>
        /// <param name="enummer">枚举值</param>
        /// <returns></returns>
        [Description("获取枚举值对应颜色")]
        public Color GetColor(int enummer)
        {
            switch (enummer)
            {
                case (int)UserStatus.Active:
                    return Color.Gold;
                case (int)UserStatus.Disable:
                    return Color.Tomato;
            }
            return Color.Transparent;
        }

    }
}
