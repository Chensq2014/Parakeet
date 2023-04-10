using Newtonsoft.Json;

namespace Parakeet.Net.Extensions.Mapping
{
    /// <summary>
    /// 使用第三方序列化反序列化工具
    /// 还有automapper
    /// </summary>
    public class SerializeMapper
    {
        /// <summary>
        /// 序列化反序列化方式
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        public static TOut Trans<TIn, TOut>(TIn tIn)
        {
            return JsonConvert.DeserializeObject<TOut>(JsonConvert.SerializeObject(tIn));
        }

        public void BinarySerialize()
        {

        }
    }
}
