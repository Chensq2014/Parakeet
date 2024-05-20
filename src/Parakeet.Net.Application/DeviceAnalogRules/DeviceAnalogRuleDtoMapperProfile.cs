using AutoMapper;
using Common.Dtos;
using Common.Entities;
using Volo.Abp.AutoMapper;

namespace Parakeet.Net.DeviceAnalogRules
{
    public class DeviceAnalogRuleDtoMapperProfile : Profile
    {
        public DeviceAnalogRuleDtoMapperProfile()
        {
            CreateMap<DeviceAnalogRule, DeviceAnalogRuleDto>()
                .ForMember(m => m.DeviceName, n => n.MapFrom(o => o.Device.Name))
                .ForMember(m => m.FakeNo, n => n.MapFrom(o => o.Device.FakeNo));
            CreateMap<DeviceAnalogRuleDto, DeviceAnalogRule>()
                .Ignore(x => x.IsDeleted)
                .Ignore(x => x.DeleterId)
                .Ignore(x => x.DeletionTime)
                .Ignore(x => x.LastModifierId)
                .Ignore(x => x.LastModificationTime)
                .Ignore(x => x.CreatorId)
                .Ignore(x => x.CreationTime)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp);
            CreateMap<CreateUpdateDeviceAnalogRuleInput, DeviceAnalogRule>()
                .Ignore(x => x.IsDeleted)
                .Ignore(x => x.DeleterId)
                .Ignore(x => x.DeletionTime)
                .Ignore(x => x.LastModifierId)
                .Ignore(x => x.LastModificationTime)
                .Ignore(x => x.CreatorId)
                .Ignore(x => x.CreationTime)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp);
        }
    }
}
