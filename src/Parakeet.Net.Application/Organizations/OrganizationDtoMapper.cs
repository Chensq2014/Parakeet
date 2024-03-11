using AutoMapper;
using Common.Dtos;
using Common.Entities;

namespace Parakeet.Net.Organizations
{
    public class OrganizationDtoMapper : Profile
    {
        public OrganizationDtoMapper()
        {
            CreateMap<Organization, OrganizationDto>()
                .ForMember(m => m.Users, opt => opt.Ignore())
                .ForMember(m => m.Children, opt => opt.Ignore());
            CreateMap<OrganizationUser, OrganizationUserDto>()
                .ForMember(m => m.User, opt => opt.Ignore());
        }
    }
}
