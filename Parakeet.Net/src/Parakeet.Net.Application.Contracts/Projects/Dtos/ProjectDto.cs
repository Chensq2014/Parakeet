using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Dtos;
using Parakeet.Net.Dtos;
using Parakeet.Net.Dtos;
using Parakeet.Net.ValueObjects;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 项目
    /// </summary>
    [Description("项目")]
    public class ProjectDto : BaseDto
    {
        #region 基础字段

        /// <summary>
        /// 创建页面没有保存数据前，使用保存文件功能保存的缓存文件，生成一个全球唯一Guid字符串作为目录，
        /// ~/upload/temp/{UploadGuid}/目录下，以区别网站的并发情况
        /// </summary>
        [Description("上传Guid")]
        public string UploadGuid { get; set; }

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
        public virtual LocationAreaDto LocationArea { get; set; }
        #endregion

        #region 项目附件

        /// <summary>
        /// 项目附件
        /// </summary>
        [NotSet, Description("项目附件")]
        public virtual List<ProjectAttachmentDto> Attachments { get; set; } = new List<ProjectAttachmentDto>();

        #endregion

        #region 项目用户

        /// <summary>
        /// 项目用户/学员
        /// </summary>
        [NotSet, Description("项目用户")]
        public virtual List<ProjectUserDto> ProjectUsers { get; set; } = new List<ProjectUserDto>();

        #endregion

        #region 项目设备

        /// <summary>
        /// 项目设备
        /// </summary>
        public virtual List<DeviceDto> Devices { get; set; } = new List<DeviceDto>();

        #endregion

        #region 项目地块

        /// <summary>
        /// 项目地块
        /// </summary>
        public virtual List<SectionDto> Sections { get; set; } = new List<SectionDto>();

        #region 统计

        /// <summary>
        /// 房间合计
        /// </summary>
        [Description("房间合计")]
        public decimal? HouseTotal => Sections.Sum(m => m.HouseTotal ?? 0);

        /// <summary>
        /// 总工价
        /// </summary>
        [Description("总工价")]
        public decimal? CostTotal => Sections.Sum(m => m.CostTotal ?? 0);

        /// <summary>
        /// 总利润
        /// </summary>
        [Description("总利润")]
        public decimal? ProfitTotal => Sections.Sum(m => m.ProfitTotal ?? 0);

        /// <summary>
        /// 用工总计
        /// </summary>
        [Description("用工总计")]
        public decimal? WorkerTotal => Sections.Sum(m => m.WorkerTotal ?? 0);

        #endregion

        #endregion

        #region 组织

        /// <summary>
        /// 组织外键
        /// </summary>
        [Description("组织外键")]
        public Guid? OrganizationId { get; set; }

        ///// <summary>
        ///// 组织名称
        ///// </summary>
        //[Description("组织名称")]
        //public string OrganizationName => Organization?.Name;

        /// <summary>
        /// 组织
        /// </summary>
        [NotSet, Description("组织")]
        public virtual OrganizationDto Organization { get; set; }

        #endregion
    }
}