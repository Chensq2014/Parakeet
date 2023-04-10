using System;
using System.ComponentModel;
using System.Drawing;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 状态颜色控制
    /// </summary>
    [Description("状态颜色控制")]
    public class PlanStatusColorProvider : IColorProvider
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
                case PlanStatus.未开始:
                    return Color.LightBlue;
                case PlanStatus.进行中:
                    return Color.Orange;
                case PlanStatus.已存档:
                    return Color.Teal;
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
                case (int)PlanStatus.未开始:
                    return Color.LightBlue;
                case (int)PlanStatus.进行中:
                    return Color.Orange;
                case (int)PlanStatus.已存档:
                    return Color.Teal;
            }
            return Color.Transparent;
        }
    }
}
