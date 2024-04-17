namespace Parakeet.Net.ROClient.Models
{
    public class FaceModel
    {
        /// <summary>
        /// 图片唯一标识ID[必填]
        /// </summary>
        /// <value></value>
        public string FaceId { get; set; }

        /// <summary>
        /// Base64图片[必填]
        /// </summary>
        /// <value></value>
        public string Image { get; set; }

        /// <summary>
        /// 图片Url地址(建议填些)
        /// </summary>
        public string Url { get; set; }
    }
}