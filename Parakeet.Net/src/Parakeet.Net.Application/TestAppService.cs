using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace Parakeet.Net;

/* test 服务*/
public class TestAppService : NetAppService
{
    private ICurrentTenant _currentTenant;
    protected TestAppService(ICurrentTenant currentTenant)
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
