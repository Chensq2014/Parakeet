using IdentityServer4.Models;
using System.Collections.Generic;

namespace Parakeet.Net.Web.Extentions
{
    /// <summary>
    /// 客户端模式
    /// </summary>
    public class ClientInitConfig
    {
        /// <summary>
        /// 定义ApiResource   
        /// 这里的资源（Resources）指的就是管理的API
        /// </summary>
        /// <returns>多个ApiResource</returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource("parakeet", "系统API")
            };
        }

        /// <summary>
        /// 定义验证条件的Client
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            var claims = new List<ClientClaim>
            {
                new ClientClaim(IdentityModel.JwtClaimTypes.Role, "Admin"),
                new ClientClaim(IdentityModel.JwtClaimTypes.NickName, "Chensq"),
                new ClientClaim("eMail", "290668617@qq.com")
            };
            return new List<Client>
            {
                new Client
                {
                    ClientId = "Parakeet_Server",//客户端惟一标识
                    ClientSecrets = new [] { new Secret("1q2w3e*".Sha256()) },//客户端密码，进行了加密
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    //授权方式，客户端认证，只要ClientId+ClientSecrets
                    AllowedScopes = new [] { "parakeet" },//允许访问的资源
                    Claims=claims
                }
            };
        }
    }
}
