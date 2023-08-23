using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 根据用户票据获取用户敏感信息输入类
    /// </summary>
    public class GetWeixinUserSpecialInfoInputDto
    {
        /// <summary>
        /// 用户票据
        /// </summary>
        [Required]
        public string UserTicket { get; set; }
    }
}
