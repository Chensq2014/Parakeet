//using AutoMapper.QueryableExtensions;
//using Common.Dtos;
//using Common.Entities;
//using Common.Interfaces;
//using DevExtreme.AspNet.Data;
//using DevExtreme.AspNet.Data.ResponseModel;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.EntityFrameworkCore;
//using Parakeet.Net.LinqExtensions;
//using Parakeet.Net.Permissions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Threading.Tasks;
//using Common.Extensions;
//using Volo.Abp;
//using Volo.Abp.Application.Dtos;
//using Volo.Abp.Domain.Repositories;

//namespace Parakeet.Net.DeviceAnalogRules
//{
//    /// <summary>
//    /// 设备规则服务
//    /// </summary>
//    //[Authorize]
//    public class DeviceAnalogRuleAppService : BaseParakeetAppService<DeviceAnalogRule>, IDeviceAnalogRuleAppService//CustomerAppService,
//    {
//        private readonly IParakeetRepository<DeviceAnalogRule> _deviceAnalogRuleRepository;
//        private readonly IAuthorizationService _authorizationService;

//        public DeviceAnalogRuleAppService(IParakeetRepository<DeviceAnalogRule> deviceAnalogRuleRepository,
//            IAuthorizationService authorizationService) : base(deviceAnalogRuleRepository)
//        {
//            _deviceAnalogRuleRepository = deviceAnalogRuleRepository;
//            _authorizationService = authorizationService;
//        }

//        //public DeviceAnalogRuleAppService(IRepository<DeviceAnalogRule, Guid> deviceAnalogRuleRepository,
//        //    IAuthorizationService authorizationService)
//        //{
//        //    _deviceAnalogRuleRepository = deviceAnalogRuleRepository;
//        //    _authorizationService = authorizationService;
//        //}

//        #region 数据模拟DxGrid

//        #region 重写父类 GridData

//        /// <summary>
//        /// 获取Get(扩展) 提供给devExtreme
//        /// </summary>
//        /// <param name="loadOptions"></param>
//        /// <returns></returns>
//        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
//        {
//            var result = await DataSourceLoader.LoadAsync(await GridDto<DeviceAnalogRuleDto>(), loadOptions);
//            return result;
//        }

//        #endregion


//        #region 设备数据规则分页管理

//        /// <summary>
//        /// 获取陕设备所有数据规则分页集合
//        /// </summary>
//        /// <param name="loadOptions"></param>
//        /// <returns></returns>
//        public async Task<LoadResult> GetDeviceRulesByDeviceId(DataSourceLoadOptionsBase loadOptions)
//        {
//            var id = GetRequestPrimarykey();
//            var query = (await _deviceAnalogRuleRepository.GetQueryableAsync()).AsNoTracking()
//                .Where(m => m.DeviceId == id)
//                .OrderByDescending(o => o.CreationTime)
//                .ProjectTo<DeviceAnalogRuleDto>(Configuration);
//            return await DataSourceLoader.LoadAsync(query, loadOptions);
//        }

//        #endregion

//        #endregion

//        #region IDeviceAnalogRuleAppService接口


//        /// <summary>
//        /// 根据Id获取规则
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Default)]
//        public async Task<DeviceAnalogRuleDto> GetAsync(InputIdDto input)
//        {
//            var entity = await (await _deviceAnalogRuleRepository.GetQueryableAsync()).AsNoTracking()
//                .Include(x => x.Device)
//                .FirstOrDefaultAsync(m=>m.Id==input.Id);
//            return ObjectMapper.Map<DeviceAnalogRule, DeviceAnalogRuleDto>(entity);
//        }

//        /// <summary>
//        /// 获取规则数据分页
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Default)]
//        public async Task<IPagedResult<DeviceAnalogRuleDto>> GetPageListAsync(GetDeviceAnalogRuleListInput input)
//        {
//            return await (await _deviceAnalogRuleRepository.GetQueryableAsync()).AsNoTracking()
//                .WhereIf(input.DeviceId.HasValue, m => m.DeviceId == input.DeviceId)
//                .WhereIf(input.ProjectId.HasValue, m => m.Device.ProjectId == input.ProjectId)
//                .WhereIf(input.Area.HasValue(), m => m.Device.Area == input.Area)
//                .WhereIf(input.IsEnabled.HasValue, m => m.IsEnabled == input.IsEnabled)
//                .WhereIf(input.Filter.HasValue(), m => m.Device.Name.Contains(input.Filter) || m.Device.FakeNo.Contains(input.Filter) || m.Remark.Contains(input.Filter))
//                .OrderBy(input.Sorting)
//                .ProjectTo<DeviceAnalogRuleDto>(Configuration)
//                .ToPageResultAsync(input,input.FindTotalCount);
//        }

