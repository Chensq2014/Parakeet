using System;
using System.Text;

namespace Parakeet.Net.Converter.Server
{
    /// <summary>
    /// 二进制转换器
    /// </summary>
    public class BINServer : BaseServer
    {
        public override char[] CharArray => SystemConstant.BINCharArray.ToCharArray();
        public override int BitType => SystemConstant.BINType;

        /// <summary>
        /// 字符串转二进制
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string StrToBin(string str)
        {
            byte[] data = Encoding.Unicode.GetBytes(str);
            StringBuilder sb = new StringBuilder(data.Length * 8);
            foreach (byte item in data)
            {
                sb.Append(Convert.ToString(item, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 二进制转字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string BinToStr(string str)
        {
            System.Text.RegularExpressions.CaptureCollection cs = System.Text.RegularExpressions.Regex.Match(str, @"([01]{8})+").Groups[1].Captures;
            byte[] data = new byte[cs.Count];
            for (int i = 0; i < cs.Count; i++)
            {
                data[i] = Convert.ToByte(cs[i].Value, 2);
            }
            return Encoding.Unicode.GetString(data, 0, data.Length);
        }
    }
}
