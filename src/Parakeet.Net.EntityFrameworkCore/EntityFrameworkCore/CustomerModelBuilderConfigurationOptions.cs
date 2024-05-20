using Common;
using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Parakeet.Net.EntityFrameworkCore
{
    public class CustomerModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public CustomerModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = CommonConsts.DefaultDbTablePrefix,
            [CanBeNull] string schema = CommonConsts.ParakeetSchema)
            : base(tablePrefix, schema)
        {
        }
    }
}
