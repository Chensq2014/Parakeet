using System;
using Parakeet.Net.Dtos;
using Parakeet.Net.Enums;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 消息列表输入类
    /// </summary>
    public class NotifyInput : PagedInputDto
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        public Guid? ProjectId { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public NotifyType? NotifyType { get; set; }


    }
}
