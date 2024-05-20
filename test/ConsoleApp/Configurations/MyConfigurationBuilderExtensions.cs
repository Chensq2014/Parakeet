using System;
using System.Collections.Generic;
using System.Text;
using ConsoleApp.Configurations;
using Microsoft.Extensions.Configuration;


namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// 首先把扩展方法的命名空间放在 config 的命名空间而不是自己的命名空间，
    /// 这样方便在引用的时候直接使用而无需加载具体的命名空间
    /// </summary>
    public static class MyConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddMyConfiguration(this IConfigurationBuilder builder)
        {
            builder.Add(new MyConfigurationSource());
            return builder;
        }
    }
}
