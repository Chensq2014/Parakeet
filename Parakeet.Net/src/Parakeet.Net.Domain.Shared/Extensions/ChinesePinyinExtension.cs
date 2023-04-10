using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Text;
using Serilog;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// 汉语转拼音
    /// </summary>
    public class ChinesePinyinExtension
    {
        //private static Encoding _unicode = Encoding.GetEncoding("GB2312");

        /// <summary>
        /// 汉字转全拼
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string ConvertToAllSpell(string strChinese)
        {
            try
            {
                if (strChinese.Length != 0)
                {
                    var fullSpell = new StringBuilder();
                    foreach (var chr in strChinese)
                    {
                        fullSpell.Append(GetSpell(chr));
                    }

                    return fullSpell.ToString().ToUpper();
                }
            }
            catch (Exception e)
            {
                Log.Logger.Information($"全拼转化出错！{e.Message}");
            }

            return string.Empty;
        }

        /// <summary>
        /// 汉字转首字母
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string GetFirstSpell(string strChinese)
        {
            //NPinyin.Pinyin.GetInitials(strChinese)  有Bug  洺无法识别
            //return NPinyin.Pinyin.GetInitials(strChinese);

            try
            {
                if (strChinese.Length != 0)
                {
                    var fullSpell = new StringBuilder();
                    foreach (var chr in strChinese)
                    {
                        fullSpell.Append(GetSpell(chr)[0]);
                    }

                    return fullSpell.ToString().ToUpper();
                }
            }
            catch (Exception e)
            {
                Log.Logger.Information($"首字母转化出错！{e.Message}");
            }

            return string.Empty;
        }

        private static string GetSpell(char chr)
        {
            var coverchr = NPinyin.Pinyin.GetPinyin(chr);

            bool isChineses = ChineseChar.IsValidChar(coverchr[0]);
            var chineseChar = new ChineseChar(coverchr[0]);
            if (isChineses)
            {
                foreach (var value in chineseChar.Pinyins)
                {
                    if (value.HasValue())
                    {
                        return value.Remove(value.Length - 1, 1);
                    }
                }
            }
            return coverchr;
        }
    }
}
