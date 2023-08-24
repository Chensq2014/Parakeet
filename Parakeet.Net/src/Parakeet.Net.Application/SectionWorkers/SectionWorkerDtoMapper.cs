using AutoMapper;
using Parakeet.Net.Dtos;
using Parakeet.Net.Entities;
using Volo.Abp.AutoMapper;

namespace Parakeet.Net.SectionWorkers
{
    /// <summary>
    /// 客户需求实体-dto映射
    /// </summary>
    public class SectionWorkerDtoMapper : Profile
    {
        public SectionWorkerDtoMapper()
        {
            CreateMap<SectionWorker, SectionWorkerDto>()
                .Ignore(x => x.SectionWorkerDetails)
                .Ignore(x => x.CostTotal)
                .Ignore(x => x.ProfitTotal)
                .Ignore(x => x.Total);

            CreateMap<SectionWorkerDto, SectionWorker>();

        }
    }
}
