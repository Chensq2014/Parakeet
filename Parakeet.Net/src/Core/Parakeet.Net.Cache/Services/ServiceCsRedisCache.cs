using CSRedis;
using Parakeet.Net.Interfaces;
using static ServiceStack.Diagnostics.Events;

namespace Parakeet.Net.Cache.Services
{
    /// <summary>
    /// CsRedisCache
    /// </summary>
    public class ServiceCsRedisCache : CSRedisClient, IHandlerType
    {
        /// <summary>
        /// CsRedis
        /// </summary>
        public string HandlerType => "CsRedis";


        public ServiceCsRedisCache(string connectionString) : base(connectionString)
        {
        }

        public ServiceCsRedisCache(string connectionString, string[] sentinels, bool readOnly = false) : base(connectionString, sentinels, readOnly)
        {
        }

        public ServiceCsRedisCache(string connectionString, string[] sentinels, bool readOnly, SentinelMasterConverter convert) : base(connectionString, sentinels, readOnly, convert)
        {
        }

        /// <summary>
        /// 创建redis分区访问类，通过 KeyRule 对 key 进行分区，连接对应的 connectionString
        /// </summary>
        /// <param name="nodeRule">按key分区规则，返回值格式：127.0.0.1:6379/13，默认方案(null)：取key哈希与节点数取模</param>
        /// <param name="connectionStrings">127.0.0.1[:6379],password=123456,defaultDatabase=13,poolsize=50,ssl=false,writeBuffer=10240,prefix=key前辍</param>

        public ServiceCsRedisCache(Func<string, string> nodeRule, params string[] connectionStrings) : base(nodeRule, connectionStrings)
        {

        }

        protected ServiceCsRedisCache(Func<string, string> nodeRule, string[] sentinels, bool readOnly, SentinelMasterConverter convert = null, params string[] connectionStrings) : base(nodeRule, sentinels, readOnly, convert, connectionStrings)
        {
        }

        //        ///// <summary>
        //        ///// ChangeDb
        //        ///// </summary>
        //        ///// <param name="dbNum"></param>
        //        //public void ChangeDb(int dbNum)
        //        //{
        //        //    RedisHelper.Nodes.
        //        //}

        #region string

        ////-------------字符串(string)----------------
        //// 添加字符串键-值对
        //csredis.Set("hello", "1");
        //csredis.Set("world", "2");
        //csredis.Set("hello", "3");

        //// 根据键获取对应的值
        //csredis.Get("hello");

        //// 移除元素
        //csredis.Del("world");

        ///*    数值操作    */
        //csredis.Set("num-key", "24");

        //// value += 5
        //csredis.IncrBy("num-key",5); 
        //// output -> 29

        //// value -= 10
        //csredis.IncrBy("num-key", -10); 
        //// output -> 19

        ///*    字节串操作    */
        //csredis.Set("string-key", "hello ");

        //// 在指定key的value末尾追加字符串
        //csredis.Append("string-key", "world"); 
        //// output -> "hello world"

        //// 获取从指定范围所有字符构成的子串（start:3,end:7）
        //csredis.GetRange("string-key",3,7)  
        //// output ->  "lo wo"

        //// 用新字符串从指定位置覆写原value（index:4）
        //csredis.SetRange("string-key", 4, "aa"); 
        //// output -> "hellaaword"
        #endregion

        #region list


        ////-----------------列表(list)----------------
        //// 从右端推入元素
        //csredis.RPush("my-list", "item1", "item2", "item3", "item4"); 
        //// 从右端弹出元素
        //csredis.RPop("my-list");
        //// 从左端推入元素
        //csredis.LPush("my-list","LeftPushItem");
        //// 从左端弹出元素
        //csredis.LPop("my-list");

        //// 遍历链表元素（start:0,end:-1即可返回所有元素）
        //foreach (var item in csredis.LRange("my-list", 0, -1))
        //{
        //    Console.WriteLine(item);
        //}
        //    // 按索引值获取元素（当索引值大于链表长度，返回空值，不会报错）
        //    Console.WriteLine($"{csredis.LIndex("my-list", 1)}"); 

        //// 修剪指定范围内的元素（start:4,end:10）
        //csredis.LTrim("my-list", 4, 10);

        //// 将my-list最后一个元素弹出并压入another-list的头部
        //csredis.RPopLPush("my-list", "another-list");



        #endregion

        #region set

        ////------------------集合(set)----------------
        //// 实际上只插入了两个元素("item1","item2")
        //csredis.SAdd("my-set", "item1", "item1", "item2"); 

        //// 集合的遍历
        //foreach (var member in csredis.SMembers("my-set"))
        //{
        //    Console.WriteLine($"集合成员：{member.ToString()}");
        //}

        //// 判断元素是否存在
        //string member = "item1";
        //Console.WriteLine($"{member}是否存在:{csredis.SIsMember("my-set", member)}");
        //// output -> True

        //// 移除元素
        //csredis.SRem("my-set", member);
        //Console.WriteLine($"{member}是否存在:{csredis.SIsMember("my-set", member)}");
        //// output ->  False

        //// 随机移除一个元素
        //csredis.SPop("my-set");

