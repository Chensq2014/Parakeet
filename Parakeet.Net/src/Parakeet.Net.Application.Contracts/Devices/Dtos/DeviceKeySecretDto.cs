using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 设备扩展
    /// </summary>
    public class DeviceKeySecretDto : BaseDto
    {
        #region 基础字段

        /// <summary>
        /// 区域
        /// </summary>
        [MaxLength(6), Description("区域")]
        public string Area { get; set; }

        /// <summary>
        /// 供应商授权id，全局唯一
        /// </summary>
        [Description("供应商授权id，全局唯一")]
        public string SupplierKeyId { get; set; }

        /// <summary>
        /// 供应商密钥值，用于请求签名，不在请求中传输，需要妥善保管
        /// </summary>
        [Description("供应商密钥值，用于请求签名，不在请求中传输，需要妥善保管")]
        public string SupplierKeySecret { get; set; }

        /// <summary>
        /// 项目授权id，全局唯一
        /// </summary>

        [Description("项目授权id，全局唯一")]
        public string ProjectKeyId { get; set; }

        /// <summary>
        /// 项目密钥的值，用于请求签名，不在请求中传输，需要妥善保管
        /// </summary>
        [Description("项目密钥的值，用于请求签名，不在请求中传输，需要妥善保管")]
        public string ProjectKeySecret { get; set; }

        #endregion

        #region 设备

        /// <summary>
        /// 设备Id
        /// </summary>
        [Required]
        [Description("设备Id")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public virtual DeviceDto Device { get; set; }

        #endregion
    }
}
