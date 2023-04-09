using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Web;

[Dependency(ReplaceServices = true)]
public class NetBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Net";
}
