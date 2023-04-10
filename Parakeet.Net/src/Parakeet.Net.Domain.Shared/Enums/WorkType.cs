using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 投入性质
    /// </summary>
    [Description("投入性质")]
    public enum WorkType
    {
        /// <summary>
        /// 开发
        /// </summary>
        [Description("开发")]
        开发 = 0,
        /// <summary>
        /// 测试
        /// </summary>
        [Description("测试")]
        测试 = 10,
        /// <summary>
        /// 变更
        /// </summary>
        [Description("变更")]
        变更 = 20,
        /// <summary>
        /// 设计
        /// </summary>
        [Description("设计")]
        设计 = 30
    }
}
