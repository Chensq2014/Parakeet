using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 源/目标类型
    /// </summary>
    [Description("源/目标类型")]
    public enum SourceOrTargetType
    {
        /// <summary>
        /// 默认
        /// </summary>
        [Description("默认")]
        None = 0,
        /// <summary>
        /// 用户
        /// </summary>
        [Description("用户")]
        User = 10,
        /// <summary>
        /// 员工
        /// </summary>
        [Description("员工")]
        Staff = 20,
        /// <summary>
        /// 企业
        /// </summary> 
        [Description("企业")]
        Company = 30,
        /// <summary>
        /// 项目
        /// </summary> 
        [Description("项目")]
        Project = 40,
        /// <summary>
        /// 项目员工
        /// </summary> 
        [Description("项目员工")]
        ProjectStaff = 50,
        /// <summary>
        /// 项目公司
        /// </summary> 
        [Description("项目公司")]
        ProjectCompany = 60,
        /// <summary>
        /// 部门
        /// </summary> 
        [Description("部门")]
        Department = 70,
        /// <summary>
        /// 申请中的用户
        /// </summary> 
        [Description("申请中的用户")]
        ApplyUser = 80,
        /// <summary>
        /// 申请中的员工
        /// </summary> 
        [Description("申请中的员工")]
        ApplyStaff = 90,
        /// <summary>
        /// 申请/审批流程
        /// </summary> 
        [Description("申请/审批流程")]
        Application = 100
    }
}
