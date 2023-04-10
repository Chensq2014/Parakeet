using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 机构类型
    /// </summary>
    [Description("机构类型")]
    public enum OrganizationType
    {
        /// <summary>
        /// 岗位
        /// </summary>
        [Description("岗位")]
        Post = 0,
        /// <summary>
        /// 部门
        /// </summary>
        [Description("部门")]
        Department = 10,
        /// <summary>
        /// 公司
        /// </summary>
        [Description("公司")]
        Company = 20,
        /// <summary>
        /// 城市
        /// </summary>
        [Description("城市")]
        City = 30,
        /// <summary>
        /// 集团
        /// </summary>
        [Description("集团")]
        Group = 40
    }
}