//        /// <summary>
//        /// 获取规则数据不分页
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        //[Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Default)] //自动任务会调用 暂移除权限
//        public async Task<IList<DeviceAnalogRuleDto>> GetFilterListAsync(GetDeviceAnalogRuleBaseInput input)
//        {
//            return await (await _deviceAnalogRuleRepository.GetQueryableAsync()).AsNoTracking()
//                .WhereIf(input.Filter.HasValue(), m => m.Device.Name.Contains(input.Filter) || m.Device.FakeNo.Contains(input.Filter) || m.Remark.Contains(input.Filter))
//                .WhereIf(input.DeviceId.HasValue, m => m.DeviceId == input.DeviceId)
//                .WhereIf(input.ProjectId.HasValue, m => m.Device.ProjectId == input.ProjectId)
//                .WhereIf(input.Area.HasValue(), m => m.Device.Area == input.Area)
//                .WhereIf(input.IsEnabled.HasValue, m => m.IsEnabled == input.IsEnabled)
//                .OrderBy(input.Sorting)
//                .ProjectTo<DeviceAnalogRuleDto>(Configuration)
//                .ToListAsync();
//        }


//        /// <summary>
//        /// 创建规则
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Create)]
//        public async Task<DeviceAnalogRuleDto> CreateAsync(CreateUpdateDeviceAnalogRuleInput input)
//        {
//            input.Id = GuidGenerator.Create();
//            input.LastSendTime = input.LastSendTime.ToUniversalTime();
//            var rule = ObjectMapper.Map<CreateUpdateDeviceAnalogRuleInput, DeviceAnalogRule>(input);
//            var exist = await _deviceAnalogRuleRepository.AnyAsync(m => m.DeviceId == input.DeviceId);
//            if (exist)
//            {
//                throw new UserFriendlyException($"{input.DeviceId}设备已存在模拟数据规则");
//            }
//            await _deviceAnalogRuleRepository.InsertAsync(rule);
//            return ObjectMapper.Map<DeviceAnalogRule, DeviceAnalogRuleDto>(rule);
//        }


//        /// <summary>
//        /// 更新规则
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Update)]
//        public async Task<DeviceAnalogRuleDto> UpdateAsync(CreateUpdateDeviceAnalogRuleInput input)
//        {
//            input.LastSendTime = input.LastSendTime.ToUniversalTime();
//            var entity = ObjectMapper.Map<CreateUpdateDeviceAnalogRuleInput, DeviceAnalogRule>(input);
//            await _deviceAnalogRuleRepository.UpdateAsync(entity);
//            return ObjectMapper.Map<DeviceAnalogRule, DeviceAnalogRuleDto>(entity);
//        }

//        /// <summary>
//        /// 创建/更新规则
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Default)]
//        public async Task<DeviceAnalogRuleDto> CreateOrUpdateAsync(CreateUpdateDeviceAnalogRuleInput input)
//        {
//            var isGranted = await _authorizationService.IsGrantedAsync(DeviceAnalogRulePermissions.DeviceAnalogRule.Create)
//                           || await _authorizationService.IsGrantedAsync(DeviceAnalogRulePermissions.DeviceAnalogRule.Update);
//            if (!isGranted)
//            {
//                throw new UnauthorizedAccessException("没有权限");
//                //throw new UserFriendlyException("没有权限");
//            }
//            if (input.Id == Guid.Empty)
//            {
//                return await CreateAsync(input);
//            }
//            return await UpdateAsync(input);
//        }

//        /// <summary>
//        /// 根据Id删除规则
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Delete)]
//        public async Task DeleteAsync(InputIdDto input)
//        {
//            await _deviceAnalogRuleRepository.DeleteAsync(input.Id);
//        }

//        /// <summary>
//        /// 规则启用禁用
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Update)]
//        public async Task<bool> SetEnabled(DeviceAnalogRuleEnabledInputDto input)
//        {
//            var rule = await _deviceAnalogRuleRepository.GetAsync(input.Id);
//            rule.IsEnabled = input.IsEnabled;
//            return await Task.FromResult(rule.IsEnabled);
//        }


//        /// <summary>
//        /// 更新LastSendTime
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task UpdateLastSendTimeAsync(InputIdDto input)
//        {
//            var entity = await _deviceAnalogRuleRepository.GetAsync(input.Id);
//            entity.LastSendTime = DateTime.Now.ToUniversalTime();
//        }


//        #endregion

//    }
//}
