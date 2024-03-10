using Volo.Abp.Emailing;
using Volo.Abp.Settings;

namespace Parakeet.Net.Settings;

/// <summary>
/// 邮件设置(ABP会自动发现并注册设置的定义)
/// 对EmailSettingNames中定义的参数项进行设置
/// </summary>
public class EmailSettingProvider : SettingDefinitionProvider
{
    /// <summary>
    /// ontext读取节点 对应配置文件 Settings节点下
    /// </summary>
    /// <param name="context"></param>
    public override void Define(ISettingDefinitionContext context)
    {
        #region 通过配置文件的方式赋默认值   

        // 配置文件IConfiguration 从容器中获取出来更合理
        //var section = CustomConfigurationManager.Configuration.GetSection("App").GetSection("Email");
        //context.Add(new SettingDefinition("Smtp.Host", section?.GetSection("SmtpHost")?.Value??"127.0.0.1"),
        //    new SettingDefinition("Smtp.Port", section?.GetSection("Port")?.Value),
        //    new SettingDefinition("Smtp.UserName",section?.GetSection("UserName")?.Value),
        //    new SettingDefinition("Smtp.Password",section?.GetSection("License")?.Value),
        //    new SettingDefinition("Smtp.EnableSsl", "true") 
        #endregion

        #region 添加Settings节点下自定义设置

        context.Add(new SettingDefinition("Smtp.Host"),
            new SettingDefinition("Smtp.Port"),
            new SettingDefinition("Smtp.UserName"),
            new SettingDefinition("Smtp.Password"),
            new SettingDefinition("Smtp.EnableSsl")
        );
        #endregion

        #region EmailSettingNames 配置文件Settings节点中已正确配置,这里不设置默认值

        context.Add(
            new SettingDefinition(EmailSettingNames.Smtp.Host, "smtp.qq.com"),
            new SettingDefinition(EmailSettingNames.Smtp.Port, "547"),
            new SettingDefinition(EmailSettingNames.Smtp.UserName, "chensq0523@foxmail.com"),
            new SettingDefinition(EmailSettingNames.Smtp.Password, "cqcxtfqmunlrddbc"),//,isEncrypted: false
            new SettingDefinition(EmailSettingNames.Smtp.Domain),
            new SettingDefinition(EmailSettingNames.Smtp.EnableSsl, "true"),
            new SettingDefinition(EmailSettingNames.Smtp.UseDefaultCredentials, "false"),
            new SettingDefinition(EmailSettingNames.DefaultFromAddress, "chensq0523@foxmail.com"),
            new SettingDefinition(EmailSettingNames.DefaultFromDisplayName, "Parakeet")
        );

        // 对应配置文件 Settings节点下
        //"Settings": { //配置 EmailSettingProvider:这些key属性的默认值
        //    "DefaultFromAddress": "chensq0523@foxmail.com", //"chenshuangquan@xywgzs1.onexmail.com",
        //    "DefaultFromDisplayName": "Chensq",
        //    "Smtp.Host": "smtp.qq.com", //"smtp.exmail.qq.com", //
        //    "Smtp.Port": "587", //"465", //
        //    "Smtp.UserName": "chensq0523@foxmail.com",
        //    "Smtp.Password": "",
        //    "Smtp.EnableSsl": "false", //"true",//587端口在emailsender配置中不允许ssl加密  465才可以
        //    "Smtp.UseDefaultCredentials": "fasle",
        //    "Abp.Mailing.DefaultFromAddress": "chensq0523@foxmail.com",
        //    "Abp.Mailing.DefaultFromDisplayName": "Chensq",
        //    "Abp.Mailing.Smtp.Host": "smtp.qq.com", //"smtp.exmail.qq.com", //
        //    "Abp.Mailing.Smtp.Port": "587", //"465", //
        //    "Abp.Mailing.Smtp.UserName": "chensq0523@foxmail.com", //"chenshuangquan@xywgzs1.onexmail.com",
        //    "Abp.Mailing.Smtp.Password": "",
        //    "Abp.Mailing.Smtp.EnableSsl": "false", //"true",//587端口在emailsender配置中不允许ssl加密  465才可以
        //    "Abp.Mailing.Smtp.UseDefaultCredentials": "false"
        //},

        #endregion

    }
}
