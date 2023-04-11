using System;
using Parakeet.Net.Enums;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 提供给第三方接口调用考勤记录
    /// </summary>
    public class GateRecordInputDto : BaseDto<Guid>
    {
        [Required(ErrorMessage = "设备编码必填")]
        [MaxLength(CustomerConsts.MaxLength64, ErrorMessage = "最大长度为64")]
        public string SerialNo { get; set; }

        /// <summary>
        /// 人员唯一标识
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [MaxLength(CustomerConsts.MaxLength64, ErrorMessage = "最大长度为64")]
        public string PersonnelId { get; set; }

        /// <summary>
        /// 员工名字
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [MaxLength(CustomerConsts.MaxLength64, ErrorMessage = "最大长度为64")]
        public string PersonnelName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [MaxLength(CustomerConsts.MaxLength18, ErrorMessage = "最大长度为18")]
        public string IdCard { get; set; }

        /// <summary>
        /// 考勤照片
        /// </summary>
        [Required(ErrorMessage = "必填")]
        public string Photo { get; set; }

        /// <summary>
        /// 考勤照片Url
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength512, ErrorMessage = "最大长度为512")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 进出状态 【1-进】 【2-出】
        /// </summary>
        [Required(ErrorMessage = "必填")]
        public EntryState InOrOut { get; set; } = EntryState.进场;

        /// <summary>
        /// 用户工号（建委下发的）
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkerNo { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        [Required(ErrorMessage = "必填")]
        public DateTime? RecordTime { get; set; }
    }
}
