using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Parakeet.Net.Web.Auth
{
    /// <summary>
    /// QQEmailHandler
    /// </summary>
    public class QQEmailHandler: AuthorizationHandler<QQEmailRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, QQEmailRequirement requirement)
        {
            if (context.User != null && context.User.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                var email = context.User.FindFirst(c => c.Type == ClaimTypes.Email).Value;
                if (email.EndsWith("@qq.com", StringComparison.OrdinalIgnoreCase))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    //context.Fail();//没成功就留给别人处理
                }
            }
            return Task.CompletedTask;
        }
    }
}
