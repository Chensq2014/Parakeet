using Parakeet.Net.Dtos;
using Parakeet.Net.Entities;
using Parakeet.Net.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Organizations
{
    /// <summary>
    /// IOrganizationService 服务
    /// </summary>
    [Description("组织机构")]
    public interface IOrganizationAppService : IBaseNetAppService<Organization>, ITransientDependency
    {
        /// <summary>
        ///  获取列表数据(分页)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IPagedResult<OrganizationDto>> GetPagedResult(PagedInputDto input);

        /// <summary>
        /// 获取所有叶子节点Id
        /// </summary>
        /// <returns>获取所有叶子节点Id</returns>
        Task<List<Guid>> GetLeafNodeIds();

        /// <summary>
        /// 获取所有父级节点Id
        /// </summary>
        /// <returns>获取所有父级节点Id</returns>
        Task<List<Guid>> GetParentNodeIds();

        /// <summary>
        /// 获取所有根节点Ids
        /// </summary>
        /// <returns></returns>
        Task<List<Guid>> GetRootNodeIds();

        /// <summary>
        /// 获取所有根节点与已经为父级的节点Ids
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetRootAndParentNodeIds();

        /// <summary>
        /// 获取所有父级SelectBoxsGetParentSelectList
        /// </summary>
        /// <param name="id"></param>
        /// <returns>SelectBoxs</returns>
        Task<List<SelectBox<string, Guid?>>> GetParentSelectList(InputIdNullDto id);

        /// <summary>
        /// 根级节点treeView数据源
        /// </summary>
        /// <param name="input">ids:selectedIds</param>
        /// <returns>TreeDtos</returns>
        Task<List<TreeDto>> GetRootItemTree(InputIdsDto input);

        /// <summary>
        /// 所有节点treeView数据源
        /// </summary>
        /// <param name="input">ids:selectedIds</param>
        /// <returns>TreeDto</returns>
        Task<List<TreeDto>> GetRootTrees(InputIdsDto input);

        /// <summary>
        /// 根据当前节点Id寻找根级目录Id
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Guid?> GetRootByNodeId(InputIdNullDto input);
        #region 根据父级节点获取子节点

        /// <summary>
        /// 获取某组织机构下的子级机构
        /// </summary>
        /// <param name="input">ids:parentIds父级Ids集合</param>
        /// <returns>某组织机构下的子级机构</returns>
        Task<IEnumerable<OrganizationDto>> GetChildrenByParents(InputIdsDto input);

        /// <summary>
        /// 获取某组织机构的所有子级
        /// </summary>
        /// <param name="input">Id:parentId</param>
        /// <returns></returns>
        Task<IEnumerable<OrganizationDto>> GetChildrenByParent(InputIdDto input);

        /// <summary>
        /// 获取某组织下所有子级机构
        /// </summary>
        /// <param name="input">Id:organizationId</param>
        /// <returns></returns>
        Task<List<OrganizationDto>> GetAllChildrenByParentId(InputIdDto input);

        /// <summary>
        /// 获取某组织集合 下所有子级机构
        /// </summary>
        /// <param name="input">ids：parentIds某组织集合</param>
        /// <returns>子级机构集合</returns>
        Task<List<OrganizationDto>> GetAllChildrenByParentIds(InputIdsDto input);

        #endregion


        #region 放在领域层的方法 根据父级获取子级 直接返回实体集合

        ///// <summary>
        ///// 根据父级机构集合 获取所有子级机构集合 GetChildrens
        ///// </summary>
        ///// <param name="parents">父级机构集合</param>
        ///// <returns>子级机构集合</returns>
        //Task<List<Organization>> GetChildrenByParents(IQueryable<Organization> parents);

        ///// <summary>
        ///// 根据父级找出所有同级节点
        ///// </summary>
        ///// <param name="input">id:parentId父级机构Id</param>
        ///// <returns>其子级与所有同级机构</returns>
        //Task<List<Organization>> GetBrothersByParentId(InputIdDto input);

        ///// <summary>
        ///// 根据当前节点父级Id 找出所有父级
        ///// </summary>
        ///// <param name="input">id:parentId当前节点父级Id</param>
        ///// <returns>所有父级机构集合</returns>
        //Task<List<Organization>> GetAllParentsByParentId(InputIdDto input);

        ///// <summary>
        ///// 更新子级层级 UpdateChildrenLevels
        ///// </summary>
        ///// <returns>SelectBoxs</returns>
        //void UpdateChildrenLevels(IQueryable<Organization> parents);

        #endregion


    }
}
