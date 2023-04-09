using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Extensions;
using Parakeet.Net.ValueObjects;
using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 基础附件类
    /// </summary>
    [Description("基础附件类")]
    public abstract class BaseAttachmentDto : BaseDto
    {
        /// <summary>
        /// 附件基本信息
        /// </summary>
        [Description("附件基本信息")]
        public virtual Attachment Attachment { get; set; }

        /// <summary>
        /// 文件顺序
        /// </summary>
        [Description("文件顺序")]
        public decimal? Order { get; set; }

        /// <summary>
        /// 图片扩展名规则
        /// </summary>
        [Description("图片扩展名规则"), NotSet]
        private string _extensions = ".jpg.jpeg.png.gif.bmp.tiff";

        /// <summary>
        /// 是否图片文件
        /// </summary>
        [Description("是否图片"), NotSet]
        public bool IsImage => Attachment.Extention.HasValue() && _extensions.Contains(Attachment.Extention);
    }
}
