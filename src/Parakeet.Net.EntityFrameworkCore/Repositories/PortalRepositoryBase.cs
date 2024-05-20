using Common;
using Common.Cache;
using Common.CustomAttributes;
using Common.Dtos;
using Common.Entities;
using Common.Extensions;
using Common.Helpers;
using Common.Interfaces;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Parakeet.Net.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Parakeet.Net.Repositories
{
    /// <summary>
    /// Base class for custom repositories of the application.可以在这里扩展批量插入 
    /// 需要在模块中依赖注入泛型仓储来扩展默认仓储没有的实现，泛型接口是独立的，并不影响默认仓储
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public class PortalRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepository<PortalDbContext, TEntity, TPrimaryKey>
        , IPortalRepository<TEntity, TPrimaryKey>
        where TEntity : EntityBase<TPrimaryKey>
    {
        protected PortalRepositoryBase(IDbContextProvider<PortalDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        // 添加所有存储库的通用方法
        // Add your common methods for all repositories

        #region SqlServer PostgreSQL Mysql Bulk
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task BulkInsertAsync(IList<TEntity> entities)
        {
            await (await GetDbContextAsync()).BulkInsertAsync(entities);
        }

        /// <summary>
        ///  批量更新或插入
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task BulkInsertOrUpdateAsync(IList<TEntity> entities)
        {
            await (await GetDbContextAsync()).BulkInsertOrUpdateAsync(entities);
        }

        /// <summary>
        ///  批量更新或插入或删除
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task BulkInsertOrUpdateOrDeleteAsync(IList<TEntity> entities)
        {
            await (await GetDbContextAsync()).BulkInsertOrUpdateOrDeleteAsync(entities);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task BulkDeleteAsync(IList<TEntity> entities)
        {
            await (await GetDbContextAsync()).BulkDeleteAsync(entities);
        }

        #endregion


    }

    /// <summary>
    /// Base class for custom repositories of the application.
    /// This is a shortcut of <see cref="PortalRepositoryBase{TEntity,TPrimaryKey}"/> for <see cref="Guid"/> primary key.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>

    //[Dependency(ReplaceServices = true)]
    public class PortalRepositoryBase<TEntity> : PortalRepositoryBase<TEntity, Guid>
        , IPortalRepository<TEntity>//IRepository<TEntity>//
        where TEntity : EntityBase
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private readonly ICacheService _cacheTypePropertyService;

        /// <summary>
        /// IServiceProvider负责提供实例 (IServiceCollection(context.Services)负责注册)
        /// </summary>
        //private readonly IServiceProvider _serviceProvider;
        public PortalRepositoryBase(
            //IServiceProvider serviceProvider,
            ICacheService cacheTypePropertyService,
            IDbContextProvider<PortalDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
            _cacheTypePropertyService = cacheTypePropertyService;
            //_serviceProvider = serviceProvider;
            //var service=_serviceProvider.GetRequiredService(typeof(CacheService));
        }

        // Do not add any method here, add to the class above (since this inherits it)!!!

        #region sql参数化 cqrs 的一个解决方案
        /// <summary>
        /// 实体集除公共属性与字段外的insert sql参数化语句 读写分离--写
        /// 1、默认的dbcontext 链接读数据库的字符串为 "读"数据字符串。
        /// 2、在框架里面写一个 批量插入，更新，和删除的sql操作，数据库连接字符串读取配置文件里面的"写"数据库字符串。 
        /// </summary>
        /// <param name="entities">插入数据集合</param>
        /// <returns></returns>
        public async Task<bool> BulkSqlInsert(IList<TEntity> entities)
        {
            if (entities.Any())
            {
                var type = typeof(TEntity);
                if (type.IsDefined(typeof(TableAttribute), true))
                {
                    var sql = new StringBuilder();
                    var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
                    var propInfos = await _cacheTypePropertyService.GetCachePropertyInfos(type);//CacheTypeProperties<TEntity>.GetPropertyInfos(type);//redis缓存实体属性
                    var propNames = await _cacheTypePropertyService.GetCachePropNames(type);//CacheTypeProperties<TEntity>.GetPropNames(type);//redis缓存数据表列名
                    var sqlParameterRows = new Dictionary<string, SqlParameter[]>();//每行sql都准备为一个单独的sqlparameter
                    var sqlParameters = new List<SqlParameter>();
                    var parameterIndex = 0;//单行sqlparameter 参数标识不重复
                    foreach (var entity in entities)
                    {
                        SqlInsertRow(propInfos, entity, sqlParameters, tableName, propNames, sqlParameterRows, sql, ref parameterIndex);
                    }
                    #region 插入多条数据 拼接一条sql直接参数化执行....
                    //await ExecuteSqlWithParameterAsync(new KeyValuePair<string, SqlParameter[]>(sql.ToString(), sqlParameters.ToArray()));
                    //return true;
                    #endregion

                    #region 插入多条数据,一次连接执行多个command 每行insert sql参数化命令 (读写分离)
                    return await ExecuteSqlWithParameters(sqlParameterRows, async (commandList) =>
                    {
                        try
                        {
                            foreach (var sqlCommand in commandList)
                            {
                                await sqlCommand.ExecuteNonQueryAsync();
                            }
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    });
                    #endregion
                }
            }
            return false;
        }

        /// <summary>
        /// 单个实体sql插入
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<bool> SqlInsert(TEntity entity)
        {
            var type = entity.GetType();//var type = typeof(TEntity);
            var sql = new StringBuilder();
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
            var propInfos = await _cacheTypePropertyService.GetCachePropertyInfos(type);//CacheTypeProperties<TEntity>.GetPropertyInfos(type);//redis缓存实体属性
            var propNames = await _cacheTypePropertyService.GetCachePropNames(type);//CacheTypeProperties<TEntity>.GetPropNames(type);//redis缓存数据表列名
            var sqlParameterRows = new Dictionary<string, SqlParameter[]>();//每行sql都准备为一个单独的sqlparameter
            var parameterIndex = 0;//单行sqlparameter 参数标识不重复
            var sqlParameters = new List<SqlParameter>();
            //typeof(EntityBase).IsAssignableFrom(type))//type.IsDefined(typeof(TableAttribute), true))
            if (type.IsAssignableTo(typeof(EntityBase)))
            {
                SqlInsertRow(propInfos, entity, sqlParameters, tableName, propNames, sqlParameterRows, sql, ref parameterIndex);

                #region 插入多条数据 也可以拼接一条sql直接参数化执行....
                //await ExecuteSqlWithParameterAsync(new KeyValuePair<string, SqlParameter[]>(sql.ToString(), sqlParameters.ToArray()));
                //return true;
                #endregion

                #region 插入多条数据,一次连接执行多个command 每行insert sql参数化命令 (读写分离)
                return await ExecuteSqlWithParameters(sqlParameterRows, async (commandList) =>
                {
                    try
                    {
                        foreach (var sqlCommand in commandList)
                        {
                            await sqlCommand.ExecuteNonQueryAsync();
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Information(ex.Message);
                        return false;
                    }
                });
                #endregion
            }
            return false;
        }

         
        ///// <summary>
        ///// 测试多个数据库sql插入
        ///// </summary>
        ///// <param name="entity">实体</param>
        ///// <returns></returns>
        //public async Task<bool> WriteSqlInsert(TEntity entity)
        //{
        //    var type = entity.GetType();//var type = typeof(TEntity);
        //    var sql = new StringBuilder();
        //    var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
        //    var propInfos = await _cacheTypePropertyService.GetCachePropertyInfos(type);//CacheTypeProperties<TEntity>.GetPropertyInfos(type);//redis缓存实体属性
        //    var propNames = await _cacheTypePropertyService.GetCachePropNames(type);//CacheTypeProperties<TEntity>.GetPropNames(type);//redis缓存数据表列名
        //    var sqlParameterRows = new Dictionary<string, SqlParameter[]>();//每行sql都准备为一个单独的sqlparameter
        //    var parameterIndex = 0;//单行sqlparameter 参数标识不重复
        //    var sqlParameters = new List<SqlParameter>();
        //    //typeof(BaseEntity).IsAssignableFrom(type))//type.IsDefined(typeof(TableAttribute), true))
        //    if (type.IsAssignableTo(typeof(BaseEntity)))
        //    {
        //        SqlInsertRow(propInfos, entity, sqlParameters, tableName, propNames, sqlParameterRows, sql, ref parameterIndex);

        //        #region 插入多条数据 也可以拼接一条sql直接参数化执行....
        //        //await ExecuteSqlWithParameterAsync(new KeyValuePair<string, SqlParameter[]>(sql.ToString(), sqlParameters.ToArray()));
        //        //return true;
        //        #endregion

        //        #region 插入多条数据,一次连接执行多个command 每行insert sql参数化命令 (读写分离)
        //        return await WriteExecuteSqlWithParameters(sqlParameterRows, async (commandList) =>
        //        {
        //            try
        //            {
        //                foreach (var sqlCommand in commandList)
        //                {
        //                    await sqlCommand.ExecuteNonQueryAsync();
        //                }
        //                return true;
        //            }
        //            catch (Exception ex)
        //            {
        //                Log.Logger.Information(ex.Message);
        //                return false;
        //            }
        //        });
        //        #endregion
        //    }
        //    return false;
        //}


        /// <summary>
        /// 单行sql/sqlparameter
        /// </summary>
        /// <param name="propInfos">实体属性</param>
        /// <param name="entity">实体</param>
        /// <param name="sqlParameters">拼接sql需要的所有参数集合</param>
        /// <param name="tableName">实体表名</param>
        /// <param name="propNames">实体属性列名</param>
        /// <param name="sqlParameterRows">字典：每行sql都准备一个单独的sqlparameter</param>
        /// <param name="sql">拼接sql对象</param>
        /// <param name="parameterIndex">单行sqlparameter 参数标识不重复</param>
        /// <returns></returns>
        private Dictionary<string, SqlParameter[]> SqlInsertRow(List<PropertyInfo> propInfos, TEntity entity, List<SqlParameter> sqlParameters, string tableName,
            List<string> propNames, Dictionary<string, SqlParameter[]> sqlParameterRows, StringBuilder sql, ref int parameterIndex)
        {
            var rowParameters = new List<SqlParameter>();
            foreach (var prop in propInfos)
            {
                #region 列信息：判断是否枚举/可空枚举类型/其它类型 并给当前列赋值,组装sql

                var columValue = prop.PropertyType.IsEnum
                    ? $"{(int)Enum.Parse(prop.PropertyType, prop.GetValue(entity).ToString())}"
                    : prop.PropertyType.IsValueType
                      && prop.PropertyType.IsGenericType
                      && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                      && prop.PropertyType.GetGenericArguments()[0].IsEnum
                        ? $"{(prop.GetValue(entity) == null ? "NULL" : Convert.ToInt32(prop.GetValue(entity)).ToString())}"
                        : $"'{prop.GetValue(entity) ?? DBNull.Value}'";
                var columnParameter = new SqlParameter($"@{prop.Name}_{parameterIndex.ToString()}", columValue);
                if (prop.Name.EndsWith("Id"))
                {
                    columValue = columValue.Replace("'","");
                    if (columValue.HasValue())
                    {
                        //columnParameter.SqlDbType = SqlDbType.UniqueIdentifier;
                        //columnParameter.DbType = DbType.Guid;
                        columnParameter.Value = Guid.Parse(columValue);
                    }
                    else
                    {
                        columnParameter.SqlValue = DBNull.Value;
                    }
                }
                rowParameters.Add(columnParameter);
                parameterIndex++;

                #endregion
            }

            #region 数据库的Id和CreationTime这两个必填参数也必须使用参数化插入
            rowParameters.Add(new SqlParameter
            {
                ParameterName = $"@Id_{parameterIndex++.ToString()}",
                SqlDbType = SqlDbType.UniqueIdentifier,
                DbType = DbType.Guid,
                Value = Guid.NewGuid(),
            });
            rowParameters.Add(new SqlParameter
            {
                ParameterName = $"@CreationTime_{parameterIndex.ToString()}",
                SqlDbType = SqlDbType.DateTime,
                DbType = DbType.DateTime,
                Value = DateTime.Now,
            });
            #endregion
            sqlParameters.AddRange(rowParameters);// 数据库UniqueIdentifier转为字符串：CAST(@Id AS VARCHAR(36))
            var parameterSql = $@"insert into [{CommonConsts.DefaultDbSchema}].[{CommonConsts.DefaultDbTablePrefix}{tableName}s] ({string.Join(",", propNames.Select(name => $"[{name}]"))},[Id],[CreationTime]) values ({string.Join(",", rowParameters.Select(m => m.ParameterName))})";
            sqlParameterRows.Add(parameterSql, rowParameters.ToArray());
            sql.Append(parameterSql); //sql.AppendLine(parameterSql);//拼接一条参数化sql
            return sqlParameterRows;
        }

        /// <summary> 
        /// 更新操作的Sql语句 读写分离--写
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> SqlUpdate<TEntity>(KeyValueDto<Guid, string> input)
        {
            var type = typeof(TEntity);
            var entity = Newtonsoft.Json.JsonConvert.DeserializeObject<TEntity>(input.Value);//JObject
            var setSqlParameter = string.Join(",", GetPropertiesFromJson(type, input.Value).Select(m => $@"{m.GetColumnName()}=@{m.Name}"));
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
            var sql = $"UPDATE [{CommonConsts.DefaultDbSchema}].[{CommonConsts.DefaultDbTablePrefix}{tableName}s] SET {setSqlParameter} WHERE Id=@Id;";

            var paraArray = GetPropertiesFromJson(type, input.Value)
                .Select(prop => new SqlParameter($"@{prop.Name}",
                    prop.PropertyType.IsEnum
                        ? $"{(int)Enum.Parse(prop.PropertyType, prop.GetValue(entity).ToString())}"
                        : prop.PropertyType.IsValueType
                          && prop.PropertyType.IsGenericType
                          && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                          && prop.PropertyType.GetGenericArguments()[0].IsEnum
                            ? $"{(prop.GetValue(entity) == null ? "NULL" : Convert.ToInt32(prop.GetValue(entity)).ToString())}"
                            : $"'{prop.GetValue(entity) ?? DBNull.Value}'"))
                .Append(new SqlParameter { ParameterName = "@Id", DbType = DbType.Guid, Value = input.Id }).ToArray();

            await ExecuteSqlWithParameterAsync(new KeyValuePair<string, SqlParameter[]>(sql, paraArray));

            return true;
        }

        /// <summary>
        /// 就是通过json字符串找出更新的字段 排除Id
        /// </summary>
        /// <param name="type"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> GetPropertiesFromJson(Type type, string json)
        {
            return type.GetProperties().Where(p => p.Name != "Id" && (json.Contains($@"'{p.Name}':") || json.Contains($@"\{p.Name}\:")));
        }

        /// <summary>
        /// 删除单条实体的Sql参数化语句 读写分离--写
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> SqlDelete<TEntity>(InputIdDto input)
        {
            var type = typeof(TEntity);
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
            var sql = $"Delete From [{CommonConsts.DefaultDbSchema}].[{CommonConsts.DefaultDbTablePrefix}{tableName}s] WHERE Id=@Id;";
            var parameter = new SqlParameter("@Id", input.Id.ToString());
            await ExecuteSqlWithParameterAsync(new KeyValuePair<string, SqlParameter[]>(sql, new[] { parameter }));
            return true;
        }

        /// <summary>
        /// 删除多条实体的Sql参数化语句 读写分离--写
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> BulkSqlDelete<TEntity>(InputIdsDto input)
        {
            var sql = new StringBuilder();
            var type = typeof(TEntity);
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
            var parameters = new List<SqlParameter>();
            foreach (var id in input.Ids)
            {
                sql.Append($"Delete From [{CommonConsts.DefaultDbSchema}].[{CommonConsts.DefaultDbTablePrefix}{tableName}s] WHERE Id=@Id_{parameters.Count};");
                var parameter = new SqlParameter($"@Id_{parameters.Count}", id.ToString());
                parameters.Add(parameter);
            }
            await ExecuteSqlWithParameterAsync(new KeyValuePair<string, SqlParameter[]>(sql.ToString(), parameters.ToArray()));
            return true;
        }

        /// <summary>
        /// 根据id查询实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<TEntity> SqlQuery<TEntity>(InputIdDto input) where TEntity : EntityBase, new()
        {
            var type = typeof(TEntity);
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
            var propInfos = await _cacheTypePropertyService.GetCachePropertyInfos(type);
            var columnsString = string.Join(",", propInfos.Select(p => $"[{p.GetColumnName()}]"));
            var sql = $@"Select [Id],{columnsString} From [{CommonConsts.DefaultDbSchema}].[{CommonConsts.DefaultDbTablePrefix}{tableName}s] Where Id=@Id;";
            var parameters = new List<SqlParameter> { new SqlParameter($"@Id", input.Id.ToString()) };
            return await ExecuteQuary(new KeyValuePair<string, SqlParameter[]>(sql, parameters.ToArray()), (command) =>
            {
                var reader = command.ExecuteReader();
                var entity = new TEntity();
                while (reader.Read())//一行一行的读sql查询出来的结果
                {
                    foreach (var prop in type.GetProperties())
                    {
                        var propName = prop.GetColumnName();
                        prop.SetValue(entity, reader[propName] is DBNull
                            ? null//可空类型  设置成null而不是数据库查询的值
                                  //: prop.PropertyType.IsEnum
                                  //  || (prop.PropertyType.IsValueType
                                  //   && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                                  //   && prop.PropertyType.GetGenericArguments()[0].IsEnum)
                                  //    ? Enum.ToObject(prop.PropertyType, reader[propName])
                                : reader[propName]);
                    }
                }
                return entity;
            });
        }

        /// <summary>
        /// 根据ids查询实体集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<IList<TEntity>> SqlQueryList<TEntity>(InputIdsDto input) where TEntity : EntityBase, new()
        {
            var type = typeof(TEntity);
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
            var propInfos = await _cacheTypePropertyService.GetCachePropertyInfos(type);
            var columnsString = string.Join(",", propInfos.Select(p => $"[{p.GetColumnName()}]"));
            var sql = $@"exec(Select [Id],{columnsString} From [{CommonConsts.DefaultDbSchema}].[{CommonConsts.DefaultDbTablePrefix}{tableName}s] Where cast([Id] as varchar(36)) in (@Ids));";
            var parameters = new List<SqlParameter> { new SqlParameter($"@Ids", string.Join(",", input.Ids.Select(m => m.ToString()))) };
            return await ExecuteQuaryList(new KeyValuePair<string, SqlParameter[]>(sql, parameters.ToArray()), (command) =>
            {
                var entities = new List<TEntity>();
                var reader = command.ExecuteReader();
                while (reader.Read())//一行一行的读sql查询出来的结果
                {
                    TEntity entity = new TEntity();
                    foreach (var prop in type.GetProperties())
                    {
                        var propName = prop.GetColumnName();
                        prop.SetValue(entity, reader[propName] is DBNull
                            ? null//可空类型  设置成null而不是数据库查询的值
                                  //: prop.PropertyType.IsEnum
                                  //  || (prop.PropertyType.IsValueType
                                  //   && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                                  //   && prop.PropertyType.GetGenericArguments()[0].IsEnum)
                                  //    ? Enum.ToObject(prop.PropertyType, reader[propName])
                                : reader[propName]);
                    }
                    entities.Add(entity);
                }
                return entities;
            });
        }


        /// <summary>
        /// 根据表达式目录树转sql参数化查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<IList<TEntity>> SqlQueryByCondition<TEntity>(Expression<Func<TEntity, bool>> func) where TEntity : EntityBase, new()
        {
            var type = typeof(TEntity);
            var propInfos = await _cacheTypePropertyService.GetCachePropertyInfos(type);
            string columnsString = string.Join(",", propInfos.Select(p => $"[{p.GetMappingName()}]"));
            string where = func.ToWhere<TEntity>(out List<SqlParameter> parameters);
            string sql = $"SELECT [Id],{columnsString} FROM [{CommonConsts.DefaultDbSchema}].[{CommonConsts.DefaultDbTablePrefix}{type.GetMappingName()}s] WHERE {where}";
            return await ExecuteQuaryList(new KeyValuePair<string, SqlParameter[]>(sql, parameters.ToArray()), (command) =>
            {
                var entities = new List<TEntity>();
                var reader = command.ExecuteReader();
                while (reader.Read())//一行一行的读sql查询出来的结果
                {
                    var entity = new TEntity();
                    foreach (var prop in propInfos)
                    {
                        var propName = prop.GetColumnName();
                        prop.SetValue(entity, reader[propName] is DBNull
                            ? null//可空类型  设置成null而不是数据库查询的值
                                  //: prop.PropertyType.IsEnum
                                  //  || (prop.PropertyType.IsValueType
                                  //   && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                                  //   && prop.PropertyType.GetGenericArguments()[0].IsEnum)
                                  //    ? Enum.ToObject(prop.PropertyType, reader[propName])
                            : reader[propName]);
                    }
                    entities.Add(entity);
                }
                return entities;
            });
        }

        /// <summary>
        /// 异步直接执行单条sql带parameters语句  可做读写分离-默认dbcontext上的连接字符串
        /// </summary>
        /// <param name="sqlParameterPair">sql参数化字符串/参数值</param>
        /// <returns>task</returns>
        private async Task ExecuteSqlWithParameterAsync(KeyValuePair<string, SqlParameter[]> sqlParameterPair)
        {
            var transaction = await (await GetDbContextAsync()).Database.BeginTransactionAsync();
            var cancel = new Task(async () =>
            {
                await transaction.RollbackAsync();
                throw new Exception("执行sql失败！");
            });
            var action = string.IsNullOrWhiteSpace(sqlParameterPair.Key)
                ? new Task(() => { })
                : (await GetDbContextAsync()).Database.ExecuteSqlRawAsync(sqlParameterPair.Key, sqlParameterPair.Value, cancel);//sqlParameterPair.Value.Select(m => m.Value)
            await action;
            await transaction.CommitAsync();//action委托逻辑完毕之后再执行commit
        }

        /// <summary>
        /// 参数化执行多条sql 多个SqlCommand 读写分离--写操作的都通过这个方法执行参数化的sql
        /// </summary>
        /// <param name="sqlParametersDic">参数化sql与参数对应的值的集合</param>
        /// <param name="func">上端传入的委托逻辑(下端需要传入一个List《SqlCommand> cmdList对象)</param>
        /// <returns>一个整数</returns>
        private async Task<bool> ExecuteSqlWithParameters(Dictionary<string, SqlParameter[]> sqlParametersDic, Func<List<SqlCommand>, Task<bool>> func)
        {
            //读写分离:默认为写数据库连接字符串
            var connectionString = EnvironmentHelper.DatabaseConnectionString;//CustomConfigurationManager.GetConnectionString(SqlConnectionType.Write);
            using (var conn = new SqlConnection(connectionString))
            {
                Log.Logger.Information($"写WriteConnectionString：{connectionString}");
                conn.Open(); //一次连接执行多个command sql参数化命令 外层统一使用sql数据库事务
                var transaction = conn.BeginTransaction();
                try
                {
                    var commandList = new List<SqlCommand>();
                    foreach (var keyValue in sqlParametersDic)
                    {
                        var command = new SqlCommand(keyValue.Key, conn){ Transaction = transaction };
                        command.Parameters.AddRange(keyValue.Value);
                        commandList.Add(command);
                        Log.Logger.Information($"添加commandList：{command.CommandText}");
                    }
                    var sucess = await func(commandList);
                    await transaction.CommitAsync();
                    return sucess;
                }
                catch (Exception ex)
                {
                    Log.Logger.Information(ex.Message);
                    Log.Logger.Information("正在回滚更新...");
                    transaction.Rollback();
                    throw new UserFriendlyException("执行sql命令失败!");
                }
            }
        }

        ///// <summary>
        ///// 测试第二写库 无事务
        ///// </summary>
        ///// <param name="sqlParametersDic">参数化sql与参数对应的值的集合</param>
        ///// <param name="func">上端传入的委托逻辑(下端需要传入一个List《SqlCommand> cmdList对象)</param>
        ///// <returns>一个整数</returns>
        //private async Task<bool> WriteExecuteSqlWithParameters(Dictionary<string, SqlParameter[]> sqlParametersDic, Func<List<SqlCommand>, Task<bool>> func)
        //{
        //    //读写分离:默认为写数据库连接字符串
        //    var connectionString = "Server=.;Database=ParakeetNew;Trusted_Connection=True;MultipleActiveResultSets=true;";
        //    // CustomConfigurationManager.GetConnectionString(SqlConnectionType.Read);
        //    using (var conn = new SqlConnection(connectionString))
        //    {
        //        Log.Logger.Information($"写WriteConnectionString：{connectionString}");
        //        conn.Open(); //一次连接执行多个command sql参数化命令 外层统一使用sql数据库事务
        //        var commandList = new List<SqlCommand>();
        //        foreach (var keyValue in sqlParametersDic)
        //        {
        //            var command = new SqlCommand(keyValue.Key, conn);//{ Transaction = transaction };
        //            command.Parameters.AddRange(keyValue.Value);
        //            commandList.Add(command);
        //            Log.Logger.Information($"添加commandList：{command.CommandText}");
        //        }
        //        var sucess = await func(commandList);
        //        return sucess;
        //    }
        //}



        /// <summary>
        /// 根据Id查询实体委托
        /// </summary>
        /// <typeparam name="TEntity">类型</typeparam>
        /// <param name="sqlParameterPair">sql参数化字符串/参数值</param>
        /// <param name="func">上端传入委托逻辑(需要传入一个SqlCommand cmd对象，返回TEntity对象)</param>
        /// <returns>TEntity对象</returns>
        private async Task<TEntity> ExecuteQuary<TEntity>(KeyValuePair<string, SqlParameter[]> sqlParameterPair, Func<SqlCommand, TEntity> func)
        {
            //读写分离:读数据库连接字符串
            var connectionString = EnvironmentHelper.DatabaseConnectionString;//CustomConfigurationManager.GetConnectionString(SqlConnectionType.Read);
            using (var conn = new SqlConnection(connectionString))
            {
                Log.Logger.Information($"执行ReadConnectionString：{connectionString}");
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    var command = new SqlCommand(sqlParameterPair.Key, conn) { Transaction = transaction };
                    command.Parameters.AddRange(sqlParameterPair.Value);
                    Log.Logger.Information($"执行command：{command.CommandText}");
                    var entity = func(command);
                    await transaction.CommitAsync();
                    return entity;
                }
                catch (Exception ex)
                {
                    Log.Logger.Information(ex.Message);
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 查询表中所有实体委托
        /// </summary>
        /// <typeparam name="TEntity">类型</typeparam>
        /// <param name="sqlParameterPair">查询sql参数化字符串/参数值</param>
        /// <param name="func">上端传入委托逻辑(需要传入一个SqlCommand cmd对象返回List TEntity>对象集合)</param>
        /// <returns>List TEntity>对象集合</returns>
        private async Task<List<TEntity>> ExecuteQuaryList<TEntity>(KeyValuePair<string, SqlParameter[]> sqlParameterPair, Func<SqlCommand, List<TEntity>> func)
        {
            //读写分离:读数据库连接字符串
            var connectionString = EnvironmentHelper.DatabaseConnectionString;//CustomConfigurationManager.GetConnectionString(SqlConnectionType.Read);
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    var command = new SqlCommand(sqlParameterPair.Key, conn){ Transaction = transaction };
                    command.Parameters.AddRange(sqlParameterPair.Value);
                    var list = func(command);
                    await transaction.CommitAsync();
                    return list;
                }
                catch (Exception ex)
                {
                    Log.Logger.Information(ex.Message);
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 写数据库 同一个连接多次操作之后 使用数据库事务统一SaveChange  上面func的返回值为void的特例
        /// </summary>
        public void SaveChange(List<SqlCommand> commandList)
        {
            var connectionString = EnvironmentHelper.DatabaseConnectionString;//CustomConfigurationManager.GetConnectionString(SqlConnectionType.Write);
            if (commandList.Count > 0)
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var command in commandList)
                            {
                                command.Connection = conn;
                                command.Transaction = trans;
                                command.ExecuteNonQuery();
                            }
                            trans.Commit();
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            throw;
                        }
                        finally
                        {
                            commandList?.Clear();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
