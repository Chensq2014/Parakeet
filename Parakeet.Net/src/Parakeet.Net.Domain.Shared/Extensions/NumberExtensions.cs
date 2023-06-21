using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// 数字类型静态扩展
    /// </summary>
    public static class NumberExtensions
    {
        private static readonly string _digit = "零壹贰叁肆伍陆柒捌玖";
        private static readonly string _money = "仟佰拾万仟佰拾亿仟佰拾万仟佰拾元角分厘";

        public static int ToInt(this decimal number)
        {
            return (int)number;
        }

        public static int ToInt(this double number)
        {
            return (int)number;
        }

        public static int ToInt(this long number)
        {
            return (int)number;
        }

        public static int ToInt(this float number)
        {
            return (int)number;
        }

        public static long ToLong(this double number)
        {
            return (long)number;
        }

        public static long ToLong(this decimal number)
        {
            return (long)number;
        }

        public static long ToLong(this float number)
        {
            return (long)number;
        }

        public static decimal ToDecimal(this double number)
        {
            return (decimal)number;
        }

        public static decimal ToDecimal(this float number)
        {
            return (decimal)number;
        }

        public static double ToDecimal(this decimal number)
        {
            return (double)number;
        }

        public static float ToFloat(this decimal number)
        {
            return (float)number;
        }
        public static float ToFloat(this double number)
        {
            return (float)number;
        }
        
        #region  统计最大连续数字重复个数

        /// <summary>
        /// 循环计算重复数字个数 
        /// 11123140000000.98   11123140000000 中有  3 一定是连续的数字个数且不为0  因为出现了3个连续的1
        /// 10000430004500   0的最大次数4
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Dictionary<int, int> ComputeMaxContinueNumCount(this long num)
        {
            var integers = num.ToString().ToCharArray().Select(x => (int)char.GetNumericValue(x)).ToList();
            var lastNum = integers.First();
            var index = 0;//, maxContinueValue = 0, maxCount = 0;
            var numContinueCountDic = new Dictionary<int, int>();
            while (true)
            {
                var continueCount = 1;
                while (++index < integers.Count && integers[index] == lastNum) continueCount++;
                //if (continueCount > maxCount) { maxCount = continueCount; maxContinueValue = lastNum; }
                if (numContinueCountDic.ContainsKey(lastNum))
                {
                    if (numContinueCountDic[lastNum] < continueCount)
                    {
                        numContinueCountDic[lastNum] = continueCount;
                    }
                }
                else
                {
                    numContinueCountDic.Add(lastNum, continueCount);
                }
                if (index < integers.Count)
                {
                    lastNum = integers[index];
                }
                else
                {
                    break;
                }
            }
            return numContinueCountDic;
        }

        /// <summary>
        /// 计算末尾0的个数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int ComputeLastZeroContinueCount(this long num)
        {
            var count = 0;
            while (num % 10 == 0)
            {
                count++;
                num = num / 10;
            }
            return count;
        }

        #endregion
            
        #region 关于decimal/数字字符串 与中文金额大写的相互转换算法 待测试
        ///第一种方法：将小写金额转换成大写金额
        ///用正则表达式
        public static string ToChineseString(this string money)
        {
            return decimal.Parse(money).ToChineseString();
            //string str = decimal.Parse(money).ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            //string temp = Regex.Replace(str, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            //return Regex.Replace(temp, ".", m => "负圆空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万億兆京垓秭穰"[m.Value[0] - '-'].ToString());
        }

        /// <summary>
        /// 金额转中文
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string ToChineseString(this decimal? amount)
        {
            return amount?.ToChineseString();
        }

        /// <summary>
        /// 金额转中文
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string ToChineseString(this decimal amount)
        {
            string str = amount.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string temp = Regex.Replace(str, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            if (amount < 0)
            {
                temp = temp.Insert(0, "-");
            }
            return Regex.Replace(temp, ".", m => "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰"[m.Value[0] - '-'].ToString());
        }

        //第二种方法：将小写金额转换成大写金额
        //采用数组
        /// <summary>
        /// 数字金额转中文大写
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string ToChineseBigMoney(this string money)
        {
            string[] myScale = { "分", "角", "元", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "兆", "拾", "佰", "仟" };
            string[] myBase = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string bigMoney = "";
            bool isPoint = false;
            if (money.IndexOf(".", StringComparison.Ordinal) != -1)
            {
                money = money.Remove(money.IndexOf(".", StringComparison.Ordinal), 1);
                isPoint = true;
            }
            for (int i = money.Length; i > 0; i--)
            {
                int myData = Convert.ToInt16(money[money.Length - i].ToString());//?
                bigMoney += myBase[myData];//?
                bigMoney += isPoint ? myScale[i - 1] : myScale[i + 1];
            }
            return bigMoney;
        }

        /// <summary>
        /// 数字字符串转人名币大写 待测试
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string ToChineseMoney(this string money)
        {
            //string[] intArr = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", };
            string[] strArr = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", };
            string[] chinese = { "元", "十", "百", "千", "万", "十", "百", "千", "亿" };
            char[] tmpArr = money.ToArray();
            string tmpVal = string.Empty;
            for (int i = 0; i < tmpArr.Length; i++)
            {
                tmpVal += strArr[tmpArr[i] - 48];//ASCII编码 0为48
                tmpVal += chinese[tmpArr.Length - 1 - i];//根据对应的位数插入对应的单位
            }
            return tmpVal;
        }

        /// <summary>
        /// 整数转中文大写 待测试
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToChineseBigAmount(this int number)
        {
            var res = "";
            var str = number.ToString();
            var schar = str.Substring(0, 1);
            switch (schar)
            {
                case "1":
                    res = "一";
                    break;
                case "2":
                    res = "二";
                    break;
                case "3":
                    res = "三";
                    break;
                case "4":
                    res = "四";
                    break;
                case "5":
                    res = "五";
                    break;
                case "6":
                    res = "六";
                    break;
                case "7":
                    res = "七";
                    break;
                case "8":
                    res = "八";
                    break;
                case "9":
                    res = "九";
                    break;
                default:
                    res = "零";
                    break;
            }
            if (str.Length > 1)
            {
                switch (str.Length)
                {
                    case 2:
                    case 6:
                    case 10:
                    case 14:
                    case 18:
                        res += "十";
                        break;
                    case 3:
                    case 7:
                    case 11:
                    case 15:
                        res += "百";
                        break;
                    case 4:
                    case 8:
                    case 12:
                    case 16:
                        res += "千";
                        break;
                    case 5:
                    case 13:
                        res += "万";
                        break;
                    case 9:
                    case 17:
                        res += "亿";
                        break;
                    default:
                        res += "";
                        break;
                }
                res += ToChineseBigAmount(int.Parse(str.Substring(1, str.Length - 1)));
            }
            return res;
        }

        /// <summary>
        /// decimal转中文大写 待测试
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToChineseBigString(this decimal value)
        {
            var valueStr = value.ToString("000000000000000000.000000000");
            valueStr = valueStr.Remove(valueStr.IndexOf(".", StringComparison.Ordinal), 1);
            var str = new StringBuilder();
            for (int i = 0; i < valueStr.Length; i++)
            {
                if (valueStr[i] != '0')
                {
                    str.Append(_digit[valueStr[i] - '0']);
                    str.Append(_money[i]);
                }
                else if (i > 0 && valueStr[i - 1] != '0')
                {
                    str.Append(_digit[valueStr[i] - '0']);
                }
            }

            return str.ToString().Trim("零".ToCharArray());
        }

        /// <summary>
        /// 中文大写转double数字
        /// </summary>
        /// <param name="chineseStr">中文大写字符串</param>
        /// <returns></returns>
        public static double ToDouble(string chineseStr)
        {
            chineseStr = chineseStr.Replace("亿亿", "兆");
            chineseStr = chineseStr.Replace("万万", "亿");
            chineseStr = chineseStr.Replace("点", "元");
            chineseStr = chineseStr.Replace("块", "元");
            chineseStr = chineseStr.Replace("毛", "角");
            double vResult = 0;
            double vTemp = 0;
            double vNumber = 0; // 当前数字
            int vDecimal = 0; // 是否出现小数点
            foreach (char vChar in chineseStr)
            {
                int i = "零一二三四五六七八九".IndexOf(vChar);
                if (i < 0) i = "洞幺两三四五六拐八勾".IndexOf(vChar);
                if (i < 0) i = "零壹贰叁肆伍陆柒捌玖".IndexOf(vChar);
                if (i > 0)
                {
                    vNumber = i;
                    if (vDecimal > 0)
                    {
                        vResult += vNumber * Math.Pow(10, -vDecimal);
                        vDecimal++;
                        vNumber = 0;
                    }
                }
                else
                {
                    i = "元十百千万亿".IndexOf(vChar);
                    if (i < 0) i = "整拾佰仟万亿兆".IndexOf(vChar);
                    if (i == 5) i = 8;
                    if (i == 6) i = 12;
                    if (i > 0)
                    {
                        if (i >= 4)
                        {
                            vTemp += vNumber;
                            vTemp = vTemp > 0 ? vTemp : 1;
                            vResult += vTemp * Math.Pow(10, i);
                            vTemp = 0;
                        }
                        else vTemp += vNumber * Math.Pow(10, i);
                    }
                    else
                    {
                        i = "元角分".IndexOf(vChar);
                        if (i > 0)
                        {
                            vTemp += vNumber;
                            vResult += vTemp * Math.Pow(10, -i);
                            vTemp = 0;
                        }
                        else if (i == 0)
                        {
                            vTemp += vNumber;
                            vResult += vTemp;
                            vDecimal = 1;
                            vTemp = 0;
                        }
                    }
                    vNumber = 0;
                }
            }
            return vResult + vTemp + vNumber;
        }

        #endregion

        /// <summary>
        /// source是否在min和max中，包含min和max。min&gt;=source&lt;=max
        /// </summary>
        /// <typeparam name="T">值类型或实现了IComparable接口的成员</typeparam>
        /// <param name="source"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool In<T>(this T source, T min, T max) where T : IComparable<T>
        {
            //个值，指示要比较的对象的相对顺序。 返回值的含义如下： 值 含义 小于零 此对象小于 other 参数。 零 此对象等于 other。 大于零 此对象大于
            var s = source as IComparable<T>;
            return s.CompareTo(min) >= 0 && s.CompareTo(max) <= 0;
        }

        public static List<T> OrderByZhNumber<T>(this List<T> data, Func<T, string> field, int charIndex) where T : class
        {
            var zhNumbers = new List<char> { '一', '二', '三', '四', '五', '六', '七', '八', '九', '十' };
            data = data.OrderBy(n => zhNumbers.IndexOf(field(n)[charIndex])).ToList();
            return data;
        }

        public static string NullString(this decimal? amount)
        {
            return amount?.ToString() ?? "NULL";
        }

        public static string NullString(this int? id)
        {
            return id?.ToString() ?? "NULL";
        }

        public static string NullString(this double? amount)
        {
            return amount?.ToString() ?? "NULL";
        }

        /// <summary>
        /// 将10位时间戳Timestamp转换成日期
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static DateTime ToLocalDateTime(this int target)
        {
            //621355968000000000:TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).Ticks
            var date = new DateTime(621355968000000000 + (long)target * 10000000, DateTimeKind.Local);
            return date.ToLocalTime();
        }

        /// <summary>
        /// 将10位时间戳Timestamp转换成日期
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static DateTime ToLocalDateTime(this long target)
        {
            return ToLocalDateTime(int.Parse(target.ToString()));
        }


        /// <summary>
        /// 获取Unix时间（精确到秒）
        /// </summary>
        public static long GetUnixTimeTicks(this long? unixDate)
        {
            //Ticks：此属性的值表示自 0001 年 1 月 1 日凌晨 00:00:00 以来已经过的时间的以 100 纳秒为间隔的间隔数(1毫秒有10000个计时周期)。
            //从 0000年00月00日00：00：00 - 1970年01月01日00：00：00的刻度值(毫秒)1970 × 365 × 24 × 60 × 60 × 1000 × 10000 大概等于 621355968000000000
            //以上表达式的意思是要取得从1970/01/01 00:00:00 到现在经过的毫秒数了
            unixDate ??= (DateTime.Now.Ticks - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).Ticks) / 10000/1000;
            return unixDate.Value;
        }


        /// <summary>
        /// Unix时间转换为正常时间
        /// </summary>
        /// <param name="unixTime">Unix时间 10位</param>
        /// <returns></returns>
        public static DateTime FromUnix(this long unixTime)
        {
            //foreach (var item in TimeZoneInfo.GetSystemTimeZones())
            //{
            //    Log.Logger.Information($"{item.StandardName}_{item.BaseUtcOffset}_{item.DisplayName}");
            //}
            //Log.Logger.Information($"BaseUtcOffset:{TimeZoneInfo.Local.BaseUtcOffset}");
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            DateTime dateTime = startTime.AddSeconds(unixTime).AddMilliseconds(TimeZoneInfo.Local.BaseUtcOffset.TotalMilliseconds);
            return dateTime;
        }

        /// <summary>
        /// Unix时间转换为正常时间
        /// </summary>
        /// <param name="unixTime">Unix时间 13位</param>
        /// <returns></returns>
        public static DateTime FromUnixMillonSeconds(this long unixTime)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            DateTime dateTime = startTime.AddMilliseconds(unixTime).AddMilliseconds(TimeZoneInfo.Local.BaseUtcOffset.TotalMilliseconds);
            return dateTime;
        }


        /// <summary>
        /// 复制堆栈(先进后出)
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="stack">堆栈</param>
        /// <returns></returns>
        public static Stack<T> ReverseStack<T>(this Stack<T> stack)
        {
            var newStack = new Stack<T>();
            while (stack.Any())
            {
                newStack.Push(stack.Pop());
            }
            return newStack;
        }



        /// <summary>  
        /// 该方法用于生成指定位数的随机数  静态方法(非静态扩展)
        /// </summary>  
        /// <param name="codeNum">参数是随机数的位数</param>  
        /// <returns>返回一个随机数字符串</returns>  
        public static string GenerateRandomString(int codeNum)
        {
            //验证码可以显示的字符集合
            var vchar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            var vcArray = vchar.Split(",");//vchar.Split(new char[] { ',' });//拆分成数组   
            var code = "";//产生的随机数  
            var temp = -1;//记录上次随机数值，尽量避避免生产几个一样的随机数  
            var rand = new Random(); //采用一个简单的算法以保证生成随机数的不同  
            for (var i = 1; i < codeNum + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));//初始化随机类  
                }
                var t = rand.Next(62);//获取随机数  
                if (temp == t)
                {
                    //如果获取的随机数重复，则递归调用  
                    return GenerateRandomString(codeNum);
                }

                temp = t;//把本次产生的随机数记录起来  
                code += vcArray[t];//随机数的位数加一  
            }
            return code;
        }

    }
}
