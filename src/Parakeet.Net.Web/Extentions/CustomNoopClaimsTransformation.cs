using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Web.Extentions
{
    /// <summary>
    /// 需要注册到AddAuthentication之后
    /// </summary>
    //[Dependency(ReplaceServices = true)]
    public class CustomNoopClaimsTransformation : IClaimsTransformation
    {
        async Task<ClaimsPrincipal> IClaimsTransformation.TransformAsync(ClaimsPrincipal principal)
        {
            //这里是对principal做修改 或者额外扩展的
            if (principal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Country))?.Value.Equals("Chinese") ?? false)
            {
                principal.Claims.Append(new Claim("language", "cn"));
            }
            return await Task.FromResult(principal);
        }
    }
}
