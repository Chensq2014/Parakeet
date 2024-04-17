using System;
using Serilog;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp.Cache
{
    /// <summary>
    /// 缓存应用场景？
    /// 1、访问频繁+耗时耗资源+相对稳定+体积小(视情况而定) 存一次能查3次就值得缓存(大型项目标准)
    /// 2、字典/省市区/配置文件/网站公告/部门/用户/权限/热搜/类别列表/产品列表
    /// 3、商品评论 虽然评论会变，但不重要，而且第一页一般不变
    /// </summary>
    public static class CustomCache
    {
        /// <summary>
        /// 多线程操作非线程安全容器会造成冲突
        /// 1、线程安全容器 ConcurrentDictionary Api差别大
        /// 2、使用lock---Add Remove foreach
        /// 怎么降低影响，提升性能
        /// 多个数据容器，多个锁，容器之间可以并发  
        /// </summary>
        private static readonly object _lock = new object();
        private static void LockAction(Action action)
        {
            lock (_lock)
            {
                action.Invoke();
            }
        }
        /// <summary>
        /// private 保护数据   
        /// static  全局唯一  不释放
        /// Dictionary  保存多项数据
        /// </summary>
        private static Dictionary<string, DataModel> _customCacheDictionary;
        /// <summary>
        /// 清理缓存之后的事件
        /// </summary>
        public static event Action ClearEvent;

        private static int _cpuNumber = 3;//获取系统cpu数
        private static List<Dictionary<string, object>> _dictionaryList = new List<Dictionary<string, object>>();
        private static List<object> _lockList = new List<object>();
        
        /// <summary>
        /// 主动清理+被动清理，保证过期数据不会被查询;过期数据也不会滞留太久
        /// </summary>

        static CustomCache()
        {
            _cpuNumber = 3;
            for (int i = 0; i < _cpuNumber; i++)
            {
                _dictionaryList.Add(new Dictionary<string, object>());
                _lockList.Add(new object());
            }


            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var keyList = new List<string>();
                        for (int i = 0; i < _cpuNumber; i++)
                        {
                            lock (_lockList[i])
                            {
                                foreach (var key in _customCacheDictionary.Keys)
                                {
                                    if (_customCacheDictionary.ContainsKey(key))
                                    {
                                        var model = _customCacheDictionary[key];

                                        if (model.ObsoluteType != ObsoluteType.Never && model.DeadLine < DateTime.Now)
                                        {
                                            keyList.Add(key);
                                        }
                                    }
                                }
                                keyList.ForEach(Remove);
                                Thread.Sleep(1000 * 60 * 10);//10分钟清理一次缓存
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Debug(e.Message);
                        throw;
                    }
                }
            });
        }

        //public static void Remove(string key)
        //{
        //    int hash = key.GetHashCode();//相对均匀且稳定
        //    int index = hash % _cpuNumber;
        //    lock (_lockList[index])
        //    {
        //        _dictionaryList[index].Remove(key);
        //    }
        //}



        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="oValue"></param>
        /// <param name="timeOutSecond"></param>
        public static void Add(string key, object oValue, int timeOutSecond)
        {
            _customCacheDictionary.Add(key, new DataModel
            {
                Value = oValue,
                ObsoluteType = ObsoluteType.Absolutely,
                DeadLine = DateTime.Now.AddSeconds(timeOutSecond),
                //Duration = timeOutSecond
            });
        }


        /// <summary>
        /// 假如给key 你怎么知道在哪里？需要多一个规则
        /// </summary>
        /// <param name="key"></param>
        /// <param name="oValue"></param>
        public static void Add(string key, object oValue)
        {
            int hash = key.GetHashCode();//相对均匀且稳定
            int index = hash % _cpuNumber;
            lock (_lockList[index])
            {
                _dictionaryList[index].Add(key, oValue);
            }
        }


        /// <summary>
        /// 检查是否存在
        /// 
        /// 清理一下，除非我们去访问这条缓存，才会去清理  被动清理，任何过期的数据，都不可以被查到
        /// 可能有垃圾留在缓存里面
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exsit(string key)
        {
            if (_customCacheDictionary.ContainsKey(key))
            {
                var model = _customCacheDictionary[key];

                if (model.ObsoluteType == ObsoluteType.Never)
                {
                    return true;
                }
                else
                if (model.DeadLine < DateTime.Now)//当前时间超过过期时间了
                {
                    int hash = key.GetHashCode();//相对均匀且稳定
                    int index = hash % _cpuNumber;
                    lock (_lockList[index])
                    {
                        Remove(key);//清理一下
                    }
                    return false;
                }
                else
                if (model.ObsoluteType == ObsoluteType.Relative)//没有过期而且是滑动过期 所以要更新
                {
                    model.DeadLine = DateTime.Now.Add(model.Duration);
                    //CustomCacheDictionary[key] = model;//model本来就是引用类型
                    return true;
                }
            }

            return false;
        }

        ///// <summary>
        ///// 不能用这个锁，因为调用它的地方使用了锁，要保证这里面锁跟外面的锁相同，干脆去掉
        ///// </summary>
        ///// <param name="key"></param>
        //public static void Remove(string key)
        //{
        //    int hash = key.GetHashCode();//相对均匀且稳定
        //    int index = hash % _cpuNumber;
        //    lock (_lockList[index])
        //    {
        //        Remove(key);
        //    }
        //}

        /// <summary>
        /// 不能用这个锁，因为调用它的地方使用了锁，要保证这里面锁跟外面的锁相同，干脆去掉
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            _customCacheDictionary.Remove(key);
            //LockAction(() =>
            //{
            //    _customCacheDictionary.Remove(key);
            //});
        }
        public static void RemoveAll()
        {
            LockAction(() =>
            {
                _customCacheDictionary.Clear();
            });
        }
    }


}
