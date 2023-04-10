using Volo.Abp.Uow;

namespace Parakeet.Net.Interfaces
{
    /// <summary>
    /// (原netcore操作单元接口)
    /// 事务及批量插入统一接口操作单元
    /// 提供获取及提交事务，一系列与数据库交互的方法，它可以对调用层公开，为了减少连库次数
    /// </summary>
    public interface ICustomUnitOfWork //:IUnitOfWork, IDependency//, IDisposable
    {
    //    #region  EF ORM IUnitOfWork 操作单元

    //    #region 获取DbSet

    //    DbSet<TEntity> GetEntities<TEntity>() where TEntity : BaseEntity;

    //    #endregion

    //    #region 附加/解除实体更改到上下文

    //    /// <summary>
    //    /// 附加实体更改到上下文
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entity"></param>
    //    /// <param name="state"></param>
    //    /// <returns></returns>
    //    TEntity AttachEntityToContext<TEntity>(TEntity entity, EntityState state = EntityState.Modified)
    //        where TEntity : BaseEntity;

    //    /// <summary>
    //    /// Detach 实体对象
    //    /// </summary>
    //    /// <param name="entity"></param>
    //    void Detach(object entity);
    //    #endregion

    //    #region 提交变更到数据库

    //    /// <summary>
    //    /// 提交变更
    //    /// </summary>
    //    /// <returns></returns>
    //    int Commit();

    //    /// <summary>
    //    /// 异步提交变更
    //    /// </summary>
    //    /// <returns></returns>
    //    Task<int> CommitAsync();
    //    #endregion

    //    #region 数据库连接对象

    //    /// <summary>
    //    /// 数据库连接对象
    //    /// </summary>
    //    /// <returns></returns>
    //    DbConnection GetConnection();

    //    #endregion

    //    #region 数据库及事务操作

    //    /// <summary>
    //    /// 获取当前数据库事务
    //    /// </summary>
    //    /// <returns></returns>
    //    DbTransaction GetTransaction();

    //    /// <summary>
    //    /// 提交当前事务
    //    /// </summary>
    //    void CommitTransaction();

    //    /// <summary>
    //    /// 开启新事务
    //    /// </summary>
    //    IDbContextTransaction BeginTransaction();

    //    Task<IDbContextTransaction> BeginTransactionAsync();
    //    /// <summary>
    //    /// 提交当前事务并开启新事务
    //    /// </summary>
    //    IDbContextTransaction BeginNewTransaction();

    //    Task<IDbContextTransaction> BeginNewTransactionAsync();
    //    /// <summary>
    //    /// 事务回滚
    //    /// </summary>
    //    void Rollback();
    //    #endregion

    //    #endregion

    //    #region EFCore 普通CRUD操作方法

    //    #region 基础查询

    //    /// <summary>
    //    /// 判断是否有数据
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <returns></returns>
    //    bool Any<TEntity>() where TEntity : BaseEntity;

    //    Task<bool> AnyAsync<TEntity>() where TEntity : BaseEntity;
    //    /// <summary>
    //    /// 根据条件判断是否有数据
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="filter"></param>
    //    /// <returns></returns>
    //    bool Any<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;

    //    Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;
    //    /// <summary>
    //    /// 条件判断是否存在
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="filter"></param>
    //    /// <returns></returns>
    //    bool IsExsit<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;

    //    Task<bool> IsExsitAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;
    //    /// <summary>
    //    /// 根据Id查询数据
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="id"></param>
    //    /// <returns></returns>
    //    TEntity Get<TEntity>(int id) where TEntity : BaseEntity;

    //    Task<TEntity> GetAsync<TEntity>(int id) where TEntity : BaseEntity;
    //    /// <summary>
    //    /// 根据条件过滤数据
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="filter"></param>
    //    /// <returns></returns>
    //    IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 异步根据条件过滤数据
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="filter"></param>
    //    /// <returns></returns>
    //    Task<IEnumerable<TEntity>> GetAsync<TEntity>(Expression<Func<TEntity, bool>> filter)
    //        where TEntity : BaseEntity;
    //    /// <summary>
    //    /// 查询所有
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <returns></returns>
    //    IQueryable<TEntity> GetAll<TEntity>() where TEntity : BaseEntity;

    //    TEntity First<TEntity>() where TEntity : BaseEntity;
    //    Task<TEntity> FirstAsync<TEntity>() where TEntity : BaseEntity;
    //    TEntity First<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;
    //    Task<TEntity> FirstAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;
    //    TEntity FirstOrDefault<TEntity>() where TEntity : BaseEntity;
    //    Task<TEntity> FirstOrDefaultAsync<TEntity>() where TEntity : BaseEntity;
    //    TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;
    //    Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;
    //    #endregion

    //    #region 基础插入

    //    void Insert<TEntity>(TEntity entity) where TEntity : BaseEntity;
    //    Task<int> InsertAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;
    //    void Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 批量插入
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entities"></param>
    //    /// <returns></returns>
    //    Task<int> InsertAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;
    //    #endregion

    //    #region 基础删除

    //    void Delete<TEntity>(int id) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 根据Id异步删除
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="id"></param>
    //    /// <returns></returns>
    //    Task DeleteAsync<TEntity>(int id) where TEntity : BaseEntity;
    //    void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 异步删除
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entity"></param>
    //    /// <returns></returns>
    //    Task DeleteAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;
    //    void Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 异步批量删除
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entities"></param>
    //    /// <returns></returns>
    //    Task DeleteAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;
    //    /// <summary>
    //    /// 根据条件异步删除
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="filter"></param>
    //    /// <returns>task</returns>
    //    Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;
    //    void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;

    //    void DeleteAll<TEntity>() where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 异步批量删除
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <returns>task</returns>
    //    Task DeleteAllAsync<TEntity>() where TEntity : BaseEntity;
    //    #endregion

    //    #endregion

    //    #region EFCore.BulkExtensions 批量操作 接口实现

    //    /// <summary>
    //    /// 批量插入
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entities"></param>
    //    void Insert<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 批量异步插入
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entities"></param>
    //    Task InsertAsync<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 批量插入或更新
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entities"></param>
    //    void InsertOrUpdate<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 异步批量插入或更新
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entities"></param>
    //    Task InsertOrUpdateAsync<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 批量增改删
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entities"></param>
    //    void InsertOrUpdateOrDelete<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 异步批量增改删
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entities"></param>
    //    Task InsertOrUpdateOrDeleteAsync<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 批量删除
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entities"></param>
    //    void Delete<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity;

    //    /// <summary>
    //    /// 异步批量删除
    //    /// </summary>
    //    /// <typeparam name="TEntity"></typeparam>
    //    /// <param name="entities"></param>
    //    Task DeleteAsync<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity;

    //    #endregion

    //    #region 执行sql sql参数化

    //    /// <summary>
    //    /// 直接执行sql语句，含事务
    //    /// </summary>
    //    /// <param name="sql">sql字符串</param>
    //    /// <param name="parameters">字符参数</param>
    //    /// <returns></returns>
    //    int ExecuteSql(string sql, params object[] parameters);

    //    /// <summary>
    //    /// 执行拼接完的sql
    //    /// </summary>
    //    /// <param name="sql"></param>
    //    /// <returns></returns>
    //    int ExecuteSql(StringBuilder sql = null);

    //    /// <summary>
    //    /// 异步直接执行sql语句，含事务
    //    /// </summary>
    //    /// <param name="sql">sql字符串</param>
    //    /// <param name="parameters">字符参数</param>
    //    /// <returns></returns>
    //    Task ExecuteSqlAsync(string sql, params object[] parameters);

    //    #endregion
    }
}
