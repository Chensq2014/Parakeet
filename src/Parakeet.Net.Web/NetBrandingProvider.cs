using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Web;

[Dependency(ReplaceServices = true)]
public class NetBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "小鹦鹉工作室"; //商标名称// CustomerConsts.AppName;
    public override string LogoUrl => $@"~\images\logo-parakeet17.png";//?
}
