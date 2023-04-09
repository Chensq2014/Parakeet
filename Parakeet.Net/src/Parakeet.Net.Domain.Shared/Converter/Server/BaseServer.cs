using System;
using System.Linq;
using System.Threading.Tasks;

namespace Parakeet.Net.Converter.Server
{
    /// <summary>
    /// 进制转换服务
    /// </summary>
    public abstract class BaseServer
    {
        /// <summary>
        /// 进制字符数组
        /// </summary>
        public abstract char[] CharArray { get; }

        /// <summary>
        /// 进制类型 2 4 8 16
        /// </summary>
        public abstract int BitType { get; }

        /// <summary>
        /// 进制字符转整数
        /// </summary>
        /// <param name="charVal"></param>
        /// <returns></returns>
        protected virtual int CharToInt(char charVal)
        {
            return int.Parse(charVal.ToString());
        }


        /// <summary>
        /// 整数转字符
        /// </summary>
        /// <param name="charVal"></param>
        /// <returns></returns>
        protected virtual char IntToChar(int charVal)
        {
            return char.Parse(charVal.ToString());
        }


        /// <summary>
        /// 验证是否所属进制字符串
        /// </summary>
        /// <param name="val"></param>
        /// <param name="charArray"></param>
        /// <returns></returns>
        protected bool IsValid(string val, char[] charArray)
        {
            foreach (var s in val)
            {
                if (charArray.Any(x => x == s))
                {
                    continue;
                }

                return false;
            }

            return true;
        }


        /// <summary>
        /// 当前进制转十进制
        /// </summary>
        /// <returns></returns>
        public virtual async Task<string> Self2DEC(string originalValue)
        {
            if (IsValid(originalValue, CharArray) == false)
            {
                return "值无效";
            }

            var finalValue = 0;
            var charArray = originalValue.ToCharArray().Reverse().ToArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                var bitValue = CharToInt(charArray[i]);
                if (bitValue != 0)
                {
                    finalValue += bitValue * (int)Math.Pow(BitType, i);
                }
            }
            return await Task.FromResult(finalValue.ToString());
        }


        /// <summary>
        /// 十进制转当前进制
        /// </summary>
        /// <param name="originalValue"></param>
        /// <returns></returns>
        public virtual async Task<string> DEC2Self(string originalValue)
        {
            if (IsValid(originalValue, SystemConstant.DECCharArray.ToCharArray()) == false)
            {
                return "值无效";
            }
            var container = new BitContainer();
            var parseVal = int.Parse(originalValue);

            //int32类型的整数,所以最大32位
            Recursion(parseVal, 64);

            void Recursion(int val, int maxI)
            {
                for (int i = 0; i < maxI; i++)
                {
                    //除数
                    var divisor = (int)Math.Pow(BitType, i);
                    //求商
                    var quotient = val / divisor;
                    if (quotient < BitType)
                    {
                        char quotientStr = IntToChar(quotient);
                        container.Add(i, quotientStr);
                        //取模
                        var modulus = val % divisor;
                        Recursion(modulus, i);
                        return;
                    }
                }
            }

            /*例：将十进制的(796)D转换为十六进制的步骤如下：

            1. 将商796除以16，商49余数为12，对应十六进制的C；

            2. 将商49除以16，商3余数为1；

            3. 将商3除以16，商0余数为3；

            4. 读数，因为最后一位是经过多次除以16才得到的，因此它是最高位，读数字从最后的余数向前读，31C，即(796)D=(31C)H。
            
            */
            return await Task.FromResult(container.ToString());
        }

    }
}
