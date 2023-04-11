using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 通过手机号获取企业微信用户id输入类
    /// </summary>
    public class GetWeixinUserIdByPhoneInputDto
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        public string Mobile { get; set; }
    }
}
