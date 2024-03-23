namespace ARchGL.IOT.RoClient.Models
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
    }
}