using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Parakeet.Net.ROServer.Handlers
{
    /// <summary>
    /// 授权 Policy->GrpcRequirement-->GrpcRequireHandler: AuthorizationHandler<GrpcRequirement>
    /// Handler必须继承自AuthorizationHandler<IAuthorizationRequirement>,框架设计
    /// </summary>
    public class GrpcRequireHandler : AuthorizationHandler<GrpcRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GrpcRequirement requirement)
        {
            if (context.User != null && context.User.HasClaim(c => c.Type == "appId"))
            {
                var appId = context.User.Claims.FirstOrDefault(m => m.Type == "appId")?.Value;
                if (string.IsNullOrWhiteSpace(appId))
                {
                    Log.Logger.Information($"appId为空,请检查请求header!");
                    context.Fail();
                }
                else
                {
                    context.Succeed(requirement);
                }
            }
            else
            {
                Log.Logger.Error($"未授权,appId exist?={context.User?.HasClaim(c => c.Type == "appId")}");
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}