using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Parakeet.Net.Enums;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 设备反馈消息
    /// </summary>
    [Description("设备反馈消息")]
    public class FeedbackBase : DeviceRecordBase
    {
        public FeedbackBase()
        {
        }

        public FeedbackBase(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 反馈类型:
        /// 0-人员删除成功，
        /// 1-人员删除失败，
        /// 2-人员下发成功，
        /// 3-人员下发失败
        /// </summary>
        [Description("反馈类型 0-人员删除成功 1-人员删除失败 2-人员下发成功 3-人员下发失败")]
        public FeedbackType? Type { get; set; }

        /// <summary>
        /// 反馈消息内容
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        [Description("反馈消息内容")]
        public string Message { get; set; }

        /// <summary>
        /// 消息序号
        /// </summary>
        [Description("消息序号")]
        public long? SequenceNo { get; set; }

        /// <summary>
        /// 人员Id
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength36)]
        [Description("人员Id")]
        public string PersonnelId { get; set; }

        /// <summary>
        /// 处理类型 Register = 1, Delete = 2, Update = 3
        /// </summary>
        [Description("处理类型 Register = 1, Delete = 2, Update = 3")]
        public FeedbackHandlerType? HandlerType { get; set; }

        /// <summary>
        /// 响应码
        /// </summary>
        [Description("响应码")]
        public int? Code { get; set; }
    }
}
