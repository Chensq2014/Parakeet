using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 宇泛添加人员
    /// </summary>
    public class PersonAddDto
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [Required]
        public string SerialNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 身份证编号
        /// </summary>
        [Required]
        public string IDNumber { get; set; }

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
        /// 刷脸权限 默认打开为2 非必填
        /// </summary>
        public string FacePermission { get; set; } = "2";

        /// <summary>
        /// 刷卡权限 默认打开为2 非必填
        /// </summary>
        public string IdCardPermission { get; set; } = "2";

        /// <summary>
        /// 人卡权限 默认打开为2 非必填
        /// </summary>
        public string FaceAddCardPermission { get; set; } = "2";

        /// <summary>
        /// 人脸对比权限 默认打开为2 非必填
        /// </summary>
        public string IDPermission { get; set; } = "2";
    }
}
