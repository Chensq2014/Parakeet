using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Parakeet.Net.Dtos;
using Parakeet.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Parakeet.Net
{
    /// <summary>
    /// 泛型基类AppService接口 完成所有公共接口
    /// </summary>
    /// <typeparam name="TEntity">实体类</typeparam>
    /// <typeparam name="TPrimaryKey">实体类主键类型</typeparam>
    public interface IBaseNetAppService<TEntity, TPrimaryKey>
        where TEntity : Entity<TPrimaryKey> //where TPrimaryKey : struct
    {
        #region 查询扩展
        /// <summary>
        /// 获取实体BaseRepository
        /// </summary>
        /// <returns></returns>
        INetRepository<TEntity, TPrimaryKey> GetBaseRepository();

        /// <summary>
        /// 获取实体Queryable
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<TEntity>> GetAll();

        /// <summary>
        /// 表达式目录树filter参数过滤数据
        /// </summary>
        /// <param name="filter">表达式目录树</param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> Get(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="input">实体主键输入类</param>
        /// <returns></returns>
        Task<TEntity> GetByPrimaryKey(InputIdDto<TPrimaryKey> input);

        /// <summary>
        /// 默认GetGet 获取GridData 提供给devExtreme
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions);

        /// <summary>
        /// 获取GridData Post
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<LoadResult> GridData(LoadOptionInputDto input);

        /// <summary>
        /// 按创建时间降序直接获取全部数据 泛型
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<TEntityDto>> GridDto<TEntityDto>() where TEntityDto : BaseDto;

        /// <summary>
        ///  获取列表数据(分页) 泛型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IPagedResult<TEntityDto>> GetPagedResult<TEntityDto>(PagedInputDto input) where TEntityDto : BaseDto;
        #endregion

        #region 插入扩展

        /// <summary>
        /// 添加实体 提供给devExtreme 前端控件验证实体
        /// </summary>
        /// <returns></returns>
        [Description("添加实体")]
        Task<TPrimaryKey> InsertInsert();

        /// <summary>
        /// 添加实体 提供给devExtreme 前端控件验证实体
        /// </summary>
        /// <returns></returns>
        [Description("添加实体")]
        Task<TEntityDto> InsertWithDto<TEntityDto>() where TEntityDto : BaseDto;

        /// <summary>
        /// 添加实体 提供给devExtreme 前端控件一次性提交多个实体
        /// 注意控件默认工作方式并不是这样工作的
        /// </summary>
        /// <returns></returns>
        [Description("添加实体")]
        Task BulkInsert();

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entities">实体集</param>
        /// <returns></returns>
        [Description("批量添加数据")]
        Task BulkInsertAsync(IList<TEntity> entities);
        #endregion

        #region 更新扩展

        /// <summary>
        /// UpdateUpdate修改实体 提供给devExtreme,前端控件验证实体
        /// </summary>
        /// <returns></returns>
        [Description("修改实体")] 
        Task UpdateUpdate();

        #endregion
        
        #region 删除扩展

        /// <summary>
        /// 根据主键Id删除实体 DeleteDelete 默认提供给devExtreme
        /// </summary>
        /// <returns></returns>
        [Description("删除实体")]
        Task DeleteDelete();

        /// <summary>
        /// 根据主键Id删除实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteByPrimaryKey(InputIdDto<TPrimaryKey> input);

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="input">实体主键集合</param>
        /// <returns></returns>
        [Description("批量删除实体")]
        Task BulkDeleteByPrimaryKeies(InputIdsDto<TPrimaryKey> input);

        /// <summary>
        /// 批量删除实体集
        /// </summary>
        /// <param name="entities">实体集</param>
        /// <returns></returns>
        [Description("批量删除实体集")]
        Task BulkDeleteAsync(IList<TEntity> entities);

        #endregion

        #region 重复性验证 重新设计

        /// <summary>
        /// 单字段重复性验证
        /// </summary>
        /// <param name="input">单个字段选项</param>
        /// <returns></returns>
        Task<bool> CheckField(FieldCheckOptionInputDto<TPrimaryKey> input);

        /// <summary>
        /// 多字段重复性验证，dxGrid在新建/编辑数据时 时时验证
        /// </summary>
        /// <param name="input">多字段dto选项</param>
        /// <returns></returns>
        Task<bool> CheckFields(FieldsCheckOptionInputDto<TPrimaryKey> input);

        #endregion

        #region 根据前端Grid传递的values json字符串反序列化为当前实体对象

        /// <summary>
        /// 从HttpContext Form获取values对象字符串转为泛型实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        TEntity GetValues<TEntity>();

        /// <summary>
        /// 根据Form values对象字符串转泛型对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        List<TEntity> GetItemValues<TEntity>();

        /// <summary>
        /// 根据values对象字符串转泛型对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="valueString"></param>
        /// <returns></returns>
        TEntity GetValues<TEntity>(string valueString);

        /// <summary>
        /// 从httpcontext Form获取values对象字符串
        /// </summary>
        /// <returns></returns>
        string GetValuesString();

        /// <summary>
        /// 从httpcontext Query获取values对象字符串
        /// </summary>
        /// <returns></returns>
        string GetValuesQueryString();
        /// <summary>
        /// 从httpcontext Query获取input对象字符串
        /// </summary>
        /// <returns></returns>
        string GetInputQueryString();

        /// <summary>
        /// 获取主键string转int/guid 注意TPrimaryKey非int?与guid？
        /// </summary>
        /// <param name="key">主键字符串</param>
        /// <returns>主键类型值</returns>
        TPrimaryKey GetTPrimaryKey(string key = "");

        /// <summary>
        /// 获取key字符串
        /// </summary>
        /// <returns></returns>
        string GetKeyString();

        /// <summary>
        /// 获取Id
        /// </summary>
        /// <returns></returns>
        TPrimaryKey GetPostId();

        /// <summary>
        /// 获取Request.Query["id"]
        /// </summary>
        /// <returns></returns>
        TPrimaryKey GetQueryId();

        #endregion
    }

    /// <summary>
    /// 基类AppService接口
    /// </summary>
    /// <typeparam name="TEntity">实体类</typeparam>
    public interface IBaseNetAppService<TEntity> : IBaseNetAppService<TEntity, Guid>
        where TEntity : Entity<Guid>
    {

    }
}
