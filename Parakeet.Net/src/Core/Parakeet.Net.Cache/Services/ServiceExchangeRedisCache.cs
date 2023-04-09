#nullable enable
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Parakeet.Net.Cache.Extensions;
using Parakeet.Net.Extensions;
using Parakeet.Net.Helper;
using Parakeet.Net.Interfaces;
using StackExchange.Redis;
using Volo.Abp.Caching.StackExchangeRedis;

namespace Parakeet.Net.Cache.Services
{
    /// <summary>
    /// ServiceExchangeRedisCache
    /// </summary>
    public class ServiceExchangeRedisCache : AbpRedisCache, IHandlerType
    {
        /// <summary>
        /// ServiceExchange
        /// </summary>
        public string HandlerType => "ServiceExchange";

        private readonly IMultiRedisProvider _multiRedisProvider;
        public ServiceExchangeRedisCache(IOptions<RedisCacheOptions> optionsAccessor,
            IMultiRedisProvider multiRedisProvider) : base(optionsAccessor)
        {
            _multiRedisProvider = multiRedisProvider;
        }

        #region 初始化MutiRedis

        /// <summary>
        /// 初始化redis
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void InitMutiRedis(IServiceProvider serviceProvider)
        {
            var redisconnect = EnvironmentHelper.ExchangeRedisConn;

            //可以添加多个连接 默认情况下只读取单个
            _multiRedisProvider.InitializeAsync(new List<(string ConnectionName, string ConnectionString)>()
            {
                (_multiRedisProvider.DefaultConnectionName,redisconnect)
                //(mutiRedisProvider.DefaultConnectionName,redisconnect),
                //(mutiRedisProvider.DefaultConnectionName,redisconnect),
                //(mutiRedisProvider.DefaultConnectionName,redisconnect),
            });
        }

        #endregion

        #region ChangeDb

        /// <summary>
        /// ChangeDb
        /// </summary>
        /// <param name="dbNum"></param>
        public async Task<IDatabase?> ChangeDb(int dbNum)
        {
            //RedisDatabase.Database 当前redis数据库dbNum
            var database = await GetRedisClientAsync(dbNum);
            return database;
        }
        #endregion

        #region GetRedisClientAsync

        /// <summary>
        /// 获取当前databaseDb
        /// </summary>
        /// <param name="dbNum"></param>
        /// <returns></returns>
        public async Task<IDatabase> GetRedisClientAsync(int? dbNum = 0)
        {
            //todo：扩展枚举返回单独或集群某具体database
            var database = RedisDatabase;
            if (RedisDatabase.Multiplexer is not null)
            {
                database = RedisDatabase.Multiplexer?.GetDatabase(dbNum ?? 0);
            }
            return database ??= await _multiRedisProvider.GetCurrentRedisClientAsync(dbNum ?? 0);
        }

        #endregion

        #region FillKeyAsync

        /// <summary>
        /// 填充真实key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> FillKeyAsync(string key)
        {
            //if (EgmpEnvironment.Instance.DeployModel == EgmpDeployModel.Single)
            //{
            //    return key;
            //}
            //else
            //{
            //    var tenant = _tenantEnvironmentInstanceProvider.GetCurrentTenant();
            //    if (tenant == null)
            //    {
            //        throw new Exception("未获取到租户环境");
            //    }

            //    if (key.StartsWith(tenant.WebSiteUrl.ToString(), StringComparison.OrdinalIgnoreCase))
            //    {
            //        return key;
            //    }

            //    if (string.IsNullOrEmpty(AppVersionNo.VersionNo))
            //    {
            //        return $"{tenant.WebSiteUrl}:{tenant.EnvType}:{key}";
            //    }
            //    else
            //    {
            //        return $"{tenant.WebSiteUrl}:{tenant.EnvType}:{AppVersionNo.VersionNo}:{key}";
            //    }
            //}
            return await Task.FromResult(key);//await _multiRedisProvider.FillKeyAsync(key);
        }


        #endregion

        #region 设置过期时间

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="expire">过期时间</param>
        /// <param name="database">redis客户端database</param>
        /// <returns></returns>
        public async Task<bool> ExpireAsync(string key, TimeSpan expire, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            return await (database ?? RedisDatabase).KeyExpireAsync(key, expire);
            //key = await _multiRedisProvider.FillKeyAsync(key);
            //return await database.KeyExpireAsync(key, expire);
        }

        #endregion
        
        #region redis  api

        #region key

        /// <summary>
        /// 根据patterns查询个数
        /// </summary>
        /// <param name="keyPatterns"></param>
        /// <returns></returns>
        public async Task<long> KeysCount(string keyPatterns)
        {
            long count = 0;
            var database = await GetRedisClientAsync();
            var redisResult = await database.ScriptEvaluateAsync(LuaScript.Prepare(
                //Redis的keys模糊查询：
                " local res = redis.call('KEYS', @keypattern) " +
                " return res "), new { @keypattern = (await _multiRedisProvider.FillKeyAsync(keyPatterns)) + "*" });

            if (!redisResult.IsNull)
            {
                RedisKey[] preSult = (RedisKey[])redisResult;
                if (preSult != null) count = preSult.Length;
            }

            return count;
        }

        /// <summary>
        /// 根据keys删除
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<long> DelAsync(List<string> keys)
        {
            if (keys?.Count > 0)
            {
                //var ks = keys.Select(async key=> await FillKeyAsync(key)).ToList();
                var database = await GetRedisClientAsync();
                var count = await database.KeyDeleteAsync(keys.ToArray().ToRedisKeys());
                return count;
            }

            return 0;
        }

        /// <summary>
        /// 删除缓存
        /// 如果多个key必须在同一个db中
        /// </summary>
        /// <param name="keys"></param>
        /// <param name=""></param>
        /// <returns>是否成功</returns>
        public async Task<long> DelAsync(string keys)
        {
            var database = await GetRedisClientAsync();
            var count = await database.KeyDeleteAsync(keys);
            return count ? 1 : 0;
        }

