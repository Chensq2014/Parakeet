using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// System.Text.Json 序列化
    /// </summary>
    public static class TextJsonConvert
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,//序列化后驼峰命名规则
            IgnoreNullValues = true,
            //MaxDepth = 1//导航属性序列化深度 默认为0
        };

        public static string SerializeObject<T>(T obj)
        {
            return obj.Serialize();
        }

        public static string SerializeObject<T>(T obj, JsonSerializerOptions options)
        {
            options.WriteIndented = true;
            return obj.Serialize(options);
        }

        public static T DeserializeObject<T>(string json) where T : class, new()
        {
            return json.Deserialize<T>();
        }

        public static T Deserialize<T>(this byte[] bytes) where T : class, new()
        {
            return JsonSerializer.Deserialize<T>(bytes, Options);
        }

    }
}
