using AutoMapper;
using Common.Dtos;
using Common.Entities;

namespace Parakeet.Net.AreaTenants
{
    public class AreaTenantsDtoMapper : Profile
    {
        public AreaTenantsDtoMapper()
        {
            CreateMap<AreaTenant, AreaTenantListDto>()
                .ForMember(m => m.LocationAreaName, 
                    opt => opt.MapFrom(n=>$"{n.LocationArea.Parent.Name}【{n.LocationArea.Name}】"));
            CreateMap<TenantDbConnectionString, TenantDbConnectionStringDto>()
                .ForMember(m => m.AreaTenantName, 
                    opt => opt.MapFrom(n=>$"{n.AreaTenant.DisplayName}【{n.AreaTenant.Name}】"));
        }
    }
}
