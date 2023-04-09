using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// System.Text.Json 的Json序列化
    /// </summary>
    public static class JsonSerializerExtensions
    {
        static readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,//序列化后驼峰命名规则
            IgnoreNullValues = true,
            MaxDepth = 1//导航属性序列化深度 默认为0
        };

        public static T Deserialize<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }

        public static object Deserialize(this byte[] bytes, Type type)
        {
            return JsonSerializer.Deserialize(bytes, type, options);
        }

        public static string Serialize<T>(this T data)
        {
            return JsonSerializer.Serialize(data, options);
        }

        public static string Serialize<T>(this T data, JsonSerializerOptions serializerOptions)
        {
            return JsonSerializer.Serialize(data, serializerOptions);
        }

        public static string Serialize(this object data, Type type)
        {
            return JsonSerializer.Serialize(data, type, options);
        }

        public static byte[] SerializeToUtf8Bytes<T>(this T data)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data, options);
        }

        public static T Deserialize<T>(this byte[] bytes) where T : class, new()
        {
            return JsonSerializer.Deserialize<T>(bytes, options);
        }

        public static byte[] SerializeToUtf8Bytes(this object data, Type type)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data, type, options);
        }

        public static object Deserialize(this string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type);
        }
    }
}
