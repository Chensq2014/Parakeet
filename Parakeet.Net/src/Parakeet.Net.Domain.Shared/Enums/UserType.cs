using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 用户类型
    /// </summary>
    [Description("用户类型")]
    public enum UserType
    {
        /// <summary>
        /// 董事
        /// </summary>
        [Description("董事")]
        President = 0,
        /// <summary>
        /// 经理
        /// </summary>
        [Description("经理")]
        Manager = 10,
        /// <summary>
        /// 财务
        /// </summary>
        [Description("财务")]
        Financial = 20,
        /// <summary>
        /// 合伙人
        /// </summary>
        [Description("合伙人")]
        Partner = 30,
        /// <summary>
        /// 校长
        /// </summary>
        [Description("校长")]
        Principal = 40,
        /// <summary>
        /// 班主任
        /// </summary>
        [Description("班主任")]
        Headmaster = 50,
        /// <summary>
        /// 高级学员
        /// </summary>
        [Description("高级学员")]
        SeniorStudent = 60,
        /// <summary>
        /// 中级学员
        /// </summary>
        [Description("中级学员")]
        IntermediateStudent = 70,
        /// <summary>
        /// 初级学员
        /// </summary>
        [Description("初级学员")]
        JuniorStudent = 80
    }
}
