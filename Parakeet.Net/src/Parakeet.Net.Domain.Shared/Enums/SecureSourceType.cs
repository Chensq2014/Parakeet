using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 安全策略数据源类型
    /// None = 0
    /// Role=10
    /// Company=20
    /// Department=30
    /// </summary>
    [Description("安全策略数据源类型")]
    public enum SecureSourceType
    {
        /// <summary>
        /// 未设置
        /// </summary>
        [Description("未设置")]
        None = 0,
        /// <summary>
        /// 角色
        /// </summary>
        [Description("角色")]
        Role = 10,
        /// <summary>
        /// 公司
        /// </summary>
        [Description("公司")]
        Company = 20,

        /// <summary>
        /// 部门
        /// </summary>
        [Description("部门")]
        Department = 30
    }
}
