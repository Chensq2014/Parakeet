using System;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// guid扩展
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// 根据Guid获取唯一数字序列(二进制数组就是唯一的转为long即可)
        /// </summary>
        /// <returns></returns>
        public static long ToInt64()
        {
            return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(),0);
        }
    }
}
