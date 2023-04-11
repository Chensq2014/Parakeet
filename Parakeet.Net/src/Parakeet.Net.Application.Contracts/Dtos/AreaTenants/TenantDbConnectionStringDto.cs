using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 租户连接字符串信息
    /// </summary>
    [Description("租户连接字符串信息")]
    public class TenantDbConnectionStringDto : EntityDto<Guid>
    {
        #region 基础字段

        /// <summary>
        /// 字符串连接名称
        /// </summary>
        [Description("字符串连接名称")]
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Name { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        [Description("连接字符串")]
        [MaxLength(CustomerConsts.MaxLength2048)]
        public string Value { get; set; }

        /// <summary>
        /// 是否是主库
        /// </summary>
        [Description("是否是主库")]
        public bool IsMaster { get; set; }

        /// <summary>
        /// 按时间节点(年/月) 分库
        /// </summary>
        [Description("数据库时间年")]
        public DateTime? Date { get; set; }

        #endregion

        #region 区域租户

        /// <summary>
        /// 区域租户
        /// </summary>
        [Description("区域租户AreaTenantId")]
        public Guid AreaTenantId { get; set; }

        /// <summary>
        /// 区域租户
        /// </summary>
        public string AreaTenantName { get; set; }

        ///// <summary>
        ///// 区域租户  暂不需要映射
        ///// </summary>
        //public virtual AreaTenantDto AreaTenant { get; set; }
        #endregion
    }
}
