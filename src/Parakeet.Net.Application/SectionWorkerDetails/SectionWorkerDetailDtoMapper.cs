using AutoMapper;
using Common.Dtos;
using Common.Entities;
using Volo.Abp.AutoMapper;

namespace Parakeet.Net.SectionWorkers
{
    /// <summary>
    /// 客户需求实体-dto映射
    /// </summary>
    public class SectionWorkerDetailDtoMapper : Profile
    {
        public SectionWorkerDetailDtoMapper()
        {
            CreateMap<SectionWorkerDetail, SectionWorkerDetailDto>()
                .Ignore(x => x.CostTotal)
                .Ignore(x => x.ProfitTotal)
                .Ignore(x => x.Total);

            CreateMap<SectionWorkerDetailDto, SectionWorkerDetail>()
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
