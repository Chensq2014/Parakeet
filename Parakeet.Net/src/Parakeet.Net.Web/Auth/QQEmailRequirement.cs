using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Parakeet.Net.Web.Auth
{
    public class QQEmailRequirement: NameAuthorizationRequirement
    {
        public QQEmailRequirement(string requiredName) : base(requiredName)
        {
        }
    }
}
