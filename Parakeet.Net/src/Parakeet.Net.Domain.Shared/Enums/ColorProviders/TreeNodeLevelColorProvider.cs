using System;
using System.ComponentModel;
using System.Drawing;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 状态颜色控制
    /// </summary>
    [Description("状态颜色控制")]
    public class TreeNodeLevelColorProvider : IColorProvider
    {
        /// <summary>
        /// 获取枚举值对应颜色
        /// </summary>
        /// <param name="source">枚举值</param>
        /// <returns></returns>
        [Description("获取枚举值对应颜色")]
        public Color GetColor(Enum source)
        {
            Color color;
            switch (source)
            {
                case TreeNodeLevel.根级:
                    color = Color.Gold;
                    break;
                case TreeNodeLevel.一级:
                    color = Color.Tomato;
                    break;
                case TreeNodeLevel.二级:
                    color = Color.Transparent;
                    break;
                case TreeNodeLevel.三级:
                    color = Color.Transparent;
                    break;
                case TreeNodeLevel.四级:
                    color = Color.Transparent;
                    break;
                case TreeNodeLevel.五级:
                    color = Color.Transparent;
                    break;
                case TreeNodeLevel.六级:
                    color = Color.Transparent;
                    break;
                case TreeNodeLevel.七级:
                    color = Color.Transparent;
                    break;
                case TreeNodeLevel.八级:
                    color = Color.Transparent;
                    break;
                case TreeNodeLevel.九级:
                    color = Color.Transparent;
                    break;
                case TreeNodeLevel.十级:
                    color = Color.Transparent;
                    break;
                default:
                    color = Color.Transparent;
                    break;
            }
            return color;
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
