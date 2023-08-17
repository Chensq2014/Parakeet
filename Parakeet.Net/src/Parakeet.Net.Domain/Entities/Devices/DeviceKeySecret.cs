using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 各区域设备转发密钥
    /// </summary>
    [Description("各区域设备转发密钥")]
    [Table("Parakeet_DeviceKeySecrets", Schema = "parakeet")]
    public class DeviceKeySecret : Entity<Guid>
    {
        public DeviceKeySecret()
        {
        }

        public DeviceKeySecret(Guid id) : base(id)
        {
        }

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
        [ForeignKey("DeviceId")]
        public virtual Device Device { get; set; }

        #endregion
    }
}
