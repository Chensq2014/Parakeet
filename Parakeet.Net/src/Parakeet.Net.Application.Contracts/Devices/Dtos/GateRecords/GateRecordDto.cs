using Parakeet.Net.Enums;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 考勤记录
    /// </summary>
    public class GateRecordDto : DeviceRecordDto
    {
        /// <summary>
        /// 人员唯一标识
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string PersonnelId { get; set; }

        /// <summary>
        /// 员工名字
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string PersonnelName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength18)]
        public string IdCard { get; set; }

        /// <summary>
        /// 考勤照片
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// 考勤照片Url
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength512)]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 进出状态 【1-进】 【2-出】 【3-采集】
        /// </summary>
        public EntryState InOrOut { get; set; } = EntryState.进场;

        /// <summary>
        /// 用户工号（建委下发的）
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkerNo { get; set; }
    }
}
