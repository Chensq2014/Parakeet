using Parakeet.Net.Dtos;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 区域租户
    /// </summary>
    [Description("区域租户")]
    public class AreaTenantListDto : BaseDto
    {
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
        public string LocationAreaName { get; set; }

        #endregion

    }
}
