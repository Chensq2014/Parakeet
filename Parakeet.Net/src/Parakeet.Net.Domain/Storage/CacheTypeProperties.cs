using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace Parakeet.Net.Storage
{
    ///// <summary>
    ///// 缓存固定类型列属性 以便参数化拼装Sql
    ///// </summary>
    //public class CacheTypeProperties<TEntity> where TEntity : BaseEntity
    //{
    //    #region 拼接sql需要的实体类型属性/列名 静态缓存 private为了安全 static不被GC KeyValuePair，dictionary 读写效率高
    //    /// <summary>
    //    /// KeyValuePair 和 Dictionary 的关系
    //    ///1、KeyValuePair 
    //    ///    a、KeyValuePair 是一个结构体（struct）；
    //    ///    b、KeyValuePair 只包含一个Key、Value的键值对。
    //    ///2、Dictionary 
    //    ///    a、Dictionary 可以简单的看作是KeyValuePair 的集合；
    //    ///    b、Dictionary 可以包含多个Key、Value的键值对。
    //    /// </summary>

    //    private static KeyValuePair<Type, List<PropertyInfo>> _typeProperties;//用来缓存实体类型对应的PropertyInfos
    //    private static KeyValuePair<Type, List<string>> _typePropNames;//缓存数据库列名

    //    //private static Dictionary<Type, List<PropertyInfo>> _propertiesDic;
    //    //private static Dictionary<Type, List<string>> _namesDic;

    //    #endregion

    //    #region 静态构造函数
    //    /// <summary>
    //    /// 静态构造函数，被初始化一次，优化性能
    //    /// </summary>
    //    static CacheTypeProperties()
    //    {
    //        var type = typeof(TEntity);
    //        //var dictionary=new Dictionary<Type,List<PropertyInfo>>();
    //        _typeProperties = new KeyValuePair<Type, List<PropertyInfo>>(type, GetPropertyInfos(type));
    //        _typePropNames = new KeyValuePair<Type, List<string>>(type, GetPropNames(type));
    //    }

    //    #endregion

    //    #region 静态方法
    //    /// <summary>
    //    /// 缓存实体类型的 所有可用属性/字段(未被特殊属性标记：如BaseField)
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns>返回当前缓存的实体属性集合</returns>
    //    public static List<PropertyInfo> GetPropertyInfos(Type type)
    //    {
    //        if (_typeProperties.Key != type)
    //        {
    //            var propInfos = new List<PropertyInfo>();

    //            if (type.IsDefined(typeof(TableAttribute), true))
    //            {
    //                foreach (var prop in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
    //                {
    //                    if (prop.IsDefined(typeof(KeyAttribute), true)
    //                        || prop.IsDefined(typeof(NotSetAttribute), true)
    //                        || prop.IsDefined(typeof(BaseFieldAttribute), true))
    //                    {
    //                        continue;
    //                    }
    //                    propInfos.Add(prop);
    //                }
    //            }
    //            _typeProperties = new KeyValuePair<Type, List<PropertyInfo>>(type, propInfos);
    //        }
    //        return _typeProperties.Value;
    //    }

    //    /// <summary>
    //    /// 缓存类型的 所有可用属性/字段名称(未被特殊属性标记：如BaseField)
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns>返回实体列名集合</returns>
    //    public static List<string> GetPropNames(Type type)
    //    {
    //        if (_typePropNames.Key != type)
    //        {
    //            var propNames = new List<string>();
    //            foreach (var prop in _typeProperties.Value)
    //            {
    //                propNames.Add(prop.GetColumnName());
    //            }
    //            _typePropNames = new KeyValuePair<Type, List<string>>(type, propNames);
    //        }
    //        return _typePropNames.Value;
    //    }

    //    /// <summary>
    //    /// 缓存泛型sql
    //    /// 实体除公共属性与字段外的insert sql语句
    //    /// </summary>
    //    /// <param name="entities">插入数据集合</param>
    //    /// <param name="sql"></param>
    //    /// <returns>返回sql给用户可继续拼接</returns>
    //    public static StringBuilder GetInsertSql(List<TEntity> entities, StringBuilder sql = null)
    //    {
    //        var type = typeof(TEntity);
    //        sql = sql ?? new StringBuilder();
    //        if (type.IsDefined(typeof(TableAttribute), true))
    //        {
    //            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
    //            var propInfos = _typeProperties.Value;//读缓存
    //            var propNames = new List<string>(GetPropNames(type));//读缓存

    //            #region 可按需顺序添加自定义列区域A

    //            //被标识为BaseField的公共字段 propNames 可自定义按需顺序添加
    //            //propNames.Add($"City_ProjectId");
    //            //propNames.Add($"CreatorId");
    //            //propNames.Add($"CreatedAt");
    //            //propNames.Add($"UpdatedAt");
    //            #endregion

    //            entities.ForEach(entity =>
    //            {
    //                //需要插入数据库的columns 每个实体都要根据属性映射一遍(浪费性能？)实测拼接过程几乎可忽略
    //                var columns = new List<string>();
    //                propInfos.ForEach(prop =>
    //                {
    //                    //列信息：判断是否枚举/可空枚举类型/其它类型 并赋值
    //                    columns.Add(prop.PropertyType.IsEnum
    //                        ? $"{(int)Enum.Parse(prop.PropertyType, prop.GetValue(entity).ToString())}"
    //                        : prop.PropertyType.IsValueType
    //                           && prop.PropertyType.IsGenericType
    //                           && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
    //                           && prop.PropertyType.GetGenericArguments()[0].IsEnum
    //                            ? $"{(prop.GetValue(entity) == null ? "NULL" : Convert.ToInt32(prop.GetValue(entity)).ToString()) }"
    //                            : $"'{prop.GetValue(entity) ?? DBNull.Value}'");
    //                });

    //                #region 可按需顺序添加自定义列对照区域A
    //                //被标识为BaseField的公共字段 columns中 可自定义按需顺序添加
    //                //columns.Add($"'{projectId.NullString()}'");
    //                //columns.Add($"'{userId}'");
    //                //columns.Add($"'{entity.CreatedAt.ToDateTimeString()}'");
    //                //columns.Add($"'{DateTime.Now.ToDateTimeString()}'");
    //                #endregion

    //                //因此处自己拼接一个集合为一条sql，不便使用参数化插入
    //                sql.AppendLine($@"insert into [dbo].[{tableName}] ([{string.Join("],[", propNames)}]) values({string.Join(",", columns)})");
    //            });
    //        }
    //        return sql;
    //    }

    //    #endregion
    //}
}
