using System;
using Parakeet.Net.Helper;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Formats.Png;


namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// ImageSharp 水印验证码等图片输入基类
    /// </summary>
    public class ImageInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ImageInputDto()
        {
            FontPath = EnvironmentHelper.FontPath;//默认字体路径
            FontName = new FontCollection().Install(FontPath);
            EnbEncoder = new PngEncoder { CompressionLevel = 9, BitDepth = PngBitDepth.Bit8 };//, Quantizer = new WuQuantizer(200)
        }

        /// <summary>
        /// 水印文本默认字体
        /// </summary>
        public string FontPath { get; set; }
        /// <summary>
        /// 字体
        /// </summary>
        public FontFamily FontName { get; set; }

        /// <summary>
        /// 默认图片压缩编码png格式
        /// </summary>
        public PngEncoder EnbEncoder { get; set; }

    }
}
