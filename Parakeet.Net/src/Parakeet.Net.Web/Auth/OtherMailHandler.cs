using Microsoft.AspNetCore.Authorization;
using Parakeet.Net.CustomAttributes;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Parakeet.Net.Web.Auth
{
    /// <summary>
    /// 验证是否有其它邮箱
    /// </summary>
    public class OtherMailHandler : AuthorizationHandler<DoubleEmailRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DoubleEmailRequirement requirement)
        {
            if (context.User != null && context.User.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                var email = context.User.FindFirst(c => c.Type == ClaimTypes.Email).Value;
                if (Regexes.IsEmail(email))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    //context.Fail();//不设置失败
                }
            }
            return Task.CompletedTask;
        }
    }
}
