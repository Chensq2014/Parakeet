using System.ComponentModel.DataAnnotations;
using Parakeet.Net.Enums;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 设备反馈记录
    /// </summary>
    public class FeedbackDto : DeviceRecordDto
    {
        /// <summary>
        /// 反馈类型:
        /// 0-人员删除成功，
        /// 1-人员删除失败，
        /// 2-人员下发成功，
        /// 3-人员下发失败
        /// </summary>
        public FeedbackType? Type { get; set; }

        /// <summary>
        /// 反馈消息内容
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Message { get; set; }

        /// <summary>
        /// 消息序号
        /// </summary>
        public long? SequenceNo { get; set; }

        /// <summary>
        /// 人员Id
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength36)]
        public string PersonnelId { get; set; }

        /// <summary>
        /// 处理类型
        /// Register = 1,
        /// Delete = 2,
        /// Update = 3
        /// </summary>
        public FeedbackHandlerType? HandlerType { get; set; }

        /// <summary>
        /// 响应码
        /// </summary>
        public int? Code { get; set; }
    }
}
