using System.Text;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// 二进制扩展
    /// </summary>
    public static class BinaryExtensions
    {
        /// <summary>
        /// 2进制转16进制输出
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="withSpace"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] bits, bool withSpace = false)
        {
            var builder = new StringBuilder();
            foreach (var bit in bits)
            {
                builder.Append(bit.ToString("X2"));
                if (withSpace)
                {
                    builder.Append(" ");
                }
            }

            return builder.ToString();
        }
    }
}
