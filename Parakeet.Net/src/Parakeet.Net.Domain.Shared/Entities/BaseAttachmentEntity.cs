using System;
using System.ComponentModel;
using Parakeet.Net.CustomAttributes;
using Parakeet.Net.ValueObjects;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 附件基类
    /// </summary>
    [Description("附件基类")]
    public class BaseAttachmentEntity : BaseEntity
    {
        public BaseAttachmentEntity()
        {
        }

        public BaseAttachmentEntity(Guid id)
        {
            SetEntityPrimaryKey(id);
        }

        /// <summary>
        /// 附件基本信息
        /// </summary>
        [NotSet,Description("附件基本信息")]
        public virtual Attachment Attachment { get; set; }

        /// <summary>
        /// 文件顺序使用decimal类型排序更好
        /// </summary>
        [Description("文件顺序")]
        public decimal? Order { get; set; }
    }
}
