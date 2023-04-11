using Parakeet.Net.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 设备基础字段列表对象
    /// </summary>
    public class DeviceListDto : BaseDto
    {
        #region 设备基础字段
        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64), Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 设备编码（设备真实设备序列号）
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength64), Description("设备编码")]
        public string SerialNo { get; set; }

        /// <summary>
        /// 转发编码（对外提供设备序列号）
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength64), Description("转发编码")]
        public string FakeNo { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [Required, Description("设备类型")]
        public DeviceType Type { get; set; }

        /// <summary>
        /// 设备密钥
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength255), Description("设备密钥")]
        public string Key { get; set; }

        /// <summary>
        /// 备案号
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64), Description("备案号")]
        public string RegisterNo { get; set; }
        /// <summary>
        /// 设备是否启用  默认禁用false
        /// </summary>
        [Description("是否启用")]
        public bool IsEnable { get; set; }

        #endregion

        #region 位置信息

        /// <summary>
        /// 父级区域（省）
        /// </summary>
        [MaxLength(6)]
        public string ParentArea { get; set; }

        /// <summary>
        /// 区域（区分成都，重庆等）
        /// </summary>
        [MaxLength(6)]
        public string Area { get; set; }

        /// <summary>
        /// 子区域（区/县）
        /// </summary>
        [MaxLength(6)]
        public string SubArea { get; set; }

        /// <summary>
        ///     区域Id
        /// </summary>
        [Description("区域Id")]
        public Guid? LocationAreaId { get; set; }

        #endregion

        #region 供应商

        /// <summary>
        /// 供应商Id
        /// </summary>
        public Guid SupplierId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }


        #endregion

        #region 阈值

        /// <summary>
        /// 阈值Id
        /// </summary>
        public Guid? ThresholdId { get; set; }

        #endregion
        
        #region 区域租户

        /// <summary>
        /// 所属租户区域Id
        /// </summary>
        public Guid? AreaTenantId { get; set; }

        #endregion

        #region 设备区域排序

        /// <summary>
        /// 记录设备排序的SequenceId
        /// </summary>
        public Guid? SequenceId { get; set; }

        #endregion

        #region 设备所属项目

        /// <summary>
        /// 项目Id
        /// </summary>
        [Description("项目Id")]
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [Description("项目名称")]
        public string ProjectName { get; set; }

        #endregion

    }
}
