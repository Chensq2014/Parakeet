using Parakeet.Net.Enums;
using Parakeet.Net.Helper;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 水印图传输对象
    /// </summary>
    public class WatermarkInputDto : ImageInputDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WatermarkInputDto()
        {
            //CustomConfigurationManager.Configuration["Watermark"] ?? "https://www.parakeet.vip"; //中文会乱码?
            WatermarkText = EnvironmentHelper.Watermark; 
        }

        /// <summary>
        /// 要加水印的原图 base64字符串及加水印后的图片 base64字符串
        /// </summary>
        public string Base64String { get; set; }

        /// <summary>
        /// 加水印后的图片 base64字符串
        /// </summary>
        public string WatermarkBase64String { get; set; }

        /// <summary>
        /// 水印文本
        /// </summary>
        public string WatermarkText { get; set; }

        /// <summary>
        /// 水印类型
        /// </summary>
        public WaterMarkType WaterMarkType { get; set; }

        /// <summary>
        /// 是否循环水印 默认为false
        /// </summary>
        public bool IsCircle { get; set; }
    }
}
