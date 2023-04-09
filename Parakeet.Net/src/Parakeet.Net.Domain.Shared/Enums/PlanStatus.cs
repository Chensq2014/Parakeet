using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 计划状态
    /// </summary>
    [Description("计划状态")]
    public enum PlanStatus
    {
        /// <summary>
        /// 未开始
        /// </summary>
        [Description("未开始")]
        未开始 = 10,
        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        进行中 = 20,
        /// <summary>
        /// 已存档
        /// </summary>
        [Description("已存档")]
        已存档 = 30
    }
}
