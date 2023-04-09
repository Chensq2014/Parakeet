using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Interfaces;
using Parakeet.Net.Singletons;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// 容器扩展：根据节点 注册单例类
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static TModle ConfigureSingleton<TModle>(this IServiceCollection services, IConfiguration configuration, Func<TModle> pocoProvider) where TModle : class
        {
            CheckAuguments(services, configuration);
            var model = pocoProvider();
            configuration.Bind(model);
            services.AddSingleton(model);
            return model;
        }

        public static TModle ConfigureSingleton<TModle>(this IServiceCollection services, IConfiguration configuration, TModle model) where TModle : class
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            CheckAuguments(services, configuration);

            configuration.Bind(model);
            services.AddSingleton(model);
            return model;
        }


        private static void CheckAuguments(IServiceCollection services, IConfiguration configuration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
        }

        /// <summary>
        /// 配置自定义模块 StartUp中的ConfigureServices  向容器中注册服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureModuleServices(this IServiceCollection services, IConfiguration configuration)
        {
            //加载自定义模块(插件)
            foreach (var file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Plugins"), "*.dll", SearchOption.AllDirectories))
            {
                var assembly = Assembly.LoadFile(file);

                var typeToRegister = assembly.GetTypes().FirstOrDefault(type => type.BaseType != null &&
                                                                                typeof(ICustomStartup).IsAssignableFrom(type));
                if (typeToRegister != null)
                {
                    var startUp = (ICustomStartup)Activator.CreateInstance(typeToRegister, configuration);
                    startUp.ConfigureServices(services);
                    CustomModulePool.Instance.Add(startUp);
                }
            }
        }
    }
}
