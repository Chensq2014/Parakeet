using AutoMapper;
using Common.Dtos;
using Common.Entities;
using Volo.Abp.AutoMapper;

namespace Parakeet.Net.Products
{
    /// <summary>
    /// 客户需求实体-dto映射
    /// </summary>
    public class ProductDtoMapper : Profile
    {
        public ProductDtoMapper()
        {
            CreateMap<Product, ProductDto>()
                .Ignore(x => x.Total);
            CreateMap<Product, ProductListDto>()
                .ForMember(m => m.HouseNumber, n => n.MapFrom(o => o.House.Number))
                .ForMember(m => m.SectionName, n => n.MapFrom(o => o.House.Section.Name))
                .ForMember(m => m.SectionId, n => n.MapFrom(o => o.House.SectionId))
                .Ignore(x => x.ChargeTypeDisplayName)
                .Ignore(x => x.Total);
            CreateMap<ProductDto, Product>()
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
