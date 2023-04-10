using Volo.Abp.Settings;

namespace Parakeet.Net.Settings;

public class NetSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(NetSettings.MySetting1));
        //context读取节点 对应配置文件 Settings节点下
    }
}
