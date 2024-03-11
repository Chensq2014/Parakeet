using AutoMapper;
using Common.Dtos;
using Common.Entities;
using Volo.Abp.AutoMapper;

namespace Parakeet.Net.Thresholds
{
    public class ThresholdDtoMapperProfile : Profile
    {
        public ThresholdDtoMapperProfile()
        {
            CreateMap<Threshold, ThresholdDto>()
            .Ignore(x => x.Devices);
            CreateMap<ThresholdDto, Threshold>()
                .Ignore(x => x.IsDeleted)
                .Ignore(x => x.DeleterId)
                .Ignore(x => x.DeletionTime)
                .Ignore(x => x.LastModifierId)
                .Ignore(x => x.LastModificationTime)
                .Ignore(x => x.CreatorId)
                .Ignore(x => x.CreationTime)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Devices);
        }
    }
}
