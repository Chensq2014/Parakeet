using Common.Entities;
using Common.Interfaces;
using System;

namespace Parakeet.Net.Controllers
{
    /// <summary>
    /// 泛型基类Controller，用于DevExtreme的CRUD
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    ///  <typeparam name="TPrimaryKey">实体主键类型</typeparam>
    public abstract class BaseEntityController<TEntity, TPrimaryKey> : NetController where TEntity : BaseEntity<TPrimaryKey> where TPrimaryKey : struct
    {
        public IBaseNetAppService<TEntity, TPrimaryKey> BaseService;

        protected BaseEntityController(IBaseNetAppService<TEntity, TPrimaryKey> baseService)
        {
            BaseService = baseService;
        }

        #region GetGridData扩展
        ///// <summary>
        ///// 获取GridData 提供给devExtreme
        ///// </summary>
        ///// <param name="loadOptions"></param>
        ///// <returns>IActionResult</returns>
        //[HttpGet]
        //public virtual async Task<IActionResult> Get(DataSourceLoadOptionsBase loadOptions)
        //{
        //    ////带上过滤项filter 额外传递得参数 GetValuesQueryString() 
        //    //var filter = JsonConvert.DeserializeObject<FilterDto>(GetInputQueryString());
        //    //if (loadOptions.PrimaryKey.Any())
        //    //{
        //    //    //filter.FilterId = Guid.Parse(loadOptions.PrimaryKey.FirstOrDefault());
        //    //}
        //    var result = DJson(await BaseService.GetGet(loadOptions));
        //    return result;
        //}

        ///// <summary>
        ///// 获取GridData Post 提供给devExtreme 待测试
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns>IActionResult</returns>
        //[HttpPost]
        //public virtual async Task<IActionResult> DataGrid(LoadOptionInputDto input)
        //{
        //    //参数及数据处理，留给子类重写
        //    //await DataSourceLoader.LoadAsync(BaseService.GetBaseRepository().AsNoTracking().Where(m=>m.Id==input.Id),input.LoadOptions);
        //    var result = DJson(await BaseService.GridData(input));
        //    return result;
        //}

        ///// <summary>
        ///// 根据主键获取实体
        ///// </summary>
        ///// <param name="key">实体主键</param>
        ///// <returns></returns>
        //public async Task<TEntity> Get(TPrimaryKey key)
        //{
        //    return await BaseService.GetAsync(new InputIdDto<TPrimaryKey> { Id = key });
        //    //return Json(entity);
        //    //BaseService.GetBaseRepository().UnitOfWorkManager.Current.Options
        //    //UnitOfWorkManager.Current.SetFilterParameter()
        //    //UnitOfWorkManager.Begin();
        //    //CurrentUnitOfWork.Completed+=
        //    //UnitOfWorkManager.Begin().CompleteAsync()
        //    //CurrentUnitOfWork.SaveChanges();
        //}
        #endregion

        #region 插入数据
        ///// <summary>
        ///// 添加实体 提供给devExtreme 前端控件验证实体
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost, Description("添加实体")]
        //public virtual async Task<IActionResult> Insert()
        //{
        //    return DJson(await BaseService.InsertInsert());
        //}

