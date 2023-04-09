using System;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// 日期扩展函数
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 格式化时间为字符串格式
        /// </summary>
        public static string TextForDate(this DateTime date, string format = CustomerConsts.DateFormatString)
        {
            return date.ToString(format);
        }

        /// <summary>
        /// 获取该日期中的时间部分 HH:mm:ss.ttt
        /// </summary>
        public static TimeSpan ToTime(this DateTime date)
        {
            return new TimeSpan(0, date.Hour, date.Minute, date.Second, date.Millisecond);
        }

        /// <summary>
        /// 设置开始日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime SetStartDate(this DateTime date)
        {
            return date.Date.AddSeconds(-1);
        }

        /// <summary>
        /// 设置结束日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime? SetEndDate(this DateTime? date)
        {
            return date?.Date.AddDays(1).AddSeconds(-1);
        }

        public static DateTime ToMonthDate(this DateTime time)
        {
            return SkipDhms(time);
            //return new DateTime(time.Year, time.Month, 1);
        }

        public static DateTime ToMonthDate(this DateTime? time)
        {
            return ToMonthDate(time ?? DateTime.MaxValue);
        }

        /// <summary>
        /// 取年月1号
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime SkipDhms(this DateTime time)
        {
            return time.AddDays(1 - time.Day).AddHours(-time.Hour).AddMinutes(-time.Minute).AddSeconds(-time.Second);
        }

        /// <summary>
        /// 取年月和1号
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime SkipDhms(this DateTime? time)
        {
            return SkipDhms(time ?? DateTime.Now);
        }

        /// <summary>
        /// 取年月日，忽略时分秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime SkipHms(this DateTime time)
        {
            return time.AddHours(-time.Hour).AddMinutes(-time.Minute).AddSeconds(-time.Second);
        }


        public static bool MonthEqual(this DateTime time, DateTime compareTime)
        {
            return time.Year == compareTime.Year && time.Month == compareTime.Month;
        }

        /// <summary>
        ///  计算当前时间与目标时间的月份差距
        /// </summary>
        /// <param name="current">当前时间</param>
        /// <param name="target">目标时间</param>
        /// <returns></returns>
        public static int GetMonthFromCurrentToTargetTime(this DateTime current, DateTime target)
        {
            return (current.Year - target.Year) * 12 + current.Month - target.Month + 1;
        }

        /// <summary>
        /// 获取日期间(不含本月)月份实际相差月数
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int GetDeltaMonths(DateTime? start, DateTime? end)
        {
            var deltaMonth = (end?.Year - start?.Year) * 12 + end?.Month - start?.Month;
            return deltaMonth ?? 0;
        }

        /// <summary>
        /// 获取日期间(包含本月)实际相差月数
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int GetDeltaDateCycle(DateTime? start, DateTime? end)
        {
            var deltaMonth = (end?.Year - start?.Year) * 12 + end?.Month - start?.Month + 1;
            return deltaMonth ?? 0;
        }

        /// <summary>
        /// 21世纪到当前过了多少天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long CenturyPastDays(this DateTime time)
        {
            var startdate = new DateTime(2000, 1, 1);
            var totalDays = (time - startdate).TotalDays;
            return (long)totalDays;
        }

        /// <summary>
        /// 返回10位时间戳 Timestamp
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static long ToUnixTimestamp(this DateTime target)
        {
            //Ticks：此属性的值表示自 0001 年 1 月 1 日凌晨 00:00:00 以来已经过的时间的以 100 纳秒为间隔的间隔数(1毫秒有10000个计时周期)。
            //从 0000年00月00日00：00：00 - 1970年01月01日00：00：00的刻度值(毫秒)1970 × 365 × 24 × 60 × 60 × 1000 × 10000 大概等于 621355968000000000
            //以上表达式的意思是要取得从1970/01/01 00:00:00 到现在经过的毫秒数了
            return  (target.Ticks - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).Ticks) / 10000 / 1000;
            //return (target.ToUniversalTime().Ticks - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).ToUniversalTime().Ticks) / 10000 / 1000;
        }

        /// <summary>
        /// 获取Unix时间
        /// </summary>
        /// <returns></returns>
        public static long GetUnixTime()
        {
            return DateTime.Now.ToUnixTimestamp();
        }

        /// <summary>
        /// 返回时间戳 Timestamp (默认10位)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="length"></param>
        public static long ToUnixTimeTicks(this DateTime target, int length = 10)
        {
            //((DateTime.Now.Ticks - 621355968000000000) / 10000000)
            //var t1970 = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).Ticks;
            //Log.Logger.Information($"1970:{t1970}");
            //var ticks = target.Ticks - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).Ticks;

            //这里应该这么理解：0000-1970年 的时间戳 时间过了多少秒数是固定的,没有UTC与Local的区分。
            //而当前程序所在服务器target相对时间来讲，因为跨时区,它就有UTC和Local之分了，咱们这个时区就有8小时的差距。
            //为了兼容其它语言，目标语言都要转为UTC时间  目标语言再减去这个固定秒数，就是UTC标准时间
            var ticks = target.ToUniversalTime().Ticks - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).Ticks;

            var local = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            var utc = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
            Console.WriteLine($"local:{local.Ticks}");
            Console.WriteLine($"........................................................");
            Console.WriteLine($"utc:{utc.Ticks}");
            Console.WriteLine($"local.ToUniversalTime():{local.ToUniversalTime().Ticks}");
            Console.WriteLine($"........................................................");
            Console.WriteLine($"targetLocal:{target.Ticks}");
            Console.WriteLine($"targetUtc:{target.ToUniversalTime().Ticks}");
            Console.WriteLine($"........................................................");
            Console.WriteLine($"targetLocal-1970Local:{target.Ticks-local.Ticks}");
            Console.WriteLine($"targetUtc-1970Utc:{target.ToUniversalTime().Ticks-utc.Ticks}");
            Console.WriteLine($"........................................................");

            Console.WriteLine($"为了满足java等其它调用方 utc-1970(Net里的Local):{ticks}");


            switch (length)
            {
                case 13: return ticks / 10000;//13位时间戳 精确到毫秒
                case 10: return ticks / 10000000;//10位时间戳 精确到秒
                default: return ticks / 10000000;//默认精确到秒
            }
        }

        /// <summary>
        /// 获得13位的时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp(this DateTime time)
        {
            var ts = time.ToUnixInt();
            return ts.ToString();
        }

        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ToUnixInt(this DateTime time)
        {
            //TimeZoneInfo.Local.GetUtcOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            //TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            //TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            var startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            var t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位(毫秒)
            //Ticks：此属性的值表示自 0001 年 1 月 1 日凌晨 00:00:00 以来已经过的时间的以 100 纳秒为间隔的间隔数(1毫秒有10000个计时周期)。
            //从 0000年00月00日00：00：00 - 1970年01月01日00：00：00的刻度值(毫秒)1970 × 365 × 24 × 60 × 60 × 1000 × 10000 大概等于 621355968000000000
            //以上表达式的意思是要取得从1970/01/01 00:00:00 到现在经过的毫秒数了
            return t;
        }
    }
}
