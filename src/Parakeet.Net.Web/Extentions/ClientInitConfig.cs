using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

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
            return new[]
            {
                new Client
                {
                    ClientId = "Parakeet_Server",//客户端惟一标识
                    ClientSecrets = new [] { new Secret("1q2w3e*".Sha256()) },//客户端密码，进行了加密
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    //授权方式，客户端认证，只要ClientId+ClientSecrets
                    AllowedScopes = new [] { "parakeet" },//允许访问的资源
                    Claims=new List<ClientClaim>{
                        new(IdentityModel.JwtClaimTypes.Role,"Admin"),
                        new (IdentityModel.JwtClaimTypes.NickName,"Chensq"),
                        new ("eMail","290668617@qq.com")
                    }
                }
            };
        }
    }
}
