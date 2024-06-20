using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Dtos;
using Common.Entities;

namespace Parakeet.Net.Services
{
    /// <summary>
    /// 组织机构领域服务 就算不继承ITransientDependency此接口也会在上层依赖注入时被动加上，
    /// 因为上层接口实现会自动检查依赖，并先注册其依赖的接口，IOrganizationManager
    /// </summary>
    public interface IOrganizationManager //: ITransientDependency
    {
        /// <summary>
        /// 根据父级节点集合获取子集实体集合
        /// </summary>
        /// <param name="parents"></param>
        /// <returns></returns>
        Task<List<Organization>> GetChildrenByParents(IQueryable<Organization> parents);

        /// <summary>
        /// 根据当前父级Id获取所有同级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<Organization>> GetBrothersByParentId(InputIdDto input);
        /// <summary>
        /// 获取含当前节点的所有父级节点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<Organization>> GetAllParentsByParentId(InputIdDto input);
        /// <summary>
        /// 根据当前节点Ids获取所有上级目录(含当前节点)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<Organization>> GetParentsByNodeIds(InputIdsNullDto input);

        /// <summary>
        /// 更新子级Level
        /// </summary>
        /// <param name="parents"></param>
        void UpdateChildrenLevels(IQueryable<Organization> parents);

        #region treeDto


        /// <summary>
        /// 给定根节点下TreeView数据源方法
        /// </summary>
        /// <param name="items">集合</param>
        /// <param name="selectedIds">选中项</param>
        /// <param name="disabled">是否禁用</param>
        /// <returns>TreeDtos</returns>
        List<TreeDto> GetItemTree(IEnumerable<Organization> items, List<Guid> selectedIds, bool disabled = false);

        /// <summary>
        /// 单个节点设置
        /// </summary>
        /// <param name="item">节点</param>
        /// <param name="selectedIds">选中项</param>
        /// <param name="disabled">是否禁用</param>
        /// <returns>TreeDto</returns>
        TreeDto GetTreeDto(Organization item, List<Guid> selectedIds, bool disabled = false);

        /// <summary>
        /// 仅根级节点列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<TreeDto>> GetRootItemTree(InputIdsDto input);

        /// <summary>
        /// 所有根节点下,选中Ids的TreeView数据源方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<TreeDto>> GetRootTrees(InputIdsDto input);

        /// <summary>
        /// 根据当前节点Id寻找根级目录Id
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Guid?> GetRootByNodeId(InputIdNullDto input);

        #endregion
    }
}
