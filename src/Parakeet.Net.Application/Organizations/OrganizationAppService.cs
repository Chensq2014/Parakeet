//using Common.Dtos;
//using Common.Entities;
//using Common.Enums;
//using Common.Interfaces;
//using DevExtreme.AspNet.Data;
//using DevExtreme.AspNet.Data.ResponseModel;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using Parakeet.Net.Services;
//using Serilog;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Transactions;
//using Volo.Abp;
//using Volo.Abp.Application.Dtos;

//namespace Parakeet.Net.Organizations
//{
//    /// <summary>
//    /// 组织机构服务
//    /// </summary>
//    //[ApiExplorerSettings(GroupName = "V2")]
//    [Description("组织机构")]
//    public class OrganizationAppService : BaseNetAppService<Organization>, IOrganizationAppService
//    {
//        private readonly INetRepository<Organization> _organizationRepository;
//        private readonly IOrganizationManager _organizationManager;
//        public OrganizationAppService(
//            IOrganizationManager organizationManager,
//            INetRepository<Organization> baseRepository) : base(baseRepository)
//        {
//            _organizationRepository = baseRepository;
//            _organizationManager = organizationManager;
//        }

//        #region 重写GetGridData 获取分页数据GetPagedResult
//        /// <summary>
//        /// 获取Get(扩展) 提供给devExtreme
//        /// </summary>
//        /// <param name="loadOptions"></param>
//        /// <returns></returns>
//        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
//        {
//            var result = await DataSourceLoader.LoadAsync(await GridDto<OrganizationDto>(), loadOptions);
//            return result;
//        }

//        /// <summary>
//        /// 默认Post获取GridData(可扩展)  提供给devExtreme
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public override async Task<LoadResult> GridData(LoadOptionInputDto input)
//        {
//            //根据input过滤数据源
//            var result = await DataSourceLoader.LoadAsync((await GridDto<OrganizationDto>()).Where(m => m.Id == input.Id), input.LoadOptions);
//            return result;
//        }

//        /// <summary>
//        ///  获取列表数据(分页)
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<IPagedResult<OrganizationDto>> GetPagedResult(PagedInputDto input)
//        {
//            return await base.GetPagedResult<OrganizationDto>(input);
//        }
//        #endregion

//        #region 重写crud
//        /// <summary>
//        /// 添加实体 提供给devExtreme 前端控件验证实体
//        /// </summary>
//        /// <returns></returns>
//        [Description("添加实体")]
//        public override async Task<Guid> InsertInsert()
//        {
//            var entity = GetValues<Organization>();
//            entity = await Repository.InsertAsync(entity);
//            entity.Parent = entity.ParentId.HasValue
//                ? await _organizationRepository.GetAsync(entity.ParentId.Value)
//                : null;
//            entity.Level = entity.Parent?.Level + 1 ?? 0;
//            return entity.Id;//ObjectMapper.Map<Organization,OrganizationDto>(entity);
//        }


//        /// <summary>
//        /// UpdateUpdate修改实体 提供给devExtreme,前端控件验证实体
//        /// </summary>
//        /// <returns></returns>
//        [Description("修改实体")]
//        public override async Task UpdateUpdate()
//        {
//            var key = GetTPrimaryKey();
//            var entity = await (await _organizationRepository.GetQueryableAsync())
//                .Include(n => n.Parent).Include(o => o.Children)
//                .FirstOrDefaultAsync(m=>m.Id==key);
//            JsonConvert.PopulateObject(GetValuesString(), entity);
//            if (entity.ParentId != entity.Parent?.Id)
//            {
//                entity.Parent = entity.ParentId.HasValue
//                    ? await Repository.GetAsync(entity.ParentId.Value)
//                    : null;
//                if (entity.Level != entity.Parent?.Level + 1)
//                {
//                    //所有子级变更Level
//                    entity.Level = (entity.Parent?.Level + 1) ?? 0;
//                    _organizationManager.UpdateChildrenLevels((await _organizationRepository.GetQueryableAsync()).Where(m => m.Id == key));
//                }
//            }
//        }

//        /// <summary>
//        /// 根据主键Id删除实体 DeleteDelete 默认提供给devExtreme
//        /// </summary>
//        /// <returns></returns>
//        [HttpDelete, Description("删除实体")]
//        public override async Task DeleteDelete()
//        {
//            //找所有的子级然后删除
//            var key = GetTPrimaryKey();
//            var deleteIds = (await GetAllChildrenByParentId(new InputIdDto { Id = key })).Select(m => m.Id).ToList();
//            await Repository.DeleteAsync(m => deleteIds.Contains(m.Id) || m.Id == key);
//        }

//        #endregion

//        #region tree结构相关

