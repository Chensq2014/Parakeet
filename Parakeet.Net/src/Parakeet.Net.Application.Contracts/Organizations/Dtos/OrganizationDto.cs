using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Projects.Dtos;
using Parakeet.Net.Dtos;
using Parakeet.Net.Enums;
using Parakeet.Net.ValueObjects;

namespace Parakeet.Net.Organizations.Dtos
{
    /// <summary>
    /// 组织机构
    /// </summary>
    [Description("组织机构")]
    public class OrganizationDto : BaseDto//, IValidatableObject
    {
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
        public virtual LocationAreaDto LocationArea { get; set; }

        #endregion

        #region 机构/岗位用户

        /// <summary>
        /// 用户 多个用户
        /// </summary>
        [Description("用户"), NotSet]
        public virtual List<OrganizationUserDto> Users { get; set; } = new List<OrganizationUserDto>();

        #endregion

        #region 项目列表
        /// <summary>
        /// 项目列表
        /// </summary>
        [Description("项目列表"), NotSet]
        public virtual List<ProjectDto> Projects { get; set; } = new List<ProjectDto>();

        #endregion

        #region 父/子级机构

        /// <summary>
        /// 父级机构Id
        /// </summary>
        [Description("父级机构Id")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 父级机构名称
        /// </summary>
        [Description("父级机构名称")]
        public string ParentName => Parent?.Name;

        /// <summary>
        /// 父级机构
        /// </summary>
        [Description("父级机构"), NotSet]
        public virtual OrganizationDto Parent { get; set; }

        /// <summary>
        /// 子级机构
        /// </summary>
        [Description("子级机构"), NotSet]
        public virtual List<OrganizationDto> Children { get; set; } = new List<OrganizationDto>();

        #endregion

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    //if (!Phone.Validate())
        //    //{
        //    //    context.Results.Add(new ValidationResult("请输入正确的电话号码!"));
        //    //}
        //    //if (Files.Count > 20)
        //    //{
        //    //    context.Results.Add(new ValidationResult("批量上传文件最多20个"));
        //    //}
        //}
    }
}