        ///// <summary>
        ///// 添加实体 提供给devExtreme 前端控件一次性提交多个实体 控件工作方式并不是这样工作的
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost, Description("添加实体")]
        //public async Task BulkInsert()
        //{
        //    await BaseService.BulkInsert();
        //}

        ///// <summary>
        ///// 批量添加实体
        ///// </summary>
        ///// <param name="entities">实体集</param>
        ///// <returns></returns>
        //[HttpPost, Description("批量添加数据")]
        //protected async Task<IActionResult> BulkInsertAsync(List<TEntity> entities)
        //{
        //    await BaseService.BulkInsertAsync(entities);
        //    return SJson();
        //}

        #endregion

        #region 修改数据
        ///// <summary>
        ///// 修改实体 提供给devExtreme,前端控件验证实体
        ///// </summary>
        ///// <returns></returns>
        //[HttpPut, Description("修改实体")]
        //public virtual async Task Update()
        //{
        //    await BaseService.UpdateUpdate();
        //}

        #endregion

        #region 删除数据
        ///// <summary>
        ///// 删除实体 提供给devExtreme
        ///// </summary>
        ///// <returns></returns>
        //[HttpDelete, Description("批量删除数据")]
        //public virtual async Task Delete()
        //{
        //    await BaseService.DeleteDelete();
        //}

        ///// <summary>
        ///// 批量删除实体 通用 sqlserver
        ///// </summary>
        ///// <param name="input">实体主键集</param>
        ///// <returns></returns>
        //[HttpPost, Description("批量删除数据")]
        //public async Task BulkDeleteAllAsync(InputIdsDto<TPrimaryKey> input)
        //{
        //    await BaseService.BulkDeleteByPrimaryKeys(input);
        //}

        #endregion

        #region 根据前端Grid传递的values json字符串反序列化为当前实体对象 放入BaseApplication

        //protected TEntity GetValues<TEntity>()
        //{
        //    var formData = Request.Form["values"];
        //    var entity = JsonConvert.DeserializeObject<TEntity>(formData);
        //    return entity;
        //}
        //protected List<TEntity> GetItemValues<TEntity>()
        //{
        //    var formData = Request.Form["values"];
        //    var entities = JsonConvert.DeserializeObject<List<TEntity>>(formData);
        //    return entities;
        //}
        //protected TEntity GetValues<TEntity>(string valueString)
        //{
        //    var entity = JsonConvert.DeserializeObject<TEntity>(valueString);
        //    return entity;
        //}
        //protected string GetValuesString()
        //{
        //    var formData = Request.Form["values"].ToString();
        //    //formData = formData.Replace(" ", "");//去掉所有空格
        //    return formData.Trim();
        //}
        //protected string GetValuesQueryString()
        //{
        //    var formData = Request.Query["values"].ToString();
        //    return formData.Trim();//formData = formData.Replace(" ", "");//去掉所有空格
        //}

        //protected string GetInputQueryString()
        //{
        //    var formData = Request.Query["input"].ToString();
        //    return formData.Trim();//formData = formData.Replace(" ", "");//去掉所有空格
        //}

        ///// <summary>
        ///// 获取主键string转int/guid 注意TPrimaryKey非int?与guid？
        ///// </summary>
        ///// <param name="key">主键字符串</param>
        ///// <returns>主键类型值</returns>
        //protected TPrimaryKey GetTPrimaryKey(string key = null)
        //{
        //    key = key ?? Request.Form["key"];//一定有key不做null判断
        //    //怎样才能排除这个if判断？有没有一个struct类型互转的通用方法？头发已掉100根
        //    if (typeof(TPrimaryKey) == typeof(Guid))
        //    {
        //        return (TPrimaryKey)Convert.ChangeType(Guid.Parse(key), typeof(TPrimaryKey));
        //    }
        //    return (TPrimaryKey)Convert.ChangeType(key, typeof(TPrimaryKey));
        //}

        //protected string GetKeyString()
        //{
        //    var formData = Request.Form["key"];
        //    return formData;
        //}

        //protected TPrimaryKey GetId()
        //{
        //    var value = Request.Form["id"];
        //    return string.IsNullOrWhiteSpace(value)
        //        ? default(TPrimaryKey)
        //        : GetTPrimaryKey(value);
        //}

        #endregion
    }

    public abstract class BaseEntityController<TEntity> : BaseEntityController<TEntity, Guid> where TEntity : BaseEntity
    {
        protected BaseEntityController(IBaseNetAppService<TEntity, Guid> baseService) : base(baseService)
        {
        }
    }
}
