using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 区域租户
    /// </summary>
    [Description("区域租户")]
    [Table("Parakeet_AreaTenants", Schema = "parakeet")]
    public class AreaTenant : BaseEntity
    {
        public AreaTenant() { }
        public AreaTenant(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

        #region 基础字段

        /// <summary>
        /// 区域租户名称，全局唯一，如Chongqing
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength32)]
        [Description("区域租户名称，全局唯一")]
        public string Name { get; set; }

        /// <summary>
        /// 区域租户显示名称,如重庆
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength32)]
        [Description("区域租户显示名称")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength2048)]
        [Description("备注")]
        public string Remark { get; set; }

        #endregion

        #region 位置信息

        /// <summary>
        /// 区域租户码，一般为省级编码，如500000
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength16)]
        [Description("区域租户码")]
        public string AreaCode { get; set; }

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

        #region  租户连接字符串信息

        /// <summary>
        /// 租户连接字符串信息
        /// </summary>
        public virtual ICollection<TenantDbConnectionString> TenantDbConnectionStrings { get; set; } = new HashSet<TenantDbConnectionString>();

        #endregion

        #region  设备

        /// <summary>
        /// 设备
        /// </summary>
        public virtual ICollection<Device> Devices { get; set; } = new HashSet<Device>();

        #endregion

        #region 设备人员

        /// <summary>
        /// 设备人员
        /// </summary>
        public virtual ICollection<DeviceWorker> DeviceWorkers { get; set; } = new HashSet<DeviceWorker>();

        #endregion
    }
}
