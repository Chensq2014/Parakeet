using Common.Dtos;
using Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Parakeet.Net.EnumServices
{
    /// <summary>
    ///     构造枚举类型上下文
    /// </summary>
    public class EnumContext
    {
        #region 静态

        ///// <summary>
        /////     枚举类所在程序集名称 默认值 //"Parakeet.Net.Domain.Shared";
        ///// </summary>
        //public static string EnumAssemblyNames = CustomConfigurationManager.Configuration["App:EnumAssemblyNames"] ?? typeof(NetDomainSharedModule).Assembly.GetName().Name;

        /// <summary>
        ///     静态实例
        /// </summary>
        public static EnumContext Instance => new EnumContext();

        #endregion

        #region 初始化枚举类型字典

        /// <summary>
        ///     枚举项key itemName itemDescription itemKeyString 集合
        /// </summary>
        private Dictionary<Type, List<EnumTypeItemDto>> EnumTypeItemKeyNameDescriptions { get; }

        private EnumContext()
        {
            EnumTypeItemKeyNameDescriptions = new Dictionary<Type, List<EnumTypeItemDto>>();
        }

        /// <summary>
        ///     初始化枚举类型元数据
        /// </summary>
        /// <param name="type"></param>
        private void InitEnumTypeMetaData(Type type)
        {
            if (EnumTypeItemKeyNameDescriptions.ContainsKey(type))
            {
                return;
            }

            var typeItemKeyValueNameDescriptions = new List<EnumTypeItemDto>();
            var values = Enum.GetValues(type);
            foreach (var value in values)
            {
                var key = (int)value; ////EnumValue 枚举项的整型值
                var name = value.ToString(); ////EnumValueName 枚举项字符串
                var description = type.GetField(name).GetCustomAttribute<DescriptionAttribute>(); ////枚举项描述
                var enumDto = new EnumTypeItemDto(key, name, description?.Description);
                typeItemKeyValueNameDescriptions.Add(enumDto);
            }

            EnumTypeItemKeyNameDescriptions.Add(type, typeItemKeyValueNameDescriptions);
        }

        /// <summary>
        ///     确保枚举所有项已加载
        /// </summary>
        /// <param name="type"></param>
        private void LoadEnumTypeContext(Type type)
        {
            if (!EnumTypeItemKeyNameDescriptions.ContainsKey(type))
            {
                InitEnumTypeMetaData(type);
            }
        }

        #endregion

        #region 列出系统所有枚举列表及根据名称获取枚举类型方法

        /// <summary>
        ///     返回系统所有枚举类型 类型名称->类型描述/全名
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValueDto<string, string>> GetAllEnumTypeNames()
        {
            var keyValues = new List<KeyValueDto<string, string>>();
            //var assemblies = EnumAssemblyNames.Split(",").ToList();
            var assemblies = EnvironmentHelper.EnumAssemblyNames.Split(",").ToList();
            assemblies.ForEach(assembly => keyValues.AddRange(Assembly
                .Load(assembly).GetTypes() //全是枚举类的type,不存在属性的可空枚举,不用判断可空枚举
                .Where(l => l.IsEnum)//|| l.IsGenericType && l.GetGenericTypeDefinition() == typeof(Nullable<>) && l.GetGenericArguments()[0].IsEnum
                .Select(m => new KeyValueDto<string, string>(m.Name, m.GetCustomAttribute<DescriptionAttribute>()?.Description ?? m.Name))));
            return keyValues;
        }

        /// <summary>
        ///     根据枚举类名获取枚举类型的类型：typeOf(EnumType)
        /// </summary>
        /// <param name="input">枚举类名</param>
        /// <returns></returns>
        public Type GeTypeByName(InputNameDto input)
        {
            return GeTypeByName(input.Name);
        }

        /// <summary>
        ///     根据枚举类名获取枚举类型的类型：typeOf(EnumType) 
        /// </summary>
        /// <param name="name">枚举类名</param>
        /// <returns></returns>
        public Type GeTypeByName(string name)
        {
            name = name?.ToUpper();
            var assemblies = EnvironmentHelper.EnumAssemblyNames.Split(",").ToList();
            return assemblies.SelectMany(assembly => Assembly
                .Load(assembly).GetTypes()
                .Where(m => m.IsEnum && m.Name.ToUpper() == name))////同理：枚举类的type没有可空枚举的概念
                .FirstOrDefault();
        }

        #endregion

        #region 获取 枚举项集合 EnumTypeItemKeyNameDescriptions 返回给上端可组织任意返回数据格式

        /// <summary>
        ///     获取 枚举项集合 EnumTypeItemKeyNameDescriptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<EnumTypeItemDto> GetEnumTypeItemKeyNameDescriptions(InputNameDto input)
        {
            var type = GeTypeByName(input);
            if (!EnumTypeItemKeyNameDescriptions.ContainsKey(type))
            {
                InitEnumTypeMetaData(type);
            }

            return EnumTypeItemKeyNameDescriptions[type];
        }

        /// <summary>
        ///     获取 枚举项集合 EnumTypeItemKeyNameDescriptions
        /// </summary>
        /// <param name="name">枚举类名称</param>
        /// <returns></returns>
        public List<EnumTypeItemDto> GetEnumTypeItemKeyNameDescriptions(string name)
        {
            var type = GeTypeByName(name);
            if (!EnumTypeItemKeyNameDescriptions.ContainsKey(type))
            {
                InitEnumTypeMetaData(type);
            }

            return EnumTypeItemKeyNameDescriptions[type];
        }

        #endregion
    }
}