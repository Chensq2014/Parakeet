using Common.Dtos;
using Common.Entities;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Permissions;
using System;
using System.Threading.Tasks;

namespace Parakeet.Net.Controllers
{
    [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Default)]
    public class DeviceAnalogRuleController : BaseEntityController<DeviceAnalogRule>
    {
        private readonly IAuthorizationService _authorizationService;
        //private readonly IDeviceAnalogRuleAppService _deviceAnalogRuleAppService;
        public DeviceAnalogRuleController(IDeviceAnalogRuleAppService baseService, IAuthorizationService authorizationService) : base(baseService)
        {
            _authorizationService = authorizationService;
            //_deviceAnalogRuleAppService = baseService;
        }

        [HttpGet]
        [Route("deviceAnalogRule-list")]
        [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Default)]
        public IActionResult Index()
        {
            ViewBag.AuthorizationService = _authorizationService;
            return View();
        }

        [HttpGet]
        [Route("deviceAnalogRule-dx-list")]
        [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Default)]
        public IActionResult DxIndex()
        {
            ViewBag.AuthorizationService = _authorizationService;
            return View();
        }

        [HttpGet]
        [Route("deviceAnalogRule-add")]
        [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Create)]
        public async Task<IActionResult> Add()
        {
            var dto = new DeviceAnalogRuleDto();
            return View(dto);
        }

        [HttpGet]
        [Route("deviceAnalogRule-edit/{id:guid}")]
        [Authorize(DeviceAnalogRulePermissions.DeviceAnalogRule.Update)]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await BaseService.GetByPrimaryKey(new InputIdDto { Id = id });
            return View("Add", ObjectMapper.Map<DeviceAnalogRule, DeviceAnalogRuleDto>(entity));
        }
    }
}
