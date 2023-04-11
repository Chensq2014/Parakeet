using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 宇泛添加人员
    /// </summary>
    public class PersonAddFaceDto
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Required]
        public string SerialNo { get; set; }

        /// <summary>
        /// 非必填 只允许数字和英文字母 长度255、
        /// PersonnelID
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        /// 卡号 非必填
        /// </summary>
        public string IdCardNum { get; set; }

        /// <summary>
        /// base64头像
        /// </summary>
        public string Image { get; set; }
    }
}
