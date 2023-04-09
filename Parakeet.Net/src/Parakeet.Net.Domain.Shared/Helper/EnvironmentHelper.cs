using System;
using System.Collections.Generic;

namespace Parakeet.Net.Helper
{
    /// <summary>
    /// 环境变量帮助类
    /// </summary>
    public class EnvironmentHelper
    {
        #region  属性

        /// <summary>
        /// 数据库类型 1 sqlserver 2 mysql 3 pgsql
        /// </summary>
        public static int DatabaseType => GetValue("DATABASE_TYPE", 3);

        /// <summary>
        /// 数据库连接地址
        /// </summary>
        public static string DatabaseConnectionString => GetValue("DATABASE_CONNECTION_STRING", "");

        /// <summary>
        /// csredis数据库连接地址
        /// </summary>
        public static string CsRedisConfigurationBase => GetValue("CSREDISCONFIGURATIONBASE", $"127.0.0.1:6379,password=,prefix=iot");

        /// <summary>
        /// StackRedisConn连接地址
        /// </summary>
        public static string StackRedisConn => GetValue("STACKREDISCONN", $"127.0.0.1:6379");
        
        /// <summary>
        /// ExchangeRedisConn连接地址
        /// </summary>
        public static string ExchangeRedisConn => GetValue("EXCHANGEREDISCONN", $"127.0.0.1:6379");

        /// <summary>
        /// 枚举类型所在程序集名 EnumAssemblyNames
        /// "Parakeet.Net.Domain.Shared,Parakeet.Net.Application", //枚举类型所在程序集名称,逗号分隔 
        /// </summary>
        public static string EnumAssemblyNames => GetValue("ENUMASSEMBLY_NAMES", typeof(EnvironmentHelper).Assembly.GetName().Name);

        /// <summary>
        /// 加密解密字符串 "Parakeet.Vip"
        /// </summary>
        public static string DesKey => GetValue("DESKEY", "Parakeet.Vip");

        /// <summary>
        /// 当前程序根目录
        /// </summary>
        public static string RootPath => Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        
        #endregion

        #region GetValue方法 Environment launchSetting只能配置 字符串数字和布尔类型 

        public static string GetValue(string name, string defaultValue = null)
        {
            //var val=NacosEnvironment.GetValue(name);
            //if (val != null) return val;

            return Environment.GetEnvironmentVariable(name) ?? defaultValue;
        }

        public static int GetValue(string name, int defaultValue)
        {
            //var val = NacosEnvironment.GetValue(name);
            //if (val != null) return Convert.ToInt32(val);

            var strValue = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrEmpty(strValue) ? defaultValue : int.Parse(strValue);
        }

        public static bool GetValue(string name, bool defaultValue)
        {
            //var val = NacosEnvironment.GetValue(name);
            //if (val != null) return Convert.ToBoolean(val);

            var strValue = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrEmpty(strValue) ? defaultValue : bool.Parse(strValue);
        }
        #endregion


        #region RedisConnections

        /// <summary>
        /// 获取CsRedis 16个db连接集合
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCsRedisDbConnections()
        {
            var connections = new List<string>();
            for (var index = 0; index < 15; index++)
            {
                connections.Add($"{CsRedisConfigurationBase},defaultDatabase={index}");
            }

            return connections;
        }

        #endregion
    }
}
