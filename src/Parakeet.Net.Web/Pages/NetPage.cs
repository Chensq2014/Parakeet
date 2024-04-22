using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Parakeet.Net.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Parakeet.Net.Web.Pages
{
    /* Inherit your UI Pages from this class. To do that, add this line to your Pages (.cshtml files under the Page folder):
     * @inherits Parakeet.NetCore.Web.Pages.NetCorePage
     */
    public abstract class NetPage : AbpPage
    {
        [RazorInject]
        public IHtmlLocalizer<NetResource> L { get; set; }
    }
}