        //csredis.SAdd("set-a", "item1", "item2", "item3", "item4", "item5");
        //csredis.SAdd("set-b", "item2", "item5", "item6", "item7");

        //// 差集
        //csredis.SDiff("set-a", "set-b");
        //// output -> "item1", "item3","item4"

        //// 交集
        //csredis.SInter("set-a", "set-b");
        //// output -> "item2","item5"

        //// 并集
        //csredis.SUnion("set-a", "set-b");
        //// output -> "item1","item2","item3","item4","item5","item6","item7"


        #endregion

        #region zset


        ////------------------有序集合----------------
        //// 向有序集合添加元素
        //csredis.ZAdd("Quiz", (79, "Math"));
        //csredis.ZAdd("Quiz", (98, "English"));
        //csredis.ZAdd("Quiz", (87, "Algorithm"));
        //csredis.ZAdd("Quiz", (84, "Database"));
        //csredis.ZAdd("Quiz", (59, "Operation System"));

        ////返回集合中的元素数量
        //csredis.ZCard("Quiz");

        //// 获取集合中指定范围(90~100)的元素集合
        //csredis.ZRangeByScore("Quiz", 90, 100);

        //// 获取集合所有元素并升序排序
        //csredis.ZRangeWithScores("Quiz", 0, -1);

        //// 移除集合中的元素
        //csredis.ZRem("Quiz", "Math");

        ////Key的过期
        //redis.Set("MyKey", "hello,world");
        //Console.WriteLine(redis.Get("MyKey"));
        //// output -> "hello,world"

        //redis.Expire("MyKey", 5); // key在5秒后过期，也可以使用ExpireAt方法让它在指定时间自动过期

        //Thread.Sleep(6000); // 线程暂停6秒
        //Console.WriteLine(redis.Get("MyKey"));
        //// output -> ""

        #endregion

        #region hash

        ////------------------散列(hashmap)----------------
        //// 向散列添加元素
        //csredis.HSet("ArticleID:10001", "Title", "了解简单的Redis数据结构");
        //csredis.HSet("ArticleID:10001", "Author", "xscape");
        //csredis.HSet("ArticleID:10001", "PublishTime", "2019-01-01");

        //// 根据Key获取散列中的元素
        //csredis.HGet("ArticleID:10001", "Title");

        //// 获取散列中的所有元素
        //foreach (var item in csredis.HGetAll("ArticleID:10001"))
        //{
        //    Console.WriteLine(item.Value);
        //}

        ////HMGet和HMSet是他们的多参数版本，一次可以处理多个键值对
        //var keys = new string[] { "Title", "Author", "publishTime" };
        //csredis.HMGet("ID:10001", keys);

        ////和处理字符串一样，我们也可以对散列中的值进行自增、自减操作，原理同字符串是一样的
        //csredis.HSet("ArticleID:10001", "votes", "257");
        //csredis.HIncrBy("ID:10001", "votes", 40);
        //// output -> 297

        #endregion

        #region Subscribe PSubscribe

        // 高级玩法：发布订阅
        //        rds.Subscribe(
        //        ("chan1", msg => Console.WriteLine(msg.Body)),
        //        ("chan2", msg => Console.WriteLine(msg.Body)));

        ////模式订阅（通配符）
        //        rds.PSubscribe(new[] { "test*", "*test001", "test*002" }, msg => {
        //            Console.WriteLine($"PSUB   {msg.MessageId}:{msg.Body}    {msg.Pattern}: chan:{msg.Channel}");
        //        });
        ////模式订阅已经解决的难题：
        ////1、分区的节点匹配规则，导致通配符最大可能匹配全部节点，所以全部节点都要订阅
        ////2、本组 "test*", "*test001", "test*002" 订阅全部节点时，需要解决同一条消息不可执行多次

        ////发布
        //    rds.Publish("chan1", "123123123");
        ////无论是分区或普通模式，rds.Publish 都可以正常通信



        #endregion

        #region 缓存壳

        //高级玩法：缓存壳
        //        //不加缓存的时候，要从数据库查询
        //        var t1 = Test.Select.WhereId(1).ToOne();

        //        //一般的缓存代码，如不封装还挺繁琐的
        //        var cacheValue = rds.Get("test1");
        //            if (!string.IsNullOrEmpty(cacheValue)) {
        //            try {
        //                return JsonConvert.DeserializeObject(cacheValue);
        //            } catch {
        //                //出错时删除key
        //                rds.Remove("test1");
        //                throw;
        //            }
        //        }
        //        var t1 = Test.Select.WhereId(1).ToOne();
        //        rds.Set("test1", JsonConvert.SerializeObject(t1), 10); //缓存10秒

        ////使用缓存壳效果同上，以下示例使用 string 和 hash 缓存数据
        //        var t1 = rds.CacheShell("test1", 10, () => Test.Select.WhereId(1).ToOne());
        //        var t2 = rds.CacheShell("test", "1", 10, () => Test.Select.WhereId(1).ToOne());
        //        var t3 = rds.CacheShell("test", new[] { "1", "2" }, 10, notCacheFields => new[] {
        //            ("1", Test.Select.WhereId(1).ToOne()),
        //            ("2", Test.Select.WhereId(2).ToOne())
        //        });


        #endregion




    }
}