        /// <summary>
        /// 根据指定模式删除
        /// </summary>
        /// <param name="database"></param>
        /// <param name="patterns">模式（正则表达式）</param>
        /// <returns></returns>
        public async Task<long> DelByPatternAsync(params string[] patterns)
        {
            if (patterns == null || patterns.Length == 0)
            {
                return 0;
            }

            long delCount = 0;
            var database = await GetRedisClientAsync();
            foreach (var pattern in patterns)
            {
                var redisResult = await database.ScriptEvaluateAsync(LuaScript.Prepare(
                    //Redis的keys模糊查询：
                    " local res = redis.call('KEYS', @keypattern) " +
                    " return res "), new { @keypattern = (await FillKeyAsync(pattern)) + "*" });

                if (!redisResult.IsNull)
                {
                    RedisKey[] preSult = (RedisKey[])redisResult;
                    if (preSult != null)
                    {
                        database.KeyDelete(preSult);
                        delCount += preSult.Count();
                    }
                }
            }

            return delCount;
        }

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns>是否成功</returns>
        public async Task<bool> ExistsAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.KeyExistsAsync(key);
        }

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="database">redis客户端database</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string key, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            return await (database ?? RedisDatabase).KeyExistsAsync(key);
        }

        public async Task<bool> ExpireAsync(string key, TimeSpan? expire, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            if (expire.HasValue)
            {
                return await database.KeyExpireAsync(key, expire.Value);
            }

            return await database.KeyPersistAsync(key);
        }

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="seconds">过期秒数</param>
        /// <returns>是否成功</returns>
        public async Task<bool> ExpireAsync(string key, int seconds)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.KeyExpireAsync(key, TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        /// <returns>是否成功</returns>
        public async Task<bool> ExpireAsync(string key, TimeSpan expire)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.KeyExpireAsync(key, expire);
        }


        /// <summary>
        /// 为给定 key 设置过期时间，指定时间过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">指定时间</param>
        /// <returns>是否成功</returns>
        public async Task<bool> ExpireAtAsync(string key, DateTime expire)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.KeyExpireAsync(key, expire);
        }

        ///// <summary>
        ///// 该返回给定 key 锁储存的值所使用的内部表示(representation)
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public async Task<string> ObjectEncodingAsync(string key)
        //{
        //    key = await FillKeyAsync(key);
        //    var database = await GetRedisClientAsync();
        //    return await database.KeyEncodingAsync(key);//当前StackExchange.Redis版本需要更新到2.6.86
        //}

        ///// <summary>
        ///// 该返回给定 key 引用所储存的值的次数。此命令主要用于除错
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public async Task<long?> ObjectRefCountAsync(string key)
        //{
        //    key = await FillKeyAsync(key);
        //    var database = await GetRedisClientAsync();
        //    return await database.KeyRefCountAsync(key);//当前StackExchange.Redis版本需要更新到2.6.86
        //}

        /// <summary>
        /// 返回给定 key 自储存以来的空转时间(idle， 没有被读取也没有被写入)，以秒为单位
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long?> ObjectIdleTimeAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var timespan = await database.KeyIdleTimeAsync(key);
            return timespan?.Seconds;
        }

        /// <summary>
        /// 移除 key 的过期时间，key 将持久保持
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> PersistAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.KeyPersistAsync(key);
        }

        /// <summary>
        /// 以秒为单位，返回给定 key 的剩余生存时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> TtlAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var timespan = await database.KeyTimeToLiveAsync(key);
            return (long)(timespan?.TotalSeconds ?? 0);
        }

        public async Task<long> TtlAsync(string key, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            var timespan = await database.KeyTimeToLiveAsync(key);
            return timespan?.Seconds ?? -1;
        }

        /// <summary>
        /// 以毫秒为单位返回 key 的剩余的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> PTtlAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var timespan = await database.KeyTimeToLiveAsync(key);
            return timespan?.Milliseconds ?? 0;
        }

        /// <summary>
        /// 修改 key 的名称,newKey存在时覆盖
        /// </summary>
        /// <param name="key">旧名称</param>
        /// <param name="newKey">新名称</param>
        /// <returns></returns>
        public async Task<bool> RenameAsync(string key, string newKey)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            newKey = await FillKeyAsync(newKey);
            return await database.KeyExistsAsync(key) && !key.Equals(newKey) &&
                   await database.KeyRenameAsync(key, newKey, when: When.Exists);
        }

        /// <summary>
        /// 修改 key 的名称，newKey存在时，返回失败
        /// </summary>
        /// <param name="key">旧名称，</param>
        /// <param name="newKey">新名称，</param>
        /// <returns></returns>
        public async Task<bool> RenameNxAsync(string key, string newKey)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            newKey = await FillKeyAsync(newKey);
            return await database.KeyRenameAsync(key, newKey, when: When.NotExists);
        }

        #endregion

        #region String

        /// <summary>
        /// 如果 key 已经存在并且是一个字符串， APPEND 命令将指定的 value 追加到该 key 原来值（value）的末尾
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">字符串</param>
        /// <returns>追加指定值之后， key 中字符串的长度</returns>
        public async Task<long> AppendAsync(string key, string value)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.StringAppendAsync(key, value);
        }

        /// <summary>
        /// 获取指定 key 的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.StringGetAsync(key);
        }

        /// <summary>
        /// 获取指定 key 的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            return await database.StringGetAsync(key);
        }

        /// <summary>
        /// 获取指定 key 的值
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>    
        public async Task<T> GetAsync<T>(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var v = await database.StringGetAsync(key);
            return v.ToString().ToObjectValue<T>();
        }

        /// <summary>
        /// 获取指定 key 的值
        /// </summary>
        /// <param name="database"></param>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            var v = await database.StringGetAsync(key);
            return v.ToString().ToObjectValue<T>();
        }

        /// <summary>
        /// 返回 key 中字符串值的子字符
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="end">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<string> GetRangeAsync(string key, long start, long end)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.StringGetRangeAsync(key, start, end);
        }

        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<string> GetSetAsync(string key, object value)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.StringGetSetAsync(key, value.ToRedisValue());
        }

        public async Task<string> GetSetAsync(string key, object value, TimeSpan expireTimeSpan)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var oldvalue = database.StringGet(key);
            database.StringSet(key, value.ToRedisValue(), expireTimeSpan);
            return oldvalue;
        }

        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<T> GetSetAsync<T>(string key, object value)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var v = await database.StringGetSetAsync(key, value.ToRedisValue());
            return v.ToString().ToObjectValue<T>();
        }

        public async Task<T> GetSetAsync<T>(string key, object value, TimeSpan expireTimeSpan)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var oldvalue = database.StringGet(key);
            database.StringSet(key, value.ToRedisValue(), expireTimeSpan);
            return oldvalue.ToString().ToObjectValue<T>();
        }

        /// <summary>
        /// 获取多个指定 key 的值(数组)，必须在同一个db中
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<string[]> MGetAsync(string[] keys)
        {
            var ks = new List<string>(keys.Length);
            foreach (var item in keys)
            {
                ks.Add(await FillKeyAsync(item));
            }

            var database = await GetRedisClientAsync();
            return (await database.StringGetAsync(ks.ToArray().ToRedisKeys()))
                .Select(x => x.ToString())
                .ToArray();
        }

        /// <summary>
        /// 获取多个指定 key 的值(数组)，必须在同一个db中
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<T[]> MGetAsync<T>(string[] keys)
        {
            var ks = new List<string>(keys.Length);
            foreach (var item in keys)
            {
                ks.Add(await FillKeyAsync(item));
            }

            var database = await GetRedisClientAsync();
            return (await database.StringGetAsync(ks.ToArray().ToRedisKeys()))
                .Select(x => x.ToString().FromJsonString<T>())
                .ToArray();
        }

        /// <summary>
        /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <param name="expireSeconds">过期(秒单位)</param>
        /// <param name="exists">Nx, Xx</param>
        /// <param name="isToTenant"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync(string key, object value, int expireSeconds = -1,
            When exists = When.Always)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.StringSetAsync(key, value.ToRedisValue(),
                expireSeconds == -1 ? null : TimeSpan.FromSeconds(expireSeconds), exists);
        }

        /// <summary>
        ///  设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="database"></param>
        /// <param name="expireSeconds"></param>
        /// <param name="exists"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync(string key, object value, IDatabase? database = null, int expireSeconds = -1,
            When exists = When.Always)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            return await database.StringSetAsync(key, value.ToRedisValue(),
                expireSeconds == -1 ? null : TimeSpan.FromSeconds(expireSeconds), exists);
        }

        /// <summary>
        /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        /// <param name="exists">Nx, Xx</param>
        /// <returns></returns>
        public async Task<bool> SetAsync(string key, object value, TimeSpan expire, When exists = When.Always)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.StringSetAsync(key, value.ToRedisValue(), expire, exists);
        }

        /// <summary>
        /// 返回 key 所储存的字符串值的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> StrLenAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.StringLengthAsync(key);
        }

        /// <summary>
        /// 只有在 key 不存在时设置 key 的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SetNxAsync(string key, object value)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.StringSetAsync(key, value.ToRedisValue(), null, When.NotExists);
        }

        #endregion

        #region Hash

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        public async Task<long> HDelAsync(string key, params string[] fields)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.HashDeleteAsync(key, fields.ToRedisValueArray());
        }

        /// <summary>
        /// 查看哈希表 key 中，指定的字段是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public async Task<bool> HExistsAsync(string key, string field)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.HashExistsAsync(key, field);
        }

        /// <summary>
        /// 获取存储在哈希表中指定字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public async Task<string> HGetAsync(string key, string field)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.HashGetAsync(key, field);
        }

        /// <summary>
        /// 获取存储在哈希表中指定字段的值
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public async Task<T> HGetAsync<T>(string key, string field)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var value = await database.HashGetAsync(key, field);
            return value.ToString().ToObjectValue<T>();
        }

        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> HGetAllAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = await database.HashGetAllAsync(key);
            var dic = new Dictionary<string, string>(res.Length);
            foreach (var item in res)
            {
                dic.Add(item.Name.ToString(), item.Value);
            }

            return dic;
        }

        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, T>> HGetAllAsync<T>(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = await database.HashGetAllAsync(key);
            var dic = new Dictionary<string, T>(res.Length);
            foreach (var item in res)
            {
                try
                {
                    dic.Add(item.Name.ToString(), item.Value.ToString().FromJsonString<T>());
                }
                catch (Exception ex)
                {
                    //Log
                }
            }

            return dic;
        }

        /// <summary>
        /// 获取所有哈希表中的字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> HKeysAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.HashKeysAsync(key)).Select(x => x.ToString());
        }

        /// <summary>
        /// 获取哈希表中字段的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> HLenAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.HashLengthAsync(key);
        }

        /// <summary>
        /// 获取存储在哈希表中多个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        public async Task<string[]> HMGetAsync(string key, string[] fields)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.HashGetAsync(key, fields.ToArray().Select(x => new RedisValue(x)).ToArray()))
                .ToStringArray();
        }

        /// <summary>
        /// 获取存储在哈希表中多个字段的值
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="fields">一个或多个字段</param>
        /// <returns></returns>
        public async Task<T[]> HMGetAsync<T>(string key, string[] fields)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = await database.HashGetAsync(key, fields.Select(x => new RedisValue(x)).ToArray());
            return res.Select(x => x.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fieldValues">一个或多个 field value 值</param>
        /// <returns></returns>
        public async Task HMSetAsync(string key, (string field, object value)[] fieldValues)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            await database.HashSetAsync(key,
                fieldValues.Select(t => new HashEntry(t.field, t.value.ToRedisValue())).ToArray());
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fieldValues"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public async Task HMSetAsync(string key, Dictionary<string, string> fieldValues, TimeSpan? expireTime = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            await database.HashSetAsync(key,
                fieldValues.Select(t => new HashEntry(t.Key, t.Value.ToRedisValue())).ToArray());
            if (expireTime != null)
            {
                await database.KeyExpireAsync(key, expireTime);
            }
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <param name="database">Redis客户端</param>
        /// <param name="key"></param>
        /// <param name="fieldValues">一个或多个 field value 值</param>
        /// <returns></returns>
        public async Task HMSetAsync(IDatabase database, string key,
            params (string Field, object Value)[] fieldValues)
        {
            key = await FillKeyAsync(key);
            //database ??= await GetRedisClientAsync();
            await database.HashSetAsync(key,
                fieldValues.Select(x => new HashEntry(name: x.Field, value: x.Value.ToRedisValue()))
                    .ToArray());
        }

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns>如果字段是哈希表中的一个新建字段，并且值设置成功，返回true。如果哈希表中域字段已经存在且旧值已被新值覆盖，返回false。</returns>
        public async Task<bool> HSetAsync(string key, string field, object value, TimeSpan? expireTime = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = await database.HashSetAsync(key, field, value.ToRedisValue());
            if (expireTime != null)
            {
                await database.KeyExpireAsync(key, expireTime);
            }

            return res;
        }

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns>如果字段是哈希表中的一个新建字段，并且值设置成功，返回true。如果哈希表中域字段已经存在且旧值已被新值覆盖，返回false。</returns>
        public async Task<bool> HSetAsync(string key, string field, object value, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            return await database.HashSetAsync(key, field, value.ToRedisValue());
        }

        /// <summary>
        /// 迭代哈希表中的键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public async IAsyncEnumerable<(string field, string value)> HScanAsync(string key, int cursor,
            string pattern = null, int? count = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = database.HashScanAsync(key, pattern, pageSize: count ?? 250, cursor: cursor);
            await foreach (var x in res)
            {
                (string field, string value) valueTuple;
                valueTuple.field = x.Name;
                valueTuple.value = x.Value;
                yield return valueTuple;
            }
        }

        /// <summary>
        /// 迭代哈希表中的键值对
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public async IAsyncEnumerable<(string field, T value)> HScanAsync<T>(string key, int cursor,
            string pattern = null, int? count = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = database.HashScanAsync(key, pattern, pageSize: count ?? 250, cursor: cursor);
            await foreach (var x in res)
            {
                (string field, T value) valueTuple;
                valueTuple.field = x.Name;
                valueTuple.value = x.Value.ToString().FromJsonString<T>();
                yield return valueTuple;
            }
        }

        #endregion

        #region List

        /// <summary>
        /// 获取列表指定范围内的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<string[]> LRangeAsync(string key, long start, long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.ListRangeAsync(key, start, stop)).Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        /// 获取列表指定范围内的元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<T[]> LRangeAsync<T>(string key, long start, long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = (await database.ListRangeAsync(key, start, stop)).Select(x => x.ToString()).ToArray();
            return res.ToValues<T>();
        }

        /// <summary>
        /// 获取列表长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> LLenAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.ListLengthAsync(key);
        }

        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public async Task<string> LIndexAsync(string key, long index)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.ListGetByIndexAsync(key, index);
        }

        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public async Task<T> LIndexAsync<T>(string key, long index)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var v = await database.ListGetByIndexAsync(key, index);
            return v.ToString().ToObjectValue<T>();
        }

        /// <summary>
        /// 根据参数 count 的值，移除列表中与参数 value 相等的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count">移除的数量，大于0时从表头删除数量count，小于0时从表尾删除数量-count，等于0移除所有</param>
        /// <param name="value">元素</param>
        /// <returns></returns>
        public async Task<long> LRemAsync(string key, long count, object value)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.ListRemoveAsync(key, value.ToRedisValue(), count);
        }

        /// <summary>
        /// 通过索引设置列表元素的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task LSetAsync(string key, long index, object value)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            await database.ListSetByIndexAsync(key, index, value.ToRedisValue());
        }

        /// <summary>
        /// 对一个列表进行修剪，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task LTrimAsync(string key, long start, long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            await database.ListTrimAsync(key, start, stop);
        }

        /// <summary>
        /// 移出并获取列表的第一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> LPopAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.ListLeftPopAsync(key);
        }

        /// <summary>
        /// 移出并获取列表的第一个元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> LPopAsync<T>(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = await database.ListLeftPopAsync(key);
            return res.ToString().FromJsonString<T>();
        }

        /// <summary>
        /// 移除并获取列表最后一个元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> RPopAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.ListRightPopAsync(key);
        }

        /// <summary>
        /// 移除并获取列表最后一个元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> RPopAsync<T>(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = await database.ListRightPopAsync(key);
            return res.ToString().FromJsonString<T>();
        }


        /// <summary>
        /// 在列表中添加一个或多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">一个或多个值</param>
        /// <returns>执行 RPUSH 命令后，列表的长度</returns>
        public async Task<long> RPushAsync<T>(string key, params T[] value)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.ListRightPushAsync(key,
                value.ToRedisValues());
        }

        /// <summary>
        /// 为已存在的列表添加值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">一个或多个值</param>
        /// <returns>执行 RPUSHX 命令后，列表的长度</returns>
        public async Task<long> RPushXAsync(string key, object value)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.ListRightPushAsync(key, value.ToRedisValue(), When.Exists);
        }

        /// <summary>
        /// 将一个或多个值插入到列表头部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">一个或多个值</param>
        /// <returns>执行 LPUSH 命令后，列表的长度</returns>
        public async Task<long> LPushAsync<T>(string key, T[] value)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.ListLeftPushAsync(key,
                value.ToRedisValues());
        }

        /// <summary>
        /// 将一个值插入到已存在的列表头部
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <returns>执行 LPUSHX 命令后，列表的长度。</returns>
        public async Task<long> LPushXAsync(string key, object value)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.ListLeftPushAsync(key, value.ToRedisValue(), When.Exists);
        }

        #endregion

        #region Set

        /// <summary>
        /// 向集合添加一个或多个成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="members">一个或多个成员</param>
        /// <returns></returns>
        public async Task<long> SAddAsync<T>(string key, T[] members, TimeSpan? expireTime = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var result = await database.SetAddAsync(key, members.ToRedisValues());
            // 过期时间
            if (expireTime != null)
            {
                await database.KeyExpireAsync(key, expireTime);
            }

            return result;
        }

        /// <summary>
        /// 获取集合的成员数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SCardAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SetLengthAsync(key);
        }

        /// <summary>
        /// 返回给定所有集合的差集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<string[]> SDiffAsync(string[] keys)
        {
            var ks = new List<string>(keys.Length);
            foreach (var item in keys)
            {
                ks.Add(await FillKeyAsync(item));
            }
            var database = await GetRedisClientAsync();
            return (await database.SetCombineAsync(SetOperation.Difference, ks.ToArray().ToRedisKeys()))
                .Select(y => y.ToString()).ToArray();
        }

        /// <summary>
        /// 返回给定所有集合的差集
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<T[]> SDiffAsync<T>(string[] keys)
        {
            var ks = new List<string>(keys.Length);
            foreach (var item in keys)
            {
                ks.Add(await FillKeyAsync(item));
            }
            var database = await GetRedisClientAsync();
            return (await database.SetCombineAsync(SetOperation.Difference, keys.ToArray().ToRedisKeys()))
                .Select(x => x.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 返回给定所有集合的交集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<string[]> SInterAsync(string[] keys)
        {
            var ks = new List<string>(keys.Length);
            foreach (var item in keys)
            {
                ks.Add(await FillKeyAsync(item));
            }
            var database = await GetRedisClientAsync();
            return (await database.SetCombineAsync(SetOperation.Intersect, ks.ToArray().ToRedisKeys()))
                .Select(y => y.ToString()).ToArray();
        }

        /// <summary>
        /// 返回给定所有集合的交集
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<T[]> SInterAsync<T>(string[] keys)
        {
            var ks = new List<string>(keys.Length);
            foreach (var item in keys)
            {
                ks.Add(await FillKeyAsync(item));
            }
            var database = await GetRedisClientAsync();
            return (await database.SetCombineAsync(SetOperation.Intersect, ks.ToArray().ToRedisKeys()))
                .Select(x => x.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 判断 member 元素是否是集合 key 的成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public async Task<bool> SIsMemberAsync(string key, object member)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SetContainsAsync(key, member.ToRedisValue());
        }

        /// <summary>
        /// 返回集合中的所有成员
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string[]> SMembersAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SetMembersAsync(key)).Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        /// 返回集合中的所有成员
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T[]> SMembersAsync<T>(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SetMembersAsync(key)).Select(x => x.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 移除并返回集合中的一个随机元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> SPopAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SetPopAsync(key);
        }

        /// <summary>
        /// 移除并返回集合中的一个随机元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> SPopAsync<T>(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SetPopAsync(key)).ToString().ToObjectValue<T>();
        }

        /// <summary>
        /// 返回集合中的一个随机元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> SRandMemberAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SetRandomMemberAsync(key);
        }

        /// <summary>
        /// 返回集合中的一个随机元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> SRandMemberAsync<T>(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SetRandomMemberAsync(key)).ToString().ToObjectValue<T>();
        }

        /// <summary>
        /// 返回集合中一个或多个随机数的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count">返回个数</param>
        /// <returns></returns>
        public async Task<string[]> SRandMembersAsync(string key, int count = 1)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SetRandomMembersAsync(key, count)).Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        /// 返回集合中一个或多个随机数的元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="count">返回个数</param>
        /// <returns></returns>
        public async Task<T[]> SRandMembersAsync<T>(string key, int count = 1)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SetRandomMembersAsync(key, count)).Select(x => x.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 移除集合中一个或多个成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="members">一个或多个成员</param>
        /// <returns></returns>
        public async Task<long> SRemAsync<T>(string key, T[] members)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SetRemoveAsync(key,
                members.Select(x => x.ToRedisValue()).ToArray());
        }

        /// <summary>
        /// 返回所有给定集合的并集
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<string[]> SUnionAsync(string[] keys)
        {
            var ks = new List<string>(keys.Length);
            foreach (var item in keys)
            {
                ks.Add(await _multiRedisProvider.FillKeyAsync(item));
            }
            var database = await GetRedisClientAsync();
            return (await database.SetCombineAsync(SetOperation.Union, ks.ToArray().ToRedisKeys()))
                .Select(y => y.ToString()).ToArray();
        }

        /// <summary>
        /// 返回所有给定集合的并集
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<T[]> SUnionAsync<T>(string[] keys)
        {
            var ks = new List<string>(keys.Length);
            foreach (var item in keys)
            {
                ks.Add(await _multiRedisProvider.FillKeyAsync(item));
            }
            var database = await GetRedisClientAsync();
            return (await database.SetCombineAsync(SetOperation.Union, ks.ToArray().ToRedisKeys()))
                .Select(y => y.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 迭代集合中的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public async IAsyncEnumerable<string> SScanAsync(string key, int cursor, string pattern = null,
            int? count = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = database.SetScanAsync(key, pattern, pageSize: count ?? 250, cursor: cursor);
            await foreach (var item in res)
            {
                yield return item.ToString();
            }
        }

        /// <summary>
        /// 迭代集合中的元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public async IAsyncEnumerable<T> SScanAsync<T>(string key, int cursor, string pattern = null,
            int? count = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = database.SetScanAsync(key, pattern, pageSize: count ?? 250, cursor: cursor);
            await foreach (var item in res)
            {
                yield return item.ToString().ToObjectValue<T>();
            }
        }

        /// <summary>
        /// 集合添加一个或多个成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="scoreMembers"></param>
        /// <returns></returns>
        public async Task<long> ZAddAsync(string key, (decimal score, object member)[] scoreMembers)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            SortedSetEntry[] sortedSetEntries = scoreMembers.Select(x =>
                new SortedSetEntry(score: (double)x.score, element: new RedisValue(x.member.ToRedisValue()))).ToArray();
            return await database.SortedSetAddAsync(key, sortedSetEntries);
        }


        #endregion

        #region Sorted Set

        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="scoreMembers">一个或多个成员分数</param>
        /// <returns></returns>
        public async Task<long> ZAddAsync(string key, (double Score, object Member)[] scoreMembers)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SortedSetAddAsync(key, scoreMembers.Select(x =>
                    new SortedSetEntry(score: (double)x.Score, element: x.Member.ToRedisValue()))
                .ToArray());
        }

        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        /// <param name="database">Redis客户端</param>
        /// <param name="key"></param>
        /// <param name="scoreMembers">一个或多个成员分数</param>
        /// <returns></returns>
        public async Task<long> ZAddAsync(IDatabase database, string key,
            params (double Score, object Member)[] scoreMembers)
        {
            key = await FillKeyAsync(key);
            //database ??= await GetRedisClientAsync();
            return await database.SortedSetAddAsync(key,
                scoreMembers.Select(x => new SortedSetEntry(score: (double)x.Score, element: x.Member.ToRedisValue()))
                    .ToArray());
        }

        /// <summary>
        /// 获取有序集合的成员数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> ZCardAsync(string key)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SortedSetLengthAsync(key);
        }

        /// <summary>
        /// 计算在有序集合中指定区间分数的成员数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <returns></returns>
        public async Task<long> ZCountAsync(string key, decimal min, decimal max)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByScoreAsync(key, (double)min, (double)max)).Length;
        }

        /// <summary>
        /// 计算在有序集合中指定区间分数的成员数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <returns></returns>
        public async Task<long> ZCountAsync(string key, string min, string max)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByValueAsync(key, new RedisValue(min), new RedisValue(max))).Length;
        }

        /// <summary>
        /// 有序集合中对指定成员的分数加上增量 increment
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">成员</param>
        /// <param name="increment">增量值(默认=1)</param>
        /// <returns></returns>
        public async Task<decimal> ZIncrByAsync(string key, string member, decimal increment = 1)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (decimal)(await database.SortedSetIncrementAsync(key, member, (double)increment));
        }

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<string[]> ZRangeAsync(string key, long start, long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByRankAsync(key, start, stop)).Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<T[]> ZRangeAsync<T>(string key, long start, long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByRankAsync(key, start, stop)).Select(x => x.ToString())
                .ToValues<T>();
        }

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员和分数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<(string member, double score)[]> ZRangeWithScoresAsync(string key, long start,
            long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = await database.SortedSetRangeByRankWithScoresAsync(key, start, stop);
            return res.Select(x =>
            {
                (string member, double score) v;
                v.member = x.Element.ToString();
                v.score = x.Score;
                return v;
            }).ToArray();
        }

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员和分数
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<(T member, decimal score)[]> ZRangeWithScoresAsync<T>(string key, long start,
            long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = await database.SortedSetRangeByRankWithScoresAsync(key, start, stop);
            return res.Select(x =>
            {
                (T member, decimal score) v;
                v.member = x.Element.ToString().ToObjectValue<T>();
                v.score = (decimal)x.Score;
                return v;
            }).ToArray();
        }

        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<string[]> ZRangeByScoreAsync(string key, decimal min, decimal max, long? limit = null,
            long offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByScoreAsync(key, (double)min, (double)max, exclude: Exclude.None,
                Order.Ascending,
                offset, limit ?? 1)).Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<T[]> ZRangeByScoreAsync<T>(string key, decimal min, decimal max, long? limit = null,
            long offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByScoreAsync(key, (double)min, (double)max, exclude: Exclude.None,
                Order.Ascending,
                offset, limit ?? 1)).Select(x => x.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<string[]> ZRangeByScoreAsync(string key, string min, string max, long? limit = null,
            long offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByValueAsync(key, min, max, Exclude.None, Order.Ascending, offset,
                    limit ?? 0))
                .Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<T[]> ZRangeByScoreAsync<T>(string key, string min, string max, long? limit = null,
            long offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByValueAsync(key, min, max, Exclude.None, Order.Ascending, offset,
                    limit ?? 0))
                .Select(x => x.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员和分数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<(string member, decimal score)[]> ZRangeByScoreWithScoresAsync(string key, decimal min,
            decimal max, long? limit = null, long offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByScoreWithScoresAsync(key, (double)min, (double)max,
                Exclude.None, Order.Ascending, offset, limit ?? 0)).Select(x =>
                {
                    (string member, decimal score) v;
                    v.score = (decimal)x.Score;
                    v.member = x.Element;
                    return v;
                }).ToArray();
        }

        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员和分数
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<(T member, decimal score)[]> ZRangeByScoreWithScoresAsync<T>(string key, decimal min,
            decimal max, long? limit = null, long offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByScoreWithScoresAsync(key, (double)min, (double)max,
                Exclude.None, Order.Ascending, offset, limit ?? 0)).Select(x =>
                {
                    (T member, decimal score) v;
                    v.score = (decimal)x.Score;
                    v.member = x.Element.ToString().ToObjectValue<T>();
                    return v;
                }).ToArray();
        }

        ///// <summary>
        ///// 通过分数返回有序集合指定区间内的成员和分数
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="min">分数最小值 -inf (1 1</param>
        ///// <param name="max">分数最大值 +inf (10 10</param>
        ///// <param name="limit">返回多少成员</param>
        ///// <param name="offset">返回条件偏移位置</param>
        ///// <returns></returns>
        //public async Task<(string member, decimal score)[]> ZRangeByScoreWithScoresAsync(string key, string min, string max, long? limit = null, long offset = 0)
        //{
        //    key = await FillKeyAsync(key);
        //    var database = await GetRedisClientAsync();
        //    return await database.ZRangeByScoreWithScoresAsync(key, min, max, limit, offset);
        //}

        ///// <summary>
        ///// 通过分数返回有序集合指定区间内的成员和分数
        ///// </summary>
        ///// <typeparam name="T">byte[] 或其他类型</typeparam>
        ///// <param name="key"></param>
        ///// <param name="min">分数最小值 -inf (1 1</param>
        ///// <param name="max">分数最大值 +inf (10 10</param>
        ///// <param name="limit">返回多少成员</param>
        ///// <param name="offset">返回条件偏移位置</param>
        ///// <returns></returns>
        //public async Task<(T member, decimal score)[]> ZRangeByScoreWithScoresAsync<T>(string key, string min, string max, long? limit = null, long offset = 0)
        //{
        //    key = await FillKeyAsync(key);
        //    var database = await GetRedisClientAsync();
        //    return await database.ZRangeByScoreWithScoresAsync<T>(key, min, max, limit, offset);
        //}

        /// <summary>
        /// 返回有序集合中指定成员的索引
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public async Task<long?> ZRankAsync(string key, object member)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SortedSetRankAsync(key, member.ToRedisValue());
        }

        /// <summary>
        /// 移除有序集合中的一个或多个成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">一个或多个成员</param>
        /// <returns></returns>
        public async Task<long> ZRemAsync<T>(string key, T[] member)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SortedSetRemoveAsync(key, member.ToRedisValues());
        }

        /// <summary>
        /// 移除有序集合中给定的排名区间的所有成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<long> ZRemRangeByRankAsync(string key, long start, long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SortedSetRemoveRangeByRankAsync(key, start, stop);
        }

        /// <summary>
        /// 移除有序集合中给定的分数区间的所有成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <returns></returns>
        public async Task<long> ZRemRangeByScoreAsync(string key, double min, double max)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SortedSetRemoveRangeByScoreAsync(key, (double)min, (double)max);
        }

        /// <summary>
        /// 移除有序集合中给定的分数区间的所有成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <returns></returns>
        public async Task<long> ZRemRangeByScoreAsync(string key, string min, string max)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SortedSetRemoveRangeByValueAsync(key, min, max);
        }

        /// <summary>
        /// 返回有序集中指定区间内的成员，通过索引，分数从高到底
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<string[]> ZRevRangeAsync(string key, long start, long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByRankAsync(key, start, stop, Order.Descending))
                .Select(x => x.ToString())
                .ToArray();
        }

        /// <summary>
        /// 返回有序集中指定区间内的成员，通过索引，分数从高到底
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<T[]> ZRevRangeAsync<T>(string key, long start, long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByRankAsync(key, start, stop, Order.Descending))
                .Select(x => x.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 返回有序集中指定区间内的成员和分数，通过索引，分数从高到底
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<(string member, double score)[]> ZRevRangeWithScoresAsync(string key, long start,
            long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByRankWithScoresAsync(key, start, stop, Order.Descending)).Select(
                x =>
                {
                    (string member, double score) v;
                    v.member = x.Element.ToString();
                    v.score = x.Score;
                    return v;
                }).ToArray();
        }

        /// <summary>
        /// 返回有序集中指定区间内的成员和分数，通过索引，分数从高到底
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<(T member, double score)[]> ZRevRangeWithScoresAsync<T>(string key, long start,
            long stop)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByRankWithScoresAsync(key, start, stop, Order.Descending)).Select(
                x =>
                {
                    (T member, double score) v;
                    v.member = x.Element.ToString().ToObjectValue<T>();
                    v.score = x.Score;
                    return v;
                }).ToArray();
        }

        /// <summary>
        /// 返回有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <param name="key"></param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<string[]> ZRevRangeByScoreAsync(string key, decimal max, decimal min,
            long? limit = null, long? offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByScoreAsync(key, (double)min, (double)max, Exclude.None,
                    Order.Descending,
                    offset ?? 0, limit ?? -1))
                .Select(x => x.ToString()).ToArray()
                ;
        }

        /// <summary>
        /// 返回有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<T[]> ZRevRangeByScoreAsync<T>(string key, decimal max, decimal min, long? limit = null,
            long offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByScoreAsync(key, (double)min, (double)max, Exclude.None,
                    Order.Descending,
                    0, limit ?? -1))
                .Select(x => x.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 返回有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <param name="key"></param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<string[]> ZRevRangeByScoreAsync(string key, string max, string min, long? limit = null,
            long? offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByValueAsync(key, min, max, Exclude.None, Order.Descending,
                    0, limit ?? -1))
                .Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        /// 返回有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<T[]> ZRevRangeByScoreAsync<T>(string key, string max, string min, long? limit = null,
            long offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByValueAsync(key, min, max, Exclude.None, Order.Descending,
                    0, limit ?? -1))
                .Select(x => x.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 返回有序集中指定分数区间内的成员和分数，分数从高到低排序
        /// </summary>
        /// <param name="key"></param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<(string member, decimal score)[]> ZRevRangeByScoreWithScoresAsync(string key,
            decimal max, decimal min, long? limit = null, long offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByScoreWithScoresAsync(key, (double)min, (double)max, Exclude.None,
                    Order.Descending,
                    0, limit ?? -1))
                .Select(x =>
                {
                    (string member, decimal score) v;
                    v.member = x.Element.ToString();
                    v.score = (decimal)x.Score;
                    return v;
                }).ToArray();
        }

        /// <summary>
        /// 返回有序集中指定分数区间内的成员和分数，分数从高到低排序
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        public async Task<(T member, decimal score)[]> ZRevRangeByScoreWithScoresAsync<T>(string key,
            decimal max, decimal min, long? limit = null, long offset = 0)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.SortedSetRangeByScoreWithScoresAsync(key, (double)min, (double)max, Exclude.None,
                    Order.Descending,
                    0, limit ?? -1))
                .Select(x =>
                {
                    (T member, decimal score) v;
                    v.member = x.Element.ToString().ToObjectValue<T>();
                    v.score = (decimal)x.Score;
                    return v;
                }).ToArray();
        }

        ///// <summary>
        ///// 返回有序集中指定分数区间内的成员和分数，分数从高到低排序
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="max">分数最大值 +inf (10 10</param>
        ///// <param name="min">分数最小值 -inf (1 1</param>
        ///// <param name="limit">返回多少成员</param>
        ///// <param name="offset">返回条件偏移位置</param>
        ///// <returns></returns>
        //public async Task<(string member, decimal score)[]> ZRevRangeByScoreWithScoresAsync(string key, string max, string min, long? limit = null, long offset = 0)
        //{
        //    key = await FillKeyAsync(key);
        //    var database = await GetRedisClientAsync();
        //    return await database.ZRevRangeByScoreWithScoresAsync(key, max, min, limit, offset);
        //}

        ///// <summary>
        ///// 返回有序集中指定分数区间内的成员和分数，分数从高到低排序
        ///// </summary>
        ///// <typeparam name="T">byte[] 或其他类型</typeparam>
        ///// <param name="key"></param>
        ///// <param name="max">分数最大值 +inf (10 10</param>
        ///// <param name="min">分数最小值 -inf (1 1</param>
        ///// <param name="limit">返回多少成员</param>
        ///// <param name="offset">返回条件偏移位置</param>
        ///// <returns></returns>
        //public async Task<(T member, decimal score)[]> ZRevRangeByScoreWithScoresAsync<T>(string key, string max, string min, long? limit = null, long offset = 0)
        //{
        //    key = await FillKeyAsync(key);
        //    var database = await GetRedisClientAsync();
        //    return await database.ZRevRangeByScoreWithScoresAsync<T>(key, max, min, limit, offset);
        //}

        /// <summary>
        /// 返回有序集合中指定成员的排名，有序集成员按分数值递减(从大到小)排序
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public async Task<long?> ZRevRankAsync(string key, object member)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SortedSetRankAsync(key, member.ToRedisValue(), Order.Descending);
        }

        /// <summary>
        /// 返回有序集中，成员的分数值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public async Task<decimal?> ZScoreAsync(string key, object member)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (decimal)(await database.SortedSetScoreAsync(key, member.ToRedisValue()) ?? 0);
        }

        /// <summary>
        /// 迭代有序集合中的元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public async IAsyncEnumerable<(string member, decimal score)> ZScanAsync(string key, int cursor,
            string pattern = null, int? count = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = database.SortedSetScanAsync(key, pattern, pageSize: count ?? 250, cursor);
            await foreach (var item in res)
            {
                (string member, decimal score) v;
                v.member = item.Element.ToString();
                v.score = (decimal)item.Score;
                yield return v;
            }
        }

        /// <summary>
        /// 迭代有序集合中的元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key"></param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public async IAsyncEnumerable<(T member, decimal score)> ZScanAsync<T>(string key, int cursor,
            string pattern = null, int? count = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            var res = database.SortedSetScanAsync(key, pattern, pageSize: count ?? 250, cursor);
            await foreach (var item in res)
            {
                (T member, decimal score) v;
                v.member = item.Element.ToString().ToObjectValue<T>();
                v.score = (decimal)item.Score;
                yield return v;
            }
        }

        ///// <summary>
        ///// 当有序集合的所有成员都具有相同的分值时，有序集合的元素会根据成员的字典序来进行排序，这个命令可以返回给定的有序集合键 key 中，值介于 min 和 max 之间的成员。
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="min">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        ///// <param name="max">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        ///// <param name="limit">返回多少成员</param>
        ///// <param name="offset">返回条件偏移位置</param>
        ///// <returns></returns>
        //public async Task<string[]> ZRangeByLexAsync(string key, string min, string max, long? limit = null, long offset = 0)
        //{
        //    key = await FillKeyAsync(key);
        //    var database = await GetRedisClientAsync();
        //    return await database.ZRangeByLexAsync(key, min, max, limit, offset);
        //}

        ///// <summary>
        ///// 当有序集合的所有成员都具有相同的分值时，有序集合的元素会根据成员的字典序来进行排序，这个命令可以返回给定的有序集合键 key 中，值介于 min 和 max 之间的成员。
        ///// </summary>
        ///// <typeparam name="T">byte[] 或其他类型</typeparam>
        ///// <param name="key"></param>
        ///// <param name="min">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        ///// <param name="max">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        ///// <param name="limit">返回多少成员</param>
        ///// <param name="offset">返回条件偏移位置</param>
        ///// <returns></returns>
        //public async Task<T[]> ZRangeByLexAsync<T>(string key, string min, string max, long? limit = null, long offset = 0)
        //{
        //    key = await FillKeyAsync(key);
        //    var database = await GetRedisClientAsync();
        //    return await database.ZRangeByLexAsync<T>(key, min, max, limit, offset);
        //}

        /// <summary>
        /// 当有序集合的所有成员都具有相同的分值时，有序集合的元素会根据成员的字典序来进行排序，这个命令可以返回给定的有序集合键 key 中，值介于 min 和 max 之间的成员。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <param name="max">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <returns></returns>
        public async Task<long> ZRemRangeByLexAsync(string key, string min, string max)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SortedSetRemoveRangeByValueAsync(key, min, max);
        }

        /// <summary>
        /// 当有序集合的所有成员都具有相同的分值时，有序集合的元素会根据成员的字典序来进行排序，这个命令可以返回给定的有序集合键 key 中，值介于 min 和 max 之间的成员。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="min">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <param name="max">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <returns></returns>
        public async Task<long> ZLexCountAsync(string key, string min, string max)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.SortedSetLengthByValueAsync(key, min, max);
        }

        public async Task<bool> GeoAddAsync(string key, double longitude, double latitude, object member)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.GeoAddAsync(key, longitude, latitude, member.ToRedisValue());
        }

        #endregion

        #region Geo redis-server 3.2 经纬度 范围

        /// <summary>
        /// 将指定的地理空间位置（纬度、经度、成员）添加到指定的key中。这些数据将会存储到sorted set这样的目的是为了方便使用GEORADIUS或者GEORADIUSBYMEMBER命令对数据进行半径查询等操作。
        /// </summary>
        /// <param name="database">Redis客户端</param>
        /// <param name="key"></param>
        /// <param name="values">批量添加的值</param>
        /// <returns>添加到sorted set元素的数目，但不包括已更新score的元素。</returns>
        public async Task<long> GeoAddAsync(IDatabase database, string key,
            params (double Longitude, double Latitude, object Member)[] values)
        {
            key = await FillKeyAsync(key);
            //database ??= await GetRedisClientAsync();
            return await database.GeoAddAsync(key,
                values.Select(x => new GeoEntry(longitude: (double)x.Longitude, latitude: (double)x.Latitude,
                    x.Member.ToRedisValue())).ToArray());
        }

        /// <summary>
        /// 将指定的地理空间位置（纬度、经度、成员）添加到指定的key中。这些数据将会存储到sorted set这样的目的是为了方便使用GEORADIUS或者GEORADIUSBYMEMBER命令对数据进行半径查询等操作。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="member">成员</param>
        /// <returns>是否成功</returns>
        public async Task<bool> GeoAddAsync(string key, decimal longitude, decimal latitude, object member)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.GeoAddAsync(key, (double)longitude, (double)latitude, member.ToRedisValue());
        }

        /// <summary>
        /// 将指定的地理空间位置（纬度、经度、成员）添加到指定的key中。这些数据将会存储到sorted set这样的目的是为了方便使用GEORADIUS或者GEORADIUSBYMEMBER命令对数据进行半径查询等操作。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values">批量添加的值</param>
        /// <returns>添加到sorted set元素的数目，但不包括已更新score的元素。</returns>
        public async Task<long> GeoAddAsync(string key,
            (decimal longitude, decimal latitude, object member)[] values)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.GeoAddAsync(key, values
                .Select(x => new GeoEntry(longitude: (double)x.longitude, latitude: (double)x.latitude,
                    x.member.ToRedisValue())).ToArray());
        }

        /// <summary>
        /// 将指定的地理空间位置（纬度、经度、成员）添加到指定的key中。这些数据将会存储到sorted set这样的目的是为了方便使用GEORADIUS或者GEORADIUSBYMEMBER命令对数据进行半径查询等操作。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values">批量添加的值</param>
        /// <returns>添加到sorted set元素的数目，但不包括已更新score的元素。</returns>
        public async Task<long> GeoAddAsync(string key, (double longitude, double latitude, object member)[] values)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.GeoAddAsync(key, values
                .Select(x => new GeoEntry(longitude: (double)x.longitude, latitude: (double)x.latitude,
                    x.member.ToRedisValue())).ToArray());
        }

        /// <summary>
        /// 返回两个给定位置之间的距离。如果两个位置之间的其中一个不存在， 那么命令返回空值。GEODIST 命令在计算距离时会假设地球为完美的球形， 在极限情况下， 这一假设最大会造成 0.5% 的误差。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member1">成员1</param>
        /// <param name="member2">成员2</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <returns>计算出的距离会以双精度浮点数的形式被返回。 如果给定的位置元素不存在， 那么命令返回空值。</returns>
        public async Task<decimal?> GeoDistAsync(string key, object member1, object member2,
            GeoUnit unit = GeoUnit.Meters)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (decimal?)(await database.GeoDistanceAsync(key, member1.ToRedisValue(), member2.ToRedisValue(),
                unit));
        }

        /// <summary>
        /// 返回一个或多个位置元素的 Geohash 表示。通常使用表示位置的元素使用不同的技术，使用Geohash位置52点整数编码。由于编码和解码过程中所使用的初始最小和最大坐标不同，编码的编码也不同于标准。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="members">多个查询的成员</param>
        /// <returns>一个数组， 数组的每个项都是一个 geohash 。 命令返回的 geohash 的位置与用户给定的位置元素的位置一一对应。</returns>
        public async Task<string[]> GeoHashAsync(string key, object[] members)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await database.GeoHashAsync(key, members.Select(x => x.ToRedisValue()).ToArray
                ());
        }

        /// <summary>
        /// 从key里返回所有给定位置元素的位置（经度和纬度）。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="members">多个查询的成员</param>
        /// <returns>GEOPOS 命令返回一个数组， 数组中的每个项都由两个元素组成： 第一个元素为给定位置元素的经度， 而第二个元素则为给定位置元素的纬度。当给定的位置元素不存在时， 对应的数组项为空值。</returns>
        public async Task<(double longitude, double latitude)?[]> GeoPosAsync(string key, object[] members)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoPositionAsync(key, members.Select(x => x.ToRedisValue()).ToArray
                ())).Select(y =>
                {
                    (double longitude, double latitude)? v = null;
                    if (y == null) return v;
                    else
                    {
                        v = (
                        (double)y.Value.Longitude, (double)y.Value.Latitude
                    );
                        return v;
                    }
                }).ToArray();
        }

        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<string[]> GeoRadiusAsync(string key, decimal longitude, decimal latitude,
            decimal radius, GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null
        )
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, (double)longitude, (double)latitude, (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x => x.Member.ToString()).ToArray()
                ;
        }

        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<T[]> GeoRadiusAsync<T>(string key, decimal longitude, decimal latitude, decimal radius,
            GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, (double)longitude, (double)latitude, (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x => x.ToString()).ToValues<T>()
                ;
        }

        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离）。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<(string member, decimal dist)[]> GeoRadiusWithDistAsync(string key, decimal longitude,
            decimal latitude, decimal radius, GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null
        )
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, (double)longitude, (double)latitude, (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x =>
                {
                    (string member, decimal dist) v;
                    v.member = x.Member;
                    v.dist = (decimal)(x.Distance ?? 0);
                    return v;
                }).ToArray();
        }

        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离）。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<(T member, decimal dist)[]> GeoRadiusWithDistAsync<T>(string key, decimal longitude,
            decimal latitude, decimal radius, GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, (double)longitude, (double)latitude, (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x =>
                {
                    (T member, decimal dist) v;
                    v.member = x.Member.ToString().ToObjectValue<T>();
                    v.dist = (decimal)(x.Distance ?? 0);
                    return v;
                }).ToArray();
        }

        public async Task<(string member, double? dist, double? longitude, double? latitude)[]>
            GeoRadiusWithDistAndCoordAsync(string key, double longitude, double latitude, double radius,
                GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, longitude, latitude, radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x =>
                {
                    (string member, double? dist, double? longitude, double? latitude) v;
                    v.member = x.Member.ToString();
                    v.dist = x.Distance;
                    v.longitude = x.Position?.Longitude;
                    v.latitude = x.Position?.Latitude;
                    return v;
                }).ToArray();
        }

        public async Task<(T member, double? dist, double? longitude, double? latitude)[]>
            GeoRadiusWithDistAndCoordAsync<T>(string key, double longitude, double latitude, double radius,
                GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, longitude, latitude, radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x =>
                {
                    (T member, double? dist, double? longitude, double? latitude) v;
                    v.member = x.Member.ToString().ToObjectValue<T>();
                    v.dist = x.Distance;
                    v.longitude = x.Position?.Longitude;
                    v.latitude = x.Position?.Latitude;
                    return v;
                }).ToArray();
        }

        public async Task<string[]> GeoRadiusByMemberAsync(string key, object member, double radius,
            GeoUnit unit = GeoUnit.Meters, long? count = null,
            Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, member.ToRedisValue(), (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x => x.Member.ToString()).ToArray();
        }

        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离、经度、纬度）。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<(string member, decimal dist, decimal longitude, decimal latitude)[]>
            GeoRadiusWithDistAndCoordAsync(string key, decimal longitude, decimal latitude, decimal radius,
                GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, (double)longitude, (double)latitude, (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x =>
                {
                    (string member, decimal dist, decimal longitude, decimal latitude) v;
                    v.member = x.Member.ToString();
                    v.dist = (decimal)(x.Distance ?? 0);
                    v.longitude = (decimal)(x.Position?.Longitude ?? 0);
                    v.latitude = (decimal)(x.Position?.Latitude ?? 0);
                    return v;
                }).ToArray();
        }

        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离、经度、纬度）。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<(T member, decimal dist, decimal longitude, decimal latitude)[]>
            GeoRadiusWithDistAndCoordAsync<T>(string key, decimal longitude, decimal latitude, decimal radius,
                GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, (double)longitude, (double)latitude, (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x =>
                {
                    (T member, decimal dist, decimal longitude, decimal latitude) v;
                    v.member = x.Member.ToString().ToObjectValue<T>();
                    v.dist = (decimal)(x.Distance ?? 0);
                    v.longitude = (decimal)(x.Position?.Longitude ?? 0);
                    v.latitude = (decimal)(x.Position?.Latitude ?? 0);
                    return v;
                }).ToArray();
        }

        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<string[]> GeoRadiusByMemberAsync(string key, object member, decimal radius,
            GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, member.ToRedisValue(), (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x => x.Member.ToString()).ToArray()
                ;
        }

        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<T[]> GeoRadiusByMemberAsync<T>(string key, object member, decimal radius,
            GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, member.ToRedisValue(), (double)radius, unit, (int)(count ?? -1),
                    sorting))
                .Select(x => x.ToString()).ToValues<T>();
        }

        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离）。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<(string member, double dist)[]> GeoRadiusByMemberWithDistAsync(string key,
            object member, decimal radius, GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, member.ToRedisValue(), (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x =>
                {
                    (string member, double dist) v;
                    v.member = x.Member;
                    v.dist = (x.Distance ?? 0);
                    return v;
                }).ToArray()
                ;
        }

        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离）。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<(T member, decimal dist)[]> GeoRadiusByMemberWithDistAsync<T>(string key,
            object member, decimal radius, GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, member.ToRedisValue(), (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x =>
                {
                    (T member, decimal dist) v;
                    v.member = x.Member.ToString().ToObjectValue<T>();
                    v.dist = (decimal)(x.Distance ?? 0);
                    return v;
                }).ToArray()
                ;
        }

        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离、经度、纬度）。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<(string member, decimal dist, decimal longitude, decimal latitude)[]>
            GeoRadiusByMemberWithDistAndCoordAsync(string key, object member, decimal radius,
                GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, member.ToRedisValue(), (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x =>
                {
                    (string member, decimal dist, decimal longitude, decimal latitude) v;
                    v.member = x.Member.ToString();
                    v.dist = (decimal)(x.Distance ?? 0);
                    v.longitude = (decimal)(x.Position?.Longitude ?? 0);
                    v.latitude = (decimal)(x.Position?.Latitude ?? 0);
                    return v;
                }).ToArray()
                ;
        }

        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离、经度、纬度）。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        public async Task<(T member, decimal dist, decimal longitude, decimal latitude)[]>
            GeoRadiusByMemberWithDistAndCoordAsync<T>(string key, object member, decimal radius,
                GeoUnit unit = GeoUnit.Meters, long? count = null, Order? sorting = null)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return (await database.GeoRadiusAsync(key, member.ToRedisValue(), (double)radius, unit,
                    (int)(count ?? -1), sorting))
                .Select(x =>
                {
                    (T member, decimal dist, decimal longitude, decimal latitude) v;
                    v.member = x.Member.ToString().ToObjectValue<T>();
                    v.dist = (decimal)(x.Distance ?? 0);
                    v.longitude = (decimal)(x.Position?.Longitude ?? 0);
                    v.latitude = (decimal)(x.Position?.Latitude ?? 0);
                    return v;
                }).ToArray()
                ;
        }

        #endregion

        #endregion

        
        #region 委托方法调用

        #region String

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <param name="database">客户端</param>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <param name="expireFunc">设置过期时间方法</param>
        /// <returns></returns>
        private async Task<string> GetValueAsync(string key,
            Func<Task<string>> getValue, bool resetExpire, Func<Task<bool>> expireFunc, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            var value = await database.StringGetAsync(key);
            if (value.HasValue)
            {
                if (resetExpire)
                {
                    await expireFunc();
                }

                return value;
            }
            value = await getValue();
            await database.StringSetAsync(key, value);
            await expireFunc();
            return value;
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <param name="database">客户端</param>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key, Func<Task<string>> getValue,
            bool resetExpire = false, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            return await GetValueAsync(key, getValue, resetExpire, async () => resetExpire
                ? await database.KeyExpireAsync(key, TimeSpan.FromHours(1))
                : await database.KeyExpireAsync(key, TimeSpan.FromDays(7)), database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <param name="database">客户端</param>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="seconds">过期秒数</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key, Func<Task<string>> getValue,
            int seconds, bool resetExpire = false, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            return await GetValueAsync(key, getValue, resetExpire,
                async () => await database.KeyExpireAsync(key, TimeSpan.FromSeconds(seconds)), database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <param name="database">客户端</param>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="expire">过期时间</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key, Func<Task<string>> getValue,
            TimeSpan expire, bool resetExpire = false, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            return await GetValueAsync(key, getValue, resetExpire,
                async () => await database.KeyExpireAsync(key, expire), database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <param name="database">客户端</param>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="expire">指定的过期时间</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key, Func<Task<string>> getValue,
            DateTime expire, bool resetExpire = false, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            return await GetValueAsync(key, getValue, resetExpire,
                async () => await database.KeyExpireAsync(key, expire), database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key, Func<Task<string>> getValue, bool resetExpire = false)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await GetAsync(key, getValue, resetExpire, database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="seconds">过期秒数</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key, Func<Task<string>> getValue, int seconds,
            bool resetExpire = false)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await GetAsync(key, getValue, seconds, resetExpire, database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="expire">过期时间</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key, Func<Task<string>> getValue, TimeSpan expire,
            bool resetExpire = false)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await GetAsync(key, getValue, expire, resetExpire, database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="expire">指定的过期时间</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key, Func<Task<string>> getValue, DateTime expire,
            bool resetExpire = false)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await GetAsync( key, getValue, expire, resetExpire, database);
        }

        #endregion

        #region 泛型

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">客户端</param>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <param name="expireFunc">设置过期时间方法</param>
        /// <returns></returns>
        private async Task<T> GetValueAsync<T>(string key, Func<Task<T>> getValue,
            bool resetExpire, Func<Task<bool>> expireFunc, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            var valueStr = await database.StringGetAsync(key);

            if (valueStr.HasValue)
            {
                var value = valueStr.ToString().ToObjectValue<T>();
                if (resetExpire)
                {
                    await expireFunc();
                }

                return value;
            }
            else
            {
                var value = await getValue();
                await database.StringSetAsync(key, value.ToRedisValue());
                await expireFunc();
                return value;
            }
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">客户端</param>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> getValue,
            bool resetExpire = false, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            return await GetValueAsync(key, getValue, resetExpire, async () =>
                await database.KeyExpireAsync(key, TimeSpan.FromHours(1)), database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">客户端</param>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="seconds">过期秒数</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> getValue,
            int seconds, bool resetExpire = false, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            return await GetValueAsync(key, getValue, resetExpire,
                async () => await database.KeyExpireAsync(key, TimeSpan.FromSeconds(seconds)), database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">客户端</param>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="expire">过期时间</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> getValue,
            TimeSpan expire, bool resetExpire = false, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            return await GetValueAsync(key, getValue, resetExpire,
                async () => await database.KeyExpireAsync(key, expire), database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">客户端</param>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="expire">指定的过期时间</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> getValue,
            DateTime expire, bool resetExpire = false, IDatabase? database = null)
        {
            key = await FillKeyAsync(key);
            database ??= await GetRedisClientAsync();
            return await GetValueAsync(key, getValue, resetExpire,
                async () => await database.KeyExpireAsync(key, expire), database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> getValue, bool resetExpire = false)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await GetAsync(key, getValue, resetExpire, database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="seconds">过期秒数</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> getValue, int seconds,
            bool resetExpire = false)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await GetAsync(key, getValue, seconds, resetExpire, database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="expire">过期时间</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> getValue, TimeSpan expire,
            bool resetExpire = false)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await GetAsync(key, getValue, expire, resetExpire, database);
        }

        /// <summary>
        /// 获取指定Key的值
        /// 如何key不存在，则从委托中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="getValue">委托</param>
        /// <param name="expire">指定的过期时间</param>
        /// <param name="resetExpire">重置过期时间</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> getValue, DateTime expire,
            bool resetExpire = false)
        {
            key = await FillKeyAsync(key);
            var database = await GetRedisClientAsync();
            return await GetAsync(key, getValue, expire, resetExpire, database);
        }

        #endregion

        #endregion
    }
}
