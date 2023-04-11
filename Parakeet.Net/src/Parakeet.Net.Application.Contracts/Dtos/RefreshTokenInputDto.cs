using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     提供给代理刷新Token接口类
    /// </summary>
    public class RefreshTokenInputDto
    {
        /// <summary>
        ///     登录后返回的RefreshToken
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }

        /// <summary>
        ///     授权模式 暂未使用   "refresh_token" //"authorization_code",//"client_credentials",//
        /// </summary>
        public string GrantType { get; set; }
    }
}