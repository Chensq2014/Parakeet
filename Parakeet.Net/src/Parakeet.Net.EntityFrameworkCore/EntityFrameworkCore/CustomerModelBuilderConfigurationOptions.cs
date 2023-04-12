using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Parakeet.Net.EntityFrameworkCore
{
    public class CustomerModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public CustomerModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = CustomerConsts.DefaultDbTablePrefix,
            [CanBeNull] string schema = CustomerConsts.ParakeetSchema)
            : base(tablePrefix, schema)
        {
        }
    }
}
