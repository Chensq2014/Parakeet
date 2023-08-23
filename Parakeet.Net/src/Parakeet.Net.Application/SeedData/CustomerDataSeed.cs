using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace Parakeet.Net.SeedData
{
    /// <summary>
    ///     初始化数据 放到application模块即可,继承自IDataSeedContributor接口的类，再主项目配置每次启动都执行
    /// </summary>
    public class CustomerDataSeed : IDataSeedContributor, ITransientDependency
    {
        //private readonly IRepository<FileFolder, Guid> _fileFolderRepository;
        //private readonly IRepository<TableTemplate, Guid> _tableTemplateRepository;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;
        public CustomerDataSeed(
            //IRepository<FileFolder, Guid> fileFolderRepository,
            //IRepository<TableTemplate, Guid> tableTemplateRepository,
            Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager)
        {
            //_fileFolderRepository = fileFolderRepository;
            //_tableTemplateRepository = tableTemplateRepository;
            _roleManager = roleManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        { 
            await Task.Run(() =>{});
            //默认创建一个Customer角色
            //if (await _roleManager.FindByNameAsync("Customer") == null)
            //{
            //    await _roleManager.CreateAsync(new IdentityRole(Guid.NewGuid(), "Customer"));
            //}
            //var parent = await _fileFolderRepository
            //    .FirstOrDefaultAsync(m => m.Level == FolderLevel.子分部工程分类 && m.Name == "地基");
            //if (await _tableTemplateRepository.AnyAsync(m => m.SerialNumber > 0)) return;

            //await _tableTemplateRepository.InsertAsync(new TableTemplate(Guid.NewGuid(), "010101灰土地基工程检验批质量验收记录", "", 10)
            //{
            //    Title = "灰土", //分项名称
            //    FileFolderId = parent.Id,
            //    TemplateType = TemplateTableType.检验批表,
            //    SummaryType = SummaryType.工程质量验收汇总,
            //    ExcelDataId = Guid.Parse("1a93a6a3-914c-b8bf-377a-39f0e55c2aa9"), //Guid.Empty,
            //    ExtendDataId = Guid.Empty
            //});
        }
    }
}