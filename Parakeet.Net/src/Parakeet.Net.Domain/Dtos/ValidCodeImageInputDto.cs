using System;
using System.Text;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 验证码传输类
    /// </summary>
    public class ValidCodeImageInputDto : ImageInputDto
    {
        /// <summary>
        /// 生成验证码的大写字母和数字字符
        /// </summary>
        const string Letters = "ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789";

        /// <summary>
        /// 构造函数
        /// </summary>
        public ValidCodeImageInputDto()
        {
            GenerateCode();//验证码随机生成
            XLength = 150;
            YLength = 25;
            ZLength = 0;
        }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// x坐标宽度
        /// </summary>
        public int XLength { get; set; }

        /// <summary>
        /// y坐标高度
        /// </summary>
        public int YLength { get; set; }

        /// <summary>
        /// z坐标深度
        /// </summary>
        public int ZLength { get; set; }

        /// <summary>
        /// 验证码base64字符串
        /// </summary>
        public string CodeBase64String { get; set; }

        /// <summary>
        /// 随机抽取5个字符赋值给Code
        /// </summary>
        /// <returns></returns>
        private string GenerateCode()
        {
            var random = new Random();
            var sb = new StringBuilder();
            //添加随机的五个字母
            for (int x = 0; x < 5; x++)
            {
                string letter = Letters.Substring(random.Next(0, Letters.Length - 1), 1);
                sb.Append(letter);
            }
            Code = sb.ToString();
            return Code;
        }
    }
}
