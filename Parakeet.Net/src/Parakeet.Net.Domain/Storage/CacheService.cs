using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Parakeet.Net.Cache;
using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Entities;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;

namespace Parakeet.Net.Storage
{
    /// <summary>
    /// 缓存实体类型properties服务
    /// </summary>
    public class CacheService : DomainService, ICacheService
    {
        //PropertyInfo缓存失败了 PropertyInfo是一个abstract类这个里面有不能被json序列化的对象
        //换一种缓存方式试试静态变量
        private readonly IDistributedCache<List<PropertyInfo>> _cacheTypeProperties;
        private readonly IDistributedCache<List<string>> _cacheTypePropNames;
        //泛型缓存
        //private static KeyValuePair<Type, List<PropertyInfo>> _typeProperties;//用来缓存实体类型对应的PropertyInfos
        public CacheService(
            IDistributedCache<List<PropertyInfo>> cacheTypeProperties,
            IDistributedCache<List<string>> cacheTypePropNames)
        {
            _cacheTypeProperties = cacheTypeProperties;
            _cacheTypePropNames = cacheTypePropNames;
        }

        /// <summary>
        /// 缓存实体类型的 所有可用属性/字段(未被特殊属性标记：如BaseField)
        /// </summary>
        /// <param name="type"></param>
        /// <returns>返回当前缓存的实体属性集合</returns>
        public async Task<List<PropertyInfo>> GetCachePropertyInfos(Type type)
        {
            //var propInfos = await _cacheTypeProperties.GetAsync($"{type.Name}_proInfos")??new List<PropertyInfo>();
            //if (!propInfos.Any())
            //{
            //    foreach (var prop in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
            //    {
            //        if (prop.IsDefined(typeof(KeyAttribute), true)
            //            || prop.IsDefined(typeof(NotSetAttribute), true)
            //            || prop.IsDefined(typeof(BaseFieldAttribute), true))
            //            //||prop.Name=="Id")
            //        {
            //            continue;
            //        }
            //        propInfos.Add(prop);
            //    }
            //    _cacheTypeProperties.Set(type.Name,propInfos);
            //}
            //return propInfos;
            return await _cacheTypeProperties.GetOrAddAsync($"{type.Name}_proInfos", async () =>
            {
                var propInfos = new List<PropertyInfo>();
                if (typeof(BaseEntity).IsAssignableFrom(type))//type.IsDefined(typeof(TableAttribute), true))
                {
                    foreach (var prop in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (prop.IsDefined(typeof(KeyAttribute), true)
                            || prop.IsDefined(typeof(NotSetAttribute), true)
                            || prop.IsDefined(typeof(BaseFieldAttribute), true))
                        //||prop.Name=="Id")
                        {
                            continue;
                        }
                        propInfos.Add(prop);
                    }
                }
                return propInfos;
            }, () => new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
            });
        }

        /// <summary>
        /// 缓存类型的 所有可用属性/字段名称(未被特殊属性标记：如BaseField)
        /// </summary>
        /// <param name="type"></param>
        /// <returns>返回实体列名集合</returns>
        public async Task<List<string>> GetCachePropNames(Type type)
        {
            return await _cacheTypePropNames.GetOrAddAsync($"{type.Name}_propNames", async () =>
            {
                var properties = await GetCachePropertyInfos(type);
                var propNames = new List<string>();
                foreach (var prop in properties)
                {
                    propNames.Add(prop.GetColumnName());
                }
                return propNames;
            }, () => new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
            });
        }

        /// <summary>
        /// 缓存泛型sql 未参数化
        /// 实体除公共属性与字段外的insert sql语句
        /// </summary>
        /// <param name="entities">插入数据集合</param>
        /// <param name="sql"></param>
        /// <returns>返回sql给用户可继续拼接</returns>
        public async Task<StringBuilder> GetInsertSql<TEntity>(List<TEntity> entities, StringBuilder sql = null) where TEntity:BaseEntity
        {
            var type = typeof(TEntity);
            sql ??= new StringBuilder();
            if (type.IsDefined(typeof(TableAttribute), true))
            {
                var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
                var propInfos = await GetCachePropertyInfos(type);
                var propNames = await GetCachePropNames(type);

                #region 可按需顺序添加自定义列区域A

                //被标识为BaseField的公共字段 propNames 可自定义按需顺序添加
                //propNames.Add($"City_ProjectId");
                //propNames.Add($"CreatorId");
                //propNames.Add($"CreatedAt");
                //propNames.Add($"UpdatedAt");
                #endregion

                entities.ForEach(entity =>
                {
                    //需要插入数据库的columns 每个实体都要根据属性映射一遍(浪费性能？)实测拼接过程几乎可忽略
                    var columns = new List<string>();
                    propInfos.ForEach(prop =>
                    {
                        //列信息：判断是否枚举/可空枚举类型/其它类型 并赋值
                        columns.Add(prop.PropertyType.IsEnum
                            ? $"{(int)Enum.Parse(prop.PropertyType, prop.GetValue(entity).ToString())}"
                            : prop.PropertyType.IsValueType
                               && prop.PropertyType.IsGenericType
                               && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                               && prop.PropertyType.GetGenericArguments()[0].IsEnum
                                ? $"{(prop.GetValue(entity) == null ? "NULL" : Convert.ToInt32(prop.GetValue(entity)).ToString()) }"
                                : $"'{prop.GetValue(entity) ?? DBNull.Value}'");
                    });

                    #region 可按需顺序添加自定义列对照区域A
                    //被标识为BaseField的公共字段 columns中 可自定义按需顺序添加
                    //columns.Add($"'{projectId.NullString()}'");
                    //columns.Add($"'{userId}'");
                    //columns.Add($"'{entity.CreatedAt.ToDateTimeString()}'");
                    //columns.Add($"'{DateTime.Now.ToDateTimeString()}'");
                    #endregion

                    //因此处自己拼接一个集合为一条sql，不使用参数化插入
                    sql.AppendLine($@"insert into [{tableName}] ({string.Join(",", propNames.Select(m=>$"[{m}]"))}) values({string.Join(",", columns)})");
                });
            }
            return sql;
        }
    }
}
