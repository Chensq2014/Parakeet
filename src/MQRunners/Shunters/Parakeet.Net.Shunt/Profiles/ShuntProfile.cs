using AutoMapper;
using Common.Dtos;
using Common.Entities;

namespace Parakeet.Net.Shunt.Profiles
{
    public class ShuntProfile : Profile
    {
        public ShuntProfile()
        {
            CreateMap<Device, DeviceDto>().ReverseMap();
            CreateMap<DeviceExtend, DeviceExtendDto>().ReverseMap();
            CreateMap<DeviceMediator, DeviceMediatorDto>().ReverseMap();
            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<Supplier, SupplierDto>().ReverseMap();
        }
    }
}
