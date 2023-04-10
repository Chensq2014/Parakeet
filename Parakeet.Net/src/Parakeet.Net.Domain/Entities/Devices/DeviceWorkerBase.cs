using Parakeet.Net.Entities.LocationAreas;
using Parakeet.Net.Entities.Workers;
using Parakeet.Net.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Parakeet.Net.Entities.Devices
{
    /// <summary>
    ///  设备人员
    /// </summary>
    [Description("设备人员")]
    [Table("Parakeet_DeviceWorkers", Schema = "parakeet")]
    public class DeviceWorkerBase : Entity<Guid>
    {
        public DeviceWorkerBase()
        {
        }

        public DeviceWorkerBase(Guid id) : base(id)
        {
        }

        #region 基础字段

        /// <summary>
        /// Ic卡
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength32)]
        [Description("Ic卡")]
        public string IcCard { get; set; }

        /// <summary>
        /// 人员ID(下发到设备) 默认为worker的personnelId
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength36)]
        [Description("人员ID(下发到设备) 默认为worker的personnelId")]
        public string PersonId { get; set; }

        /// <summary>
        /// 用户工号（第三方平台）
        /// </summary>
        [Description("用户工号")]
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkerNo { get; set; }

        /// <summary>
        /// 人员类型
        /// 持久层统一存储格式
        /// 【1-劳务人员】【2-管理人员】//【3-采集】
        /// </summary>
        [Description("人员类型 【1-劳务人员】【2-管理人员】")]
        public StaffType PersonnelType { get; set; } = StaffType.劳务人员;

        /// <summary>
        /// 注册类型
        /// </summary>
        [Description("注册类型")]
        public RegType RegisterType { get; set; } = RegType.人脸采集;

        /// <summary>
        /// 是否第三方平台注册
        /// </summary>
        [Description("是否第三方平台注册")]
        public bool IsOuterRegistered { get; set; }

        /// <summary>
        /// 是否发送给设备(false/true true时可认位人员已经下发到设备成功(严格意义上查下发人员的feedback返回正常时才可以赋为true))
        /// </summary>
        [Description("是否发送给设备(false/true true时可认位人员已经下发到设备成功(严格意义上查下发人员的feedback返回正常时才可以赋为true))")]
        public bool IsSendToDevice { get; set; }
        #endregion

        #region 区域租户

        /// <summary>
        /// 设备区域租户
        /// </summary>
        [Description("设备区域租户")]
        public Guid AreaTenantId { get; set; }

        /// <summary>
        /// 设备区域租户
        /// </summary>
        public virtual AreaTenant AreaTenant { get; set; }

        #endregion

        #region 劳务人员
        /// <summary>
        /// 工人Id
        /// </summary>
        [Description("工人Id")]
        public Guid WorkerId { get; set; }

        /// <summary>
        /// 人员实体
        /// </summary>
        public virtual Worker Worker { get; set; }

        #endregion

        #region 设备
        /// <summary>
        /// 设备Id
        /// </summary>
        [Description("设备Id")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public virtual Device Device { get; set; }

        #endregion

        #region 位置信息

        /// <summary>
        /// 区域ID
        /// </summary>
        [Description("区域LocationAreaId")]
        public Guid? LocationAreaId { get; set; }

        /// <summary>
        /// 所在区域
        /// </summary>
        public virtual LocationArea LocationArea { get; set; }

        #endregion

    }
}