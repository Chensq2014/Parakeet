using Volo.Abp.Domain.Services;

namespace Parakeet.Net.Entities.DemoManagement
{
    /// <summary>
    /// 领域服务
    /// </summary>
    public class DemoProjectManager : DomainService
    {
        //private readonly IRepository<DemoProject, Guid> _demoProjectRepository;
        //private readonly IRepository<DemoUnitProject, Guid> _demoUnitProjectRepository;

        //public DemoProjectManager(IRepository<DemoProject, Guid> demoProjectRepository,
        //    IRepository<DemoUnitProject, Guid> demoUnitProjectRepository)
        //{
        //    _demoProjectRepository = demoProjectRepository;
        //    _demoUnitProjectRepository = demoUnitProjectRepository;
        //}

        ////*
        // * 领域对象建议通过领域服务来创建，而不是在application应用层创建，领域根对象的构造方法建议声明为internal来保护
        // */
        ///// <summary>
        ///// 通过领域服务创建项目
        ///// </summary>
        ///// <param name="projectName"></param>
        ///// <returns></returns>
        //public async Task<DemoProject> CreateProject(string projectName)
        //{
        //    var project = new DemoProject(Guid.NewGuid(), projectName);
        //    return await _demoProjectRepository.InsertAsync(project);
        //}

        ///// <summary>
        ///// 创建子项目并绑定的指定项目中去
        ///// </summary>
        ///// <returns></returns>
        //public async Task<DemoUnitProject> CreateUnitProjectToProject(string unitProjectName, string unitProjectAddress, Guid projectId)
        //{
        //    var unitProject = new DemoUnitProject(GuidGenerator.Create(), unitProjectName, projectId, unitProjectAddress);
        //    return await _demoUnitProjectRepository.InsertAsync(unitProject);
        //}
    }
}