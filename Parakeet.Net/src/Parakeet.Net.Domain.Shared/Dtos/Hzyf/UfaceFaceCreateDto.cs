namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 下发人员照片对象
    /// </summary>
    public class UfaceFaceCreateDto
    {
        /// <summary>
        /// interfaceName
        /// </summary>
        public string InterfaceName { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 人员Id
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        /// 人脸Id
        /// </summary>
        public string FaceId { get; set; }

        /// <summary>
        /// 人员照片
        /// </summary>
        public string ImgBase64 { get; set; }

        /// <summary>
        /// IsEasyWay
        /// </summary>
        public string IsEasyWay { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }
    }
}