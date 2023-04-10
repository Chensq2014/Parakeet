using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Parakeet.Net.Enums;
using Volo.Abp.Domain.Entities;

namespace Parakeet.Net.Entities.Devices
{
    /// <summary>
    /// 设备编号自增序列
    /// </summary>
    [Description("设备编号自增序列")]
    [Table("Parakeet_DeviceSequences", Schema = "parakeet")]
    public class DeviceSequence : Entity<Guid>
    {
        public DeviceSequence()
        {
        }

        public DeviceSequence(Guid id) : base(id)
        {
        }

        #region 基础字段

        /// <summary>
        /// 某具体区域
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength16)]
        [Description("某具体区域")]
        public string Area { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [Required]
        [Range(1000, 9999)]
        [Description("设备类型")]
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Description("序号")]
        public long Sequence { get; set; }

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
