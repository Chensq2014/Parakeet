using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Dtos;
using Common.Entities;
using Common.Enums;
using Common.Extensions;
using Volo.Abp.Domain.Repositories;
using Common.Interfaces;

namespace Parakeet.Net.Services
{
    /// <summary>
    /// 组织机构领域层服务
    /// </summary>
    public class OrganizationManager : NetDomainServiceBase, IOrganizationManager
    {
        private readonly IPortalRepository<Organization> _organizationRepository;
        public OrganizationManager(IPortalRepository<Organization> organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        /// <summary>
        /// 根据父级节点集合获取子集实体集合
        /// </summary>
        /// <param name="parents"></param>
        /// <returns></returns>
        public async Task<List<Organization>> GetChildrenByParents(IQueryable<Organization> parents)
        {
            var items = parents.SelectMany(n => n.Children);
            var parentItems = items.Where(m => m.Children.Any());
            var list = await items.ToListAsync();//.Except(parentItems)根据情况是否把有子级节点的节点排除
            if (parentItems.Any())
            {
                list.AddRange(await GetChildrenByParents(parentItems));
            }
            return list;
        }

        /// <summary>
        /// 根据当前父级Id获取所有同级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<Organization>> GetBrothersByParentId(InputIdDto input)
        {
            var brothers = await (await _organizationRepository.GetQueryableAsync())
                .AsNoTracking()
                .Where(m => m.ParentId == input.Id)
                .ToListAsync();
            return brothers;
        }

        /// <summary>
        /// 获取含当前节点的所有父级节点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<Organization>> GetAllParentsByParentId(InputIdDto input)
        {
            var list = new List<Organization>();
            var parent = await (await _organizationRepository.GetQueryableAsync())
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == input.Id);
            if (parent != null)
            {
                list.Add(parent);
                list.AddRange(await GetAllParentsByParentId(new InputIdDto { Id = parent.ParentId ?? Guid.Empty }));
            }
            return list;
        }

        /// <summary>
        /// 根据当前节点Ids获取所有上级目录(含当前节点)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<Organization>> GetParentsByNodeIds(InputIdsNullDto input)
        {
            var parents = new List<Organization>();
            if (input.Ids.Any())
            {
                var items = await (await _organizationRepository.GetQueryableAsync())
                    .AsNoTracking()
                    .Where(m => input.Ids.Contains(m.Id)).ToListAsync();
                if (items.Any())
                {
                    parents.AddRange(items);
                    parents.AddRange(await GetParentsByNodeIds(new InputIdsNullDto
                    { Ids = items.Select(m => m.ParentId).Distinct().ToList() }));
                }
            }
            return parents.Distinct().ToList();
        }

        /// <summary>
        /// 更新子级Level
        /// </summary>
        /// <param name="parents"></param>
        public void UpdateChildrenLevels(IQueryable<Organization> parents)
        {
            var items = parents.SelectMany(n => n.Children);
            items.Include(l => l.Parent).ToList().ForEach(m =>
            {
                m.Level = m.Parent?.Level + 1 ?? 0;
            });
            if (items.Any())
            {
                UpdateChildrenLevels(items);
            }
        }

        #region treeDto


        /// <summary>
        /// 给定根节点下TreeView数据源方法
        /// </summary>
        /// <param name="items">集合</param>
        /// <param name="selectedIds">选中项</param>
        /// <param name="disabled">是否禁用</param>
        /// <returns>TreeDtos</returns>
        public List<TreeDto> GetItemTree(IEnumerable<Organization> items, List<Guid> selectedIds, bool disabled = false)
        {
            var list = new List<TreeDto>();
            foreach (var item in items)
            {
                list.Add(GetTreeDto(item, selectedIds, disabled));
                if (item.Children.Any())
                {
                    list.AddRange(GetItemTree(item.Children, selectedIds, disabled));
                }
            }
            return list;
        }

        /// <summary>
        /// 单个节点设置
        /// </summary>
        /// <param name="item">节点</param>
        /// <param name="selectedIds">选中项</param>
        /// <param name="disabled">是否禁用</param>
        /// <returns>TreeDto</returns>
        public TreeDto GetTreeDto(Organization item, List<Guid> selectedIds, bool disabled = false)
        {
            return new TreeDto
            {
                Name = item.Name,
                ShowSelectBox = true,
                Disabled = disabled,
                Expanded = false,
                Selected = selectedIds.Contains(item.Id),
                Key = item.Id.ToString(),
                Remark = item.Name,
                Level = item.Level.ToInt().ToEnum<TreeNodeLevel>(),
                TypeIcon = "list-alt",
                Id = item.Id,
                DbId = item.Id,
                ParentId = item.ParentId
            };
        }

        /// <summary>
        /// 仅根级节点列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<TreeDto>> GetRootItemTree(InputIdsDto input)
        {
            var roots = await (await _organizationRepository.GetQueryableAsync()).AsNoTracking()
                .Where(m => m.Level == TreeNodeLevel.根级).ToListAsync();
            var list = new List<TreeDto>();
            roots.ForEach(costItem =>
            {
                list.Add(GetTreeDto(costItem, input.Ids));
                //不需要展示子级
                //if (costItem.Childrens.Any())
                //{
                //    list.AddRange(GetItemTree(costItem.Childrens.ToList(), selectedIds, true));
                //}
            });
            return list;
        }

        /// <summary>
        /// 所有根节点下,选中Ids的TreeView数据源方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<TreeDto>> GetRootTrees(InputIdsDto input)
        {
            var query = await _organizationRepository.GetQueryableAsync();
            var roots = query.AsNoTracking()
                .Where(m => m.Level == TreeNodeLevel.根级);

            var list = GetItemTree(roots, input.Ids);
            return list;
        }

        /// <summary>
        /// 根据当前节点Id寻找根级目录Id
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Guid?> GetRootByNodeId(InputIdNullDto input)
        {
            if (input.Id.HasValue)
            {
                var item = await (await _organizationRepository.GetQueryableAsync()).AsNoTracking().FirstOrDefaultAsync(m => m.Id == input.Id);
                return item.ParentId.HasValue ? await GetRootByNodeId(new InputIdNullDto { Id = item.ParentId }) : item.Id;
            }
            return input.Id;
        }

        #endregion

    }
}
