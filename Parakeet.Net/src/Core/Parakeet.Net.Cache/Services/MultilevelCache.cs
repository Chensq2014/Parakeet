using EasyCaching.Core;

namespace Parakeet.Net.Cache
{
    /// <summary>
    /// 多级缓存
    /// </summary>
    public class MultilevelCache<TData>
    {
        /// <summary>
        /// 缓存泛型data
        /// </summary>
        private TData _data;

        /// <summary>
        /// 缓存接口提供器
        /// </summary>
        private readonly IEasyCachingProvider _cachingProvider;

        /// <summary>
        /// 上次更新时间 默认创建时间
        /// </summary>
        public DateTime LastTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 本地一级缓存过期间隔，默认一分钟
        /// </summary>
        public TimeSpan LocalExpireTimeSpan { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Redis二级缓存过期间隔，默认一小时
        /// </summary>
        public TimeSpan RedisExpireTimeSpan { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// 缓存key，建议使用redis的key命名规则，如Device:SerialNo
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// 获取真实数据的委托
        /// </summary>
        public Func<Task<TData>> GetCacheFunc { get; set; }

        /// <summary>
        /// 主时间锁，只有一个进程在控制它
        /// </summary>
        private int _monitorTimeLock;

        /// <summary>
        /// 本地缓存是否过期
        /// </summary>
        public bool IsExpire => DateTime.Now - LastTime > LocalExpireTimeSpan;

        /// <summary>
        /// 是否自动刷新Redis缓存
        /// </summary>
        public bool AutoRefreshRedis { get; }

        /// <summary>
        /// 自动刷新Redis缓存的时间间隔,默认为30分钟刷新一次
        /// </summary>
        public int AutoRefreshRedisInterval { get; }

        /// <summary>
        /// 自动刷新Redis缓存时间间隔Timer
        /// </summary>
        private System.Timers.Timer _autoRefreshRedisTimer;


        /// <summary>
        /// 初始化多级缓存
        /// </summary>
        /// <param name="cachingProvider">缓存提供器</param>
        /// <param name="cacheKey">缓存key</param>
        /// <param name="getCacheFunc">获取缓存真实数据委托</param>
        /// <param name="autoRefreshRedis">是否自动刷新redis缓存</param>
        /// <param name="autoRefreshRedisInterval">默认自动刷新redis缓存时间间隔</param>
        public MultilevelCache(IEasyCachingProvider cachingProvider, string cacheKey, Func<Task<TData>> getCacheFunc, bool autoRefreshRedis = false, int autoRefreshRedisInterval = 1000 * 60 * 30)
        {
            _cachingProvider = cachingProvider;
            CacheKey = cacheKey;
            GetCacheFunc = getCacheFunc;
            AutoRefreshRedisInterval = autoRefreshRedisInterval;
            AutoRefreshRedis = autoRefreshRedis;
            if (autoRefreshRedis)
            {
                var r = new Random();
                //增加抖动值，避免并发访问数据库
                var interval = AutoRefreshRedisInterval + r.Next(1000, 30000);
                _autoRefreshRedisTimer = new System.Timers.Timer(interval);
                _autoRefreshRedisTimer.Elapsed += async (sender, args) =>
                {
                    var data = await getCacheFunc();
                    if (data == null)
                    {
                        await _cachingProvider.RemoveAsync(cacheKey);
                    }
                    else
                    {
                        await _cachingProvider.SetAsync(cacheKey, data, RedisExpireTimeSpan);
                        _data = data;
                        LastTime = DateTime.Now;
                    }
                };
                _autoRefreshRedisTimer.Start();
            }
        }

        /// <summary>
        /// 从一级缓存或二级、三级缓存中获取设备信息和转发信息
        /// </summary>
        /// <returns></returns>
        public async Task<TData> GetCacheData()
        {
            //本地缓存未过期就直接使用本地一级缓存
            if (_data != null && !IsExpire)
            {
                return _data;
            }

            if (_data == null)
            {
                //第一次进入 _data是null
                //读取redis二级缓存,redis里面没有就去委托里面找(委托可以是再一次缓存或者从数据库里直接读取)
                var cacheValue = await _cachingProvider.GetAsync(CacheKey, GetCacheFunc, RedisExpireTimeSpan);
                _data = cacheValue.Value;
                LastTime = DateTime.Now;
            }//_monitorTimeLock如果等于0的话,使用当前线程把它变为1之后，进入块语句,别的线程来了之后发现它为1了自动跳过此语句块
            else if (Interlocked.CompareExchange(ref _monitorTimeLock, 1, 0) == 0)
            {
                try
                {
                    //读取redis二级缓存,redis里面没有就去委托里面找(委托可以是再一次缓存或者从数据库里直接读取)
                    var cacheValue = await _cachingProvider.GetAsync(CacheKey, GetCacheFunc, RedisExpireTimeSpan);
                    _data = cacheValue.Value;
                    LastTime = DateTime.Now;
                }
                finally
                {
                    //恢复_monitorTimeLock初始值0
                    Interlocked.Exchange(ref _monitorTimeLock, 0);
                }
            }

            return _data;
        }
    }
}
