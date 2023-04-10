using System;
using System.Reflection;
using Parakeet.Net.Entities;
using Parakeet.Net.Extensions;
using Parakeet.Net.Helper;
using Volo.Abp.Application.Services;

namespace Parakeet.Net.Factory
{
    /// <summary>
    /// 泛型简单工厂:通过配置文件+反射创建实例
    /// 去掉泛型 CreateInstance也可以返回一个object对象
    /// </summary>
    public class ModelFactory<TEntity> where TEntity : BaseEntity
    {
        private static readonly Type DllType = null;
        static ModelFactory()
        {
            var dllConfig = EnvironmentHelper.GetValue("APPMODELDLL");//CustomConfigurationManager.Configuration["App:ModelDll"];
            if (dllConfig.HasValue())
            {
                var dllName = dllConfig.Split(',')[0];
                var typeName = dllConfig.Split(',')[1];

                Assembly assembly = Assembly.Load(dllName);
                DllType = assembly.GetType(typeName);
            }
        }

        public static IApplicationService CreateInstance()
        {
            return (IApplicationService)Activator.CreateInstance(DllType);
        }

    }
}
