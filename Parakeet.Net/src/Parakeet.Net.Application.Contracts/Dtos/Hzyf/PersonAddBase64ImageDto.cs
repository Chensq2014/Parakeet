using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 宇泛添加人员
    /// </summary>
    public class PersonAddBase64ImageDto
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Required]
        public string SerialNo { get; set; }

        /// <summary>
        /// 只允许数字和英文字母 长度255、
        /// PersonnelID
        /// </summary>
        [Required]
        public string PersonId { get; set; }

        /// <summary>
        /// 非必填  照片Id
        /// </summary>
        public string FaceId { get; set; }

        /// <summary>
        /// base64头像
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 是否严格模式 默认false
        /// </summary>
        public bool IsEasyWay { get; set; }
    }
}
