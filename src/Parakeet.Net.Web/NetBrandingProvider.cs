using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Common;

namespace Parakeet.Net.Web;

[Dependency(ReplaceServices = true)]
public class NetBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => CommonConsts.AppDisplayName;
    //public override string LogoUrl => $@"\images\logo-parakeet17.png";//?
}
