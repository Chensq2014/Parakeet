using Volo.Abp.Settings;

namespace Parakeet.Net.Settings;

public class NetSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(NetSettings.MySetting1));
    }
}
