using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 图片类型
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// 默认类型
        /// </summary>
        [Description("默认类型")]
        Default = 0,
        /// <summary>
        /// 原图
        /// </summary>
        [Description("Base64原图")]
        Original = 10,
        /// <summary>
        /// 水印图
        /// </summary>
        [Description("水印图")]
        WateRmark = 20
    }
}
