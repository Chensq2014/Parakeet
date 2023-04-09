using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// 对象扩展
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 默认 JsonSetting 配置
        /// </summary>
        static readonly JsonSerializerSettings DefaultJsonSetting = new JsonSerializerSettings();
        
        #region json 对象扩展

        /// <summary>
        /// 对象转json格式数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, DefaultJsonSetting);
        }

        /// <summary>
        /// 对象转json格式数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, JsonSerializerSettings setting)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, setting);
        }

        /// <summary>
        /// 对象转json格式数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(this object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, typeof(T), DefaultJsonSetting);
        }

        /// <summary>
        /// 对象转json格式数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string ToJson<T>(this object obj, JsonSerializerSettings setting)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, typeof(T), setting);
        }

        /// <summary>
        /// 将对象转成JSON字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="lowerCase">是否小驼峰</param>
        /// <param name="indented">是否缩进</param>
        /// <param name="timeFormat">时间格式转换规则</param>
        /// <returns>JSON字符串</returns>
        public static string ToJsonString(this object obj, bool lowerCase = false, bool indented = false, string timeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            var settings = new JsonSerializerSettings()
            {
                DateFormatString = timeFormat
            };
            if (lowerCase)
            {
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            if (indented)
            {
                settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// 将对象转成JSON字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="converters">JSON转换器</param>
        /// <param name="lowerCase">是否小驼峰</param>
        /// <param name="indented">是否缩进</param>
        /// <param name="timeFormat">时间格式转换规则</param>
        /// <returns>JSON字符串</returns>
        public static string ToJsonString(this object obj, IEnumerable<JsonConverter> converters, bool lowerCase = false, bool indented = false, string timeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatString = timeFormat
            };
            if (lowerCase)
            {
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            if (indented)
            {
                settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }
            (settings.Converters as List<JsonConverter>)?.AddRange(converters);
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// 将JSON字符串转成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">JSON字符串</param>
        /// <returns></returns>
        public static T FromJsonString<T>(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 将JSON字符串转成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">JSON字符串</param>
        /// <returns></returns>
        public static T FromObject<T>(this object value)
        {
            //IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return value != null 
                ? FromJsonString<T>(value is string 
                    ? value.ToString() 
                    : JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.None, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" })) 
                : default(T);
        }

        #endregion

        #region Xml
        /// <summary>
        /// 对象转xml格式数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding">编码格式</param>
        /// <param name="omitXmlDeclaration">是否去除xml的声明</param>
        /// <param name="omitNameSpaces">是否忽略命名空间</param>
        /// <param name="indent">是否缩进元素</param>
        /// <param name="indentChars">缩进格式化字符</param>
        /// <returns></returns>
        public static string ToXml(this object obj, Encoding encoding = null, bool omitXmlDeclaration = false, bool omitNameSpaces = true, bool indent = true, string indentChars = null)
        {
            if (obj == null)
                return null;
            if (encoding == null)
                encoding = new UTF8Encoding(false);

            XmlSerializer ser = new XmlSerializer(obj.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            //去除xml声明
            settings.OmitXmlDeclaration = omitXmlDeclaration;
            settings.Encoding = encoding;
            settings.Indent = indent;
            if (indent && indentChars != null)
                settings.IndentChars = indentChars;

            using MemoryStream ms = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(ms, settings))
            {
                if (omitNameSpaces)
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    ser.Serialize(writer, obj, ns);
                }
                else
                {
                    ser.Serialize(writer, obj);
                }
            }
            return encoding.GetString(ms.ToArray());
        }

        #endregion

    }
}