//        /// <summary>
//        /// 获取所有叶子节点Id
//        /// </summary>
//        /// <returns></returns>
//        public async Task<List<Guid>> GetLeafNodeIds()
//        {
//            var nodes = await (await Repository.GetQueryableAsync()).AsNoTracking().Select(n => new { n.Id, n.ParentId }).ToListAsync();
//            var leafNodeIds = nodes.Where(n => nodes.All(m => m.ParentId != n.Id)).Select(n => n.Id).Distinct().ToList();
//            return leafNodeIds;
//        }

//        /// <summary>
//        /// 获取所有已经为父级的节点Ids
//        /// </summary>
//        /// <returns></returns>
//        public async Task<List<Guid>> GetParentNodeIds()
//        {
//            var parentNodes = await (await Get(m => m.ParentId.HasValue)).AsNoTracking()
//                .Select(n => n.ParentId.Value)
//                .Distinct()
//                .ToListAsync();
//            return parentNodes;
//        }

//        /// <summary>
//        /// 获取所有根节点Ids
//        /// </summary>
//        /// <returns></returns>
//        public async Task<List<Guid>> GetRootNodeIds()
//        {
//            return await (await Repository.GetQueryableAsync()).AsNoTracking().Where(m => m.Level == TreeNodeLevel.根级).Select(n => n.Id).ToListAsync();
//        }

//        /// <summary>
//        /// 获取所有根节点与已经为父级的节点Ids
//        /// </summary>
//        /// <returns></returns>
//        //[RemoteService(IsEnabled = true, IsMetadataEnabled = false)]//允许内部调用但禁止生成api
//        public async Task<IEnumerable<Guid>> GetRootAndParentNodeIds()
//        {
//            var parentIds = await GetParentNodeIds();
//            var rootIds = await GetRootNodeIds();
//            return parentIds.Union(rootIds).Distinct();
//        }

//        /// <summary>
//        /// 获取父级节点SelectList列表 故意为post
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<SelectBox<string, Guid?>>> GetParentSelectList(InputIdNullDto input)
//        {
//            //不能设置当前节点和自己的子级节点为自己的父级
//            var sonNodeIds = input.Id.HasValue && input.Id != Guid.Empty
//                ? (await GetAllChildrenByParentId(new InputIdDto { Id = input.Id ?? Guid.Empty })).Select(m => m.Id).ToList()
//                : new List<Guid>();
//            var selectNodes = await (await Get(m => m.Id != input.Id)).AsNoTracking()
//                .WhereIf(sonNodeIds.Any(), p => !sonNodeIds.Contains(p.Id))
//                .Select(m => new SelectBox<string, Guid?>
//                {
//                    Text = m.Name,
//                    Value = m.Id
//                }).ToListAsync();
//            if (selectNodes.Any())
//            {
//                selectNodes.AddFirst(new SelectBox<string, Guid?>
//                {
//                    Text = "无",
//                    Value = default
//                });
//            }
//            return selectNodes;
//        }

//        /// <summary>
//        /// 仅根级节点列表
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<TreeDto>> GetRootItemTree(InputIdsDto input)
//        {
//            return await _organizationManager.GetRootItemTree(input);
//        }

//        /// <summary>
//        /// 所有根节点下,选中Ids的TreeView数据源方法
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<TreeDto>> GetRootTrees(InputIdsDto input)
//        {
//            return await _organizationManager.GetRootTrees(input);
//        }

//        /// <summary>
//        /// 根据当前节点Id寻找根级目录Id
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<Guid?> GetRootByNodeId(InputIdNullDto input)
//        {
//            return await _organizationManager.GetRootByNodeId(input);
//        }

//        #endregion

//        #region 根据父级节点获取子节点

//        /// <summary>
//        /// 根据父级Ids集合获取子级实体Dto集合
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<IEnumerable<OrganizationDto>> GetChildrenByParents(InputIdsDto input)
//        {
//            var parents =await Get(m => input.Ids.Contains(m.Id));
//            var childrens = ObjectMapper.Map<List<Organization>, IEnumerable<OrganizationDto>>(await _organizationManager.GetChildrenByParents(parents));
//            //下面的功能也可，但不能将导航属性赋值给Dto
//            //var list = ExpMapper<Organization, OrganizationDto>.Trans(childrens);
//            return childrens;
//        }

//        /// <summary>
//        /// 根据当前父级Id获取当前父级下的所有子级
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<IEnumerable<OrganizationDto>> GetChildrenByParent(InputIdDto input)
//        {
//            var parents = (await Get(m => m.Id == input.Id)).AsNoTracking();
//            var childrens = ObjectMapper.Map<List<Organization>, IEnumerable<OrganizationDto>>(await _organizationManager.GetChildrenByParents(parents));
//            //下面的功能也可，但不能将导航属性赋值给Dto
//            //var list = ExpMapper<Organization, OrganizationDto>.Trans(childrens);
//            return childrens;
//        }

