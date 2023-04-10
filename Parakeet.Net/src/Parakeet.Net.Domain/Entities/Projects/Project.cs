using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Entities.Devices;
using Parakeet.Net.Entities.Organizations;
using Parakeet.Net.Events;
using Parakeet.Net.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Parakeet.Net.Entities.LocationAreas;

namespace Parakeet.Net.Entities.Projects
{
    /// <summary>
    /// 项目
    /// </summary>
    [Description("项目")]
    [Table("Parakeet_Projects", Schema = "parakeet")]
    public class Project : BaseEntity
    {
        public Project()
        {
        }
        public Project(Guid id)
        {
            SetEntityPrimaryKey(id);
        }

        #region 基础字段

        /// <summary>
        /// 课程期数
        /// </summary>
        [Description("期数")]
        public decimal? Period { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255), Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [Description("开始日期")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 计划结束日期
        /// </summary>
        [Description("计划结束日期")]
        public DateTime? PlanEndDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [Description("结束日期")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        [Description("总金额")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// 百分比
        /// </summary>
        [Description("百分比")]
        public decimal? Percent { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [Description("单价")]
        public decimal? Price { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength2048), Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 项目示意图文件服务器虚拟路径
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64), Description("项目示意图")]
        public string MapPath { get; set; }

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

        #region 项目附件

        /// <summary>
        /// 项目附件
        /// </summary>
        [NotSet, Description("项目附件")]
        public virtual HashSet<ProjectAttachment> Attachments { get; set; } = new HashSet<ProjectAttachment>();

        /// <summary>
        /// 添加项目附件
        /// </summary>
        /// <param name="attachments"></param>
        public virtual void AddProjectAttachments(IList<ProjectAttachment> attachments)
        {
            if (attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    Attachments.Add(attachment);
                }
            }
        }

        /// <summary>
        /// 移除当前项目所有项目附件
        /// 如果没有使用文件服务，还需要移除附件物理文件
        /// </summary>
        public virtual void RemoveAllProjectAttachments()
        {
            if (Attachments.Any(m => m.ProjectId == Id))
            {
                Attachments.RemoveAll(m => m.ProjectId == Id);
            }
        }
        #endregion

        #region 项目用户

        /// <summary>
        /// 项目用户/学员
        /// </summary>
        [NotSet, Description("项目用户")]
        public virtual HashSet<ProjectUser> ProjectUsers { get; set; } = new HashSet<ProjectUser>();

        /// <summary>
        /// 添加项目用户
        /// </summary>
        /// <param name="projectUsers"></param>
        public virtual void AddProjectUsers(IList<ProjectUser> projectUsers)
        {
            if (projectUsers.Any())
            {
                foreach (var projectUser in projectUsers)
                {
                    ProjectUsers.Add(projectUser);
                }
            }
        }

        /// <summary>
        /// 移除当前项目所有项目用户
        /// </summary>
        public virtual void RemoveAllProjectUsers()
        {
            if (ProjectUsers.Any(m => m.ProjectId == Id))
            {
                ProjectUsers.RemoveAll(m => m.ProjectId == Id);
            }
        }

        #endregion

        #region 项目设备

        /// <summary>
        /// 项目设备
        /// </summary>
        public virtual ICollection<Device> Devices { get; set; } = new HashSet<Device>();

        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="devices"></param>
        public virtual void AddDevices(IList<Device> devices)
        {
            if (devices.Any())
            {
                foreach (var device in devices)
                {
                    Devices.Add(device);
                }
            }
        }

        /// <summary>
        /// 当前项目所有设备的projectId 置空
        /// </summary>
        public virtual void RemoveAllDevices()
        {
            if (Devices.Any(m => m.ProjectId == Id))
            {
                foreach (var device in Devices)
                {
                    device.ProjectId = null;//设备的projectId置空
                }
            }
        }

        #endregion

        #region 组织

        /// <summary>
        /// 组织外键
        /// </summary>
        [Description("组织外键")]
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        [NotSet, Description("组织")]
        public virtual Organization Organization { get; set; }

        #endregion

        #region 修改项目信息等成员函数

        /// <summary>
        ///     修改项目名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Project SetName(string name)
        {
            if (Name != name)
            {
                AddLocalEvent(new RemoveCacheEvent(CustomerConsts.UserLocker));
                //AddDistributedEvent(new RemoveCacheEvent(CustomerConsts.UserLocker));
                Name = name;
            }

            return this;
        }

        #endregion
    }
}
