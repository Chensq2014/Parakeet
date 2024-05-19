using AutoMapper.QueryableExtensions;
using Common.Dtos;
using Common.Entities;
using Common.Enums;
using Common.Interfaces;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parakeet.Net.SecurePolicies
{
    /// <summary>
    /// 安全策略服务
    /// </summary>
    public class SecurePolicyAppService : BaseNetAppService<SecurePolicy>, ISecurePolicyAppService
    {
        public SecurePolicyAppService(IPortalRepository<SecurePolicy, Guid> baseRepository) : base(baseRepository)
        {
        }

        #region 重写父类 GridData

        /// <summary>
        /// 获取Get(扩展) 提供给devExtreme
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
        {
            var result = await DataSourceLoader.LoadAsync(await GridDto<SecurePolicyDto>(), loadOptions);
            return result;
        }


        #endregion


        /// <summary>
        /// 获取当前登录用户所有启用的安全策略
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<SecurePolicyDto>> GetCurrentUserPolicies(InputIdDto input)
        {
            //var userInfo = CurrentUser;//_organizationCacheService.GetUserInfoById(input.Id);
            //var roleIds = CurrentUser?.Roles;//userInfo.UserRoles.Select(m => (Guid?)m.RoleId).ToList();
            //var companyIds = userInfo.UserCompanys.Select(m => (Guid?)m.CompanyId).ToList();
            //var departmentIds = userInfo.UserDepartments.Select(m => (Guid?)m.DepId).ToList();
            input.Id = CurrentUser?.Id ?? input.Id;
            var policies = await (await Repository.GetQueryableAsync()).AsNoTracking()
                .Where(m => m.IsEnable)
                .Where(m => m.SecureSourceType == SecureSourceType.None
                            || m.SecureSourceId == input.Id && m.SecureSourceType == SecureSourceType.Role)
                .ProjectTo<SecurePolicyDto>(Configuration)
                .ToListAsync();
            return policies;
        }
    }
}
