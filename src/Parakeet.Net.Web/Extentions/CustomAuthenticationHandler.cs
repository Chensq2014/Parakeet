using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Web.Extentions
{
    /// <summary>
    /// 自定义hander
    /// </summary>
    //[Dependency(ReplaceServices=true)]
    public class CustomAuthenticationHandler : IAuthenticationHandler
    {
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            throw new System.NotImplementedException();
        }
        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            // 这里是一个简化的示例，实际应用中你需要根据具体的认证机制来实现  
            // 检查请求中的认证信息（如Cookie、Token等）  
            // 验证这些信息是否有效，并构建一个AuthenticateResult来表示认证结果  

            // 假设我们在这里只是简单地模拟了一个成功的认证结果  
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity("ExampleScheme")), "ExampleScheme");
            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            throw new System.NotImplementedException();
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            throw new System.NotImplementedException();
        }
    }
}
