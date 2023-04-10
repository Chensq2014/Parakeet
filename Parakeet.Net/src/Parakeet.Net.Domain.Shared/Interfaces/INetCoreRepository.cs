using Parakeet.Net.Dtos;
using Parakeet.Net.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Parakeet.Net.Interfaces
{
    /// <summary>
    /// Repository扩展 支持批量插入 项目中就直接使用INetCoreRepository
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    /// <typeparam name="TPrimaryKey">实体主键类型</typeparam>
    public interface INetCoreRepository<TEntity, TPrimaryKey>
        : IRepository<TEntity, TPrimaryKey>
        where TEntity : BaseEntity<TPrimaryKey>//Entity<TPrimaryKey>//
    {
        #region SqlServer Bulk

        /// <summary>
        /// sqlserver 批量插入
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task BulkInsertAsync(IList<TEntity> entities);

        /// <summary>
        /// sqlserver 批量删除
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task BulkDeleteAsync(IList<TEntity> entities);

        #endregion

        #region PostgreSQL Bulk

        /// <summary>
        /// Messaia.Net.PostgreSQL.BulkExtensions 批量插入
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task PostgreBulkInsert(IEnumerable<TEntity> entities);

        #endregion
    }

    public interface INetCoreRepository<TEntity> : INetCoreRepository<TEntity, Guid>
        where TEntity : BaseEntity//Entity<Guid> //
    {
        #region sql参数化 cqrs 的一个解决方案
        //1、dbcontext 默认链接读数据库的字符串为 "读"数据字符串。
        //2、在框架里面写一个 批量插入，更新，和删除的sql操作，数据库连接字符串读取配置文件里面的"写"数据库字符串。 

        /// <summary>
        /// 实体集除公共属性与字段外的insert sql参数化语句 读写分离--写
        /// </summary>
        /// <param name="entities">插入数据集合</param>
        /// <returns></returns>
        Task<bool> BulkSqlInsert(IList<TEntity> entities);

        /// <summary>
        /// 单个实体sql插入
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<bool> SqlInsert(TEntity entity);

        ///// <summary>
        ///// 单个实体sql插入
        ///// </summary>
        ///// <param name="entity">实体</param>
        ///// <returns></returns>
        //Task<bool> WriteSqlInsert(TEntity entity);

        /// <summary> 
        /// 更新操作的Sql语句 读写分离--写
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> SqlUpdate<TEntity>(KeyValueDto<Guid, string> input);

        /// <summary>
        /// 删除单条实体的Sql参数化语句 读写分离--写
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> SqlDelete<TEntity>(InputIdDto input);

        /// <summary>
        /// 删除多条实体的Sql参数化语句 读写分离--写
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> BulkSqlDelete<TEntity>(InputIdsDto input);

        /// <summary>
        /// 根据id查询实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<TEntity> SqlQuery<TEntity>(InputIdDto input) where TEntity : BaseEntity, new();

        /// <summary>
        /// 根据表达式目录树转sql参数化查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<IList<TEntity>> SqlQueryByCondition<TEntity>(Expression<Func<TEntity, bool>> func)
            where TEntity : BaseEntity, new();

        /// <summary>
        /// 根据ids查询实体集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<IList<TEntity>> SqlQueryList<TEntity>(InputIdsDto input) where TEntity : BaseEntity, new();

        #endregion
    }

}
