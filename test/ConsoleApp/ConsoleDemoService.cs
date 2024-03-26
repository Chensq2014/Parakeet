using Common.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace Parakeet.Net.ConsoleApp;

public class ConsoleDemoService : ITransientDependency
{
    private readonly IConfiguration _configuration;
    private AbpSequentialGuidGeneratorOptions _abpSequentialGuidGeneratorOptions;
    private readonly IGuidGenerator _guidGenerator;

    public ConsoleDemoService(IConfiguration configuration, IOptions<AbpSequentialGuidGeneratorOptions> abpSequentialGuidGeneratorOptions, IGuidGenerator guidGenerator)
    {
        _configuration = configuration;
        _guidGenerator = guidGenerator;
        _abpSequentialGuidGeneratorOptions = abpSequentialGuidGeneratorOptions.Value;
    }

    public async Task RunAsync()
    {
        #region 配置文件配置加密测试

        {

            #region ConnectionStrings

            var node = "ConnectionStrings";
            Console.WriteLine($"\"{node}\":{{");
            var dbKey = "MultiTenant";
            var key = $"{node}:{dbKey}";
            var conn = _configuration[key];
            var encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //var dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\" ");

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
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\" ");
            Console.WriteLine($"}},");


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
            Console.WriteLine($"}},");

            #endregion

            #region AuthServer

            node = "AuthServer";
            Console.WriteLine($"\"{node}\":{{");
            Console.WriteLine($"    \"Authority\": \"https://localhost:44331\",");
            Console.WriteLine($"    \"RequireHttpsMetadata\": true,");
            Console.WriteLine($"    \"ClientId\": \"Parakeet_Web\",");

            dbKey = "ClientSecret";
            key = $"{node}:{dbKey}";
            conn = _configuration[key];
            encryptStr = EncodingEncryptHelper.Encrypt(conn);
            //dEncryptStr = EncodingEncryptHelper.DEncrypt(encryptStr);
            //Console.WriteLine($"{key}={conn}\n加密后：encryptStr={encryptStr}\n解密后：dEncryptStr={dEncryptStr}");
            //Console.WriteLine($"{key}==dEncryptStr? {conn == dEncryptStr}");
            Console.WriteLine($"    \"{dbKey}\": \"{encryptStr}\",");

            Console.WriteLine($"    \"IsContainerized\": false");
            Console.WriteLine($"}},");

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

            Console.WriteLine($"}},");

            #endregion
        }

        #endregion

        #region 同位数练习
        {

            ////Given two strings s and t, return true if t is an anagram of s, and false otherwise.
            ////An Anagram is a word or phrase formed by rearranging the letters of a different word or phrase, typically using all the original letters exactly once.
            ////Requirement: You cannot use methods in linq and class libraries.
            ////Example 1:
            ////Input: s = "anagram", t = "nagaram"
            ////Output: true

            ////Example 2:
            ////Input: s = "rat", t = "car"
            ////Output: false

            //var parameterS = Console.ReadLine();
            //var parameterT = Console.ReadLine();

            //var isAnagram = IsAnagram(parameterS, parameterT);
            //Console.WriteLine($"{parameterT} is an anagram of {parameterS} :{isAnagram}");

            //Console.ReadKey();
            bool IsAnagram(string s, string t)
            {
                //题意说每个字母只使用一遍，例如Example 1中原字符串中a出现了三次，那变位数也只能出现三次，并且变位数长度一定等于原字符串长度
                //不能用任何linq和类库，但是可以使用自带的dictionary，笨办法，循环多次，这题挺有意思，细节挺多
                var result = s.Length == t.Length;
                if (result)
                {
                    var sLetterDic = GetLetterDic(s);
                    var tLetterDic = GetLetterDic(t);
                    foreach (var tLetter in t)
                    {
                        result = sLetterDic.ContainsKey(tLetter) && sLetterDic[tLetter] == tLetterDic[tLetter];
                        if (!result)
                        {
                            break;
                        }
                    }
                }
                return result;
            }

            ConcurrentDictionary<char, int> GetLetterDic(string str)
            {
                var letterDic = new ConcurrentDictionary<char, int>();
                foreach (char sLetter in str)
                {
                    if (letterDic.ContainsKey(sLetter))
                    {
                        letterDic[sLetter]++;
                    }
                    else
                    {
                        letterDic.TryAdd(sLetter, 1);
                    }
                }
                return letterDic;
            }
        }

        #endregion


        #region Guid.Parse 有序Guid生成 测试

        {
            var guidOne = _guidGenerator.Create();
            var guidTwo = _guidGenerator.Create();
            var guidThree = _guidGenerator.Create();

            Console.WriteLine($" guidOne:{guidOne}\r\n guidTwo:{guidTwo}\r\n guidThree:{guidThree}");
            Console.ReadKey();

            ////Console.WriteLine($"{Guid.Parse("e9f8e91180e941759adf1a85944ada50")}");//Guid.Parse 可以添加上短横线
            var option = new AbpSequentialGuidGeneratorOptions
            {
                //DefaultSequentialGuidType = SequentialGuidType.SequentialAtEnd//sqlserver=>SequentialAtEnd
                //DefaultSequentialGuidType = SequentialGuidType.SequentialAsBinary
                DefaultSequentialGuidType = SequentialGuidType.SequentialAsString//mysql=>SequentialAsString
            };
            var optionWarpper = new OptionsWrapper<AbpSequentialGuidGeneratorOptions>(option);
            ////由timeStamp二进制转换的一定时间顺序的guid 够用约5900年，满足大部分项目
            var sequentialGuidGenerator = new SequentialGuidGenerator(optionWarpper);
            //var sequenceGuidNext1 = sequentialGuidGenerator.Create();
            //var sequenceGuidNext2 = sequentialGuidGenerator.Create();
            //var guid = SimpleGuidGenerator.Instance.Create();//=>等同于Guid.NewGuid();
            //Console.WriteLine($"sequenceGuidNext1:{sequenceGuidNext1}\nsequenceGuidNext2:{sequenceGuidNext2}\nsimpleGuid:{guid}");
            //Console.ReadKey();

            //var sqType = WorkType.设计;
            Console.WriteLine($"打印SequentialGuidType枚举字符串");
            Console.WriteLine($"{SequentialGuidType.SequentialAsString}");
            Console.WriteLine($"{SequentialGuidType.SequentialAsBinary}");
            Console.WriteLine($"{SequentialGuidType.SequentialAtEnd}");
            //Console.WriteLine($"sqType:{sqType}");
            //Console.ReadKey();
        }
        #endregion

    }
}
