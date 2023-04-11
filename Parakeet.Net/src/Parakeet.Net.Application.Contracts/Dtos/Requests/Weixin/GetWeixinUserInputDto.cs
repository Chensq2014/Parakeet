using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos.Requests
{
    /// <summary>
    /// 获取企业某用户输入类
    /// </summary>
    public class GetWeixinUserInputDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Required]
        public string UserId { get; set; }
    }
}
