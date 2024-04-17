using System;
using Common.Extensions;
using System.IO;
using System.Text;

namespace ConsoleApp.Extends
{
    public class Base64Util
	{
		private static readonly char[] base64EncodeChars = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/' };

		private static sbyte[] base64DecodeChars = new sbyte[] { (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, 62, (sbyte)-1, (sbyte)-1, (sbyte)-1, 63, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1, (sbyte)-1 };

		private Base64Util()
		{
		}

		/// <summary>
		/// 将字节数组编码为字符串
		/// </summary>
		/// <param name="data"> </param>
		public static string Encode(byte[] data)
		{
			StringBuilder sb = new StringBuilder();
			int len = data.Length;
			int i = 0;
			int b1, b2, b3;

			while (i < len)
			{
				b1 = data[i++] & 0xff;
				if (i == len)
				{
					sb.Append(base64EncodeChars[(int)((uint)b1 >> 2)]);
					sb.Append(base64EncodeChars[(b1 & 0x3) << 4]);
					sb.Append("==");
					break;
				}
				b2 = data[i++] & 0xff;
				if (i == len)
				{
					sb.Append(base64EncodeChars[(int)((uint)b1 >> 2)]);
					sb.Append(base64EncodeChars[((b1 & 0x03) << 4) | ((int)((uint)(b2 & 0xf0) >> 4))]);
					sb.Append(base64EncodeChars[(b2 & 0x0f) << 2]);
					sb.Append("=");
					break;
				}
				b3 = data[i++] & 0xff;
				sb.Append(base64EncodeChars[(int)((uint)b1 >> 2)]);
				sb.Append(base64EncodeChars[((b1 & 0x03) << 4) | ((int)((uint)(b2 & 0xf0) >> 4))]);
				sb.Append(base64EncodeChars[((b2 & 0x0f) << 2) | ((int)((uint)(b3 & 0xc0) >> 6))]);
				sb.Append(base64EncodeChars[b3 & 0x3f]);
			}
			return sb.ToString();
		}

		/// <summary>
		/// 将base64字符串转换转换为byte数组
		/// </summary>
		/// <param name="str"> </param>
		//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		//ORIGINAL LINE: public static byte[] decode(String str) throws Exception
		public static byte[] Decode(string str)
        {
			
            byte[] data = str.SerializeToUtf8Bytes();//str.GetBytes(Encoding.UTF8);
			int len = data.Length;
			var buf = new MemoryStream(len);
			int i = 0;
			int b1, b2, b3, b4;

			while (i < len)
			{
				/* b1 */
				do
				{
					b1 = base64DecodeChars[data[i++]];
				} while (i < len && b1 == -1);
				if (b1 == -1)
				{
					break;
				}

				/* b2 */
				do
				{
					b2 = base64DecodeChars[data[i++]];
				} while (i < len && b2 == -1);
				if (b2 == -1)
				{
					break;
				}

                var x = b1 << 2;
                var y = ((int) ((uint) (b2 & 0x30) >> 4));
                byte z =  (byte)(x | y);
				buf.WriteByte(z);

				/* b3 */
				do
				{
					b3 = data[i++];
					if (b3 == 61)
					{
						return buf.ToArray();
					}
					b3 = base64DecodeChars[b3];
				} while (i < len && b3 == -1);
				if (b3 == -1)
				{
					break;
				}
				buf.WriteByte((byte)(((b2 & 0x0f) << 4) | ((int)((uint)(b3 & 0x3c) >> 2))));

				/* b4 */
				do
				{
					b4 = data[i++];
					if (b4 == 61)
					{
						return buf.ToArray();
					}
					b4 = base64DecodeChars[b4];
				} while (i < len && b4 == -1);
				if (b4 == -1)
				{
					break;
				}
				buf.WriteByte((byte)(((b3 & 0x03) << 6) | b4));
			}
			return buf.ToArray();
		}

	}

}
