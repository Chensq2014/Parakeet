using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 宇泛删除人员
    /// </summary>
    public class PersonDeleteDto
    {
        /// <summary>
        /// 设备序列号 (真实序列号不能更改)
        /// </summary>
        [Required]
        public string SerialNo { get; set; }

        /// <summary>
        /// id(personnelId)字符串 逗号分开
        /// </summary>
        [Required]
        public string PersonId { get; set; }
    }
}
