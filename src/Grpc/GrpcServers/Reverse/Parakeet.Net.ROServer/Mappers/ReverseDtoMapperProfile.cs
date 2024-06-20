using AutoMapper;
using Common.Dtos;
using Common.Entities;

namespace Parakeet.Net.ROServer.Mappers
{
    public class ReverseDtoMapperProfile : Profile
    {
        public ReverseDtoMapperProfile()
        {
            CreateMap<Device, DeviceDto>();
        }
    }
}
