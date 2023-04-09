using Parakeet.Net.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Parakeet.Net.Web.Pages;

public abstract class NetPageModel : AbpPageModel
{
    protected NetPageModel()
    {
        LocalizationResourceType = typeof(NetResource);
    }
}
