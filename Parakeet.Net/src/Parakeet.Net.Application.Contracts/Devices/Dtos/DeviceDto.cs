using Parakeet.Net.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Parakeet.Net.Projects.Dtos;
using Parakeet.Net.Thresholds.Dtos;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceDto : DeviceListDto//FullAuditedEntityDto<Guid>
    {
        #region 位置信息

        /// <summary>
        ///     所在区域
        /// </summary>
        [Description("区域")]
        public virtual LocationAreaDto LocationArea { get; set; }


        #endregion

        #region 供应商

        /// <summary>
        /// 供应商
        /// </summary>
        public virtual SupplierDto Supplier { get; set; }


        #endregion

        #region 阈值

        /// <summary>
        /// 阈值
        /// </summary>
        public ThresholdDto Threshold { get; set; }

        #endregion

        #region 区域租户

        /// <summary>
        /// 租户
        /// </summary>
        public virtual AreaTenantDto AreaTenant { get; set; }

        #endregion

        #region 设备区域排序

        /// <summary>
        /// 设备序号一对一
        /// </summary>
        public virtual DeviceSequenceDto Sequence { get; set; }

        #endregion

        #region 设备所属项目

        /// <summary>
        /// 所属项目
        /// </summary>
        [Column("ProjectId"), Description("项目")]
        public virtual ProjectDto Project { get; set; }

        #endregion

        #region 设备信息扩展表

        /// <summary>
        /// 设备信息扩展表
        /// </summary>
        public virtual List<DeviceExtendDto> Extends { get; set; } = new List<DeviceExtendDto>();

        #endregion

        #region 设备转发关联
        /// <summary>
        /// 设备转发关联
        /// </summary>
        public virtual List<DeviceMediatorDto> Mediators { get; set; } = new List<DeviceMediatorDto>();

        #endregion

        #region 设备所有人员
        /// <summary>
        /// 设备所有人员
        /// </summary>
        public virtual List<DeviceWorkerDto> DeviceWorkers { get; set; } = new List<DeviceWorkerDto>();
        #endregion
    }
}
