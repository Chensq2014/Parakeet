using Parakeet.Net.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 设备
    /// </summary>
    [Table("Parakeet_Devices", Schema = "parakeet")]
    public class Device : BaseEntity
    {
        public Device()
        {
        }
        public Device(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

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
        /// 设备唯一密钥
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength255), Description("设备唯一密钥")]
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
        [MaxLength(CustomerConsts.MaxLength16), Description("父级区域(省)")]
        public string ParentArea { get; set; }

        /// <summary>
        /// 项目区域(市)
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength16), Description("项目区域(市)")]
        public string Area { get; set; }

        /// <summary>
        /// 子区域(区/县)
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength16), Description("子区域(区/县)")]
        public string SubArea { get; set; }

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

        #region 设备所属项目

        /// <summary>
        /// 项目Id
        /// </summary>
        [Description("项目Id")]
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        [Column("ProjectId"), Description("项目")]
        public virtual Project Project { get; set; }

        #endregion

        #region 供应商

        /// <summary>
        /// 供应商Id
        /// </summary>
        [Required]
        [Description("供应商Id")]
        public Guid SupplierId { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [Column("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        #endregion

        #region 阈值

        /// <summary>
        /// 阈值ThresholdId
        /// </summary>
        [Description("阈值Id")]
        public Guid? ThresholdId { get; set; }

        /// <summary>
        /// 阈值Threshold
        /// </summary>
        [Column("ThresholdId")]
        public virtual Threshold Threshold { get; set; }

        #endregion

        #region 区域租户

        /// <summary>
        /// 所属区域租户Id
        /// </summary>

        [Description("所属租户区域Id")]
        public Guid? AreaTenantId { get; set; }

        /// <summary>
        /// 所属区域租户
        /// </summary>
        [Column("AreaTenantId")]
        public virtual AreaTenant AreaTenant { get; set; }

        #endregion

        #region 设备区域排序

        /// <summary>
        /// 设备区域序号SequenceId
        /// </summary>
        [Description("设备区域序号Id")]
        public Guid? SequenceId { get; set; }

        /// <summary>
        /// 设备区域序号
        /// </summary>
        [Column("SequenceId")]
        [Description("设备区域序号")]
        public virtual DeviceSequence Sequence { get; set; }

        #endregion

        #region 设备信息扩展表

        /// <summary>
        /// 设备信息扩展表
        /// </summary>
        public virtual ICollection<DeviceExtend> Extends { get; set; } = new HashSet<DeviceExtend>();

        /// <summary>
        /// 添加设备扩展
        /// </summary>
        /// <param name="deviceExtendeds"></param>
        public virtual void AddDeviceExtendeds(IList<DeviceExtend> deviceExtendeds)
        {
            if (deviceExtendeds.Any())
            {
                foreach (var deviceExtended in deviceExtendeds)
                {
                    Extends.Add(deviceExtended);
                }
            }
        }

        /// <summary>
        /// 移除当前设备所有扩展信息
        /// </summary>
        public virtual void RemoveAllExtendeds()
        {
            if (Extends.Any(m => m.DeviceId == Id))
            {
                Extends.RemoveAll(m => m.DeviceId == Id);
            }
        }

        #endregion

        #region 设备区域转发密钥表  每个区域仅有一条记录

        /// <summary>
        /// 设备各区域转发密钥
        /// </summary>
        public virtual ICollection<DeviceKeySecret> KeySecrets { get; set; } = new HashSet<DeviceKeySecret>();

        /// <summary>
        /// 添加设备区域转发密钥
        /// </summary>
        /// <param name="keySecrets"></param>
        public virtual void AddDeviceKeySecrets(IList<DeviceKeySecret> keySecrets)
        {
            if (keySecrets.Any())
            {
                foreach (var secretKey in keySecrets)
                {
                    KeySecrets.Add(secretKey);
                }
            }
        }

        /// <summary>
        /// 移除当前设备所有区域转发密钥信息
        /// </summary>
        public virtual void RemoveAllKeySecrets()
        {
            if (KeySecrets.Any(m => m.DeviceId == Id))
            {
                KeySecrets.RemoveAll(m => m.DeviceId == Id);
            }
        }

        #endregion

        #region 设备转发关联

        /// <summary>
        /// 设备转发关联
        /// </summary>
        public virtual ICollection<DeviceMediator> Mediators { get; set; } = new HashSet<DeviceMediator>();

        /// <summary>
        /// 添加设备转发器
        /// </summary>
        /// <param name="deviceMediators"></param>
        public virtual void AddDeviceMediators(IList<DeviceMediator> deviceMediators)
        {
            if (deviceMediators.Any())
            {
                var existMediatorIds = Mediators.Select(m => m.MediatorId).ToList();
                var newMediators = deviceMediators.Where(m => !existMediatorIds.Contains(m.MediatorId)).ToList();
                foreach (var deviceMediator in newMediators)
                {
                    Mediators.Add(deviceMediator);
                }
            }
        }

        /// <summary>
        /// 移除当前设备所有转发
        /// </summary>
        public virtual void RemoveAllDeviceMediators()
        {
            if (Mediators.Any(m => m.DeviceId == Id))
            {
                Mediators.RemoveAll(m => m.DeviceId == Id);
            }
        }


        #endregion

        #region 设备所有人员

        /// <summary>
        /// 设备所有人员
        /// </summary>
        public virtual ICollection<DeviceWorker> DeviceWorkers { get; set; } = new HashSet<DeviceWorker>();

        #endregion
    }
}