//        /// <summary>
//        /// 根据当前父级Id获取当前父级下的所有子级
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
//        public async Task<List<OrganizationDto>> GetAllChildrenByParentId(InputIdDto input)
//        {
//            var childrens = new List<OrganizationDto>();
//            var items = await (await Get(m => m.Id == input.Id)).SelectMany(n => n.Children).ToListAsync();
//            foreach (var organization in items)
//            {
//                childrens.AddRange(await GetAllChildrenByParentId(new InputIdDto { Id = organization.Id }));
//            }

//            if (items.Any())
//            {
//                childrens.AddRange(ObjectMapper.Map<List<Organization>, List<OrganizationDto>>(items));
//            }
//            return childrens;
//        }

//        /// <summary>
//        /// 根据当前节点Ids获取所有子级
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<OrganizationDto>> GetAllChildrenByParentIds(InputIdsDto input)
//        {
//            var items = await (await Get(m => input.Ids.Contains(m.Id))).AsNoTracking().SelectMany(n => n.Children).ToListAsync();
//            var childrens = items.Any() 
//                ? ObjectMapper.Map<List<Organization>, List<OrganizationDto>>(items) 
//                : new List<OrganizationDto>();
//            childrens.AddRange(await GetAllChildrenByParentIds(new InputIdsDto { Ids = items.Select(m => m.Id).ToList() }));
//            return childrens;
//        }

//        #endregion

//        #region 读写分离测试

//        /// <summary>
//        /// 读写分离的sql测试 
//        /// </summary>
//        /// <returns></returns>
//        public async Task TransactionScopeReadAndWriteTest()
//        {
//            //不同的数据库当然是不同的链接--就是分布式事务---需要启动下电脑DTC服务---
//            //.Net2.1之后就不支持了---不过，在.NetFramework下面，MSDTC确实可以保证多个数据库中间完成事务

//            //使用系统UnitOfWorkManager 多个数据库操作 //没办法控制多库事务
//            //using (var unitOfWork = UnitOfWorkManager.Begin(true))
//            //{
//            //    var organization = new Organization
//            //    {
//            //        Name = $@"testName",
//            //        Code = $@"testCode",
//            //        ParentId = Guid.Parse("F1CB326B-7841-C11A-55CA-39F36F4D1D0E"),
//            //        OrganizationType = OrganizationType.Group,
//            //        Level = TreeNodeLevel.根级
//            //    };
//            //    await _organizationRepository.SqlInsert(organization);//写库 直接成功了，没办法控制事务了，需要把数据事务去掉
//            //    var organizations = await _organizationRepository.SqlQueryByCondition<Organization>(m => m.Name.Contains("test") || m.Code == "test");
//            //    //Log.Logger.Information($@"写入组织机构：{organization.Name}, 读取{organizations.Count}个组织");//读库
//            //    await unitOfWork.CompleteAsync();
//            //}

//            #region 使用系统自带scope/unitOfWork  就是多个数据库操作  事务不能控制，第一个写库更新成功，第二个写库失败

//            //使用系统自带scope  就是多个数据库操作 测试结果：事务控制只能写第一个库，第二个库就失败了，读库无影响
//            using var scope = new TransactionScope();
//            var newId = GuidGenerator.Create().ToString();
//            //用写库1写2条数据
//            var organization = new Organization
//            {
//                Name = $@"testName{newId}",
//                Code = $@"testCode{newId}",
//                ParentId = Guid.Parse("F1CB326B-7841-C11A-55CA-39F36F4D1D0E"),
//                OrganizationType = OrganizationType.Group,
//                Level = TreeNodeLevel.一级
//            };
//            var organization2 = new Organization
//            {
//                Name = $@"testName{GuidGenerator.Create()}",
//                Code = $@"testCode{GuidGenerator.Create()}",
//                ParentId = Guid.Parse("F1CB326B-7841-C11A-55CA-39F36F4D1D0E"),
//                OrganizationType = OrganizationType.Group,
//                Level = TreeNodeLevel.一级
//            };
//            await _organizationRepository.SqlInsert(organization);
//            await _organizationRepository.SqlInsert(organization2);

//            ////用写库2写一条数据
//            //var organization1 = new Organization
//            //{
//            //    Name = $@"testCode33333",
//            //    Code = $@"testCode33333",
//            //    ParentId = Guid.Parse("F1CB326B-7841-C11A-55CA-39F36F4D1D0E"),
//            //    OrganizationType = OrganizationType.Group,
//            //    Level = TreeNodeLevel.一级
//            //};
//            //await _organizationRepository.WriteSqlInsert(organization1);

//            //用读库1读取多条数据
//            var organizations = await _organizationRepository.SqlQueryByCondition<Organization>(m => m.Name.Contains("test") || m.Code == "test");

//            Log.Logger.Information($@"写入组织机构：{organization.Name}, 读取{organizations.Count}个组织");
//            scope.Complete();

//            #endregion
//        }

//        #endregion
//    }
//}
