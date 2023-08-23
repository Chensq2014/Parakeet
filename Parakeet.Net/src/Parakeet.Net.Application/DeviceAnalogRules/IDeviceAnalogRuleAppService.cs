using Parakeet.Net.Dtos;
using Parakeet.Net.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parakeet.Net.Entities;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.DeviceAnalogRules
{
    /// <summary>
    /// 设备模拟数据规则服务
    /// </summary>
    public interface IDeviceAnalogRuleAppService : IBaseNetAppService<DeviceAnalogRule>, ITransientDependency
    {
        /// <summary>
        /// 根据Id获取规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<DeviceAnalogRuleDto> GetAsync(InputIdDto input);

        /// <summary>
        /// 获取规则数据分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IPagedResult<DeviceAnalogRuleDto>> GetPageListAsync(GetDeviceAnalogRuleListInput input);

        /// <summary>
        /// 获取规则数据不分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IList<DeviceAnalogRuleDto>> GetFilterListAsync(GetDeviceAnalogRuleBaseInput input);

        /// <summary>
        /// 创建规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<DeviceAnalogRuleDto> CreateAsync(CreateUpdateDeviceAnalogRuleInput input);

        /// <summary>
        /// 更新规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<DeviceAnalogRuleDto> UpdateAsync(CreateUpdateDeviceAnalogRuleInput input);

        /// <summary>
        /// 创建/更新规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<DeviceAnalogRuleDto> CreateOrUpdateAsync(CreateUpdateDeviceAnalogRuleInput input);
        

        /// <summary>
        /// 根据Id删除规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteAsync(InputIdDto input);

        /// <summary>
        /// 规则启用禁用
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> SetEnabled(DeviceAnalogRuleEnabledInputDto input);

        /// <summary>
        /// 更新LastSendTime
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateLastSendTimeAsync(InputIdDto input);
    }
}