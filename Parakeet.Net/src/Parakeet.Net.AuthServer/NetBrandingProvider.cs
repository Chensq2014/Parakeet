using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net;

[Dependency(ReplaceServices = true)]
public class NetBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Net";
}
