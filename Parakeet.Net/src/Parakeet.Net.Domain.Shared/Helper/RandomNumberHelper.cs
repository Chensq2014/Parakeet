using System;

namespace Parakeet.Net.Helper
{
    public static class RandomNumberHelper
    {
        /// <summary>
        /// 生成随机decimal数
        /// </summary>
        /// <param name="min">种子/最小区间</param>
        /// <param name="max">最大区间值</param>
        /// <param name="determinant">系数</param>
        /// <returns></returns>
        public static decimal RandomDecimal(int min = 0, int max = 1, decimal determinant = 0.1m)
        {
            min = min <= max ? min : max;
            var random = new Random(min).Next(min, max) * determinant;
            return random;
        }

        /// <summary>
        /// 生成随机整数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="determinant">系数</param>
        /// <returns></returns>
        public static int RandomInt(int min = 0, int max = 1, int determinant = 1)
        {
            min = min <= max ? min : max;
            var random = new Random(min).Next(min, max) * determinant;
            return random;
        }

        /// <summary>
        /// 准备18位身份证号
        /// </summary>
        /// <returns></returns>
        public static string RandomIdCard()
        {
            return $"500223{DateTime.Now.AddYears(new Random().Next(-30, -20)):yyyyMMdd}1751";
        }
    }
}
