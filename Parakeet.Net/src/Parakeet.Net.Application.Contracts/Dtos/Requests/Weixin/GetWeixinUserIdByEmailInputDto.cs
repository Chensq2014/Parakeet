using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos.Requests
{
    /// <summary>
    /// 通过邮箱获取企业微信用户id输入类
    /// </summary>
    public class GetWeixinUserIdByEmailInputDto
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// 邮箱类型 邮箱类型：1-企业邮箱（默认）；2-个人邮箱
        /// </summary>
        public int EmailType { get; set; } = 1;
    }
}
