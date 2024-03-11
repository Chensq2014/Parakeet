using AutoMapper;
using Common.Dtos;
using Common.Entities;
using Volo.Abp.AutoMapper;

namespace Parakeet.Net.Sections
{
    /// <summary>
    /// 客户需求实体-dto映射
    /// </summary>
    public class SectionDtoMapper : Profile
    {
        public SectionDtoMapper()
        {
            CreateMap<Section, SectionDto>()
                //.Ignore(x => x.SectionWorkers)
                //.Ignore(x => x.Houses)
                .Ignore(x => x.CostTotal)
                .Ignore(x => x.ProfitTotal)
                .Ignore(x => x.HouseTotal)
                .Ignore(x => x.WorkerTotal);

            CreateMap<SectionDto, Section>()
                .Ignore(x => x.IsDeleted)
                .Ignore(x => x.DeleterId)
                .Ignore(x => x.DeletionTime)
                .Ignore(x => x.LastModifierId)
                .Ignore(x => x.LastModificationTime)
                .Ignore(x => x.CreatorId)
                .Ignore(x => x.CreationTime)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.LocationArea);

        }
    }
}
