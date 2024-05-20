namespace ConsoleApp.Cache
{
    public enum ObsoluteType
    {
        /// <summary>
        /// 永不过期
        /// </summary>
        Never,
        /// <summary>
        /// 绝对过期
        /// </summary>
        Absolutely,
        /// <summary>
        /// 滑动过期
        /// </summary>
        Relative
    }
}
