using Parakeet.Net.CustomAttributes;
using System;
using System.ComponentModel;

namespace Parakeet.Net.Entities.Needs
{
    /// <summary>
    /// 需求附件表
    /// </summary>
    [Description("需求附件")]
    public class NeedAttachment : BaseAttachmentEntity
    {
        public NeedAttachment()
        {
        }

        public NeedAttachment(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 需求外键
        /// </summary>
        [Description("需求外键")]
        public Guid? NeedId { get; set; }

        /// <summary>
        /// 需求实体
        /// </summary>
        [NotSet,Description("需求")]
        public virtual Need Need { get; set; }
    }
}
