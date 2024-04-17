namespace System
{
    internal static class SystemExtension
    {
        private const long TicksFrom0To1970 = 621355968000000000;

        /// <summary>
        /// 返回时间戳 Timestamp (默认10位,eg:1619061289)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="length"></param>
        public static long ToUnixTimeTicks(this DateTime dateTime, int length = 10)
        {
            var ticks = (dateTime.ToUniversalTime().Ticks - TicksFrom0To1970);

            switch (length)
            {
                //10位时间戳(秒)
                case 10: return ticks / 10000000;
                //13位时间戳(毫秒)
                case 13: return ticks / 10000;
                default: return ticks / 10000000;
            }
        }
    }
}
