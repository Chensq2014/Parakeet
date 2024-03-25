using Common.Helpers;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.ConsoleApp;

public class ConsoleDemoService : ITransientDependency
{
    private readonly IConfiguration _configuration;
    public ConsoleDemoService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task RunAsync()
    {
        #region ConnectionStrings

        var node = "ConnectionStrings";
        Console.WriteLine($"\"{node}\":{{");
        var dbKey = "MutiTenant";
        var key = $"{node}:{dbKey}";
        var conn = _configuration[key];
        var encryptStr = EncodingEncryptHelper.Encrypt(conn);
        //var dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
        //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
        //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
        Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

        dbKey = "MySql";
        key = $"{node}:{dbKey}";
        conn = _configuration[key];
        encryptStr = EncodingEncryptHelper.Encrypt(conn);
        //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
        //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
        //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
        Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");
        
        dbKey = "PgSql";
        key = $"{node}:{dbKey}";
        conn = _configuration[key];
        encryptStr = EncodingEncryptHelper.Encrypt(conn);
        //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
        //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
        //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
        Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

        dbKey = "SqlServer";
        key = $"{node}:{dbKey}";
        conn = _configuration[key];
        encryptStr = EncodingEncryptHelper.Encrypt(conn);
        //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
        //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
        //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
        Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

        dbKey = "Write";
        key = $"{node}:{dbKey}";
        conn = _configuration[key];
        encryptStr = EncodingEncryptHelper.Encrypt(conn);
        //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
        //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
        //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
        Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

        dbKey = "Read";
        key = $"{node}:{dbKey}";
        conn = _configuration[key];
        encryptStr = EncodingEncryptHelper.Encrypt(conn);
        //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
        //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
        //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
        Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\"");
        Console.WriteLine($"}}");


        #endregion

        #region Redis

        node = "Redis";
        Console.WriteLine($"\"{node}\":{{");

        dbKey = "Configuration";
        key = $"{node}:{dbKey}";
        conn = _configuration[key];
        encryptStr = EncodingEncryptHelper.Encrypt(conn);
        //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
        //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
        //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
        Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

        dbKey = "CsRedisConfiguration";
        key = $"{node}:{dbKey}";
        conn = _configuration[key];
        encryptStr = EncodingEncryptHelper.Encrypt(conn);
        //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
        //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
        //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
        Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");


        Console.WriteLine($"    \"InstanceName\": \"parakeet\",");
        Console.WriteLine($"    \"ConnectionStrings\": \"localhost\",");
        Console.WriteLine($"    \"DatabaseId\": 1");
        Console.WriteLine($"}}");

        #endregion

        #region AuthServer

        node = "AuthServer";
        Console.WriteLine($"\"{node}\":{{");
        Console.WriteLine($"    \"Authority\": \"https://localhost:44331\",");
        Console.WriteLine($"    \"RequireHttpsMetadata\": true,");
        Console.WriteLine($"    \"ClientId\": \"Net_Web\",");
        
        dbKey = "ClientSecret";
        key = $"{node}:{dbKey}";
        conn = _configuration[key];
        encryptStr = EncodingEncryptHelper.Encrypt(conn);
        //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
        //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
        //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
        Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

        Console.WriteLine($"    \"IsContainerized\": false");
        Console.WriteLine($"}}");

        #endregion
        #region Settings

        node = "Settings";
        Console.WriteLine($"\"{node}\":{{");

        Console.WriteLine($"      \"DefaultFromAddress\": \"chensq0523@foxmail.com\", //\"chenshuangquan@xywgzs1.onexmail.com\",");
        Console.WriteLine($"      \"DefaultFromDisplayName\": \"Chensq\",");
        Console.WriteLine($"      \"Smtp.Host\": \"smtp.qq.com\", //\"smtp.exmail.qq.com\", //");
        Console.WriteLine($"      \"Smtp.Port\": \"587\", //\"465\", //");
        Console.WriteLine($"      \"Smtp.UserName\": \"chensq0523@foxmail.com\",");

        dbKey = "Smtp.Password";
        key = $"{node}:{dbKey}";
        conn = _configuration[key];
        encryptStr = EncodingEncryptHelper.Encrypt(conn);
        //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
        //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
        //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
        Console.WriteLine($"      \"{dbKey}\": \"{encryptStr}\",");

        Console.WriteLine($"      \"Smtp.EnableSsl\": \"false\", //\"true\",//587端口在emailsender配置中不允许ssl加密  465才可以");
        Console.WriteLine($"      \"Smtp.UseDefaultCredentials\": \"fasle\",");
        Console.WriteLine($"      \"Abp.Mailing.DefaultFromAddress\": \"chensq0523@foxmail.com\",");
        Console.WriteLine($"      \"Abp.Mailing.DefaultFromDisplayName\": \"Chensq\",");
        Console.WriteLine($"      \"Abp.Mailing.Smtp.Host\": \"smtp.qq.com\", //\"smtp.exmail.qq.com\", //");
        Console.WriteLine($"      \"Abp.Mailing.Smtp.Port\": \"587\", //\"465\", //");
        Console.WriteLine($"      \"Abp.Mailing.Smtp.UserName\": \"chensq0523@foxmail.com\", //\"chenshuangquan@xywgzs1.onexmail.com\",");
        Console.WriteLine($"      \"Abp.Mailing.{dbKey}\": \"{encryptStr}\",");
        Console.WriteLine($"      \"Abp.Mailing.Smtp.EnableSsl\": \"false\", //\"true\",//587端口在emailsender配置中不允许ssl加密  465才可以");
        Console.WriteLine($"      \"Abp.Mailing.Smtp.UseDefaultCredentials\": \"false\"");
        
        Console.WriteLine($"}}");

        #endregion


    }
}
