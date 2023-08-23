namespace Parakeet.Net.Projects
{
    ///// <summary>
    ///// 演示服务
    ///// </summary>
    //public class DemoProjectAppService //: ParakeetAppService, IDemoProjectAppService
    //{
    //    //private readonly IRepository<DemoProject, Guid> _demoProjectRepository;

    //    //private readonly DemoProjectManager _demoProjectManager;

    //    public DemoProjectAppService(
    //        //IRepository<DemoProject, Guid> demoProjectRepository, 
    //        //DemoProjectManager demoProjectManager
    //        )
    //    {
    //        //_demoProjectRepository = demoProjectRepository;
    //        //_demoProjectManager = demoProjectManager;
    //    }

    //    #region Implementation of IDemoProjectAppService

    //    ///// <summary>
    //    ///// 添加项目
    //    ///// </summary>
    //    ///// <param name="input"></param>
    //    ///// <returns></returns>
    //    //[Authorize]
    //    //public async Task<Guid> AddProject(AddProjectInput input)
    //    //{
    //    //    var project = await _demoProjectManager.CreateProject(input.ProjectName);
    //    //    foreach (var unitProjectItem in input.ProjectItems)
    //    //    {
    //    //        await _demoProjectManager.CreateUnitProjectToProject(unitProjectItem.Name, unitProjectItem.Address, project.Id);
    //    //    }
    //    //    return project.Id;
    //    //}

    //    ///// <summary>
    //    ///// 获取我创建的项目
    //    ///// </summary>
    //    ///// <param name="input"></param>
    //    ///// <returns></returns>
    //    //[Authorize]
    //    //public Task<IPagedResult<ProjectItemDto>> GetMyProjects(GetMyProjectsInput input)
    //    //{
    //    //    return _demoProjectRepository
    //    //        .AsNoTracking()
    //    //        .Where(x => x.CreatorId == CurrentUser.Id)
    //    //        .WhereIf(!string.IsNullOrEmpty(input.Name), x => x.Name.Contains(input.Name))
    //    //        //.WhereCreatedDateRange(input.FromDate, input.ToDate)
    //    //        .OrderBy(input.Sorting)
    //    //        .ProjectTo<ProjectItemDto>(Configuration) //需在ProjectDtoMapper中进行映射配置
    //    //        .ToPageResultAsync(input);
    //    //}

    //    ///// <summary>
    //    ///// 更新项目
    //    ///// </summary>
    //    ///// <param name="input"></param>
    //    ///// <returns></returns>
    //    //[Authorize]
    //    //public async Task UpdateProject(UpdateProjectInput input)
    //    //{
    //    //    //var project = await _demoProjectRepository.IncludeIf(input.SyncUnitProject,x=>x.DemoUnitProjects).Include(x => x.DemoUnitProjects).GetAsync(input.ProjectId);
    //    //    //使用下面的方法仅仅是演示如何将错误消息返回给前端，建议直接使用上面的方式，抛出系统异常，不用将此异常返回给前端
    //    //    var project = await _demoProjectRepository
    //    //        .IncludeIf(input.SyncUnitProject, x => x.DemoUnitProjects)
    //    //        .FirstOrDefaultAsync(x => x.Id == input.ProjectId);
    //    //    if (project == null || project.CreatorId != CurrentUser.Id.Value)
    //    //    {
    //    //        throw new UserFriendlyException("未找到指定的项目");
    //    //    }

    //    //    if (input.SyncUnitProject)
    //    //    {
    //    //        project.SynchronizationUnitProjectName(input.Name);
    //    //    }
    //    //    else
    //    //    {
    //    //        project.UpdateName(input.Name);
    //    //    }
    //    //}

    //    ///// <summary>
    //    ///// 获取项目详情
    //    ///// </summary>
    //    ///// <param name="id"></param>
    //    ///// <returns></returns>
    //    //public Task<ProjectDto> GetProject(Guid id)
    //    //{
    //    //    return _demoProjectRepository.GetProjectToDtoAsync<DemoProject, Guid, ProjectDto>(id, Configuration);
    //    //}

    //    #endregion
    //}
}