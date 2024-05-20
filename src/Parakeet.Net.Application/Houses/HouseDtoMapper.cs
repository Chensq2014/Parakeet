using AutoMapper;
using Common.Dtos;
using Common.Entities;
using Volo.Abp.AutoMapper;

namespace Parakeet.Net.Houses
{
    /// <summary>
    /// 客户需求实体-dto映射
    /// </summary>
    public class HouseDtoMapper : Profile
    {
        public HouseDtoMapper()
        {
            CreateMap<House, HouseDto>()
                .Ignore(x => x.Total);
            CreateMap<HouseDto, House>()
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
