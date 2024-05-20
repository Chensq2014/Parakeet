using AutoMapper;
using Common.Dtos;
using Common.Entities;

namespace Parakeet.Net.LocationAreas
{
    /// <summary>
    ///     区域DtoMapper
    /// </summary>
    public class LocationAreaDtoMapper : Profile
    {
        public LocationAreaDtoMapper()
        {
            CreateMap<LocationArea, LocationAreaDto>()
                .ForMember(m => m.ParentName, opt => opt.MapFrom(n => n.Parent.Name))
                //.ForMember(m => m.ParentCode, opt => opt.MapFrom(n => n.Parent.Code))//本来就有ParentCode字段
                .ForMember(m => m.Chidren, opt => opt.Ignore());
            //chidren数据量太大...
            //.ForMember(m => m.Chidren, opt => opt.MapFrom(n => n.Children.MapTo<List<LocationAreaDto>>()));
        }
    }
}