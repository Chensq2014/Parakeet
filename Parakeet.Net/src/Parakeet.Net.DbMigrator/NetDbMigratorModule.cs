using Parakeet.Net.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Parakeet.Net.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(NetEntityFrameworkCoreModule),
    typeof(NetApplicationContractsModule)
    )]
public class NetDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
