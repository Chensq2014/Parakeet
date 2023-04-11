using System;
using Parakeet.Net.Dtos;
using Parakeet.Net.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.LocationAreas
{
    /// <summary>
    /// 省市区区域服务
    /// </summary>
    public interface ILocationAreaAppService
    {
        /// <summary>
        ///     获取省市区区域列表公共接口(3-5级联动共用)
        /// </summary>
        /// <returns></returns>
        Task<IList<LocationAreaDto>> GetLocationAreas(LocationAreaInputDto input);

        /// <summary>
        ///     获取省市区区域列表(分页)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IPagedResult<LocationAreaDto>> GetPagedResult(GetLocationAreaListDto input);

        /// <summary>
        /// 获取Get(扩展) 提供给devExtreme
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<TreeDto>> GetLocationAreaTree(InputIdsNullDto input);

        /// <summary>
        /// 获取Get(扩展) 提供给devExtreme
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        Task<LoadResult> GetAreaTreeList(DataSourceLoadOptionsBase loadOptions);

        /// <summary>
        /// 添加实体 提供给devExtreme 前端控件验证实体
        /// </summary>
        /// <returns></returns>
        [Description("添加实体")]
        Task<Guid> InsertAreaTreeList();

        /// <summary>
        /// UpdateUpdate修改实体 提供给devExtreme,前端控件验证实体
        /// </summary>
        /// <returns></returns>
        [Description("修改实体")]
        Task UpdateAreaTreeList();

        /// <summary>
        /// 根据主键Id删除实体 DeleteDelete 默认提供给devExtreme
        /// </summary>
        /// <returns></returns>
        [Description("删除实体")]
        Task DeleteAreaTreeList();

        /// <summary>
        /// 提供给devExtreme lookup
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IList<SelectBox>> GetLocationAreaSelectBox(LocationAreaInputDto input);

        /// <summary>
        /// 获取父级节点SelectList列表 故意为post
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<SelectBox<string, Guid?>>> GetParentSelectList(InputIdNullDto input);

        /// <summary>
        /// 根据当前父级Id获取当前父级下的所有子级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<LocationAreaDto>> GetAllChildrenByParentId(InputIdDto input);

        /// <summary>
        /// 单字段重复性验证
        /// </summary>
        /// <param name="input">单个字段选项</param>
        /// <returns>bool</returns>
        Task<bool> CheckField(FieldCheckOptionInputDto<Guid> input);

        /// <summary>
        /// 多字段重复性验证，dxGrid在新建/编辑数据时 时时验证
        /// </summary>
        /// <param name="input">多字段dto选项</param>
        /// <returns>bool</returns>
        Task<bool> CheckFields(FieldsCheckOptionInputDto<Guid> input);

        /// <summary>
        /// 添加同(Level)区域数据(读取Json数据) 从小到大添加
        /// </summary>
        /// <param name="level">0-5</param>
        /// <returns></returns>
        Task AddLevelAreasInOrder(DeepLevelType level);

        /// <summary>
        /// 数据初始化：导入excel文件 读取数据导入到数据库
        /// </summary>
        /// <param name="input">多文件上传对象</param>
        /// <returns></returns>
        Task ImportFromExcel(ImportFileDto input);
    }
}