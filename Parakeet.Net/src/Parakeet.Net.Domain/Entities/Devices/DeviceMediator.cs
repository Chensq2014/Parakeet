using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 设备转发
    /// </summary>
    [Description("设备转发")]
    [Table("Parakeet_DeviceMediators", Schema = "parakeet")]
    public class DeviceMediator : Entity<Guid>
    {
        public DeviceMediator()
        {
        }

        public DeviceMediator(Guid id) : base(id)
        {
        }

        #region 基础字段

        /// <summary>
        /// 是否转发
        /// </summary>
        [Description("是否转发")]
        public bool Forward { get; set; }

        /// <summary>
        /// 是否持久化
        /// </summary>
        [Description("是否持久化")]
        public bool Persist { get; set; }

        #endregion

        #region 设备

        [Description("设备Id")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public virtual Device Device { get; set; }

        #endregion

        #region 转发器

        /// <summary>
        /// 转发器Id
        /// </summary>
        [Description("转发器Id")]
        public Guid MediatorId { get; set; }

        /// <summary>
        /// 转发器
        /// </summary>
        [Description("转发器")]
        public virtual Mediator Mediator { get; set; }

        #endregion

    }
}
