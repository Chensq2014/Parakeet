using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace Parakeet.Net;

/* play 服务*/
public class PlayAppService : CustomerAppService
{
    private ICurrentTenant _currentTenant;
    protected PlayAppService(ICurrentTenant currentTenant)
    {
        _currentTenant = currentTenant;
    }

    /// <summary>
    /// 多租户测试
    /// </summary>
    /// <param name="__tenantId"></param>
    /// <returns></returns>
    public async Task<object> GetTenantId(string __tenantId)
    {
        return await Task.FromResult(_currentTenant);
    }
}
