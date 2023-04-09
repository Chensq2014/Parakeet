using Parakeet.Net.Interfaces;
using System;
using System.Collections.Concurrent;

namespace Parakeet.Net.Singletons
{
    /// <summary>
    /// 自定义Module 模块池
    /// </summary>
    public class CustomModulePool
    {
        //private static CustomModulePool _single;
        //private static readonly object _flag = new object();

        public ConcurrentDictionary<string, ICustomStartup> Modules { get; private set; }

        private CustomModulePool()
        {
            Modules = new ConcurrentDictionary<string, ICustomStartup>();
        }

        public static CustomModulePool Instance = new CustomModulePool();
        //{
        //    get
        //    {
        //        //判断是否实例化过
        //        if (_single == null)
        //        {
        //            //进入lock
        //            lock (_flag)
        //            {
        //                //判断是否实例化过
        //                if (_single == null)
        //                {
        //                    _single = new CustomModulePool();
        //                }
        //            }
        //        }

        //        return _single;
        //    }
        //}

        public void Add(ICustomStartup startup)
        {
            Modules.AddOrUpdate(startup.Module.Name, startup, (k, v) => startup);
        }

        public ICustomStartup this[string key]
        {
            get
            {
                Modules.TryGetValue(key, out ICustomStartup startup);
                return startup;
            }
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            foreach (var startUpModule in Modules)
            {
                startUpModule.Value.Configure(serviceProvider);
            }
        }
    }
}
