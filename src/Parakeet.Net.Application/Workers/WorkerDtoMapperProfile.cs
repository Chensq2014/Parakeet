using AutoMapper;
using Common.Dtos;
using Common.Entities;
using Volo.Abp.AutoMapper;

namespace Parakeet.Net.Workers
{
    public class WorkerDtoMapperProfile : Profile
    {
        public WorkerDtoMapperProfile()
        {
            CreateMap<Worker, WorkerDto>();
                //.Ignore(x => x.Devices);
            CreateMap<WorkerDto, Worker>()
                .Ignore(x => x.IsDeleted)
                .Ignore(x => x.DeleterId)
                .Ignore(x => x.DeletionTime)
                .Ignore(x => x.LastModifierId)
                .Ignore(x => x.LastModificationTime)
                .Ignore(x => x.CreatorId)
                .Ignore(x => x.CreationTime)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.DeviceWorkers)
                .Ignore(x => x.SectionWorkers);
        }
    }
}
