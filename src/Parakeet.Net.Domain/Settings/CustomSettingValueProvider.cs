using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Volo.Abp.Settings;

namespace Parakeet.Net.Settings
{
    /// <summary>
    /// 自定义设置值提供程序
    /// </summary>
    public class CustomSettingValueProvider : SettingValueProvider
    {
        public override string Name => CommonConsts.AppName;
        public CustomSettingValueProvider(ISettingStore settingStore) : base(settingStore)
        {
        }

        public override Task<string> GetOrNullAsync(SettingDefinition setting)
        {
            /* Return the setting value or null
             Use the SettingStore or another data source */
            return Task.FromResult(setting.DefaultValue);
        }

        public override Task<List<SettingValue>> GetAllAsync(SettingDefinition[] settings)
        {
            return Task.FromResult(settings.Select(x=>new SettingValue(x.Name,x.DefaultValue)).ToList());
        }

    }
}
