namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 宇泛人脸识别通用返回类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultInfo<T>
    {
        /// <summary>
        /// 设备Code(不同于序列号和编码)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 表示接口是否调通，1 成功，0 失败，通常只要设备服务器能响应， 该值均为 1
        /// </summary>
        public int Result;

        /// <summary>
        /// 此次操作是否成功，成功为 true，失败为 false 
        /// </summary>
        public bool Success;

        /// <summary>
        /// 接口返回的业务数据，类型可为数值、字符串或集合等
        /// </summary>
        public T Data;

        /// <summary>
        /// 接口返回的信息，通常是错误类型码的原因信息
        /// </summary>
        public string Msg;
    }
}
