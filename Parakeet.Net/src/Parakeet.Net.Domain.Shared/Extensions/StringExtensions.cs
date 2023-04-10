using Newtonsoft.Json;
using Parakeet.Net.CustomAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    ///     字符串静态扩展类
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>Check that a string is not null or empty</summary>
        /// <param name="input">String to check</param>
        /// <returns>bool</returns>
        public static bool HasValue(this string input)
        {
            return !String.IsNullOrWhiteSpace(input);
        }

        /// <summary>
        /// format扩展
        /// </summary>
        /// <param name="source"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string FormatEx(this string source, params object[] items)
        {
            return String.Format(source, items);
        }

        /// <summary>
        /// contains扩展
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsEx(this string source, string value)
        {
            return !source.HasValue() && source.Contains(value);
        }

        /// <summary>
        /// 转换为整数
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string source, int defaultValue = 0)
        {
            if (!Int32.TryParse(source, out var result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 转换为long整数
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToInt64(this string source, long defaultValue = 0)
        {
            if (!Int64.TryParse(source, out var result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 转换为decimal类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string source, decimal defaultValue = 0)
        {
            if (!Decimal.TryParse(source, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        ///     json格式数据转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T ToJson<T>(this string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }

        /// <summary>
        ///     json格式数据转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static T ToJson<T>(this string source, Type sourceType)
        {
            return (T)JsonConvert.DeserializeObject(source, sourceType);
        }

        /// <summary>
        /// 将JSON字符串转换为匿名类型
        /// </summary>
        /// <typeparam name="T">匿名类型</typeparam>
        /// <param name="json">JSON字符串 </param>
        /// <param name="anonymousTypeObject">匿名类型</param>
        /// <returns></returns>
        public static T FromJsonString<T>(this string json, T anonymousTypeObject)
        {
            return string.IsNullOrWhiteSpace(json)
                ? default(T)
                : JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
        }

        /// <summary>
        /// 将JSON字符串转成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">JSON字符串 </param>
        /// <param name="converters">JSON转换器</param>
        /// <returns></returns>
        public static T FromJsonString<T>(this string json, IEnumerable<JsonConverter> converters)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            var settings = new JsonSerializerSettings();
            (settings.Converters as List<JsonConverter>)?.AddRange(converters);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
        
        /// <summary>
        ///     转<see cref="DateTime" />对象
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string source)
        {
            DateTime.TryParse(source, out var date);
            return date;
        }

        /// <summary>
        ///     转<see cref="DateTime" />对象，如果时间小于1970/1/1则返回null
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime? ToDbDateTime(this string source)
        {
            DateTime.TryParse(source, out var date);
            if (date < new DateTime(1970, 1, 1))
            {
                return null;
            }
            return date;
        }

        /// <summary>
        ///     转枚举对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string source)
        {
            return (T)Enum.Parse(typeof(T), source);
        }


        /// <summary>
        /// xml格式数据转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T ToXml<T>(this string xml, Encoding encoding = null)
        {
            if (xml.IsNullOrWhiteSpace())
            {
                return default(T);
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(xml)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return (T)ser.Deserialize(sr);
                }
            }
        }

        /// <summary>
        /// 转化为半角字符串
        /// </summary>
        /// <param name="input">要转化的字符串</param>
        /// <returns>半角字符串</returns>
        public static string ToSbc(this string input)//single byte charactor
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)//全角空格为12288，半角空格为32
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)//其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
                {
                    c[i] = (char)(c[i] - 65248);
                }
            }
            return new string(c);
        }

        /// <summary>
        /// 转化为全角字符串
        /// </summary>
        /// <param name="input">要转化的字符串</param>
        /// <returns>全角字符串</returns>
        public static string ToDbc(this string input)//double byte charactor 
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                {
                    c[i] = (char)(c[i] + 65248);
                }
            }
            return new string(c);
        }


        /// <summary>
        /// 字符串删除最后一个规定字符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="removeChar">最后要移除的字符</param>
        /// <returns></returns>
        public static string RemoveLastChar(this string str, char removeChar)
        {
            if (str.HasValue())
            {
                str = str.Substring(0, str.LastIndexOf(removeChar));
            }
            return str;
        }

        /// <summary>
        /// 保证字符串的首字母大写
        /// Uppercase first letters of all words in the string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToInitialCapitalization(this string str)
        {
            return Regex.Replace(str, Regexes.CamelCaseFirst, match =>
           {
               string v = match.ToString();
               return Char.ToUpper(v[0]) + v.Substring(1);
           });
        }

        //public static string ToInitialCapitalization2(string s)
        //{
        //    s = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
        //    return s;
        //}
        //public static string ToInitialCapitalization3(string s)
        //{
        //    if (s.HasValue())
        //    {
        //        s = s.Length >= 2 ? char.ToUpper(s[0]) + s.Substring(1) : char.ToUpper(s[0]).ToString();
        //    }
        //    return s;
        //}

        /// <summary>
        /// 首字母小写 驼峰命名规则
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCamelCase(string str)
        {
            if (str.HasValue())
            {
                str = str.Length > 2 ? Char.ToLower(str[0]) + str.Substring(1) : Char.ToLower(str[0]).ToString();
            }
            return str;
        }

        /// <summary>
        /// 实体名转换为前端html标签id格式BaseEntity(-base-enity)
        /// 将大写字母转换为'-小写字母格式'
        /// 返回模块名+实体组成的htmlid格式字符串： name-base-entity 
        /// </summary>
        /// <param name="htmlId">传入的大小写单词字符串</param>
        /// <returns></returns>
        public static string ToHtmlId(this string htmlId)
        {
            #region 以前的方法体

            //if (htmlId.HasValue())
            //{
            //    var chars = htmlId.ToCharArray();
            //    var newchars = new List<char>();
            //    for (var index = 0; index < chars.Count(); index++)
            //    {
            //        var letter = chars[index];
            //        if (char.IsUpper(letter))
            //        {
            //            letter = char.ToLower(letter);
            //            chars[index] = letter;
            //            newchars.Add('-');
            //        }
            //        newchars.Add(letter);
            //    }
            //    htmlId = new string(newchars.ToArray());
            //}
            //return htmlId;

            #endregion

            #region 巧用正则表达式 少写验证判断组装数据等逻辑且提高性能

            return Regex.Replace($"{CustomerConsts.AppName.ToLower()}{htmlId}", Regexes.CapitalLetters, match =>
            {
                string v = match.ToString();
                return $"-{Char.ToLower(v[0])}{v.Substring(1)}";
            });

            #endregion
        }


        /// <summary>
        /// 隐藏11位手机号的中间4位
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HideTel(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            var outReplace = Regex.Replace(input, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            return outReplace;
        }

        /// <summary>
        /// 转base64格式数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToBase64(this string source, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return Convert.ToBase64String(encoding.GetBytes(source));
        }

        /// <summary>
        /// 规范正则rule
        /// </summary>
        /// <param name="rule">正则表达式 不为null 传入正则表达式要求要么都带^$，要么都不带^$</param>
        /// <returns></returns>
        public static string SetRegExpression(this string rule)
        {
            //判断是否带开始结束标记，没有就要加上去 传入正则表达式要求要么都带^$，要么都不带^$
            rule = rule.HasValue() && rule.IndexOf("^", StringComparison.Ordinal) == -1// && rule.IndexOf("$", StringComparison.Ordinal) == -1
                ? $@"^{rule}$"
                : rule;
            return rule;
        }

        /// <summary>
        ///     给base64图片加上默认(自定义)前缀png
        /// </summary>
        /// <param name="str"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string KeepBase64ImagePrefix(this string str, string prefix = null)
        {
            prefix = str.HasValue() || str.Contains("base64,")
                ? ""
                : prefix ?? "data:image/png;base64,";
            str = $"{prefix}{str}";
            return str;
        }

        /// <summary>
        ///     去base64图片前缀
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveBase64ImagePrefix(this string str)
        {
            return str?.Split(',').LastOrDefault();
        }

        /// <summary>
        ///     Base64图片UrlEncode编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(this string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 计算ip的数值
        /// </summary>
        /// <param name="ipStr"></param>
        /// <returns></returns>
        public static long? IpToNumber(this string ipStr)
        {
            if (ipStr.HasValue())
            {
                var pass = Regexes.IsMatch(ipStr, Regexes.Ipv4);
                if (!pass)
                {
                    throw new Exception($"{ipStr}不是有效的Ipv4地址");
                }
            }
            double? ipNumber = null;
            var baseNum = 255;
            var nums = ipStr?.Split(".")?.Reverse()?.ToList();
            for (int i = 0; i < nums?.Count; i++)
            {
                ipNumber = (ipNumber ?? 0) + (int.Parse(nums[i]) * Math.Pow(baseNum, i));
            }

            return (long?)ipNumber;
        }
    }
}