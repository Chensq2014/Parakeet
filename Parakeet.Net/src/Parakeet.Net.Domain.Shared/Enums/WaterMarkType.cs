using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 水印的类型
    /// </summary>
    [Description("水印的类型")]
    public enum WaterMarkType
    {
        /// <summary>
        /// 文字水印
        /// </summary>
        [Description("文字水印")]
        TextMark = 0,
        /// <summary>
        /// 图片水印
        /// </summary>
        [Description("图片水印")]
        ImageMark = 10
    }
}
