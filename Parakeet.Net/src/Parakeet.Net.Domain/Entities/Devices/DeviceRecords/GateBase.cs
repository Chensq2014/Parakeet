using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Parakeet.Net.Enums;

namespace Parakeet.Net.Entities.Devices.DeviceRecords
{
    /// <summary>
    /// 考勤
    /// </summary>
    [Description("考勤")]
    public class GateBase : DeviceRecordBase
    {
        public GateBase()
        {
        }

        public GateBase(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 人员唯一标识
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Description("人员唯一标识")]
        public string PersonnelId { get; set; }

        /// <summary>
        /// 员工名字
        /// </summary>
        [MaxLength(50)]
        [Description("员工名字")]
        public string PersonnelName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [MaxLength(18)]
        [Description("身份证号")]
        public string IdCard { get; set; }

        /// <summary>
        /// 进出状态 【1-进】 【2-出】 【3-采集】
        /// </summary>
        [Description("进出状态 【1-进】 【2-出】 【3-采集】")]
        public EntryState InOrOut { get; set; } = EntryState.进场;

        /// <summary>
        /// 考勤照片
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength8192)]
        [Description("考勤照片")]
        public string Photo { get; set; }

        /// <summary>
        /// 考勤照片Url
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength512)]
        [Description("考勤照片Url")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 用户工号（建委下发的）
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        [Description("用户工号")]
        public string WorkerNo { get; set; }

    }
}
