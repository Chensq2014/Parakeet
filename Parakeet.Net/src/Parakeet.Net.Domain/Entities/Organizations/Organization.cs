using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Enums;
using Parakeet.Net.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 组织机构/岗位
    /// 设计：1、类型为岗位的组织有多个用户(学员) 用户(学员)有多个岗位
    ///       2、机构都有项目(课程)列表，项目(课程)只有一个机构
    /// 注：这里的岗位设计在机构层面，对岗位分配数据权限,所有表均有机构外键 过滤数据用
    ///     用户(学员)中有学员类型(级别)  这样设计是为了学员与公司员工共用User表
    /// 另：项目(课程)可包含多个用户(学员),用户(学员)可以参加多个项目(课程)
    /// </summary>
    [Description("组织机构/岗位")]
    public class Organization : BaseEntity
    {

        public Organization()
        {
        }


        public Organization(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

        #region 机构/岗位基础字段
        /// <summary>
        /// 机构名称
        /// </summary>
        [Description("名称"), MaxLength(CustomerConsts.MaxLength255)]
        public string Name { get; set; }

        /// <summary>
        /// 机构代码
        /// </summary>
        [Description("代码"), MaxLength(CustomerConsts.MaxLength64)]
        public string Code { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Description("类型")]
        public OrganizationType OrganizationType { get; set; }

        /// <summary>
        /// 节点层级
        /// </summary>
        [Description("层级")]
        public TreeNodeLevel Level { get; set; }

        #endregion

        #region 地址/位置信息
        /// <summary>
        /// 所属地区
        /// </summary>
        [Description("所属地区")]
        public virtual Address Address { get; set; } = new Address();

        /// <summary>
        ///     区域Id
        /// </summary>
        [Description("区域Id")]
        public Guid? LocationAreaId { get; set; }

        /// <summary>
        ///     所在区域
        /// </summary>
        [Description("区域")]
        public virtual LocationArea LocationArea { get; set; }
        #endregion

        #region 机构/岗位用户
        /// <summary>
        /// 用户 多个用户
        /// </summary>
        [Description("用户"), NotSet]
        public virtual HashSet<OrganizationUser> Users { get; set; } = new HashSet<OrganizationUser>();


        /// <summary>
        /// 添加构/岗位用户
        /// </summary>
        /// <param name="organizationUsers"></param>
        public virtual void AddUsers(IList<OrganizationUser> organizationUsers)
        {
            if (organizationUsers.Any())
            {
                foreach (var organizationUser in organizationUsers)
                {
                    Users.Add(organizationUser);
                }
            }
        }

        /// <summary>
        /// 移除当前项目所有机构/岗位用户
        /// </summary>
        public virtual void RemoveAllUsers()
        {
            if (Users.Any(m => m.OrganizationId == Id))
            {
                Users.RemoveAll(m => m.OrganizationId == Id);
            }
        }

        #endregion

        #region 项目列表
        /// <summary>
        /// 项目列表
        /// </summary>
        [Description("项目列表"), NotSet]
        public virtual HashSet<Project> Projects { get; set; } = new HashSet<Project>();

        #endregion

        #region 父/子级机构
        /// <summary>
        /// 父级机构Id
        /// </summary>
        [Description("父级机构Id")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 父级机构
        /// </summary>
        [Column("ParentId"), Description("父级机构"), NotSet]//[ForeignKey("ParentId")]
        public virtual Organization Parent { get; set; }

        /// <summary>
        /// 子级机构
        /// </summary>
        [Description("子级机构"), NotSet]
        public virtual HashSet<Organization> Children { get; set; } = new HashSet<Organization>();

        #endregion
    }
}
