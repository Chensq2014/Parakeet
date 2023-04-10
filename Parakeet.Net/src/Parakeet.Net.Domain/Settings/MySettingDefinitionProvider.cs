using Parakeet.Net.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Parakeet.Net.Settings;

/// <summary>
/// 扩展配置中心
/// </summary>
public class MySettingDefinitionProvider : SettingDefinitionProvider
{
    /// <summary>
    ///  context读取节点 对应配置文件 Settings节点下
    /// </summary>
    /// <param name="context"></param>
    public override void Define(ISettingDefinitionContext context)
    {
        //context读取节点 对应配置文件 Settings节点下
        var smtpHost = context.GetOrNull("Abp.Mailing.Smtp.Host");
        if (smtpHost != null)
        {
            smtpHost.DefaultValue = "smtp.qq.com";
            smtpHost.DisplayName =
                new LocalizableString(
                    typeof(NetResource),
                    "Home"
                );
        }
    }
}
