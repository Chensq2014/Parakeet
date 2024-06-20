using AutoMapper;
using Common.Dtos;
using Common.Entities;

namespace Parakeet.Net.Register.Mappers
{
    /// <summary>
    /// 注册模块dto映射 Profile
    /// </summary>
    public class RegisterMapperProfile : Profile
    {
        public RegisterMapperProfile()
        {
            CreateMap<Device, DeviceDto>();
            CreateMap<WorkerDto, Worker>();
            CreateMap<DeviceWorkerDto, DeviceWorker>();
            //.Ignore(m=>m.Worker);

        }
    }
}
