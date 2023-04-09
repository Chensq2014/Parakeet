using Parakeet.Net.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Parakeet.Net.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class NetController : AbpControllerBase
{
    protected NetController()
    {
        LocalizationResource = typeof(NetResource);
    }
}
