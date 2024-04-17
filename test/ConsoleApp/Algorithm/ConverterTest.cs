using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NPOI.SS.Util;
using Common.Converter.Server;
using Common.Extensions;
using RestSharp.Extensions;

namespace ConsoleApp
{
    /// <summary>
    /// 进制转换
    /// </summary>
    public class ConverterTest
    {
        /// <summary>
        /// 十六进制转十进制
        /// 写出一个程序，接受一个十六进制的数，输出该数值的十进制表示。
        /// Convert.ToString(number, 2);/8/10/16
        /// </summary>
        public static void HexToDec()
        {
            //第一种方式：Convert.ToInt32(input, 16)
            DoWihileReadTrue();

            var input = Console.ReadLine();//input字符串是ASCII码 假设输入一个十六进制字符串
            
            //这里只能用Unicode编码，因为字符串默认是Unicode编码的，Unicode编码有4个字节，Default就只有2个字节
            var binary = Encoding.Unicode.GetBytes(input);//Encoding.Unicode.GetString(binary)
                                                          //处理每一个bit 转十六进制
                                                          //var bitToHex = string.Empty;//BitConverter.ToString(binary)?.Replace("-", "");

            var sb = new StringBuilder(binary.Length * 8);//每个字节8个bit位长度

            foreach (var bit in binary)
            {
                var bit2Bin = Convert.ToString(bit, 2);
                var hexStr = Convert.ToString(bit, 16);
                //bitToHex += $"{item}";
                Console.WriteLine($"每个bit十六进制字符串：{hexStr}");//.ToUpper()
                Console.WriteLine($"每个bit十进制：{Convert.ToString(bit, 10)}");
                Console.WriteLine($"每个bit二进制：{bit2Bin}");
                Console.WriteLine($"每个bit转8位二进制：{bit2Bin.PadLeft(8, '0')}");
                Console.WriteLine($"每个bit十六进制字符串转十进制：{Convert.ToInt32(hexStr, 16)}");

                //一个字节8个bit 不足够就向左边补0
                sb.Append(bit2Bin.PadLeft(8, '0'));
            }

            Console.WriteLine($"字符串{input} Unicode编码转二进制字符串：{sb}");
            Console.WriteLine($"字符串{input}转十六进制字符串BitConverter.ToString(binary)：{BitConverter.ToString(binary)}");

            Console.WriteLine($"----------------------------------------------------------------------------------");

            //进制转换服务
            var hexServer = new HEXServer();
            var binServer = new BINServer();
            var decServer = new DECServer();

            var decValue = hexServer.Self2DEC(input).Result;
            var binValue = binServer.DEC2Self(decValue).Result;

            Console.WriteLine($"十六进制{input}转十进制：{decValue}");
            Console.WriteLine($"十六进制{input}转十进制：{decValue}=>十再转二：{binValue}");

            Console.WriteLine($"----------------------------------------------------------------------------------");


            var bin2str0 = binServer.BinToStr(sb.ToString());

            var input2bin = binServer.StrToBin(input);
            var bin2str = binServer.BinToStr(input2bin);
            Console.WriteLine($"字符串{input}Unicode编码转二进制：{input2bin}");
            Console.WriteLine($"二进制{input2bin}转字符串：{bin2str}");
            Console.WriteLine($"二进制{sb}转字符串：{bin2str0}");


            Console.WriteLine($"----------------------------------------------------------------------------------");

            var num = int.Parse(decValue);
            //Console.WriteLine($"数字{num}转字符串：{char.Parse(num.ToString())}");
            Console.WriteLine($"数字{num}转2进制：{Convert.ToString(num, 2)}");
            Console.WriteLine($"数字{num}转8进制：{Convert.ToString(num, 8)}");
            Console.WriteLine($"数字{num}转10进制：{Convert.ToString(num, 10)}");
            Console.WriteLine($"数字{num}转16进制：{Convert.ToString(num, 16)}");

            //0x2a=2*16+a=2*16+10=42  0x标识十六进制  2a标识 十六进制字符
            Console.WriteLine($"数字{0xa}转10进制：{Convert.ToString(0xa, 10)}");
            Console.WriteLine($"数字{0xa}转2进制：{Convert.ToString(0xa, 2)}");


            var hexTestStr = hexServer.DEC2Self(796.ToString());
            Console.WriteLine($"十进制数字{796}转16进制：{hexTestStr}");

        }


        /// <summary>
        /// 一直输入 转成功为止
        /// </summary>
        public static void DoWihileReadTrue()
        {
            string input;
            while ((input= Console.ReadLine())!=null)
            {
                if (input.Contains("0x") == true)
                {
                    input = input.Replace("0x", "");
                }
                var charArray = input.ToCharArray();
                var hexChars = $"0123456789AaBbCcDdEe";
                var valid = true;
                foreach (var c in charArray)
                {
                    if (!hexChars.Contains(c))
                    {
                        //重新输入
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    Console.WriteLine($"{Convert.ToInt32(input, 16)}");
                }
                else
                {
                    Console.WriteLine($"{input}无效，16进制字符串只能包含{hexChars},请重新输入:");
                    HexToDec();
                }
            }
        }
    }
}
