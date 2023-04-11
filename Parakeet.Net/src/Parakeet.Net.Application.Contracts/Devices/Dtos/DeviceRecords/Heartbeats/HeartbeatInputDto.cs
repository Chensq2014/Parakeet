using System;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 设备心跳接口调用
    /// </summary>
    public class HeartbeatInputDto 
    {
        [Required(ErrorMessage = "设备编码必填")]
        [MaxLength(CustomerConsts.MaxLength64, ErrorMessage = "最大长度为64")]
        public string SerialNo { get; set; }

        /// <summary>
        /// 记录采集时间
        /// </summary>
        public DateTime? RecordTime { get; set; }
    }
}
