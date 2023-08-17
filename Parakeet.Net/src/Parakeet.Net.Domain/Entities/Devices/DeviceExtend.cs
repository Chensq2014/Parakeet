using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 设备扩展
    /// </summary>
    [Description("设备扩展")]
    [Table("Parakeet_DeviceExtends", Schema = "parakeet")]
    public class DeviceExtend : Entity<Guid>
    {
        public DeviceExtend()
        {
        }

        public DeviceExtend(Guid id) : base(id)
        {
        }

        #region 基础字段

        /// <summary>
        /// 自定义键不重复
        /// </summary>
        [Required, MaxLength(CustomerConsts.MaxLength64)]
        [Description("自定义键不重复")]
        public string Key { get; set; }

        /// <summary>
        /// 自定义值
        /// </summary>
        [Required, MaxLength(CustomerConsts.MaxLength2048)]
        [Description("自定义值")]
        public string Value { get; set; }
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
        public virtual Device Device { get; set; }

        #endregion
    }
}
