using Common.Dtos;
using Common.Entities;
using Common.Interfaces;
using Common.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Parakeet.Net.Controllers
{
    [Route("api/[controller]/[action]")]
    public class LocationAreaController : NetController
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocationAreaAppService _locationAreaAppService;
        private readonly IRepository<AppUser> _userRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<OrganizationUser> _organizationUserRepository;
        public LocationAreaController(IServiceProvider serviceProvider,
            ILocationAreaAppService locationAreaAppService,
            IRepository<AppUser> userRepository,
            IHttpContextAccessor httpContextAccessor,
            IRepository<OrganizationUser> organizationUserRepository,
            IRepository<Organization> organizationRepository)
        {
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _organizationUserRepository = organizationUserRepository;
            _organizationRepository = organizationRepository;
            _locationAreaAppService = locationAreaAppService;
        }

        [HttpGet]
        public async Task Init()
        {
            #region 证实IRepository<AppUser, Guid> _userRepository 注入
            var user = await _userRepository.FirstOrDefaultAsync();
            //var organization = new Organization
            //{
            //    Name = "test",
            //    Code = "test",
            //    OrganizationType = OrganizationType.City,
            //    OrganizationUsers = new HashSet<OrganizationUser>
            //    {
            //        new OrganizationUser
            //        {
            //            UserId = user?.Id
            //        }
            //    }
            //};
            //await _organizationRepository.InsertAsync(organization);

            var organizationUser = await _organizationUserRepository.FirstOrDefaultAsync();
            //Logger.LogInformation($"{user?.Name}_{user?.Sex.ToString()}_{organizationUser?.User?.Name} _userRepository 注入成功!");
            #endregion

            #region 从Header字典中获取数据
            var dic = _httpContextAccessor.HttpContext.Request.Headers;

            if (dic.ContainsKey("ProjectId"))
            {
                var projectId = dic["ProjectId"];//StringValues
                Log.Logger.Information($"FirstOrDefault:{projectId.FirstOrDefault()}");
            }

            #endregion

            await Task.CompletedTask;
        }

        /// <summary>
        /// 根据导入的模板数据生成结构树
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[Route("ImportFiles")]
        public async Task ImportFiles(ImportFileDto input)
        {
            await _locationAreaAppService.ImportFromExcel(input);
        }
    }
}
