using System;
using System.Globalization;
using System.Timers;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp.Configurations
{
    /// <summary>
    /// 把 Provider 定义为 internal 的，默认是 internal，
    /// 如果说分发到第三方的话，internal 的类是不能被引用的，
    /// 这样就意味着只需要暴露一个扩展方法，而不需要暴露具体的配置源的实现
    /// </summary>
    class MyConfigurationProvider : ConfigurationProvider
    {
        System.Timers.Timer timer;
        private void Timer_Elapsed(object sender, ElapsedEventArgs e) => Load(true);
        public MyConfigurationProvider() : base()
        {
            // 用一个线程模拟配置发生变化，每三秒钟执行一次，告诉我们要重新加载配置
            timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 5000;
            timer.Start();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="reload">是否重新加载数据</param>
        void Load(bool reload)
        {
            // Data 表示 Key-value 数据，这是由 ConfigurationProvider 提供的一个数据承载的集合
            // 我们把最新的时间填充进去
            Data["lastTime"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            if (reload)
            {
                base.OnReload();
            }
        }
    }
}
