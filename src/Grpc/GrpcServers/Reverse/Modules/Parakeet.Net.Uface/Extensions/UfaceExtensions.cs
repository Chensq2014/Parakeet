using Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Parakeet.Net.Uface.Extensions
{
    public static class UfaceExtensions
    {
        private static readonly object _locker = new object();
        public static void AddUface(this IServiceCollection services)
        {
            #region Chongqing

            services.AddScoped<IReverseCommand, Commands.Chongqing.AddPersonCommand>();
            services.AddScoped<IReverseCommand, Commands.Chongqing.DeletePersonCommand>();

            #endregion Chongqing

            #region Standard

            services.AddScoped<IReverseCommand, Commands.Standard.AddPersonCommand>();
            services.AddScoped<IReverseCommand, Commands.Standard.DeletePersonCommand>();

            #endregion Standard
        }

        /// <summary>
        /// 延时随机数毫秒时间 多线程需加锁
        /// 写在公共静态类UfaceExtensions里面会导致  所有请求都排队(业务上应该相同业务操作的请求调用才排队)
        /// </summary>
        public static void Delay()
        {
            lock (_locker)
            {
                Task.Delay(new Random().Next(100, 500)).Wait();
            }
        }
    }
}