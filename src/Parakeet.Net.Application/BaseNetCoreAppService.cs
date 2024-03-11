using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Parakeet.Net.LinqExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Dtos;
using Common.Entities;
using Common.Extensions;
using Common.Interfaces;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories;

namespace Parakeet.Net
{
    /// <summary>
    /// 基类AppService
    /// </summary>
    /// <typeparam name="TEntity">实体类</typeparam>
    /// <typeparam name="TPrimaryKey">实体类主键类型</typeparam>
    public abstract class BaseNetAppService<TEntity, TPrimaryKey> : CustomerAppService, IBaseNetAppService<TEntity, TPrimaryKey> where TEntity : BaseEntity<TPrimaryKey>
        //where TPrimaryKey : struct
    {
        public INetRepository<TEntity, TPrimaryKey> Repository;
        protected BaseNetAppService(INetRepository<TEntity, TPrimaryKey> repository)
        {
            Repository = repository;
        }

        #region 查询扩展

        /// <summary>
        /// 获取实体BaseRepository
        /// </summary>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public INetRepository<TEntity, TPrimaryKey> GetBaseRepository()
        {
            return Repository;
        }

        /// <summary>
        /// 获取实体Queryable
        /// </summary>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public async Task<IQueryable<TEntity>> GetAll()
        {
            return await Repository.GetQueryableAsync();
        }

        /// <summary>
        /// 表达式目录树filter参数过滤数据 仅提供内部调用 不生成api
        /// </summary>
        /// <param name="filter">表达式目录树</param>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public async Task<IQueryable<TEntity>> Get(Expression<Func<TEntity, bool>> filter)
        {
            var query = await GetAll();
            return query.Where(filter);
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="input">实体主键输入类</param>
        /// <returns></returns>
        [Description("根据主键获取实体")]
        public virtual async Task<TEntity> GetByPrimaryKey(InputIdDto<TPrimaryKey> input)
        {
            return await Repository.GetAsync(input.Id);
        }

        /// <summary>
        /// 默认GetGet 获取GridData 提供给devExtreme
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        public virtual async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
        {
            var result = await DataSourceLoader.LoadAsync((await GetAll()).AsNoTracking(), loadOptions);
            return result;
        }

        /// <summary>
        /// 获取GridData Post
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<LoadResult> GridData(LoadOptionInputDto input)
        {
            //DataSourceLoader.LoadAsync(BaseRepository.AsNoTracking().Where(m.ParentId==input.Id),input.LoadOptions);
            return await GetGet(input.LoadOptions);
        }

        /// <summary>
        /// 按创建时间降序直接获取全部数据 泛型
        /// </summary>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public virtual async Task<IQueryable<TEntityDto>> GridDto<TEntityDto>() where TEntityDto : BaseDto
        {
            var result = (await GetAll()).AsNoTracking()
                //.OrderByDescending(o => o.CreationTime)
                .ProjectTo<TEntityDto>(Configuration);
            return result;
        }

        /// <summary>
        ///  获取列表数据(分页) 泛型 ()
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<IPagedResult<TEntityDto>> GetPagedResult<TEntityDto>(PagedInputDto input) where TEntityDto : BaseDto
        {
            return await (await GetAll())
                .AsNoTracking()
                //.OrderBy(input.Sorting)
                .ProjectTo<TEntityDto>(Configuration)
                .ToPageResultAsync(input,input.FindTotalCount);
        }

        #endregion

        #region 插入扩展
        /// <summary>
        /// 添加实体 提供给devExtreme 前端控件验证实体
        /// </summary>
        /// <returns></returns>
        [Description("添加实体")]
        public virtual async Task<TPrimaryKey> InsertInsert()
        {
            var entity = GetValues<TEntity>(); 
            await Repository.InsertAsync(entity);
            return entity.Id;
        }

        /// <summary>
        /// 添加实体 提供给devExtreme 前端控件验证实体
        /// </summary>
        /// <returns></returns>
        [HttpPost("InsertInsertWithDto"),Description("添加实体")]
        public virtual async Task<TEntityDto> InsertWithDto<TEntityDto>() where TEntityDto : BaseDto
        {
            var entity = GetValues<TEntity>();
            await Repository.InsertAsync(entity);
            return ObjectMapper.Map<TEntity, TEntityDto>(entity);
        }

        /// <summary>
        /// 添加实体 提供给devExtreme 前端控件一次性提交多个实体
        /// 注意控件默认工作方式并不是这样工作的
        /// </summary>
        /// <returns></returns>
        [Description("添加实体")]
        public async Task BulkInsert()
        {
            var entities = GetItemValues<TEntity>();
            await BulkInsertAsync(entities);
        }

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entities">实体集</param>
        /// <returns></returns>
        [Description("批量添加数据")]
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public async Task BulkInsertAsync(IList<TEntity> entities)
        {
            await Repository.BulkInsertAsync(entities);
        }

        #endregion

        #region 更新扩展

        /// <summary>
        /// UpdateUpdate修改实体 提供给devExtreme,前端控件验证实体
        /// </summary>
        /// <returns></returns>
        [Description("修改实体")]
        public virtual async Task UpdateUpdate()
        {
            var entity = await GetByPrimaryKey(new InputIdDto<TPrimaryKey> { Id = GetTPrimaryKey() });
            JsonConvert.PopulateObject(GetValuesString(), entity);
            if (entity is IHasModificationTime)
            {
                entity.LastModificationTime = Clock.Now;
            }
        }

        #endregion

        #region 删除扩展
        /// <summary>
        /// 根据主键Id删除实体 DeleteDelete 默认提供给devExtreme
        /// </summary>
        /// <returns></returns>
        [HttpDelete, Description("删除实体")]
        public virtual async Task DeleteDelete()
        {
            var input = new InputIdDto<TPrimaryKey>() { Id = GetTPrimaryKey() };
            await DeleteByPrimaryKey(input);
            //await Repository.DeleteAsync(m => m.Id == input.Id);
        }

        /// <summary>
        /// 根据主键Id删除实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task DeleteByPrimaryKey(InputIdDto<TPrimaryKey> input)
        {
            await Repository.DeleteAsync(m => m.Id.Equals(input.Id));
            //await Repository.DeleteAsync(m => m.Id.ToString() == input.Id.ToString());
        }

        /// <summary>
        /// 根据Ids批量删除实体 sqlserver
        /// </summary>
        /// <param name="input">实体主键集合</param>
        /// <returns>IActionResult</returns>
        [Description("批量删除实体")]
        public virtual async Task BulkDeleteByPrimaryKeies(InputIdsDto<TPrimaryKey> input)
        {
            var entities = await (await GetAll())
                .Where(m => input.Ids.Contains(m.Id))
                .ToListAsync();
            await BulkDeleteAsync(entities);
        }

        /// <summary>
        /// 批量删除实体集 sqlserver
        /// </summary>
        /// <param name="entities">实体集</param>
        /// <returns></returns>
        [Description("批量删除实体集")]
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public virtual async Task BulkDeleteAsync(IList<TEntity> entities)
        {
            await Repository.BulkDeleteAsync(entities);
        }

        #endregion

        #region 重复性验证 重新设计

        /// <summary>
        /// 单字段重复性验证
        /// </summary>
        /// <param name="input">单个字段选项</param>
        /// <returns>bool</returns>
        [HttpPost]
        public async Task<bool> CheckField(FieldCheckOptionInputDto<TPrimaryKey> input)
        {
            var isRepeat = await Repository.AnyAsync();//如果没有数据，默认为false 表示没有重复
            if (isRepeat)
            {
                if (!input.Field.Equal)
                {
                    input.Field.Equal = true;
                }

                input.Field.Field = input.Field.Field.ToInitialCapitalization();//要把字符串数据都转换为大写
                var dynamicFieldExpression = ExpressionExtension.DynamicField<TEntity, TPrimaryKey>(input.Field);
                isRepeat = await (await GetAll()).Where(dynamicFieldExpression).AnyAsync();//是否重复
            }
            return isRepeat;
        }

        /// <summary>
        /// 多字段重复性验证，dxGrid在新建/编辑数据时 时时验证
        /// </summary>
        /// <param name="input">多字段dto选项</param>
        /// <returns>bool</returns>
        [HttpPost]
        public async Task<bool> CheckFields(FieldsCheckOptionInputDto<TPrimaryKey> input)
        {
            var query = await GetAll();
            var isRepeat = await query.AnyAsync();//如果没有数据，默认为false 前端返回true表示通过
            if (isRepeat)
            {
                foreach (var field in input.Fields)
                {
                    field.Field = field.Field.ToInitialCapitalization(); //要把字符串数据都转换为大写
                    query = query.Where(field.CheckLambda<TEntity, TPrimaryKey>());//构造表达式目录树
                }
                isRepeat = await query.AnyAsync();//根据条件验证是否重复
            }
            return isRepeat;
        }

        #endregion

        #region JsonResultExtension 可弃用，因为core返回数据直接会变为json解析

        /// <summary>
        /// 返回null json对象
        /// </summary>
        /// <returns></returns>
        protected JsonResult DJson()
        {
            return DJson("null");
        }

        /// <summary>
        /// 返回object json数据对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected JsonResult DJson(object data)
        {
            return new JsonResult(data);
        }

        /// <summary>
        /// 返回成功status和msg的对象消息
        /// </summary>
        /// <returns></returns>
        protected JsonResult SJson()
        {
            return RJson(true);
        }

        /// <summary>
        /// 返回成功status和msg的对象消息和Data对象数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected JsonResult SJson(object data)
        {
            return new JsonResult(new AjaxReturnMessage { Status = true, Data = data, Msg = "请求成功" });
        }

        /// <summary>
        /// 返回错误status和msg消息+Data对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected JsonResult EJson(object data)
        {
            return new JsonResult(new AjaxReturnMessage { Status = false, Data = data, Msg = "错误！" });
        }

        /// <summary>
        /// 提供给返回字符串专用
        /// </summary>
        /// <param name="status"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private JsonResult RJson(bool status, string msg = "成功！")
        {
            return new JsonResult(new AjaxReturnMessage { Status = status, Msg = msg });
        }

        #endregion

        #region 根据前端Grid传递的values json字符串反序列化为当前实体对象

        /// <summary>
        /// 从HttpContext Form获取values对象字符串转为泛型实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public TEntity GetValues<TEntity>()
        {
            var formData = ContextAccessor.HttpContext?.Request.Form["values"];
            var entity = JsonConvert.DeserializeObject<TEntity>(formData);
            return entity;
        }

        /// <summary>
        /// 根据Form values对象字符串转泛型对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public List<TEntity> GetItemValues<TEntity>()
        {
            var formData = ContextAccessor.HttpContext?.Request.Form["values"];
            var entities = JsonConvert.DeserializeObject<List<TEntity>>(formData);
            return entities;
        }

        /// <summary>
        /// 根据values对象字符串转泛型对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="valueString"></param>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public TEntity GetValues<TEntity>(string valueString)
        {
            var entity = JsonConvert.DeserializeObject<TEntity>(valueString);
            return entity;
        }

        /// <summary>
        /// 从httpcontext Form获取values对象字符串
        /// </summary>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public string GetValuesString()
        {
            var formData = ContextAccessor.HttpContext?.Request.Form["values"].ToString();
            //formData = formData.Replace(" ", "");//去掉所有空格
            return formData?.Trim();
        }

        /// <summary>
        /// 从httpcontext Query获取values对象字符串
        /// </summary>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public string GetValuesQueryString()
        {
            var formData = ContextAccessor.HttpContext?.Request.Query["values"].ToString();
            return formData?.Trim();//formData = formData.Replace(" ", "");//去掉所有空格
        }

        /// <summary>
        /// 从httpcontext Query获取input对象字符串
        /// </summary>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public string GetInputQueryString()
        {
            var formData = ContextAccessor.HttpContext?.Request.Query["input"].ToString();
            return formData?.Trim();//formData = formData.Replace(" ", "");//去掉所有空格
        }

        /// <summary>
        /// 获取主键string转int/guid 注意TPrimaryKey非int?与guid？
        /// </summary>
        /// <param name="key">主键字符串</param>
        /// <returns>主键类型值</returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public TPrimaryKey GetTPrimaryKey(string key = null)
        {
            key = key ?? GetRequestPrimarykeyString();
            if (string.IsNullOrWhiteSpace(key))
            {
                return default;
            }
            //怎样才能排除这个if判断？有没有一个struct类型互转的通用方法？
            if (typeof(TPrimaryKey) == typeof(Guid))
            {
                return (TPrimaryKey)Convert.ChangeType(Guid.Parse(key), typeof(TPrimaryKey));
            }
            return (TPrimaryKey)Convert.ChangeType(key, typeof(TPrimaryKey));
        }

        /// <summary>
        /// 获取key字符串
        /// </summary>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public string GetKeyString()
        {
            var formData = ContextAccessor.HttpContext?.Request.Form["key"];
            return formData;
        }

        /// <summary>
        /// 获取PostId
        /// </summary>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public TPrimaryKey GetPostId()
        {
            var context = ContextAccessor.HttpContext;
            var value = context.Request.Form["id"].Any() ? context.Request.Form["id"].ToString() : context.Request.Query["id"].ToString();
            return string.IsNullOrWhiteSpace(value)
                ? default(TPrimaryKey)
                : GetTPrimaryKey(value);
        }

        /// <summary>
        /// 获取Request.Query["id"]
        /// </summary>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public TPrimaryKey GetQueryId()
        {
            var value = ContextAccessor.HttpContext?.Request.Query["id"].ToString();
            return string.IsNullOrWhiteSpace(value)
                ? default(TPrimaryKey)
                : GetTPrimaryKey(value);
        }

        #endregion
    }

    /// <summary>
    /// 基类Appservices 
    /// </summary>
    /// <typeparam name="TEntity">实体类</typeparam>
    public class BaseNetAppService<TEntity> : BaseNetAppService<TEntity, Guid>
        , IBaseNetAppService<TEntity>
        where TEntity : BaseEntity
    {
        public BaseNetAppService(
            INetRepository<TEntity, Guid> baseRepository) : base(baseRepository)
        {
        }
    }
}
